using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the hit and miss of the targets in the accuracy game for the gaze approach.
/// </summary>
public class HitOnGui : MonoBehaviour
{
    // The raycaster for the accuracy game
    public GvrPointerGraphicRaycaster guiRaycasterGame;

    // Pooler for the miss position objects for accuracy game
    public ObjectPooler missClickPoolerGame;

    // Pooler for the hit position objects for accuracy game
    public ObjectPooler hitClickPoolerGame;

    // Pooler for the target borders
    public ObjectPooler borderPoolerGame;

    // The accuracy game
    public AccuracyGame accuracyGame;

    // Flag for hitting a target in accuracy game
    private bool hitGame = false;

    // Get the collision position of the hit ray
    private Vector3 ray_worldposition;

    // They raycaster for the training game
    public GvrPointerGraphicRaycaster guiRaycasterTraining;

    // Pooler for the miss position objects for training game
    public ObjectPooler missClickPoolerTraining;

    // Pooler for the hit position objects for training game
    public ObjectPooler hitClickPoolerTraining;

    // Pooler for the target borders for training game
    public ObjectPooler borderPoolerTraining;

    // Spawns the target for the training game
    public TrainingSpawner trainingSpawner;

    //Flag for hitting a target in training game
    private bool hitTraining = false;

    // Update is called once per frame
    void Update()
    {
        //Gaze approach: button/touchscreen press
        if (Input.GetMouseButtonDown(0))
        {
            //Get the collision object with ray
            PointerEventData ped = new PointerEventData(null);
            List<RaycastResult> results = new List<RaycastResult>();

            guiRaycasterGame.Raycast(ped, results);
            foreach (RaycastResult hitRay in results)
            {
                //Check if collision object is a target
                if (hitRay.gameObject.tag == "UI_Target")
                {
                    //Log hit time
                    CsvWriter.hitTargetTimes.Add(Time.time);

                    //Despawn target and spawn target border object
                    hitGame = true;
                    GameObject hitborder = borderPoolerGame.GetPooledObject();
                    hitborder.gameObject.GetComponent<RectTransform>().localScale = hitRay.gameObject.GetComponent<RectTransform>().localScale;
                    hitborder.gameObject.transform.position = hitRay.gameObject.transform.position;
                    hitborder.gameObject.SetActive(true);

                    hitRay.gameObject.SetActive(false);
                }
                ray_worldposition = hitRay.worldPosition;
            }

            //If an object was hit with the ray
            if (results.Count > 0)
            {
                //Target was hit, spawn hit position object
                if (hitGame)
                {
                    accuracyGame.hitCount++;
                    GameObject hitclick = hitClickPoolerGame.GetPooledObject();
                    hitclick.gameObject.transform.position = ray_worldposition;
                    hitclick.gameObject.SetActive(true);
                }

                //Target was not hit, spawn miss position object
                else
                {
                    accuracyGame.missCount++;
                    GameObject missclick = missClickPoolerGame.GetPooledObject();
                    missclick.gameObject.transform.position = ray_worldposition;
                    missclick.gameObject.SetActive(true);
                }
                hitGame = false;
            }

            //For training game
            guiRaycasterTraining.Raycast(ped, results);
            foreach (RaycastResult hitRay in results)
            {
                //Check if ray collision object is target
                if (hitRay.gameObject.tag == "UI_Target")
                {

                    //Despawn target and spawn target border object
                    hitTraining = true;
                    GameObject hitborder = borderPoolerTraining.GetPooledObject();
                    hitborder.gameObject.GetComponent<RectTransform>().localScale = hitRay.gameObject.GetComponent<RectTransform>().localScale;
                    hitborder.gameObject.transform.position = hitRay.gameObject.transform.position;
                    hitborder.gameObject.SetActive(true);

                    trainingSpawner.SpawnTargetLocation();

                    hitRay.gameObject.SetActive(false);
                }
                ray_worldposition = hitRay.worldPosition;
            }

            //If an object was hit with the ray
            if (results.Count > 0)
            {
                //Target was hit, spawn hit position object
                if (hitTraining)
                {
                    GameObject hitclick = hitClickPoolerGame.GetPooledObject();
                    hitclick.gameObject.transform.position = ray_worldposition;
                    hitclick.gameObject.SetActive(true);
                }

                //Target was not hit, spawn miss position object
                else
                {
                    GameObject missclick = missClickPoolerGame.GetPooledObject();
                    missclick.gameObject.transform.position = ray_worldposition;
                    missclick.gameObject.SetActive(true);
                }
                hitTraining = false;
            }
        }
    }
}
