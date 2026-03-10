using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PeopleTipsPanel : PanelBase
{
    PeopleData p;
    public Transform grid;
    public Text txt_zhanDouLi;
    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        ClearCertainParentAllSingle<SinglePropertyView>(grid);

        for (int i = 0; i < p.curBattleProList.Count; i++)
        {
            if (p.curBattleProIdList[i] != (int)PropertyIdType.MpNum)
                PanelManager.Instance.OpenSingle<BigStudentSinglePropertyView>(grid, p.curBattleProList[i].id, p.curBattleProList[i].num, (Quality)(int)p.curBattleProList[i].quality);
        }
        txt_zhanDouLi.SetText("战力" + RoleManager.Instance.CalcZhanDouLi(p));
    }
}
