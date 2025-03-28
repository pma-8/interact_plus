using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// For debug purposes. Determines the distance between two targets.
/// </summary>
public class ClosestPoint : MonoBehaviour
{

    public GameObject secondTarget;
    public GameObject firstTarget;

    // Update is called once per frame
    void Update()
    {
        CircleCollider2D secondCollider = secondTarget.GetComponent<CircleCollider2D>();
        Vector3 secondClosestPoint = secondCollider.ClosestPoint(firstTarget.transform.position);
        secondClosestPoint.z = -7.15f;

        CircleCollider2D firstCollider = firstTarget.GetComponent<CircleCollider2D>();
        Vector3 firstClosestPoint = firstCollider.ClosestPoint(secondTarget.transform.position);
        firstClosestPoint.z = -7.15f;

        Debug.DrawLine(firstClosestPoint, secondClosestPoint, Color.blue);

        if (Input.GetKeyDown(KeyCode.W))
        {
            print("Length: " + (secondClosestPoint - firstClosestPoint).magnitude);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            firstTarget.transform.localPosition = secondTarget.transform.localPosition;
        }
    }
}
