//ybzuo-dena
using UnityEngine;
using System.Collections.Generic;
using GameCore;
using System.Linq;

public class SoundMgr : MonoSingle<SoundMgr> {
    static SoundMgr _single;

    public static SoundMgr GetSingle() {
        if(_single == null) {
            _single = GameObject.FindObjectOfType<SoundMgr>();
        }
        return _single;
    }
    public AudioListener audioListener;
    class SoundInfo {
        public string sound;
        public float time;
        public bool loop;
        public bool queue;
        public float val;
    }

    public void Awake() {


        Init();

    }
    bool m_init = false;
    void Init() {
        if(m_init) {
            return;
        }
        m_init = true;
        foreach(AudioListener _l in GameObject.FindObjectsOfType(typeof(AudioListener))) {
            GameObject.Destroy(_l);
        }
        GameObject _se_root = new GameObject();
        _se_root.name = "SeRoot";
        _se_root.transform.parent = transform;
        m_se_source = _se_root.AddComponent<AudioSource>();
        m_se_source.playOnAwake = false;


        GameObject _bobao_root = new GameObject();
        _bobao_root.name = "BoBaoRoot";
        _bobao_root.transform.parent = transform;
        m_bobao_source = _bobao_root.AddComponent<AudioSource>();
        m_bobao_source.playOnAwake = false;

        GameObject _long_se_root = new GameObject();
        _long_se_root.name = "LongSeRoot";
        _long_se_root.transform.parent = transform;
        m_l_se_source = _long_se_root.AddComponent<AudioSource>();
        m_l_se_source.loop = false;
        m_l_se_source.playOnAwake = false;

        GameObject _bgm_root1 = new GameObject();
        _bgm_root1.name = "BgmRoot1";
        _bgm_root1.transform.parent = transform;
        m_bgm_source1 = _bgm_root1.AddComponent<AudioSource>();
        m_bgm_source1.loop = true;
        m_bgm_source1.playOnAwake = false;

        GameObject _bgm_root2 = new GameObject();
        _bgm_root2.name = "BgmRoot2";
        _bgm_root2.transform.parent = transform;
        m_bgm_source2 = _bgm_root2.AddComponent<AudioSource>();
        m_bgm_source2.loop = true;

        m_bgm_source2.volume = bg2Val;
        m_bgm_source2.playOnAwake = false;

        audioListener = gameObject.AddComponent<AudioListener>();
        SetBgmVolume(GameCore.Config.MusicVolume);
    }

    void Update() {
        if(m_play_se_dic.Count > 0) {
            var data = m_play_se_dic.GetEnumerator();
            while(data.MoveNext()) {
                data.Current.Value.time -= Time.deltaTime;
            }
        }
        //foreach(SoundInfo s in m_play_se_dic.Values) {
        //    s.time -= Time.deltaTime;
        //}

        for(int i = 0;i < m_ready_se_list.Count;++i) {
            m_ready_se_list[i].time -= Time.deltaTime;
            if(m_ready_se_list[i].time <= 0.0f) {
                if(m_play_se_dic.ContainsKey(m_ready_se_list[i].sound)) {
                    if(m_se_dic[m_ready_se_list[i].sound].length - m_play_se_dic[m_ready_se_list[i].sound].time < 0.01f) {
                        m_remove_se_list.Add(m_ready_se_list[i]);
                        m_end_se_list.Add(m_ready_se_list[i]);
                    } else {
                        m_play_se_dic[m_ready_se_list[i].sound].time = 0f;
                    }
                } else {
                    m_ready_se_list[i].time = m_se_dic[m_ready_se_list[i].sound].length;
                    //					Debug.Log("PlayOneShot:"+m_se_dic[m_ready_se_list[i].sound].name+" "+m_ready_se_list[i].val);
                    m_se_source.PlayOneShot(m_se_dic[m_ready_se_list[i].sound],m_ready_se_list[i].val);
                    m_play_se_dic.Add(m_ready_se_list[i].sound,m_ready_se_list[i]);
                    m_remove_se_list.Add(m_ready_se_list[i]);
                }
            }
        }
        if(m_remove_se_list.Count > 0) {
            for(int i = 0;i < m_remove_se_list.Count;i++)
                m_ready_se_list.Remove(m_remove_se_list[i]);
            //foreach(SoundInfo _si in m_remove_se_list) {
            //    m_ready_se_list.Remove(_si);
            //}
            m_remove_se_list.Clear();
        }

        if(m_play_se_dic.Count > 0) {
            var data = m_play_se_dic.GetEnumerator();
            while(data.MoveNext()) {
                if(data.Current.Value.time <= 0)
                    m_end_se_list.Add(data.Current.Value);
            }
            //foreach(SoundInfo s in m_play_se_dic.Values) {
            //    if(s.time <= 0) m_end_se_list.Add(s);
            //}
        }

        if(m_end_se_list.Count > 0) {
            for(int i = 0;i < m_end_se_list.Count;i++) {
                m_play_se_dic.Remove(m_end_se_list[i].sound);
            }
            //foreach(SoundInfo _si in m_end_se_list) {
            //    m_play_se_dic.Remove(_si.sound);
            //}
            m_idle_se_list.AddRange(m_end_se_list);
            m_end_se_list.Clear();
        }
    }
    public void enableAudioListen(bool enable) {
        AudioListener.volume = enable ? 1.0f : 0.0f;
    }


