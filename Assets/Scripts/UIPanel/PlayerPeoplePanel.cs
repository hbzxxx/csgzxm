using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPeoplePanel : PanelBase
{
    public Button btn_knapsackTag;//背包tag
    public Button btn_skillTag;//技能tag
    public GameObject obj_skillTagRedPoint;
    public Transform trans_sonParent;//子面板
    public int openIndex;
    public override void Init(params object[] args)
    {
        base.Init(args);
        openIndex = (int)args[0];
        addBtnListener(btn_knapsackTag, ShowKnapsackPanel);
        addBtnListener(btn_skillTag, ShowSkillPanel);
        RegisterEvent(TheEventType.RefreshSkillRedPoint, RefreshRedPointShow);
        //btn_emptyClose.gameObject.SetActive(false);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
   
        btn_knapsackTag.onClick.Invoke();
        ShowGuide();
        RefreshRedPointShow();
        AuditionManager.Instance.PlayVoice(AudioClipType.OpenKnapsack);
    }

    /// <summary>
    /// 红点
    /// </summary>
    void RefreshRedPointShow()
    {
        RedPointManager.Instance.SetRedPointUI(obj_skillTagRedPoint, RedPointType.MainPanel_Btn_Knapsack_SkillTag, 0);
    }
    void ShowGuide()
    {
        if (TaskManager.Instance.guide_upgradeSkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_skillTag.gameObject);
        }
        else if (TaskManager.Instance.guide_equipSkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_skillTag.gameObject);
        }
    }

    /// <summary>
    /// 显示人物属性面板
    /// </summary>
    void ShowPlayerProPanel()
    {
        //btn_knapsackTag.GetComponent<Image>().color = Color.white;
        //btn_skillTag.GetComponent<Image>().color = Color.white;


        //PanelManager.Instance.CloseAllPanel(trans_sonParent);
        //PanelManager.Instance.OpenPanel<PeopleProPanel>(trans_sonParent);
    }

    /// <summary>
    /// 显示背包面板
    /// </summary>
    void ShowKnapsackPanel()
    {
        btn_skillTag.GetComponent<Image>().color = Color.white;

        btn_knapsackTag.GetComponent<Image>().color = ConstantVal.color_choosed;

        PanelManager.Instance.CloseAllPanel(trans_sonParent);
        PanelManager.Instance.OpenPanel<KnapsackPanel>(trans_sonParent);

    }

    /// <summary>
    /// 显示技能面板
    /// </summary>
    void ShowSkillPanel()
    {
        btn_knapsackTag.GetComponent<Image>().color = Color.white;
        btn_skillTag.GetComponent<Image>().color = ConstantVal.color_choosed;

        PanelManager.Instance.CloseAllPanel(trans_sonParent);
        PanelManager.Instance.OpenPanel<SkillPanel>(trans_sonParent,RoleManager.Instance._CurGameInfo.playerPeople);

    }


    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllPanel(trans_sonParent);
    }
}
