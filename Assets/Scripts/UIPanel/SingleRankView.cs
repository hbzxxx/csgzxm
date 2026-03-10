using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SingleRankView : SingleViewBase
{
    public Image img_rank;//排位
    public Text txt_rank;//排名
    public Text txt_name;
    public Text txt_chengHao;
    public Text txt_score;//分数
    public Text txt_zhanLi;
    public int rank;//排名
    public Transform trans_starShow;//星
    SingleOtherZongMenData singleOtherZongMenData;
    public bool isDownPos;

    public override void Init(params object[] args)
    {
        base.Init(args);

        singleOtherZongMenData = args[0] as SingleOtherZongMenData;
        rank = (int)args[1];
        isDownPos = (bool)args[2];
        txt_name.SetText(singleOtherZongMenData.zongMenName);
        string jia = "";
        if (rank == 99)
            jia = "+";
        txt_rank.SetText(rank.ToString()+ jia);
        txt_zhanLi.SetText("战力" + singleOtherZongMenData.totalZhanDouLi.ToString());
        txt_chengHao.SetText(ConstantVal.MatchRankName(singleOtherZongMenData.curRankLevel) + singleOtherZongMenData.curStar);
        txt_score.SetText(singleOtherZongMenData.theR.ToString());
        img_rank.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_paiwei" + singleOtherZongMenData.curRankLevel);
        img_rank.SetNativeSize();

        if (isDownPos)
        {
            trans_starShow.gameObject.SetActive(true);
            for (int i = 0; i < 5; i++)
            {
                if (i < singleOtherZongMenData.curStar)
                    trans_starShow.GetChild(i).gameObject.SetActive(true);
                else
                    trans_starShow.GetChild(i).gameObject.SetActive(false);

            }
        }
        else
        {
            trans_starShow.gameObject.SetActive(false);


        }
    }
}
