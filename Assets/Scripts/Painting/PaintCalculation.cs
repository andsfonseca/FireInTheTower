using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCalculation : MonoBehaviour
{
    Renderer rend;
    Texture mainTex;
    Texture altTex;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int[] calcPaint() {
        rend = GetComponent<Renderer>();
        altTex = rend.material.GetTexture("_DrawingTex");
        Texture2D alttex2D = new Texture2D(altTex.width, altTex.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = new RenderTexture(alttex2D.width, alttex2D.height, 32);
        Graphics.Blit(altTex, renderTexture);

        RenderTexture.active = renderTexture;
        alttex2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        alttex2D.Apply();

        Color[] pixels = alttex2D.GetPixels();

        RenderTexture.active = currentRT;
        renderTexture.Release();
        int countGreen = 0;
        int countRed = 0;
        int countYellow = 0;
        int countBlue = 0;
        //Debug.Log("LENGTH: " + pixels.Length);
        for (int i = 0; i < pixels.Length; i++) {
            //0.54f, 0.88f, 0.38f
            if (pixels[i].r <= 0.60f && pixels[i].r >= 0.50f && pixels[i].g <= 0.90f && pixels[i].g >= 0.80f && pixels[i].b <= 0.40f && pixels[i].b >= 0.30f) {
                countGreen++;
            }
            //0.98f, 0.51f, 0.2f
            if (pixels[i].r <= 1 && pixels[i].r >= 0.90f && pixels[i].g <= 0.55f && pixels[i].g >= 0.45f && pixels[i].b <= 0.25f && pixels[i].b >= 0.15f) {
                countRed++;
            }
            //1, 0.85f, 0.28f
            if (pixels[i].r <= 1 && pixels[i].r >= 0.90f && pixels[i].g <= 0.90f && pixels[i].g >= 0.80f && pixels[i].b <= 0.33f && pixels[i].b >= 0.23f) {
                countYellow++;
            }
            //0.21f, 0.73f, 0.95f
            if (pixels[i].r <= 0.26f && pixels[i].r >= 0.16f && pixels[i].g <= 0.78f && pixels[i].g >= 0.68f && pixels[i].b <= 1f && pixels[i].b >= 0.90f) {
                countBlue++;
            }
        }
        /* 
        //DEBUG logs
        Debug.Log("PIXEL TEST: " + pixels[0] + pixels[100]);
        Debug.Log("GREEN COUNT: " + countGreen);
        Debug.Log("BLUE COUNT: " + countBlue);
        Debug.Log("RED COUNT: " + countRed);
        Debug.Log("YELLOW COUNT: " + countYellow);
        */
        int[] counts = new int[] { countRed, countYellow, countGreen, countBlue };
        return counts;
    }
}
