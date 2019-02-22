using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingStartPage : Page
{
    public override void OnNavigatedTo()
    {
        base.OnNavigatedTo();
    }

    public void OnGameStart()
    {
        UIEngine.Forward<ScanPage>();
    }

    public void OnHelp()
    {
        //UIEngine.Forward<>();
    }
}
