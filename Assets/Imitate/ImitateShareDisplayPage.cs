using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ImitateShareDisplayPage : Page
{
    public void SetImage(Texture2D texture)
    {
        RawImage_preview.texture = texture;
        RawImage_preview.SetNativeSize();

        // 计算 rawImage 需要缩放的尺寸
        var textureSize = new Vector2(texture.width, texture.height);
        var holderSize = PlaceHolder.sizeDelta;
        var scale = ImitateUtil.GetScaleLetRectACoverRectB(textureSize, holderSize);
        RawImage_preview.rectTransform.localScale = new Vector2(scale, scale);
    }

    public void OnButton(string msg)
    {
        if(msg == "close")
        {
            Close();
        }
    }

    private void Close()
    {
        {
            var top = UIEngine.Top;
            if(top.name == nameof(ImitateShareDisplayPage))
            {
                UIEngine.Back();
            }
        }

        {
            var top = UIEngine.Top;
            if(top.name == nameof(ImitateSharePage))
            {
                var page = top as ImitateSharePage;
                page.CloseAsync();
            }  
        }

    }
}
