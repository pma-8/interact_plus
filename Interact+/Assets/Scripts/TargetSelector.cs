using UnityEngine;

/// <summary>
/// Handles the movement of the pointer in the accuracy game and the collision between pointer and targets.
/// </summary>
public class TargetSelector : MonoBehaviour
{

    //Pointer in accuracy game for the OK approach
    public GameObject okPointerGame;

    //Pointer in accuracy game for the Five approach
    public GameObject fivePointerGame;

    //The accuracy game
    public AccuracyGame accuracyGame;

    //Spawns the targets in training game
    public TrainingSpawner trainingSpawner;

    //Pointer in training game for the OK approach
    public GameObject okPointerTraining;

    //Pointer in training game for the Five approach
    public GameObject fivePointerTraining;

    // Pooler for the miss position objects for training game
    public ObjectPooler missClickPooler;

    // Pooler for the hit position objects for training game
    public ObjectPooler hitClickPooler;

    // Pooler for the target borders for training game
    public ObjectPooler borderPooler;


    public float maxTargetSize = 0.3f;
    public float maxDepth = 0.3f;

    public ChangeApproaches changeApproaches;

    public void Update()
    {
        //For debug purpose
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EvaluatPalmPos("-0,33182192|0,6476109|0,5965333|0,41388607");
        }

