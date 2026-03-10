using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WithoutKuangConsumeView : SingleViewBase
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
    public override void Init(params object[] args)
    {
        base.Init(args);

        int id = (int)args[0];
        num = (int)args[1];
        consumeType = (ConsumeType)args[2];

        switch (consumeType)
        {
            case ConsumeType.Property:
                propertySetting = DataTable.FindPropertySetting(id);
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.propertyIconFolderPath + propertySetting.UiName);
                txt.SetText(num.ToString());
                break;
            case ConsumeType.Item:
                itemSetting = DataTable.FindItemSetting(id);
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + itemSetting.UiName);
                ItemData itemData = new ItemData();
                itemData.settingId = id;
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
                    txt.color = Color.green;
                }
                else
                {
                    txt.color = Color.red;
                }
                addBtnListener(btn, () =>
                {

                    PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData,true);
                });
                break;

        }
        rect.sizeDelta = new Vector2(xOffset + txt.preferredWidth, rect.sizeDelta.y);
    }

}
