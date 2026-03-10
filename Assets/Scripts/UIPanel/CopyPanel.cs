using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPanel : PanelBase
{
    public Transform tran_grid;
    private List<SingleMiJingView> wanfaViewList = new List<SingleMiJingView>();

    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        CreateWanfaViews();
    }

    void CreateWanfaViews()
    {
        // 清理之前的视图
        ClearWanfaViews();

        // 玩法数据配置
        var singleMiJingData = new List<SingleMiJingData>
        {
            new SingleMiJingData
            {
                settingId=10001,
                title="焦土深渊",
                iconName="mine_icon"
            },
            new SingleMiJingData
            {
                settingId=10002,
                title="尘埃峡谷",
                iconName="mine_icon"
            },
            new SingleMiJingData
            {
                settingId=10003,
                title="幽深废墟",
                iconName="mine_icon"
            },
            new SingleMiJingData
            {
                settingId=10004,
                title="神兵遗址",
                iconName="mine_icon"
            },
        };


        // 创建 WanfaView
        for (int i = 0; i < singleMiJingData.Count; i++)
        {
            SingleMiJingView view = PanelManager.Instance.OpenSingle<SingleMiJingView>(tran_grid, singleMiJingData[i], this);
            wanfaViewList.Add(view);
        }
    }
    void ClearWanfaViews()
    {
        // 1. 先销毁列表中所有的视图对象
        foreach (var view in wanfaViewList)
        {
            if (view != null)
            {
                // 如果 SingleMiJingView 有自己的销毁方法，优先调用
                // view.DestroySelf();

                // 直接销毁游戏物体（通用方式）
                Object.Destroy(view.gameObject);
            }
        }

        // 2. 清空列表
        wanfaViewList.Clear();

        // 3. 额外防护：遍历并销毁 tran_grid 下的所有子物体
        // 倒序遍历避免索引错乱
        for (int i = tran_grid.childCount - 1; i >= 0; i--)
        {
            Transform child = tran_grid.GetChild(i);
            if (child != null)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        ClearWanfaViews();
    }
}
public class SingleMiJingData
{
    public int settingId;//关卡ID
    public string title; // 标题
    public string iconName; // 图标资源名
}
