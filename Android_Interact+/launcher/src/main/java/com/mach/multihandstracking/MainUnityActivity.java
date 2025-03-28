package com.mach.multihandstracking;

import android.content.Intent;
import android.graphics.SurfaceTexture;
import android.os.Bundle;
import android.util.Size;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;

import android.widget.Button;
import android.widget.FrameLayout;

import com.Mach.CardBoardVR.R;
import com.google.mediapipe.components.CameraHelper;
import com.google.mediapipe.components.CameraXPreviewHelper;
import com.google.mediapipe.components.FrameProcessor;
import com.google.mediapipe.components.PermissionHelper;
import com.google.mediapipe.formats.proto.LandmarkProto;
import com.google.mediapipe.framework.AndroidAssetUtil;
import com.google.mediapipe.framework.PacketGetter;
import com.google.mediapipe.glutil.EglManager;
import com.mach.product.OverrideUnityActivity;
import com.unity3d.player.UnityPlayer;

import java.util.List;

/**
 * Handles the Android and Google Mediapipe Activity.
 */
public class MainUnityActivity extends OverrideUnityActivity {

    private static final String TAG = "MainActivity";
    private static final String BINARY_GRAPH_NAME = "multihandtrackinggpu.binarypb";
    private static final String INPUT_VIDEO_STREAM_NAME = "input_video";
    private static final String OUTPUT_VIDEO_STREAM_NAME = "output_video";
    private static final String OUTPUT_LANDMARKS_STREAM_NAME = "multi_hand_landmarks";
    private static final CameraHelper.CameraFacing CAMERA_FACING = CameraHelper.CameraFacing.BACK;

    // Flips the camera-preview frames vertically before sending them into FrameProcessor to be
    // processed in a MediaPipe graph, and flips the processed frames back when they are displayed.
    // This is needed because OpenGL represents images assuming the image origin is at the bottom-left
    // corner, whereas MediaPipe in general assumes the image origin is at top-left.
    private static final boolean FLIP_FRAMES_VERTICALLY = true;

    static {
        // Load all native libraries needed by the app.
        System.loadLibrary("mediapipe_jni");
        System.loadLibrary("opencv_java3");
    }

