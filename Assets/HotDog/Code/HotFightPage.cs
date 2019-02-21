using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;
public class HotFightPage : Page
{


    public override void OnNavigatedFrom()
    {
        base.OnNavigatedFrom();
        //EmotionSDK.CloseDetection();
    }
    public override void OnNavigatedTo()
    {
        HotDogRoot.GetSingle().StopBgm();
        m_time = 15.0f;
        TimeLabel.text = "15";
        m_count = 0;
        m_eat_num = 0;
        CountLabel.text = "X0";
        m_eat_tick = mc_eat_indev;
        m_bar.fillAmount = 1.0f;
        Root.SetActive(false);
        m_ready_num = 4;
        ReadyDown();
        m_ready_root.SetActive(true);
        HotDogRoot.GetSingle().PlaySe("daojishi");
        //m_offset_min = HotDotData.MinOffset;
        //m_offset_max = HotDotData.MaxOffset;
        // EmotionSDK.InitDetection();
        //EmotionSDK.OpenDetection();
        if(!Application.isEditor)
        {
            CreateCameraTextureForRawImage();
        }

       
        // m_sicong_ani.CrossFade("rougou_ani_001");



    }// Start is called before the first frame update
     // Update is called once per frame
    private WebCamDevice FinddFrontFacingCamera()
    {

        // find camera
        var deviceList = WebCamTexture.devices;
        if (deviceList.Length == 0)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("No camera found");
            }
            else
            {
                throw new Exception("No camera found");
            }

        }
        WebCamDevice? device = null;
        foreach (var d in deviceList)
        {
            if (d.isFrontFacing)
            {
                device = d;
            }
        }
        if (device == null)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("No FrontFacing camera found");
            }
            else
            {
                throw new Exception("No FrontFacing camera found");
            }

        }
        return device.Value;
    }
    private void CreateCameraTextureForRawImage()
    {
     
        if (m_texture == null)
        {
            var device = this.FinddFrontFacingCamera();
            m_texture = new WebCamTexture(device.name);
            m_texture.Play();
            
            // rawImage 旋转角修正
        
            var v = new Vector3();
            v.z = -m_texture.videoRotationAngle;
            moniter.rectTransform.localEulerAngles = v;
            moniter.texture = m_texture;

            // 安卓修正
            if (Application.platform == RuntimePlatform.Android)
            {
                var s = moniter.rectTransform.localScale;
                moniter.rectTransform.localScale = new Vector2(s.x, -s.y);
            }


            //Debug.Log("width:" + m_texture.width + " height:" + m_texture.height);
            //Debug.Log("reqWidth:" + m_texture.requestedWidth + "reqHeight:" + m_texture.requestedHeight);
            //Debug.Log("requestedFPS:" + m_texture.requestedFPS);
        }
        else
        {
           m_texture.Play();
        }
    }
    void Update()
    {

        if (m_over_time>0.0f)
        {
            m_over_time -= Time.deltaTime;
            if(m_over_time<=0.0f)
            {
                m_over_time = -1.0f;
                UIEngine.Forward<HotSharePage>();
            }
        }
        else
        {
            if (m_ready_time>0.0f)
            {
                m_ready_time -= Time.deltaTime;
                if(m_ready_time<=0.0f)
                {
                    ReadyDown();
                }
            }
            else
            {
                m_time -= Time.deltaTime;
                if (m_time < 0.0f)
                {
                    m_time = 0.0f;
                    Over();
                }
                TimeLabel.text = ((int)m_time).ToString();
                m_bar.fillAmount = (m_time / 15.0f);

                m_indev_tick -= Time.deltaTime;
                if (m_indev_tick <= 0.0f)
                {
                    m_indev_tick = HotDotData.ShotIndev;
                    if (Application.isEditor)
                    {
                        if(UnityEngine.Random.value<0.1f)
                        {
                            m_eat_num++;
                        }
                        //Debug.Log(Time.time+"Editor EatNum ++" + m_eat_num);
                    }
                    else
                    {
                       //Debug.Log("hereA");
                       CheckFace();
                    }

                }

                if(m_eat_num>0)
                {
                    Eat();
                   
                   // Debug.Log(Time.time + "EatNum --" + m_eat_num);
                }


            }

        }






    }

    void Eat()
    {
        m_eat_num--;
        HotDogRoot.GetSingle().PlaySe("jujue");
        m_count++;
        CountLabel.text = "X" + m_count;
        m_sicong_ani.Play("rougou_ani_001");
        //m_sicong_ani.CrossFadeQueued("hot_ani_idle");
    }
    void Over()
    {
        HotDotData.HotDogNum = m_count;
        m_over_time = 1.0f;
        HotDotRankInfo _info = new HotDotRankInfo();
        _info.name = "本地玩家" + HotDotData.RankList.Count + 1;
        _info.num = m_count;
        _info.pid = UnityEngine.Random.Range(1, 5);
        HotDotData.RankList.Add(_info);
    }
    void ReadyDown()
    {
        m_ready_num--;
        if (m_ready_num<=0)
        {
            m_ready_root.SetActive(false);
            Root.SetActive(true);
            HotDogRoot.GetSingle().PlayBgm("battle_mus");
            m_ready_time = -1.0f;
        }
        else
        {
            m_ready_label.text = m_ready_num.ToString();
            m_ready_time = 1.0f;
        }
    }




    public void CheckFace()
    {
       
        //m_eat_num++;

        // return;
       //Debug.Log("Time1:"+Time.time);
 
        var input = ImitateUtil.OrientQuick(m_texture, m_texture.videoRotationAngle);

        //moniter_ref.texture = input;
        //Debug.Log("Time2:" + Time.time);
        // input = ImitateUtil.CompassPic(input,3);
        //Debug.Log("Time3:" + Time.time);

       // Debug.Log("hereB");
        var inputBase64 = ImitateUtil.TextureToBase64(input);
       //Debug.Log("Time4:" + Time.time);

        EmotionSDK.CheckAsync(inputBase64).ContinueWith(task => {
            //Debug.Log("Time5:" + Time.time);

            //Debug.Log("hereC");

           // string _log = "CheckFace " + Time.time;




            float x_dis = task.Result[0].rightMouthCornerPos.x - task.Result[0].leftMouthCornerPos.x;
            float y_dis = task.Result[0].leftMouthCornerPos.y - task.Result[0].leftEyesPos.y;

   

            float _offset = Mathf.Abs(y_dis - x_dis);
            float _raw_offset = _offset;


            HotDotData.MinOffset = Mathf.Min(_offset, HotDotData.MinOffset);
            HotDotData.MaxOffset = Mathf.Max(_offset, HotDotData.MaxOffset);

            //_log += _ofsset + "(" + m_offset_min + "," + m_offset_max + ")";

          
            //Debug.Log("hereD");

            float _value_dis = Mathf.Abs(HotDotData.MaxOffset - _offset) - Mathf.Abs(_offset - HotDotData.MinOffset);

           
            if (_value_dis>5)
            {
                // _log += "闭嘴检测 ";
                Debug.Log("闭嘴检测 Raw:" + _raw_offset + " Dis:" + _value_dis + " OffsetMin:" + HotDotData.MinOffset + " OffsetMax:" + HotDotData.MaxOffset+" "+m_open_mouth);
                if (m_open_mouth)
                {
                    m_open_mouth = false;
                    m_eat_num++;
                   
                }

            }
            else if(_value_dis<-5)
            {
                // _log += "张嘴检测 ";
                Debug.Log("张嘴检测 Raw:" + _raw_offset + " Dis:" + _value_dis + " OffsetMin:" + HotDotData.MinOffset + " OffsetMax:" + HotDotData.MaxOffset+" "+m_open_mouth);
                if (!m_open_mouth)
                {
                    m_open_mouth = true;
                    //m_sicong_ani.Play("hot_ani_open");
                    //m_sicong_ani.PlayQueued("hot_ani_open_idle");
                }
            }
            float _new_max = HotDotData.MaxOffset - Mathf.Max(Mathf.Abs(HotDotData.MaxOffset * 0.1f), 1.0f);
            if (_new_max > HotDotData.MinOffset)
            {
                HotDotData.MaxOffset = _new_max;
            }
            float _new_min = HotDotData.MinOffset + Mathf.Max(Mathf.Abs(HotDotData.MaxOffset * 0.1f), 1.0f);
            if (_new_min < HotDotData.MaxOffset)
            {
                HotDotData.MinOffset = _new_min;
            }

            // Debug.Log(_log);
        });
    }
    public RawImage moniter;
    // public RawImage moniter_ref;
    WebCamTexture m_texture;

    float m_over_time = -1.0f;
    float m_time = 1500.0f;
    int m_count = 0;
    public Text TimeLabel;
    public Text CountLabel;
    float m_eat_tick = -1.0f;
    public float mc_eat_indev = 0.5f;
    public Image m_bar;
    public Animation m_sicong_ani;
    public GameObject Root;
    float m_ready_time =1.0f;
    int m_ready_num = 4;
    public GameObject m_ready_root;
    public Text m_ready_label;







    volatile bool m_open_mouth = false;


    //float m_offset_min = 20;
    //float m_offset_max = 55;

    //float m_current_max = 20;
 

    float m_indev_tick = 0.5f;
    //float mc_indev_time = 0.1f;

    volatile int m_eat_num = 0;
}

