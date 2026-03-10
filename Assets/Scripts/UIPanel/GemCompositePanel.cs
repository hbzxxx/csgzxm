using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemCompositePanel : PanelBase
{

    public Transform trans_noGem;//没有任何宝石
    public Transform trans_scroll;//所有宝石
    List<GemItemView> gemItemViewList = new List<GemItemView>();
    public Transform trans_chooseCompositePanel;
    public Transform grid_chooseItemGrid;//选择物品
     public ItemData choosedToCompositeGem;//选择谁去合成
    public ItemSetting curGemItemSetting;

    public Transform grid_allItem;//选择谁去合成的所有宝石界面
    public Transform grid_chooseToCompositeGemProGrid;//选择谁去合成的属性界面
    public Transform grid_matPro;//材料属性
    //public Button btn_chooseToComposite;//选择去合成
    public Button btn_cancelChoosedGem;//取消选择的宝石
    //public Transform trans_composite;//合成材料页面
    public Transform trans_choosedMatGird;//选择了什么材料展示父物体
    //public Transform trans_gemMatGrid;//用作材料的宝石
    //public Button btn_backComposite;//从合成界面返回
    public List<GemCompositeMatItemView> gemCompositeMatItemViewList = new List<GemCompositeMatItemView>();
    //public Transform trans_noMat;//没有适合的材料
    public GemCompositeMatItemView curChoosedMatView;//当前选的材料

    public Button btn_composite;//合成
    public Transform trans_compositeLingShiNeedMat;//合成需要灵石
    //public Transform trans_curItemParent;//当前显示的装备
    public Transform trans_gemProGrid;//宝石属性格子
    //public Text txt_name;
    public SingleDanFarmData farmData;
    public override void Init(params object[] args)
    {
        base.Init(args);
        //gemItemData = args[0] as ItemData;
        //curGemItemSetting = DataTable.FindItemSetting(gemItemData.SettingId);
        farmData = args[0] as SingleDanFarmData;
        RegisterEvent(TheEventType.CompositeGem, OnCompositeGem);

        addBtnListener(btn_composite, () =>
        {
            if (curChoosedMatView.GetItemData().gemData.isInlayed)
            {
                PanelManager.Instance.OpenCommonHint("该宝石已被镶嵌，合成将消耗该宝石，继续吗？", () =>
                {
                    ItemManager.Instance.OnCompositeGem(choosedToCompositeGem.onlyId, curChoosedMatView.GetItemData().onlyId, farmData);

                }, null);
            }
            else
            {
                ItemManager.Instance.OnCompositeGem(choosedToCompositeGem.onlyId, curChoosedMatView.GetItemData().onlyId, farmData);
            }
        });
        //addBtnListener(btn_chooseToComposite, () =>
        //{
        //    if (choosedToCompositeGem != null)
        //    {
        //       int gemSettingId= choosedToCompositeGem.settingId;
        //        GemSetting gemSetting = DataTable.FindGemSetting(gemSettingId);
        //        if (gemSetting.nextGem.ToInt32() == -1)
        //        {
        //            PanelManager.Instance.OpenFloatWindow("该宝石已合成至最高级");
        //            return;
        //        }
        //     }

        //    OnShowMatPage();
        //});

        addBtnListener(btn_cancelChoosedGem, () =>
         {
             OnShowChooseGemPage();
         });

     
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        OnShowChooseGemPage();
    }

    /// <summary>
    /// 选择材料页面显示
    /// </summary>
    void OnShowMatPage()
    {
        //trans_chooseCompositePanel.gameObject.SetActive(false);
        //trans_composite.gameObject.SetActive(true);

        ShowGemItem();
        ShowAllMat();
        if (gemCompositeMatItemViewList.Count > 0)
        {
            //gemCompositeMatItemViewList[0].btn.onClick.Invoke();
            trans_noGem.gameObject.SetActive(false);
            //btn_composite.gameObject.SetActive(true);
        }
        else
        {
            trans_noGem.gameObject.SetActive(true);
           // btn_composite.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 选择宝石界面点击
    /// </summary>
    void OnShowChooseGemPage()
    {
        gemItemViewList.Clear();
        //trans_composite.gameObject.SetActive(false);
        trans_chooseCompositePanel.gameObject.SetActive(true);
        ClearCertainParentAllSingle<SingleConsumeView>(trans_compositeLingShiNeedMat);
        ClearCertainParentAllSingle<SingleViewBase>(grid_allItem);
        ClearCertainParentAllSingle<GemPropertyView>(grid_chooseToCompositeGemProGrid);
        ClearCertainParentAllSingle<GemPropertyView>(grid_matPro);

        ClearCertainParentAllSingle<ItemView>(trans_choosedMatGird);
        //显示所有材料
        List<ItemData> gemList = ItemManager.Instance.FindItemListByType(ItemType.Gem);
        for(int i = 0; i < gemList.Count; i++)
        {
            ItemData item = gemList[i];
            GemSetting setting = DataTable.FindGemSetting(item.gemData.gemSettingId);
            if (setting.Level.ToInt32() >= 3)
                continue;
            gemItemViewList.Add(AddSingle<GemItemView>(grid_allItem, item,this));
        }
        ClearCertainParentAllSingle<ItemView>(grid_chooseItemGrid);
        if (gemItemViewList.Count > 0)
        {
            trans_scroll.gameObject.SetActive(true);
            trans_noGem.gameObject.SetActive(false);
            //btn_chooseToComposite.gameObject.SetActive(true);
            //gemItemViewList[0].btn.onClick.Invoke();
        }
        else
        {
            trans_scroll.gameObject.SetActive(false);
            trans_noGem.gameObject.SetActive(true);
            //btn_chooseToComposite.gameObject.SetActive(false);
        }
        btn_composite.gameObject.SetActive(false);
    }

    /// <summary>
    /// 选择用来强化的
    /// </summary>
    public void OnChoosedGemToComposite(GemItemView gemItemView)
    {
        

        choosedToCompositeGem = gemItemView.GetItemData();
        curGemItemSetting = DataTable.FindItemSetting(gemItemView.GetItemData().settingId);

        for(int i = 0; i < gemItemViewList.Count; i++)
        {
            GemItemView theView = gemItemViewList[i];
            if (theView.GetItemData().onlyId == gemItemView.GetItemData().onlyId)
            {
                theView.OnChoosed(true);
            }
            else
            {
                theView.OnChoosed(false);
            }
        }
        ClearCertainParentAllSingle<ItemView>(grid_chooseItemGrid);
        AddSingle<ItemView>(grid_chooseItemGrid, choosedToCompositeGem);
        ClearCertainParentAllSingle<GemPropertyView>(grid_matPro);
        ClearCertainParentAllSingle<GemPropertyView>(grid_chooseToCompositeGemProGrid);
        for (int i = 0; i < choosedToCompositeGem.gemData.propertyList.Count; i++)
        {
            AddSingle<GemPropertyView>(grid_chooseToCompositeGemProGrid, true, choosedToCompositeGem.gemData.propertyList[i],gemItemView.GetItemData().gemData);
        }
        //int emptyCount = 3 - choosedToCompositeGem.gemNeiCunData.singlePropertyNeiCunDataList.Count;
        //for (int i = 0; i < emptyCount; i++)
        //{
        //    AddSingle<GemPropertyView>(grid_chooseToCompositeGemProGrid, false);
        //}

        //直接进入合成页面
        OnShowMatPage();
    }

    /// <summary>
    /// 显示宝石
    /// </summary>
    public void ShowGemItem()
    {
        //PanelManager.Instance.CloseAllSingle(trans_curItemParent);
        //PanelManager.Instance.OpenSingle<ItemView>(trans_curItemParent, choosedToCompositeGem);
        //txt_name.SetText(curGemItemSetting.name);
        PanelManager.Instance.CloseAllSingle(trans_gemProGrid);
        int proNum = choosedToCompositeGem.gemData.propertyList.Count;
        for (int i = 0; i < proNum; i++)
        {
            PanelManager.Instance.OpenSingle<GemPropertyView>(trans_gemProGrid,true, choosedToCompositeGem.gemData.propertyList[i],choosedToCompositeGem.gemData);
        }
        //if (proNum < 3)
        //{
        //    int emptyProNum = 3 - proNum;
        //    for(int i = 0; i < emptyProNum; i++)
        //    {
        //        PanelManager.Instance.OpenSingle<GemPropertyView>(trans_gemProGrid, false);

        //    }
        //}
    }

    /// <summary>
    /// 显示所有材料
    /// </summary>
    public void ShowAllMat()
    {
        curChoosedMatView = null;
        PanelManager.Instance.CloseAllSingle(trans_choosedMatGird);
        gemCompositeMatItemViewList.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid_allItem);
        List<ItemData> allGemList = ItemManager.Instance.FindItemListByType(ItemType.Gem);
        for(int i = 0; i < allGemList.Count; i++)
        {
            //同类
            ItemData theData = allGemList[i];
            int itemId = theData.settingId;
            ulong onlyId = theData.onlyId;
            if (itemId == choosedToCompositeGem.settingId
                &&onlyId!= choosedToCompositeGem.onlyId
                && theData.quality== choosedToCompositeGem.quality)
            {
                GemCompositeMatItemView view = PanelManager.Instance.OpenSingle<GemCompositeMatItemView>(grid_allItem, theData, this);
                gemCompositeMatItemViewList.Add(view);
            }
        
        }

    }
    /// <summary>
    /// 选择了材料
    /// </summary>
    /// <param name="view"></param>
    public void OnClickedItem(GemCompositeMatItemView view)
    {
        if (curChoosedMatView != view)
        {
            btn_composite.gameObject.SetActive(true);
            //显示材料
            ItemSetting setting = view.setting;
            GemSetting gemSetting = DataTable.FindGemSetting(setting.Param.ToInt32());
            GemSetting nextGemSetting = DataTable.FindGemSetting(gemSetting.NextGem.ToInt32());

            ClearCertainParentAllSingle<ItemView>(trans_choosedMatGird);
            AddSingle<ItemView>(trans_choosedMatGird, view.GetItemData());

            ClearCertainParentAllSingle<SingleConsumeView>(trans_compositeLingShiNeedMat);
            AddSingle<SingleConsumeView>(trans_compositeLingShiNeedMat, (int)ItemIdType.LingShi, nextGemSetting.Consume.ToInt32(), ConsumeType.Item);
            curChoosedMatView = view;
            for (int i = 0; i < gemCompositeMatItemViewList.Count; i++)
            {
                GemCompositeMatItemView theView = gemCompositeMatItemViewList[i];

                if (theView.GetItemData().onlyId == view.GetItemData().onlyId)
                {
                    theView.OnChoosed(true);
                }
                else
                {
                    theView.OnChoosed(false);
                }
            }

            ClearCertainParentAllSingle<GemPropertyView>(grid_matPro);
             for (int i = 0; i < view.GetItemData().gemData.propertyList.Count; i++)
            {
                AddSingle<GemPropertyView>(grid_matPro, true, view.GetItemData().gemData.propertyList[i],view.GetItemData().gemData);
            }
        }
        else
        {
            ClearCertainParentAllSingle<GemPropertyView>(grid_matPro);

            btn_composite.gameObject.SetActive(false);
            ClearCertainParentAllSingle<ItemView>(trans_choosedMatGird);
            ClearCertainParentAllSingle<SingleConsumeView>(trans_compositeLingShiNeedMat);
            curChoosedMatView = null;
            view.OnChoosed(false);
        }

       
    }

    public void OnCompositeGem(object[] param)
    {
        //PanelManager.Instance.ClosePanel(this);
        //选择刚才的宝石点击
        //OnShowChooseMatPage();
        OnShowChooseGemPage();
    }

    public override void Clear()
    {
        base.Clear();
        curChoosedMatView = null;
        btn_composite.gameObject.SetActive(false);
    }
}
