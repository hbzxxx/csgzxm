using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingModePanel : PanelBase
{
    public Transform trans_singleParent;
    public int buildingId;
    public bool onlyMove = false;
    public Button btn_quit;
    public override void Init(params object[] args)
    {
        base.Init(args);
        onlyMove = (bool)args[0];
        
        RegisterEvent(TheEventType.QuitBuildingMode, QuitMode);
        addBtnListener(btn_quit, () =>
        {
            BuildingManager.Instance.QuitBuildingMode();
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    /// <summary>
    /// 退出该模式
    /// </summary>
    public void QuitMode()
    {
        PanelManager.Instance.ClosePanel(this);
    }
}
