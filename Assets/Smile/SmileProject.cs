using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SmileProject : MonoBehaviour
{
    public CanvasScaler Root;
    public Animation StartupAnim;
    //public Animation FlowerAnim;
    //public GameObject FlowerGo;
    public Camera Camera;

    public List<HotDotRankInfo> RankList = new List<HotDotRankInfo>();

    float m_dance_tick = -1.0f;

    static SmileProject mInstance;
    public static SmileProject Instance {
        get {
            if (mInstance == null) {
                mInstance = GameObject.Find ("Main Camera").GetComponent<SmileProject> ();
            }
            return mInstance;
        }
    }

    private Lutify mLutify = null;
    public Lutify LutifyComp {
        get {
            if (mLutify == null)
                mLutify = gameObject.GetComponent<Lutify> ();
            return mLutify;
        }
    }

    int mSmilePoints = 1200;
    public int GetSmilePoints () {  
        return mSmilePoints;
    }
    public void AddScore (int score) {
        mSmilePoints += score;
    }
    public void DonateScore (int score) {
        mSmilePoints -= score;
    }
    // Use this for initialization
    public void Start()
    {
        StaticData.Init();
        SimplePageManager.Instance.Show("SmileMainPage");
       // LutifyComp.Blend = 0;
       // FlowerAnim.Play("FlowerFadeIn");
        StartupAnim.Play();
        m_dance_tick = 3.0f;
    }



    //void logInfo()
    //{
    //    float ratio = (float)backCam.width / (float)backCam.height;
    //    float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
    //    int orient = -backCam.videoRotationAngle;
    //    Debug.LogFormat ("ratio: {0}\nwidth: {1} height: {2}\nscaleY: {3} orient: {4}", ratio, backCam.width, backCam.height, scaleY, orient);
    //}


    // Update is called once per frame
    private void Update()
    {

        if(m_dance_tick>0.0f)
        {
            m_dance_tick -= Time.deltaTime;
            if(m_dance_tick<=0.0f)
            {
                m_dance_tick = -1.0f;
                Debug.Log("FlowerDance");
               // FlowerAnim.CrossFade("FlowerDance");
            }
        }

    }

 

    //void UpdateReviewRegion(Texture tex)
    //{
    //    int orient0 = backCam.videoRotationAngle;
    //    bool verticalRot = orient0 != 0 && orient0 != 180;
    //    ReviewRawIamge.texture = tex;
    //    if (verticalRot)
    //        ReviewRawIamge.gameObject.GetComponent<AspectRatioFitter> ().aspectRatio = (float)cachedHeight / cachedWidth;
    //    else
    //        ReviewRawIamge.gameObject.GetComponent<AspectRatioFitter> ().aspectRatio = (float)cachedWidth / cachedHeight;
    //    ReviewRawIamge.color = Color.white;
    //}



    void HandleBase64Image(string base64)
    {
        EmotionSDK.SetEmotionCheck (true);
        EmotionSDK.SetDirectionCheck (true);
        // 检测表情
        EmotionSDK.CheckAsync (base64).ContinueWith (task => {

            // 任务返回一个字典, 使用 task.Result 访问
            Debug.Log ($"[C#] complete");
            var faceList = task.Result;
            var faceCount = faceList.Count;
            Debug.Log ($"face count: {faceCount}");
        });
    }

    public void OnButton_SelectPhoto()
    {
        Photo.SelectAsync ().ContinueWith (task => {
            var base64 = task.Result;
            HandleBase64Image (base64);
        });
    }

    public void OnButton_Shot()
    {
        // TODO: 需要将拍照代码移动到这里...
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
        Debug.Log("Play:" + _name);
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
