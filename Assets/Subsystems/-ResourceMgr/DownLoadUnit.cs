using UnityEngine;
using System.IO;

public enum ResourceType
{
	RT_BYTE = 0,
	RT_TEXT,
	RT_TEXTURE_PNG,
	RT_TEXTURE_JPG,
	RT_ASSETBUNDLE,
	RT_ZIP,
	RT_STREAM
}

public class DownLoadUnitReq
{
    public string mUrl = "";
	public string mExtention 
	{
		get
		{
			switch(mType)
			{
				default:
				case ResourceType.RT_ASSETBUNDLE: return ".unity3d";
				case ResourceType.RT_TEXTURE_PNG: return ".png";
				case ResourceType.RT_TEXTURE_JPG: return ".jpg";
				case ResourceType.RT_ZIP: return ".gz";
				case ResourceType.RT_TEXT: return ".txt";
				case ResourceType.RT_STREAM: return "";
			}
		}
	}
	public bool noticeError;
	//public bool forceLoadFromSource=false;
	//public int mVersion = 0;
	public ResourceType mType = ResourceType.RT_ASSETBUNDLE;
    public ThreadPriority mPriority = ThreadPriority.Normal;
}
public class DownLoadUnit
{
    protected WWW mWWW = null;
	public DownLoadUnitReq mReq;
    public DownLoadUnit(DownLoadUnitReq req)
    {
		mReq = req;
		string filePath = GameInfo.FilePath + req.mUrl + req.mExtention;
	

		if( req.mType == ResourceType.RT_ASSETBUNDLE)
		{
			string url = DownLoadUrl.BaseURL(req.mType) + req.mUrl;
			if(!url.EndsWith(req.mExtention))url+=req.mExtention;
            Debug.Log("LoadAssetBundle>>>>>>>>>>>>>>>>>:"+url);
			mWWW = new WWW(url);

		}
		else if( req.mType == ResourceType.RT_STREAM)
		{
			string streamPath = DownLoadUrl.BaseURL(req.mType) + req.mUrl;
			if(!streamPath.EndsWith(req.mExtention))streamPath+=req.mExtention;
            Debug.Log("streamPath>>>>>>>>>>>>>>>>>:"+streamPath);
			mWWW = new WWW(streamPath);

		}

		else if(File.Exists(filePath))
		{
			Debug.Log("WWWFromFilePath>>>>>>>>>>>>>>>>>:"+filePath);
			mWWW = new WWW("file:///"+filePath);
		}
		else 
		{

			string url = DownLoadUrl.BaseURL(req.mType) + req.mUrl;
			if(!url.EndsWith(req.mExtention))url+=req.mExtention;
            Debug.Log("WWW>>>>>>>>>>>>>>>>>:"+url);
			mWWW = new WWW(url);
		}
		mWWW.threadPriority = req.mPriority;
    }
	
    public void ReleaseRes()
    {
        mWWW.Dispose();
        mWWW = null;
    }
	
    public bool Loaded()
    {
        if ( mWWW != null)
        {
            return  mWWW.isDone;
        }
        else
        {
            return false;
        }
    }
	
    public float GetProgress()
    {
        if ( mWWW == null)
        {
            return 0.0f;
        }
        else
        {
            return  mWWW.progress;
        }
    }
	
    public WWW GetWWW()
    {
        return mWWW;
    }
	
    public string GetUrl()
    {
        return mWWW.url;
    }
}

