using System.Collections.Generic;
using UnityEngine;

public class SmileRankPage : SimplePage
{
    public void OnClose()
    {
        SmileProject.Instance.PlaySe("smile_click");
        SimplePageManager.Instance.Show ("SmileMainPage");
    }
    public override void Refresh()
    {
        StatusBarManager.BarStyle (4);
        StatusBarManager.Show (true);

        if (SmileProject.Instance.RankList.Count == 0)
        {
            for (int i = 0; i < 7; ++i)
            {
                HotDotRankInfo _unit_info = new HotDotRankInfo();
                _unit_info.name = "测试用户" + (i + 1) + "" + Random.Range(1, 100);
                _unit_info.pid = i % 5;
                _unit_info.num = Random.Range(60, 300);
                SmileProject.Instance.RankList.Add(_unit_info);

            }

            for (int i = 0; i < 7; i++)
            {
                GameObject _go = GameObject.Instantiate(UnitPref);
                _go.transform.parent = UnitRoot.transform;
                _go.transform.localScale = UnitPref.transform.localScale;
                _go.transform.localPosition = new Vector3(0, i * (-130), 0);
                _go.SetActive(true);
                SmileRankUnit _rank_unit = _go.GetComponent<SmileRankUnit>();
                m_rank_list.Add(_rank_unit);
            }
            SmileProject.Instance.RankList[0].name = "爱心少女";
            SmileProject.Instance.RankList[0].num = 1200;
            SmileProject.Instance.RankList[0].pid = 1;
            m_self_info = SmileProject.Instance.RankList[0];
        }
        m_self_info.num = SmileProject.Instance.GetSmilePoints();
   

        SmileProject.Instance.RankList.Sort(HotDotData._compass);
    
        for (int i = 0; i <7; ++i)
        {
            HotDotRankInfo _info = SmileProject.Instance.RankList[i];
            m_rank_list[i].SetInfo(SpriteList[_info.pid], _info.name, _info.num);
        }
    }
    public GameObject UnitPref;
    public GameObject UnitRoot;
    List<SmileRankUnit> m_rank_list = new List<SmileRankUnit>();
    public Sprite[] SpriteList;
    HotDotRankInfo m_self_info;
}