    private SoundInfo GetSoundInfo {
        get {
            if(m_idle_se_list.Count > 0) {
                SoundInfo idle = m_idle_se_list[0];
                m_idle_se_list.RemoveAt(0);
                return idle;
            } else return new SoundInfo();
        }
    }
    public void stopCurSe() {
        m_se_source.Stop();
    }
    public void stop_se() {
        //Debug.Log(Time.time+"Stop Se");
        Init();
        m_se_source.Stop();
        m_ready_se_list.Clear();
        m_remove_se_list.Clear();
    }

    private void InitOffsetDic() {
        if(m_offset_dic == null) {
            m_offset_dic = new Dictionary<string,float>();
            for(int i = 0;i < StaticDatar.G.m_se_list.Count;++i) {
                JDse _ccse = StaticDatar.G.m_se_list[i];
                if(!m_offset_dic.ContainsKey(_ccse.name)) {
                    m_offset_dic.Add(_ccse.name,_ccse.offset);
                    //					Debug.Log(_ccse.name+" "+_ccse.offset);
                }
            }
        }
    }


    public void Mute(bool isMute) {
        if(isMute) {
            m_offset_all = 0.0f;
        } else {
            m_offset_all = 1.0f;
        }
        RefreshSoundOffset();
    }
    public float GetScale(string _se) {
        InitOffsetDic();
        if(m_offset_dic.ContainsKey(_se)) return m_offset_dic[_se];
        else return 1.0f;
    }

    public void PlaySe(string _se,float _offset,float _delay) {
        PlaySe(_se,_offset,false,_delay,false);
    }
    public void play_commentary_se(string _se,float _offset = -1.0f,bool queue = false,float _delay = 0.0f,bool _loop = false) {
        if(!GameCore.Config.SoundCommentaryOpen)
            return;
        else
            PlaySe(_se,_offset,queue,_delay,_loop);
    }
    public void PreLoadSe(string _se) {
        if(!GameCore.Config.SoundOpen) return;
        if(!m_se_dic.ContainsKey(_se)) {
            ResourceLoad.ResourceHandler.Instance.LoadRes<AudioClip>("Sound/Se/" + _se,delegate(Object obj) {
                if(obj == null) {
                    Debug.LogWarning("Cant find the se " + _se);
                    return;
                }
                AudioClip _ac = obj as AudioClip;
                m_se_dic.Add(_se,_ac);
                _ac.LoadAudioData();
            },false);
        }
    }
    public void PlaySe(string _se,float _offset = -1.0f,bool queue = false,float _delay = 0.0f,bool _loop = false) {
        if(GameCore.Config.SoundOpen) {
            if(m_se_source == null) {
                return;
            }
            //Debug.Log("IsLoaded:"+StaticDatar.IsLoaded);
            if(StaticDatar.IsLoaded)//must check
                InitOffsetDic();
            if(!m_se_dic.ContainsKey(_se)) {
                ResourceLoad.ResourceHandler.Instance.LoadRes<AudioClip>("Sound/Se/" + _se,delegate(Object obj) {
                    if(obj == null) {
                        Debug.LogWarning("Cant find the se " + _se);
                        return;
                    }
                    AudioClip _ac = obj as AudioClip;
                    m_se_dic.Add(_se,_ac);
                    PlaySe(_se,_offset,queue,_delay,_loop);
                },false);
                return;
            }

            if(m_se_dic.ContainsKey(_se)) {
                float val = 1.0f;

                if(m_offset_dic != null && m_offset_dic.ContainsKey(_se)) {
                    val = m_offset_dic[_se];
                }
                if(_offset > 0.0f) {
                    val *= _offset;
                }
                SoundInfo _si = GetSoundInfo;
                _si.sound = _se;
                _si.time = _delay;
                _si.val = val * m_offset_final;
                _si.loop = _loop;
                if(_si.loop) {
                    Debug.Log("Loop se:" + _se);
                }
                _si.queue = queue;
                m_ready_se_list.Add(_si);
            } else {
                Debug.LogWarning("cant load se " + _se);
            }
        } else {
            return;
        }
    }


