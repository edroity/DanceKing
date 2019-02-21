using System.Collections.Generic;
using UnityEngine;
public class DownLoader:IProcess
{ 
	private static DownLoader mInstance = null; 
	public static DownLoader Instance
	{
		get
		{
			if ( mInstance == null )
			{
				mInstance = new DownLoader();
				mInstance.mMaxLoads = MaxLoadsCount;

			}
			return mInstance;
		}
	}	

	public static int MaxLoadsCount
	{
		get
		{

			#if UNITY_IPHONE
			return 5;
			#elif UNITY_ANDROID
				if(SystemInfo.systemMemorySize <= 1024)
				return 3;
				else
				return 5;
			#endif
            return 5;
		}
	}

    #region interface
//		public void Request(string url, ResourceType type, int version, ThreadPriority priority)
//		{
//			Request (url, type, version, priority, false);
//		}
	
	public void Request(string url, ResourceType type,ThreadPriority priority, bool noticeError = false)
    {
		//Debug.LogError("Request www url:"+url);
        if (!mWorkingPool.ContainsKey(url) && !mStoragePool.ContainsKey(url)) 
        {
			//Debug.Log("mWaitingPool.Count"+mWaitingPool.Count);
            DownLoadUnitReq req = new DownLoadUnitReq();
            req.mUrl = url;
			req.mType = type;
			//req.mVersion = version;
            req.mPriority = priority;
			//req.compress = comp;
			req.noticeError = noticeError;
			switch(priority)
			{
				default:
				case ThreadPriority.Normal: mNormalWaitingPool.Enqueue(req);break;
				case ThreadPriority.Low: mLowWaitingPool.Enqueue(req);break;
			}
			ProcessManager.Add(Instance);
			//Debug.Log("mNormalWaitingPool.Count"+mNormalWaitingPool.Count);
			//Debug.Log("mLowWaitingPool.Count"+mLowWaitingPool.Count);
        }
    }

	public void Start()
	{
		
	}
	
	public void End()
	{
		
	}
	public bool IsFinished()
	{
//			Debug.Log("DOWNLOADR");
		return mWorkingPool.Count == 0 && mNormalWaitingPool.Count == 0 && mLowWaitingPool.Count == 0;
	}

	public void Update(float deltaTime)
    {
		//Debug.Log("Update:"+deltaTime);

        foreach (KeyValuePair<string,DownLoadUnit> keyVal in mWorkingPool)
        {
			//Debug.Log("test");
            if (keyVal.Value.Loaded())
            {
				//Debug.LogError("loaded:"+keyVal.Key);
                mStoragePool.Add(keyVal.Key, keyVal.Value);
                mWorkingPool.Remove(keyVal.Key);
                break;
            }
        }

        if (mWorkingPool.Count < mMaxLoads)
        {
			//Debug.Log("Update1:"+deltaTime);

            if (mNormalWaitingPool.Count > 0)
            {
				//Debug.Log("Updat2:"+deltaTime);
				DownLoadUnitReq req =  mNormalWaitingPool.Dequeue();
				if (!mWorkingPool.ContainsKey(req.mUrl) 
				&& !mStoragePool.ContainsKey(req.mUrl)) 
				{
					//Debug.LogError("req.mUrl:"+req.mUrl+">>>>"+mWorkingPool.Count);
					mWorkingPool.Add(req.mUrl, new DownLoadUnit(req));
				}
            } 
			else if(mLowWaitingPool.Count > 0)
			{
				DownLoadUnitReq req =  mLowWaitingPool.Dequeue();
				if (!mWorkingPool.ContainsKey(req.mUrl) 
				&& !mStoragePool.ContainsKey(req.mUrl)) 
				{

					mWorkingPool.Add(req.mUrl, new DownLoadUnit(req));
				}
			}
			
        }
    }
	
    public bool Loaded(string url)
    {
        if (url != null && mStoragePool.ContainsKey(url))
        {
            return true;
        }
        return false;
    }
	
	public float GetCurrentProgress()
	{
		if(mWorkingPool.Count>0)
		{
			foreach(KeyValuePair<string,DownLoadUnit> _kv in mWorkingPool)
			{
				if(_kv.Value.GetProgress()>0.0f)
				{
					return _kv.Value.GetProgress();
				}
			}
		}
		return 0.0f;
	}
	
    public float GetProgress(string url)
    {
        if (mWorkingPool.ContainsKey(url))
        {
            return mWorkingPool[url].GetProgress();
        }
        return 0.0f;
    }
	
    public DownLoadUnit Fetch(string url)
    {
        DownLoadUnit dlu=null;
        if (mStoragePool.ContainsKey(url))
        {
            dlu = mStoragePool[url];
            mStoragePool.Remove(url);
        }
        return dlu;
    }

    #endregion
	
    private int mMaxLoads = 5;
    private Queue<DownLoadUnitReq> mNormalWaitingPool = new Queue<DownLoadUnitReq>();
	private Queue<DownLoadUnitReq> mLowWaitingPool = new Queue<DownLoadUnitReq>();
    private Dictionary<string, DownLoadUnit> mWorkingPool = new Dictionary<string, DownLoadUnit>();
    private Dictionary<string, DownLoadUnit> mStoragePool = new Dictionary<string, DownLoadUnit>();
	//private Dictionary<string, GameObject> mLoadedResTable = new Dictionary<string, GameObject>();
}



