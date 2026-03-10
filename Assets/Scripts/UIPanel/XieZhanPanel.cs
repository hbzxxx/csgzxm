using DG.Tweening;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class XieZhanPanel : PanelBase
{
    public Transform contentParent;
    public Transform startPos;
    public Transform centerPos1;
    public Transform centerPos2;
    public Transform endPos;

    public float phase1Time = 0.1f;
    public float phase2Time = 1f;
    public float phase3Time = 0.1f;

    public Transform trans_head;

    public Portrait portrait;
    public PeopleData p;
    public Text txt_name;

    bool startSkillCenter;//开始技能
    float skillCenterTime;//技能
    float skillCenterTimer;//技能
    SingleSkillData curChoosedSkill = null;

    List<float> skillDamageTimeList = new List<float>();//造成伤害的时间节点
    int finishedDamage = -1;//哪些段伤害已结束
    bool finishedAllAttack = false;//结束了所有多段攻击

    ulong enemyOnlyId = 0;

    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        portrait.Refresh(p);
        if (BattleManager.Instance.CheckIfLeftP(p))
        {
            contentParent.localEulerAngles = new Vector3(0, 0, 0);
            enemyOnlyId = BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId;
        }
        else
        {
            contentParent.localEulerAngles = new Vector3(0, 180, 0);
            enemyOnlyId = BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId;
        }
        Move();
        curChoosedSkill = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[0], p.allSkillData);
        SkillSetting skillSetting = DataTable.FindSkillSetting(curChoosedSkill.skillId);
        skillCenterTime = skillSetting.CenterTime.ToFloat();
        skillCenterTimer = 0;
        startSkillCenter = false;

        string[] skillDamageTimeArr = skillSetting.DamageTimeArr.Split('|');
        skillDamageTimeList.Clear();
        finishedDamage = -1;
        for (int i = 0; i < skillDamageTimeArr.Length; i++)
        {
            skillDamageTimeList.Add(skillDamageTimeArr[i].ToFloat());
        }
        txt_name.SetText(p.name);

        int voiceXingGe = (int)XingGeType.AiJia;
        if (p.socializationData != null)
        {
            voiceXingGe = p.socializationData.xingGe;
        }
        else
        {
            voiceXingGe = RandomManager.Next(1, (int)XingGeType.End);
        }
        if (p.gender != (int)Gender.None)
        {
            AuditionManager.Instance.PlayVoice(Camera.main.transform,
            AuditionManager.Instance.JiaoAudio(voiceXingGe, (Gender)(int)p.gender));
        }
        else
        {
            AuditionManager.Instance.PlayVoice(Camera.main.transform, AuditionManager.Instance.MonsterJiaoAudio());
        }
    }
    private void Update()
    {
        if (startSkillCenter)
        {
            skillCenterTimer += Time.deltaTime;
            if (skillCenterTimer >= skillCenterTime)
            {
                //结束技能
                BattleManager.Instance.OnFinishZhuZhan(p);
                PanelManager.Instance.ClosePanel(this);
                //role.endZhuZhanAttack = true;
                //role.curChoosedSkill = null;
                //startSkillCenter = false;
            }
            else
            {
                //特定时间节点造成杀伤
                if (!finishedAllAttack)
                {
                    int index = CommonUtil.GetPhaseIndex(skillCenterTimer, skillDamageTimeList);
                    if (index > finishedDamage)
                    {
                        finishedDamage = index;
                        BattleManager.Instance.SingleAttack(p, curChoosedSkill, index,true);
                    }
                    //该技能所有伤害都打完了
                    if (index >= skillDamageTimeList.Count - 1)
                    {
                        finishedAllAttack = true;
                        //判断对方是否死了
                        //BattleManager.Instance.JudgeIfPeopleDead();
                    }
                }
            }
        }
    }
    void Move()
    {
        
        trans_head.DOKill();
        trans_head.gameObject.SetActive(true);
        trans_head.position = startPos.position;
        trans_head.DOMoveX(centerPos1.position.x, phase1Time).OnComplete(() =>
        {
            trans_head.DOMoveX(centerPos2.position.x, phase2Time).OnComplete(() =>
            {
                startSkillCenter = true;
                BattleManager.Instance.ShowBattleEffect(p,  curChoosedSkill);

                trans_head.DOMoveX(endPos.position.x, phase3Time).OnComplete(() =>
                 {
                     trans_head.gameObject.SetActive(false);


                 });
             });
        });
    }

    public override void Clear()
    {
        base.Clear();
        startSkillCenter = false;
        skillCenterTimer = 0;
        finishedAllAttack = false;
    }
}
