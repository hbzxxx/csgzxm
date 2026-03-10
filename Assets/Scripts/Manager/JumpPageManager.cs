 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 跳转
/// </summary>
public class JumpPageManager : CommonInstance<JumpPageManager>
{
   
 

    /// <summary>
    /// 跳转去世界地图
    /// </summary>
    public void JumpToWorldMap( )
    {
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.SingleMap)
        {

            PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.当前正在界域裂隙请掌门回到宗门后再强化吧));
            return;
        }
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.Battle)
        {
             return;
        }


        GameSceneManager.Instance.GoToScene(SceneType.WorldMap);


    }
    /// <summary>
    /// 跳转去讨伐
    /// </summary>
    public void JumpToTaoFa()
    {
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.SingleMap)
        {
            PanelManager.Instance.OpenFloatWindow( LanguageUtil.GetLanguageText((int)LanguageIdType.当前正在界域裂隙请掌门回到宗门后再强化吧));
            return;
        }
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.Battle)
        {
            return;
        }
        GameSceneManager.Instance.GoToScene(SceneType.OutsideMap);
        OutsidePanel outsidePanel = GameObject.Find("OutsidePanel").GetComponent<OutsidePanel>();

        SingleMiJingView view = outsidePanel.taoFaViewList[0];

        outsidePanel.scrollViewNevigation.NevigateImmediately(view.GetComponent<RectTransform>());
     }

    /// <summary>
    /// 跳转修炼
    /// </summary>
    public void JumpToXiuLian()
    {
        PanelManager.Instance.OpenPanel<StudentHandlePanel>(PanelManager.Instance.trans_layer2);
    }

    /// <summary>
    /// 跳转炼器房
    /// </summary>
    public void JumpToEquipBuilding()
    {
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.SingleMap)
        {
            return;
        }
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.Battle)
        {
            return;
        }
        GameSceneManager.Instance.GoToScene(SceneType.Mountain);
        List<SingleDanFarmData> equipFarmList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.LianQi);
        if (equipFarmList.Count <= 0)
        {

            PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.您还没有炼器房请先建造一个));
            return;
        }
        MountainPanel mountainPanel = GameObject.Find("MountainPanel").GetComponent<MountainPanel>();

        SingleFarmView farmView = mountainPanel.FindFarmViewByOnlyId(equipFarmList[0].OnlyId);
 
        mountainPanel.scrollViewNevigation.NevigateImmediately(farmView.GetComponent<RectTransform>());
        farmView.OnClickedFarm();

    }

    /// <summary>
    /// 跳转技能
    /// </summary>
    public void JumpToSkill()
    {
        PlayerPeoplePanel panel= PanelManager.Instance.OpenPanel<PlayerPeoplePanel>(PanelManager.Instance.trans_layer2,0);
        panel.btn_skillTag.onClick.Invoke();

    }

    /// <summary>
    /// 跳转宝石房
    /// </summary>
    public void JumpToGemBuilding()
    {
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.SingleMap)
        {
            return;
        }
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.Battle)
        {
            return;
        }
        GameSceneManager.Instance.GoToScene(SceneType.Mountain);
        List<SingleDanFarmData> fanrmList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.BaguaLu);
        if (fanrmList.Count <= 0)
        {
            PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.您还没有八卦炉请先建造一个));
            return;
        }
        MountainPanel mountainPanel = GameObject.Find("MountainPanel").GetComponent<MountainPanel>();

        SingleFarmView farmView = mountainPanel.FindFarmViewByOnlyId(fanrmList[0].OnlyId);

        mountainPanel.scrollViewNevigation.NevigateImmediately(farmView.GetComponent<RectTransform>());
        farmView.OnClickedFarm();
    }

    /// <summary>
    /// 跳转炼丹房
    /// </summary>
    public void JumpToLianDanFang()
    {
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.SingleMap)
        {
            return;
        }
        if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.Battle)
        {
            return;
        }
        GameSceneManager.Instance.GoToScene(SceneType.Mountain);
        List<SingleDanFarmData> fanrmList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.LianDanLu);
        if (fanrmList.Count <= 0)
        {
            PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.您还没有炼丹炉请先建造一个));
            return;
        }
        MountainPanel mountainPanel = GameObject.Find("MountainPanel").GetComponent<MountainPanel>();

        SingleFarmView farmView = mountainPanel.FindFarmViewByOnlyId(fanrmList[0].OnlyId);

        mountainPanel.scrollViewNevigation.NevigateImmediately(farmView.GetComponent<RectTransform>());
        farmView.OnClickedFarm();

    }

    /// <summary>
    /// 跳转到切换血脉
    /// </summary>
    public void JumpToChangeYuanSu()
    {
        TrainPanel trainPanel = PanelManager.Instance.OpenPanel<TrainPanel>(PanelManager.Instance.trans_layer2);
        XueMaiPanel xueMaiPanel = PanelManager.Instance.OpenPanel<XueMaiPanel>(PanelManager.Instance.trans_layer2,RoleManager.Instance._CurGameInfo.playerPeople);
        xueMaiPanel.btn_change.onClick.Invoke();

    }
}
