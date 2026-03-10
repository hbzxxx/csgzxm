using cfg;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleConsumeJineng : SingleViewBase
{
    public PropertySetting propertySetting;
    public ItemSetting itemSetting;
    public Image icon;
    public Text txt;
    public ConsumeType consumeType;
    public float xOffset;
    public Button btn;
    public RectTransform rect;
    public int num;
    public Image img_bg;
    public float minWidth = 100;
    public override void Init(params object[] args)
    {
        base.Init(args);

        int id = (int)args[0];
        num = (int)args[1];
        consumeType = (ConsumeType)args[2];

        txt.gameObject.SetActive(true);

        switch (consumeType)
        {
            case ConsumeType.Property:
                propertySetting = DataTable.FindPropertySetting(id);
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.propertyIconFolderPath + propertySetting.UiName);
                txt.SetText(num.ToString());
                break;
            case ConsumeType.Item:
                itemSetting = DataTable.FindItemSetting(id);
                if (itemSetting == null) break;
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + itemSetting.UiName);
                ItemData itemData = new ItemData();
                itemData.settingId = id;
                if (img_bg != null)
                {
                    img_bg.ShowItemFrameImg(itemData);
                }
                long myNum = 0;
                if (id == (int)ItemIdType.LingShi)
                {
                    myNum = ItemManager.Instance.FindLingShiCount();
                }
                else
                {
                    myNum = (long)ItemManager.Instance.FindItemCount(id);
                }
                txt.SetText(UIUtil.ShowBigCount(myNum) + "/" + UIUtil.ShowBigCount(num));
                if (myNum >= num)
                {
                    txt.color = new Color32(0, 99, 0, 255);
                }
                else
                {
                    txt.color = Color.red;
                }
                addBtnListener(btn, () =>
                {
                    PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData, true);
                });
                break;
        }
        float theWidth = xOffset + txt.preferredWidth;
        if (theWidth < minWidth)
            theWidth = minWidth;
        rect.sizeDelta = new Vector2(theWidth, rect.sizeDelta.y);
    }
}
