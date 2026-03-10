using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductPropertyDesPanel : PanelBase
{

    public Transform grid;
    public PeopleData p;
    public Text txt_shuxin;
    public override void Init(params object[] args)
    {
        base.Init(args);
        p = (PeopleData)args[0];
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        StudentTalent curTalent = (StudentTalent)p.talent;
        txt_shuxin.SetText("当前属下属性为:"+StudentManager.Instance.TalentNameByTalent(curTalent));
        AddSingle<SingleProductProView>(grid, curTalent);
        StudentTalent[] allTalents = new StudentTalent[]
        {
            StudentTalent.LianJing,
            StudentTalent.DuanZhao,
            StudentTalent.CaiKuang,
            StudentTalent.ChaoYao,
            StudentTalent.JingWen,
            StudentTalent.BaoShi,
            StudentTalent.JingShang
        };
        for (int i = 0; i < allTalents.Length; i++)
        {
            if (allTalents[i] != curTalent)
            {
                AddSingle<SingleProductProView>(grid, allTalents[i]);
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
    }
}
