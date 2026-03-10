using Framework.Data;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareManager : CommonInstance<ShareManager>
{
    public GameInfo gameInfo;
    public override void Init()
    {
        base.Init();
        gameInfo = RoleManager.Instance._CurGameInfo;
    }
    public string ShareTxt()
    {
        
return "";
    }
     
}
