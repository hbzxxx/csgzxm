 using DG.Tweening;
using Framework.Data;
using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleScenePanel : PanelBase
{
    public Transform trans_effectParent;
    public Transform trans_effectSingleParent;//单个技能父物体

    public Transform p1Trans;
    public Transform p2Trans;

    public Transform trans_p1TeammateParent;//玩家未上阵队友父节点
    public Transform trans_p2TeammateParent;//敌人未上阵队友父节点
    public List<BattleSceneTeammateView> p1TeammateViewList = new List<BattleSceneTeammateView>();//玩家未上阵队友视图列表
    public List<BattleSceneTeammateView> p2TeammateViewList = new List<BattleSceneTeammateView>();//敌人未上阵队友视图列表

    public Image img_blackMask;
    public Transform trans_p1EffectParent;
    public Transform trans_p2EffectParent;
    public List<Transform> trans_studentPosList;//弟子助战的位置
    public List<ulong> studentPosOnlyIdList;//弟子助战唯一id
    public BattlePeopleView p1View;
    public BattlePeopleView p2View;


    public bool escape = false;//逃跑
    public float escapeTime = 1;//逃跑计时器
    public float escapeTimer = 0;//逃跑计时器

    public bool deadQieRen = false;//死亡切人
    public float deadQieRenTime = 1;//死亡切人计时器
    public float deadQieRenTimer = 0;//死亡切人

    public bool changePhase = false;//改变形态
    public float changePhaseTime = 1;//改变形态计时器
    public float changePhaseTimer = 0;//改变形态计时器

    public Transform trans_scene;//场景
    public Transform trans_scenePos;//场景位置

    public override void Init(params object[] args)
    {
        base.Init(args);
        deadQieRen = false;
        changePhase = false;
        studentPosOnlyIdList.Clear();
        for(int i = 0; i < 4; i++)
        {
            studentPosOnlyIdList.Add(0);
        }
        //测试场景 
        if (SceneManager.GetActiveScene().name != "SkillEffect")
        {
            PeopleData p1 = args[0] as PeopleData;
            PeopleData p2 = args[1] as PeopleData;
            p1View=PanelManager.Instance.OpenSingle<BattlePeopleView>(p1Trans, p1,p2.onlyId, true,0);
            p2View= PanelManager.Instance.OpenSingle<BattlePeopleView>(p2Trans, p2,p1.onlyId, false, 0);

        }
        RegisterEvent(TheEventType.RemoveBattleBuff, OnRemoveBattleBuff);

        RegisterEvent(TheEventType.ShowBattleEffect, OnShowBattleEffect);
        RegisterEvent(TheEventType.BattleBeHit, OnBeHit);
        RegisterEvent(TheEventType.CloseBattleScene,CloseThePanel);
        RegisterEvent(TheEventType.BlackBattleScene, OnShowBlack);
        RegisterEvent(TheEventType.DisappearQTE, HideBlack);

        RegisterEvent(TheEventType.ZhuZhan, OnZhuZhan);
        RegisterEvent(TheEventType.RemoveZhuZhan, OnRemoveZhuZhan);
        RegisterEvent(TheEventType.BattlePeopleQieRen, OnQieRen);
        RegisterEvent(TheEventType.BattlePeopleDead, OnBattlePeopleDead);
        RegisterEvent(TheEventType.BattlePeopleChangePhase, OnBattlePeopleChangePhase);
        RegisterEvent(TheEventType.BattlePeopleSuccessfulChangePhase, OnSuccessfulChangePhase);
        RegisterEvent(TheEventType.PlayEscapeAnim, PlayEscapeAnim);

        img_blackMask.color = new Color(0, 0, 0, 0);

        // 初始化未上阵队友显示
        RefreshTeammateViews();
    }

    public override void Clear()
    {
        base.Clear();
        EventCenter.Remove(TheEventType.ShowBattleEffect, OnShowBattleEffect);
        EventCenter.Remove(TheEventType.BattleBeHit, OnBeHit);
        EventCenter.Remove(TheEventType.CloseBattleScene, CloseThePanel);
        EventCenter.Remove(TheEventType.BlackBattleScene, OnShowBlack);
        EventCenter.Remove(TheEventType.DisappearQTE, HideBlack);

        if (SceneManager.GetActiveScene().name != "SkillEffect")
        {
            PanelManager.Instance.CloseAllSingle(p1Trans);
            PanelManager.Instance.CloseAllSingle(p2Trans);
        }
        for(int i= trans_effectParent.childCount - 1; i >= 0; i--)
        {
            Transform trans = trans_effectParent.GetChild(i);
            SkillEffectSingleParent skillEffectSingleParent = trans.GetComponent<SkillEffectSingleParent>();
            if (skillEffectSingleParent != null)
            {
                for(int j = trans.childCount - 1; j >= 0; j--)
                {
                    Transform effect = trans.GetChild(j);
                    SkillEffect effectView = effect.GetComponent<SkillEffect>();
                    if (effectView != null)
                    {
                        PanelManager.Instance.CloseSingle(effectView);

                    }

                }
            }
        }
        //PanelManager.Instance.CloseAllSingle(trans_effectParent);
        PanelManager.Instance.CloseAllSingle(trans_p1EffectParent);
        PanelManager.Instance.CloseAllSingle(trans_p2EffectParent);

        // 清理未上阵队友视图
        ClearTeammateViews();
    }

    /// <summary>
    /// 清理未上阵队友视图
    /// </summary>
    void ClearTeammateViews()
    {
        if (trans_p1TeammateParent != null)
        {
            PanelManager.Instance.CloseAllSingle(trans_p1TeammateParent);
        }
        if (trans_p2TeammateParent != null)
        {
            PanelManager.Instance.CloseAllSingle(trans_p2TeammateParent);
        }
        p1TeammateViewList.Clear();
        p2TeammateViewList.Clear();
    }

    /// <summary>
    /// 刷新未上阵队友显示
    /// </summary>
    void RefreshTeammateViews()
    {
        ClearTeammateViews();

        // 玩家方未上阵队友
        if (trans_p1TeammateParent != null && BattleManager.Instance.p1List.Count > 1)
        {
            for (int i = 0; i < BattleManager.Instance.p1List.Count; i++)
            {
                if (i != BattleManager.Instance.p1Index)
                {
                    PeopleData p = BattleManager.Instance.p1List[i];
                    BattleSceneTeammateView view = AddSingle<BattleSceneTeammateView>(trans_p1TeammateParent, p, true);
                    p1TeammateViewList.Add(view);
                }
            }
        }

        // 敌人方未上阵队友
        if (trans_p2TeammateParent != null && BattleManager.Instance.p2List.Count > 1)
        {
            for (int i = 0; i < BattleManager.Instance.p2List.Count; i++)
            {
                if (i != BattleManager.Instance.p2Index)
                {
                    PeopleData p = BattleManager.Instance.p2List[i];
                    BattleSceneTeammateView view = AddSingle<BattleSceneTeammateView>(trans_p2TeammateParent, p, false);
                    p2TeammateViewList.Add(view);
                }
            }
        }
    }

    /// <summary>
    /// 切人
    /// </summary>
    void OnQieRen()
    {
        //p1切了
        if(p1View.peopleData.onlyId!= BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId)
        {
            if (SceneManager.GetActiveScene().name != "SkillEffect")
            {
                PanelManager.Instance.CloseSingle(p1View);
                p1View = PanelManager.Instance.OpenSingle<BattlePeopleView>(p1Trans, BattleManager.Instance.p1List[BattleManager.Instance.p1Index],
                BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId, true,BattleManager.Instance.p1Index);
            }
            else
            {
                p1View.Clear();
                p1View.Init(BattleManager.Instance.p1List[BattleManager.Instance.p1Index],
                BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId, true, BattleManager.Instance.p1Index);
                p1View.OnOpenIng();
            }


            p2View.enemyOnlyId = p1View.peopleData.onlyId;
            //特效
            PanelManager.Instance.OpenSingle<qiehuanrenwutexiao>(trans_p1EffectParent, new Vector3(0, -100, 0));

            for (int i = 0; i < BattleManager.Instance.p1List.Count; i++)
            {
                if (i != BattleManager.Instance.p1Index) { 
                    
                }
            }
        }
        //p2切了
        if (p2View.peopleData.onlyId != BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId)
        {
            if (SceneManager.GetActiveScene().name != "SkillEffect")
            {
                PanelManager.Instance.CloseSingle(p2View);
                p2View = PanelManager.Instance.OpenSingle<BattlePeopleView>(p2Trans, BattleManager.Instance.p2List[BattleManager.Instance.p2Index],
                BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId, false,BattleManager.Instance.p2Index);
            }
            else
            {
                p2View.Clear();
                p2View.Init(BattleManager.Instance.p2List[BattleManager.Instance.p2Index],
                BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId, false, BattleManager.Instance.p2Index);
                p2View.OnOpenIng();
            }
            p1View.enemyOnlyId = p2View.peopleData.onlyId;
            PanelManager.Instance.OpenSingle<qiehuanrenwutexiao>(trans_p2EffectParent, new Vector3(0, -100, 0));

        }

        // 切人后刷新未上阵队友显示
        RefreshTeammateViews();
    }
    /// <summary>
    /// 成功改变形态
    /// </summary>
    void OnSuccessfulChangePhase()
    {
        //p1切了
        if (BattleManager.Instance.changePhaseP.onlyId==p1View.peopleData.onlyId)
        {
            if (SceneManager.GetActiveScene().name != "SkillEffect")
            {
                PanelManager.Instance.CloseSingle(p1View);
                p1View = PanelManager.Instance.OpenSingle<BattlePeopleView>(p1Trans, BattleManager.Instance.p1List[BattleManager.Instance.p1Index],
   BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId, true,BattleManager.Instance.p1Index);
            }
            else
            {
                p1View.Clear();
                p1View.Init(BattleManager.Instance.p1List[BattleManager.Instance.p1Index],
               BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId, true);
                p1View.OnOpenIng();
            }


            p2View.enemyOnlyId = p1View.peopleData.onlyId;
        }
        //p2切了
        if (BattleManager.Instance.changePhaseP.onlyId == p2View.peopleData.onlyId)
        {
            if (SceneManager.GetActiveScene().name != "SkillEffect")
            {
                PanelManager.Instance.CloseSingle(p2View);
                p2View = PanelManager.Instance.OpenSingle<BattlePeopleView>(p2Trans, BattleManager.Instance.p2List[BattleManager.Instance.p2Index],
       BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId, false, BattleManager.Instance.p2Index);
            }
            else
            {
                p2View.Clear();
                p2View.Init(BattleManager.Instance.p2List[BattleManager.Instance.p2Index],
       BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId, false, BattleManager.Instance.p2Index);
                p2View.OnOpenIng();
            }
            p1View.enemyOnlyId = p2View.peopleData.onlyId;

        }
    }
    /// <summary>
    /// 助战 这里放助战动画
    /// </summary>
    public void OnZhuZhan(object[] args)
    {
        PeopleData p = args[0] as PeopleData;

        PeopleData beZhuZhanP = null;
        if (BattleManager.Instance.CheckIfLeftP(p))
        {
            beZhuZhanP = BattleManager.Instance.p1List[BattleManager.Instance.p1Index];
        }
        else
        {
            beZhuZhanP = BattleManager.Instance.p2List[BattleManager.Instance.p2Index];
        }
        HaoGanLevelType curHaoGanLevelType = StudentManager.Instance.GetStudentHaoGanLevelType(p, beZhuZhanP);
        bool speak = false;
        //没记录 则说话 并记录
        if (!p.socializationData.recordedZhuZhanPList.Contains(beZhuZhanP.onlyId))
        {
            p.socializationData.recordedZhuZhanPList.Add(beZhuZhanP.onlyId);
            p.socializationData.recordedZhuZhanPHaoGanLevel.Add((int)curHaoGanLevelType);

            beZhuZhanP.socializationData.recordedZhuZhanPList.Add(p.onlyId);
            beZhuZhanP.socializationData.recordedZhuZhanPHaoGanLevel.Add((int)curHaoGanLevelType);


            speak = true;

         }
        else
        {
            int index = p.socializationData.recordedZhuZhanPList.IndexOf(beZhuZhanP.onlyId);
            int recordedHaoGanLevel = p.socializationData.recordedZhuZhanPHaoGanLevel[index];
            //有记录 但好感度更高了，则说话并记录
            if ((int)curHaoGanLevelType > recordedHaoGanLevel)
            {
                //说话
                p.socializationData.recordedZhuZhanPHaoGanLevel[index] = (int)curHaoGanLevelType;
                speak = true;

            }
            else
            {
                speak = false;

                //PanelManager.Instance.OpenPanel<XieZhanPanel>(PanelManager.Instance.trans_layer2, p);

            }
        }
        if (speak)
        {
                       PanelManager.Instance.OpenPanel<XieZhanPanel>(PanelManager.Instance.trans_layer2, p);

        }
        else
        {
            PanelManager.Instance.OpenPanel<XieZhanPanel>(PanelManager.Instance.trans_layer2, p);

        }

    }

    public string ReplaceDialog(PeopleData p, string str)
    {
        str = str.Replace("[a]",p.name);
        return str;
    }
    /// <summary>
    /// 移除助战
    /// </summary>
    /// <param name="args"></param>
    public void OnRemoveZhuZhan(object[] args)
    {
        //PeopleData p = args[0] as PeopleData;
        //for(int i = studentPosOnlyIdList.Count-1; i >=0 ; i--)
        //{
        //    if (studentPosOnlyIdList[i] == p.OnlyId)
        //        studentPosOnlyIdList[i]=0;
        //}
    }

    void OnShowBlack()
    {
        img_blackMask.DOFade(.58f, 1);
    }

    void HideBlack()
    {
        img_blackMask.DOFade(0, 1);

    }
    void CloseThePanel()
    {
        PanelManager.Instance.ClosePanel(this);
    }
    /// <summary>
    /// 被打震屏
    /// </summary>
    void OnBeHit(object[] param)
    {
        AttackResData attackResData = param[0] as AttackResData;
        if (attackResData == null)
            return;
        //震屏
        if (attackResData.skill ==null)
                return;
        SkillSetting skillSetting =BattleManager.Instance.FindSkillSetting(attackResData.skill.skillId);
        int index = attackResData.damageIndex;
        string[] shakeArr = skillSetting.ScreenShakeStrength.Split('|');
        int strength = shakeArr[index].ToInt32();
        //trans_scene.DOKill();
        //trans_scene.DOShakePosition(.5f, strength).OnComplete(() =>
        //{
        //    trans_scene.localPosition = trans_scenePos.localPosition;
        //});
    }

    /// <summary>
    /// 显示技能特效
    /// </summary>
    void OnShowBattleEffect(object[] param)
    {
        PeopleData data = param[0] as PeopleData;
        string effectName = (string)param[1];

        if (string.IsNullOrWhiteSpace(effectName))
            return;

        SkillEffectSingleParent parent = PanelManager.Instance.OpenSingle<SkillEffectSingleParent>(trans_effectParent);
        //角色打的
        if (data.isMyTeam)
        {
            parent.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            parent.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        ObjectPoolSingle single=(ObjectPoolSingle)Enum.Parse(typeof(ObjectPoolSingle), effectName);
        PanelManager.Instance.OpenSingle(single, parent.transform);
    }

    /// <summary>
    /// 移除战斗buff
    /// </summary>
    /// <param name="param"></param>
    void OnRemoveBattleBuff(object[] param)
    {
        PeopleData data = param[0] as PeopleData;
        BattleBuff buff= param[1] as BattleBuff;
        string effectName = buff.buffSetting.EffectName;
        for (int i = trans_effectParent.childCount-1; i >=0 ; i--)
        {
            Transform trans = trans_effectParent.GetChild(i);
            SkillEffect skillEffect = trans.GetComponentInChildren<SkillEffect>();
            if (skillEffect == null)
            {

            }
            if (skillEffect.gameObject.name == effectName
                &&buff.buffSetting.AutoRemove!="1")
            {
                PanelManager.Instance.CloseSingle(skillEffect);
            }
        }
    }

    private void Update()
    {
        if (escape)
        {
            escapeTimer += Time.deltaTime;
            if (escapeTimer >= escapeTime)
            {
                escape = false;
                BattleManager.Instance.SuccessEscape();
            }
        }
        if (deadQieRen)
        {
            deadQieRenTimer += Time.deltaTime;
            if (deadQieRenTimer >= deadQieRenTime)
            {
                deadQieRen = false;
                BattleManager.Instance.DeadQieRen();
            }
        }
        if (changePhase)
        {
            changePhaseTimer += Time.deltaTime;
            if (changePhaseTimer >= changePhaseTime)
            {
                changePhase = false;
                BattleManager.Instance.ChangeToNextPhase();
            }
 
        }
    }

    /// <summary>
    /// 有人死 播放动画 动画完了通过有没有活人决定是切人还是结束战斗
    /// </summary>
    /// <param name="param"></param>
    public void OnBattlePeopleDead(object[] param)
    {
        ulong onlyId = (ulong)param[0];
        if (onlyId == p1View.peopleData.onlyId)
        {
            DeathFogView view = AddSingle<DeathFogView>(trans_p1EffectParent);

        }
        else
        {
            DeathFogView view = AddSingle<DeathFogView>(trans_p2EffectParent);

        }

        // 更新协战队友死亡状态（场景中变灰）
        //UpdateXieZhanDeadState(onlyId);

        deadQieRen = true;
        deadQieRenTimer = 0;
    }
    /// <summary>
    /// 开始逃跑 播放动画
    /// </summary>
    /// <param name="args"></param>
    void PlayEscapeAnim(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        if (onlyId == p1View.peopleData.onlyId)
        {
            DeathFogView view = AddSingle<DeathFogView>(trans_p1EffectParent);

        }
        else
        {
            DeathFogView view = AddSingle<DeathFogView>(trans_p2EffectParent);

        }
        escape = true;
        escapeTimer = 0;
    }
    /// <summary>
    /// 有人改变形态 播放动画
    /// </summary>
    /// <param name="param"></param>
    public void OnBattlePeopleChangePhase(object[] param)
    {
        ulong onlyId = (ulong)param[0];
        if (onlyId == p1View.peopleData.onlyId)
        {
            DeathFogView view = AddSingle<DeathFogView>(trans_p1EffectParent);

        }
        else
        {
            DeathFogView view = AddSingle<DeathFogView>(trans_p2EffectParent);

        }
        changePhase = true;
        changePhaseTimer = 0;
    }
}
