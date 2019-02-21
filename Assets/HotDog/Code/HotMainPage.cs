using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HotMainPage : Page
{
    public override void OnNavigatedFrom()
    {
        base.OnNavigatedFrom();
        //EmotionSDK.CloseDetection();

    }


    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnNavigatedTo()
    {

        HotDogRoot.GetSingle().PlayBgm("mainpage_mus");
        //EmotionSDK.InitDetection();
        //EmotionSDK.OpenDetection();
    }
    public void RankB()
    {
        HotDogRoot.GetSingle().PlaySe("dianji");
        UIEngine.Forward<HotRankPage>();
    }
    public void StartB()
    {
       // HotDotData.ShotIndev = float.Parse(m_input_field.text);
        HotDogRoot.GetSingle().PlaySe("dianji");
        if(HotDotData.NeedAdjust)
        {
            UIEngine.Forward<HotAdjustPage>();
        }
        else
        {
            UIEngine.Forward<HotFightPage>();
        }

        //UIEngine.Forward<HotFightPage>();
    }
    public InputField m_input_field;
}
