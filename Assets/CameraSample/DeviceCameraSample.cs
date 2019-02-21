using System;
using UnityEngine;
using UnityEngine.UI;
public class DeviceCameraSample: MonoBehaviour {
    private bool camAvailable;
    private WebCamTexture backCam;
    private WebCamTexture frontCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;
    public CanvasScaler scaler;
    public RawImage ReviewRawIamge;

    // Use this for initialization
    private void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0) {
            Debug.Log ("No camera detected");
            camAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++) {
        
            if (devices[i].isFrontFacing) {    //開啟前鏡頭
                var w = 640;//t.sizeDelta.x;
                var h = 1136;//t.sizeDelta.y;
                backCam = new WebCamTexture (devices[i].name, w, h);
                //backCam = new WebCamTexture (devices[i].name, Screen.width, Screen.height);
            }
        }
        if (backCam == null) {
            Debug.Log ("Unable to find back camera");
            return;
        }

        backCam.Play ();
        background.texture = backCam;

        camAvailable = true;
    }
    void logInfo () {
        float ratio = (float)backCam.width / (float)backCam.height;
        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        int orient = -backCam.videoRotationAngle;
        Debug.LogFormat ("ratio: {0}\nwidth: {1} height: {2}\nscaleY: {3} orient: {4}", ratio, backCam.width,backCam.height, scaleY, orient);
    }

    bool isHorizenalMirrored = true;
    bool isVerticallyMirrored = false;
    int orientation = 0;

    int cachedWidth = 0;
    int cachedHeight = 0;
    // Update is called once per frame
    private void Update()
    {
        if (!camAvailable)
            return;
        // moved to OnButton_Shot
        // if (Input.GetMouseButton (0))
        //     HandleBase64Image (TextureToBase64 (backCam.GetPixels32 ()));
        if (cachedWidth != backCam.width || cachedHeight != backCam.height) {
            cachedWidth = backCam.width;
            cachedHeight = backCam.height;
            background.rectTransform.offsetMax = new Vector2 (cachedWidth / 2.0f, cachedHeight / 2.0f);
            background.rectTransform.offsetMin = -background.rectTransform.offsetMax;

            int orient0 = backCam.videoRotationAngle;
            int screenWidth, screenHeight;
            if (orient0 != 0 && orient0 != -180) {
                screenWidth = Screen.height;
                screenHeight = Screen.width;
            } else {
                screenWidth = Screen.width;
                screenHeight = Screen.height;
            }

            if ((float)cachedWidth / cachedHeight > (float)screenWidth / screenHeight) {
                scaler.scaleFactor = (float)screenWidth / cachedWidth;
            } else {
                scaler.scaleFactor = (float)screenHeight / cachedHeight;
            }
            Debug.LogFormat ("Screen: {0}x{1} Camera: {2}x{3}", screenWidth, screenHeight, cachedWidth, cachedHeight);
        }
            
        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3 (-1f, scaleY, 1f);    //鏡像

        int orient = backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3 (0, 0, orient);

        isVerticallyMirrored = backCam.videoVerticallyMirrored;
        orientation = backCam.videoRotationAngle;
        // Capture texture
        // TODO: 当点击选取照片按钮时，也会触发这一段代码，需要将拍照移动到按钮中去
        if (Input.GetMouseButton (0)) {
            Color32[] result = backCam.GetPixels32 ();
            if (orientation != 0)
                result = Orient (result, orientation, cachedWidth, cachedHeight);
                
            bool verticalRot = orientation != 0 && orientation != 180;
            var width = verticalRot ? cachedHeight : cachedWidth;
            var height = verticalRot ? cachedWidth : cachedHeight;
            if (isHorizenalMirrored) {
                result = Horizen (result, width, height);
            }

            HandleBase64Image (TextureToBase64 (result, width, height));

            logInfo ();
        }
    }

    string TextureToBase64 (Color32[] tex, int width, int height) {
        var newTex = new Texture2D (width, height);
        newTex.SetPixels32 (tex);
        newTex.Apply ();
        //newTex.Resize (Mathf.FloorToInt (backCam.width / 4), Mathf.FloorToInt (backCam.height / 4), TextureFormat.RGBA32, false);
        //newTex.Apply ();

        // Review
        int orient0 = backCam.videoRotationAngle;
        bool verticalRot = orient0 != 0 && orient0 != 180;
        ReviewRawIamge.texture = newTex;
        if (verticalRot)
            ReviewRawIamge.gameObject.GetComponent<AspectRatioFitter> ().aspectRatio = (float)cachedHeight / cachedWidth;
        else
            ReviewRawIamge.gameObject.GetComponent<AspectRatioFitter> ().aspectRatio = (float)cachedWidth / cachedHeight;
        ReviewRawIamge.color = Color.white;

        byte[] bytes = newTex.EncodeToPNG ();
        //Destroy (newTex);

        string enc = Convert.ToBase64String (bytes);
        //Debug.Log (enc);
        return enc;
    }
    Color32[] Vertical (Color32[] colors, int width, int height) {
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
    Color32[] Horizen (Color32[] colors, int width, int height) {
        var result = new Color32[colors.Length];
        int index = 0;
        for (int i = 0; i < height; i++) {
            for (int j = width - 1; j >= 0 ; j--) {
                result[index] = colors[j + i * width];
                index++;
            }
        }


        return result;
    }
    Color32[] Orient (Color32[] colors, int orient, int width, int height) {
        int index = 0;
        var result = new Color32[colors.Length];
            
        switch (orient) {
        case 270:
            for (int j = 0; j < width ; j++) {
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


    void HandleBase64Image (string base64) {
        EmotionSDK.SetEmotionCheck(true);
        EmotionSDK.SetDirectionCheck(true);
        // 检测表情
        EmotionSDK.CheckAsync(base64).ContinueWith(task =>{
            
            // 任务返回一个字典, 使用 task.Result 访问
            Debug.Log($"[C#] complete");
            var faceList = task.Result;
            var faceCount = faceList.Count;
            Debug.Log($"face count: {faceCount}");
        });
    }

    public void OnButton_SelectPhoto()
    {
        Photo.SelectAsync().ContinueWith(task=>{
            var base64 = task.Result;
            HandleBase64Image(base64);
        });
    }

    public void OnButton_Shot()
    {
        // TODO: 需要将拍照代码移动到这里...
    }
}