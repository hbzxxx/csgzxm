using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanfaDatingPanel : PanelBase
{
    public Transform tran_grid;
    private List<WanfaView> wanfaViewList = new List<WanfaView>();
    private List<SingleLiLianView> singleLiLianViewList = new List<SingleLiLianView>();

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
        var wanfaDataList = new List<WanfaData>
        {
            new WanfaData 
            { 
                title = "达洛尔矿洞", 
                iconName = "mine_icon", 
                bgName = "mine_bg",
                wanfaType = WanfaType.Mining,
                isEnabled = true
            },
            new WanfaData 
            { 
                title = "哥布林巢穴", 
                iconName = "mine_icon", 
                bgName = "goblin_bg",
                wanfaType = WanfaType.Goblin,
                isEnabled = true
            },
            new WanfaData 
            { 
                title = "", 
                iconName = "", 
                bgName = "jqqd_bg",
                wanfaType = WanfaType.ComingSoon,
                isEnabled = false
            }
        };
        var leLiLianData = new List<LeLiLianData>
        {
            new LeLiLianData
            {
                settingId=10001,
                title="天命宫",
                bgName = "mine_bg",
                iconName="mine_icon"
            },
            new LeLiLianData
            {
                settingId=10002,
                title="玄武殿",
                bgName = "mine_bg",
                iconName="mine_icon"
            },
            new LeLiLianData
            {
                settingId=10003,
                title="斗化星",
                bgName = "mine_bg",
                iconName="mine_icon"
            },
            new LeLiLianData
            {
                settingId=10004,
                title="太微垣",
                bgName = "mine_bg",
                iconName="mine_icon"
            },
            new LeLiLianData
            {
                settingId=10005,
                title="天堑峰",
                bgName = "mine_bg",
                iconName="mine_icon"
            },
        };
        
        // 创建 WanfaView
        for (int i = 0; i < wanfaDataList.Count; i++)
        {
            WanfaView view = PanelManager.Instance.OpenSingle<WanfaView>(tran_grid, wanfaDataList[i], this);
            wanfaViewList.Add(view);
        }
        // 创建 SingleLiLianView
        for (int i = 0; i < wanfaDataList.Count; i++)
        {
            SingleLiLianView view = PanelManager.Instance.OpenSingle<SingleLiLianView>(tran_grid, leLiLianData[i], this);
            singleLiLianViewList.Add(view);
        }
    }

    void ClearWanfaViews()
    {
        PanelManager.Instance.CloseAllSingle(tran_grid);
        wanfaViewList.Clear();
    }

    public override void Clear()
    {
        base.Clear();
        ClearWanfaViews();
    }
}

// 玩法数据结构
public class WanfaData
{
    public string title;
    public string iconName;
    public string bgName;
    public WanfaType wanfaType;
    public bool isEnabled;
}

// 玩法类型枚举
public enum WanfaType
{
    Mining,
    Goblin,
    ComingSoon
}
public class LeLiLianData
{
    public int settingId;//关卡ID
    public string title; // 标题
    public string bgName;
    public string iconName; // 图标资源名
}
