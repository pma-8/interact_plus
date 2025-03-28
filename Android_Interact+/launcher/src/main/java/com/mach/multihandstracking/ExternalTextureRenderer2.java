package com.mach.multihandstracking;

import android.graphics.SurfaceTexture;
import android.opengl.GLES20;

import com.google.mediapipe.glutil.CommonShaders;
import com.google.mediapipe.glutil.ShaderUtil;

import java.nio.FloatBuffer;
import java.util.HashMap;
import java.util.Map;

/**
 * Google Mediapipe internal class. Only the FLIPPED_LANDSCAPE_TEXTURE_VERTICES was added.
 */
public class ExternalTextureRenderer2 {
    private static final FloatBuffer TEXTURE_VERTICES = ShaderUtil.floatBuffer(new float[]{0.0F, 0.0F, 1.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F});
    private static final FloatBuffer FLIPPED_TEXTURE_VERTICES = ShaderUtil.floatBuffer(new float[]{0.0F, 1.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F});

    // Customized float array to allow landscape preview of the mediapipe hand detection framework.
    private static final FloatBuffer FLIPPED_LANDSCAPE_TEXTURE_VERTICES = ShaderUtil.floatBuffer(new float[]{1.0F, 1.0F, 1.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F});

    private static final String TAG = "ExternalTextureRend";
    private static final int ATTRIB_POSITION = 1;
    private static final int ATTRIB_TEXTURE_COORDINATE = 2;
    private int program = 0;
    private int frameUniform;
    private int textureTransformUniform;
    private float[] textureTransformMatrix = new float[16];
    private boolean flipY;

    public ExternalTextureRenderer2() {
    }

    public void setup() {
        Map<String, Integer> attributeLocations = new HashMap();
        attributeLocations.put("position", 1);
        attributeLocations.put("texture_coordinate", 2);
        this.program = ShaderUtil.createProgram("uniform mat4 texture_transform;\nattribute vec4 position;\nattribute mediump vec4 texture_coordinate;\nvarying mediump vec2 sample_coordinate;\n\nvoid main() {\n  gl_Position = position;\n  sample_coordinate = (texture_transform * texture_coordinate).xy;\n}", "#extension GL_OES_EGL_image_external : require\nvarying mediump vec2 sample_coordinate;\nuniform samplerExternalOES video_frame;\n\nvoid main() {\n  gl_FragColor = texture2D(video_frame, sample_coordinate);\n}", attributeLocations);
        this.frameUniform = GLES20.glGetUniformLocation(this.program, "video_frame");
        this.textureTransformUniform = GLES20.glGetUniformLocation(this.program, "texture_transform");
        ShaderUtil.checkGlError("glGetUniformLocation");
    }

    public void setFlipY(boolean flip) {
        this.flipY = flip;
    }

    public void render(SurfaceTexture surfaceTexture) {
        GLES20.glClear(16384);
        GLES20.glActiveTexture(33984);
        ShaderUtil.checkGlError("glActiveTexture");
        surfaceTexture.updateTexImage();
        surfaceTexture.getTransformMatrix(this.textureTransformMatrix);
        GLES20.glTexParameteri(36197, 10241, 9729);
        GLES20.glTexParameteri(36197, 10240, 9729);
        GLES20.glTexParameteri(36197, 10242, 33071);
        GLES20.glTexParameteri(36197, 10243, 33071);
        ShaderUtil.checkGlError("glTexParameteri");
        GLES20.glUseProgram(this.program);
        ShaderUtil.checkGlError("glUseProgram");
        GLES20.glUniform1i(this.frameUniform, 0);
        ShaderUtil.checkGlError("glUniform1i");
        GLES20.glUniformMatrix4fv(this.textureTransformUniform, 1, false, this.textureTransformMatrix, 0);
        ShaderUtil.checkGlError("glUniformMatrix4fv");
        GLES20.glEnableVertexAttribArray(1);
        GLES20.glVertexAttribPointer(1, 2, 5126, false, 0, CommonShaders.SQUARE_VERTICES);
        GLES20.glEnableVertexAttribArray(2);
        GLES20.glVertexAttribPointer(2, 2, 5126, false, 0, this.flipY ? FLIPPED_LANDSCAPE_TEXTURE_VERTICES : TEXTURE_VERTICES);
        ShaderUtil.checkGlError("program setup");
        GLES20.glDrawArrays(5, 0, 4);
        ShaderUtil.checkGlError("glDrawArrays");
        GLES20.glBindTexture(36197, 0);
        ShaderUtil.checkGlError("glBindTexture");
        GLES20.glFinish();
    }

    public void release() {
        GLES20.glDeleteProgram(this.program);
    }

    private float[] generateVector() {

        float[][] allBits = {
        };

        return new float[]{0, 0, 0, 0, 0, 0, 0, 0};
    }
}
