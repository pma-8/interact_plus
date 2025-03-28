package com.mach.product;

import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;

/**
 * Custom interface for UnityPlayerActivity class.
 * So the Unity component can be accessed from code.
 */
public abstract class OverrideUnityActivity extends UnityPlayerActivity {

    // Access Point for other classes.
    public static OverrideUnityActivity instance = null;

    /**
     * On start-up of the UnityPlayerActivity, assign this interface to the instance variable.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        instance = this;
    }

    /**
     * On shutdown of the UnityPlayerActivity, remove this interface from the instance variable.
     */
    @Override
    protected void onDestroy() {
        super.onDestroy();
        instance = null;
    }
}
