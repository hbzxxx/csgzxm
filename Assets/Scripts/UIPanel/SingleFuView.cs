using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleFuView : SingleViewBase
{
    public Button btn;
    public Text txt_fuName;
    int fuIndex;
    public ArchivePanel parentPanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        fuIndex = (int)args[0];

        parentPanel = args[1] as ArchivePanel;

        addBtnListener(btn, () =>
        {
            parentPanel.OnChoosedFu(fuIndex);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_fuName.SetText((fuIndex - 2) + "区·"  );
    }


}
