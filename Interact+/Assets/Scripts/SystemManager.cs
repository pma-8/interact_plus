using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the gesture approach in the unity component.
/// </summary>
public class SystemManager : MonoBehaviour
{

    // The accuracy game
    public AccuracyGame accuracyGame;

    // Raycaster for accuracy game (for debugging)
    public GvrPointerGraphicRaycaster GuiRaycaster;

    // Training game
    public TrainingGame trainingGame;

    // Selection pointer for the gesture approach
    public GameObject selectFivePointer;

    // Class for changing approaches
    public ChangeApproaches changeApproaches;

    void Update()
    {
        //For debug purposes
        if (Input.GetKeyDown(KeyCode.E))
        {
            EvaluateFiveSelection();
        }

        //For debug purposes
        if (Input.GetKeyDown(KeyCode.R))
        {
            PointerEventData ped = new PointerEventData(null);
            List<RaycastResult> results = new List<RaycastResult>();
            GuiRaycaster.Raycast(ped, results);
            foreach (RaycastResult hitRay in results)
            {
                print(hitRay);
            }
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    MoveOkSelectionPointer("0,0997|0,8185|0,0403|0,73906");
        //    MoveFiveSelectionPointer("0,0997|0,8185|0,0403|0,73906");
        //}
        Debug.DrawRay(selectFivePointer.transform.position, selectFivePointer.transform.position - Camera.main.transform.position, Color.blue);
    }

    /// <summary>
    /// Move the selection pointer of the gesture approach accordingly to the given coordinates.
    /// Gets called by the android component.
    /// </summary>
    /// <param name="pCoor">2D-coordinates string</param>
    void MoveFiveSelectionPointer(string pCoor)
    {
        if (Approaches.selectionApproach == Approaches.SelectionApproach.Gesture && Approaches.gestureApproach == Approaches.GestureApproach.FiveFour)
        {
            if (pCoor == "")
            {
                return;
            }

            //Split the coordinate string
            string[] CoorArr = pCoor.Split('|');
            string thumbCoorX = CoorArr[0];
            string thumbCoorY = CoorArr[1];
            string firstCoorX = CoorArr[2];
            string firstCoorY = CoorArr[3];

            //Calculate the pointer position
            Vector2 thumbPos = new Vector2(float.Parse(thumbCoorX), float.Parse(thumbCoorY));
            Vector2 firstPos = new Vector2(float.Parse(firstCoorX), float.Parse(firstCoorY));

            Vector2[] fingers = { thumbPos, firstPos };
            Vector2 mid = (fingers[0] + fingers[1]) / 2;
            selectFivePointer.transform.localPosition = new Vector3(mid.x, mid.y, selectFivePointer.transform.localPosition.z);
        }
    }

    /// <summary>
    /// Shoots ray from player in direction of the pointer, recognizes the button and executes the right function accordingly.
    /// Gets called by the android component.
    /// </summary>
    void EvaluateFiveSelection()
    {
        if (Approaches.selectionApproach == Approaches.SelectionApproach.Gesture && Approaches.gestureApproach == Approaches.GestureApproach.FiveFour)
        {
            Ray ray = new Ray(selectFivePointer.transform.position, selectFivePointer.transform.position - Camera.main.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "GestureBtn")
                {
                    changeApproaches.ActivateGestureApproach();
                }
                else if (hit.transform.tag == "GazeBtn")
                {
                    changeApproaches.ActivateGazeApproach();
                }
                else if (hit.transform.tag == "AccuracyStartBtn")
                {
                    accuracyGame.TimerIncreaseStart(accuracyGame.increaseSpeedGesture);
                }
                else if (hit.transform.tag == "AccuracyEndBtn")
                {
                    accuracyGame.EndGame();
                }
                else if (hit.transform.tag == "TrainingStartBtn")
                {
                    trainingGame.TimerIncreaseStart(trainingGame.increaseSpeedGesture);
                }
                else if (hit.transform.tag == "AccuracyContinueBtn")
                {
                    accuracyGame.TimerIncreaseContinue(accuracyGame.increaseSpeedGesture);
                }
            }
        }
    }
}
