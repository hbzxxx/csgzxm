using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ExploreTeamStatusView : SingleViewBase
{
    public Image bar;
    public Text txt_remainDay;
    public SingleExploreData exploreData;
    public Transform trans_waiting;
    public Transform trans_moving;
    public Button btn;

    public override void Init(params object[] args)
    {
        base.Init(args);
        exploreData = args[0] as SingleExploreData;
        RegisterEvent(TheEventType.OnTeamExploreMoving, OnExploreMoving);
        RegisterEvent(TheEventType.OnTeamExploreArrived, OnExploreArrived);

        addBtnListener(btn, () =>
        {
            if (MapManager.Instance.curChoosedExploreId == exploreData.SettingId
            && RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.MiJingExplore)
                return;
            MapManager.Instance.curChoosedExploreId = exploreData.SettingId;
            GameSceneManager.Instance.GoToScene(SceneType.MiJingExplore, false);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
    }

    void RefreshShow()
    {
        if (exploreData.ExploreTeamData.Status == (int)ExploreTeamStatus.Idling)
        {
            trans_waiting.gameObject.SetActive(true);
            trans_moving.gameObject.SetActive(false);
        }
        else
        {
            trans_waiting.gameObject.SetActive(false);
            trans_moving.gameObject.SetActive(true);
            float rate = (exploreData.ExploreTeamData.TotalDay - exploreData.ExploreTeamData.RemainDay) / (float)exploreData.ExploreTeamData.TotalDay;
            bar.fillAmount = rate;
            txt_remainDay.SetText(exploreData.ExploreTeamData.RemainDay / 7 + "周");
        }
    
    }

    /// <summary>
    /// 探险移动中
    /// </summary>
    /// <param name="args"></param>
    void OnExploreMoving(object[] args)
    {
        SingleExploreData theData = args[0] as SingleExploreData;
        if (theData.SettingId == exploreData.SettingId)
        {
            exploreData = theData;
            RefreshShow();

        }
    }

    /// <summary>
    /// 探险到达
    /// </summary>
    /// <param name="args"></param>
    void OnExploreArrived(object[] args)
    {
        SingleExploreData theData = args[0] as SingleExploreData;
        if (theData.SettingId == exploreData.SettingId)
        {
            exploreData = theData;
            RefreshShow();

        }
    }
}