    public void pause_bgm(int _layer = 1) {
        Init();
        if(_layer == 2) {
            m_bgm_source2.Pause();
        } else {
            m_bgm_source1.Pause();
        }

    }

    public string CurrentBgm1(int layer = 1) {
        Init();
        if(layer == 1)
            return m_current_bgm1;
        else return m_current_bgm2;

    }

    public void play_bgm(int _layer = 1) {
        Init();
        if(_layer == 2) {
            m_bgm_source2.Play();
        } else {
            m_bgm_source1.Play();
        }
    }

    void RefreshSoundOffset() {
        m_offset_final = m_offset_all * m_offset_dyn;
        m_bgm_source1.volume = bg1Val * m_offset_all * m_offset_dyn;
        m_bgm_source2.volume = bg2Val * m_offset_all * m_offset_dyn;
        m_l_se_source.volume = seVal * m_offset_all * m_offset_dyn;
        m_se_source.volume = seVal * m_offset_all * m_offset_dyn;
        m_bobao_source.volume = bbVal * m_offset_all * m_offset_dyn;
    }


    public void ScaleDownBgm() {
        m_offset_all = 0.01f;
        RefreshSoundOffset();
    }

    public void ScaleUpBgm() {
        m_offset_all = 1.0f;
        RefreshSoundOffset();
    }
    public void SetBgmVolume(float volume) {
        m_offset_dyn = volume;
        RefreshSoundOffset();
    }

    public void stop_bgm(int _layer = 1) {
        //		Debug.Log("stop_bgm");
        if(_layer == 2) {
            m_bgm_source2.Stop();
            m_bgm_source2.clip = null;
            m_current_bgm2 = "";
        } else {
            m_bgm_source1.Stop();
            m_bgm_source1.clip = null;
            m_current_bgm1 = "";
        }

    }

    public void play_long_se(string se) {
        if(!GameCore.Config.SoundOpen) return;
        if(!m_se_dic.ContainsKey(se)) {
            ResourceLoad.ResourceHandler.Instance.LoadRes<AudioClip>("Sound/Se/" + se,delegate(Object obj) {
                if(obj == null) {
                    Debug.LogWarning("Cant find the se " + se);
                    return;
                }
                AudioClip _ac = obj as AudioClip;
                m_se_dic.Add(se,_ac);
                play_long_se(se);
            },false);
            return;
        }
        StopLongSe();
        float val = seVal;
        if(m_offset_dic != null && m_offset_dic.ContainsKey(se)) {
            val = m_offset_dic[se];
        }
        //		Debug.Log("PlayLongSe:"+se+" "+val);
        m_l_se_source.volume = val * m_offset_all * m_offset_dyn;
        m_l_se_source.clip = m_se_dic[se];
        m_l_se_source.Play();

    }

    public void StopLongSe() {
        m_l_se_source.Stop();
        m_l_se_source.clip = null;
    }

