using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the change between selection approaches.
/// </summary>
public class ChangeApproaches : MonoBehaviour
{
    //Tooltip when changing to gaze selection
    public GameObject gazeSelectionText;

    //Tooltip when changing to gesture selection
    public GameObject gestureSelectionText;

    //Pointer for gaze selection
    public GameObject gvrReticlePointer;

    //Selection pointer for gesture selection
    public GameObject selectionFivePointer;

    //Pointer for the accuracy game for the gesture selection
    public GameObject accuracyFivePointer;

    //Interface signal that shows if hand is recognized or not
    public GameObject handLamp;

    //Interface text that shows recognized gesture
    public GameObject gestureText;

    //Enum of the different approaches
    enum Approach
    {
        Gaze, Gesture, Five, Ok
    }

    /// <summary>
    /// Activates the gaze approach and deactivates the gesture approach.
    /// </summary>
    public void ActivateGazeApproach()
    {
        Approaches.selectionApproach = Approaches.SelectionApproach.Gaze;
        gvrReticlePointer.SetActive(true);
        gazeSelectionText.SetActive(true);
        DeactivateGestureApproach();
    }

    /// <summary>
    /// Activates the gesture approach and deactivates the gaze approach.
    /// </summary>
    public void ActivateGestureApproach()
    {
        Approaches.selectionApproach = Approaches.SelectionApproach.Gesture;
        gestureSelectionText.SetActive(true);
        handLamp.SetActive(true);
        gestureText.SetActive(true);
        gestureSelectionText.SetActive(true);
        selectionFivePointer.SetActive(true);
        DeactivateGazeApproach();
    }

    /// <summary>
    /// Deactivates the gaze approach.
    /// </summary>
    void DeactivateGazeApproach()
    {
        gvrReticlePointer.SetActive(false);
        DeactivateRestText(Approach.Gesture);
    }

    /// <summary>
    /// Deactivates the gesture approach.
    /// </summary>
    void DeactivateGestureApproach()
    {
        handLamp.SetActive(false);
        gestureText.SetActive(false);
        selectionFivePointer.SetActive(false);
        accuracyFivePointer.SetActive(false);
        DeactivateRestText(Approach.Gaze);
    }

    /// <summary>
    /// Deactivates the tooltip of the selection approach.
    /// </summary>
    /// <param name="pApproach">Selection approach</param>
    void DeactivateRestText(Approach pApproach)
    {
        switch (pApproach)
        {
            case Approach.Gaze:
                ResetGestureSelectionText();
                break;
            case Approach.Gesture:
                ResetGazeSelectionText();
                break;
        }
    }

    /// <summary>
    /// Resets the tooltip of the gaze selection.
    /// </summary>
    void ResetGazeSelectionText()
    {
        gazeSelectionText.SetActive(false);
        Color gazeCol = gazeSelectionText.GetComponent<Image>().color;
        gazeCol.a = 1;
        gazeSelectionText.GetComponent<Image>().color = gazeCol;
    }

    /// <summary>
    /// Resets the tooltip of the gesture selection.
    /// </summary>
    void ResetGestureSelectionText()
    {
        gestureSelectionText.SetActive(false);
        Color gestureCol = gestureSelectionText.GetComponent<Image>().color;
        gestureCol.a = 1;
        gestureSelectionText.GetComponent<Image>().color = gestureCol;
    }
}
