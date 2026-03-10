using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 修为丹
/// </summary>
public class XiuWeiDanItemView : ItemView
{
    public Text txt_name;//名字
    public Text txt_param;//修为加多少

    public Text txt_count;

    public float initSpeed = 1;
    public float maxSpeed = 0.1f;
    public bool pressed = false;
    public LongPressBtnView theBtn;
    public StudentHandlePanel parentPanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as StudentHandlePanel;
        RegisterEvent(TheEventType.SuccessXiuLian, OnSuccessXiuLian);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        txt_count.SetText(itemData.count.ToString());
        int maxCount = (int)(ulong)itemData.count;
        theBtn.callBack = delegate
        {
            //修为丹修炼动画
            if(maxCount>0)
                parentPanel.OnUseXiuWeiDan(this);
            AuditionManager.Instance.PlayVoice(AudioClipType.EatDan);
            maxCount--;

            //RoleManager.Instance.OnDanXiuLian(itemData);
        };
        txt_name.SetText(setting.Name);
        txt_param.SetText(ItemManager.Instance.XiuWeiDanXiuWeiAdd(itemData).ToString());

        txt_name.color = new Color32(140, 203, 21, 255);
        txt_param.color = CommonUtil.RarityColor((Rarity)(int)itemData.quality);

    }
    void OnSuccessXiuLian()
    {
        if (itemData.count > 0)
        {
            txt_count.SetText(itemData.count.ToString());

        }
        else
        {
            PanelManager.Instance.CloseSingle(this);
        }
    }
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

    void Update()
    {

    }
}
