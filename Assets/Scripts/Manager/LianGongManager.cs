using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class LianGongManager : CommonInstance<LianGongManager>
{
   
    /// <summary>
    /// 增加一个练功弟子
    /// </summary>
    public void AddALianGongStudent(PeopleData p)
    {
        p.studentCurEnergy = 100;
        p.talent = (int)StudentTalent.LianGong;
       // RoleManager.Instance._CurGameInfo.StudentData.LianGongStudentList.Add(p);

    }
}
