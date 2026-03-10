using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventRecruitStudentPanel : PanelBase
{

    public Transform trans_grid;
    PeopleData p;

    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        RegisterEvent(TheEventType.SuccessRecruit, OnSuccessRecruit);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ClearCertainParentAllSingle<CandidateStudentView>(trans_grid);
        AddSingle<CandidateStudentView>(trans_grid,p, RecruitStudentType.MapEvent);
    }

    void OnSuccessRecruit(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;

        if(p.onlyId== theP.onlyId)
            PanelManager.Instance.ClosePanel(this);
    
    }

    public override void Clear()
    {
        base.Clear();
    }


}
