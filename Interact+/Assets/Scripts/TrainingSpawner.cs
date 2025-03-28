using UnityEngine;

/// <summary>
/// Handles the target spawn for the training game.
/// </summary>
public class TrainingSpawner : MonoBehaviour
{
    //Canvas on which the targets will be spawned on
    public Canvas parentCanvas;

    //Target object pooler
    public ObjectPooler targetPooler;

    //Button to start the training game
    public GameObject startBtn;

    /*
     * Radial timer for pressing and holding a button. The timer will be filled when pressed.
     * If the timer is completly filled, the button will be activated.
     */
    public GameObject radialTimer;

    //Wall that hides the accuracy game and disappears after finish the training game
    public GameObject gameWall;

    //The training game
    public TrainingGame trainingGame;

    //Scale of the targets
    public float scaleTarget = 1;

    //Counter for the amount of spawned targets
    private int counter = 0;

    //New spawn position for the targets
    private Vector3 newPos;

    //Positions of every single target
    public static Vector3[] targetPositions =
    {
        new Vector3(-1f, -0.35f, 0),
            // Distance between the targets: 0.4
            new Vector3(1.25f, 0.9f, 0),
            // 0.8
            new Vector3(2f, -0.8f, 0),
    };

    private void OnEnable()
    {
        counter = 0;
        SpawnTargetLocation();
    }

    /// <summary>
    /// Spawn the target on the right position.
    /// </summary>
    public void SpawnTargetLocation()
    {
        if (counter == targetPositions.Length)
        {
            EndTraining();
            return;
        }
        newPos = targetPositions[counter];
        counter++;

        GameObject target = targetPooler.GetPooledObject();
        target.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        target.GetComponent<RectTransform>().localPosition = newPos;
        target.GetComponent<RectTransform>().localScale = new Vector3(scaleTarget, scaleTarget, scaleTarget);
        target.SetActive(true);
    }


    /// <summary>
    /// Ends the training game.
    /// </summary>
    private void EndTraining()
    {
        trainingGame.DeActivatePointer(true);
        gameWall.SetActive(false);
        counter = 0;
        startBtn.SetActive(true);
        gameObject.SetActive(false);
        radialTimer.SetActive(true);
    }
}
