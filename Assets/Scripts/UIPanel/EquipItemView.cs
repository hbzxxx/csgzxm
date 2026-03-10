using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipItemView : ItemView
{
    public EquipChangePanel parentPanel;
    public Text txt;
    public PeopleData belongP;
    public Transform trans_tag;
    public GameObject obj_choose;
    public Text dengji;
    public override void Init(params object[] args)
    {
        base.Init(args);
        belongP = args[1] as PeopleData;
        parentPanel = args[2] as EquipChangePanel;
        addBtnListener(btn, () =>
        {
            if (itemData?.equipProtoData?.setting == null || parentPanel == null)
                return;
            int posIndex = itemData.equipProtoData.setting.Pos.ToInt32();
            parentPanel.OnChoosedItem(this);
            ////如果已装备 则弹小窗
            //if (itemData.equipProtoData.isEquipped)
            //{
            //    PanelManager.Instance.CloseAllPanel(parentPanel.sonPanelParent);
            //    PanelManager.Instance.OpenPanel<EquipChangePanel>(parentPanel.sonPanelParent, belongP, posIndex, parentPanel);
           
            //    //PanelManager.Instance.OpenPanel<UpItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData, belongP,false);
            //}//未装备，弹在下面
            //else
            //{
            //    //如果该位置有已装备的 弹小窗
            //    if (belongP.curEquipItemList[posIndex] != null)
            //    {
            //        PanelManager.Instance.OpenPanel<UpItemTipsPanel>(PanelManager.Instance.trans_layer2, belongP.curEquipItemList[posIndex], belongP, false);
            //    }
            //    //该位置没有装备
            //    else
            //    {

            //    }

            //    //PanelManager.Instance.OpenPanel<HandleItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData, belongP, false);
                
            //}
        });

        RegisterEvent(TheEventType.EquipIntense, RefreshShow);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    public override void RefreshShow()
    {
        base.RefreshShow();      
        txt.SetText("Lv." + itemData.equipProtoData.curLevel);
        if(dengji!=null)
            dengji.SetText(itemData.equipProtoData.curLevel.ToString());
        if (itemData.equipProtoData.isEquipped)
        {
            trans_tag.gameObject.SetActive(true);
        }
        else
        {
            trans_tag.gameObject.SetActive(false);
        }
    }

    public void OnChoose(bool choose)
    {
        obj_choose.SetActive(choose);
    }

    public override void Clear()
    {
        base.Clear();
        OnChoose(false);
    }
}
