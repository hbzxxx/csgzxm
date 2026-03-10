using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddStudentRecruitNumADPanel : PanelBase
{
    public Button btn_addRecruitLimit;
    public override void Init(params object[] args)
    {
        base.Init(args);

        addBtnListener(btn_addRecruitLimit, () =>
        {
            if (RoleManager.Instance._CurGameInfo.studentData.thisYearWatchedADNum)
            {
                PanelManager.Instance.OpenFloatWindow("今年已经用过金手指了，明年再来哦");
                return;
            }
            ADManager.Instance.WatchAD(ADType.ADRecruitStudentNum);
            PanelManager.Instance.ClosePanel(this);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

    }
}
