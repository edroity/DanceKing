using System;
using UnityEngine;
using UnityEngine.UI;
public class HotAdjustPage : Page
{
    // Start is called before the first frame update

   //public Texture2D tex;
    public void ContinueB()
    {

        //Shot();
        //return;
        HotDogRoot.GetSingle().PlaySe("dianji");
        if(m_texture!=null)
        {
            m_texture.Stop();
        }
        m_texture = null;
        UIEngine.Forward<HotFightPage>();
        HotDotData.NeedAdjust = false;
    }


    private WebCamDevice FinddFrontFacingCamera()
    {

        // find camera
        var deviceList = WebCamTexture.devices;
        if (deviceList.Length == 0)
        {
            if(Application.isEditor)
            {
                ////Debug.LogWarning("No camera found");
            }
            else
            {
                throw new Exception("No camera found");
            }
          
        }
        WebCamDevice? device = null;
        foreach (var d in deviceList)
        {
            if (d.isFrontFacing)
            {
                device = d;
            }
        }
        if (device == null)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("No FrontFacing camera found");
            }
            else
            {
                throw new Exception("No FrontFacing camera found");
            }
           
        }
        return device.Value;
    }



    private void CreateCameraTextureForRawImage(RawImage rawImage)
    {
        var device = this.FinddFrontFacingCamera();
        if(m_texture==null)
        {
            m_texture = new WebCamTexture(device.name);
            m_texture.Play();
            ////Debug.Log("==============================");
            ////Debug.Log("width:" + m_texture.width + " height:" + m_texture.height);
            ////Debug.Log("reqWidth:" + m_texture.requestedWidth + "reqHeight:" + m_texture.requestedHeight);
            ////Debug.Log("requestedFPS:" + m_texture.requestedFPS);
            rawImage.texture = m_texture;

        }
        else
        {
            m_texture.Play();
        }
    }

    void Update()
    {
        if(Application.isEditor)
        {
            return;
        }
        // rawImage 使用照相机返回贴图的尺寸
        moniter.SetNativeSize();

        // rawImage 旋转角修正
        var t = moniter.texture as WebCamTexture;
        var v = new Vector3();
        v.z = -t.videoRotationAngle;
        moniter.rectTransform.localEulerAngles = v;

        // rawImage 需要缩放以铺满屏幕
        var rawImageFinalSize = new Vector2(t.height, t.width);
        var canvas = UIEngine.Canvas;
        var canvasTransform = canvas.GetComponent<RectTransform>();
        var canvasSize = canvasTransform.sizeDelta;
        var scale = ImitateUtil.GetScaleLetRectACoverRectB(rawImageFinalSize, canvasSize);
        moniter.rectTransform.localScale = new Vector2(scale, scale);

        // 安卓修正
        if (Application.platform == RuntimePlatform.Android)
        {
            var s = moniter.rectTransform.localScale;
            moniter.rectTransform.localScale = new Vector2(s.x, -s.y);
        }

        //return;

        if (m_need_adjust)
        {
            m_down_time -= Time.deltaTime;
            if(m_down_time<=0.0f)
            {
                m_down_time = -1.0f;
                button_go.SetActive(true);
            }
            m_indev_tick -= Time.deltaTime;
            if (m_indev_tick <= 0.0f)
            {
                m_indev_tick = 2.0f;
                //Shot();
            }
        }
    }



    public override void OnNavigatedTo()
    {
        m_need_adjust = true;
        if (Application.isEditor)
        {
            button_go.SetActive(true);
        }
        else
        {
            button_go.SetActive(false);
            CreateCameraTextureForRawImage(moniter);
            EmotionSDK.SetDirectionCheck(false);
            EmotionSDK.SetEmotionCheck(false);
            m_down_time = 3.0f;
        }



        //button_go.SetActive(true);
    }



    public void Shot()
    {
    
        if (Application.platform != RuntimePlatform.OSXEditor)
        {

            if (m_check_lock)
            {
                Debug.Log("AI LOCKING");
                return;
            }
            var t = moniter.texture as WebCamTexture;
            var texture = ImitateUtil.OrientQuick(t, t.videoRotationAngle);
            CheckFace(texture);
        }
        else
        {
            return;
            // 如果是测试模式，使用源图片作为输入图片
           // input = ImitateManager.origin;
        }

    }

    


    public void CheckFace(Texture2D input)
    {
        m_check_lock = true;
        var inputBase64 = ImitateUtil.TextureToBase64(input);
        EmotionSDK.CheckAsync(inputBase64).ContinueWith(task => {
            m_check_lock = false;
            if (task.Result.Count<=0)
            {
                return;
            }
            float x_dis=task.Result[0].rightMouthCornerPos.x - task.Result[0].leftMouthCornerPos.x;
            float y_dis = task.Result[0].leftMouthCornerPos.y - task.Result[0].leftEyesPos.y;
            float _offset = Mathf.Abs(y_dis - x_dis);
            HotDotData.MinOffset = Mathf.Min(_offset, HotDotData.MinOffset);
            HotDotData.MaxOffset = Mathf.Max(_offset, HotDotData.MaxOffset);
            Debug.Log("Min:" + HotDotData.MinOffset + " Max:" + HotDotData.MaxOffset);

        });
    }
    public void DebugNor()
    {
        if(Application.isEditor)
        {
            //var t = m_local_tex;
            //var texture = ImitateUtil.OrientQuick(t, t.videoRotationAngle);
            ////Debug.Log("原图大小" + texture.height + " " + texture.width);
            // texture = ImitateUtil.CompassPic(texture,2);
            ////Debug.Log("压图大小" + texture.height + " " + texture.width);
            /// 
            // 预览修正后的贴图
            var texture = ImitateUtil.OrientQuickDebug(m_local_tex);
            moniter_ref.texture = m_local_tex;
            //CheckFace(texture);
        }
        else
        {
            var t = moniter.texture as WebCamTexture;
            var texture = ImitateUtil.OrientQuick(t, t.videoRotationAngle);
            ////Debug.Log("原图大小" + texture.height + " " + texture.width);
            // texture = ImitateUtil.CompassPic(texture,2);
            ////Debug.Log("压图大小" + texture.height + " " + texture.width);
            /// 
            // 预览修正后的贴图
            moniter_ref.texture = texture;
            CheckFace(texture);
        }

    }
    public void DebugCom()
    {
        if (Application.isEditor)
        {
            //var t = m_local_tex;
            //var texture = ImitateUtil.OrientQuick(t, t.videoRotationAngle);
            ////Debug.Log("原图大小" + texture.height + " " + texture.width);
            // texture = ImitateUtil.CompassPic(texture,2);
            ////Debug.Log("压图大小" + texture.height + " " + texture.width);
            /// 
            // 预览修正后的贴图
           // moniter.texture = m_local_tex;
            var texture = ImitateUtil.OrientQuickDebug(m_local_tex);
            texture.Apply();
            moniter_ref.texture = texture;
            //CheckFace(texture);
        }

        else
        {
            var t = moniter.texture as WebCamTexture;
            Debug.Log("原图大小" + t.height + " " + t.width);
            var texture = ImitateUtil.OrientQuick(t, t.videoRotationAngle);
            Debug.Log("压图大小" + texture.height + " " + texture.width);
            ////Debug.Log("压图大小" + texture.height + " " + texture.width);
            // 预览修正后的贴图
            CheckFace(texture);
            texture.Apply();
            if (moniter_ref.texture == null)
            {
                moniter_ref.texture = texture;
            }
        }

    }

    public RawImage moniter;
    public RawImage moniter_ref;
    WebCamTexture m_texture;

    public GameObject button_go;
    public Text OutLabel;

    public Texture2D m_local_tex;


  
  


    float m_indev_tick = 0.5f;
    bool m_need_adjust = true;


    float m_down_time = 3.0f;

    volatile bool m_check_lock = false;

    // float mc_indev_time = 0.5f;
}
