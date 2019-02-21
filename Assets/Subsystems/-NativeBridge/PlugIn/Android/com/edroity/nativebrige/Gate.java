package com.edroity.nativebrige;

import android.util.Log;
import com.unity3d.player.UnityPlayer;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

/**
 * Created by zhiyuan.peng on 2017/8/27.
 */

public class Gate {

    public static String TAG = "Gate";

    public static boolean isSandbox()
    {
        return false;
        //return (NativeGameManifestManager.tryGet("env", "production").equals("sandbox"));
    }

    // downstream notify
    public static void onNotify(String clazzName, String methodName, String arg)
    {
        if(isSandbox()) {
            Log.i(TAG, " -> onNotify " + clazzName + "." + methodName + " " + arg);
        }
        try
        {
            Class clazz = Class.forName("bridgeClass." + clazzName);
            Method method = clazz.getMethod(methodName, String.class);
            method.invoke(null, arg);
            if(isSandbox()) {
                Log.i(TAG, "notify: " + clazzName + "." + methodName + " compete");
            }
        }
        catch (Exception e) {
            Log.e(TAG, "error in onNotify, class:" + clazzName + ", msg: " + methodName + ", arg: " + arg + ", exception: " + e.getMessage());
            e.printStackTrace();
        }
    }

    // downsteram call
    public static void onCall(String callId, String clazzName, String msg, String arg)
    {
        if(isSandbox()) {
            Log.i(TAG, "onCall [" + callId + "] clazzName." + msg + " " + arg);
        }
        try
        {
            Class clazz = Class.forName("bridgeClass." + clazzName);
            Method method = clazz.getMethod(msg, String.class, String.class);
            method.invoke(null, callId, arg);
        }
        catch (Exception e) {
            Log.e(TAG, "error in onCall, class:" + clazzName + ", msg: " + msg + ", arg: " + arg + ", exception: " + e.getMessage());
            e.printStackTrace();
            callReturn(callId, "");
        }
    }


    public static void callReturn(String callId, String ret)
    {
        if(ret == null)
        {
            ret = "";
        }
        if(isSandbox()) {
            Log.i(TAG, " -> call [" + callId + "] returns " + ret);
        }
        UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnCallReturnSetId", callId);
        UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnCallReturnSetValue", ret);
        UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnCallReturnComplete", "");
    }

    // upstream notify
    public static void upstreamNotify(String clazz, String method, String arg)
    {
        Log.i(TAG, " -> upstream notify " + clazz + "." + method + " " + arg);
        UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnNotifySetClass", clazz);
        UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnNotifySetMethod", method);
        UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnNotifySetArg", arg);
        UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnNotifySend", "");
    }

    // upstream call
    public static int upstreamCallCount = 0;
    public static HashMap<String, CallResult> upstreamCallDic = new HashMap<>();
    public static List<String> csharpMethodList = new ArrayList<>();

    public static void upstreamCall(String clazz, String method, String arg, CallResult callback)
    {
        upstreamCallCount++;
        String callId = upstreamCallCount + "";
        upstreamCallDic.put(callId, callback);
        Log.i(TAG, " -> upstream call [" + callId + "] " + clazz + "." + method + " " + arg);
        if(csharpHasMethod(clazz, method))
        {
            UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnCallSetId", callId);
            UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnCallSetClass", clazz);
            UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnCallSetMethod", method);
            UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnCallSetArg", arg);
            UnityPlayer.UnitySendMessage("NativeBridgeReceiver", "OnCallInvoke", "");
        }
        else
        {
            Log.i(TAG, " csharp method " + clazz + "." + method + " not exists.");
            onUpstreamCallReturn(callId, "");
        }
    }

    public static void onUpstreamCallReturn(String callId, String result)
    {
        Log.i(TAG, " -> upstream call [" + callId + "] returns " + result);
        if(upstreamCallDic.containsKey(callId))
        {
            CallResult callback = upstreamCallDic.get(callId);
            upstreamCallDic.remove(callId);
            callback.onResult(result);
        }

    }

    public static void onRegisterCSharpMethod(String clazz, String method)
    {
        Log.i(TAG, " -> register csharp method " + clazz + "." + method);
        csharpMethodList.add(clazz + "." + method);
    }

    public static boolean csharpHasMethod(String clazz, String method)
    {
        return csharpMethodList.contains(clazz + "." + method);
    }

    // downstream syn call
    public static String onSynCall(String clazzName, String msg, String arg)
    {
        Log.i(TAG, "onSynCall " + clazzName + "." + msg + " " + arg);
        String ret = "";
        try
        {
            Class clazz = Class.forName("bridgeClass." + clazzName);
            Method method = clazz.getMethod(msg, String.class);
            Object retobj = method.invoke(null, arg);
            ret = (String)retobj;
        }
        catch (Exception e) {
            Log.e(TAG, "error in onSynCall, class:" + clazzName + ", msg: " + msg + ", arg: " + arg + ", exception: " + e.getMessage());
            e.printStackTrace();
        }
        return ret;
    }

}
