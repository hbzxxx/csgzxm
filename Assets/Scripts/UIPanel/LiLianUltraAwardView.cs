using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 历练获取的额外物品
/// </summary>
public class LiLianUltraAwardView : SingleViewBase
{
    public float speed;
    public LiLianPanel parentPanel;
    public ItemData item;
    public Image img;
    bool valid = false;
    public override void Init(params object[] args)
    {
        base.Init(args);
        Vector2 localPos = (Vector2)args[0];
        item = args[1] as ItemData;
        parentPanel = args[2] as LiLianPanel;
        transform.localPosition = localPos;
        valid = true;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
         if(item.settingId==(int)ItemIdType.LingShi
            &&item.count== 80)
        {
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + "img_fufai3");
        }
        else
        {
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + DataTable.FindItemSetting(item.settingId).UiName);
        }
      
    }

    private void Update()
    {
        if(!parentPanel.startYieldShowFlyAnim)
        transform.localPosition =new Vector2(transform.localPosition.x - speed * Time.deltaTime, transform.localPosition.y);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (valid
            &&collision.gameObject.name == "lilianSke")
        {

            //获得物品
            parentPanel.OnGetUltraItem(this);
            valid = false;
            PanelManager.Instance.CloseSingle(this);
        }
    }
}
