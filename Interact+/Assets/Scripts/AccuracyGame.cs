using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Handles the accuracy game such as starting or ending.
/// </summary>
public class AccuracyGame : MonoBehaviour
{

    //Sets the scale size of the targets per round in the accuracy game
    public GameObject randomTargetSpawner;

    //Button to start the next round in the accuracy game
    public GameObject continueBtn;

    //Button to start the accuracy game
    public GameObject startBtn;

    //Selection pointer of the five approach
    public GameObject selectFivePointer;

    //Accuracy game pointer of the five approach
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

    //Flag for starting the radial timer of the continue button
    private bool isActivatedContinue = false;

    //Counter of target hits per round
    public int hitCount = 0;

    //Counter of target misses per round
    public int missCount = 0;

    //Maximum targets per round
    public int maxTargets = 10;

    //Current round
    public int round = 0;

    //Flag if accuracy game is currently activ
    public bool activeGame = false;

    //Maximum rounds of the accuracy game
    private int maxRounds = targetSizes.Length;

    //Target sizes of each round
    public static float[] targetSizes = { 1f, 0.75f, 0.5f };

    //Positions of every single target in every round
    public static Vector3[][] allTargetPositions =
    {
        new Vector3[] {
            new Vector3(0.5f, 0.2f, 0),
            // Distance between the targets: 0.4
            new Vector3(1.108f, 1, 0),
            // 0.8
            new Vector3(2.428f, 0.53f, 0),
            // 1.2
            new Vector3(1.3f, -0.8835f, 0),
            // 1.6
            new Vector3(-0.775f, -0.15f, 0),
            // 2
            new Vector3(1.42f, 1.25f, 0),
            // 2.4
            new Vector3(-1.58f, 1.18f, 0),
            // 2.8
            new Vector3(0.94f, -1.1025f, 0),
            // 3.2
            new Vector3(-2.675f, 0.07f, 0),
            // 3.6
            new Vector3(1.501f, -0.38f, 0),
        },
        new Vector3[]
        {
            new Vector3(-0.5f, -0.2f, 0),
            // Distance between the targets: 0.4
            new Vector3(-1.2342f, -0.63f, 0),
            // 0.8
            new Vector3(-1.66f, 0.546f, 0),
            // 1.2
            new Vector3(-0.14f, 1.19f, 0),
            // 1.6
            new Vector3(1.5f, -0.04f, 0),
            // 2
            new Vector3(-0.56f, -1.37f, 0),
            // 2.4
            new Vector3(-1.95f, 1.1185f, 0),
            // 2.8
            new Vector3(1.1564f, 0.16f, 0),
            // 3.2
            new Vector3(-2.1f, -1.49f, 0),
            // 3.6
            new Vector3(1.634f, 0.08f, 0),
        },
        new Vector3[]
        {
            new Vector3(0, 0.5f, 0),
            // Distance between the targets: 0.4
            new Vector3(0.26f, 1.15f, 0),
            // 0.8
            new Vector3(-0.8165f, 1.38f, 0),
            // 1.2
            new Vector3(-1.87f, 0.312f, 0),
            // 1.6
            new Vector3(-0.358f, -0.84f, 0),
            // 2
            new Vector3(1.9f, -1.3f, 0),
            // 2.4
            new Vector3(2.67f, 1.288f, 0),
            // 2.8
            new Vector3(-0.43f, 1.29f, 0),
            // 3.2
            new Vector3(-2.65f, -1.417f, 0),
            // 3.6
            new Vector3(1.0175f, -0.09f, 0),
        }

    };

    void Update()
    {
        //Stops the round after all the targets in the round got destroyed
        if (hitCount >= maxTargets)
        {
            StopRound();
        }

        //For debug purpose
        if (Input.GetKey(KeyCode.T))
        {
            TimerIncreaseStart(increaseSpeedGesture);
        }

        //For debug purpose
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTimer();
        }

        //Increases the fill amount of the radial timer for the start button
        if (isActivatedStart)
        {
            TimerIncreaseStart(increaseSpeedGaze);
        }

