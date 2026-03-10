using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipPanel : PanelBase
{
    public StudentHandlePanel parentPanel;
    public List<SingleEquipPosView> posViewList;
    public ItemData choosedEquip;
     //public int curPosIndex;
    public PeopleData p;
    public Text txt_taoZhuangDes;//套装描述
    public Transform sonPanelParent;
    public Transform grid_pro;

    public Button btn_autoEquip;
    public Button btn_autoUnEquip;
    public Button btn_change;

    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        for(int i = 0; i < posViewList.Count; i++)
        {
            posViewList[i].Clear();
            posViewList[i].Init(p,this);
            posViewList[i].OnOpenIng();

        }

        addBtnListener(btn_autoEquip, () =>
        {
            EquipmentManager.Instance.AuToEquip(p);
        });
        addBtnListener(btn_autoUnEquip, () =>
        {
            for(int i = 0; i < p.curEquipItemList.Count; i++)
            {
                ItemData item = p.curEquipItemList[i];
                int index = i;
                if (item != null)
                    EquipmentManager.Instance.OnUnEquip(p, item, index);
            }
        });
        addBtnListener(btn_change, () =>
        {
            posViewList[0].btn.onClick.Invoke();
        });
        RegisterEvent(TheEventType.OnEquip, OnEquip);
        RegisterEvent(TheEventType.OnUnEquip, OnEquip);
        RegisterEvent(TheEventType.DissolvedEquip, OnEquipDissolve);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
        ShowGuide();
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_equipEquip)
        {
            PanelManager.Instance.ShowTaskGuidePanel(posViewList[0].gameObject);
        }
        if (TaskManager.Instance.guide_intenseEquip)
        {
            ItemView target = null;
            for(int i = 0; i < posViewList.Count; i++)
            {
                ItemView theView = posViewList[i].itemView;
                if (theView != null)
                {
                    target = theView;
                }
            }
            if(target!=null)
            PanelManager.Instance.ShowTaskGuidePanel(target.btn.gameObject);
        }
    }

    void RefreshShow()
    {
        for (int i = 0; i < posViewList.Count; i++)
        {
            posViewList[i].RefreshShow();

        }
        List<EquipTaoZhuangType> taoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(p);
        string des = "";
        if (taoZhuangList.Count > 0)
        {
            EquipTaoZhuangType type1 = taoZhuangList[0];
            des += "<color=green><b>" + DataTable.FindEquipTaoZhuangSetting((int)type1).Name+ "</b></color>" + "\n";

            des += EquipmentManager.Instance.TaoZhuangDes(type1, false);

            if (taoZhuangList.Count >= 2)
            {
                EquipTaoZhuangType type2 = taoZhuangList[1];
                if (type1 == type2)
                {
                    des += "\n" + EquipmentManager.Instance.TaoZhuangDes(type2, true);
                }
                else
                {
                    des += "\n"+ "<color=green><b>" + DataTable.FindEquipTaoZhuangSetting((int)type2).Name+ "</b></color>"+ "\n";

                    des += EquipmentManager.Instance.TaoZhuangDes(type2, false);
                }
            }
        }
 
  
     

        txt_taoZhuangDes.SetText(des);
        ClearCertainParentAllSingle<SingleViewBase>(grid_pro);

        List<SinglePropertyData> proList = new List<SinglePropertyData>();
        for(int i = 0; i < p.curEquipItemList.Count; i++)
        {
            ItemData item = p.curEquipItemList[i];
            // 检查 item 是否有效（settingId > 0 表示有效的装备数据）
            if (item != null && item.settingId > 0 && item.equipProtoData != null)
            {
                for(int j = 0; j < item.equipProtoData.propertyList.Count; j++)
                {
                    proList.Add(item.equipProtoData.propertyList[j]);
                }
            }
        }
        proList=PropertyManager.Instance.CombineProperty(proList);
        for(int i = 0; i < proList.Count; i++)
        {
            proList[i].quality = (int)Quality.None;
            AddSingle<BigStudentSinglePropertyView>(grid_pro, proList[i].id, proList[i].num, (Quality)(int)proList[i].quality);
        }

    }
    void OnEquip(object[] args)
    {
        RefreshShow();
    }

    void OnEquipDissolve(object[] args)
    {
        RefreshShow();
    }
     

    public override void Clear()
    {
        base.Clear();
 
        for(int i = 0; i < posViewList.Count; i++)
        {
            posViewList[i].Clear();
        }
        PanelManager.Instance.CloseAllPanel(sonPanelParent);
        ClearCertainParentAllSingle<SingleViewBase>(grid_pro);

    }


    public override void OnClose()
    {
        base.OnClose();
    }
}
