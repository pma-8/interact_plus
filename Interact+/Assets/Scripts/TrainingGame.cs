using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the training game such as starting or ending.
/// </summary>
public class TrainingGame : MonoBehaviour
{

    //Flag if accuracy game is active currently
    public bool activeGame = false;

    //Target spawner
    public GameObject randomTargetSpawner;

    //Start button for the training game
    public GameObject startBtn;

    //Selection pointer for the OK approach
    public GameObject selectOkPointer;

    //Selection pointer for the Five approach
    public GameObject selectFivePointer;

    //Pointer for the accuracy game for the OK approach
    public GameObject accuracyOkPointer;

    //Pointer for the accuracy game for the Five approach
    public GameObject accuracyFivePointer;

    /*
     * Radial timer for pressing and holding a button. The timer will be filled when pressed.
     * If the timer is completly filled, the button will be activated.
     */
    public Image radialTimer;

    /*
     * The duration that the button has to be pressed to activate it with the gaze approach,
     * which is also the speed that determines how fast the radial timer is filled. 
     */
    public float increaseSpeedGaze = 0.03f;

    /*
     * The duration that the button has to be pressed to activate it with the gesture approach,
     * which is also the speed that determines how fast the radial timer is filled.
     */
    public float increaseSpeedGesture = 0.05f;

    //The speed that determines how fast the radial timer decreases after releasing the button
    public float decreaseSpeed = 0.01f;

    //Flag for starting the radial timer of the start button
    private bool isActivatedStart = false;

    // Update is called once per frame
    void Update()
    {
        //Increases the fill amount of the radial timer for the start button
        if (isActivatedStart)
        {
            TimerIncreaseStart(increaseSpeedGaze);
        }

        //Resets the timer and set fill amount to 0
        if (radialTimer.fillAmount <= 0)
        {
            ResetTimer();
        }

        //Decreases the fill amount of the radial timer
        radialTimer.fillAmount -= decreaseSpeed;
    }

    /// <summary>
    /// Activates the radial timer of the start button for the gaze approach.
    /// </summary>
    public void ActivateTimerIncreaseStart()
    {
        isActivatedStart = true;
    }

    /// <summary>
    /// Deactivates the radial timer of the start button for the gaze approach.
    /// </summary>
    public void DeactivateTimerIncreaseStart()
    {
        isActivatedStart = false;
    }

    /// <summary>
    /// Resets the fill amount of the timer.
    /// </summary>
    private void ResetTimer()
    {
        radialTimer.fillAmount = 0;
    }

    /// <summary>
    /// Increases the fill amount of the radial timer of the start button with given speed/amount.
    /// </summary>
    /// 
    /// <param name="pSpd"> Fill amount/Speed </param>
    public void TimerIncreaseStart(float pSpd)
    {
        radialTimer.fillAmount += pSpd;
        if (radialTimer.fillAmount >= 0.99f)
        {
            DeactivateTimerIncreaseStart();
            radialTimer.gameObject.SetActive(false);
            ResetTimer();
            StartGame();
        }
    }

    /// <summary>
    /// Starts the training game.
    /// </summary>
    public void StartGame()
    {
        activeGame = true;

        DeActivatePointer(false);
        randomTargetSpawner.SetActive(true);
        startBtn.SetActive(false);
    }

    /// <summary>
    /// Activate one pointer and disable the other pointer of the gesture selection
    /// </summary>
    /// <param name="pActive">Activate or not</param>
    public void DeActivatePointer(bool pActive)
    {
        if (Approaches.selectionApproach == Approaches.SelectionApproach.Gesture)
        {
            if (Approaches.gestureApproach == Approaches.GestureApproach.FiveFour)
            {
                selectFivePointer.SetActive(pActive);
                accuracyFivePointer.SetActive(!pActive);
            }
            else if (Approaches.gestureApproach == Approaches.GestureApproach.OkOpen)
            {
                selectOkPointer.SetActive(pActive);
                accuracyOkPointer.SetActive(!pActive);
            }
        }
    }
}
