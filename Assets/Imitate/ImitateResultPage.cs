using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using CustomLitJson;
using DG.Tweening;


public partial class ImitateResultPage : Page
{
    public enum PageTab
    {
        Score,
        Detail,
    }

    public struct DetailRow
    {
        public string title;
        public float percent;
    }

    public void OnButton(string msg)
    {
        if(msg == "back")
        {
            UIEngine.BackTo<ImitateMainPage>();
        }
        else if(msg == "score_tab")
        {
            //SetupTab(PageTab.Score);
        }
        else if(msg == "detail_tab")
        {
            //SetupTab(PageTab.Detail);
        }
        else if(msg == "next_level")
        {
            UIEngine.BackTo<ImitateReadyPage>();
        }
        else if(msg == "restart")
        {
            UIEngine.BackTo<ImitateReadyPage>();
        }
        else if(msg == "save")
        {
            SavePhotoAsync();
        }
        else if(msg == "share")
        {
            Share();
        }
        else if(msg == "switch_photo")
        {
            SwitchPhoto();
        }
        else if(msg == "pass")
        {
            ImitateLevelManager.levelIndex++;
            UIEngine.BackTo<ImitateReadyPage>();
        }
        else if(msg == "return")
        {
            UIEngine.BackTo<ImitateMainPage>();
        }
    }

    private void Share()
    {
        var page = UIEngine.Forward<ImitateSharePage>();
        page.PlayFadeIn();
    }

    private void SwitchPhoto()
    {
        var nowTexture = RawImage_preview.texture as Texture2D;
        Texture2D target = null;
        if(nowTexture == ImitateManager.input)
        {
            target = ImitateManager.origin;
        }
        else if(nowTexture == ImitateManager.origin)
        {
            target = ImitateManager.input;
        }
        SetImage(target);
    }  

    private async void SavePhotoAsync()
    {
        var texture = RawImage_preview.texture as Texture2D;
        var base64 = ImitateUtil.TextureToBase64(texture);
        await Photo.SaveAsync(base64);
        ImitateUIService.Dialog("已保存");
    }

    public override void OnPush()
    {
        RateAsync();
        SetupTab(PageTab.Score);
    }

    private void PlayScoreAnimation(int score)
    {
        Text_score.text = "0%";
        var value = 0f;
        DOTween.To(()=>{
            return value;
        },v=>{
            value = v;
            var percentInt = (int)(value);
            Text_score.text = $"{percentInt}%";
        }, score, 1f);
    }
    public async void RateAsync()
    {
        Text_score.text = "...";
        CleanDetailData();
        SetupRating();
        try
        {
            var result = await ImitateManager.RateAsync();

            var json = JsonMapper.Instance.ToJson(result);
            Debug.Log(json);

            PlayScoreAnimation(result.score);
            // detail tab
            var dataRowList = ResultToDetailRowList(result);
            SetDetailData(dataRowList);
            // 记录
            var row = new RankRowData()
            {
                name = "你",
                icon = "icon1",
                score = result.score,
            };
            ImitateRankManager.IOInsertRow(row);

            // 是否通关判断
            if(result.score >= 40)
            {
                ImitateLevelManager.levelIndex++;
                if(ImitateLevelManager.levelIndex >=3)
                {
                    ImitateLevelManager.levelIndex = 0;
                    SetupReturn();
                }
                else
                {
                    SetupPass();
                }
                
            }
            else
            {
                SetupNotPass();
            }
            
        }
        catch
        {
            Text_score.text = "0%";
            SetupNotPass();
            throw;
        }
    }

    public void PlayFlash()
    {
        var animation = this.GetComponent<Animation>();
        animation.Play();
    }

    public Texture2D GetImage()
    {
        return RawImage_preview.texture as Texture2D;
    }

    public void SetImage(Texture2D texture)
    {
        // 设置按原始尺寸显示
        RawImage_preview.texture = texture;   
        RawImage_preview.SetNativeSize();

        // 计算 rawImage 需要缩放的尺寸
        var textureSize = new Vector2(texture.width, texture.height);
        var holderSize = PreviewRectHolder.sizeDelta;
        var scale = ImitateUtil.GetScaleLetRectACoverRectB(textureSize, holderSize);
        RawImage_preview.rectTransform.localScale = new Vector2(scale, scale);

        // 对齐显示区域
        var pos = PreviewRectHolder.position;
        RawImage_preview.rectTransform.position = pos;
    }

    public void SetupTab(PageTab tab)
    {
        if(tab == PageTab.Score)
        {
            Button_scoreTab.gameObject.SetActive(false);
            Button_detailTab.gameObject.SetActive(true);
            Tab_score.gameObject.SetActive(true);
            Tab_detail.gameObject.SetActive(false);
        }
        else if(tab == PageTab.Detail)
        {
            Button_scoreTab.gameObject.SetActive(true);
            Button_detailTab.gameObject.SetActive(false);
            Tab_score.gameObject.SetActive(false);
            Tab_detail.gameObject.SetActive(true);
        }
    }

    public void SetDetailData(List<DetailRow> dataList)
    {
        ImitateDetailItemBar1.Title = dataList[0].title;
        ImitateDetailItemBar1.PlayPercentAnimation(dataList[0].percent);
        ImitateDetailItemBar2.Title = dataList[1].title;
        ImitateDetailItemBar2.PlayPercentAnimation(dataList[1].percent);
    }

    public void CleanDetailData()
    {
        ImitateDetailItemBar1.Title = "...";
        ImitateDetailItemBar1.Percent = 0;
        ImitateDetailItemBar2.Title = "...";
        ImitateDetailItemBar2.Percent = 0;
    }

    private List<DetailRow> ResultToDetailRowList(ImitateResult result)
    {
        var ret = new List<DetailRow>();
        foreach(var item in result.itemList)
        {
            var title = ImitateTranslateService.GetTextByIndex(item.index);
            Debug.Log($"title: {item.index} -> {title}");
            var likely = ImitateManager.RateSignleItem01(item.target, item.input);
            Debug.Log($"likely: {item.target} : {item.input} -> {likely}");
            var row = new DetailRow();
            row.title = title;
            row.percent = likely;
            ret.Add(row);
        }
        return ret;
    }

    private void SetupPass()
    {
        ImitateButton_nextLevel.gameObject.SetActive(true);
        ImitateButton_restart.gameObject.SetActive(false);
        ImitateButton_rating.gameObject.SetActive(false);
        ImitateButton_return.gameObject.SetActive(false);
    }

    private void SetupNotPass()
    {
        ImitateButton_nextLevel.gameObject.SetActive(false);
        ImitateButton_restart.gameObject.SetActive(true);
        ImitateButton_rating.gameObject.SetActive(false);
        ImitateButton_return.gameObject.SetActive(false);
    }

    private void SetupRating()
    {
        ImitateButton_nextLevel.gameObject.SetActive(false);
        ImitateButton_restart.gameObject.SetActive(false);
        ImitateButton_rating.gameObject.SetActive(true);
        ImitateButton_return.gameObject.SetActive(false);
    }

    private void SetupReturn()
    {
        ImitateButton_nextLevel.gameObject.SetActive(false);
        ImitateButton_restart.gameObject.SetActive(false);
        ImitateButton_rating.gameObject.SetActive(false);
        ImitateButton_return.gameObject.SetActive(true);
    }
}



