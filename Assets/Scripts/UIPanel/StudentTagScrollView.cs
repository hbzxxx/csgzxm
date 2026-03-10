using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentTagScrollView : MultipleTagScrollView
{
    public bool handle = false;

    public override void Init(params object[] args)
    {
        base.Init(args);
        handle = (bool)args[0];
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        OnTagBtnClick(0);
    }

    public override void OnTagBtnClick(int index)
    {
        base.OnTagBtnClick(index);
        PanelManager.Instance.CloseAllSingle(trans_scrollGrid);
        PeopleData p = null;

        //switch (index)
        //{
        //    //全部 炼丹 炼器 练功
        //    case 0:
        //        for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Count; i++)
        //        {
        //            if (!handle)
        //                PanelManager.Instance.OpenSingle<SingleStudentView>(trans_scrollGrid, RoleManager.Instance._CurGameInfo.StudentData.AllStudentList[i]);
        //            else
        //                PanelManager.Instance.OpenSingle<SingleHandleStudentView>(trans_scrollGrid, RoleManager.Instance._CurGameInfo.StudentData.AllStudentList[i]);

        //        }
        //        break;
        //        //炼丹
        //    case 1:
        //        for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Count; i++)
        //        {
        //            p = RoleManager.Instance._CurGameInfo.StudentData.AllStudentList[i];
        //            if (p.Talent == (int)StudentTalent.LianDan)
        //            {
        //                if (!handle)
        //                    PanelManager.Instance.OpenSingle<SingleStudentView>(trans_scrollGrid, p);
        //                else
        //                    PanelManager.Instance.OpenSingle<SingleHandleStudentView>(trans_scrollGrid, p);

        //            }

        //        }
        //        break;
        //        //炼器
        //    case 2:

        //        for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Count; i++)
        //        {
        //            p = RoleManager.Instance._CurGameInfo.StudentData.AllStudentList[i];
        //            if (p.Talent == (int)StudentTalent.EquipMake)
        //            {
        //                if (!handle)
        //                    PanelManager.Instance.OpenSingle<SingleStudentView>(trans_scrollGrid, p);
        //                else
        //                    PanelManager.Instance.OpenSingle<SingleHandleStudentView>(trans_scrollGrid, p);

        //            }


        //        }
        //        break;
        //        //亲传弟子
        //    case 3:

        //        for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Count; i++)
        //        {
        //            p = RoleManager.Instance._CurGameInfo.StudentData.AllStudentList[i];
        //            if (p.Talent == (int)StudentTalent.LianGong)
        //            {
        //                if (!handle)
        //                    PanelManager.Instance.OpenSingle<SingleStudentView>(trans_scrollGrid, p);
        //                else
        //                    PanelManager.Instance.OpenSingle<SingleHandleStudentView>(trans_scrollGrid, p);

        //            }


        //        }
        //        break;
        //}


    }
}
