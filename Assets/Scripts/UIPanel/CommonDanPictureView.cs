using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonDanPictureView : ItemView
{
    public LianDanBuildingPanel parentPanel;

    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as LianDanBuildingPanel;

    
    }
}
