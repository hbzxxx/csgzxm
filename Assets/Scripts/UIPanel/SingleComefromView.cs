
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SingleComefromView : SingleViewBase
{
    public ItemSetting itemSetting;
    public ItemData itemData;
    public int index;

    public Text txt;
    public Button btn;

    public override void Init(params object[] args)
    {
        base.Init(args);
        itemData = args[0] as ItemData;
        itemSetting = args[1] as ItemSetting;
        index = (int)args[2];

        addBtnListener(btn, () =>
        {
            if(itemData.settingId==(int)ItemIdType.BoGuo
            || itemData.settingId == (int)ItemIdType.JiuLingGuo
            || itemData.settingId == (int)ItemIdType.YinYangGuo
            || itemData.settingId == (int)ItemIdType.QingMingGuo
            || itemData.settingId == (int)ItemIdType.LongLingGuo)
            {
                JumpPageManager.Instance.JumpToWorldMap();

            }
            //清灵草
            else if (itemSetting.ItemType.ToInt32() == (int)ItemType.StudentBook)
            {
                JumpPageManager.Instance.JumpToWorldMap();

            }
            else if (itemSetting.ItemType.ToInt32() == (int)ItemType.LingQuan)
            {
                JumpPageManager.Instance.JumpToTaoFa();
            }
            else if (itemSetting.ItemType.ToInt32() == (int)ItemType.XingChen)
            {
                JumpPageManager.Instance.JumpToTaoFa();
            }
            else if (itemSetting.ItemType.ToInt32() == (int)ItemType.SkillBook)
            {
                JumpPageManager.Instance.JumpToTaoFa();
            }
            else if (itemSetting.ItemType.ToInt32() == (int)ItemType.YuanLi)
            {
                JumpPageManager.Instance.JumpToWorldMap();
            }
            else if (itemData.settingId == (int)ItemIdType.HuWangShenJin
            || itemData.settingId == (int)ItemIdType.YuSui
            || itemData.settingId == (int)ItemIdType.XingSui
            || itemData.settingId == (int)ItemIdType.XianZhuShi
            || itemData.settingId == (int)ItemIdType.BuTianShi)
            {
                JumpPageManager.Instance.JumpToWorldMap();
            }
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (!string.IsNullOrWhiteSpace(itemSetting.ComeFrom))
        {
            string[] str = itemSetting.ComeFrom.Split('|');
            txt.SetText(str[index]);

        }
  
     
    }
}
