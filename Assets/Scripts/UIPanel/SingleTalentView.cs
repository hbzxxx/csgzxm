using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 天赋
/// </summary>
public class SingleTalentView : SingleViewBase
{
    public Text txt;
    public Button btn;
    public TalentTestPanel talentTestPanel;

    public StudentTalent talent;
    public Quality quality;
    public YuanSuType yuanSu;//如果是修武则有元素
    public float dragDistanceNeedY = 150;//拖动超过150;就飞


    public Spine.Unity.SkeletonGraphic ske;
    public ScrollRect scroll;
    public Transform trans_effectStartPos;//粒子特效起点
    public bool choosed = false;
     public override void Init(params object[] args)
    {
        base.Init(args);

        talent = (StudentTalent)args[0];
         quality = (Quality)args[1];
        yuanSu = (YuanSuType)args[2];
        talentTestPanel = args[3] as TalentTestPanel;
        //txt.color = CommonUtil.QualityColor(quality);
        addBtnListener(btn, () =>
        {
            talentTestPanel.OnChoosedCertainTalent(this);
        });

        ske.DOKill();
        txt.DOKill();

        ske.DOFade(1, 0);
        txt.DOFade(1, 0);

        string yuanSuStr = "";
        if (talent == StudentTalent.LianGong)
        {
            yuanSuStr= ConstantVal.YuanSuName(yuanSu);
        }
        txt.SetText(ConstantVal.QualityName((int)quality)+"."+ StudentManager.Instance.TalentNameByTalent(talent)+ yuanSuStr);
        ske.transform.localPosition = Vector2.zero;
        ske.AnimationState.SetAnimation(0, ((int)quality).ToString(), true);
        scroll.vertical = false;

   
    }

    private void Update()
    {
        if (choosed)
        {
            if(ske.transform.localPosition.y>= dragDistanceNeedY)
            {
                talentTestPanel.OnSlidedTalent(this);
                //choosed = false;
            }
        }
    }

    /// <summary>
    /// 选择了
    /// </summary>
    /// <param name="choose"></param>
    public void OnChoose(bool choose)
    {
        ske.DOKill();

        if (choose)
        {
            ske.DOFade(1, .5f);
            txt.DOFade(1, .5f);
            scroll.vertical = true;
            choosed = true;
        }
        else
        {
            ske.DOFade(.2f, .5f);
            txt.DOFade(.2f, .5f);
            scroll.vertical = false;
            choosed = false;

        }
    }

    public override void Clear()
    {
        base.Clear();
        choosed = false;
    }

}
