using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 7日签到
/// </summary>
public class SevenDayQianDaoPanel : PanelBase
{
    public Transform grid;
    public Transform grid_seven;
    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        int i = 0;
        for (i = 0; i < 6; i++)
        {
            AddSingle<SingleSevenDayQianDaoView>(grid,i+1);
        }
        AddSingle<SingleSevenDayQianDaoE>(grid_seven, i + 1);
    }
}
