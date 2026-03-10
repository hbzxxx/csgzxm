using DG.Tweening;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
/// <summary>
/// 7日签到
/// </summary>
public class SingleSevenDayQianDaoView : SingleViewBase
{
    public Button btn_qian;
    public GameObject obj_haveQian;
    public int day;//第几天的签到

    public Text txt_day;//第几天
    public Transform trans_awardGrid;//奖品
    public QianDaoSetting qianDaoSetting;
    public override void Init(params object[] args)
    {
        base.Init(args);
        day = (int)args[0];
        qianDaoSetting = DataTable.FindQianDaoSettingByTypeAndDay((int)QianDaoType.SevenDay, day);

        addBtnListener(btn_qian, () =>
        {
            if (RoleManager.Instance._CurGameInfo.QianDaoData.CanSevenDayQianDaoIndex == day
            && RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex + 1 == RoleManager.Instance._CurGameInfo.QianDaoData.CanSevenDayQianDaoIndex)
            {
                btn_qian.gameObject.SetActive(false);
                obj_haveQian.transform.DOKill();
                obj_haveQian.gameObject.SetActive(true);

                obj_haveQian.transform.localScale = new Vector3(3, 3, 3);
                obj_haveQian.transform.DOScale(1, 1f).OnComplete(() =>
                {
                    List<List<int>> award = CommonUtil.SplitCfg(qianDaoSetting.Award);
                    List<int> idList = new List<int>();
                    List<ulong> numList = new List<ulong>();

                    for(int i = 0; i < award.Count; i++)
                    {
                        List<int> single = award[i];
                        if (DataTable.FindItemSetting(single[0]) == null) continue;//物品不存在
                        idList.Add(single[0]);
                        numList.Add((ulong)single[1]);
                    }
                    ItemManager.Instance.GetItemWithAwardPanel(idList, numList);
                    RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex++;
                    RefreshShow();
                    EventCenter.Broadcast(TheEventType.RefreshRedPointShow);

                });
            }

        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();


    }

    void RefreshShow()
    {
        if (RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex >= day)
        {
            btn_qian.gameObject.SetActive(false);
            obj_haveQian.gameObject.SetActive(true);
        }
        else if (RoleManager.Instance._CurGameInfo.QianDaoData.CanSevenDayQianDaoIndex == day
            && RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex + 1 == RoleManager.Instance._CurGameInfo.QianDaoData.CanSevenDayQianDaoIndex)
        {
            btn_qian.gameObject.SetActive(true);
            obj_haveQian.gameObject.SetActive(false);
        }
        else
        {
            btn_qian.gameObject.SetActive(false);
            obj_haveQian.gameObject.SetActive(false);
        }
        txt_day.SetText("第" + $"<size=48>{day}</size>" + "天");

        PanelManager.Instance.CloseAllSingle(trans_awardGrid);
        List<List<int>> award = CommonUtil.SplitCfg(qianDaoSetting.Award);

        //for (int i=0;i< award.Count; i++)
        //{
        //    List<int> single = award[i];
        //    ItemData item = new ItemData();
        //    item.settingId = single[0];
        //    item.count = (ulong)single[1];
        //    if(DataTable.FindItemSetting(item.settingId) !=null)
        //        PanelManager.Instance.OpenSingle<WithCountItemView>(trans_awardGrid, item);
        //}

        List<int> single = award[0];
        ItemData item = new ItemData();
        item.settingId = single[0];
        item.count = (ulong)single[1];
        if (DataTable.FindItemSetting(item.settingId) != null)
            PanelManager.Instance.OpenSingle<WithCountItemView>(trans_awardGrid, item);
    }

}
