using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using GameCore;
using System.IO;
using System.Linq;

public enum UPK_LOADED_STATE
{
	FAIL =0,
	SUC=1,
	MD5_ERR =2
}
public delegate void UpkLoadedCallBack (UPK_LOADED_STATE result);
public class UpkMgr:Single<UpkMgr>,IProcess
{
	private List<string> keys = new List<string>();
	private Dictionary<string,  UpkLoadedCallBack> mCallBackTable = new Dictionary<string, UpkLoadedCallBack> ();


	public void LoadUPK(string upkName, UpkLoadedCallBack resLoaded, bool noticeError = true)
	{
		if (!mCallBackTable.ContainsKey (upkName))
		{
			DownLoader.Instance.Request (upkName, ResourceType.RT_STREAM, ThreadPriority.Normal, noticeError);
		}

		if(resLoaded!=null)mCallBackTable[upkName] = resLoaded;
		ProcessManager.Add(Instance);
	}

	public void Release(bool strong = false)
	{

	}
	
	public void Clear()
	{
		mCallBackTable.Clear();

	}

	public void Reset()
	{

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
				UpkLoadedCallBack cb = mCallBackTable[key];
				if (DownLoader.Instance.Loaded (key)) 
				{
					mCallBackTable.Remove(key);
					DownLoadUnit dlu = DownLoader.Instance.Fetch (key);
					if(dlu != null )
					{
						
						if (dlu.GetWWW ().error == null) 
						{
							DownLoadResultFetcher.Instance.WriteUpk(dlu, key,cb);		
						} 
						else 
						{
							if(dlu.mReq.noticeError)
							{
								cb(UPK_LOADED_STATE.FAIL);
							}
						}
						dlu.ReleaseRes ();
					}

				}
				
			}
		}
	}
}

