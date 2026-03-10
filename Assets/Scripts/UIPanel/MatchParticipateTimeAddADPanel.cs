using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 5次额外大比次数
/// </summary>
public class MatchParticipateTimeAddADPanel : PanelBase
{

    public Text txt_remain;
    public Transform trans_consumeGrid;
    public Button btn;

    public override void Init(params object[] args)
    {
        base.Init(args);

        int watchedNum = 0;
        //if (RoleManager.Instance._CurGameInfo.MatchData.WatchedADAddParticipateTime)
        //    watchedNum = 1;

        watchedNum = RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum;

        txt_remain.SetText("今日剩余"+"("+(ConstantVal.maxMatchTianJingBrushNum) + "/" + watchedNum + ")");

        addBtnListener(btn, () =>
        {
            watchedNum = RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum;

            if (watchedNum >= ConstantVal.maxMatchTianJingBrushNum)
            {
                PanelManager.Instance.OpenFloatWindow("今日增加次数已达上限");
                return;


            
            }else if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingJing, (ulong)ConstantVal.MatchAddCountNeedTianJing))
            {
                PanelManager.Instance.OpenCommonHint("天晶不够，是否前往购买？", () =>
                {
                    PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2,ShopTag.ChongZhi);
                }, null);
                return;
            }
            else
            {
                //可以看
                //ADManager.Instance.WatchAD(ADType.AddMatchParticipateNum);
                MatchManager.Instance.OnADAddMatchNum();
                PanelManager.Instance.ClosePanel(this);
            }

        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ClearCertainParentAllSingle<SingleConsumeView>(trans_consumeGrid);
        AddSingle<SingleConsumeView>(trans_consumeGrid, (int)ItemIdType.LingJing, ConstantVal.MatchAddCountNeedTianJing, ConsumeType.Item);
    }
}
