using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 镶嵌宝石
/// </summary>
public class EquipAddGemPanel : PanelBase
{

    public Transform trans_gemGrid;//所有宝石
    public List<GemKnapsackItemView> gemKnapsackItemViewList = new List<GemKnapsackItemView>();

    public List<Transform> trans_inlayedGemParentList;//镶嵌宝石父物体
    public List<Image> img_inlayedGemOutFrameList;//镶嵌宝石外框
    public List<Image> img_inlayedGemChooseFrameList;//镶嵌宝石外框选中特效

    public List<InlayedGemItemView> inlayedGemViewList = new List<InlayedGemItemView>();//镶嵌了的宝石
    public List<Transform> trans_proGridList;//属性


    public ItemData curChoosedGemItem;//当前选的宝石
    public ItemData equipItemData;//装备

    public Transform trans_equipShow;//装备

     public Button btn_inlay;//镶嵌
    public Button btn_offLay;//卸下

    public Transform trans_gemDes;//描述
    public Transform trans_curChoosedGemParent;//当前选的宝石
    public Text txt_curChoosedGemName;//当前选的宝石名字
    public Text txt_curChoosedGemDes;//当前选的宝石描述
    public Image img_curChoosedGemProBar;//当前选的宝石bar
    public Text txt_curChoosedGemProName;//当前选的宝石属性名
    public Text txt_curChoosedGemProNum;//当前选的宝石属性数值