        //For debug purpose
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckCollisionGestureFive();
        }
    }

    /// <summary>
    /// Calculate the position of the pointer for the five approach in the accuracy game and move it accordingly.
    /// </summary>
    /// <param name="pCoordinates">2D-coordinates string</param>
    void EvaluatPalmPos(string pCoordinates)
    {
        if (Approaches.gestureApproach == Approaches.GestureApproach.FiveFour)
        {
            Vector2[] fingers = getCoordinates(pCoordinates);
            if (fingers != null)
            {
                Vector2 mid = (fingers[0] + fingers[1]) / 2;
                fivePointerGame.transform.localPosition = new Vector3(mid.x, mid.y, fivePointerGame.transform.localPosition.z);
                fivePointerTraining.transform.localPosition = new Vector3(mid.x, mid.y, fivePointerTraining.transform.localPosition.z);
            }
        }
    }

    /// <summary>
    /// Calculate the position of the pointer for the ok approach in the accuracy game and move it accordingly.
    /// </summary>
    /// <param name="pCoordinates">2D-coordinates string</param>
    void EvaluateFingerPos(string pCoordinates)
    {
        if (Approaches.gestureApproach == Approaches.GestureApproach.OkOpen)
        {
            Vector2[] fingers = getCoordinates(pCoordinates);
            if (fingers != null)
            {
                Vector2 mid = (fingers[0] + fingers[1]) / 2;
                okPointerGame.transform.localPosition = new Vector3(mid.x, mid.y, okPointerGame.transform.localPosition.z);
            }
        }
    }

    /// <summary>
    /// Check the collision between the pointer in the accuracy game and a target for the five approach.
    /// </summary>
    void CheckCollisionGestureFive()
    {
        if (Approaches.gestureApproach == Approaches.GestureApproach.FiveFour)
        {
            CheckCollisionGestureGame(fivePointerGame);
            CheckCollisionGestureTraining(fivePointerTraining);
        }
    }

    /// <summary>
    /// Check the collision between the pointer in the accuracy game and a target for the ok approach. 
    /// </summary>
    void CheckCollisionGestureOk()
    {
        if (Approaches.gestureApproach == Approaches.GestureApproach.OkOpen)
        {
            CheckCollisionGestureGame(okPointerGame);
        }
    }

    /// <summary>
    /// Handles the behaviour of hitting and missing targets for the gesture approach in the accuracy game.
    /// </summary>
    /// <param name="pGesturePointer">Current gesture approach</param>
    void CheckCollisionGestureGame(GameObject pGesturePointer)
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("UI_Target");
        foreach (GameObject targetCheck in allTargets)
        {
            //Check if pointer is in the radius of the target
            if (InRadiusOfLastTarget(targetCheck.transform.localPosition, pGesturePointer.transform.localPosition, targetCheck.transform.localScale.x * maxTargetSize) && InRadiusOfDepthTarget(targetCheck.transform.position, pGesturePointer.transform.position, maxDepth))
            {

                //Log hit time
                CsvWriter.hitTargetTimes.Add(Time.time);

                //Create hitborder
                GameObject hitborder = borderPooler.GetPooledObject();
                hitborder.gameObject.GetComponent<RectTransform>().localScale = targetCheck.gameObject.GetComponent<RectTransform>().localScale;
                hitborder.gameObject.transform.position = targetCheck.gameObject.transform.position;
                hitborder.gameObject.SetActive(true);

                //Create hitclick
                GameObject hitclick = hitClickPooler.GetPooledObject();
                hitclick.gameObject.transform.position = pGesturePointer.transform.position;
                hitclick.gameObject.SetActive(true);

                //Despawn target
                targetCheck.gameObject.SetActive(false);

                accuracyGame.hitCount++;
            }
            else
            {
                accuracyGame.missCount++;

                //Create miss click object
                GameObject missclick = missClickPooler.GetPooledObject();
                missclick.gameObject.transform.position = pGesturePointer.transform.position;
                missclick.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Handles the behaviour of hitting and missing targets for the gesture approach in the training game.
    /// </summary>
    /// <param name="pGesturePointer">Current gesture approach</param>
    void CheckCollisionGestureTraining(GameObject pGesturePointer)
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("UI_Target");
        foreach (GameObject targetCheck in allTargets)
        {
            //Check if pointer is in the radius of the target
            if (InRadiusOfLastTarget(targetCheck.transform.localPosition, pGesturePointer.transform.localPosition, targetCheck.transform.localScale.x * maxTargetSize) && InRadiusOfDepthTarget(targetCheck.transform.position, pGesturePointer.transform.position, maxDepth))
            {

                //Create hitborder
                GameObject hitborder = borderPooler.GetPooledObject();
                hitborder.gameObject.GetComponent<RectTransform>().localScale = targetCheck.gameObject.GetComponent<RectTransform>().localScale;
                hitborder.gameObject.transform.position = targetCheck.gameObject.transform.position;
                hitborder.gameObject.SetActive(true);

                //Create hitclick
                GameObject hitclick = hitClickPooler.GetPooledObject();
                hitclick.gameObject.transform.position = pGesturePointer.transform.position;
                hitclick.gameObject.SetActive(true);

                //Despawn target
                targetCheck.gameObject.SetActive(false);
                //Spawn next target
                trainingSpawner.SpawnTargetLocation();
            }
            else
            {

                //Create miss click object
                GameObject missclick = missClickPooler.GetPooledObject();
                missclick.gameObject.transform.position = pGesturePointer.transform.position;
                missclick.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Convert 2D-coordinates string into an array for easier use. 
    /// </summary>
    /// <param name="pCoordinates">2D-coordinates string</param>
    /// <returns>2D-coordinates string array</returns>
    Vector2[] getCoordinates(string pCoordinates)
    {
        if (pCoordinates == "")
        {
            return null;
        }

        string[] CoorArr = pCoordinates.Split('|');
        string thumbCoorX = CoorArr[0];
        string thumbCoorY = CoorArr[1];
        string firstCoorX = CoorArr[2];
        string firstCoorY = CoorArr[3];

        Vector2 thumbPos = new Vector2(float.Parse(thumbCoorX), float.Parse(thumbCoorY));
        Vector2 firstPos = new Vector2(float.Parse(firstCoorX), float.Parse(firstCoorY));

        Vector2[] fingers = { thumbPos, firstPos };
        return fingers;
    }

    /// <summary>
    /// Check if the position of one object is in the radius of a second object.
    /// </summary>
    /// <param name="pNewPos">Position of first object</param>
    /// <param name="pOldPos">Position of second object</param>
    /// <param name="pRadius">Radius of second position</param>
    /// <returns>In radius or not</returns>
    bool InRadiusOfLastTarget(Vector3 pNewPos, Vector3 pOldPos, float pRadius)
    {
        return (pNewPos.x >= pOldPos.x - pRadius && pNewPos.x <= pOldPos.x + pRadius) && (pNewPos.y >= pOldPos.y - pRadius && pNewPos.y <= pOldPos.y + pRadius);
    }

    /// <summary>
    /// Check if the z-coordinate of one object is in the radius of a second object.
    /// </summary>
    /// <param name="pNewPos">Position of first object</param>
    /// <param name="pOldPos">Position of second object</param>
    /// <param name="pDepth">Radius of second position</param>
    /// <returns>In radius or not</returns>
    bool InRadiusOfDepthTarget(Vector3 pNewPos, Vector3 pOldPos, float pDepth)
    {
        return pNewPos.z >= pOldPos.z - pDepth && pNewPos.z <= pOldPos.z + pDepth;
    }
}
