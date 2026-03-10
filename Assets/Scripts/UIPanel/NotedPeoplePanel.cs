using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotedPeoplePanel : PanelBase
{
    public Transform grid;
    
    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        for(int i = 0; i < RoleManager.Instance._CurGameInfo.NotedPeopleData.NotedList.Count; i++)
        {
            AddSingle<SingleStudentView>(grid, RoleManager.Instance._CurGameInfo.NotedPeopleData.NotedList[i].P);

        }
    }

    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleStudentView>(grid);
    }
}
