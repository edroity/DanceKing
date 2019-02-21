
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Linq;
using GameCore;
using System.Threading;

public class DownLoadResultFetcher:IProcess
{
	public List<string> workingThreadList=new List<string>();
	public List<string> endThreadList=new List<string>();
	private Queue<UPKData> upkQueue = new Queue<UPKData>(); 

	public void Start()
	{
		
	}

	public int MaxThreadCount
	{
		get
		{

			#if UNITY_IPHONE
			return 5;
			#elif UNITY_ANDROID
			if(SystemInfo.systemMemorySize <= 1024)
			return 3;
			else return 5;
			#else
			return 5;
			#endif
		}
	}
	public void Update(float deltaTime)
	{
		lock(endThreadList)
		{
			if(endThreadList.Count>0)
			{
				for(int i=0; i< endThreadList.Count;i++)
				{
					ThreadMgr.Instance.End(endThreadList[i]);
					workingThreadList.Remove(endThreadList[i]);
				}
				endThreadList.Clear();
			}
		}

		if(upkQueue.Count>0 && workingThreadList.Count <= MaxThreadCount)
		{
			UPKData data =upkQueue.Dequeue();
			workingThreadList.Add(data.key);
			ThreadMgr.Instance.Start(data.key,new System.Threading.Thread(new  System.Threading.ParameterizedThreadStart(UnFoldUpkThread)),data as object);
		}

	}
	public void End()
	{
		upkQueue.Clear();
	}
	public bool IsFinished()
	{
		return upkQueue.Count==0;
	}


    #region single
    static DownLoadResultFetcher mInstance = null;
    static public DownLoadResultFetcher Instance
    {
		get
		{
	        if (mInstance == null)
	        {
	            mInstance = new DownLoadResultFetcher();
	        }
	        return mInstance;
		}
    }
    #endregion
    public string FetchString(DownLoadUnit dlu)
    {
        if (dlu.Loaded())
        {
            return dlu.GetWWW().text;
        }
        else
        {
            return null;
        }
    }

    public XmlDocument FecthXml(DownLoadUnit dlu)
    {
        if (dlu.Loaded())
        {
            XmlDocument tempXml = new XmlDocument();
            string val = dlu.GetWWW().text;
            ////bom判断
            if (val[0] != '<')
            {
                val = val.Substring(1, val.Length - 1);
            }
            tempXml.LoadXml(val);
            return tempXml;
        }
        else
        {
            return null;
        }
    }
	
    public byte[] FetchByte(DownLoadUnit dlu)
    {
        if (dlu.Loaded())
        {
            return dlu.GetWWW().bytes;
        }
        else
        {
            return null;
        }
    }
    public AssetBundle FetchAssetBundle(DownLoadUnit dlu)
    {
        if (dlu.Loaded())
        {
            if (dlu.GetWWW().error == null)
            {
                return dlu.GetWWW().assetBundle;
            }
        }
        return null;
    }
    public Texture2D FetchTex(DownLoadUnit dlu)
    {
        if (dlu.Loaded())
        {
            if (dlu.GetWWW().error == null)
            {
				return dlu.GetWWW().texture;
            }

        }
        return null;
    }
	
	public Texture2D FetchTexAlone(DownLoadUnit dlu)
    {
        if (dlu.Loaded())
        {
            if (dlu.GetWWW().error == null)
            {
                return dlu.GetWWW().textureNonReadable;
            }

        }
        return null;
    }
	
    public AudioClip FetchAudio(DownLoadUnit dlu)
    {
        if (dlu.Loaded())
        {
            if (dlu.GetWWW().error == null)
            {
                return dlu.GetWWW().GetAudioClip();
            }

        }
        return null;
    }
		
	private void WriteAssetBundleThread(object p)
	{
		BundleData param = p as BundleData;
		byte[] bytes = param.data;
		using (FileStream fs = File.OpenWrite (param.tarPath))
		{
			fs.Write (bytes, 0, bytes.Length);
			fs.Close();
			fs.Dispose();
		}
		param.cb(true);

	}

