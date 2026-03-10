using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleConsumeView : SingleViewBase
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
        if (txt != null)
            txt.gameObject.SetActive(true);

        switch (consumeType)
        {
            case ConsumeType.Property:
                propertySetting = DataTable.FindPropertySetting(id);
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.propertyIconFolderPath + propertySetting.UiName);
                if (txt != null)
                    txt.SetText(num.ToString());
                break;
            case ConsumeType.Item:
                itemSetting = DataTable.FindItemSetting(id);
                if (itemSetting == null) break;
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + itemSetting.UiName);
                ItemData itemData = new ItemData();
                itemData.settingId = id;
                img_bg.ShowItemFrameImg(itemData);
                long myNum = 0;
                if (id == (int)ItemIdType.LingShi)
                {
                    myNum = ItemManager.Instance.FindLingShiCount();
                }
                else
                {
                    myNum = (long)ItemManager.Instance.FindItemCount(id);
                }
                if (txt != null)
                    txt.SetText(UIUtil.ShowBigCount(myNum)+"/"+ UIUtil.ShowBigCount(num));
                if (myNum >= num)
                {
                    if (txt != null)
                        txt.color = new Color32(147, 213,18,255);
                }
                else
                {
                    if (txt != null)
                        txt.color = new Color32(214,45,45,255);
                }
                addBtnListener(btn, () =>
                {
                    PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData, true);
                });
                break;
        }
        float theWidth = xOffset ;
        if (txt != null) theWidth += txt.preferredWidth;
        if (theWidth < minWidth)
            theWidth = minWidth;
        rect.sizeDelta = new Vector2(theWidth, rect.sizeDelta.y);
    }
}

public enum ConsumeType
{
    None=0,
    Item=1,
    Property=2,
}