        //Increases the fill amount of the radial timer for the continue button
        if (isActivatedContinue)
        {
            TimerIncreaseContinue(increaseSpeedGaze);
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
    /// Activates the radial timer of the continue button for the gaze approach.
    /// </summary>
    public void ActivateTimerIncreaseContinue()
    {
        isActivatedContinue = true;
    }

    /// <summary>
    /// Deactivates the radial timer of the start button for the gaze approach.
    /// </summary>
    public void DeactivateTimerIncreaseStart()
    {
        isActivatedStart = false;
    }

    /// <summary>
    /// Deactivates the radial timer of the continue button for the gaze approach.
    /// </summary>
    public void DeactivateTimerIncreaseContinue()
    {
        isActivatedContinue = false;
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
    /// Increases the fill amount of the radial timer of the continue button with given speed/amount.
    /// </summary>
    /// 
    /// <param name="pSpd"> Fill amount/Speed </param>
    public void TimerIncreaseContinue(float pSpd)
    {
        radialTimer.fillAmount += pSpd;
        if (radialTimer.fillAmount >= 0.99f)
        {
            DeactivateTimerIncreaseContinue();
            radialTimer.gameObject.SetActive(false);
            ResetTimer();
            StartNextRound();
        }
    }

    /// <summary>
    /// Starts the next round of the accuracy game. Resets and collects data for the csv file.
    /// </summary>
    public void StartNextRound()
    {
        CsvWriter.spawnTargetTimes.Clear();
        CsvWriter.hitTargetTimes.Clear();
        CsvWriter.targetDistances.Clear();
        CsvWriter.startRoundTime = Time.time;

        DeActivatePointer(false);

        missCount = 0;
        randomTargetSpawner.GetComponent<TargetSpawner>().scaleTarget = targetSizes[round];
        randomTargetSpawner.SetActive(true);
        continueBtn.SetActive(false);
    }

    /// <summary>
    /// Stops the current round of the accuracy game and collects data for the csv file.
    /// </summary>
    public void StopRound()
    {
        radialTimer.gameObject.SetActive(true);
        CsvWriter.endRoundTime = Time.time;
        CsvWriter.hitCount = hitCount;
        CsvWriter.missCount = missCount;
        CsvWriter.accuracy = AccuracyInPercent(hitCount, missCount);
        CsvWriter.round = round;
        CsvWriter.scale = targetSizes[round];

        DeActivatePointer(true);

        //For all rounds except last
        if (round != maxRounds - 1)
        {
            print(roundInformation());
        }

        hitCount = 0;
        missCount = 0;

        if (round >= maxRounds - 1)
        {
            EndGame();
        }
        else
        {
            round++;
            GameObject[] allTargets = GameObject.FindGameObjectsWithTag("UI_Target");
            foreach (GameObject target in allTargets)
            {
                target.SetActive(false);
            }

            randomTargetSpawner.SetActive(false);
            continueBtn.SetActive(true);
        }
    }

    /// <summary>
    /// Starts the accuracy game and collects data for the csv file.
    /// </summary>
    public void StartGame()
    {
        activeGame = true;

        DeActivatePointer(false);


        randomTargetSpawner.SetActive(true);
        randomTargetSpawner.GetComponent<TargetSpawner>().scaleTarget = targetSizes[round];
        startBtn.SetActive(false);
        hitCount = 0;
        missCount = 0;
        round = 0;
        CsvWriter.spawnTargetTimes.Clear();
        CsvWriter.hitTargetTimes.Clear();
        CsvWriter.targetDistances.Clear();


        CsvWriter.startGameTime = Time.time;
        CsvWriter.endGameTime = 0;
        CsvWriter.startRoundTime = Time.time;
        CsvWriter.endRoundTime = 0;
        CsvWriter.csvEntry = "";

    }

    /// <summary>
    /// Calculates the accuracy with the hit and miss amount.
    /// </summary>
    /// <param name="pHitCount">Amount of target hits</param>
    /// <param name="pMissCount">Amount of target misses/param>
    /// <returns></returns>
    public float AccuracyInPercent(float pHitCount, float pMissCount)
    {
        return (pHitCount / (pHitCount + pMissCount)) * 100;
    }

    /// <summary>
    /// Ends the accuracy game and collects data for the csv file.
    /// </summary>
    public void EndGame()
    {
        if ((round == maxRounds - 1) && CsvWriter.hitTargetTimes.Count == maxTargets)
        {
            CsvWriter.endGameTime = Time.time;

            print(roundInformation());

            CsvWriter.WriteGameInfoToCsv("AccuracyGame_Time.csv");
        }

        DeActivatePointer(true);

        activeGame = false;
        continueBtn.SetActive(false);
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("UI_Target");
        foreach (GameObject target in allTargets)
        {
            target.SetActive(false);
        }
        randomTargetSpawner.SetActive(false);
        startBtn.SetActive(true);
        round = 0;
    }

    /// <summary>
    /// Collects data of the current round for the csv file and returns it.
    /// </summary>
    /// <returns>Data of the current round</returns>
    string roundInformation()
    {
        string preEntry = "";

        preEntry += CsvWriter.round + ","
                 + CsvWriter.scale.ToString().Replace(',', '.') + ","
                 + CsvWriter.startRoundTime.ToString().Replace(',', '.') + ","
                 + CsvWriter.endRoundTime.ToString().Replace(',', '.') + ","
                 + CsvWriter.hitCount + ","
                 + CsvWriter.missCount + ","
                 + CsvWriter.accuracy.ToString().Replace(',', '.') + ","
                 + CsvWriter.startGameTime.ToString().Replace(',', '.') + ","
                 + CsvWriter.endGameTime.ToString().Replace(',', '.');

        preEntry += Approaches.selectionApproach == Approaches.SelectionApproach.Gaze ? "," + Approaches.selectionApproach.ToString() : "";
        preEntry += Approaches.selectionApproach == Approaches.SelectionApproach.Gesture ? "," + Approaches.gestureApproach.ToString() : "";

        for (int i = 0; i < CsvWriter.spawnTargetTimes.Count; i++)
        {
            preEntry += "," + CsvWriter.spawnTargetTimes[i].ToString().Replace(',', '.') + "," + CsvWriter.hitTargetTimes[i].ToString().Replace(',', '.') + "," + CsvWriter.targetDistances[i].ToString().Replace(',', '.');
        }

        CsvWriter.csvEntry += preEntry + "\n";

        return
            " | round: " + CsvWriter.round +
            " | scale: " + CsvWriter.scale +

            " | startRoundTime: " + CsvWriter.startRoundTime +
            " | endRoundTime: " + CsvWriter.endRoundTime +

            " | hitCount: " + CsvWriter.hitCount +
            " | missCount: " + CsvWriter.missCount +
            " | accuracy: " + CsvWriter.accuracy +

            " | startGameTime: " + CsvWriter.startGameTime +
            " | endGameTime: " + CsvWriter.endGameTime +

            " | SpTarget: " + CsvWriter.spawnTargetTimes.Count +
            " | HitTarget: " + CsvWriter.hitTargetTimes.Count +
            " | distance: " + CsvWriter.targetDistances.Count +

            " | SpTarget: " + CsvWriter.spawnTargetTimes[0] +
            " | HitTarget: " + CsvWriter.hitTargetTimes[0] +
            " | distance: " + CsvWriter.targetDistances[0];
    }

    /// <summary>
    /// Activate one pointer and disable the other pointer of the gesture selection
    /// </summary>
    /// <param name="pActive">Activate or not</param>
    private void DeActivatePointer(bool pActive)
    {
        if (Approaches.selectionApproach == Approaches.SelectionApproach.Gesture)
        {

            selectFivePointer.SetActive(pActive);
            accuracyFivePointer.SetActive(!pActive);

        }
    }
}
