using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public delegate void AssetBundleLoadedCallBack (bool suc);
public class AssetBundleMgr:Single<AssetBundleMgr>,IProcess
{
	private List<string> keys = new List<string>();
	private Dictionary<string,  AssetBundleLoadedCallBack> mCallBackTable = new Dictionary<string,  AssetBundleLoadedCallBack> ();

	public void PreLoadAssetBundle(string path,AssetBundleLoadedCallBack resLoaded, bool forceLoadFromSource = false)
	{
		if(string.IsNullOrEmpty(path)) return;
		string filePath = GameInfo.FilePath + "AssetBundles_"+GameInfo.RegionString+"/"+path;
		Debug.Log("load filePath:"+filePath);

		if(File.Exists(filePath))
		{
			AssetBundle ab = AssetBundle.LoadFromFile (filePath);

			if(ab != null)
			{
				if(resLoaded!=null)resLoaded (true);
				return;
			}
		}

		
		if (!mCallBackTable.ContainsKey (path))
		{
			DownLoader.Instance.Request (path, ResourceType.RT_ASSETBUNDLE, ThreadPriority.Normal);
		}

		if(resLoaded!=null)mCallBackTable[path] = resLoaded;
		ProcessManager.Add(Instance);
	}


	public void Release(bool strong = false)
	{
//			keys.Clear();
//			foreach(string key in mStreamTable.Keys)
//			{
//				keys.Add(key);
//			}
//			foreach(string key in keys)
//			{
//				if(ResLifeManager.Instance.canRelease(key) || strong)
//				{
//					mStreamTable[key] = null;
//					mStreamTable.Remove(key);
//					ResLifeManager.Instance.Remove(key);
//				}
//			}
	}
	
	public void Clear()
	{
		mCallBackTable.Clear();

	}

	public void Reset()
	{
//			for (int i=0; i<mStreamTable.Count; i++)
//			{
//				mStreamTable[mStreamTable.Keys.ElementAt(i)] = null;
//			}
//			mStreamTable.Clear();
	}

	public void Start()
	{
		
	}

	public void End()
	{

	}
	public bool IsFinished()
	{
		return mCallBackTable.Count == 0;
	}

	public void Update (float deltaTime)
	{	
		if (mCallBackTable.Count > 0)
		{
			
			keys.Clear();
			foreach(string key in mCallBackTable.Keys)
			{
				keys.Add(key);
			}
			foreach (string key in keys) 
			{
				if(!mCallBackTable.ContainsKey(key))continue;
				AssetBundleLoadedCallBack cb = mCallBackTable[key];
				if (DownLoader.Instance.Loaded (key)) 
				{
					mCallBackTable.Remove(key);
					DownLoadUnit dlu = DownLoader.Instance.Fetch (key);
					if(dlu != null && dlu.GetWWW ().error == null)
					{

						bool suc =DownLoadResultFetcher.Instance.WriteAssetbundle(dlu, key);	
						cb(suc);
						dlu.GetWWW().assetBundle.Unload(true);
					
					}
					else cb(false);
					dlu.ReleaseRes ();

				}
			
			}
		}
	}
}

