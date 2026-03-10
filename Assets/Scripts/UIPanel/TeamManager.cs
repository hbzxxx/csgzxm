
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : CommonInstance<TeamManager>
{
    public GameInfo gameInfo;
    public override void Init()
    {
        base.Init();
        gameInfo = RoleManager.Instance._CurGameInfo;
    }


    public int FindMyTeam1PNum()
    {
        int res = 0;
        for(int i = 0; i < gameInfo.AllTeamData.TeamList1.Count; i++)
        {
            if (gameInfo.AllTeamData.TeamList1[i] > 0)
                res++;
        }
        return res;
    }
}
