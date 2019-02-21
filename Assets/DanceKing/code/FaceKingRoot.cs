using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceKingRoot : MonoBehaviour
{
    static FaceKingRoot m_single = null;
    public static FaceKingRoot GetSingle()
    {
        if (m_single == null)
        {
            m_single = GameObject.FindObjectOfType<FaceKingRoot>();
        }
        return m_single;

    }
    // Start is called before the first frame update
    void Start()
    {
        UIEngine.Init();
        UIEngine.Forward<KingStartPage>();
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void PlaySe(string _name)
    {
        AudioClip _ac = GetClip(_name);
        if (_ac != null)
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
            if (m_bgm_source.clip != null)
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
        foreach (AudioClip _ac in mClips)
        {
            if (_ac.name == _name)
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
