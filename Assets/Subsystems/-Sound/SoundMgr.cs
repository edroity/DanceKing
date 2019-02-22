//ybzuo-dena
using UnityEngine;
using System.Collections.Generic;

public class SoundMgr:MonoBehaviour
{
	static SoundMgr _single;
//	public AudioSource Audio1;
//	public AudioSource Audio2;
	public const float bg1Val = 0.5f; 
	public const float bg2Val = 0.8f; 
	public const float seVal = 0.8f; 
    public bool soundOpen = true;
	public static SoundMgr GetSingle()
	{
		if(_single==null)
		{
			_single=GameObject.FindObjectOfType<SoundMgr>();
		}
		return _single;
	}

		class SoundInfo
		{
			public string sound;
			public float time;
			public bool loop;
			public bool queue;
			public float val;
		}

		public  void Awake ()
		{ 

			foreach (AudioListener _l in GameObject.FindObjectsOfType(typeof(AudioListener))) {	
				GameObject.Destroy (_l);
			}
			GameObject _se_root = new GameObject ();
			_se_root.name = "SeRoot";
			_se_root.transform.parent = transform;
			m_se_source = _se_root.AddComponent<AudioSource> ();
			m_se_source.volume = seVal;
			m_se_source.playOnAwake = false;
				
			GameObject _long_se_root = new GameObject ();
			_long_se_root.name = "LongSeRoot";
			_long_se_root.transform.parent = transform;
			m_l_se_source = _long_se_root.AddComponent<AudioSource> ();
			m_l_se_source.volume = seVal;
			m_l_se_source.loop =false;
			m_l_se_source.playOnAwake = false;

			GameObject _bgm_root1 = new GameObject ();
			_bgm_root1.name = "BgmRoot1";
			_bgm_root1.transform.parent = transform;
			m_bgm_source1 = _bgm_root1.AddComponent<AudioSource> ();
			m_bgm_source1.loop = true;
			m_bgm_source1.volume = bg1Val;
			m_bgm_source1.playOnAwake = false;
				
			GameObject _bgm_root2 = new GameObject ();
			_bgm_root2.name = "BgmRoot2";
			_bgm_root2.transform.parent = transform;
			m_bgm_source2 = _bgm_root2.AddComponent<AudioSource> ();
			m_bgm_source2.loop = true;
			
			m_bgm_source2.volume = bg2Val;
			m_bgm_source2.playOnAwake = false;
			
			gameObject.AddComponent<AudioListener> ();
			//switch_bgm (HosSave.MusicOpen);
			//switch_se (HosSave.SoundOpen);



		/*
		for(int i=0;i<ac_coll.Length;++i){
		    m_se_dic.Add(ac_coll[i].name,ac_coll[i]);
		}
		for(int i=0;i<bgm_coll.Length;++i){
		    m_bgm_dic.Add(bgm_coll[i].name,bgm_coll[i]);
		}
		*/
//			switch_bgm (PlayerPrefs.GetInt ("Music", 1) == 1);
//			switch_se (PlayerPrefs.GetInt ("Music", 1) == 1);
//			if(Application.isEditor)
//			{
//				switch_bgm(false);
//				switch_se(false);
//			}
//			else
//			{
//
//			}

		}
		//float curSe = 0;
		void Update ()
		{
			foreach (SoundInfo s in m_play_se_dic.Values)
			{
				s.time-= Time.deltaTime;
			}
			
			for (int i=0; i<m_ready_se_list.Count; ++i) 
			{
				m_ready_se_list [i].time -= Time.deltaTime;
				if (m_ready_se_list [i].time <= 0.0f) 
				{  
					if(m_play_se_dic.ContainsKey(m_ready_se_list [i].sound))
					{
						if(m_se_dic [m_ready_se_list [i].sound].length -m_play_se_dic [m_ready_se_list [i].sound].time < 0.01f)
						{
							m_remove_se_list.Add(m_ready_se_list [i]);
							m_end_se_list.Add(m_ready_se_list [i]);
						}
						else
						{
							m_play_se_dic [m_ready_se_list [i].sound].time =0f;
						}
					}
					else
					{
						m_ready_se_list [i].time = m_se_dic [m_ready_se_list [i].sound].length;
//					Debug.LogError("se:"+m_ready_se_list [i].sound);
						m_se_source.PlayOneShot (m_se_dic [m_ready_se_list [i].sound],m_ready_se_list [i].val);
						m_play_se_dic.Add (m_ready_se_list [i].sound,m_ready_se_list [i]);
						m_remove_se_list.Add(m_ready_se_list [i]);


					}
					
				}
			}
			if (m_remove_se_list.Count > 0)
			{
				foreach (SoundInfo _si in m_remove_se_list) {
					m_ready_se_list.Remove (_si);
				}
				m_remove_se_list.Clear ();
			}

			if (m_play_se_dic.Count > 0) 
			{
				foreach (SoundInfo s in m_play_se_dic.Values) 
				{
					if (s.time <= 0)m_end_se_list.Add (s);
				}
			}

			if (m_end_se_list.Count > 0)
			{
				foreach (SoundInfo _si in m_end_se_list) 
				{
					m_play_se_dic.Remove (_si.sound);
				}
				m_idle_se_list.AddRange (m_end_se_list);
				m_end_se_list.Clear ();
			}
		}



		

