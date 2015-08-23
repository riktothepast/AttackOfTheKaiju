using UnityEngine;
using System.Collections;

public class RectViewAdapter : MonoBehaviour {
    public float baseHeight;
    public float baseWidth;

    Rect cameraRect;
    float viewPortRectValue;
    public bool adaptHeight;
    public bool adaptWidth;

    float baseRes;
	// Use this for initialization
    void Start()
    {

        /*
         *  800 * 0.13 = 104
         *  104/800 = x 
         */

        if (adaptHeight)
            cameraRect = new Rect(cameraRect.xMin, 1 - (baseHeight / Screen.height), cameraRect.width, baseHeight / Screen.height);
        else
            cameraRect = new Rect(cameraRect.xMin, cameraRect.yMin, cameraRect.width, baseHeight / Screen.height);

        if (adaptWidth)
        {
            AspectRatio();
        }

    }

    void AspectRatio()
    {
        float targetaspect = 160f / 144f;

        float windowaspect = (float)Screen.width / (float)Screen.height;

        if (windowaspect == targetaspect) return;
        

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox

            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;

            camera.rect = rect;

            float scalewidth = 1.0f / scaleheight;

            rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;

            camera.rect = rect;
        
    }
}