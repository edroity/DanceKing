using UnityEngine;
using System.Collections.Generic;
using GameCore;
using System.IO;
using System.Linq;
using System.Xml;

public class ResLifeMgr:Single<ResLifeMgr>
{
	public static int LIFE_TIMES = 3;
	private Dictionary<string, int> mLifeTime = new Dictionary<string,  int>();
	public void Live(string path)
	{
		mLifeTime[path] = LIFE_TIMES;
	}
	
	public void Die(string path)
	{
		mLifeTime[path] = -1;
	}
	public void DieAll()
	{
		List<string> keys = mLifeTime.Keys.ToList();
		foreach(string key in keys)
		{
			mLifeTime[key]=-1;
		}
	}


	public bool canRelease(string key)
	{
		return mLifeTime.ContainsKey(key) &&  mLifeTime[key] <= 0;
	}
	
	public void Remove(string path)
	{
		mLifeTime.Remove(path);
	}
	
	public void Refresh()
	{

		List<string> keys = mLifeTime.Keys.ToList();
		foreach(string key in keys)
		{
			mLifeTime[key]--;
		}
	}
}
