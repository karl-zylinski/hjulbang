using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFixer : MonoBehaviour
{
	void Start ()
    {
        var text_mesh = GetComponent<TextMesh>();
        float pixel_ratio = (Camera.main.orthographicSize * 2.0f) / Camera.main.pixelHeight;
        float fuzziness = 32.0f; // lower number is better, 32 seemed to work for me

        int previousFontSize = text_mesh.fontSize;
        text_mesh.fontSize = Mathf.RoundToInt(text_mesh.fontSize / (pixel_ratio * fuzziness));

        float ratio = previousFontSize / ((float)text_mesh.fontSize);
        transform.localScale *= ratio;
    }
}