		private SoundInfo GetSoundInfo
		{
			get
			{
				if(m_idle_se_list.Count >0)
				{
					SoundInfo idle = m_idle_se_list[0];
					m_idle_se_list.RemoveAt(0);
					return idle;
				}
				else return new SoundInfo();
			}
		}
		public void StopSeQuick()
		{
			if(m_long_fight_se_tick>1.0f)
			{
				m_long_fight_se_tick=-1.0f;
				m_se_source.Stop();
			}
		}
		public void stop_se ()
		{
			m_se_source.Stop ();
			m_ready_se_list.Clear ();
			m_remove_se_list.Clear ();
		}

		//private void InitOffsetDic()
		//{
		//	if(m_offset_dic==null)
		//	{
		//		m_offset_dic=new Dictionary<string, float>();
		//		for(int i=0;i<StaticDatar.G.m_se_list.Count;++i)
		//		{
		//			JDse _ccse=StaticDatar.G.m_se_list[i];
		//			if(!m_offset_dic.ContainsKey(_ccse.name))
		//			{
		//				m_offset_dic.Add(_ccse.name,_ccse.offset);
		//			}
		//		}
		//	}
		//}

		public float GetScale(string _se)
		{
			//InitOffsetDic();
			if(m_offset_dic.ContainsKey(_se))return m_offset_dic[_se];
			else return 1.0f;
		}
		public float play_se (string _se, bool queue = false,float _delay=0.0f,bool _loop=false)
		{
			if(m_long_fight_se_tick>0.0f)
			{
				m_long_fight_se_tick-=Time.deltaTime;
			}
			if (m_se_on)
			{
				if(m_se_source==null)
				{
					return 0.0f;
				}
				//if(StaticDatar.IsLoaded)InitOffsetDic();
//				Debug.LogError("_se:"+_se);
				if (!m_se_dic.ContainsKey (_se) || m_se_dic[_se]==null) 
				{

                    Object res = ResourceMgr.Instance.LoadRes<Object>("Sound/Se/" + _se);
    					
    				AudioClip _ac = res  as AudioClip;
    				if (_ac != null) 
    				{
    					m_se_dic[_se]=_ac;
    				}
				}
				if (m_se_dic.ContainsKey (_se) && m_se_dic[_se]!=null) 
				{
					float val=1.0f;
					if(m_offset_dic != null &&m_offset_dic.ContainsKey(_se))
					{
						val=m_offset_dic[_se];
					}
					SoundInfo _si = GetSoundInfo;
					_si.sound = _se;
					_si.time = _delay;
					_si.val = val;
					_si.loop=_loop;
					if(_si.loop)
					{
						Debug.Log("Loop se:"+_se);
					}
					_si.queue = queue;
					m_ready_se_list.Add (_si);
					return m_se_dic[_se].length;
				}
				else
				{
					Debug.Log("音效:"+_se+" 为空");
					return 0.0f;
				}
			}
			else
			{
				return 0.0f;
			}

		}
		

		public void pause_bgm (int _layer=1)
		{
			if (_layer == 2) 
			{
				m_bgm_source2.Pause();	
			} 
			else 
			{
				m_bgm_source1.Pause();
			}

		}

		public string  CurrentBgm1(int layer =1)
		{
			if(layer==1)
				return m_current_bgm1;
			else return m_current_bgm2;

		}

		public void play_bgm(int _layer=1)
		{
			if (!m_bgm_on)
				return;
			if (_layer == 2) 
			{
				m_bgm_source2.Play();	
			} 
			else 
			{
				m_bgm_source1.Play();
			}
		}

		public void ScaleDownBgm()
		{
			m_bgm_source1.volume =bg1Val/10;
			m_bgm_source2.volume =bg2Val/10;
		}

		public void Mute(bool isMute)
		{
			if(isMute)
			{
				m_bgm_source1.volume =0;
				m_bgm_source2.volume =0;
				SoundMgr.GetSingle().switch_se(false);
			}
			else
			{
				m_bgm_source1.volume =bg1Val;
				m_bgm_source2.volume =bg2Val;
				if(this.soundOpen)
				{
					SoundMgr.GetSingle().switch_se(true);
				}
			}
		}

