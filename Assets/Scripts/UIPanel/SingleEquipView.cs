using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Framework.Data;

public class SingleEquipView : SingleViewBase
{

    public Image img_icon;
    public Image img_frame;
    public EquipProtoData data;
    public Color32 color_choose = new Color32(251, 218, 65, 255);
    public MatchChoosePanel parentPanel;
    public Button btn;
    public override void Init(params object[] args)
    {
        base.Init(args);
        data = args[0] as EquipProtoData;
        parentPanel = args[1] as MatchChoosePanel;

        addBtnListener(btn, ()=> 
        {
            parentPanel.OnChoosedSingleEquipData(this);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ItemSetting setting = DataTable.FindItemSetting(data.settingId);

        img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + setting.UiName);

    }

    public void OnChoose(bool choose)
    {
        if (choose)
        {
            img_frame.color = color_choose;
        }
        else
        {
            img_frame.color = Color.white;
        }
    }

}
