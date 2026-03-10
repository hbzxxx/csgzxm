using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 弟子出战
/// </summary>
public class StudentBattlePanel : PanelBase
{
    public BattleSmallView view1;
    public BattleSmallView view2;

    public PeopleData p1;
    public PeopleData p2;

    public Transform trans_p1Parent;
    public Transform trans_p2Parent;

    public Transform trans_p1BeAttackPos;
    public Transform trans_p2BeAttackPos;

    public Transform p1LoseHpEffectPos;//掉血
    public Transform p2LoseHpEffectPos;//掉血

    public float p1Speed = 1f;
    public float p1Timer = 0;
    public bool startWait = false;
    public List<int> p1LoseHpList;
    public List<int> p2LoseHpList;

    public int curP1LoseHpIndex = 0;
    public int curP2LoseHpIndex = 0;
    public BattleType battleType;
    public override void Init(params object[] args)
    {
        base.Init(args);
        p1Timer = 0;
        curP1LoseHpIndex = 0;
        curP2LoseHpIndex = 0;
        startWait = false;

        p1 = args[0] as PeopleData;
        p2 = args[1] as PeopleData;
        battleType = (BattleType)args[2];

        RegisterEvent(TheEventType.MimicBattleRes, StartAnim);

        RegisterEvent(TheEventType.CloseStudentBattlePanel, CloseThePanel);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        //p1先出手

        PanelManager.Instance.CloseAllSingle(trans_p1Parent);
        PanelManager.Instance.CloseAllSingle(trans_p2Parent);

        view1= PanelManager.Instance.OpenSingle<BattleSmallView>(trans_p1Parent, p1,this,Direction.Left);
        view2=PanelManager.Instance.OpenSingle<BattleSmallView>(trans_p2Parent, p2,this,Direction.Right);

        view1.enemyView = view2;
        view2.enemyView = view1;
    }

    public void CloseThePanel()
    {
        PanelManager.Instance.ClosePanel(this);
    }

    public void StartAnim(object[] param)
    {
        p1LoseHpList = param[0] as List<int>;
        p2LoseHpList = param[1] as List<int>;

        
        if(p1LoseHpList.Count==0
            && p2LoseHpList.Count == 0)
        {

        }
        else
        {
            startWait = true;

        }

    }


    private void FixedUpdate()
    {
        if (startWait)
        {
            p1Timer += p1Speed;
            
            if (p1Timer >= p1Speed)
            {
                p1Timer = 0;
                startWait = false;
                //1打2
             
                    int hpNum = p2LoseHpList[curP2LoseHpIndex];
                    curP2LoseHpIndex++;

                    view1.Attack(trans_p2BeAttackPos, hpNum, p2LoseHpEffectPos);
                
            }
        }

    }

    

    /// <summary>
    /// 冲过去打完回来了
    /// </summary>
    public void OnFinishedSingleBattle(BattleSmallView view)
    {
   
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_p1Parent);
        PanelManager.Instance.CloseAllSingle(trans_p2Parent);
        PanelManager.Instance.CloseAllSingle(p1LoseHpEffectPos);
        PanelManager.Instance.CloseAllSingle(p2LoseHpEffectPos);

    }


}
