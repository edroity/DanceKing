using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using GameCore;
using System.IO;
using System.Linq;
public delegate void TextureLoadedCallBack(Texture2D texture);
public class TextureMgr:Single<TextureMgr>,IProcess
{
	private List<string> keys = new List<string>();
	private Dictionary<string, Texture2D> mTextureTable = new Dictionary<string, Texture2D>();
	private Dictionary<string, List<TextureLoadedCallBack>> mCallBackTable = new Dictionary<string, List<TextureLoadedCallBack>>();

	public void LoadIconByPicID(int pid, TextureLoadedCallBack textureLoaded)
	{

		LoadTexture("Texture/Icon/" + pid, "Texture/Icon/0", textureLoaded);
	}

	public void LoadIconByName(string picName, TextureLoadedCallBack textureLoaded)
	{
		if (string.IsNullOrEmpty(picName))
			return;
		LoadTexture("Texture/Icon/" + picName, "Texture/Icon/0", textureLoaded);
	}

	public  void LoadTexture(string path, string default_path, TextureLoadedCallBack textureLoaded, ResourceType resType = ResourceType.RT_TEXTURE_PNG)
	{
		string filePath = GameInfo.FilePath + path;
		if (resType == ResourceType.RT_TEXTURE_PNG)
			filePath += ".png";
		else
			filePath += ".jpg";

		bool loaded = false;
		Texture2D tex = ResourceMgr.Instance.LoadRes<Texture2D>(path);
		if (textureLoaded != null)
			textureLoaded(tex);
		if (tex != null || path.Equals(default_path))return;

		if (!mCallBackTable.ContainsKey(path)) {
			DownLoader.Instance.Request(path, resType, ThreadPriority.Normal);
			mCallBackTable.Add(path, new List<TextureLoadedCallBack>());
		}
		mCallBackTable[path].Add(textureLoaded);
		ProcessManager.Add(Instance);
		
	}
	
	public void Clear()
	{
		foreach (List<TextureLoadedCallBack> callbacks in mCallBackTable.Values) {
			callbacks.Clear();
		}
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

	public void Update(float deltaTime)
	{
		if (mCallBackTable.Count == 0)
			return;
		keys.Clear();
		foreach (string key in mCallBackTable.Keys) {
			keys.Add(key);
		}
		foreach (string key in keys) {
			if (!mCallBackTable.ContainsKey(key))
				continue;
			List<TextureLoadedCallBack> callbacks = mCallBackTable[key];
			if (DownLoader.Instance.Loaded(key)) {
				mCallBackTable.Remove(key);
				DownLoadUnit dlu = DownLoader.Instance.Fetch(key);
				if (dlu != null) {
					if (dlu.GetWWW().error == null) {	
						Texture2D texture = DownLoadResultFetcher.Instance.FetchTex(dlu);
						string filePath = GameInfo.FilePath + key + dlu.mReq.mExtention;
						string fileName = Path.GetFileName(filePath);
						DownLoadResultFetcher.Instance.WriteTexture(dlu, key);
						ResourceMgr.Instance.AddResource(key + dlu.mReq.mExtention, texture as Object);

						foreach (TextureLoadedCallBack cb in callbacks) {
							cb(texture);
						}
					} else {
						Debug.LogWarning(dlu.GetWWW().error + "  " + dlu.GetUrl());
					}
					dlu.ReleaseRes();
				}

			}
		}
	}
}