    public override void Init(params object[] args)
    {
        base.Init(args);
        equipItemData = args[0] as ItemData;

        RegisterEvent(TheEventType.OnInlayGem, OnInlayedGem);
        RegisterEvent(TheEventType.OnOffLayGem, OnOffLayGem);
        addBtnListener(btn_inlay, () =>
         {
             if (!curChoosedGemItem.gemData.isInlayed)
             {
                 int choosedIndex = -1;
                 //找最近的空位
                 for(int i = 0; i < equipItemData.equipProtoData.gemList.Count; i++)
                 {
                     ItemData gem = equipItemData.equipProtoData.gemList[i];
                     if (gem == null||gem.onlyId<=0)
                     {
                         choosedIndex = i;
                         break;
                     }
                     //    continue;
                     //ulong theOnlyId = equipItemData.equipProtoData.gemList[i].onlyId;
                     //if (theOnlyId == 0)
                     //{
                   
                     //}
                 }
                 if (choosedIndex >= 0)
                 {
                     if (equipItemData.equipProtoData.gemList[choosedIndex] == null || equipItemData.equipProtoData.gemList[choosedIndex].onlyId <= 0)
                     {
                         if (ItemManager.Instance.LoseItem(curChoosedGemItem.onlyId))
                         {
                             PeopleData p = StudentManager.Instance.FindStudent(equipItemData.equipProtoData.belongP);

                             EquipmentManager.Instance.OnInlayGem(p, equipItemData.equipProtoData, curChoosedGemItem, choosedIndex);

                         }
                     }
                 
                 }
                 else
                 {
                     PanelManager.Instance.OpenFloatWindow("宝石栏已满，请先卸除部分宝石");
                 }
             }

         });

        addBtnListener(btn_offLay, () =>
        {
            if (curChoosedGemItem.gemData.isInlayed)
            {
                int index = -1;
                for (int i = 0; i < equipItemData.equipProtoData.gemList.Count; i++)
                {
                    ItemData gem = equipItemData.equipProtoData.gemList[i];
                    if (gem != null&&gem.onlyId>0)
                    {
                        ulong theOnlyId = gem.onlyId;
                        if (curChoosedGemItem.onlyId == theOnlyId)
                        {
                            index = i;
                            break;
                        }
                    }
                 
                }
                if (index != -1)
                {
                    PeopleData p = StudentManager.Instance.FindStudent(equipItemData.equipProtoData.belongP);

                    EquipmentManager.Instance.OnOffLayGem(p, equipItemData.equipProtoData, index);

                }
            }
        });
        //addBtnListener(btn_info, () =>
        //{
        //    if(curChoosedGemItem!=null)
        //    PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, curChoosedGemItem);
        //});
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowEquipAndGem();
        ShowAllGem();
        btn_inlay.gameObject.SetActive(false);
        btn_offLay.gameObject.SetActive(false);
        //btn_info.gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }


    void OnInlayedGem()
    {
        ShowEquipAndGem();
        ShowAllGem();
        btn_inlay.gameObject.SetActive(false);
        btn_offLay.gameObject.SetActive(false);
        //btn_info.gameObject.SetActive(false);

        for (int i = 0; i < gemKnapsackItemViewList.Count; i++)
        {
            GemKnapsackItemView theView = gemKnapsackItemViewList[i];

            theView.OnChoosed(false);

        }
    }

    void OnOffLayGem()
    {
        ShowEquipAndGem();
        ShowAllGem();
        btn_inlay.gameObject.SetActive(false);
        btn_offLay.gameObject.SetActive(false);
       // btn_info.gameObject.SetActive(false);
        for (int i = 0; i < gemKnapsackItemViewList.Count; i++)
        {
            GemKnapsackItemView theView = gemKnapsackItemViewList[i];
         
                theView.OnChoosed(false);
            
        }
    }


    /// <summary>
    /// 显示装备和宝石
    /// </summary>
    void ShowEquipAndGem()
    {
        inlayedGemViewList.Clear();
        for (int i = 0; i < trans_inlayedGemParentList.Count; i++)
        {
            PanelManager.Instance.CloseAllSingle(trans_inlayedGemParentList[i]);
        }
        PanelManager.Instance.CloseAllSingle(trans_equipShow);

        PanelManager.Instance.OpenSingle<ItemView>(trans_equipShow, equipItemData);

        for (int i = 0; i < equipItemData.equipProtoData.gemList.Count; i++)
        {
            ItemData gem = equipItemData.equipProtoData.gemList[i];
            if (gem == null||gem.onlyId<=0)
                continue;
            ulong onlyId = gem.onlyId;
            //该位置有宝石
            if (onlyId > 0)
            {
                //ItemData gem = equipItemData.equipProtoData.gemList[i];
                InlayedGemItemView view = PanelManager.Instance.OpenSingle<InlayedGemItemView>(trans_inlayedGemParentList[i], gem, this);
                inlayedGemViewList.Add(view);
                img_inlayedGemOutFrameList[i].gameObject.SetActive(true);
                CommonUtil.ShowItemFrameImg(img_inlayedGemOutFrameList[i], gem);
            }
            else
            {
                img_inlayedGemOutFrameList[i].gameObject.SetActive(false);
            }
            
        }


      
        for(int i = 0; i < trans_proGridList.Count; i++)
        {
            Transform grid = trans_proGridList[i];

            ClearCertainParentAllSingle<InEquipGemPropertyView>(grid);
        }

        if (equipItemData.equipProtoData.gemList.Count > 0)
        {
            for (int i = 0; i < equipItemData.equipProtoData.gemList.Count; i++)
            {
                ItemData gemItem = equipItemData.equipProtoData.gemList[i];
                if (gemItem == null||gemItem.onlyId<=0)
                    continue;
                ulong onlyId = gemItem.onlyId;
                if (onlyId > 0)
                {
                    //ItemData gemItem = equipItemData.equipProtoData.gemList[i];
                    int proId = 0;
                    int proNum = 0;
                    SinglePropertyData proData = new SinglePropertyData();
                    for (int j = 0; j < gemItem.gemData.propertyIdList.Count; j++)
                    {
                        SinglePropertyData theproData = gemItem.gemData.propertyList[j];
                        proData.id = theproData.id;
                        proData.num += theproData.num;
                    
                    }
                    AddSingle<InEquipGemPropertyView>(trans_proGridList[i], true, proData,gemItem.gemData);

                  
                }
            }
        }
          
 

        //int emptyCount = 3 - itemView.GetItemData().gemData.PropertyList.Count;
        //for (int i = 0; i < emptyCount; i++)
        //{
        //    AddSingle<GemPropertyView>(trans_gemShowProGrid, false);
        //}
    }

    /// <summary>
    /// 显示所有宝石
    /// </summary>
    void ShowAllGem()
    {
        PanelManager.Instance.CloseAllSingle(trans_gemGrid);
        gemKnapsackItemViewList.Clear();
        List<ItemData> gemList = ItemManager.Instance.FindItemListByType(ItemType.Gem);
        for(int i = 0; i < gemList.Count; i++)
        {
            ItemData data = gemList[i];
            
            GemKnapsackItemView view= PanelManager.Instance.OpenSingle<GemKnapsackItemView>(trans_gemGrid, data, this);
            gemKnapsackItemViewList.Add(view);
        }
    }
    /// <summary>
    /// 点击了装配上去的宝石
    /// </summary>
    /// <param name="view"></param>
    public void OnClickedEquippedGem(InlayedGemItemView view)
    {
        curChoosedGemItem = view.GetItemData();

        for(int i = 0; i < gemKnapsackItemViewList.Count; i++)
        {
            gemKnapsackItemViewList[i].OnChoosed(false);
        }

        for (int i = 0; i < trans_inlayedGemParentList.Count; i++)
        {
            Transform trans = trans_inlayedGemParentList[i];
            if (trans.childCount > 0 && trans.GetChild(0).GetComponent<InlayedGemItemView>().GetItemData().onlyId == view.GetItemData().onlyId)
            {
                img_inlayedGemChooseFrameList[i].gameObject.SetActive(true);
            }
            else
            {
                img_inlayedGemChooseFrameList[i].gameObject.SetActive(false);

            }
        }

        for (int i = 0; i < inlayedGemViewList.Count; i++)
        {
            InlayedGemItemView theView = inlayedGemViewList[i];
            if (theView.GetItemData().onlyId == view.GetItemData().onlyId)
            {
                theView.OnChoosed(true);

            }
            else
            {
                theView.OnChoosed(false);
            }
        }
        if (curChoosedGemItem.gemData.isInlayed)
        {
            btn_offLay.gameObject.SetActive(true);
            btn_inlay.gameObject.SetActive(false);
        }
        else
        {
            btn_offLay.gameObject.SetActive(false);
            btn_inlay.gameObject.SetActive(true);
        }
        //btn_info.gameObject.SetActive(true);

        OnShowCurChoosedGem();

    }

    /// <summary>
    /// 点击了下面的宝石
    /// </summary>
    /// <param name="view"></param>
    public void OnClickedItem(GemKnapsackItemView view)
    {
        curChoosedGemItem = view.GetItemData();

        for(int i = 0; i < inlayedGemViewList.Count; i++)
        {
            inlayedGemViewList[i].OnChoosed(false);
        }
        for (int i = 0; i < trans_inlayedGemParentList.Count; i++)
        {
            Transform trans = trans_inlayedGemParentList[i];
            img_inlayedGemChooseFrameList[i].gameObject.SetActive(false);

        }
        for (int i = 0; i < gemKnapsackItemViewList.Count; i++)
        {
            GemKnapsackItemView theView = gemKnapsackItemViewList[i];
            if (theView.GetItemData().onlyId == view.GetItemData().onlyId)
            {
                theView.OnChoosed(true);
            }
            else
            {
                theView.OnChoosed(false);
            }
        }
        if (curChoosedGemItem.gemData.isInlayed)
        {
            btn_offLay.gameObject.SetActive(true);
            btn_inlay.gameObject.SetActive(false);
        }
        else
        {
            btn_offLay.gameObject.SetActive(false);
            btn_inlay.gameObject.SetActive(true);
        }
        //btn_info.gameObject.SetActive(true);

        OnShowCurChoosedGem();
    }

    /// <summary>
    /// 显示当前选的宝石
    /// </summary>
    void OnShowCurChoosedGem()
    {
         ClearCertainParentAllSingle<ItemViewbs>(trans_curChoosedGemParent);
        trans_gemDes.gameObject.SetActive(true);
        ItemViewbs itemView = AddSingle<ItemViewbs>(trans_curChoosedGemParent, curChoosedGemItem);
        txt_curChoosedGemName.SetText(itemView.setting.Name);
        txt_curChoosedGemDes.SetText(itemView.setting.Des);
        int maxProNum = 0;
        int curProNum = 0;
        string baiFen = "%";
        for(int i = 0; i < curChoosedGemItem.gemData.propertyList.Count; i++)
        {
            SinglePropertyData pro = curChoosedGemItem.gemData.propertyList[i];
            if (pro.id == (int)PropertyIdType.MPSpeed
           || pro.id == (int)PropertyIdType.JingTong)
            {
                baiFen = "";
            }
            curProNum += pro.num;
            maxProNum += ItemManager.Instance.GetGemProLimitById(curChoosedGemItem.settingId);
            PropertySetting setting = DataTable.FindPropertySetting(pro.id);
            txt_curChoosedGemProName.SetText(setting.Name);

        }
  
        txt_curChoosedGemProNum.SetText(curProNum + baiFen);
        img_curChoosedGemProBar.fillAmount = curProNum / (float)maxProNum;
    }

    public override void Clear()
    {
        base.Clear();
        gemKnapsackItemViewList.Clear();
        PanelManager.Instance.CloseAllSingle(trans_gemGrid);

        inlayedGemViewList.Clear();
        for(int i = 0; i < trans_inlayedGemParentList.Count; i++)
        {
            PanelManager.Instance.CloseAllSingle(trans_inlayedGemParentList[i]);

        }
        curChoosedGemItem = null;
        ClearCertainParentAllSingle<ItemViewbs>(trans_curChoosedGemParent);
        trans_gemDes.gameObject.SetActive(false);

        for(int i = 0; i < img_inlayedGemOutFrameList.Count; i++)
        {
            Image img_bottom = img_inlayedGemOutFrameList[i];
            img_bottom.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.itemGreenBgName);
        }

        for (int i = 0; i < trans_inlayedGemParentList.Count; i++)
        {
            img_inlayedGemChooseFrameList[i].gameObject.SetActive(false);

        }
    }
}
