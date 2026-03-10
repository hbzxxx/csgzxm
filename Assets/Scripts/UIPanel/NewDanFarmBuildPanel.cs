using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewDanFarmBuildPanel : PanelBase
{
    public SingleDanFarmData curChoosedDanFarmData;//当前选择的丹田

    public Transform trans_newDanFarmGrid;//新丹田有哪些
    public List<NewDanFarmSingleView> newDanFarmSingleViewList = new List<NewDanFarmSingleView>();//哪些新丹田
    public NewDanFarmSingleView choosedNewDanFarmSingleView;//选择了新丹田
    public Button btn_buildNewDanFarm;//建造新丹田
    public int curChoosedNewDanFarmBuildIndex;//新丹田在哪个位置建
    public ScrollViewNevigation scrollViewNevigation;

    public Text txt_curBuildingNum;//当前建筑数量
    public Transform trans_canUnlock;//能解锁
    public Transform trans_unlockConsumeParent;//解锁消耗
    public Button btn_unlock;//解锁
    public Transform trans_cannotUnlock;//宗门等级未达到不能解锁

    public override void Init(params object[] args)
    {
        base.Init(args);
 
        addBtnListener(btn_buildNewDanFarm, () =>
        {
            if (RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count >= RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum)
            {
                PanelManager.Instance.OpenFloatWindow("当前建造数量已达上限");
            }
            else if (ZongMenManager.Instance.GetFarmNumLimit(choosedNewDanFarmSingleView.danFarmSetting.Id.ToInt32()) <=LianDanManager.Instance.FindMyFarmNum(choosedNewDanFarmSingleView.danFarmSetting.Id.ToInt32()))
            {
              
                PanelManager.Instance.OpenFloatWindow( LanguageUtil.GetLanguageText((int)LanguageIdType.该建筑数量已达上限请提升宗门等级));
                return;
            }

            else
            {
                //LianDanManager.Instance.OnBuildNewDanFarm(choosedNewDanFarmSingleView.danFarmSetting.id.ToInt32());
                //PanelManager.Instance.ClosePanel(this);
                PanelManager.Instance.ClosePanel(this);
                BuildingManager.Instance.EnterBuildingBuildingMove(choosedNewDanFarmSingleView.danFarmSetting.Id.ToInt32());
            }
         
        });

        addBtnListener(btn_unlock, () =>
        {
            ZongMenManager.Instance.UnlockDanFarmPos();
            PanelManager.Instance.CloseTaskGuidePanel();
        });

        RegisterEvent(TheEventType.UnlockDanFarm, ShowUnlock);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        choosedNewDanFarmSingleView = null;
        ClearCertainParentAllSingle<NewDanFarmSingleView>(trans_newDanFarmGrid);
        newDanFarmSingleViewList.Clear();
        List<int> farmList = ZongMenManager.Instance.FindAllUnlockedFarmIdList();
        for (int i = 0; i < farmList.Count; i++)
        {
            int id = farmList[i];
            DanFarmSetting setting = DataTable.FindDanFarmSetting(id);
            if (setting == null)
            {
                Debug.LogError("找不到建筑配置 id=" + id + "，可能是存档数据与配置表不匹配");
                continue;
            }
            NewDanFarmSingleView view = AddSingle<NewDanFarmSingleView>(trans_newDanFarmGrid, setting, this);
            newDanFarmSingleViewList.Add(view);
        }
        newDanFarmSingleViewList[0].btn.onClick.Invoke();
        TaskManager.Instance.CloseTaskGuide(TaskType.DanFarmNum);
        TaskGuide();
        ShowUnlock();
    }

    /// <summary>
    /// 显示解锁情况
    /// </summary>
    void ShowUnlock()
    {
        txt_curBuildingNum.SetText("建筑数量：" + "(" + RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count 
            +"/"+ RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum+ ")");
        //能解锁
        if (RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmNumLimit > RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum)
        {
            trans_cannotUnlock.gameObject.SetActive(false);
            trans_canUnlock.gameObject.SetActive(true);
            ClearCertainParentAllSingle<SingleViewBase>(trans_unlockConsumeParent);
            AddSingle<SingleConsumeView>(trans_unlockConsumeParent, (int)ItemIdType.LingShi, RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockFarmNeedLingShiNum, ConsumeType.Item);
 
        }else
        {
            trans_cannotUnlock.gameObject.SetActive(true);
            trans_canUnlock.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// 任务引导
    /// </summary>
    void TaskGuide()
    {
        if (TaskManager.Instance.guide_buildFarm==true&& TaskManager.Instance.guide_buildFarmId != 0)
        {
            for(int i = 0; i < newDanFarmSingleViewList.Count; i++)
            {
                NewDanFarmSingleView view = newDanFarmSingleViewList[i];
                if(view.danFarmSetting.Id.ToInt32() == TaskManager.Instance.guide_buildFarmId)
                {
                    PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, view.gameObject);
                    return;
                }
            }
        }
        else if (TaskManager.Instance.guide_unlockPos)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_unlock.gameObject);
        }

    }

    /// <summary>
    /// 选择单个新丹田
    /// </summary>
    public void OnChoosedSingleNewDanFarm(NewDanFarmSingleView view)
    {
        choosedNewDanFarmSingleView = view;
        for (int i = 0; i < newDanFarmSingleViewList.Count; i++)
        {
            if (view == newDanFarmSingleViewList[i])
            {
                newDanFarmSingleViewList[i].OnChoosed(true);

            }
            else
            {
                newDanFarmSingleViewList[i].OnChoosed(false);
            }
        }
        if (TaskManager.Instance.guide_buildFarm == true && TaskManager.Instance.guide_buildFarmId != 0)
        {

            if (view.danFarmSetting.Id.ToInt32() == TaskManager.Instance.guide_buildFarmId)
            {
                PanelManager.Instance.ShowTaskGuidePanel(btn_buildNewDanFarm.gameObject);
            }
            else
            {
                TaskGuide();
            }
        }
     
    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseTaskGuidePanel();
        TaskManager.Instance.guide_buildFarm = false;
    }
}