    // {@link SurfaceTexture} where the camera-preview frames can be accessed.
    private SurfaceTexture previewFrameTexture;
    // {@link SurfaceView} that displays the camera-preview frames processed by a MediaPipe graph.
    private SurfaceView previewDisplayView;
    // Creates and manages an {@link EGLContext}.
    private EglManager eglManager;
    // Sends camera-preview frames into a MediaPipe graph for processing, and displays the processed
    // frames onto a {@link Surface}.
    private FrameProcessor processor;
    // Converts the GL_TEXTURE_EXTERNAL_OES texture from Android camera into a regular texture to be
    // consumed by {@link FrameProcessor} and the underlying MediaPipe graph.
    private ExternalTextureConverter2 converter;
    // Handles camera access via the {@link CameraX} Jetpack support library.
    private CameraXPreviewHelper cameraHelper;
    //Setup activity layout
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.tracking_layout);
        mUnityPlayer = new UnityPlayer(this);
        FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(
                FrameLayout.LayoutParams.MATCH_PARENT,
                FrameLayout.LayoutParams.MATCH_PARENT);
        FrameLayout unityLayout = findViewById(R.id.unityLayout);
        unityLayout.addView(mUnityPlayer.getView(), 0, lp);

        // Sets up the preview for the Google Mediapipe framework. The images of the camera with
        // the output of the Google Mediapipe framework can be seen.
        previewDisplayView = new SurfaceView(this);
        setupPreviewDisplayView();

        // Initialize asset manager so that MediaPipe native libraries can access the app assets, e.g.,
        // binary graphs.
        AndroidAssetUtil.initializeNativeAssetManager(this);
        eglManager = new EglManager(null);

        processor =
                new FrameProcessor(
                        this,
                        eglManager.getNativeContext(),
                        BINARY_GRAPH_NAME,
                        INPUT_VIDEO_STREAM_NAME,
                        OUTPUT_VIDEO_STREAM_NAME);
        processor.getVideoSurfaceOutput().setFlipY(FLIP_FRAMES_VERTICALLY);
        processor.addPacketCallback(
                OUTPUT_LANDMARKS_STREAM_NAME,
                (packet) -> {

                    //List of normalized landmark coordinates
                    List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks =
                            PacketGetter.getProtoVector(packet, LandmarkProto.NormalizedLandmarkList.parser());

                    //Interprets the landmark coordinates and runs functions accordingly.
                    LandmarkInterpreter.gestureInterpreter(multiHandLandmarks);

                    //Send the unity component which gesture is currently recognized
                    UnityPlayer.UnitySendMessage("GestureText", "ShowGesture", LandmarkInterpreter.handGestureRecognition(multiHandLandmarks));

                    //Send the unity component if the hand is detected or not on camera
                    UnityPlayer.UnitySendMessage("HandLamp", "HandRecognized", LandmarkInterpreter.handRecognition(multiHandLandmarks).toString());
                });
        PermissionHelper.checkAndRequestCameraPermissions(this);
    }

    //Starts the camera preview on the button press
    public void btnStartCamera(View v) {
        if (PermissionHelper.cameraPermissionsGranted(this)) {
            startCamera();
        }
    }

    //Changes the gesture interpreter to allow for right- and left-hand usage.
    public void btnChangeHand(View v){
        final Button handButton = findViewById(R.id.handButton);

        if(LandmarkInterpreter.hand_right){
            LandmarkInterpreter.handRight();
            handButton.setText("L");
        }else{
            LandmarkInterpreter.handRight();
            handButton.setText("R");
        }
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        handleIntent(intent);
        setIntent(intent);
    }

    void handleIntent(Intent intent) {
        if (intent == null || intent.getExtras() == null) return;

        if (intent.getExtras().containsKey("doQuit"))
            if (mUnityPlayer != null) {
                finish();
            }
    }

    @Override
    protected void onResume() {
        super.onResume();

        converter = new ExternalTextureConverter2(eglManager.getContext());
        converter.setFlipY(FLIP_FRAMES_VERTICALLY);
        converter.setConsumer(processor);

        if (PermissionHelper.cameraPermissionsGranted(this)) {
            startCamera();
        }
    }

    @Override
    protected void onPause() {
        super.onPause();
        converter.close();
    }

    @Override
    public void onRequestPermissionsResult(
            int requestCode, String[] permissions, int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        PermissionHelper.onRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    /**
     * Adds the preview frames of the Google Mediapipe framework to the camera layout, so the
     * Google Mediapipe output can be viewed during the use of the application.
     */
    private void setupPreviewDisplayView() {
        previewDisplayView.setVisibility(View.GONE);
        FrameLayout cameraLayout = findViewById(R.id.cameraLayout);

        cameraLayout.addView(previewDisplayView);

        previewDisplayView
                .getHolder()
                .addCallback(
                        new SurfaceHolder.Callback() {
                            @Override
                            public void surfaceCreated(SurfaceHolder holder) {
                                processor.getVideoSurfaceOutput().setSurface(holder.getSurface());
                            }

                            @Override
                            public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {
                                // (Re-)Compute the ideal size of the camera-preview display (the area that the
                                // camera-preview frames get rendered onto, potentially with scaling and rotation)
                                // based on the size of the SurfaceView that contains the display.
                                Size viewSize = new Size(width, height);
                                Size displaySize = cameraHelper.computeDisplaySizeFromViewSize(viewSize);
                                // Connect the converter to the camera-preview frames as its input (via
                                // previewFrameTexture), and configure the output width and height as the computed
                                // display size.
                                converter.setSurfaceTextureAndAttachToGLContext(
                                        previewFrameTexture, displaySize.getWidth(), displaySize.getHeight());
                            }

                            @Override
                            public void surfaceDestroyed(SurfaceHolder holder) {
                                processor.getVideoSurfaceOutput().setSurface(null);
                            }
                        });
    }

    /**
     * Starts the camera and applies camera frames to the preview frame.
     */
    private void startCamera() {
        cameraHelper = new CameraXPreviewHelper();
        cameraHelper.setOnCameraStartedListener(
                surfaceTexture -> {
                    previewFrameTexture = surfaceTexture;
                    // Make the display view visible to start showing the preview. This triggers the
                    // SurfaceHolder.Callback added to (the holder of) previewDisplayView.
                    previewDisplayView.setVisibility(View.VISIBLE);
                });
        cameraHelper.startCamera(this, CAMERA_FACING, /*surfaceTexture=*/ null);
    }
}
