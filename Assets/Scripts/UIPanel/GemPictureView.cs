using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class GemPictureView : ItemView
{
    public LianDanBuildingPanel parentPanel;
    public Image img_bg;
    public Text txt_des;
    public Transform grid_mat;
    public GemSetting gemSetting;

    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as LianDanBuildingPanel;
        gemSetting = DataTable.FindGemSetting(setting.Param.ToInt32());
        addBtnListener(btn, () =>
        {
            parentPanel.ShowChoosedGem(itemData.settingId);
        });

        RegisterEvent(TheEventType.StartMakeGem, RefreshConsumeShow);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_des.SetText(setting.Des);
        RefreshConsumeShow();
    }

    /// <summary>
    /// 刷新消耗
    /// </summary>
    void RefreshConsumeShow()
    {
        PanelManager.Instance.CloseAllSingle(grid_mat);

        List<List<int>> consumeList = CommonUtil.SplitCfg(gemSetting.Consume);
        for (int i = 0; i < consumeList.Count; i++)
        {
            List<int> singleConsume = consumeList[i];
            int id = singleConsume[0];
            int count = singleConsume[1];
            PanelManager.Instance.OpenSingle<SingleConsumeView>(grid_mat, id, count, ConsumeType.Item);
        }


    }

    public void OnChoosed(bool choose)
    {
        if (choose)
            img_bg.color = ConstantVal.color_choosed;
        else
            img_bg.color = Color.white;
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(grid_mat);
    }
}
