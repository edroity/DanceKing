using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImitateStart : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        StaticData.Init();
        ImitateDecoImageManager.Init();
        ImitateRankManager.Init();
        UIEngine.Init();
        UIEngine.Forward<ImitateMainPage>();
        AudioManager.PlayBgm("main");
    }

}
