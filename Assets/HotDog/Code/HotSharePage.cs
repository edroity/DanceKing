using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotSharePage : Page
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnRank()
    {
        HotDogRoot.GetSingle().PlaySe("dianji");
        UIEngine.Forward<HotRankPage>();
    }
    public override void OnNavigatedTo()
    {
        share_root.SetActive(false);
        m_dage_tick = Random.Range(1.0f, 5.0f);
        m_num1_label.text = HotDotData.HotDogNum + "根";
        int _offset = Mathf.Clamp(HotDotData.HotDogNum * 6, 30, 95);
        m_num2_label.text = _offset + "%";
        int _max = Mathf.Min(16, HotDotData.HotDogNum);
        for(int i=0;i<_max; ++i)
        {
            GameObject _g=GameObject.Instantiate(m_hot_dog_unit);
            _g.transform.parent = m_hot_dog_root.transform;
            _g.transform.localScale = m_hot_dog_unit.transform.localScale;
            Vector3 _pos_offset = Vector3.zero;
            _pos_offset.x = (i % 4) * 123;
            _pos_offset.y = (i / 4) * (-79);
            _g.transform.localPosition = _pos_offset;
            _g.SetActive(true);
            m_hot_dog_list.Add(_g);
        }
        m_hot_dog_unit.SetActive(false);
        m_hot_dog_root.SetActive(true);
        m_dage_tick = Random.Range(2.0f, 5.0f);
    }
    public override void OnNavigatedFrom()
    {
       for(int i=0;i<m_hot_dog_list.Count;++i)
        {
            GameObject.Destroy(m_hot_dog_list[i]);
        }
        m_hot_dog_list.Clear();
        m_hot_dog_root.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_dage_tick>0.0f)
        {
            m_dage_tick -= Time.deltaTime;
            if (m_dage_tick <= 0.0f)
            {
                m_dage_tick = -1.0f;
                HotDogRoot.GetSingle().PlaySe("dage");
            }
        }
    }
    public void OnShareB()
    {
        HotDogRoot.GetSingle().PlaySe("dianji");
       // UIEngine.BackTo<HotMainPage>();

        share_root.SetActive(true);
        share_ani.Play("HotShareAni");
    }
    public void OnSharePageB()
    {
        UIEngine.BackTo<HotMainPage>();
    }
    public Text m_num1_label;
    public Text m_num2_label;
    float m_dage_tick = -1.0f;
    public GameObject m_hot_dog_unit;
    public GameObject m_hot_dog_root;
    List<GameObject> m_hot_dog_list = new List<GameObject>();
    public GameObject share_root;
    public Animation share_ani;
}
