using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HotDotRankInfo
{
    public string name = "大卫";
    public int num = 3;
    public int pid = 1;


}


public static class HotDotData
{
    public static int _compass(HotDotRankInfo _left, HotDotRankInfo _right)
    {
        if(_left.num>_right.num)
        {
            return -1;
        }
        else if(_right.num<_left.num)
        {
            return 1;
        }
        else
        {
            return 0;
        }

    }
    public static int HotDogNum = 0;
    public static List<HotDotRankInfo> RankList = new List<HotDotRankInfo>();
    public static float ShotIndev = 0.3f;
    public static bool NeedAdjust = true;

    public static float MaxOffset = 50.0f;
    public static float MinOffset = 20.0f;

}

public class HotDogRoot : MonoBehaviour
{
    static HotDogRoot m_single=null;
    public static HotDogRoot GetSingle()
    {
        if(m_single==null)
        {
            m_single = GameObject.FindObjectOfType<HotDogRoot>();
        }
        return m_single;
       
    }
    // Start is called before the first frame update
    void Start()
    {
        UIEngine.Init();
        UIEngine.Forward<HotMainPage>();
    }
    

    // Update is called once per frame
    void Update()
    {

    }
    public void PlaySe(string _name)
    {
        AudioClip _ac = GetClip(_name);
        if(_ac!=null)
        {
            m_se_source.PlayOneShot(_ac);
        }
    }
    public void PlayBgm(string _name)
    {
       //Debug.Log("Play:" + _name);
        AudioClip _ac = GetClip(_name);
        if (_ac != null)
        {
            if(m_bgm_source.clip!=null)
            {
                if (m_bgm_source.clip.name == _name)
                {
                    return;
                }
            }
            m_bgm_source.Stop();
            m_bgm_source.clip = _ac;
            m_bgm_source.Play();
        }
    }
    public void StopBgm()
    {
        m_bgm_source.Stop();
    }
    public void ResumeBgm()
    {
        m_bgm_source.Play();
    }
    AudioClip GetClip(string _name)
    {
        foreach(AudioClip _ac in mClips)
        {
            if(_ac.name==_name)
            {
                return _ac;
            }
        }
        return null;
    }

    public AudioClip[] mClips;
    public AudioSource m_bgm_source;
    public AudioSource m_se_source;
}
