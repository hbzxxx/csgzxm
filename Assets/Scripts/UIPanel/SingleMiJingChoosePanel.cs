using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;

public class SingleMiJingChoosePanel : PanelBase
{
    public Transform trans_grid;
    public SingleMiJingPaiQianData singleMiJingPaiQianData;
    public Image img_label;

    public override void Init(params object[] args)
    {
        base.Init(args);

        singleMiJingPaiQianData = args[0] as SingleMiJingPaiQianData;

        string iconName = "";
        SingleMiJingLevelData singleMiJingLevelData = singleMiJingPaiQianData.LevelList[0];
        MiJingLevelSetting setting = DataTable.FindMiJingLevelSetting(singleMiJingLevelData.LevelId);
        switch ((TaoFaType)setting.TaoFaType.ToInt32())
        {
            case TaoFaType.YunHu:
                iconName = "img_out_yunhu";
                break;
            case TaoFaType.XingChenJian:
                iconName = "img_out_xingchengjian";
                break;
            case TaoFaType.QingLingYu:
                iconName = "img_out_qinglingcheng";
                break;
            case TaoFaType.ShenQiMuDi:
                iconName = "img_out_shenqimudi";
                break;
        }
        img_label.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.artResPath + iconName);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        for(int i=0;i< singleMiJingPaiQianData.LevelList.Count; i++)
        {
            SingleMiJingLevelData singleMiJingLevelData = singleMiJingPaiQianData.LevelList[i];
            MiJingLevelView levelView = AddSingle<MiJingLevelView>(trans_grid, singleMiJingLevelData);
        }
    }
}
