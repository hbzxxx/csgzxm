using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
public class XueMaiManager : CommonInstance<XueMaiManager>
{

    public Dictionary<XueMaiType, XueMaiUpgradeSetting> xueMaiUpgradeDicByType=new Dictionary<XueMaiType, XueMaiUpgradeSetting>();//血脉升级字典

    /// <summary>
    /// 必要数据缓存免得查表
    /// </summary>
    public override void Init()
    {
        base.Init();


    }

    /// <summary>
    /// 升级血脉
    /// </summary>
    /// <param name="p"></param>
    public void OnUpgrade(PeopleData p,XueMaiType type)
    {
        long before = 0;
        if(p.isPlayer)
            before= RoleManager.Instance.CalcZhanDouLi();

        int limit = limitLevel(p);

        int index = p.xueMai.xueMaiTypeList.IndexOf(type);
        int curLevel = p.xueMai.xueMaiLevelList[index];
       
        if (curLevel >= limit)
        {
            PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.当前武备等级已达上限请先升级人物等级));
            return;
        }
        p.xueMai.xueMaiLevelList[index]++;
        RoleManager.Instance.RefreshBattlePro(p);
        EventCenter.Broadcast(TheEventType.OnUpgradeXueMai, p, type);
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            StudentManager.Instance.RefreshRedPointShow(RoleManager.Instance._CurGameInfo.studentData.allStudentList[i]);
  
        }
        XueMaiManager.Instance.RefreshRedPoint();
        EventCenter.Broadcast(TheEventType.RefreshZhangMenRedPoint);

        if (p.isPlayer)
        {
            long after = RoleManager.Instance.CalcZhanDouLi();
            PanelManager.Instance.OpenZhanDouLiChangePanel(before, after);

        }
    }

    /// <summary>
    /// 最大等级
    /// </summary>
    /// <returns></returns>
    public int limitLevel(PeopleData p)
    {
        return (RoleManager.Instance.FindCenterLevel(p) - 1) * 5;
    }
    /// <summary>
    /// 最大等级
    /// </summary>
    /// <returns></returns>
    public int limitLevel(int trainIndex)
    {
        return (trainIndex/10) * 5;
    }
    /// <summary>
    /// 需要某个时期才能开启
    /// </summary>
    /// <param name="limitLevel"></param>
    /// <returns></returns>
    public int limitLevelNeedTrainIndex(int limitLevel)
    {
        
        return limitLevel / 5 + 1;
    }

    public int PerLevelAddNum(XueMaiType xueMaiType)
    {
        if (xueMaiType == XueMaiType.JingTong)
            return 7;
        else
            return 1;
    }

    public int FindXueMaiLevel(PeopleData p,XueMaiType xueMai)
    {
        if (p.xueMai == null)
            return 0;
        int index = p.xueMai.xueMaiTypeList.IndexOf(xueMai);
        if (index == -1) return 1;
        return p.xueMai.xueMaiLevelList[index];
 
    }

    /// <summary>
    /// 红点
    /// </summary>
    public void InitRedPoint()
    {
        RedPoint MainPanel_Btn_ZhangMen = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_ZhangMen, 0);
        RedPoint MainPanel_Btn_ZhangMen_XueMai= RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_ZhangMen_XueMai, 0);
        RedPointManager.Instance.BindRedPoint(MainPanel_Btn_ZhangMen, MainPanel_Btn_ZhangMen_XueMai);
        RefreshRedPoint();
    }

    public void RefreshRedPoint()
    {
        bool canUpgradeXueMai = false;
        int xueMailimit = XueMaiManager.Instance.limitLevel(RoleManager.Instance._CurGameInfo.playerPeople);
        for (int i = 1; i < (int)XueMaiType.End; i++)
        {

            XueMaiType type = (XueMaiType)i;
            XueMaiUpgradeSetting xueMaiUpgradeSetting = DataTable.FindXueMaiUpgradeSettingByType(type);

            int index = RoleManager.Instance._CurGameInfo.playerPeople.xueMai.xueMaiTypeList.IndexOf(type);
            int curXueMaiLevel = RoleManager.Instance._CurGameInfo.playerPeople.xueMai.xueMaiLevelList[index];
            if (curXueMaiLevel < xueMailimit)
            {
                List<List<List<int>>> itemNeedParam = CommonUtil.SplitThreeCfg(xueMaiUpgradeSetting.NeedItem);
                List<List<int>> itemNeed = itemNeedParam[curXueMaiLevel];
                bool singleItemEnough = true;
                for (int j = 0; j < itemNeed.Count; j++)
                {
                    List<int> single = itemNeed[j];
                    int id = single[0];
                    int num = single[1];
                    if (!ItemManager.Instance.CheckIfItemEnough(id, (ulong)num))
                    {
                        singleItemEnough = false;
                        break;
                    }
                }
                if (singleItemEnough)
                {
                    canUpgradeXueMai = true;
                    break;

                }

            }



        }
        RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_ZhangMen_XueMai, 0, canUpgradeXueMai);

    }

 
}
