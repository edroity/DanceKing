using System;
using UnityEngine;

public static class WebCamUtil 
{
    public static Texture2D TakePicture(WebCamTexture cam, bool isHorizenalMirrored)
    {
        Color32[] result = cam.GetPixels32 ();
        var orientation = cam.videoRotationAngle;
        var camWidth = cam.width;
        var camHeight = cam.height;
        if (orientation != 0)
            result = Orient (result, orientation, camWidth, camHeight);

        bool verticalRot = orientation != 0 && orientation != 180;
        var width = verticalRot ? camHeight : camWidth;
        var height = verticalRot ? camWidth : camHeight;
        if (isHorizenalMirrored) {
            result = Horizen (result, width, height);
        }

        var newTex = new Texture2D (width, height);
        newTex.SetPixels32 (result);
        newTex.Apply ();
        return newTex;
    }
    public static Texture2D TakePictureFromCamera (Camera camera)
    {
        var resWidth = camera.pixelWidth;
        var resHeight = camera.pixelHeight;
        RenderTexture rt = new RenderTexture (resWidth, resHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D (resWidth, resHeight, TextureFormat.RGBA32, false);
        camera.Render ();
        RenderTexture.active = rt;
        screenShot.ReadPixels (new Rect (0, 0, resWidth, resHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.Destroy (rt);

        Color32[] result = screenShot.GetPixels32 ();
        var newTex = new Texture2D (resWidth, resHeight);
        newTex.SetPixels32 (result);
        newTex.Apply ();
        return newTex;
    }

    public static string TextureToBase64(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG ();
        //Destroy (newTex);
        string enc = Convert.ToBase64String (bytes);
        //Debug.Log (enc);
        return enc;
    }

    public static Color32[] Vertical(Color32[] colors, int width, int height)
    {
        var result = new Color32[colors.Length];
        int index = 0;
        for (int i = height - 1; i >= 0; i--) {
            for (int j = 0; j < width; j++) {
                result[index] = colors[j + i * width];
                index++;
            }
        }

        return result;
    }
    public static Color32[] Horizen(Color32[] colors, int width, int height)
    {
        var result = new Color32[colors.Length];
        int index = 0;
        for (int i = 0; i < height; i++) {
            for (int j = width - 1; j >= 0; j--) {
                result[index] = colors[j + i * width];
                index++;
            }
        }

        return result;
    }
    public static Color32[] Orient(Color32[] colors, int orient, int width, int height)
    {
        int index = 0;
        var result = new Color32[colors.Length];

        switch (orient) {
        case 270:
            for (int j = 0; j < width; j++) {
                for (int i = height - 1; i >= 0; i--) {
                    result[index] = colors[j + i * width];
                    index++;
                }
            }
            break;
        case 90:
            for (int j = width - 1; j >= 0; j--) {
                for (int i = 0; i < height; i++) {
                    result[index] = colors[j + i * width];
                    index++;
                }
            }
            break;
        case 180:
            for (int i = height - 1; i >= 0; i--) {
                for (int j = width - 1; j >= 0; j--) {
                    result[index] = colors[j + i * width];
                    index++;
                }
            }
            break;
        default:
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    result[index] = colors[j + i * width];
                    index++;
                }
            }
            break;
        }

        return result;
    }
}