	private void UnFoldUpkThread(object p)
	{

		UPKData param = p as UPKData;
		string tarDir = Path.GetDirectoryName(param.tarPath);

		string md5 = Util.Instance.SimpleMD5CryptoServiceProvider(param.data);
		if(!param.tarPath.Contains(md5))
		{
			string tmd5 = Util.Instance.SimpleMD5CryptoServiceProvider(param.data);
			Debug.LogError(param.tarPath+"!=md5:"+md5);
			param.cb(UPK_LOADED_STATE.MD5_ERR);
			return;
		}
		if(!Directory.Exists(tarDir))Directory.CreateDirectory(tarDir);
   //     try
   //     {
			//SevenZipHelper.Instance.DecompressFileLZMA(param.data,param.tarPath);
			//if(!Directory.Exists(param.tarDir))Directory.CreateDirectory(param.tarDir);
			//UPKHelper.Instance.UnPackFolder(param.tarPath,param.tarDir);
        //}
        //catch(System.Exception e)
        //{
        //    Debug.Log("some exception occured: " + e.Message);
        //    param.cb(UPK_LOADED_STATE.MD5_ERR);
        //    return;
        //}
        //finally
        //{
        //    if(File.Exists(param.tarPath))
        //    {
        //        File.Delete(param.tarPath);
        //    }
        //}
		param.cb(UPK_LOADED_STATE.SUC);
		lock(endThreadList)
		{
			endThreadList.Add(param.key);
		}

	}
	public class UPKData
	{
		public byte[] data;
		public string tarPath;
		public string tarDir;
		public string key;
		public UpkLoadedCallBack cb;
	}
	public bool WriteUpk(DownLoadUnit dlu, string key,UpkLoadedCallBack cb)
	{

		if(!Application.temporaryCachePath.Equals(""))
		{

			UPKData bd = new UPKData();
	
			bd.data = dlu.GetWWW ().bytes;
			if(bd.data !=null && bd.data.Length >0)
			{
				bd.tarDir =  GameInfo.FilePath;
				bd.tarPath = GameInfo.FilePath+dlu.mReq.mUrl.Replace(".unity3d",".txt");
				bd.key = key;
				bd.cb = cb;
				upkQueue.Enqueue(bd);
				ProcessManager.Add(this);
				return true;
			}
		}
		return false;
	}
	public class BundleData
	{
		public byte[] data;
		public string tarPath;
		public string key;
		public AssetBundleLoadedCallBack cb;
	}


	public bool WriteAssetbundle(DownLoadUnit dlu, string key)
	{
		string filePath = GameInfo.FilePath + "AssetBundles_"+GameInfo.RegionString+"/"+key;
		if(!string.IsNullOrEmpty(GameInfo.FilePath) &&!File.Exists(filePath))
		{
			string fileName = Path.GetFileName(filePath);
			string directoryName = Path.GetDirectoryName(filePath);
			Debug.Log("filePath:"+filePath);
			if(!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			byte[] bytes = dlu.GetWWW ().bytes;
			if(bytes!=null && bytes.Length >0)
			{
				using (FileStream fs = File.OpenWrite (filePath))
				{
					fs.Write (bytes, 0, bytes.Length);
					fs.Close();
					fs.Dispose();
				}
				return true;
			}
		}
		return false;
	}
	
	public bool WriteTexture(DownLoadUnit dlu, string key)
	{
		string filePath = GameInfo.FilePath + key + dlu.mReq.mExtention;
		if(!string.IsNullOrEmpty(GameInfo.FilePath) && !Application.temporaryCachePath.Equals("") && !File.Exists(filePath))
		{
			string directoryName = Path.GetDirectoryName(filePath);
			if(!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			string extension = Path.GetExtension(filePath);
			byte[] bytes = DownLoadResultFetcher.Instance.FetchByte (dlu);
			using (FileStream fs = File.OpenWrite (filePath))
			{
				fs.Write (bytes, 0, bytes.Length);
				fs.Close();
				fs.Dispose();
			}
			return true;
		}
		return false;
	}

}
