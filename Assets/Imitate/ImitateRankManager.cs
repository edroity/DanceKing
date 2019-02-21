using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomLitJson;

public static class ImitateRankManager
{
    public static void Init()
    {
        // 如果排行榜中没有数据，则写入初始数据
        var list = IORankData;
        if(list.Count == 0)
        {
            for(int i = 0; i < 6; i++)
            {
                var icon = $"icon{Random.Range(1, 5)}";
                var score = Random.Range(10, 70);
                var name = RandomName(i);
                var data = new RankRowData
                {
                    name = name,
                    icon = icon,
                    score = score,
                };
                list.Add(data);
            }
        }
        SortList(list);
        IORankData = list;
    }

    private static void SortList(List<RankRowData> list)
    {
        list.Sort((a, b)=>{
            return b.score - a.score;
        });
    }

 
    public static string RandomName(int index)
    {
        var list = new string[]{"王老吉", "老司机", "原味咖啡", "waste", "康纳", "老猫", "KFC", "大西瓜", "bing", "桃子"};
        var name = list[index];
        return name;

    }

    public static void IOInsertRow(RankRowData data)
    {
        var list = IORankData;
        list.Add(data);
        SortList(list);
        while(list.Count > 10)
        {
            list.RemoveAt(list.Count - 1);
        }
        IORankData = list;
    }

    public static List<RankRowData> IORankData
    {
        set
        {
            var jd = new Dictionary<string, List<RankRowData>>();
            jd["list"] = value;
            var json = JsonMapper.Instance.ToJson(jd);
            PlayerPrefs.SetString("ImitateRankManager.list", json);
        }
        get
        {
            var json = PlayerPrefs.GetString("ImitateRankManager.list", "{\"list\":[]}");
            var dic = JsonMapper.Instance.ToObject<Dictionary<string, List<RankRowData>>>(json);
            var list = dic["list"] as List<RankRowData>;
            return list;
        }
    }
}

public class RankRowData
{
    public string icon;
    public string name;
    public int score; 
}
