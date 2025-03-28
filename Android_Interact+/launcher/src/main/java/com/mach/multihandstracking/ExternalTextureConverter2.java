package com.mach.multihandstracking;

import android.graphics.SurfaceTexture;
import android.graphics.SurfaceTexture.OnFrameAvailableListener;
import android.opengl.GLES20;
import android.util.Log;

import com.google.mediapipe.components.TextureFrameConsumer;
import com.google.mediapipe.components.TextureFrameProducer;
import com.google.mediapipe.framework.AppTextureFrame;
import com.google.mediapipe.glutil.GlThread;
import com.google.mediapipe.glutil.ShaderUtil;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;
import java.util.List;

import javax.microedition.khronos.egl.EGLContext;

/**
 * Google Mediapipe internal class.
 * Had to be copied to make the app work.
 */
public class ExternalTextureConverter2 implements TextureFrameProducer {
    private static final String TAG = "ExternalTextureConv";
    private static final int DEFAULT_NUM_BUFFERS = 2;
    private static final String THREAD_NAME = "ExternalTextureConverter";
    private RenderThread2 thread;

    public ExternalTextureConverter2(EGLContext parentContext, int numBuffers) {
        this.thread = new RenderThread2(parentContext, numBuffers);
        this.thread.setName("ExternalTextureConverter");
        this.thread.start();

        try {
            this.thread.waitUntilReady();
        } catch (InterruptedException var4) {
            Thread.currentThread().interrupt();
            Log.e("ExternalTextureConv", "thread was unexpectedly interrupted: " + var4.getMessage());
            throw new RuntimeException(var4);
        }
    }

    public void setFlipY(boolean flip) {
        this.thread.setFlipY(flip);
    }

    public ExternalTextureConverter2(EGLContext parentContext) {
        this(parentContext, 2);
    }

    public ExternalTextureConverter2(EGLContext parentContext, SurfaceTexture texture, int targetWidth, int targetHeight) {
        this(parentContext);
        this.thread.setSurfaceTexture(texture, targetWidth, targetHeight);
    }

    public void setSurfaceTexture(SurfaceTexture texture, int width, int height) {
        if (texture == null || width != 0 && height != 0) {
            this.thread.getHandler().post(() -> {
                this.thread.setSurfaceTexture(texture, width, height);
            });
        } else {
            throw new RuntimeException("ExternalTextureConverter: setSurfaceTexture dimensions cannot be zero");
        }
    }

    public void setSurfaceTextureAndAttachToGLContext(SurfaceTexture texture, int width, int height) {
        if (texture == null || width != 0 && height != 0) {
            this.thread.getHandler().post(() -> {
                this.thread.setSurfaceTextureAndAttachToGLContext(texture, width, height);
            });
        } else {
            throw new RuntimeException("ExternalTextureConverter: setSurfaceTexture dimensions cannot be zero");
        }
    }

    public void setConsumer(TextureFrameConsumer next) {
        this.thread.setConsumer(next);
    }

    public void addConsumer(TextureFrameConsumer consumer) {
        this.thread.addConsumer(consumer);
    }

    public void removeConsumer(TextureFrameConsumer consumer) {
        this.thread.removeConsumer(consumer);
    }

    public void close() {
        if (this.thread != null) {
            this.thread.getHandler().post(() -> {
                this.thread.setSurfaceTexture((SurfaceTexture) null, 0, 0);
            });
            this.thread.quitSafely();

            try {
                this.thread.join();
            } catch (InterruptedException var2) {
                Thread.currentThread().interrupt();
                Log.e("ExternalTextureConv", "thread was unexpectedly interrupted: " + var2.getMessage());
                throw new RuntimeException(var2);
            }
        }
    }

    private static class RenderThread2 extends GlThread implements OnFrameAvailableListener {
        private static final long NANOS_PER_MICRO = 1000L;
        private volatile SurfaceTexture surfaceTexture = null;
        private final List<TextureFrameConsumer> consumers;
        private List<AppTextureFrame> outputFrames = null;
        private int outputFrameIndex = -1;
        private ExternalTextureRenderer2 renderer = null;
        private long timestampOffset = 0L;
        private long previousTimestamp = 0L;
        private boolean previousTimestampValid = false;
        protected int destinationWidth = 0;
        protected int destinationHeight = 0;

        public RenderThread2(EGLContext parentContext, int numBuffers) {
            super(parentContext);
            this.outputFrames = new ArrayList();
            this.outputFrames.addAll(Collections.nCopies(numBuffers, (AppTextureFrame) null));
            this.renderer = new ExternalTextureRenderer2();
            this.consumers = new ArrayList();
        }

        public void setFlipY(boolean flip) {
            this.renderer.setFlipY(flip);
        }

        public void setSurfaceTexture(SurfaceTexture texture, int width, int height) {
            if (this.surfaceTexture != null) {
                this.surfaceTexture.setOnFrameAvailableListener((OnFrameAvailableListener) null);
            }

            this.surfaceTexture = texture;
            if (this.surfaceTexture != null) {
                this.surfaceTexture.setOnFrameAvailableListener(this);
            }

            this.destinationWidth = width;
            this.destinationHeight = height;
        }

        public void setSurfaceTextureAndAttachToGLContext(SurfaceTexture texture, int width, int height) {
            this.setSurfaceTexture(texture, width, height);
            int[] textures = new int[1];
            GLES20.glGenTextures(1, textures, 0);
            this.surfaceTexture.attachToGLContext(textures[0]);
        }

