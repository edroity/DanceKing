using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImitateMainPage : Page
{

    public void OnButton(string msg)
    {
        if(msg == "play_with_friend")
        {
            var page = UIEngine.Forward<ImitateReadyPage>();
            page.Title = "好友同玩";
        }
        else if(msg == "rank")
        {
            UIEngine.Forward<ImitateRankPage>();
        }
        else if(msg == "link")
        {
            var link = GameManifestManager.Get("link");
            Application.OpenURL(link);
        }
        else if(msg == "play")
        {
            var page = UIEngine.Forward<ImitateReadyPage>();
            page.Title = "闯关模式";
        }
    }
}
