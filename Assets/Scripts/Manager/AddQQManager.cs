 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddQQManager : CommonInstance<AddQQManager>
{
    private static string AndroidKey = "auofEKqwufakC3qgkKpGY9NVFB8ZfEvs";
 
     private static readonly string iOSUid = "655538805";
     private static readonly string iOSKey = "be6e6496097d1c603dffa0017ba94950b33c12e784e48046ed7283f3790c3a32";
 
     private AndroidJavaClass _jc;
     private AndroidJavaObject _jo;

    public void OnJoinQQGroup()
    {
        //Game.Instance.clientManager.SendRT(NetCmd.EntityRpc, "JOINQQGROUP");

        //if (!JoinQQGroup())
        //{
        //    PanelManager.Instance.OpenFloatWindow("未安装手Q或者版本不支持！");
        //}
    }
    public void OnReallyJoinQQGroup(string key)
    {
        AndroidKey = key;
        if (!JoinQQGroup())
        {
            PanelManager.Instance.OpenFloatWindow("未安装手Q或者版本不支持！");
        }
    }
    /// <summary>
    /// 加入QQ群的方法，有返回值，代表成功或者失败
    /// </summary>
    /// <returns></returns>
    private bool JoinQQGroup()
     {
 #if !UNITY_EDITOR && UNITY_ANDROID
         return CallAndroidMethod<bool>("joinQQGroup", AndroidKey);
 #elif !UNITY_EIDTOR && UNITY_IOS
         return iOSJoinQQGroup(iOSKey, iOSUid);
 #else
         return false;
 #endif
     }
 
     /// <summary>
     /// 调用一个带有返回值的原生Android方法
     /// </summary>
     /// <typeparam name="ReturnType"></typeparam>
     /// <param name="method"></param>
     /// <param name="args"></param>
     /// <returns></returns>
     public ReturnType CallAndroidMethod<ReturnType>(string method, params object[] args)
     {
       _jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
       _jo = _jc.GetStatic<AndroidJavaObject>("currentActivity");
#if !UNITY_EDITOR && UNITY_ANDROID
         return _jo.Call<ReturnType>(method, args);
#endif
        return default(ReturnType);
     }

    /// <summary>
    /// 调用一个无返回值的原生Android方法
    /// </summary>
    /// <typeparam name="ReturnType"></typeparam>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public void CallAndroidMethod(string method, params object[] args)
    {
        _jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _jo = _jc.GetStatic<AndroidJavaObject>("currentActivity");

#if !UNITY_EDITOR && UNITY_ANDROID
           _jo.Call(method, args);
#endif
        return ;
    }
    //iOS方法导入
#if !UNITY_EDITOR && UNITY_IOS
     [DllImport("__Internal")]
     private static extern bool iOSJoinQQGroup(string key, string uid);
#endif
}
