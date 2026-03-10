using Coffee.UIEffects;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleYuanSuPointView : SingleViewBase
{
    public Image img;
    public YuanSuType yuanSuType;
    public int posIndex;
    public Vector2 localPos;
    public Button btn;
    public UIDissolve uIDissolve;
    public override void Init(params object[] args)
    {
        base.Init(args);
        yuanSuType = (YuanSuType)args[0];
        posIndex = (int)args[1];
        localPos = (Vector2)args[2];
        uIDissolve.DOKill();
        uIDissolve.effectFactor = 0;
        
        //新手引导的按钮
        addBtnListener(btn, OnBtnClick);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        //img.color = ConstantVal.YuanSuColor(yuanSuType);
        img.sprite = ConstantVal.YuanSuInBattleIcon(yuanSuType);

        transform.localPosition = localPos;

        transform.DOKill();
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(1, 0.1f);

    }

    void OnBtnClick()
    {

    }

    /// <summary>
    /// 移除
    /// </summary>
    public void OnRemove()
    {

        DOTween.To(() => uIDissolve.effectFactor, x => uIDissolve.effectFactor=x, 1, .6f).OnComplete(() =>
        {
            PanelManager.Instance.CloseSingle(this);
        });
    }

    public override void OnClose()
    {
        base.OnClose();
    }
}
