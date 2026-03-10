
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipChangePanel : PanelBase
{
    public List<Button> posBtnList;
    public EquipPanel equipPanel;
    public int choosedIndex;
    public List<EquipItemView> viewList = new List<EquipItemView>();
    //public Transform grid_curEquip;
    public Transform grid;
    public Transform trans_noEquip;
    public PeopleData p;

    public Transform trans_choosedEquipTipsParent;
    public Transform trans_myEquipTipsParent;
    EquipItemView curChoosedItemView;
    public Transform trans_downBtns;
    public Button btn_equip;
    public Button btn_unEquip;
    public Button btn_dissolve;
    public Button btn_handle;
    Sprite sprt_choose;
    Color choosecolor = Color.white;
    Color unchoosecolor = new Color32(239,240,161,255);

    Sprite sprt_unChoose;


    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        choosedIndex = (int)args[1];
        equipPanel = args[2] as EquipPanel;



        sprt_choose = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "btn_dizi_ui_Anniu1");
        sprt_unChoose = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "btn_dizi_ui_Anniu2");

        for (int i = 0; i < posBtnList.Count; i++)
        {
            Button btn = posBtnList[i];
            int index = i;
            addBtnListener(btn, () =>
            {
                choosedIndex = index;
                for(int j = 0; j < posBtnList.Count; j++)
                {
                    Image img = posBtnList[j].GetComponent<Image>();
                    Text txt= posBtnList[j].GetComponentInChildren<Text>();
                    if (j == index)
                    {
                        img.sprite = sprt_choose;
                        txt.color = choosecolor;
                    }
                    else
                    {
                        img.sprite = sprt_unChoose;
                        txt.color = unchoosecolor;
                    }
                }


                RefreshShow();
            });
        }

        addBtnListener(btn_dissolve, () =>
        {
            List<SingleDanFarmData> farmList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.LianQi);
            if (farmList.Count > 0)
            {
                //SingleDanFarmData farmData=ZongMenManager.Instance.FindTypeFarmList(DanFarmType.LianQi)
                PanelManager.Instance.OpenPanel<EquipDissolvePanel>(PanelManager.Instance.trans_layer2, farmList[0], curChoosedItemView.GetItemData());
            }
            else
            {
                PanelManager.Instance.OpenFloatWindow("您没有炼器房，请造一个");
            }
       
        });
        addBtnListener(btn_equip, () =>
        {
            EquipmentManager.Instance.OnEquip(p, curChoosedItemView.GetItemData(), choosedIndex);
            if (TaskManager.Instance.guide_equipEquip)
            {
                PanelManager.Instance.CloseTaskGuidePanel();
                TaskManager.Instance.guide_equipEquip = false;
            }
        });
        addBtnListener(btn_unEquip, () =>
        {
            EquipmentManager.Instance.OnUnEquip(p, curChoosedItemView.GetItemData(), choosedIndex);
        });
        addBtnListener(btn_handle, () =>
        {
            PanelManager.Instance.OpenPanel<EquipHandlePanel>(PanelManager.Instance.trans_layer2, curChoosedItemView.GetItemData());
        });


        RegisterEvent(TheEventType.OnEquip, OnEquip);
        RegisterEvent(TheEventType.OnUnEquip, OnEquip);
        RegisterEvent(TheEventType.DissolvedEquip, OnEquipDissolve);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        posBtnList[choosedIndex].onClick.Invoke();
        ShowGuide();
    }

    void ShowGuide()
    {
        //觉醒
        if (TaskManager.Instance.guide_equipEquip)
        {
            if (viewList.Count > 0)
            {
                PanelManager.Instance.ShowTaskGuidePanel(viewList[0].gameObject);
            }
        }
    }

    void RefreshShow()
    {
        PanelManager.Instance.CloseAllPanel(trans_choosedEquipTipsParent);
        PanelManager.Instance.CloseAllPanel(trans_myEquipTipsParent);

        trans_downBtns.gameObject.SetActive(false);
        viewList.Clear();
        //ClearCertainParentAllSingle<SingleViewBase>(grid_curEquip);
        //if (p.curEquipItemList[choosedIndex] != null)
        //{
        //    AddSingle<EquipItemView>(grid_curEquip, p.curEquipItemList[choosedIndex],p);
        //}

        ClearCertainParentAllSingle<SingleViewBase>(grid);
        if (p.curEquipItemList[choosedIndex] != null)
        {
            viewList.Add(AddSingle<EquipItemView>(grid, p.curEquipItemList[choosedIndex], p, this));
        }
        List<ItemData> datas = ItemManager.Instance.FindEquipItemListByPosIndex(choosedIndex);
        for (int i = 0; i < datas.Count; i++)
        {
            ItemData data = datas[i];
            PeopleData belongP = null;
            if (data.equipProtoData.isEquipped)
            {
                belongP = StudentManager.Instance.FindStudent(data.equipProtoData.belongP);
            }
            viewList.Add(AddSingle<EquipItemView>(grid, datas[i], belongP, this));
        }
        if (viewList.Count > 0)
        {
            viewList[0].btn.onClick.Invoke();
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
    /// <summary>
    /// 选中了某个装备
    /// </summary>
    /// <param name="theView"></param>
    public void OnChoosedItem(EquipItemView theView)
    {
        curChoosedItemView = theView;
        //tips
        PanelManager.Instance.CloseAllPanel(trans_myEquipTipsParent);
        PanelManager.Instance.CloseAllPanel(trans_choosedEquipTipsParent);
 
        for(int i = 0; i < viewList.Count; i++)
        {
            EquipItemView view = viewList[i];
            if (view == theView)
                view.OnChoose(true);
            else
                view.OnChoose(false);
        }
        //不是我的装备，显示对比
        if (theView.GetItemData().equipProtoData.belongP != p.onlyId)
        {
            PanelManager.Instance.OpenPanel<UpItemTipsPanel>(trans_choosedEquipTipsParent, theView.GetItemData());

            if (p.curEquipItemList[choosedIndex] != null)
            {
                PanelManager.Instance.OpenPanel<UpItemTipsPanel>(trans_myEquipTipsParent, p.curEquipItemList[choosedIndex]);
            }

            btn_equip.gameObject.SetActive(true);
            btn_unEquip.gameObject.SetActive(false);
            btn_dissolve.gameObject.SetActive(true);
        }
        //是我的装备 不显示对比
        else
        {
            PanelManager.Instance.OpenPanel<UpItemTipsPanel>(trans_myEquipTipsParent, theView.GetItemData());

            btn_equip.gameObject.SetActive(false);
            btn_unEquip.gameObject.SetActive(true);
            btn_dissolve.gameObject.SetActive(false);
        }

        trans_downBtns.gameObject.SetActive(true);
        if (TaskManager.Instance.guide_equipEquip)
        {
            if (viewList.Count > 0)
            {
                PanelManager.Instance.ShowTaskGuidePanel(btn_equip.gameObject);
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        //ClearCertainParentAllSingle<SingleViewBase>(grid_curEquip);
        ClearCertainParentAllSingle<SingleViewBase>(grid);
        viewList.Clear();
        PanelManager.Instance.CloseAllPanel(trans_choosedEquipTipsParent);
        PanelManager.Instance.CloseAllPanel(trans_myEquipTipsParent);

    }

    internal void OnChoosedItem(EquipItemposView equipItemposView)
    {
        throw new NotImplementedException();
    }
}
