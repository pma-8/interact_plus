using UnityEngine;


/// <summary>
/// Holds the current gesture and selection approach.
/// </summary>
public static class Approaches
{
    //Enum for both gestures
    public enum GestureApproach { OkOpen, FiveFour }

    //Enum for both selection approaches
    public enum SelectionApproach { Gesture, Gaze }

    //Current gesture
    public static GestureApproach gestureApproach = GestureApproach.FiveFour;

    //Current selection approach
    public static SelectionApproach selectionApproach = SelectionApproach.Gaze;

}