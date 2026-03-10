using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Framework.Data;
using UnityEngine.UI;
/// <summary>
/// 收获面板
/// </summary>
public class PaiQianHarvestPanel : PanelBase
{
    public SingleMiJingLevelData levelData;
    public MiJingLevelSetting levelSetting;
    public Transform trans_grid;
    public Button btn_zhaohui;//召回
    public Button btn_harvest;//收获

    public override void Init(params object[] args)
    {
        base.Init(args);
        levelData = args[0] as SingleMiJingLevelData;
        levelSetting = DataTable.FindMiJingLevelSetting(levelData.LevelId);

    
    
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

   
    }


}
