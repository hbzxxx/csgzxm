using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentAddExpPanel : PanelBase
{
    public List<int> beforeExpList;
    public List<PeopleData> pList;

    public Transform trans_studentGrid;

    public override void Init(params object[] args)
    {
        base.Init(args);
        beforeExpList = args[0] as List<int>;
        pList = args[1] as List<PeopleData>;

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();


        ClearCertainParentAllSingle<SingleUpgradeStudentView>(trans_studentGrid);

        for (int i = 0; i < pList.Count; i++)
        {
            AddSingle<SingleUpgradeStudentView>(trans_studentGrid, beforeExpList[i], pList[i]);
        }
    }

}
