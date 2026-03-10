
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipFixPanel : PanelBase
{
    public Transform trans_itemGrid;
    public Text txt_process;

    public override void OnOpenIng()
    {
        base.OnOpenIng();
      
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_itemGrid);
    }
}
