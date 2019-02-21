using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using GameCore;
using System.IO;

public class AssetMgr:Single<AssetMgr>,IProcess
{
	public delegate void AssetLoadedCallBack (string s);
	private List<string> keys = new List<string> ();
	private Dictionary<string, string> mAssetTable = new Dictionary<string, string>();
	private Dictionary<string, List<AssetLoadedCallBack>> mCallBackTable = new Dictionary<string, List<AssetLoadedCallBack>>();

	public bool CheckAssetUsing(string url)
	{
		return mCallBackTable.ContainsKey(url);
	}
	
	public bool CheckAssetLoaded(string name)
	{
		return mAssetTable.ContainsKey(name) ;
	}
	
	
	public string GetAsset(string name)
	{
		if(mAssetTable.ContainsKey(name)) 
		{
			return mAssetTable[name];
		}
		return null;
	}

//		public void ForceLoadAsset(string path, AssetLoadedCallBack assetLoaded, bool compress = true)
//		{
//			Debug.Log ("LoadAsset:" + path);
//			ResLifeManager.Instance.Live(path);
//			if(mAssetTable.ContainsKey(path)) 
//			{
//				assetLoaded(mAssetTable[path]);
//				return;
//			}
//			if(!mCallBackTable.ContainsKey(path))
//			{
//				if(compress)
//					DownLoader.Instance.Request(path, ResourceType.RT_ZIP, Config.curGameVersion, ThreadPriority.Normal, false);
//				else
//					DownLoader.Instance.Request(path, ResourceType.RT_TEXT, Config.curGameVersion, ThreadPriority.Normal, false);
//				mCallBackTable.Add(path, new List<AssetLoadedCallBack>());
//			}
//			mCallBackTable[path].Add(assetLoaded);
//			ProcessManager..Add(Instance);
//		}
	class Parameters
	{
		public AssetLoadedCallBack cb;
		public string path;
		public string filePath;
	}

	class ZipParameters
	{
		public List<AssetLoadedCallBack> callbacks;
		public byte[] bytes;
		public string key;
		public string filePath;
	}
	private void ReadFileThread(object p)
	{
		Parameters param = p as Parameters;
		string filePath = param.filePath;
		if(File.Exists(filePath))
		{
			StreamReader _sr=new StreamReader(filePath);
			string s = _sr.ReadToEnd();
			param.cb(s);
			_sr.Close();
			return;
		}
	}
	public void LoadAsset(string path, AssetLoadedCallBack assetLoaded, bool compress = true)
	{
		Parameters p = new Parameters();
		p.path = path;
		p.cb = assetLoaded;
		p.filePath = GameInfo.FilePath + path + ".byte";;
		ResLifeMgr.Instance.Live(path);
		if(mAssetTable.ContainsKey(path)) 
		{
			//Debug.LogError("mAssetTable Exists");
			assetLoaded(mAssetTable[path]);
			return;
		}
		string filePath =  GameInfo.FilePath  + path + ".byte";
		TextAsset asset = null;

		if(File.Exists(filePath))
		{

			if(!ThreadMgr.Instance.disableThread)
			{
				ThreadMgr.Instance.Start("ReadFileThread",new System.Threading.Thread(new  System.Threading.ParameterizedThreadStart(ReadFileThread)),p as object);
			}
			else ReadFileThread(p);
			return;
		}
		else
		{
			Debug.Log("Resources.Load:"+path);
			asset = Resources.Load(path,typeof(TextAsset)) as TextAsset;
			if(asset != null)
			{
	
				assetLoaded(asset.text);
				return;
			}
		}

		if(!mCallBackTable.ContainsKey(path))
		{
			if(compress)
				DownLoader.Instance.Request(path, ResourceType.RT_ZIP, ThreadPriority.Normal);
			else
				DownLoader.Instance.Request(path, ResourceType.RT_TEXT, ThreadPriority.Normal);
			mCallBackTable.Add(path, new List<AssetLoadedCallBack>());
		}
		mCallBackTable[path].Add(assetLoaded);
		ProcessManager.Add(Instance);
	}

