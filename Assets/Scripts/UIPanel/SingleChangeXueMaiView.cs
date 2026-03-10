using Coffee.UIEffects;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 改变血脉属性
/// </summary>
public class SingleChangeXueMaiView : SingleViewBase
{
    public Button btn;
    public Text txt;
    public YuanSuType yuanSuType;//元素类型
    public XueMaiChangePanel parentPanel;
    public Image img;

    public override void Init(params object[] args)
    {
        base.Init(args);
        //yuanSuType = (YuanSuType)args[0];
        parentPanel = args[0] as XueMaiChangePanel;

        addBtnListener(btn, () =>
         {
             parentPanel.OnChoosedYuanSu(this);
             //RoleManager.Instance.CandidateYuanSuNum()<RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Count
         });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
        OnChoosed(false);
    }

    public void OnChoosed(bool choosed)
    {
        if (choosed)
        {
            if (GetComponent<UIEffect>().enabled == true)
            {
                GetComponent<UIEffect>().colorFactor = 0.5f;
            }
        }
        else
        {
            if (GetComponent<UIEffect>().enabled == true)
            {
                GetComponent<UIEffect>().colorFactor = 0;
            }
        }
    }

    public void RefreshShow()
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        img.DOKill();
        if (!RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Contains((int)yuanSuType))
        {
            if (RoleManager.Instance.CandidateYuanSuNum() > RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Count)
            {
                img.DOFade(.5f, 1).SetLoops(-1,LoopType.Yoyo);
                //DOTween.To(() => img.co, x => uIDissolve.effectFactor = x, 1, .6f).OnComplete(() =>
                //{
                //    PanelManager.Instance.CloseSingle(this);
                //});
            }
                img.material = PanelManager.Instance.mat_grey;
            GetComponent<UIEffect>().enabled = false;
            //obj_lock.gameObject.SetActive(true);
        }
        else
        {
            img.material = null;
            GetComponent<UIEffect>().enabled = true;
        }
        img.sprite = ConstantVal.YuanSuInBattleIcon(yuanSuType);
        //txt.SetText(ConstantVal.YuanSuName(yuanSuType));
    }

}
