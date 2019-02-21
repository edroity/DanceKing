using UnityEngine;
using GameCore;

public class DownLoadUrl
{
	static string bundleResUrl;
	static private string baseResUrl;


	public static string BaseResUrl
	{
		get
		{
			return baseResUrl;
		}
	}
	public static void Init(string base_res_url, string bundle_res_url)
	{
		baseResUrl = base_res_url;
		bundleResUrl = bundle_res_url;
	}

		
	public static string BaseURL(ResourceType type)
	{
	    switch(type)
		{
			case ResourceType.RT_STREAM:	
			case ResourceType.RT_ASSETBUNDLE: return bundleResUrl;
			case ResourceType.RT_TEXT:
			case ResourceType.RT_TEXTURE_JPG:
			case ResourceType.RT_TEXTURE_PNG: 
			case ResourceType.RT_ZIP:
			default :return baseResUrl + "/";
		}

	}
}
	

