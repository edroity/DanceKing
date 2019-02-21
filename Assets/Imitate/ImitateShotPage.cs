using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public partial class ImitateShotPage : Page
{
    bool isFronCamera;

    private WebCamDevice FindAnotherCamera(string nowDevice)
    {
        // find camera
        var deviceList = WebCamTexture.devices;
        if (deviceList.Length == 0) 
        {
            throw new Exception("No camera found");
        }
        WebCamDevice? device = null;
        foreach(var d in deviceList)
        {
            if(d.name != nowDevice)
            {
                device = d;
            }
        }
        if(device == null)
        {
            throw new Exception("No FrontFacing camera found");
        }
        return device.Value;
    }

     private WebCamDevice FindFrontFacingCamera()
    {
        // find camera
        var deviceList = WebCamTexture.devices;
        if (deviceList.Length == 0) 
        {
            throw new Exception("No camera found");
        }
        WebCamDevice? device = null;
        foreach(var d in deviceList)
        {
            if(d.isFrontFacing)
            {
                device = d;
            }
        }
        if(device == null)
        {
            throw new Exception("No FrontFacing camera found");
        }
        return device.Value;
    }

    private void CreateCameraTextureForRawImage(RawImage rawImage)
    {
        var device = this.FindFrontFacingCamera();
        var texture = new WebCamTexture (device.name);
        texture.Play();
        rawImage.texture = texture;
        this.isFronCamera = true;
    }

    void Update()
    {
        // rawImage 使用照相机返回贴图的尺寸
        this.RawImage_moniter.SetNativeSize();

        // rawImage 旋转角修正
        var t = this.RawImage_moniter.texture as WebCamTexture;
        var v = new Vector3();
        v.z = -t.videoRotationAngle;
        this.RawImage_moniter.rectTransform.localEulerAngles = v;

        // rawImage 需要缩放以铺满屏幕
        var rawImageFinalSize = new Vector2(t.height, t.width);
        var canvas = UIEngine.Canvas;
        var canvasTransform = canvas.GetComponent<RectTransform>();
        var canvasSize = canvasTransform.sizeDelta;
        var scale = ImitateUtil.GetScaleLetRectACoverRectB(rawImageFinalSize, canvasSize);
        this.RawImage_moniter.rectTransform.localScale = new Vector2(scale, scale);

        // 镜像修正
        if(!this.isFronCamera)
        {
            var s = this.RawImage_moniter.rectTransform.localScale;
            this.RawImage_moniter.rectTransform.localScale = new Vector2(s.x , -s.y);
        }

        // 安卓修正
        if(Application.platform == RuntimePlatform.Android)
        {
            var s = this.RawImage_moniter.rectTransform.localScale;
            this.RawImage_moniter.rectTransform.localScale = new Vector2(s.x , -s.y);
        }

    }

    public void OnButton(string msg)
    {
        if(msg == "shot")
        {
            Shot();
        }
        else if(msg == "close")
        {
            UIEngine.BackTo<ImitateMainPage>();
        }
        else if(msg == "change_device")
        {
            ChangeDevice(this.RawImage_moniter);
        }
        else if(msg == "use_album")
        {
            UseAlbumAsync();
        }
        else if(msg == "cheat")
        {
            var input = ImitateManager.origin;
            ImitateManager.input = input;
            var page = UIEngine.Forward<ImitateResultPage>();
            page.SetImage(input);
            page.PlayFlash();
        }
    }

    private void ChangeDevice(RawImage rawImage)
    {
        var t = rawImage.texture as WebCamTexture;
        var nowDevice = t.deviceName;
        var device = this.FindAnotherCamera(nowDevice);
        var texture = new WebCamTexture (device.name);
        texture.Play();
        rawImage.texture = texture;
        this.isFronCamera = !this.isFronCamera;
        Debug.Log($"isFronCamera: {isFronCamera}");
    }

    private void Shot()
    {
        Texture2D input;
        if(Application.platform != RuntimePlatform.OSXEditor)
        {
            var t = this.RawImage_moniter.texture as WebCamTexture;
            var texture = ImitateUtil.Orient(t, t.videoRotationAngle, this.isFronCamera);
            // 预览修正后的贴图
            RawImage_fixed.texture = texture;
            input = texture;
        }
        else
        {
            // 如果是测试模式，使用源图片作为输入图片
           input = ImitateManager.origin;
        }
        ImitateManager.input = input;
        var page = UIEngine.Forward<ImitateResultPage>();
        page.SetImage(input);
        page.PlayFlash();
    }

    private async void UseAlbumAsync()
    {
        var base64 = await Photo.SelectAsync();
        var texture = ImitateUtil.Base64ToTexture(base64);
        ImitateManager.input = texture;
        var page = UIEngine.Forward<ImitateResultPage>();
        page.SetImage(texture);
        page.PlayFlash();
    }

    public override void OnNavigatedTo()
    {
        CreateCameraTextureForRawImage(this.RawImage_moniter);
    }
}
