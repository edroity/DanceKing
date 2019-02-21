using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ImitateSharePage : Page
{
    private Animation _animation;
    private Animation Animation
    {
        get
        {
            if(_animation == null)
            {
                _animation = GetComponent<Animation>();
            }
            return _animation;
        }
    }

    public void OnButton(string msg)
    {
        if(msg == "save")
        {
            SavePhotoAsync();
        }
        else if(msg == "close")
        {
            CloseAsync();
        }
        else if(msg == "weixin")
        {
            Share();
        }
        else if(msg == "weibo")
        {
            Share();
        }
        else if(msg == "qqzone")
        {
            Share();
        }
    }

    private void Share()
    {
        // get image on result page
        var resultPage = UIEngine.FindInStack<ImitateResultPage>();
        if(resultPage == null)
        {
            return;
        }
        var texture = resultPage.GetImage();
        // navigate to display page
        var page = UIEngine.Forward<ImitateShareDisplayPage>();
        page.SetImage(texture);
    }

    private void NotImpletementYet()
    {
        ImitateUIService.Dialog("功能暂未开放");
    }

    public async void CloseAsync()
    {
        await PlayFadeOutAsync();
        var top = UIEngine.Top;
        if(top.name == nameof(ImitateSharePage))
        {
            UIEngine.Back();
        }
    }

    public void PlayFadeIn()
    {
        Animation.Play("ImitateSharePage_FadeIn");
    }
    
    TaskCompletionSource<bool> fadeOut_tcs;
    public Task PlayFadeOutAsync()
    {
        if(fadeOut_tcs != null)
        {
            return fadeOut_tcs.Task;
        }
        fadeOut_tcs = new TaskCompletionSource<bool>();
        Animation.Play("ImitateSharePage_FadeOut");
        return fadeOut_tcs.Task;
    }

    public void PlayFadeOutComplete()
    {
        fadeOut_tcs.SetResult(true);
        fadeOut_tcs = null;
    }

    private async void SavePhotoAsync()
    {
        // get image on result page
        var resultPage = UIEngine.FindInStack<ImitateResultPage>();
        if(resultPage == null)
        {
            return;
        }
        var texture = resultPage.GetImage();
        var base64 = ImitateUtil.TextureToBase64(texture);
        await Photo.SaveAsync(base64);
        ImitateUIService.Dialog("已保存");
    }
}