		public void ScaleUpBgm()
		{
			m_bgm_source1.volume =bg1Val;
			m_bgm_source2.volume =bg2Val;
		}

		public void stop_bgm (int _layer=1)
		{
			if (_layer == 2) 
			{
				m_bgm_source2.Stop();	
				m_bgm_source2.clip = null;
				m_current_bgm2 = "";
			} 
			else 
			{
				m_bgm_source1.Stop();
				m_bgm_source1.clip = null;
				m_current_bgm1 = "";
			}
			
		}

		public void play_long_se(string se)
		{
			
			if(!m_se_on)return;
			if (!m_se_dic.ContainsKey (se) || m_se_dic[se]==null) 
			{
				Debug.Log("se"+se);
                Object res = ResourceMgr.Instance.LoadRes<Object>("Sound/Se/" + se);

    			AudioClip _ac = res  as AudioClip;
    			if (_ac != null) 
    			{
    				m_se_dic[se]= _ac;
    			}

			}
			StopLongSe();
			float val=seVal;
			if(m_offset_dic != null && m_offset_dic.ContainsKey(se))
			{
				val=m_offset_dic[se];
			}
			if(m_se_dic.ContainsKey(se) && m_se_dic[se]!=null)
			{
				m_l_se_source.volume = val;
				m_l_se_source.clip = m_se_dic[se];
				m_l_se_source.Play();
			}

		}

		public void StopLongSe()
		{
			m_l_se_source.Stop();
			m_l_se_source.clip = null;
		}

		public void play_bgm (string _bgm, int _layer=1)
		{
//			Debug.LogError("play_bgm1:"+_bgm);
			if (!m_bgm_dic.ContainsKey (_bgm) || m_bgm_dic[_bgm]==null) 
			{
                    Object res = ResourceMgr.Instance.LoadRes<Object>("Sound/Bgm/" + _bgm);
					AudioClip _ac = res  as AudioClip;
					if (_ac != null) 
					{
						m_bgm_dic[_bgm]=_ac;
					}
				
			}
			if (m_bgm_dic.ContainsKey (_bgm) &&m_bgm_dic[_bgm] !=null)
			{
				if (_layer == 2) 
				{
					if(m_current_bgm2 != _bgm) 
					{
						m_bgm_dic[m_current_bgm2] = null;
						m_bgm_dic.Remove(m_current_bgm2);
						m_current_bgm2 = _bgm;
						
						m_bgm_source2.clip = m_bgm_dic[_bgm];
						if(m_bgm_on)m_bgm_source2.Play();
				//		Debug.Log("m_bgm_source2.Play():"+m_current_bgm2);

					}
				}
				else 
				{
					if ( m_current_bgm1 != _bgm) 
					{
						m_bgm_dic[m_current_bgm1] = null;
						m_bgm_dic.Remove(m_current_bgm1);
						m_current_bgm1 = _bgm;

						m_bgm_source1.clip = m_bgm_dic[_bgm];
						if(m_bgm_on)m_bgm_source1.Play();

					}
				}
			}
		}

		public void switch_se (bool _b)
		{
			m_se_on = _b;
		}

		public void switch_bgm (bool _b)
		{
			m_bgm_on = _b;
			if (m_bgm_on)
			{
				if(!m_bgm_source1.isPlaying)
				{
					m_bgm_source1.Play ();
					m_bgm_source2.Play ();
				}
			} 
			else
			{
				m_bgm_source1.Stop ();
				m_bgm_source2.Stop ();
			}
		}
		public void SetLongFightSeTime(float _tt)
		{
			m_long_fight_se_tick=Mathf.Max(_tt,m_long_fight_se_tick);
		}
	    

		public bool m_bgm_on = true;
		public bool m_se_on = true;
		AudioSource m_se_source;
		AudioSource m_l_se_source;
		public AudioSource m_bgm_source1;
		public AudioSource m_bgm_source2;

		Dictionary<string,AudioClip> m_se_dic = new Dictionary<string, AudioClip> ();
		Dictionary<string,AudioClip> m_bgm_dic = new Dictionary<string, AudioClip> ();
		List<SoundInfo> m_ready_se_list = new List<SoundInfo> ();
		Dictionary<string, SoundInfo> m_play_se_dic = new Dictionary<string,SoundInfo> ();
		List<SoundInfo> m_remove_se_list = new List<SoundInfo> ();
		List<SoundInfo> m_end_se_list = new List<SoundInfo> ();
		List<SoundInfo> m_idle_se_list= new List<SoundInfo> ();
		string m_current_bgm1 = "";
		string m_current_bgm2 = "";

	Dictionary<string,float> m_offset_dic=null;

	float m_long_fight_se_tick=-1.0f;


}


