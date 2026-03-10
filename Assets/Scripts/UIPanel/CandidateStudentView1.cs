using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandidateStudentView1 : BigStudentView
{


    public Button btn_recruit;//招募

    public Transform trans_consumeGrid;
    RecruitStudentType recruitStudentType;

    public SingleStudentViewzm StudentViewzm;

    public Text txt_name;
    public Image hong;
    public Image other;
    public Text txt_zm;
    public Color colorhzm;
    public Color colorozm;
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
        //base.OnOpenIng();

        //PanelManager.Instance.OpenSingle<SingleConsumeView>(trans_consumeGrid, (int)ItemIdType.LingShi, ConstantVal.cruitStudentNeedLingShiNum, ConsumeType.Item);
        //PanelManager.Instance.OpenSingle<SingleConsumeView>(trans_consumeGrid, (int)ItemIdType.ZhaoJiLing, 1, ConsumeType.Item);
        //singleStudentView.txt_energy.gameObject.SetActive(false);
        PanelManager.Instance.CloseAllSingle(trans_studentViewParent);
        StudentViewzm = PanelManager.Instance.OpenSingle<SingleStudentViewzm>(trans_studentViewParent, p);
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
        //StudentViewzm.txt_lv.gameObject.SetActive(false);
        if (p.talentRarity == 5)
        {
            hong.gameObject.SetActive(true);
            other.gameObject.SetActive(false);
            txt_zm.color = colorhzm;
        }
        else
        {
            hong.gameObject.SetActive(false);
            other.gameObject.SetActive(true);
            txt_zm.color = colorozm;
        }
    }

    public override void Clear()
    {
        base.Clear();
        //PanelManager.Instance.CloseAllSingle(trans_consumeGrid);
        PanelManager.Instance.CloseAllSingle(trans_studentViewParent);
    }


}
