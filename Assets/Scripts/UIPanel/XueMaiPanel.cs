using cfg;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using TooSimpleFramework.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class XueMaiPanel : PanelBase
{
    public PeopleData p;

    public List<SingleXueMaiView> singleXueMaiViewList;

    public SingleXueMaiView curChoosedXueMaiView;

    public Transform trans_curChoosedXueMai;//当前选择的血脉
    public Text txt_before;
    public Text txt_before2;
    public Text txt_after;
    public Text needCondition;//需要xx期能继续强化
    public Transform trans_needMatGrid;//需要材料

    public Button btn_intense;//强化

    public Button btn_change;//改变属性
    public Image img_curYuanSu;//当前元素

    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        addBtnListener(btn_intense, () =>
        {
            int limit = XueMaiManager.Instance.limitLevel(p);
            XueMaiType type = (XueMaiType)curChoosedXueMaiView.upgradeSetting.Type.ToInt32();

            int index = p.xueMai.xueMaiTypeList.IndexOf(type);
            int curLevel = p.xueMai.xueMaiLevelList[index];
 

            if (curLevel >= limit)
            {
                PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.当前武备等级已达上限请先升级人物等级));
                return;
            }
        
            XueMaiUpgradeSetting xueMaiUpgradeSetting = DataTable.FindXueMaiUpgradeSettingByType(type);

            List<List<List<int>>> itemNeedParam = CommonUtil.SplitThreeCfg(xueMaiUpgradeSetting.NeedItem);
            List<List<int>> itemNeed = itemNeedParam[curLevel];
            for (int i = 0; i < itemNeed.Count; i++)
            {
                List<int> single = itemNeed[i];
                int id = single[0];
                int num = single[1];
                if (!ItemManager.Instance.CheckIfItemEnough(id, (ulong)num))
                {
                    ItemSetting itemSetting = DataTable.FindItemSetting(id);
                    PanelManager.Instance.OpenFloatWindow(itemSetting.Name + "不够");
                    return;
                }
            }
            for (int i = 0; i < itemNeed.Count; i++)
            {
                List<int> single = itemNeed[i];
                int id = single[0];
                int num = single[1];
                ItemManager.Instance.LoseItem(id, (ulong)num);
            }
            XueMaiManager.Instance.OnUpgrade(p, (XueMaiType)type);
        });
        addBtnListener(btn_change, () =>
        {
            PanelManager.Instance.OpenPanel<XueMaiChangePanel>(PanelManager.Instance.trans_layer2);
        });
        RegisterEvent(TheEventType.OnUpgradeXueMai, OnUpgradeXueMai);
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();

        for(int i = 0; i < singleXueMaiViewList.Count; i++)
        {
            singleXueMaiViewList[i].Init(p, (XueMaiType)(i + 1), this);
            singleXueMaiViewList[i].RefreshShow();
        }
        singleXueMaiViewList[0].btn.onClick.Invoke();
        //txt_curYuanSu.SetText(ConstantVal.YuanSuName((YuanSuType)p.yuanSu));
        img_curYuanSu.sprite = ConstantVal.YuanSuIcon((YuanSuType)p.yuanSu);
        //if (p.isPlayer)
        //{
        //    btn_change.gameObject.SetActive(true);
        //}
        //else
        //{
        //    btn_change.gameObject.SetActive(false);
        //}
    }

    /// <summary>
    /// 提升血脉
    /// </summary>
    /// <param name="args"></param>
    void OnUpgradeXueMai(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if(p.onlyId== theP.onlyId)
        {
            for(int i = 0; i < singleXueMaiViewList.Count; i++)
            {
                singleXueMaiViewList[i].RefreshShow();
        
            }
            if (curChoosedXueMaiView != null)
                curChoosedXueMaiView.btn.onClick.Invoke();
        }
    }


    /// <summary>
    /// 选择了血脉
    /// </summary>
    public void OnChoosedXueMai(SingleXueMaiView view)
    {
        curChoosedXueMaiView = view;
        for (int i = 0; i < singleXueMaiViewList.Count; i++)
        {
            SingleXueMaiView theView = singleXueMaiViewList[i];
            if (curChoosedXueMaiView == theView) {
                theView.icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + theView.upgradeSetting.Icon + "_1");
                continue;
            }
            theView.icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + theView.upgradeSetting.Icon);
        }
        ShowCurChoosedXueMai();
    }

    /// <summary>
    /// 显示当前选择的血脉
    /// </summary>
    public void ShowCurChoosedXueMai()
    {
        ClearCertainParentAllSingle<SingleConsumeView>(trans_needMatGrid);
        trans_curChoosedXueMai.gameObject.SetActive(true);
        int limit= XueMaiManager.Instance.limitLevel(p);
        XueMaiUpgradeSetting setting = curChoosedXueMaiView.upgradeSetting;
        string percentStr = "";
        if ((XueMaiType)curChoosedXueMaiView.upgradeSetting.Type.ToInt32() != XueMaiType.JingTong)
            percentStr = "%";
        int perLevelAddNum = XueMaiManager.Instance.PerLevelAddNum((XueMaiType)curChoosedXueMaiView.upgradeSetting.Type.ToInt32());
        int maxLevel = ConstantVal.XueMaiMaxLevel;
        int index = p.xueMai.xueMaiTypeList.IndexOf((XueMaiType)curChoosedXueMaiView.upgradeSetting.Type.ToInt32());
        int curLevel = p.xueMai.xueMaiLevelList[index];
        if (curLevel == 0)
        {
            txt_before.SetText("未强化");
            txt_before2.SetText("");
        }
        else
        {
            string numText = perLevelAddNum*curLevel + percentStr;
            txt_before.SetText(setting.Function.Replace("+", ""));
            txt_before2.SetText($"<color=#80B018>+{numText}</color>");
            //if (txt_before.GetComponent<Outline>() != null)
            //{
            //    txt_before.GetComponent<Outline>().effectColor = Color.black;
            //}
        }

        if(curLevel>=limit
            && curLevel + 1 < maxLevel)
        {
            int centerLevelNeed = XueMaiManager.Instance.limitLevelNeedTrainIndex(limit+5);
            //needCondition.gameObject.SetActive(true);
            //needCondition.SetText("需要达到" +(centerLevelNeed+1)+ "级");
        }
        else
        {
            needCondition.gameObject.SetActive(false);

        }

        if (curLevel < maxLevel)
        {
            string numText = perLevelAddNum * (curLevel + 1) + percentStr;
            txt_after.SetText(setting.Function.Replace("+", "") + $"<color=#86C615>+{numText}</color>");
            //if (txt_after.GetComponent<Outline>() != null)
            //{
            //    txt_after.GetComponent<Outline>().effectColor = Color.black;
            //}
            //强化需要材料

            List<List<List<int>>> itemNeedParam = CommonUtil.SplitThreeCfg(curChoosedXueMaiView.upgradeSetting.NeedItem);
            List<List<int>> itemNeed = itemNeedParam[curLevel];
            for (int i = 0; i < itemNeed.Count; i++)
            {
                List<int> single = itemNeed[i];
                int id = single[0];
                int num = single[1];
                SingleConsumeView singleConsume = AddSingle<SingleConsumeView>(trans_needMatGrid, id, num, ConsumeType.Item);
                if (singleConsume.txt != null && singleConsume.txt.gameObject.GetComponent<Outline>() == null)
                {
                    singleConsume.txt.fontStyle = FontStyle.Normal;
                    singleConsume.txt.fontSize = 44;
                    for (int j = 0; j < 3; j++)
                    {
                        Outline outlinex = singleConsume.txt.gameObject.AddComponent<Outline>();
                        outlinex.effectColor = new Color32(24, 18, 6, 255);
                        outlinex.effectDistance = new Vector2(1,-1);
                    }
                }
            }
        }
        else
        {
            txt_after.SetText("");
        }
    }

    public override void Clear()
    {
        base.Clear();
        for(int i = 0; i < singleXueMaiViewList.Count; i++)
        {
            singleXueMaiViewList[i].Clear();
        }
    }
}
