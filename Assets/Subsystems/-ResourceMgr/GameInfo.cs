using UnityEngine;
using System.Collections.Generic;
using System;

public static class GameInfo
{
    private static string catchFilePath = "";

    public static void Init()
    {
        string filePath = "";
        #if UNITY_IPHONE
        filePath = Application.temporaryCachePath;
        #else
        filePath = Application.persistentDataPath;
        #endif
        catchFilePath = filePath + "/";
    }

    public static string VersionName
    {
        get
        {
			int v = Version;
			return (v/100) + "." + (v%100/10) + "." + (v%10);
        }
    }

	public static int Version
	{
		get
		{
			int version = 0;
			var str = GameManifestFinal.Get("version");
			int.TryParse(str, out version);
			return version;
		}
	}

	public static int Build
	{
		get
		{
			int version = 0;
			var str = GameManifestFinal.Get("build");
			int.TryParse(str, out version);
			return version;
		}
	}

    public static int ResVersion
    {
        get
        {
			int version = 0;
			var str = GameManifestFinal.Get("res_version");
			int.TryParse(str, out version);
			return version;
        }
    }

//	public static int DataVersion
//	{
//		get
//		{
//			int version = 0;
//			var str = GameManifestFinal.Get("data_version");
//			int.TryParse(str, out version);
//			return version;
//		}
//	}

    public static string HTTPSchame
    {
        get
        {
            return "http";
        }
    }


    public static string FilePath
    {
        get
        {
            return catchFilePath;
        }
    }

    public static bool UserLocalAssetBundle
    {
        get
        {
            return false;
        }
    }

    public static int SwitchDayHour
    {
        get
        {
            return 5;
        }
    }


    public static Region Region
    {
        get
        {
            var str = GameManifestFinal.Get("region", "default");
            switch (str)
            {
                case "default":
                    return Region.Default;
                case "cn":
                    return Region.CN;
                case "en":
                    return Region.EN;
                case "eu":
                    return Region.EU;
                case "tw":
                    return Region.TW;
                case "jp":
                    return Region.JP;
                case "us":
                    return Region.US;
                default:
                    throw new Exception("unknown region: " + str);
            }

        }
    }

    public static string Platform
    {
        get
        {
            #if UNITY_IPHONE
            return "ios";
            #elif UNITY_ANDROID
			return "android";
            #else
			return "other";
            #endif

		}

	}
 
    public static int PackcageVersion
    {
        get
        {
            return 0;
        }
    }

    public static string RegionString
	{
		get
		{
            return GameManifestFinal.Get("region", "default");
		}
	}

    public static Env Env
	{
		get
		{
            var str = GameManifestFinal.Get("env", "sandbox");
            switch(str)
            {
                case "sandbox":
                    return Env.Sandbox;
                case "production":
                    return Env.Prodution;
                default:
                    throw new Exception("unknown env: " + str);
            }
		}
	}

    public static int TimeZone
	{
		get
		{
			return 5;
		}
	}
}

public enum Region
{
    Default,
    CN,
    TW,
    EN,
    EU,
    JP,
    US
}

public enum Env
{
    Prodution,
    Sandbox
}