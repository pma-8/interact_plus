using UnityEngine;


/// <summary>
/// Handles rotation of the gameobject, so it always faces the camera.
/// </summary>
public class FaceCameraGUI : MonoBehaviour
{
    public Vector3 up;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, up);
        transform.GetComponent<RectTransform>().eulerAngles = Vector3.Scale(transform.GetComponent<RectTransform>().eulerAngles, new Vector3(-1, 1, 1));
        transform.GetComponent<RectTransform>().eulerAngles += new Vector3(0, 180, 0);
    }
}
