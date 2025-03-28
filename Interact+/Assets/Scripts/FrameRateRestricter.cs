using UnityEngine;

/// <summary>
/// Limits the frame rate of the application.
/// </summary>
public class FrameRateRestricter : MonoBehaviour
{

    // Desired fps
    public int target = 30;

    // Start is called before the first frame update
    void Awake()
    {
        QualitySettings.vSyncCount = 0; //Vsync must be disabled
        Application.targetFrameRate = target;
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != target)
        {
            Application.targetFrameRate = target;
        }
    }
}
