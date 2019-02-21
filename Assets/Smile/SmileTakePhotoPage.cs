using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SmileTakePhotoPage : SimplePage
{
    public Texture2D DefaultPic; 
    public Texture2D Pic;
    private Texture defaultBackground;
    public RawImage background;
    public AspectRatioFitter fit;
    //public RawImage ReviewRawIamge;
    public RectTransform Scaler;

    private bool camSwitch = false;
    private bool camAvailable;
    private WebCamTexture backCam;
    //private WebCamTexture frontCam;

    bool isHorizenalMirrored = true;
    bool isVerticallyMirrored = false;
    int orientation = 0;

    int cachedWidth = 0;
    int cachedHeight = 0;
	public GameObject[] UIObjects;

    public Text loading_tiper;
    bool m_loading = false;

    public void OnClose () {
        SimplePageManager.Instance.Show ("SmileMainPage");
        SmileProject.Instance.PlaySe("smile_click");
    }

    public WebCamTexture Camera
    {
        get
        {
            return backCam;
        }
    }

    public void SwitchCamera(bool _switch)
    {
        camSwitch = _switch;
        if (camSwitch)
        {
            InitCamera();
            if(camAvailable)
            {
                Camera.Play();
            }
        }
        else
        {
            Camera.Stop();
        }
    }

    
    private void Update()
    {
        if (camSwitch)
        {
            UpdateCamera();
        }
       // if(m_loading)
        {
            int ttt = ((int)Time.time) % 3;
            loading_tiper.text = "图片分析中";
            for(int i=0;i<ttt;++i)
            {
                loading_tiper.text +=".";
            }
        }
    }



    bool m_is_front = false;

    public void InitCamera()
    {
        if (camAvailable)
        {
            return;
        }
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {

            if (devices[i].isFrontFacing)
            {    //開啟前鏡頭
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                m_is_front = true;
            }
        }
        if (backCam == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        backCam.Play();
        background.texture = backCam;

        camAvailable = true;
    }

    void UpdateCamera()
    {
        if (!camAvailable)
            return;
        if (cachedWidth != backCam.width || cachedHeight != backCam.height)
        {
            cachedWidth = backCam.width;
            cachedHeight = backCam.height;
            background.rectTransform.offsetMax = new Vector2(cachedWidth / 2.0f, cachedHeight / 2.0f);
            background.rectTransform.offsetMin = -background.rectTransform.offsetMax;

            int orient0 = backCam.videoRotationAngle;
            int screenWidth, screenHeight;
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            int camWidth, camHeight;
            if (orient0 == 0 || orient0 == 180)
            {
                camWidth = cachedWidth;
                camHeight = cachedHeight;
            }
            else
            {
                camWidth = cachedHeight;
                camHeight = cachedWidth;
            }


            if ((float)camWidth / camHeight > (float)screenWidth / screenHeight)
            {
                Scaler.localScale = Vector3.one * (float)screenWidth / camWidth * (640f / screenWidth);
            }
            else
            {
                Scaler.localScale = Vector3.one * (float)screenHeight / camHeight * (1136f / screenHeight);
            }

            Debug.LogFormat("Screen: {0}x{1} Camera: {2}x{3}", screenWidth, screenHeight, cachedWidth, cachedHeight);
        }

        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(-1f, scaleY, 1f);    //鏡像

        int orient = backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        isVerticallyMirrored = backCam.videoVerticallyMirrored;
        orientation = backCam.videoRotationAngle;
    }
    StringBuilder logBuilder = new StringBuilder ();
    public async void OnShoot () {
        for (int i = 0; i < UIObjects.Length; i++)
            UIObjects[i].SetActive (false);
        SmileProject.Instance.PlaySe("flash");

        Texture2D tex = null;
 

        if (camAvailable)
        {
            tex = ImitateUtil.Orient(backCam, backCam.videoRotationAngle,m_is_front);
           //tex = WebCamUtil.TakePictureFromCamera (SmileProject.Instance.Camera);
        }
        else
        {
            tex = DefaultPic;
        }

        m_loading = true;
        loading_tiper.enabled = true;

        if (backCam != null)
        {
            backCam.Pause();
        }
        //SmileProject.Instance.LutifyComp.Blend = 0;


        var result = await HandleBase64Image (WebCamUtil.TextureToBase64 (tex), tex);


        var resultPage = SimplePageManager.Instance.Show ("SmileResultPage") as SmileResultPage;
        resultPage.SetInfo (MakeScore(result[0].emotion), tex,result[0].emotion);

        //logBuilder.Clear ();
        //for (int i = 0; i < result[0].emotion.Count; i++) {
        //    logBuilder.Append (EmotionSDK.GetEmotionTypeNameByIndex (i));
        //    logBuilder.Append ( ": " );
        //    logBuilder.AppendLine (result[0].emotion[i].ToString ());
        //}
        //Debug.Log (logBuilder.ToString ());
    }
    public async void OnTestPic () {
        if(Application.isEditor)
        {
            return;
        }
        var base64 = await Photo.SelectAsync ();
        if (!string.IsNullOrEmpty (base64)) {
            var tex = new Texture2D(1,1);
            var b64_bytes = System.Convert.FromBase64String (base64);
            tex.LoadImage (b64_bytes);

            var result = await HandleBase64Image (WebCamUtil.TextureToBase64 (tex), tex);
            var resultPage = SimplePageManager.Instance.Show ("SmileResultPage") as SmileResultPage;
            resultPage.SetInfo (MakeScore(result[0].emotion),tex,result[0].emotion);

            logBuilder.Clear ();
            for (int i = 0; i < result[0].emotion.Count; i++) {
                logBuilder.Append (EmotionSDK.GetEmotionTypeNameByIndex (i));
                logBuilder.Append (": ");
                logBuilder.AppendLine (result[0].emotion[i].ToString ());
            }
            Debug.Log (logBuilder.ToString ());
        } 
    }

    public int MakeScore(List<float> _emotion)
    {
        int Joy = (int)(_emotion[0] * 100*0.8f);
        int Love = (int)(_emotion[1] * 100*0.5f);
        int Optimism = (int)(_emotion[2] * 100*0.5f);
        int Trust = (int)(_emotion[8] * 100*0.5f);
        int Pride=(int)(_emotion[14] * 100*0.5f);
        int score = Joy + Optimism + Pride + Trust + Love;
        if(score<30)
        {
            score = Random.Range(20, 50);
        }
        if(score>=100)
        {
            score = Random.Range(90, 99);
        }
        return score;

    }

    public override void Refresh()
    {
        // Enable white filter
       // SmileProject.Instance.LutifyComp.Blend = 1;

        for (int i = 0; i < UIObjects.Length; i++)
            UIObjects[i].SetActive (true);
        SwitchCamera (true);
        StatusBarManager.Show (false);
        m_loading = false;
        loading_tiper.enabled = false;

    }

    async Task<List<Face>> HandleBase64Image(string base64, Texture2D tex)
    {
        EmotionSDK.SetEmotionCheck (true);
        EmotionSDK.SetDirectionCheck (true);
        // 检测表情
        var task = await EmotionSDK.CheckAsync (base64);
        return task;
    }
}
