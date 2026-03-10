 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKManager : CommonInstance<SDKManager>
{
 

    public override void Init()
    {
        base.Init();

#if TOPONSDK
        TOPONSDKManager.Instance.Init();
#endif

    }
  
    public void showVideo()
    {
#if TOPONSDK


        TOPONSDKManager.Instance.PlayVideo();
#endif

    }


}
