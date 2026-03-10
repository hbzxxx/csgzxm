using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class MatchChoosePanel : PanelBase
{

    public MatchSetting curChoosedMatchSetting;
    public Text txt_matchName;
    public Text txt_level;
    public Text txt_championNum;
    public Text txt_record;

    public Button btn_left;
    public Button btn_right;

    public Button btn_enroll;//报名

    List<SingleMatchData> availableMatchList = new List<SingleMatchData>();//可报名的比赛
    SingleMatchData curChoosedMatchData = null;
    public Transform trans_haveMatch;//有比赛
    public Transform trans_noMatch;//无比赛

    public List<SingleEquipView> viewList = new List<SingleEquipView>();//装备选择
    public Transform trans_EquipGrid;
    //public EquipProtoData curChoosedEquip;//当前选的装备

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_left, () =>
        {
            OnClickNext(true);
        });
        addBtnListener(btn_right, () =>
        {
            OnClickNext(false);
        });

        addBtnListener(btn_enroll, () =>
        {
         

     
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
     
            trans_haveMatch.gameObject.SetActive(false);
            trans_noMatch.gameObject.SetActive(true);
        
    }

    /// <summary>
    /// 选择了装备
    /// </summary>
    /// <param name="view"></param>
    public void OnChoosedSingleEquipData(SingleEquipView view)
    {
        //for (int i = 0; i < viewList.Count; i++)
        //{
        //    SingleEquipView theView = viewList[i];
        //    if (view == theView)
        //    {
        //        theView.OnChoose(true);
        //        RoleManager.Instance._CurGameInfo.playerPeople.CurEquip = theView.data;
        //    }
        //    else
        //    {
        //        theView.OnChoose(false);
        //    }
        //}
    }


    void OnShowMatch(SingleMatchData data)
    {
        int settingId = data.SettingId;
        curChoosedMatchData = data;
        curChoosedMatchSetting = DataTable.table.TbMatch.Get(settingId.ToString());
        txt_matchName.SetText(curChoosedMatchSetting.Name);
        txt_level.SetText(curChoosedMatchSetting.Level);
        string[] awardArr = curChoosedMatchSetting.ChampionAward.Split('|');
        txt_championNum.SetText(awardArr[awardArr.Length-1]);
        txt_record.SetText(data.ParticipateNum + "战" + data.ChampionNum + "胜");
    }

    void OnClickNext(bool left)
    {
        int index = availableMatchList.IndexOf(curChoosedMatchData);
        if (left)
            index--;
        else
            index++;
        if (index < 0)
            index = availableMatchList.Count - 1;
        if (index > availableMatchList.Count - 1)
            index = 0;

        OnShowMatch(availableMatchList[index]);
    }

    public override void Clear()
    {
        base.Clear();
        availableMatchList.Clear();
        PanelManager.Instance.CloseAllSingle(trans_EquipGrid);
        viewList.Clear();
    }

    public override void OnClose()
    {
        base.OnClose();

    }
}
