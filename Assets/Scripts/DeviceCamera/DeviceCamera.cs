using System;
using UnityEngine;
using UnityEngine.UI;
public class DeviceCamera: MonoBehaviour {
    private bool camAvailable;
    private WebCamTexture backCam;
    private WebCamTexture frontCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;
    public CanvasScaler scaler;

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
                backCam = new WebCamTexture (devices[i].name, Screen.width, Screen.height);
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
        Debug.LogFormat ("ratio: {0}\nwidth: {1} height: {2}\nscaleY: {3} orient: {4}", ratio, backCam.width, backCam.height, scaleY, orient);
    }

    int cachedWidth = 0;
    int cachedHeight = 0;
    // Update is called once per frame
    private void Update()
    {
        if (!camAvailable)
            return;
        if (Input.GetMouseButton (0))
            HandleBase64Image (TextureToBase64 (backCam.GetPixels32 ()));
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
                scaler.scaleFactor = (float)screenHeight / cachedHeight;
            } else {
                scaler.scaleFactor = (float)screenWidth / cachedWidth;
            }
            Debug.LogFormat ("Screen: {0}x{1} Camera: {2}x{3}", screenWidth, screenHeight, cachedWidth, cachedHeight);
        }
            
        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3 (-1f, scaleY, 1f);    //鏡像

        int orient = backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3 (0, 0, orient);
    }

    string TextureToBase64 (Color32[] tex) {
        var newTex = new Texture2D (backCam.width, backCam.height);
        newTex.SetPixels32 (tex);
        //newTex.Apply ();
        //newTex.Resize (Mathf.FloorToInt (backCam.width / 4), Mathf.FloorToInt (backCam.height / 4), TextureFormat.RGBA32, false);
        newTex.Apply ();


        byte[] bytes = newTex.EncodeToPNG ();
        Destroy (newTex);

        string enc = Convert.ToBase64String (bytes);
        //Debug.Log (enc);
        return enc;
    }

    void HandleBase64Image (string base64) {
        EmotionSDK.SetEmotionCheck(true);
        EmotionSDK.SetDirectionCheck(true);
        // 检测表情
        EmotionSDK.CheckAsync(base64).ContinueWith(task =>{
            
            // 任务返回一个字典, 使用 task.Result 访问
            // 字典包含 EmotionType.cs 中每一项表情
            // 对应值为各表情的权重
            // * 识别失败时返回 null
            Debug.Log($"[C#] complete");
            // Debug.Log($"Love: {task.Result?["Love"]}");
            var json = CustomLitJson.JsonMapper.Instance.ToJson(task.Result);
            Debug.Log(json);
        });
    }
}