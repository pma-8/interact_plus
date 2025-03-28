using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the fadeout effect for tooltips.
/// </summary>
public class FadeOutImage : MonoBehaviour
{
    // Speed of the fadeout
    public float spd;

    // Update is called once per frame
    void Update()
    {
        Image image = GetComponent<Image>();
        Color tmpColor = image.color;
        tmpColor.a -= spd;
        image.color = tmpColor;

        if (tmpColor.a <= 0)
        {
            tmpColor.a = 1;
            image.color = tmpColor;
            gameObject.SetActive(false);
        }
    }
}
