using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InlayedGemItemView : ItemView
{
    public EquipAddGemPanel parentPanel;
    public Image img_bg;

    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as EquipAddGemPanel;

        addBtnListener(btn, () =>
        {
            parentPanel.OnClickedEquippedGem(this);
        });
    }


    public void OnChoosed(bool choose)
    {
        if (choose)
        {
            img_bg.color = ConstantVal.color_choosed;
        }
        else
        {
            img_bg.color = Color.white;
        }
    }
}