	public void Clear()
	{
//			foreach(List<AssetLoadedCallBack> callbacks in mCallBackTable.Values)
//			{
//				callbacks.Clear();
//			}
	}



	public void Start()
	{
		EventManager.Instance.SendEvent<bool>(EVENTTYPE.SWITCH_FLOWER.ToString(),true);
	}

	public void End()
	{
		EventManager.Instance.SendEvent<bool>(EVENTTYPE.SWITCH_FLOWER.ToString(),false);
	}
	public bool IsFinished()
	{
		return mCallBackTable.Count == 0;
	}

	public void Release(bool strong = false)
	{
		keys.Clear();
		foreach(string key in mAssetTable.Keys)
		{
			keys.Add(key);
		}
		foreach(string key in keys)
		{
			if(ResLifeMgr.Instance.canRelease(key) || strong)
			{			
				mAssetTable[key]=null;
				mAssetTable.Remove(key);
				ResLifeMgr.Instance.Remove(key);
			}
		}
		foreach(List<AssetLoadedCallBack> callbacks in mCallBackTable.Values)
		{
			callbacks.Clear();
		}
		mCallBackTable.Clear();
	}


	private void UnZipThread(object o)
	{
		ZipParameters p = o as ZipParameters;
		string _str= Ionic.Zlib.GZipStream.UncompressString(p.bytes);
		string filePath = p.filePath;
		string directoryName = Path.GetDirectoryName(filePath);
		if(!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		StreamWriter _sw=new StreamWriter(filePath);
		_sw.Write(_str);
		_sw.Close();	
		mAssetTable[p.key] = _str;
		foreach( AssetLoadedCallBack cb in p.callbacks)
		{
			cb(_str);
		}
	}

	public void Update(float deltaTime)
	{

		if(mCallBackTable.Count == 0)return;

		keys.Clear();
		foreach(string key in mCallBackTable.Keys)
		{
			keys.Add(key);
		}
		foreach (string key in keys) 
		{
			if(!mCallBackTable.ContainsKey(key))continue;
			List<AssetLoadedCallBack> callbacks = mCallBackTable[key];
			if (DownLoader.Instance.Loaded(key))
			{
				mCallBackTable.Remove(key);
				DownLoadUnit dlu = DownLoader.Instance.Fetch(key);
				if(dlu != null)
				{
					if (dlu.GetWWW().error == null)
		            {	
						string _str = "";
						if(dlu.mReq.mType== ResourceType.RT_ZIP)
						{
							ZipParameters param = new ZipParameters();
							param.bytes = DownLoadResultFetcher.Instance.FetchByte(dlu);
							param.key= key;
							param.callbacks = callbacks;
							param.filePath = GameInfo.FilePath + key + ".byte";
							if(!ThreadMgr.Instance.disableThread)
							{
								ThreadMgr.Instance.Start("UnzipData",new System.Threading.Thread(new  System.Threading.ParameterizedThreadStart(UnZipThread)),param as object);
							}
							else 
							{
								UnZipThread(param);
							}
							//return;
						}
						else if(dlu.mReq.mType== ResourceType.RT_TEXT)
						{
							_str=DownLoadResultFetcher.Instance.FetchString(dlu);
							string filePath = GameInfo.FilePath+ key + ".byte";
							string directoryName = Path.GetDirectoryName(filePath);
							if(!Directory.Exists(directoryName))
							{
								Directory.CreateDirectory(directoryName);
							}
							StreamWriter _sw=new StreamWriter(filePath);
							_sw.Write(_str);
							_sw.Close();	
							mAssetTable[key] = _str;
							foreach( AssetLoadedCallBack cb in callbacks)
							{
								cb(_str);
							}
						}
					
					}
					else
					{
						foreach( AssetLoadedCallBack cb in callbacks)
						{
							cb(null);
						}
					}
					dlu.ReleaseRes ();
				}

			}
		}
	}
}
