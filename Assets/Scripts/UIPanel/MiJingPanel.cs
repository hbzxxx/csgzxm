using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiJingPanel : PanelBase
{
     public List<SingleMiJingView> singleMiJingViewList;

    public override void Init(params object[] args)
    {
        base.Init(args);

        for(int i = 0; i < singleMiJingViewList.Count; i++)
        {
            singleMiJingViewList[i].Init();
        }

    }


    public override void Clear()
    {
        base.Clear();

        for(int i = 0; i < singleMiJingViewList.Count; i++)
        {
            singleMiJingViewList[i].Clear();
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        for (int i = 0; i < singleMiJingViewList.Count; i++)
        {
            singleMiJingViewList[i].OnClose();
        }
    }
}
