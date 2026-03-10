using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using UnityEngine.UI;

public class BattleSmallView : SingleViewBase
{
    public Image icon;
    public PeopleData peopleData;
    public Text txt_name;
    public Text txt_hp;
    public Image img_hpBar;

    public Transform trans_movingInitPos;//移动的部分初始位置
    public Transform trans_moving;//移动的部分
    public StudentBattlePanel parentPanel;
    public Direction direction;

    public int hpNum;
    public int hpLimit;

    public BattleSmallView enemyView;

    public override void Init(params object[] args)
    {
        base.Init(args);
        peopleData = args[0] as PeopleData;
        parentPanel = args[1] as StudentBattlePanel;

    

        direction = (Direction)args[2];
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
        transform.localPosition = Vector3.zero;
        trans_moving.DOKill();
        trans_moving.position = trans_movingInitPos.position;
        SinglePropertyData pro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData);
        hpNum = pro.num;
        hpLimit = (int)pro.limit;
        if (direction == Direction.Left)
        {
            icon.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            icon.transform.localEulerAngles = new Vector3(0, -180, 0);

        }
        //txt_hp.SetText(pro.Num);
    }

    public void RefreshShow()
    {
        txt_name.SetText(peopleData.name);

        txt_name.color=CommonUtil.RarityColor((Rarity)(int)peopleData.studentRarity);



     
            SinglePropertyData pro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData);
            txt_hp.SetText(pro.num + "/" + pro.limit);
            img_hpBar.fillAmount = pro.num / (float)pro.limit;


      
     
    }



    public void Attack(Transform trans_target,int hp,Transform trans_hpLoseShowParent)
    {
        trans_moving.DOMove(trans_target.position, .1f).OnComplete(() =>
        {
            enemyView.BeAttack(hp);
            trans_moving.DOMove(trans_movingInitPos.position, .1f).OnComplete(() =>
            {
                parentPanel.OnFinishedSingleBattle(this);
            });
            //对方掉血
            PanelManager.Instance.OpenSingle<TxtFlyUpAnimView>(trans_hpLoseShowParent, hp.ToString());
        });
    }

    /// <summary>
    /// 被打
    /// </summary>
    public void BeAttack(int loseNum)
    {
        hpNum += loseNum;
        txt_hp.SetText(hpNum + "/" + hpLimit);
        img_hpBar.DOFillAmount(hpNum / (float)hpLimit, .2f);
    }
}
