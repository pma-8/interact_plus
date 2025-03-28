using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the color change accordingly to the hand recognition.
/// </summary>
public class HandRecognize : MonoBehaviour
{
    /// <summary>
    /// Change the color of the interface signal
    /// accordingly to the response from the android component.
    /// Android component can only send strings.
    /// </summary>
    /// <param name="pBool">If hand is recognized or not</param>
    public void HandRecognized(string pBool)
    {
        if (gameObject.activeSelf)
        {
            if (pBool == "true")
            {
                GetComponent<Image>().color = Color.green;
            }
            else
            {
                GetComponent<Image>().color = Color.red;
            }
        }
    }
}
