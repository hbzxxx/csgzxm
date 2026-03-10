using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YuanSuReactionTxtView : FinishKillEffect
{
    public Vector2Int RdmMoveYRange = new Vector2Int(80, 120);//y坐标随机数

    public Vector2Int RdmYRange = new Vector2Int(0, 100);//y坐标随机数
    public Vector2Int RdmXRange = new Vector2Int(-100, 100);//x坐标随机数

    public Text txt_num;
    public Text txt_function;

    public float bigScale = 1.3f;
    public float becomeBigTime = 0.2f;

    public float becomeSmallTime = 1f;

    public bool isNum = false;
    public float moveTime = 1.5f;
    //public Image img_crit;//暴击标志

    public override void Init(params object[] args)
    {
        base.Init(args);
        string str = (string)args[0];
        bool isNum = (bool)args[1];
        YuanSuType yuanSuType = (YuanSuType)args[2];
        transform.DOKill();
        transform.localPosition = new Vector2(RandomManager.Next(RdmXRange[0], RdmXRange[1]), RandomManager.Next(RdmYRange[0], RdmYRange[1]));

        //如果是数字
        if (isNum)
        {
            txt_function.gameObject.SetActive(false);

            txt_num.gameObject.SetActive(true);
            txt_num.color = ConstantVal.YuanSuColor(yuanSuType);
            txt_num.SetText(str);
        }
        else
        {
            txt_num.gameObject.SetActive(false);

            txt_function.gameObject.SetActive(true);
            txt_function.color = ConstantVal.YuanSuColor(yuanSuType);
            txt_function.SetText(str);

        }
        transform.DOLocalMoveY(transform.localPosition.y+ RandomManager.Next(RdmMoveYRange[0], RdmMoveYRange[1]), moveTime);

        transform.DOScale(bigScale, becomeBigTime).OnComplete(() =>
        {
            transform.DOScale(1, becomeSmallTime);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

}
