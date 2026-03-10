using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class ChooseFarmProductMatPanel : PanelBase
{
    public Transform trans_chooseProduct;//选择

    public List<Button> bigLevelBtnList;
    public List<GameObject> bigLevelBtnBottomImgList;

    int curChoosedBigLevel = 0;
    public ScrollViewNevigation scrollViewNevigation;

    public DanFarmSetting danFarmSetting;//
    public SingleDanFarmData singleDanFarmData;
    public Transform trans_productViewGrid;//产品
    public List<DanFarmProductChooseView> danFarmProductChooseViewList = new List<DanFarmProductChooseView>();
    public Button btn_start;//开始炼制
    public int choosedSettingId;//选择的id

    #region 正在产出
    public Transform trans_producing;//产出中
    public Text txt_jiaSuLianDanNeedd;
    public Button btn_jiaSuLianDan;
    public Image img_productIcon;
    public Text txt_productName;//产品名
    public GameObject trans_remainBar;//剩余
    public Image img_remain;//剩余
    public Text txt_remain;//剩余多少
    public Button btn_receive;
    public Button btn_cancelProduct;//取消
    #endregion

    #region slider
    public Button btn_closeSlider;
    public Transform trans_choose;//选择
    public int chooseNum;//选择数量
    public Text txt_choosedNum;//已选数量
    public Text txt_minNum;//最小
    public Text txt_maxNum;//最大
    public Slider slider;//滑动条

    public ItemSetting itemSetting;
    public Transform trans_matGrid;
    public List<SingleConsumeView> matViewList = new List<SingleConsumeView>();

    bool matEnough;
    public Button btn_confirmProduct;//确定生产
    //int choosedProductNum;//选择生产多少个
    #endregion

    public override void Init(params object[] args)
    {
        base.Init(args);
        singleDanFarmData = args[0] as SingleDanFarmData;

        danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);

        addBtnListener(btn_closeSlider, () =>
        {
            PanelManager.Instance.CloseTaskGuidePanel();
            trans_choose.gameObject.SetActive(false);
        });

        addBtnListener(btn_confirmProduct, () =>
         {
             if (chooseNum <= 0)
             {
                 PanelManager.Instance.OpenFloatWindow("未选择数量");
                 return;
             }
             else
             {
                 if (itemSetting == null )
                 {
                     return;
                 }
                 trans_choose.gameObject.SetActive(false);
                 LianDanManager.Instance.ProductNeedMatItem(singleDanFarmData, itemSetting.Id.ToInt32(), chooseNum);
                 //PanelManager.Instance.ClosePanel(this);
                 TaskManager.Instance.guide_lianDan = false;
                 PanelManager.Instance.CloseTaskGuidePanel();
             }
           
         });

       for(int i = 0; i < bigLevelBtnList.Count; i++)
        {
            Button btn = bigLevelBtnList[i];
            int index = i;
            addBtnListener(btn, () =>
             {
                 OnBigLevelBtnClick(index + 1);

                 for(int j=0;j< bigLevelBtnBottomImgList.Count; j++)
                 {
                     GameObject obj = bigLevelBtnBottomImgList[j];
                     if (j == index)
                         obj.gameObject.SetActive(true);
                     else
                         obj.gameObject.SetActive(false);
                 }
                 for (int j = 0; j < bigLevelBtnList.Count; j++)
                 {
                     Text txt = bigLevelBtnList[j].GetComponentInChildren<Text>();
                     if (j == index)
                     {
                         txt.color = Color.white;
                         txt.GetComponent<Outline>().effectColor = Color.black;
                     }
                     else
                     {
                         txt.color = new Color32(111, 105, 70, 255);
                         txt.GetComponent<Outline>().effectColor = Color.white;
                     }
                     //    obj.gameObject.SetActive(true);
                     //else
                     //    obj.gameObject.SetActive(false);
                 }
             });
        }

        addBtnListener(btn_start, () =>
        {
            if (choosedSettingId != 0)
            {
                OnOpenChoosePanel(choosedSettingId);
            }
        });
        addBtnListener(btn_cancelProduct, () =>
        {
            PanelManager.Instance.OpenCommonHint("取消将返还材料，确定取消吗？", () =>
            {
                LianDanManager.Instance.OnStopLianDan(singleDanFarmData);
            }, null
             );
        });

        addBtnListener(btn_jiaSuLianDan, () =>
        {
            //加速炼丹
            if (RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, LianDanManager.Instance.JiaSuLianDanNeedTili(singleDanFarmData)))
            {
                LianDanManager.Instance.OnJiaSuLianDan(singleDanFarmData);
            }
            else
            {
                PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);

            }
        });
        //收获
        addBtnListener(btn_receive, () =>
        {
            List<int> settingIdList = new List<int>();
            List<ulong> numList = new List<ulong>();
            for (int i = 0; i < singleDanFarmData.ProductItemList.Count; i++)
            {
                settingIdList.Add(singleDanFarmData.ProductItemList[i].settingId);
                numList.Add(singleDanFarmData.ProductItemList[i].count);
            }
            ItemManager.Instance.GetItemWithAwardPanel(settingIdList, numList);
            singleDanFarmData.ProductItemList.Clear();
            ShowContent();
        });
        RegisterEvent(TheEventType.StartMatDanFarmProduce, StartDanFarmProduce);
        RegisterEvent(TheEventType.DanFarmProduceProcess, StartDanFarmProduce);
        RegisterEvent(TheEventType.StopMatDanFarmProduce, StopDanFarmProduce);
    }


    public override void OnOpenIng()
    {
        base.OnOpenIng();

        int giantLevel = RoleManager.Instance.GiantLevel
            (RoleManager.Instance._CurGameInfo.playerPeople); //RoleManager.Instance._CurGameInfo.playerPeople.CurTrainIndex / 30 + 1;
        if (singleDanFarmData.Status == (int)DanFarmStatusType.Working)
        {
            choosedSettingId = singleDanFarmData.ProductSettingId;
            ShowProcess();
        }
        else 
        {
            if (singleDanFarmData.ProductItemList.Count > 0)
            {
                choosedSettingId = singleDanFarmData.ProductItemList[0].settingId;

                ShowProcess();
            }else
            ShowContent();

        }

        //for(int i = 0; i < singleDanFarmData.UnlockedProductIdList.Count; i++)
        //{
        //    DanFarmProductChooseView view = AddSingle<DanFarmProductChooseView>(trans_productViewGrid, singleDanFarmData.UnlockedProductIdList[i],this,false);
        //}
        trans_choose.gameObject.SetActive(false);
        txt_choosedNum.SetText("0");

        ShowGuide();
    }
    void ShowGuide()
    {
        if (TaskManager.Instance.guide_lianDan)
        {
           
            List<List<int>> dan = CommonUtil.SplitCfg(danFarmSetting.Param);
            int choosedIndex = 0;
            for(int i = 0; i < dan.Count; i++)
            {
                List<int> groupDan = dan[i];
                for(int j = 0; j < groupDan.Count; j++)
                {
                    int singleDan = groupDan[j];
                    if (singleDan == TaskManager.Instance.guide_lianDanId)
                    {
                        choosedIndex = singleDan;
                        break;
                    }
                }
            }
            int choosedBigLevel = 1;
            //找biglevel
            for(int i = 1; i < 6; i++)
            {
                int index1 = (i - 1) * 3;
                int index2 = index1 + 3;
                if (choosedIndex >= index1 && choosedIndex < index2)
                {
                    choosedBigLevel = i;
                    break;
                }
            }
            if (curChoosedBigLevel != choosedBigLevel)
                PanelManager.Instance.ShowTaskGuidePanel(bigLevelBtnList[choosedBigLevel - 1].gameObject);
            else
            {
                //选择要的丹
                for(int i = 0; i < danFarmProductChooseViewList.Count; i++)
                {
                    DanFarmProductChooseView view = danFarmProductChooseViewList[i];
                    if (view.itemSetting.Id.ToInt32() == TaskManager.Instance.guide_lianDanId)
                    {
                        //定位过去
                        PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, view.btn.gameObject);
                    }
                }
            }
        }
    }



    void OnBigLevelBtnClick(int bigLevel)
    {
        curChoosedBigLevel = bigLevel;
        //int unlockedBigLevel = (singleDanFarmData.CurLevel - 1) / 3+1;
        //if (unlockedBigLevel < bigLevel)
        //{
        //    int needLevel = (bigLevel - 1) * 3 + 1;
        //    PanelManager.Instance.OpenFloatWindow(danFarmSetting.name + "Lv." + needLevel + "解锁");
        //    return;
        //}
        //显示该等级的所有丹
        List<List<int>> dan = CommonUtil.SplitCfg(danFarmSetting.Param);
        int index1= (bigLevel - 1) * 3;
        int index2 = index1 + 3;
        danFarmProductChooseViewList.Clear();
        ClearCertainParentAllSingle<DanFarmProductChooseView>(trans_productViewGrid);
        for(int i = index1; i < index2; i++)
        {
            List<int> single = dan[i];
            bool locked = false;
            string lockTxt = "";
            if (singleDanFarmData.CurLevel < i + 1)
            {
                locked = true;
                lockTxt = danFarmSetting.Name + "Lv" + (i + 1) + "解锁";
            }

            for (int j = 0; j < single.Count; j++)
            {
                DanFarmProductChooseView view = AddSingle<DanFarmProductChooseView>(trans_productViewGrid, single[j], this, locked, lockTxt);
                danFarmProductChooseViewList.Add(view);
            }

        }
        //选中第一个
        if (!danFarmProductChooseViewList[0].locked)
        {
            danFarmProductChooseViewList[0].btn.onClick.Invoke();
        }
        else
        {
            btn_start.gameObject.SetActive(false);
            choosedSettingId = 0;
        }
    }

    /// <summary>
    /// 开始产出需要材料的产品
    /// </summary>
    /// <param name="args"></param>
    public void StartDanFarmProduce(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            ShowProcess();
        }
    }
    /// <summary>
    /// 停止产出需要材料的产品
    /// </summary>
    /// <param name="args"></param>
    public void StopDanFarmProduce(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
                ShowProcess();
         
        }
    }
    /// <summary>
    /// 显示选择丹药
    /// </summary>
    void ShowContent()
    {
        trans_producing.gameObject.SetActive(false);

        trans_chooseProduct.gameObject.SetActive(true);
        int giantLevel = RoleManager.Instance.GiantLevel
    (RoleManager.Instance._CurGameInfo.playerPeople); //RoleManager.Instance._CurGameInfo.playerPeople.CurTrainIndex / 30 + 1;
        bigLevelBtnList[giantLevel - 1].onClick.Invoke();


    }

    /// <summary>
    /// 显示正在产出
    /// </summary>
    void ShowProcess()
    {
        trans_chooseProduct.gameObject.SetActive(false);
        trans_producing.gameObject.SetActive(true);

        txt_remain.SetText((singleDanFarmData.ProductTotalNum - singleDanFarmData.ProductRemainNum) + "/" + singleDanFarmData.ProductTotalNum);

        txt_jiaSuLianDanNeedd.SetText("-" + LianDanManager.Instance.JiaSuLianDanNeedTili(singleDanFarmData));
        ItemSetting setting = Framework.Data.DataTable.FindItemSetting(choosedSettingId);
        img_productIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + setting.UiName);
        txt_productName.SetText("当前产出：" + setting.Name);
        trans_remainBar.gameObject.SetActive(true);
        img_remain.gameObject.SetActive(true);
        img_remain.fillAmount = (singleDanFarmData.ProductTotalNum - singleDanFarmData.ProductRemainNum) / (float)singleDanFarmData.ProductTotalNum;

        if (singleDanFarmData.Status == (int)DanFarmStatusType.Working)
        {
            btn_jiaSuLianDan.gameObject.SetActive(true);
            btn_cancelProduct.gameObject.SetActive(true);
            btn_receive.gameObject.SetActive(false);
        }
        else
        {
            btn_jiaSuLianDan.gameObject.SetActive(false);
            btn_cancelProduct.gameObject.SetActive(false);
            btn_receive.gameObject.SetActive(true);
        }

    }

    public void OnChoosed(DanFarmProductChooseView view)
    {
        choosedSettingId = view.itemSetting.Id.ToInt32();
        for(int i = 0; i < danFarmProductChooseViewList.Count; i++)
        {
            DanFarmProductChooseView theView = danFarmProductChooseViewList[i];
            if (theView == view)
            {
                theView.OnChoose(true);
            }
            else
            {
                theView.OnChoose(false);
            }
        }
        btn_start.gameObject.SetActive(true);
    }

    /// <summary>
    /// 打开选择面板
    /// </summary>
    /// <param name="itemId"></param>
    public void OnOpenChoosePanel(int itemSettingId)
    {
        itemSetting = DataTable.FindItemSetting(itemSettingId);

        ClearCertainParentAllSingle<SingleConsumeView>(trans_matGrid);

        matViewList.Clear();
        List<List<int>> matList = CommonUtil.SplitCfg(itemSetting.Param2);

        for (int i = 0; i < matList.Count; i++)
        {
            List<int> singleMat = matList[i];
            int needNum = singleMat[1];
            SingleConsumeView view = AddSingle<SingleConsumeView>(trans_matGrid, singleMat[0], needNum, ConsumeType.Item);
            matViewList.Add(view);
        }

        trans_choose.gameObject.SetActive(true);

        //可以做的最大数量
        int maxCount = FindCanMakeMaxCount();
        int minCount = 0;
        txt_minNum.SetText(minCount.ToString());
        txt_maxNum.SetText(maxCount.ToString());

        //int divideNum=
        slider.value = 0;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((x) =>
        {
            //slider.normalizedValue
            int num = Mathf.RoundToInt(maxCount * x);
            if (maxCount > 0)
            {
                //这里要divide一下
                //int intNum=Mathf.RoundToInt()
                slider.value = num / (float)maxCount;
            }
            else
            {
                slider.value = 0;
            }
            OnChoosedNum(num);
        });
        if (maxCount > 0)
        {
            slider.value=(1 / (float)maxCount);
        }
        if (TaskManager.Instance.guide_lianDan
            &&TaskManager.Instance.guide_lianDanId==itemSettingId)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_confirmProduct.gameObject);
        }

    }

    /// <summary>
    /// 选择了数量
    /// </summary>
    public void OnChoosedNum(int num)
    {
        matEnough = true;
        List<List<int>> matList=CommonUtil.SplitCfg(itemSetting.Param2);
       
        for(int i = 0; i < matList.Count; i++)
        {
            List<int> singleMat = matList[i];
            int showNum = num;
            if (num == 0)
                showNum = 1;
            int needNum = singleMat[1]* showNum;
            ulong myNum = ItemManager.Instance.FindItemCount(singleMat[0]);
            if (myNum < (ulong)needNum)
            {
                matEnough = false;
                return;
            }
            matViewList[i].Init(singleMat[0], needNum, ConsumeType.Item);
        }
        chooseNum = num;
        txt_choosedNum.SetText(chooseNum.ToString());
    }

    /// <summary>
    /// 找能做的最大数量
    /// </summary>
    /// <returns></returns>
    public int FindCanMakeMaxCount()
    {
        List<List<int>> matList = CommonUtil.SplitCfg(itemSetting.Param2);
        List<int> myNumList = new List<int>();
        List<int> enoughList = new List<int>();//哪几项够做多少个

        for (int i = 0; i < matList.Count; i++)
        {
            List<int> singleMat = matList[i];
            int needNum = singleMat[1];
            ulong myNum = ItemManager.Instance.FindItemCount(singleMat[0]);

            int count =(int)( myNum / (ulong)needNum);
            enoughList.Add(count);
        }
        int minval = int.MaxValue;
        //找enoughlist的最小值
        for(int i = 0; i < enoughList.Count; i++)
        {
            if (enoughList[i] <= minval)
            {
                minval = enoughList[i];
            }
        }
        return minval;
    }

    public override void OnClose()
    {
        base.OnClose();
        TaskManager.Instance.guide_lianDan = false;
        PanelManager.Instance.CloseTaskGuidePanel();
        choosedSettingId = 0;
        btn_start.gameObject.SetActive(false);
    }
}
