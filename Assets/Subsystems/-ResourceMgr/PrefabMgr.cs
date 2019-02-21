using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using GameCore;
using System.IO;
using System.Linq;

public class PrefabPool
{

	private List<GameObject> idlePool = new List<GameObject>();
	private List<GameObject> usePool = new List<GameObject>();

	public GameObject Pop()
	{
		foreach(GameObject go in idlePool)
		{
			if(go!=null)
			{
				usePool.Add(go);
				idlePool.Remove(go);
				return go;
			}
		}
		return null;
	}

	public int Count
	{
		get
		{
			return idlePool.Count;
		}
	}

	public void Add(GameObject go)
	{
		usePool.Add(go);
	}

	public void Push(GameObject go)
	{
//			Debug.LogError("Push go"+go.name);
		if(go.activeSelf)go.SetActive(false);
		usePool.Remove(go);
		idlePool.Add(go);
		go.transform.parent = PoolRoot.Instance.transform;

	}

	public void Release(bool clearall = false )
	{
		foreach(GameObject go in idlePool)
		{
			if(go!=null)GameObject.Destroy(go);

		}
		if(clearall)
		{
			foreach(GameObject go in usePool)
			{
				if(go!=null)GameObject.Destroy(go);

			}
		}
		idlePool.Clear();
		usePool.Clear();
	}
}
public class PrefabMgr:Single<PrefabMgr>
{
	public delegate void PrefabLoadedCallBack (GameObject go);
	private List<string> keys = new List<string> ();
	private Dictionary<string, PrefabPool> mPrefabPoolTable = new Dictionary<string, PrefabPool>();
	private Dictionary<string, GameObject> mPrefabTable = new Dictionary<string, GameObject>();


	public void Push(GameObject go,string path="")
	{
		//string key = path+"_clone";
		if(go==null)return;
		string key = go.name;

		if(!string.IsNullOrEmpty(path))
		{
			key = path.Substring(path.LastIndexOf("/")+1)+"(Clone)";
		}
//			Debug.LogError("push key:"+key);	
		if(mPrefabPoolTable.ContainsKey(key) && mPrefabPoolTable[key]!=null) 
		{
			mPrefabPoolTable[key].Push(go);
		}
	}
//
//		public void PrePop(string path)
//		{
//			GameObject go = Pop(path);
//			Push(go);
//		}

	public GameObject Pop(string path)
	{
		string key = path.Substring(path.LastIndexOf("/")+1)+"(Clone)";
		ResLifeMgr.Instance.Live(key);
		if(mPrefabPoolTable.ContainsKey(key) && mPrefabPoolTable[key]!=null && mPrefabPoolTable[key].Count >0) 
		{
			return  mPrefabPoolTable[key].Pop();
		}
		GameObject go = ResourceMgr.Instance.LoadGameObject(path);

		GameObject instance  = GameObject.Instantiate(go) as GameObject;
		if(!mPrefabPoolTable.ContainsKey(key)|| mPrefabPoolTable[key]==null)
		{
			mPrefabPoolTable[key] =new PrefabPool();
		}
		mPrefabPoolTable[key].Add(instance);
	
		return instance;
	}
	public bool CheckPrefabLoaded(string path)
	{
		return mPrefabTable.ContainsKey(path) ;
	}




	public GameObject GetPrefab(string path)
	{
		if(mPrefabTable.ContainsKey(path)) 
		{
			return mPrefabTable[path];
		}
		return null;
	}

	public GameObject CreatePrefab(string path)
	{
		string key = path;
		ResLifeMgr.Instance.Live(key);
		if(mPrefabTable.ContainsKey(key) && mPrefabTable[key]!=null) 
		{
			return mPrefabTable[key];
		}
		GameObject go = ResourceMgr.Instance.LoadGameObject(path);
		GameObject instance = GameObject.Instantiate(go) as GameObject;
		mPrefabTable[key] = instance;
		return instance;

	}

	public void ReleasePoolImmediately()
	{
		keys = mPrefabPoolTable.Keys.ToList();
		foreach(string key in keys)
		{
			if(ResLifeMgr.Instance.canRelease(key))
			{		
				mPrefabPoolTable[key].Release(true);	
				mPrefabPoolTable[key]=null;
				mPrefabPoolTable.Remove(key);
				ResLifeMgr.Instance.Remove(key);
			}
		}
	}

	public void Release()
	{

		keys = mPrefabTable.Keys.ToList();

		foreach(string key in keys)
		{
	
			if(ResLifeMgr.Instance.canRelease(key))
			{		
				GameObject.Destroy(mPrefabTable[key]);	
				mPrefabTable[key]=null;
				mPrefabTable.Remove(key);
				ResLifeMgr.Instance.Remove(key);
			}
		}
		keys = mPrefabPoolTable.Keys.ToList();
		foreach(string key in keys)
		{
			if(ResLifeMgr.Instance.canRelease(key))
			{		
				mPrefabPoolTable[key].Release();	
//					mPrefabPoolTable[key]=null;
//					mPrefabPoolTable.Remove(key);
//					ResLifeManager.Instance.Remove(key);
			}
		}
	}
}
