using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Template for handling communication between android and unity component
/// </summary>
public class DataExchanger : MonoBehaviour
{
    //Interface text of gesture selection
    public Text textField;

    /// <summary>
    /// Show the current recognized gesture.
    /// </summary>
    /// <param name="gesture">Recognized gesture</param>
    public void ShowGesture(string gesture)
    {
        if (gameObject.activeSelf)
            textField.text = gesture;
    }

    /// <summary>
    /// Show the positions of the landmarks. (Method which can be called from Android App)
    /// </summary>
    /// <param name="message">The message from the android component</param>
    public void ShowLandmarks(string message)
    {
        textField.text = message;
    }

    //public void PassDataToAndroid()
    //{
    //    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivitiy");
    //    activity.CallStatic("setDataFromUnity", new object[] { textField.text });
    //}
}
