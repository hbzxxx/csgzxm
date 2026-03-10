using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;

public class StudentGetNewSkillPanel : PanelBase
{
    public Portrait portrait;
    public Text txt;

    public Transform trans_skillGrid;

    public override void Init(params object[] args)
    {
        base.Init(args);

        PeopleData p = args[0] as PeopleData;
        SingleSkillData skill=args[1] as SingleSkillData;
        txt.SetText(p.name + "习得" + DataTable.FindSkillSetting(skill.skillId).Name);
        portrait.Refresh(p);
        AddSingle<SingleSkillView>(trans_skillGrid, skill, SkillViewType.Show);

    }

    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleSkillView>(trans_skillGrid);
    }
}