    public void play_bgm(string _bgm,int _layer = 1,string defaultPath = "") {
        //Debug.Log("play bgm:"+_bgm);
        if(!m_bgm_dic.ContainsKey(_bgm)) {
            ResourceLoad.ResourceHandler.Instance.LoadRes<AudioClip>(!string.IsNullOrEmpty(defaultPath) ? defaultPath : "Sound/Bgm/" + _bgm,delegate(Object obj) {
                if(obj == null) {
                    Debug.LogWarning("Cant find the bgm " + _bgm);
                    return;
                }
                AudioClip _ac = obj as AudioClip;
                m_bgm_dic.Add(_bgm,_ac);
                play_bgm(_bgm,_layer,defaultPath);
            },false);
            return;
        }
        if(StaticDatar.IsLoaded) {
            InitOffsetDic();
        }
        float val = _layer == 2 ? bg2Val : bg1Val;

        if(val > 0 && m_offset_dic != null && m_offset_dic.ContainsKey(_bgm)) {
            val = m_offset_dic[_bgm];
        }
        val *= m_offset_final;
        if(_layer == 2) {
            if(m_bgm_dic.ContainsKey(_bgm) && m_current_bgm2 != _bgm) {
                m_bgm_dic[m_current_bgm2] = null;
                m_bgm_dic.Remove(m_current_bgm2);
                m_current_bgm2 = _bgm;
                m_bgm_source2.clip = m_bgm_dic[_bgm];
                m_bgm_source2.volume = val;
                m_bgm_source2.Play();

            }
        } else {
            if(m_bgm_dic.ContainsKey(_bgm) && m_current_bgm1 != _bgm) {
                m_bgm_dic[m_current_bgm1] = null;
                m_bgm_dic.Remove(m_current_bgm1);
                m_current_bgm1 = _bgm;
                m_bgm_source1.volume = val;
                m_bgm_source1.clip = m_bgm_dic[_bgm];
                m_bgm_source1.Play();
            }
        }
    }

    public void Release() {
        var removeList = new List<string>();
        foreach(var kv in m_se_dic) {
            if(!m_ready_se_list.Any(item => item.sound == kv.Key))
                removeList.Add(kv.Key);
        }
        for(int i = 0;i < removeList.Count;i++)
            m_se_dic.Remove(removeList[i]);
    }

    public void PreLoadBoBao(string _se) {
        if(!m_se_dic.ContainsKey(_se)) {
            ResourceLoad.ResourceHandler.Instance.LoadRes<AudioClip>("Sound/zhibo/" + _se,delegate(Object obj) {
                if(obj == null) {
                    Debug.LogWarning("Cant find the zhibo se " + _se);
                    return;
                }
                AudioClip _ac = obj as AudioClip;
                m_se_dic.Add(_se,_ac);
                _ac.LoadAudioData();
            },false);
        }
    }

    public float PlayBoBao(string _se) {
        if(!GameCore.Config.SoundCommentaryOpen) {
            return 0.0f;
        }
        if(m_bobao_source == null) {
            return 0.0f;
        }
        if(!m_se_dic.ContainsKey(_se)) {
            ResourceLoad.ResourceHandler.Instance.LoadRes<AudioClip>("Sound/zhibo/" + _se,delegate(Object obj) {
                if(obj == null) {
                    Debug.LogWarning("Cant find the zhibo se " + _se);
                    return;
                }
                AudioClip _ac = obj as AudioClip;
                m_se_dic.Add(_se,_ac);
                PlayBoBao(_se);
            },false);
        } else {
            AudioClip _ac = m_se_dic[_se];
            m_bobao_source.PlayOneShot(_ac,MatchConfiger.GetSingle().GetZhiBoSoundOffset() * m_offset_final);
            return _ac.length;
        }
        return 0.0f;
    }
    public float bg1Val = 0.5f;
    public float bg2Val = 0.6f;
    public float seVal = 1;
    public float bbVal = 0.8f;
    float m_offset_all = 1.0f;
    float m_offset_dyn = 1.0f;
    float m_offset_final = 1.0f;
    //public bool m_se_on = true;
    public AudioSource m_bobao_source;
    public AudioSource m_se_source;
    public AudioSource m_l_se_source;
    public AudioSource m_bgm_source1;
    public AudioSource m_bgm_source2;

    Dictionary<string,AudioClip> m_se_dic = new Dictionary<string,AudioClip>();
    Dictionary<string,AudioClip> m_bgm_dic = new Dictionary<string,AudioClip>();
    List<SoundInfo> m_ready_se_list = new List<SoundInfo>();
    Dictionary<string,SoundInfo> m_play_se_dic = new Dictionary<string,SoundInfo>();
    List<SoundInfo> m_remove_se_list = new List<SoundInfo>();
    List<SoundInfo> m_end_se_list = new List<SoundInfo>();
    List<SoundInfo> m_idle_se_list = new List<SoundInfo>();
    string m_current_bgm1 = "";
    string m_current_bgm2 = "";
    Dictionary<string,float> m_offset_dic = null;
}