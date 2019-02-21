using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EmotionInfo
{
    public string name;
    public float score;
}
public class SmileResultPage : SimplePage
{
    public static int _compass(EmotionInfo _l, EmotionInfo _r)
    {
        if (_l.score > _r.score)
        {
            return -1;
        }
        else if (_l.score < _r.score)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void SetInfo (int score, Texture2D tex,List<float> emotion) {
        Score = score;
        ScoreText.text = score.ToString ();
        SmileProject.Instance.AddScore (score);
        m_emotion = emotion;
        Review.texture = tex;
        m_tex2d = tex;
        Review.gameObject.GetComponent<AspectRatioFitter> ().aspectRatio = (float)tex.width / tex.height;

        m_emotion = emotion;
        Review.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
        List<EmotionInfo> _info_list = new List<EmotionInfo>();
        for (int i = 0; i < m_emotion.Count; ++i)
        {
            EmotionInfo _einfo = new EmotionInfo();
            _einfo.name = ImitateTranslateService.GetTextByIndex(i);
            _einfo.score = m_emotion[i];
            _info_list.Add(_einfo);
        }
        _info_list.Sort(_compass);
        if (m_detail_list.Count == 0)
        {
            for (int i = 0; i < 5; ++i)
            {
                GameObject _u = GameObject.Instantiate(unit_pref);
                SmileDetailUnit _unit = _u.GetComponent<SmileDetailUnit>();
                _unit.SetInfo(_info_list[i].name, (int)(_info_list[i].score * 100));
                _u.transform.parent = m_root;
                _u.transform.localScale = Vector3.one;
                _u.transform.localPosition = new Vector3(0, -89 * i, 0);
                _u.SetActive(true);
                m_detail_list.Add(_unit);
            }
        }
        else
        {
            for (int i = 0; i < 5; ++i)
            {
                m_detail_list[i].SetInfo(_info_list[i].name, (int)(_info_list[i].score * 100));
                if (_info_list[i].score >= 0.1f)
                {
                    m_detail_list[i].gameObject.SetActive(true);
                }
                else
                {
                    m_detail_list[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public override void Refresh()
    {
        StatusBarManager.Show (false);
    }

    public void OnDonate () {
        SmileProject.Instance.PlaySe("smile_donate");
        SimplePageManager.Instance.Show("SmileMapPage");
     
    }
    public void OnDetail()
    {
        SmileProject.Instance.PlaySe("smile_click");
        ResultRoot.SetActive(false);
        DetailRoot.SetActive(true);
    }
    public void OnShare()
    {
        SmileProject.Instance.PlaySe("smile_click");
        ResultRoot.SetActive(true);
        DetailRoot.SetActive(false);
    }

    public Text ScoreText;
    public RawImage Review;

    public int Score;
    List<float> m_emotion;
    Texture2D m_tex2d;
    public GameObject ResultRoot;
    public GameObject DetailRoot;

    public GameObject unit_pref;

    List<SmileDetailUnit> m_detail_list = new List<SmileDetailUnit>();
    public Transform m_root;
}
