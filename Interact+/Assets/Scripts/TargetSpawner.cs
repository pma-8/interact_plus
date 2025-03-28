using UnityEngine;

/// <summary>
/// Handles the target spawn on the canvas of the accuracy game.
/// </summary>
public class TargetSpawner : MonoBehaviour
{
    //Canvas on which the targets will be spawned on
    public Canvas parentCanvas;

    //Target object pooler
    public ObjectPooler targetPooler;

    //Scale of the targets
    public float scaleTarget = 1;

    //The accuracy game
    public AccuracyGame accuracyGame;

    //Counter for the amount of spawned targets
    private int counter = 0;

    //New spawn position for the targets
    private Vector3 newPos;

    private void OnEnable()
    {
        counter = 0;
    }

    void Update()
    {
        if (counter < accuracyGame.maxTargets)
        {
            if (newPos == AccuracyGame.allTargetPositions[accuracyGame.round][accuracyGame.hitCount])
            {
                return;
            }

            //Log spawn time
            CsvWriter.targetDistances.Add(accuracyGame.hitCount * 0.4f);
            CsvWriter.spawnTargetTimes.Add(Time.time);
            SpawnTargetLocation();
        }
    }

    /// <summary>
    /// Spawn the target on the right position.
    /// </summary>
    void SpawnTargetLocation()
    {
        newPos = AccuracyGame.allTargetPositions[accuracyGame.round][accuracyGame.hitCount];
        counter++;

        GameObject target = targetPooler.GetPooledObject();
        target.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        target.GetComponent<RectTransform>().localPosition = newPos;
        target.GetComponent<RectTransform>().localScale = new Vector3(scaleTarget, scaleTarget, scaleTarget);
        target.SetActive(true);
    }
}
