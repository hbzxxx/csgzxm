using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaiLineView : SingleViewBase
{
    public List<ChainLightning> chainLightningList;
    public Transform trans_start;
    public Transform trans_end;
    public override void Init(params object[] args)
    {
        base.Init(args);
        trans_start = args[0] as Transform;
        trans_end = args[1] as Transform;

        for(int i = 0; i < chainLightningList.Count; i++)
        {
            chainLightningList[i].Init();
            chainLightningList[i].StartPosition = trans_start;
            chainLightningList[i].EndPostion = trans_end;

        }

    }
}
