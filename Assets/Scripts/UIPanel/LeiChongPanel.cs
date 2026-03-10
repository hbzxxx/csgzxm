using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeiChongPanel:PanelBase
{
    public Transform grid;

    public override void Init(params object[] args)
    {
        base.Init(args);

    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        for(int i = 0; i < DataTable._leiChongList.Count; i++)
        {
            AddSingle<SingleLeiChongView>(grid, i);
        }
    }

    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
    }
}
