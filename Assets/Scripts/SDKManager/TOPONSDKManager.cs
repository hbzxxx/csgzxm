#if TOPONSDK
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TOPONSDKManager : CommonInstance<TOPONSDKManager>
{
    string appId = "a618b74d3d45e2";
    string appKey = "66a26e49ba92be4a5d229c325024f649";

    string mPlacementId_rewardvideo_all = "b618b761b9241a";
    //public ATCallbackListener callbackListener;

    public override void Init()
    {
        base.Init();
        ////（可选配置）设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（App纬度）
        ////注意：调用此方法会清除setChannel()、setSubChannel()方法设置的信息，如果有设置这些信息，请在调用此方法后重新设置
        //ATSDKAPI.initCustomMap(new Dictionary<string, string> { { "unity3d_data", "test_data" } });

        ////（可选配置）设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（Placement纬度）
        //ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "unity3d_data_pl", "test_data_pl" } }, placementId);

        ////（可选配置）设置渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的广告数据
        ////注意：如果有使用initCustomMap()方法，必须在initCustomMap()方法之后调用此方法
        //ATSDKAPI.setChannel("unity3d_test_channel");

        ////（可选配置）设置子渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的子渠道广告数据
        ////注意：如果有使用initCustomMap()方法，必须在initCustomMap()方法之后调用此方法
        //ATSDKAPI.setSubChannel("unity3d_test_subchannel");

        //设置开启Debug日志（强烈建议测试阶段开启，方便排查问题）
        //ATSDKAPI.setLogDebug(true);

        ////判断是否在欧盟地区
        //ATSDKAPI.getUserLocation(new GetLocationListener());

        ////（必须配置）SDK的初始化
        //ATSDKAPI.initSDK(appId, appKey, new InitListener());//Use your own app_id & app_key here

        ////加载激励视频
        //loadVideo();

    }


    ////发布欧盟地区的开发者需使用以下授权代码，询问用户是否同意收集隐私数据
    //private class GetLocationListener : ATGetUserLocationListener
    //{
    //    public void didGetUserLocation(int location)
    //    {
    //        Debug.Log("Developer callback didGetUserLocation(): " + location);
    //        if (location == ATSDKAPI.kATUserLocationInEU && ATSDKAPI.getGDPRLevel() == ATSDKAPI.UNKNOWN)
    //        {
    //            ATSDKAPI.showGDPRAuth();
    //        }
    //    }
    //}
    public void showVideo()
    {
  
        Debug.Log("Developer show video....");
        //AddQQManager.Instance.CallAndroidMethod<>
        AddQQManager.Instance.CallAndroidMethod("PlayVideo");
        //ATRewardedVideo.Instance.showAd(mPlacementId_rewardvideo_all);
    }
    public void PlayVideo()
    {
        //if (isReady)
        //{
        //    showAutoAd();
        //}
        //if (ATRewardedVideo.Instance.hasAdReady(mPlacementId_rewardvideo_all))
        //{
            Debug.Log("有广告 播放广告");
            GameTimeManager.Instance.GetServiceTime((x) =>
            {
                if (x > 0)
                {
                    RoleManager.Instance._CurGameInfo.timeData.LastADWatchTime = x;
                }
            });
            showVideo();
        //}
        //else
        //{
        //    Debug.Log("没有广告 重新加载");
        //    loadVideo();

        //}
    }

    //public void loadVideo()
    //{
    //    if (callbackListener == null)
    //    {
    //        callbackListener = new ATCallbackListener();
    //        Debug.Log("Developer init video....placementId:" + mPlacementId_rewardvideo_all);
    //        ATRewardedVideo.Instance.setListener(callbackListener);
    //    }

    //    //ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "placement_custom_key", "placement_custom" } }, mPlacementId_rewardvideo_all);

    //    Dictionary<string, string> jsonmap = new Dictionary<string, string>();
    //    //jsonmap.Add(ATConst.USERID_KEY, "test_user_id");
    //    //jsonmap.Add(ATConst.USER_EXTRA_DATA, "test_user_extra_data");


    //    ATRewardedVideo.Instance.loadVideoAd(mPlacementId_rewardvideo_all, jsonmap);

    //}
    //private class InitListener : ATSDKInitListener
    //{
    //    public void initSuccess()
    //    {
    //        Debug.Log("Developer Develop callback SDK initSuccess");
    //    }
    //    public void initFail(string msg)
    //    {
    //        Debug.Log("Developer callback SDK initFail:" + msg);
    //    }
    //}
}
#endif