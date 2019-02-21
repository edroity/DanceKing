using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImitateRankPage : Page
{
    public ImitateRankRow[] rowList = new ImitateRankRow[0]; 


    public void OnButton(string msg)
    {
        if(msg == "back")
        {
            UIEngine.Back();
        }
        else if(msg == "clean")
        {
            CleanRank();
        }
        else if(msg == "shop")
        {
            GoShop();
        }
    }

    public void GoShop()
    {
        ImitateUIService.Dialog("功能暂未开放");
    }

    public void CleanRank()
    {
        var list = ImitateRankManager.IORankData;
        list.Clear();
        ImitateRankManager.IORankData = list;
        ImitateRankManager.Init();
        ImitateUIService.Dialog("Developer: 排行榜已清空");
    }

    public override void OnNavigatedTo()
    {
        var list = ImitateRankManager.IORankData;
        for(int i = 0; i < 6; i++)
        {
            var control = rowList[i];
            if(i < list.Count)
            {
                var data = list[i];
                control.PlayerName = data.name;
                control.Score = data.score;
                var iconName = data.icon;
                var iconSprite = ImitateUtilResManager.Load<Sprite>(iconName);
                control.Icon = iconSprite;
                var rank = i + 1;
                control.Rank = rank;
                control.gameObject.SetActive(true);
            }
            else
            {
                control.gameObject.SetActive(false);
            }
        }
    }
}
