using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;
/// <summary>
/// 招募候选
/// </summary>
public class CandidateStudentView : BigStudentView
{

    
    public Button btn_recruit;//招募

    public Transform trans_consumeGrid;
    RecruitStudentType recruitStudentType; 

    public Text txt_name;
    public override void Init(params object[] args)
    {
        base.Init(args);
        recruitStudentType = (RecruitStudentType)args[1];
        addBtnListener(btn_recruit, () =>
        {
            StudentManager.Instance.RecruitStudent(p, recruitStudentType);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        //性格
        if (txt_rarity != null)
            txt_rarity.SetText(ConstantVal.QualityName(p.talentRarity) + "品级");

        if (txt_xingGe != null)
        {
            txt_xingGe.SetText("性格:" + ConstantVal.SocialActivityName(p.socializationData.socialActivity));
        }

        if (txt_yanZhi != null)
        {
            int yanZhi = p.studentQuality;
            txt_yanZhi.SetText("颜值:" + yanZhi);
        }

        if (txt_name != null)
            txt_name.SetText(p.name);
        //singleStudentView.txt_lv.gameObject.SetActive(false);
    }

    public override void Clear()
    {
        base.Clear();
        //PanelManager.Instance.CloseAllSingle(trans_consumeGrid);
    }


}

/// <summary>
/// 弟子招募类型
/// </summary>
public enum RecruitStudentType
{
    None=0,
    ShanMen=1,//山门
    MapEvent=2,//事件
    JieYinLing=3,//接引令
}