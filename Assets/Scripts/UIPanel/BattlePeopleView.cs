using BlueToolkit;
using cfg;
 using Coffee.UIEffects;
using DG.Tweening;
using DragonBones;
using Framework.Data;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePeopleView : SingleViewBase
{
    public IState curState;
    public PeopleData peopleData;
    //public BattlePanel parentPanel;

    public UnityArmatureComponent armature;

    public UnityEngine.Transform trans_bulletPos;//子弹位置

    public UnityEngine.Transform trans_content;
    public bool left;//朝哪边

    public float beHitTime;//被打硬直
    public float beHitTimer;//被打倒计时
    public bool startBeHit;//开始被打

    public UnityEngine.Transform trans_loseHpParent;//掉血

    //public Image img_hpBar;

    //public Text txt_name;

    public RoleAnim roleAnim;
    //战斗角色
    public Image role_p;
    public Image img_shadow;//人物阴影


    public StateMachine _stateMachine;
    //角色状态
    RoleAttackState roleAttackState;
    RoleIdleState roleIdleState;
    RoleBeHitState roleBeHitState;
    RoleDeadState roleDeadState;
    RoleZhuZhanAttackState roleZhuZhanAttackState;
    RoleEndZhuZhanState roleEndZhuZhanState;
    RoleXuanYunState roleXuanYunState;
    //public SingleSkillData curSkill;
    public float curBeHitCD = 0.2f;//被打持续时间当前
    public float commonBeHitCD = 0.2f;//被打持续时间
    float dianLiuBeHitCD=0.5f;//电流持续时间

    public bool beHit;//被打
    public bool xuanYun;//眩晕
    public bool attackCDAccomplished = false;//完成战斗cd

    public UInt64 enemyOnlyId;//敌人

    public bool dead = false;

    public UIEffect uiEffect;

    public SingleSkillData curChoosedSkill;//当前选了什么技能

    public Image img;
    public float attackCDTimer = 0;

    public bool readyToEscape = false;//准备逃走
    public bool readyToBig = false;//准备放大
    public bool endAttack = false;//终止攻击

    public bool readyToQieRen = false;//准备切人
    public int qieRenIndex = -1;//切谁

    public bool isZhuZhanDiZi;//是助战弟子
    public bool waitZhuZhanDiZi;//等待助战弟子行动
    public bool endZhuZhanAttack = false;//终止助战攻击

    public int index;//第几个人

    public int curDaZhaoIndex;//大招技能index
    bool initOk = false;
    public EnemySetting setting;
    public override void Init(params object[] args)
    {
        base.Init(args);
        peopleData = args[0] as PeopleData;
        readyToBig = false;
        //parentPanel = args[1] as BattlePanel;
        enemyOnlyId = (ulong)args[1];
        left = (bool)args[2];
        index = (int)args[3];

        dead = false;

        //if (_stateMachine != null)
        //{
        //    _stateMachine.GetCurState().OnExit();
        //}
        _stateMachine = new StateMachine();

        roleIdleState = new RoleIdleState(this);
        roleAttackState = new RoleAttackState(this);
        roleBeHitState = new RoleBeHitState(this);
        roleDeadState = new RoleDeadState(this);
        roleZhuZhanAttackState = new RoleZhuZhanAttackState(this);
        roleEndZhuZhanState = new RoleEndZhuZhanState(this);
        roleXuanYunState = new RoleXuanYunState(this);

        At(roleIdleState, roleBeHitState, ()=>
        beHit);
        At(roleIdleState, roleXuanYunState, IdleToXuanYunCondition);
        At(roleIdleState, roleDeadState, () =>
        RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp,peopleData).num<=0);

        At(roleIdleState, roleAttackState, IdleToAttackCondition);


        At(roleAttackState, roleIdleState, () =>
          endAttack
          &&BattleManager.Instance.curZhuZhanPList.Count==0
         );


        At(roleIdleState, roleBeHitState, () =>
          beHit);

        At(roleBeHitState, roleIdleState, () =>
      !xuanYun
        &&!beHit
      &&!dead
      &&!BattleManager.Instance.logicPause);

        At(roleBeHitState, roleXuanYunState, BeHitToXuanYunCondition);

        At(roleXuanYunState, roleBeHitState, () =>
          beHit
      );
        At(roleBeHitState, roleDeadState, () =>
              dead
            );
        At(roleXuanYunState, roleIdleState, () =>
    !beHit
    &&!xuanYun
    &&!dead
     && !BattleManager.Instance.logicPause
);
        At(roleZhuZhanAttackState, roleEndZhuZhanState, () =>
         endZhuZhanAttack);

        EventCenter.Register(TheEventType.BattleBeHit, BeHitStatus);
        RegisterEvent(TheEventType.BattlePeopleDead, OnDead);
         RegisterEvent(TheEventType.AddBattleProperty, OnBattleProChange);
        RegisterEvent(TheEventType.DeBattleProperty, OnBattleProChange);
        RegisterEvent(TheEventType.ReadyToBig, OnReadyToBig);
        RegisterEvent(TheEventType.ReadyToQieRen, OnReadyToQieRen);
        RegisterEvent(TheEventType.CancelReadyToQieRen, CancelReadyToQieRen);

        RegisterEvent(TheEventType.ZhuZhan, OnReadyZhuZhanStudent);
        RegisterEvent(TheEventType.RemoveZhuZhan, OnRemoveZhuZhanStudent);
        RegisterEvent(TheEventType.StartEscape, StartEscape);
        RegisterEvent(TheEventType.CancelMai, OnCancelMai);
        RegisterEvent(TheEventType.AddBattleBuff, OnAddBattleBuff);
        RegisterEvent(TheEventType.DeBattleBuffHuiHe, DeBattleBuffHuiHe);
        RegisterEvent(TheEventType.RemoveBattleBuff, OnRemoveBattleBuff);
        RegisterEvent(TheEventType.OnDongJiang, OnDongJiang);
        RegisterEvent(TheEventType.SkillAttack, StartBigSkillAttack);
        RegisterEvent(TheEventType.BattleBeHit, OnBeHit);

        attackCDTimer = 0;//初始化攻击cd为0

        initOk = true;
        if (peopleData.enemySettingId != 0)
        {
            setting = DataTable.FindEnemySetting(peopleData.enemySettingId);
            if (!string.IsNullOrWhiteSpace(setting.InitAddBuff))
            {
                List<int> buffList = CommonUtil.SplitCfgOneDepth(setting.InitAddBuff);
                for (int i = 0; i < buffList.Count; i++)
                {
                    BattleManager.Instance.AddBattleBuff(peopleData, DataTable.FindBattleBuffSetting(buffList[i]));
                }
            }
            //role_p.GetComponent<RectTransform>().SetScaleX(1f);
        }
        //else {
        //    role_p.GetComponent<RectTransform>().SetScaleX(-1f);
        //}
    }

    /// <summary>
    /// 被打震屏
    /// </summary>
    void OnBeHit(object[] param)
    {
        AttackResData attackResData = param[0] as AttackResData;
        if (attackResData == null)
            return;
        // 只有被打的人才震动
        if (attackResData.deHpPeople.onlyId != peopleData.onlyId)
            return;
        // 震屏
        if (attackResData.skill == null)
            return;
        SkillSetting skillSetting = BattleManager.Instance.FindSkillSetting(attackResData.skill.skillId);
        if (skillSetting == null || string.IsNullOrEmpty(skillSetting.ScreenShakeStrength))
            return;
        int index = attackResData.damageIndex;
        string[] shakeArr = skillSetting.ScreenShakeStrength.Split('|');
        if (index >= shakeArr.Length)
            return;
        int strength = shakeArr[index].ToInt32();
        if (strength <= 0)
            strength = 10;
        transform.DOKill();
        transform.DOShakePosition(.5f, strength).OnComplete(() =>
        {
            transform.localPosition = Vector3.zero;
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (left)
        {
            trans_content.localEulerAngles = Vector3.zero;
            //role_p.GetComponent<RectTransform>().SetScaleX(-0.5f);
            role_p.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            trans_content.localEulerAngles = new Vector3(0, 180, 0);
            //role_p.GetComponent<RectTransform>().SetScaleX(0.5f);
            role_p.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
            
        }

        //txt_name.SetText(PeopleData.Name);

        //img_hpBar.fillAmount = 1;
        transform.DOKill();
        transform.localPosition = Vector3.zero;

        // 显示人物阴影
        if (img_shadow != null)
        {
            img_shadow.gameObject.SetActive(true);
        }

        //初始化骨骼
        //if (PeopleData.IsPlayer)
        //{
        //    roleAnim.ske.transform.localPosition = new Vector2(-90, -97);
        //}
        if (peopleData.isMyTeam)
        {
            if (peopleData.isPlayer)
            {
                if (peopleData.gender == (int)Gender.Male)
                    roleAnim.ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + "man_dadou_SkeletonData");
                else if (peopleData.gender == (int)Gender.Female)
                    roleAnim.ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + "nvdadou_SkeletonData");

            }
            else if(peopleData.gender==(int)Gender.Male)
            {
                roleAnim.ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + "man_dadou_SkeletonData");

            }else if (peopleData.gender == (int)Gender.Female)
            {
                roleAnim.ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + "nvdadou_SkeletonData");

            }
        }
        else
        {
            EnemySetting enemySetting = DataTable.FindEnemySetting(peopleData.enemySettingId);
            if (enemySetting != null && !string.IsNullOrWhiteSpace(enemySetting.Ske))
            {
                roleAnim.ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + enemySetting.Ske);

            }
            else
            {
                if (peopleData.gender == (int)Gender.Male)
                {
                    roleAnim.ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + "man_dadou_SkeletonData");

                }
                else if (peopleData.gender == (int)Gender.Female)
                {
                    roleAnim.ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + "nvdadou_SkeletonData");

                }
            }
        
        }

        roleAnim.ske.Initialize(true);
        if (img != null)
        {
            img.DOKill();
            img.color = Color.white;
        }
        else if (roleAnim!=null)
        {
            roleAnim.ske.DOKill();
            roleAnim.ske.color = Color.white;

        }

        _stateMachine.SetState(roleIdleState);

        StudentManager.Instance.SetTouxiang(role_p, peopleData);

    }


    public void ZhuZhan()
    {
        _stateMachine.SetState(roleZhuZhanAttackState);
    }
    /// <summary>
    /// 等待助战弟子
    /// </summary>
    void OnReadyZhuZhanStudent(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        //如果是同一队的
        if ((BattleManager.Instance.CheckIfLeftP(theP)
            &&BattleManager.Instance.CheckIfLeftP(peopleData))
            || (BattleManager.Instance.CheckIfLeftP(theP)
            && BattleManager.Instance.CheckIfLeftP(peopleData)))
        {
            waitZhuZhanDiZi = true;
        }
    }
    /// <summary>
    /// 移除一个助战弟子
    /// </summary>
    void OnRemoveZhuZhanStudent(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        //如果是同一队的
        if ((BattleManager.Instance.CheckIfLeftP(theP)
            && BattleManager.Instance.CheckIfLeftP(peopleData))
            || (BattleManager.Instance.CheckIfLeftP(theP)
            && BattleManager.Instance.CheckIfLeftP(peopleData)))
        {
            waitZhuZhanDiZi = false;
        }
    }

    /// <summary>
    /// 准备放大
    /// </summary>
    void OnReadyToBig(object[] args)
    {

        if (peopleData.isMyTeam)
        {
            readyToBig = true;
            curDaZhaoIndex = (int)args[0];
        }
    }
    /// <summary>
    /// 切人准备
    /// </summary>
    /// <param name="args"></param>
    public void OnReadyToQieRen(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        int index = (int)args[1];
        if (onlyId == peopleData.onlyId)
        {
            readyToQieRen = true;
            qieRenIndex = index;
        }
    }
    /// <summary>
    /// 取消切人准备
    /// </summary>
    /// <param name="args"></param>
    public void CancelReadyToQieRen(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        if (onlyId == peopleData.onlyId)
        {
            readyToQieRen = false;
            qieRenIndex = -1;
        }
    }
    /// <summary>
    /// 能量改变
    /// </summary>
    /// <param name="param"></param>
    void OnBattleProChange(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        PropertyIdType propertyIdType = (PropertyIdType)param[1];
        if (p.onlyId == peopleData.onlyId)
        {
            peopleData = p;
        }
    }
    void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);


    bool IdleToAttackCondition()
    {
    
        if (attackCDAccomplished && !BattleManager.Instance.logicPause)
        {
            //Debug.Log(peopleData.name + "attackCDAccomplishd未true且logicPause为false，发动攻击！");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 被打到眩晕
    /// </summary>
    /// <returns></returns>
    bool BeHitToXuanYunCondition()
    {
        if (!beHit
        && xuanYun)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// idle到xuanyun
    /// </summary>
    /// <returns></returns>
    bool IdleToXuanYunCondition()
    {
        if (xuanYun)
        {
            attackCDTimer = 0;
            return true;
        }
        return false;
    }


    /// <summary>
    /// qte操作成功开始打
    /// </summary>
    public void StartBigSkillAttack(object[] param)
    {
        UInt64 onlyId = (ulong)param[0];
        int index = (int)param[1];
        if (peopleData.onlyId == onlyId)
        {
            if (_stateMachine.GetCurState() == roleAttackState)
            {
                roleAttackState.StartAttack(index);
            }
        }
  
    }

    private void Update()
    {
        if (!initOk)
            return;
        _stateMachine.Tick(Time.deltaTime);

    }
    //private void FixedUpdate()
    //{
    //    _stateMachine.FixedUpdate(Time.fixedDeltaTime);

    

    //}

    /// <summary>
    /// 射击
    /// </summary>
    void OnShot(string evtName,EventObject obj)
    {
        //switch (obj.name)
        //{
        //    case "shoot":
        //        //射击
        //        PanelManager.Instance.OpenSingle<YiYangZhiEffect>(parentPanel.trans_bulletParent,!left, trans_bulletPos.position,this.PeopleData);
        //        break;
        //}
    }

 

 

    public override void Clear()
    {
        base.Clear();
        EventCenter.Remove(TheEventType.BattleBeHit, BeHitStatus);
         readyToQieRen = false;
        beHit = false;
        startBeHit = false;
        attackCDAccomplished = false;
        dead = false;
        curChoosedSkill = null;
        readyToBig = false;
        readyToEscape = false;
        endAttack = false;
        qieRenIndex = -1;
        isZhuZhanDiZi = false;
        waitZhuZhanDiZi = false;
        endZhuZhanAttack = false;
        xuanYun = false;
        initOk = false;
        
        // 重置阴影状态
        if (img_shadow != null)
        {
            img_shadow.gameObject.SetActive(false);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
    
    }



    /// <summary>
    /// 被打中
    /// </summary>
    public void BeHitStatus(object[] param)
    {
        AttackResData attackResData = param[0] as AttackResData;
        if (attackResData == null)
            return;
        //我掉血
        if (attackResData.deHpPeople.onlyId == peopleData.onlyId)
        {
            //transform.DOKill();
            //transform.DOShakePosition(.5f, 10);
            //uiEffect.DOKill();
            //DOTween.To(() => uiEffect.colorFactor, x => uiEffect.colorFactor = x, 1, 0.04f).SetLoops(8, LoopType.Yoyo).OnComplete(() =>
            //{
            //    uiEffect.colorFactor = 0;
            //});
            //AuditionManager.Instance.PlayVoice(Camera.main.transform, AudioClipType.MaleBeHit);
            if (attackResData.reactionType == ReactionType.DianLiu)
                curBeHitCD = dianLiuBeHitCD;
            else
                curBeHitCD = commonBeHitCD;

            roleAnim.Play("beida", false);

            beHit = true;
            //刷新硬直时间
            if (_stateMachine.GetCurState() == roleBeHitState)
            {
                roleBeHitState.RefreshHitRecover();
            }
        }

    }
    
    public void OnDead(object[] param)
    {
        ulong onlyId = (ulong)param[0];
        if (onlyId == peopleData.onlyId)
            dead = true;
    }

    /// <summary>
    /// 开始逃跑 播放动画
    /// </summary>
    /// <param name="args"></param>
    void StartEscape(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        if (onlyId == peopleData.onlyId)
        {
            readyToEscape = true;

        }
    }

    /// <summary>
    /// 取消脉
    /// </summary>
    void OnCancelMai(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        if (onlyId == peopleData.onlyId)
        {
            if (_stateMachine.GetCurState() == roleAttackState)
            {
                endAttack = true;
                readyToBig = false;

            }
        }
    }

    /// <summary>
    /// 被施加buff
    /// </summary>
    void OnAddBattleBuff(object[] args)
    {
        PeopleData p = args[0] as PeopleData;
        BattleBuff battleBuff = args[1] as BattleBuff;
        BattleBuffSetting setting = battleBuff.buffSetting;
        if (p.onlyId == peopleData.onlyId)
        {
            BattleBuffType type = (BattleBuffType)setting.BuffType.ToInt32();
            if (type == BattleBuffType.DingShen)
            {
                xuanYun = true;
                attackCDTimer = 0;
                if (_stateMachine.GetCurState() == roleXuanYunState)
                {
                    roleXuanYunState.RefreshXuanYunRecover();
                }
            }
        }
    

    }

    /// <summary>
    /// 被冻僵 清空cd
    /// </summary>
    void OnDongJiang(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if(peopleData.onlyId== theP.onlyId)
        {
            attackCDTimer = 0;

        }
    }

    /// <summary>
    /// 减少战斗buff回合
    /// </summary>
    /// <param name="args"></param>
    void DeBattleBuffHuiHe(object[] args)
    {
        PeopleData p = args[0] as PeopleData;
        BattleBuff buff = args[1] as BattleBuff;

        if (p.onlyId == peopleData.onlyId)
        {
            if(buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.DingShen)
            {
                attackCDTimer = 0;
            }
        }
         
    }

    /// <summary>
    /// 移除战斗buff
    /// </summary>
    void OnRemoveBattleBuff(object[] args)
    {
        PeopleData p = args[0] as PeopleData;
        BattleBuff buff = args[1] as BattleBuff;
        if (p.onlyId == peopleData.onlyId)
        {
            if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.DingShen)
            {
                xuanYun = false;
            }
        }
    }
   
}
/// <summary>
/// 后续改成接口TODO（如有必要）
/// </summary>
public enum IStateType
{
    None=0,
    Idle=1,
    BeHit=2,
    Attack,
    Dead,
}
