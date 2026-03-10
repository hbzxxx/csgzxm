using Plugins.AntiAddictionUIKit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FangChenMiManager : CommonInstance<FangChenMiManager>
{
 

    public bool canEnter = false;
    public override void Init()
    {
        canEnter = true;
//#if UNITY_EDITOR
        canEnter = true;
//#elif TOPONSDK
//        TapFangChenMiManager.Instance.Init();
//#endif
        //canEnter = true;

    }
}
