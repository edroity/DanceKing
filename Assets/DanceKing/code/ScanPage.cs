using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanPage : Page
{
    public override void OnNavigatedTo()
    {
        base.OnNavigatedTo();
        EmotionSDK.InitDetection();
        EmotionSDK.OpenDetection();
    }

    public override void OnNavigatedFrom()
    {
        EmotionSDK.CloseDetection();
    }
}
