 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuiHuanMaManager : CommonInstance<DuiHuanMaManager>
{
    public GameInfo gameInfo;

    public override void Init()
    {
        base.Init();
        gameInfo = RoleManager.Instance._CurGameInfo;
        if (gameInfo.AllDuiHuanMaData == null)
        {
            gameInfo.AllDuiHuanMaData = new AllDuiHuanMaData();
        }
    }
}
