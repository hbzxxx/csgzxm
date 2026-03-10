using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaoGanDesPanel : PanelBase
{
    public Transform grid;

    public override void OnOpenIng()
    {
        base.OnOpenIng();
      for(int i = 0; i < 3; i++)
        {
            AddSingle<SingleHaoGanDesView>(grid, i);
        }

    }

    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
    }
}
