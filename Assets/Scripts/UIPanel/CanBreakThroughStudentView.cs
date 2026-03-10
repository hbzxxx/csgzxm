using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanBreakThroughStudentView : SingleStudentView
{
    public Transform trans_breakThroughParent;//点开突破
    public Button btn_openBreakThroughParent;//打开突破
    public Transform trans_consumeGrid;//突破材料
    public Button btn_confirmBreakThrough;//突破
    public override void Init(params object[] args)
    {
  
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        trans_breakThroughParent.gameObject.SetActive(false);
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_consumeGrid);
    }
}
