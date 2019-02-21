using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotRankPage : Page
{
    // Start is called before the first frame update
    void Start()
    {
         
    }

    public override void OnNavigatedTo()
    {
        if (m_rank_list.Count==0)
        {
            for (int i = 0; i < 8; ++i)
            {
                HotDotRankInfo _unit_info = new HotDotRankInfo();
                _unit_info.name = "测试用户" + (i + 1) + "" + Random.Range(1, 100);
                _unit_info.pid = i % 5;
                _unit_info.num = Random.Range(2, 10);
                HotDotData.RankList.Add(_unit_info);

            }

            for (int i = 0; i < 8; i++)
            {
                GameObject _go = GameObject.Instantiate(UnitPref);
                _go.transform.parent = UnitRoot.transform;
                _go.transform.localScale = UnitPref.transform.localScale;
                _go.transform.localPosition = new Vector3(0, i * (-105), 0);
                _go.SetActive(true);
                HotRankUnit _rank_unit = _go.GetComponent<HotRankUnit>();
                m_rank_list.Add(_rank_unit);
            }
        }
   


        HotDotData.RankList.Sort(HotDotData._compass);
        for(int i=0;i<8;++i)
        {
            HotDotRankInfo _info = HotDotData.RankList[i];
            m_rank_list[i].SetInfo(i + 1, SpriteList[_info.pid], _info.name,_info.num);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBack()
    {
        HotDogRoot.GetSingle().PlaySe("dianji");
        UIEngine.Back();
    }
    public void OnShop()
    {
        HotDogRoot.GetSingle().PlaySe("dianji");
        ImitateUIService.Dialog("功能暂未开放");
    }
    public GameObject UnitPref;
    public GameObject UnitRoot;
    List<HotRankUnit> m_rank_list = new List<HotRankUnit>();
    public Sprite[] SpriteList;
}
