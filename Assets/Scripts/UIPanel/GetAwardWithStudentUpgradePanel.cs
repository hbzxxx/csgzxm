using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GetAwardWithStudentUpgradePanel : GetAwardPanel
{
    public Transform trans_studentGrid;//弟子经验
    List<int> studentBeforeExpList;
    List<PeopleData> studentList;

    public override void Init(params object[] args)
    {
        base.Init(args);
        studentBeforeExpList = args[1] as List<int>;
        studentList=args[2] as List<PeopleData>;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (studentList == null)
            return;
        for(int i=0;i< studentList.Count; i++)
        {
            int beforeExp = studentBeforeExpList[i];
            PeopleData p = studentList[i];
            PanelManager.Instance.OpenSingle<SingleUpgradeStudentView>(trans_studentGrid, beforeExp, p);
        }
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_studentGrid);
    }
}
