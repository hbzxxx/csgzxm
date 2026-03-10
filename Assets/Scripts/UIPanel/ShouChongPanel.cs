using Framework.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShouChongPanel : PanelBase
{
    public Transform grid;
    public Button btn_get;

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_get, () =>
        {
            ShopManager.Instance.GetShouChongAward();
        });
   
        RegisterEvent(TheEventType.GetShouChongAward, OnGetShouChongAward);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        List<List<int>> award = CommonUtil.SplitCfg(ConstantVal.shouChongAward);
        for(int i = 0; i < award.Count; i++)
        {
            List<int> singleAward = award[i];
            ItemData data = new ItemData();
            data.settingId = singleAward[0];
            data.count = (ulong)singleAward[1];
            if(DataTable.FindItemSetting(data.settingId)!=null)
                AddSingle<WithCountItemView>(grid, data);
        }
    }
    void OnGetShouChongAward()
    {
        PanelManager.Instance.ClosePanel(this);
    }
    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
    }
}
