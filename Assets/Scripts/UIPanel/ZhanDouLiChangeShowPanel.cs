using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZhanDouLiChangeShowPanel : PanelBase
{
    long beforeNum;
    long afterNum;
    public Image img;
    public Text txt_zhanLi;
    public Text txt_change;
    public float offset = 95;
    public override void Init(params object[] args)
    {
        base.Init(args);
        beforeNum = (long)args[0];
        afterNum = (long)args[1];

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        long change = afterNum - beforeNum;
        txt_zhanLi.DOKill();
        txt_change.DOKill();
        img.DOKill();

        txt_zhanLi.color = Color.white;
        txt_change.color = Color.white;
        img.color = new Color32(0, 0, 0, 115);

        txt_zhanLi.SetText("战斗力" + beforeNum);

        RectTransform zhanLiRect = txt_zhanLi.GetComponent<RectTransform>();
        zhanLiRect.sizeDelta =new Vector2(txt_zhanLi.preferredWidth, zhanLiRect.sizeDelta.y);
        txt_change.transform.localPosition = new Vector2(zhanLiRect.localPosition.x+ zhanLiRect.sizeDelta.x / 2 + offset,0);
        Color changeColor = Color.white;
        string changeTxt = "";
        if (change > 0)
        {
            changeTxt = "+";
            changeColor = Color.green;
        }
        else
        {
            changeColor = Color.red;
        }
        txt_change.SetText(changeTxt+change.ToString());
        txt_change.color = changeColor;
        //移动
        DOTween.To(() => beforeNum, (x) =>

        {
            beforeNum = x;
            txt_zhanLi.SetText("战斗力" +beforeNum.ToString());
        }, afterNum, 1).OnComplete(() =>
        {
            img.DOFade(0, 0.5f);
            txt_zhanLi.DOFade(0, 0.5f);
            txt_change.DOFade(0, 0.5f).OnComplete(() =>
            {
                PanelManager.Instance.ClosePanel(this);
            });

        });
    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.curZhanDouLiChangeShowPanel = null;
    }

}
