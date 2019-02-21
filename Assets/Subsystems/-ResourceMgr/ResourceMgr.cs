using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using GameCore;
using System.IO;
using System.Linq;
public delegate void ResLoadedCallBack (Object res);
public delegate void SceneLoadedCallBack (bool suc);
public class ResourceMgr:Single<ResourceMgr>
{
	private List<string> keys = new List<string>();
	private Dictionary<string, Object> mResourceTable = new Dictionary<string, Object> ();
	private Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
	public Dictionary<string,string>HashDic=new Dictionary<string, string>();

	public void AddResource(string path,Object res)
	{
		ResLifeMgr.Instance.Live(path);
		mResourceTable[path] =  res;
	}
	
	public void UnLoadRes(string name)
	{

		ResLifeMgr.Instance.Die (name);
		if (mResourceTable.ContainsKey (name)) {
			if (!(mResourceTable [name] is UnityEngine.GameObject)) 
			{
				Resources.UnloadAsset (mResourceTable [name]);
			}
			mResourceTable [name] = null;
			mResourceTable.Remove (name);
		}
	}

	private bool KeepInMemory(string name)
	{
		return name.StartsWith("sound/bgm");
	}

	public void UnLoadBundle(string name)
	{

		if(bundles.ContainsKey(name) && !KeepInMemory(name))
		{
			//Debug.LogError("UnLoadBundle suc:"+name);
			bundles[name].Unload (false);
			bundles[name] = null;
			bundles.Remove(name);
		}

	}

	public void UnLoadBundle()
	{
		//#if UNITY_ANDROID
//			keys.Clear();
//			foreach(string key in bundles.Keys)
//			{
//				keys.Add(key);
//			}
//			foreach(string key in keys)
//			{
//				UnLoadBundle(key);
//
//			}
		//#endif
	}
		
	public GameObject LoadGameObject (string path, bool autoRelease = true,bool noticeError = false)
	{
		return LoadRes<GameObject>(path,autoRelease,noticeError);
	}

	//private AssetBundleManifest bundleManifest;
	//public AssetBundleManifest BundleManifest
	//{
	//	get
	//	{
	//		if(bundleManifest ==null )
	//		{
	//			string bundleName = "assetbundles.unity3d";
	//			if(HashDic.ContainsKey(bundleName))bundleName =HashDic[bundleName];
	//			AssetBundle ab = ResourceMgr.Instance.LoadAssetBundle(bundleName,false); 
	//			bundleManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
	//		}
	//		return bundleManifest;
	//	}
	//}

	//public AssetBundle LoadAssetBundle(string bundleName,bool loadAllDependencies= true)
	//{


	//	if(bundles.ContainsKey(bundleName))return bundles[bundleName];
	//	string bundlePath = GameInfo.FilePath+"AssetBundles_"+GameInfo.RegionString+"/"+bundleName;
	//	Debug.Log("bundlePath"+bundlePath);
	//	if(File.Exists(bundlePath))
	//	{
	//		if(loadAllDependencies)
	//		{
	//			string[] dependences = BundleManifest.GetAllDependencies(bundleName);  
	//			if(dependences.Length >0)
	//			{
	//				for(int i=0 ; i< dependences.Length; i++)
	//				{
	//					LoadAssetBundle(dependences[i]);
	//				}
	//			}
	//		}
	//		AssetBundle ab = AssetBundle.LoadFromFile (bundlePath);
	//		if(ab != null)
	//		{

	//			bundles.Add(bundleName, ab);
	//			return ab;
	//		}

	//	}
	//	else
	//	{
			
	//		if(UnPackMgr.Instance.packgeList!=null && UnPackMgr.Instance.packgeList.Contains(bundleName))
	//		{
	//			UnPackMgr.Instance.SetDirty();
	//		}
	//	}
	//	Debug.Log("Can't find:"+bundlePath);
	//	return null;
			
	//}



	//public void LoadScene (string sceneName, SceneLoadedCallBack resLoaded)
	//{
	//	if(string.IsNullOrEmpty(sceneName)) return;
	//	//ResLifeManager.Instance.Live(sceneName);

	//	string bundleName = BundleHelper.Instance.GetBundlePath(sceneName+".unity");
	//	//Debug.LogError("bundleName:"+bundleName);
	//	if(HashDic.ContainsKey(bundleName))bundleName = HashDic[bundleName];
	//	AssetBundle ab = LoadAssetBundle(bundleName);
	//	if(resLoaded!=null)resLoaded (ab);
	
	//}

//		public GameObject LoadPrefab<T> (string path, bool autoRelease = true,bool noticeError = false)where T:Object
//		{
//			GameObject go= null;
//			LoadRes<T> (path, delegate(Object res) {
//				go = res as GameObject;
//			},autoRelease , noticeError);
//			return go;
//		}

	public T LoadRes<T> (string path, bool autoRelease = true,bool noticeError = false)where T:Object
	{
		if(string.IsNullOrEmpty(path)) return null;
		if(autoRelease)
		{
			ResLifeMgr.Instance.Live(path);
			if (mResourceTable.ContainsKey (path)) 
			{
				return mResourceTable [path] as T;					
			}
		}


		Object prefab = Resources.Load(path,typeof(T));

		if (prefab != null) 
		{
			if(autoRelease)mResourceTable[path]=prefab;
			return prefab as T;
		}
		else if(path.StartsWith("Prefabs"))
		{
			string newPath = "LocalUIPrefabs/"+Path.GetFileName(path);
			prefab = Resources.Load(newPath,typeof(T));
			if (prefab != null) 
			{
				if(autoRelease)mResourceTable[path]=prefab;
				return prefab as T;
			}

		}
//		string name = Path.GetFileNameWithoutExtension(path);
//		string dir = Path.GetDirectoryName(path);
//		string parent =dir.Substring(dir.LastIndexOf("/")+1);
//		string bundleName = BundleHelper.Instance.GetBundlePath(parent+"-"+name,typeof(T));

////			string bundleName = Utility.Instance.GetBundlePath(name,typeof(T));

//		if(HashDic.ContainsKey(bundleName))bundleName = HashDic[bundleName];
////			Debug.LogError("bundleName:"+bundleName);
		//AssetBundle ab = LoadAssetBundle(bundleName);
		//if(ab !=null)
		//{
		//	Object asset = ab.LoadAsset(name);
		//	if(asset == null)Debug.LogError("asset is null:"+bundleName);
		//	if(autoRelease)mResourceTable[path] = asset;
		//	UnLoadBundle(bundleName);
		//	return asset as T;
		//}
		return null;
		
	}

	public void Release(bool strong = false)
	{
		keys.Clear();
		foreach(string key in mResourceTable.Keys)
		{
			keys.Add(key);
		}
		foreach(string key in keys)
		{
			if(ResLifeMgr.Instance.canRelease(key) || strong)
			{
				mResourceTable[key] = null;
				mResourceTable.Remove(key);
				ResLifeMgr.Instance.Remove(key);
			}
		}
	}
}