        public void setConsumer(TextureFrameConsumer consumer) {
            synchronized (this.consumers) {
                this.consumers.clear();
                this.consumers.add(consumer);
            }
        }

        public void addConsumer(TextureFrameConsumer consumer) {
            synchronized (this.consumers) {
                this.consumers.add(consumer);
            }
        }

        public void removeConsumer(TextureFrameConsumer consumer) {
            synchronized (this.consumers) {
                this.consumers.remove(consumer);
            }
        }

        public void onFrameAvailable(SurfaceTexture surfaceTexture) {
            this.handler.post(() -> {
                this.renderNext(surfaceTexture);
            });
        }

        public void prepareGl() {
            super.prepareGl();
            GLES20.glClearColor(0.0F, 0.0F, 0.0F, 1.0F);
            this.renderer.setup();
        }

        public void releaseGl() {
            for (int i = 0; i < this.outputFrames.size(); ++i) {
                this.teardownDestination(i);
            }

            this.renderer.release();
            super.releaseGl();
        }

        protected void renderNext(SurfaceTexture fromTexture) {
            if (fromTexture == this.surfaceTexture) {
                synchronized (this.consumers) {
                    boolean frameUpdated = false;
                    Iterator var4 = this.consumers.iterator();

                    while (var4.hasNext()) {
                        TextureFrameConsumer consumer = (TextureFrameConsumer) var4.next();
                        AppTextureFrame outputFrame = this.nextOutputFrame();
                        this.updateOutputFrame(outputFrame);
                        frameUpdated = true;
                        if (consumer != null) {
                            if (Log.isLoggable("ExternalTextureConv", 2)) {
                                Log.v("ExternalTextureConv", String.format("Locking tex: %d width: %d height: %d", outputFrame.getTextureName(), outputFrame.getWidth(), outputFrame.getHeight()));
                            }

                            outputFrame.setInUse();
                            consumer.onNewFrame(outputFrame);
                        }
                    }

                    if (!frameUpdated) {
                        AppTextureFrame outputFrame = this.nextOutputFrame();
                        this.updateOutputFrame(outputFrame);
                    }

                }
            }
        }

        private void teardownDestination(int index) {
            if (this.outputFrames.get(index) != null) {
                this.waitUntilReleased((AppTextureFrame) this.outputFrames.get(index));
                GLES20.glDeleteTextures(1, new int[]{((AppTextureFrame) this.outputFrames.get(index)).getTextureName()}, 0);
                this.outputFrames.set(index, (AppTextureFrame) null);
            }

        }

        private void setupDestination(int index) {
            this.teardownDestination(index);
            int destinationTextureId = ShaderUtil.createRgbaTexture(this.destinationWidth, this.destinationHeight);
            Log.d("ExternalTextureConv", String.format("Created output texture: %d width: %d height: %d", destinationTextureId, this.destinationWidth, this.destinationHeight));
            this.bindFramebuffer(destinationTextureId, this.destinationWidth, this.destinationHeight);
            this.outputFrames.set(index, new AppTextureFrame(destinationTextureId, this.destinationWidth, this.destinationHeight));
        }

        private AppTextureFrame nextOutputFrame() {
            this.outputFrameIndex = (this.outputFrameIndex + 1) % this.outputFrames.size();
            AppTextureFrame outputFrame = (AppTextureFrame) this.outputFrames.get(this.outputFrameIndex);
            if (outputFrame == null || outputFrame.getWidth() != this.destinationWidth || outputFrame.getHeight() != this.destinationHeight) {
                this.setupDestination(this.outputFrameIndex);
                outputFrame = (AppTextureFrame) this.outputFrames.get(this.outputFrameIndex);
            }

            this.waitUntilReleased(outputFrame);
            return outputFrame;
        }

        private void updateOutputFrame(AppTextureFrame outputFrame) {
            this.bindFramebuffer(outputFrame.getTextureName(), this.destinationWidth, this.destinationHeight);
            this.renderer.render(this.surfaceTexture);
            long textureTimestamp = this.surfaceTexture.getTimestamp() / 1000L;
            if (this.previousTimestampValid && textureTimestamp + this.timestampOffset <= this.previousTimestamp) {
                this.timestampOffset = this.previousTimestamp + 1L - textureTimestamp;
            }

            outputFrame.setTimestamp(textureTimestamp + this.timestampOffset);
            this.previousTimestamp = outputFrame.getTimestamp();
            this.previousTimestampValid = true;
        }

        private void waitUntilReleased(AppTextureFrame frame) {
            try {
                if (Log.isLoggable("ExternalTextureConv", 2)) {
                    Log.v("ExternalTextureConv", String.format("Waiting for tex: %d width: %d height: %d", frame.getTextureName(), frame.getWidth(), frame.getHeight()));
                }

                frame.waitUntilReleased();
                if (Log.isLoggable("ExternalTextureConv", 2)) {
                    Log.v("ExternalTextureConv", String.format("Finished waiting for tex: %d width: %d height: %d", frame.getTextureName(), frame.getWidth(), frame.getHeight()));
                }

            } catch (InterruptedException var3) {
                Thread.currentThread().interrupt();
                Log.e("ExternalTextureConv", "thread was unexpectedly interrupted: " + var3.getMessage());
                throw new RuntimeException(var3);
            }
        }
    }
}
