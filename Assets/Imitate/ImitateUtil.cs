using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public static class ImitateUtil
{
     // 计算一个缩放值，使一个小尺寸能完美覆盖大尺寸
    public static float GetScaleLetRectACoverRectB(Vector2 size1, Vector2 size2)
    {
        var rate1 = size1.x / size1.y;
        var rate2 = size2.x / size2.y;
        if(rate1 > rate2)
        {
            // 按高缩放
            return size2.y / size1.y;
        }
        else
        {
            // 按宽缩放
            return size2.x / size1.x;
        }
    }
    static Color[] _temp_tex = new Color[100];
    static Color[] _temp_tex1 = new Color[100];
    static Color[] _temp_tex2 = new Color[100];
    static Texture2D _temp_tex2d = null;
    static Color32[] _temp32_tex = new Color32[100];
    static Color32[] _temp32_tex1 = new Color32[100];
    static Color32[] _temp32_tex2 = new Color32[100];
    static Texture2D _temp32_tex2d = null;
    // 顺时针旋转图片, 然后镜像处理
    public static Texture2D OrientQuickDebug(Texture2D texture)
    {
        var colors = texture.GetPixels();
        var height = texture.height;
        var width = texture.width;




        int _offset = 2;
        if (height > 2000 || width > 2000)
        {
            _offset = 4;
        }

        int targetHeight_t = height / _offset;
        int targetWidth_t = width / _offset;

        int _head_trime_height = (int)(targetHeight_t * 0.1f);
        //var colors = texutre.GetPixels();

        int final_size = (targetHeight_t - _head_trime_height * 2) * targetWidth_t;

        if (_temp_tex.Length != final_size)
        {
            _temp_tex = new Color[final_size];
            //Debug.Log("temp_tex1重置尺寸:"+ colors.Length);
        }






        for (int i = _head_trime_height; i < targetHeight_t - _head_trime_height; ++i)
        {
            for (int j = 0; j < targetWidth_t; ++j)
            {
                _temp_tex[(i - _head_trime_height) * targetWidth_t + j] = colors[i * targetWidth_t * _offset * _offset + j * _offset];
            }
        }
        height = targetHeight_t - _head_trime_height * 2;
        width = targetWidth_t;


        int index = 0;
        //var result = new Color32[colors.Length];
        if (_temp_tex1.Length != _temp_tex.Length)
        {
            _temp_tex1 = new Color[_temp_tex.Length];
            //Debug.Log("temp_tex1重置尺寸:"+ colors.Length);
        }


        int targetHeight;
        int targetWidth;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                _temp_tex1[index] = _temp_tex[j + i * width];
                index++;
            }
        }
        targetHeight = height;
        targetWidth = width;
  
        if ((_temp_tex2d == null) || (_temp_tex2d.width != targetWidth))
        {
            _temp_tex2d = new Texture2D(targetWidth, targetHeight);
            // Debug.Log("temp_tex2d New");
        }

        //if ((_temp_tex2d == null) || (_temp_tex2d.width != targetWidth))
        //{

        //    // Debug.Log("temp_tex2d New");
        //}

        //   Texture2D _temp_tex2d = new Texture2D(targetWidth, targetHeight);

        // 前置摄像头需要旋转
        // 后置摄像头不用
        _temp_tex2d.SetPixels(_temp_tex1);
        // _temp_tex2d.Apply ();
        return _temp_tex2d;
    }
    public static Texture2D OrientQuick(WebCamTexture texture, int orient) 
    {
        
        var colors = texture.GetPixels();
        var height = texture.height;
        var width = texture.width;




        int _offset = 2;
        if(height>2000||width>2000)
        {
            _offset = 4;
        }
      
        int targetHeight_t = height/ _offset;
        int targetWidth_t = width/ _offset;

        int _head_trime_height = (int)(targetHeight_t * 0.1f);
        //var colors = texutre.GetPixels();

        int final_size = (targetHeight_t- _head_trime_height*2) * targetWidth_t;

        if (_temp_tex.Length != final_size)
        {
            _temp_tex = new Color[final_size];
            //Debug.Log("temp_tex1重置尺寸:"+ colors.Length);
        }






        for (int i = _head_trime_height; i < targetHeight_t-_head_trime_height; ++i)
        {
            for (int j = 0; j < targetWidth_t; ++j)
            {
                _temp_tex[(i- _head_trime_height) * targetWidth_t + j] = colors[i* targetWidth_t * _offset * _offset + j * _offset];
            }
        }
        height = targetHeight_t - _head_trime_height * 2;
        width = targetWidth_t;
  

        int index = 0;
        //var result = new Color32[colors.Length];
        if(_temp_tex1.Length!= _temp_tex.Length)
        {
            _temp_tex1 = new Color[_temp_tex.Length];
            //Debug.Log("temp_tex1重置尺寸:"+ colors.Length);
        }


        int targetHeight;
        int targetWidth;
        switch (orient) {
        case 270:
            for (int j = 0; j < width ; j++) {
                for (int i = height - 1; i >= 0; i--) {
                        _temp_tex1[index] = _temp_tex[j + i * width];
                    index++;
                }
            }
            targetHeight = width;
            targetWidth = height;
            break;
        case 90:
            for (int j = width - 1; j >= 0; j--) {
                for (int i = 0; i < height; i++) {
                        _temp_tex1[index] = _temp_tex[j + i * width];
                    index++;
                }
            }
            targetHeight = width;
            targetWidth = height;
            break;
        case 180:
            for (int i = height - 1; i >= 0; i--) {
                for (int j = width - 1; j >= 0; j--) {
                        _temp_tex1[index] = _temp_tex[j + i * width];
                    index++;
                }
            }
            targetHeight = height;
            targetWidth = width;
            break;
        default:
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                        _temp_tex1[index] = _temp_tex[j + i * width];
                    index++;
                }
            }
            targetHeight = height;
            targetWidth = width;
            break;
        }

        if ((_temp_tex2d == null) || (_temp_tex2d.width != targetWidth))
        {
            _temp_tex2d = new Texture2D(targetWidth, targetHeight);
            // Debug.Log("temp_tex2d New");
        }

        //if ((_temp_tex2d == null) || (_temp_tex2d.width != targetWidth))
        //{

        //    // Debug.Log("temp_tex2d New");
        //}

     //   Texture2D _temp_tex2d = new Texture2D(targetWidth, targetHeight);

        // 前置摄像头需要旋转
        // 后置摄像头不用
        _temp_tex2d.SetPixels(_temp_tex1);
       // _temp_tex2d.Apply ();
        return _temp_tex2d;
    }

    // 镜像图片
    static void Horizen (Color[] colors, int width, int height) {
        if (_temp_tex2.Length != colors.Length)
        {
            _temp_tex2 = new Color[colors.Length];
            Debug.Log("temp_tex2重置尺寸:" + colors.Length);
        }
        int index = 0;
        for (int i = 0; i < height; i++) {
            for (int j = width - 1; j >= 0 ; j--) {
                _temp_tex2[index] = colors[j + i * width];
                index++;
            }
        }
    }


    public static Texture2D Orient(WebCamTexture texture, int orient, bool doMirror)
    {
        _temp32_tex = texture.GetPixels32();
        var height = texture.height;
        var width = texture.width;



        int index = 0;
        //var result = new Color32[colors.Length];
        if (_temp32_tex1.Length != _temp32_tex.Length)
        {
            _temp32_tex1 = new Color32[_temp32_tex.Length];
            //Debug.Log("temp_tex1重置尺寸:"+ colors.Length);
        }


        int targetHeight;
        int targetWidth;
        switch (orient)
        {
            case 270:
                for (int j = 0; j < width; j++)
                {
                    for (int i = height - 1; i >= 0; i--)
                    {
                        _temp32_tex1[index] = _temp32_tex[j + i * width];
                        index++;
                    }
                }
                targetHeight = width;
                targetWidth = height;
                break;
            case 90:
                for (int j = width - 1; j >= 0; j--)
                {
                    for (int i = 0; i < height; i++)
                    {
                        _temp32_tex1[index] = _temp32_tex[j + i * width];
                        index++;
                    }
                }
                targetHeight = width;
                targetWidth = height;
                break;
            case 180:
                for (int i = height - 1; i >= 0; i--)
                {
                    for (int j = width - 1; j >= 0; j--)
                    {
                        _temp32_tex1[index] = _temp32_tex[j + i * width];
                        index++;
                    }
                }
                targetHeight = height;
                targetWidth = width;
                break;
            default:
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        _temp32_tex1[index] = _temp32_tex[j + i * width];
                        index++;
                    }
                }
                targetHeight = height;
                targetWidth = width;
                break;
        }

        if ((_temp32_tex2d == null) || (_temp32_tex2d.width != targetWidth))
        {
            _temp32_tex2d = new Texture2D(targetWidth, targetHeight);
            // Debug.Log("temp_tex2d New");
        }

        //if ((_temp_tex2d == null) || (_temp_tex2d.width != targetWidth))
        //{

        //    // Debug.Log("temp_tex2d New");
        //}

        //   Texture2D _temp_tex2d = new Texture2D(targetWidth, targetHeight);

        // 前置摄像头需要旋转
        // 后置摄像头不用
        if (doMirror)
        {
            Horizen32(_temp32_tex1, targetWidth, targetHeight);
            _temp32_tex2d.SetPixels32(_temp32_tex2);
        }
        else
        {
            _temp32_tex2d.SetPixels32(_temp32_tex1);
        }
        _temp32_tex2d.Apply();
        return _temp32_tex2d;
    }

    static void Horizen32(Color32[] colors, int width, int height)
    {
        if (_temp32_tex2.Length != colors.Length)
        {
            _temp32_tex2 = new Color32[colors.Length];
            Debug.Log("temp_tex2重置尺寸:" + colors.Length);
        }
        int index = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = width - 1; j >= 0; j--)
            {
                _temp32_tex2[index] = colors[j + i * width];
                index++;
            }
        }
    }


    public static string TextureToBase64 (Texture2D texture) 
    {
        byte[] bytes = texture.EncodeToJPG ();
        string enc = Convert.ToBase64String (bytes);
        return enc;
    }
    public static Texture2D Base64ToTexture(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        Texture2D tex2D = new Texture2D(100, 100);
        tex2D.LoadImage(bytes);
        return tex2D;
    }

}