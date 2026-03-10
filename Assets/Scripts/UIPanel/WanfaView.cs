using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Data;

public class WanfaView : SingleViewBase
{
    public Image img_bg;//玩法背景图
    public Image img_icon;//玩法图标
    public Text txt_title;//玩法标题
    public Transform tran_items;//玩法奖励的物品列表最多只展示2个物品
    public Button btn_enter;

    private WanfaData wanfaData;
    private WanfaDatingPanel parentPanel;

    public override void Init(params object[] args)
    {
        base.Init(args);
        
        if (args.Length >= 2)
        {
            wanfaData = args[0] as WanfaData;
            parentPanel = args[1] as WanfaDatingPanel;
            
            SetupView();
        }
    }

    void SetupView()
    {
        if (wanfaData == null) return;

        // 设置标题
        if (txt_title != null)
        {
            txt_title.text = wanfaData.title;
        }

        // 设置图标
        if (img_icon != null && !string.IsNullOrEmpty(wanfaData.iconName))
        {
            img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + wanfaData.iconName);
            img_icon.gameObject.SetActive(true);
        }
        else if (img_icon != null)
        {
            img_icon.gameObject.SetActive(false);
        }

        // 设置背景
        if (img_bg != null && !string.IsNullOrEmpty(wanfaData.bgName))
        {
            img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + wanfaData.bgName);
        }

        // 设置按钮状态和事件
        if (btn_enter != null)
        {
            btn_enter.gameObject.SetActive(wanfaData.isEnabled);
            if (wanfaData.isEnabled)
            {
                addBtnListener(btn_enter, OnEnterClick);
            }
        }
    }

    void OnEnterClick()
    {
        if (wanfaData == null) return;

        switch (wanfaData.wanfaType)
        {
            case WanfaType.Mining:
                // 挖矿玩法：先进入 LiLianPanel 选择弟子
                int defaultLiLianId = int.Parse(DataTable._liLianList[0].Id);
                PanelManager.Instance.OpenPanel<LiLianPanel>(PanelManager.Instance.trans_layer2, defaultLiLianId);
                break;
                
            case WanfaType.Goblin:
                // 哥布林巢穴玩法（待实现）
                PanelManager.Instance.OpenFloatWindow("哥布林巢穴玩法即将开放");
                break;
                
            case WanfaType.ComingSoon:
                // 敬请期待
                PanelManager.Instance.OpenFloatWindow("敬请期待");
                break;
        }
    }

    public override void Clear()
    {
        base.Clear();
        wanfaData = null;
        parentPanel = null;
    }
}
