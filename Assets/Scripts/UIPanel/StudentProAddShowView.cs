 using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 弟子加成的显示动画
/// </summary>
public class StudentProAddShowView : SingleViewBase
{
    public int initVal;
    public int finalVal;

    public Text txt_Val;//值
    public Text txt_studentAdd;//弟子
    public Transform trans_moveStartPos;//从哪里开始移动

    public override void Init(params object[] args)
    {
        base.Init(args);

        initVal = (int)args[0];
        finalVal = (int)args[1];
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        txt_Val.SetText(initVal.ToString());
        txt_studentAdd.DOKill();
        txt_studentAdd.transform.position = trans_moveStartPos.position;
        txt_studentAdd.SetText(("+" + (finalVal - initVal)));

        txt_studentAdd.transform.DOLocalMoveY(0, 1f).OnComplete(() =>
        {
            DOTween.To(() => initVal,
                x =>
                {
                    initVal = x;
                    txt_Val.SetText(initVal.ToString());
                }, 
                finalVal, 1);
        });

    }
}
