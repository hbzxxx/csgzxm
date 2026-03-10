using DG.Tweening;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
/// <summary>
/// 30天签到
/// </summary>
public class SingleThirtyDayQianDaoView : SingleViewBase
{
    int day;
    public Button btn_qian;
    public GameObject obj_haveQian;
    public Transform trans_awardGrid;
    public Text txt_day;
    QianDaoSetting qianDaoSetting;
    ThirtyDayQianDaoPanel parentPanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        day = (int)args[0];
        parentPanel = args[1] as ThirtyDayQianDaoPanel;

        qianDaoSetting = DataTable.FindQianDaoSettingByTypeAndDay((int)QianDaoType.ThirtyDay, day);

        addBtnListener(btn_qian, () =>
        {
            if (RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex == day
            && RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex + 1 == RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex)
            {
                obj_haveQian.transform.DOKill();
                obj_haveQian.gameObject.SetActive(true);

                obj_haveQian.transform.localScale = new Vector3(3, 3, 3);
                obj_haveQian.transform.DOScale(1, 1f).OnComplete(() =>
                {
                    List<List<int>> award = CommonUtil.SplitCfg(qianDaoSetting.Award);
                    List<int> idList = new List<int>();
                    List<ulong> numList = new List<ulong>();

                    for (int i = 0; i < award.Count; i++)
                    {
                        List<int> single = award[i];
                        idList.Add(single[0]);
                        numList.Add((ulong)single[1]);
                    }
                    ItemManager.Instance.GetItemWithAwardPanel(idList, numList);
                    RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex++;
                    RefreshShow();
                    parentPanel.SuccessQian();
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
        if (RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex >= day)
        {
            btn_qian.gameObject.SetActive(false);
            obj_haveQian.gameObject.SetActive(true);
        }
        else if (RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex == day
            && RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex + 1 == RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex)
        {
            btn_qian.gameObject.SetActive(true);
            obj_haveQian.gameObject.SetActive(false);
        }
        else
        {
            btn_qian.gameObject.SetActive(false);
            obj_haveQian.gameObject.SetActive(false);
        }
        txt_day.SetText("第" + day + "天");
        PanelManager.Instance.CloseAllSingle(trans_awardGrid);
        List<List<int>> award = CommonUtil.SplitCfg(qianDaoSetting.Award);

        for (int i = 0; i < award.Count; i++)
        {
            List<int> single = award[i];
            ItemData item = new ItemData();
            item.settingId = single[0];
            item.count = (ulong)single[1];
            PanelManager.Instance.OpenSingle<WithCountItemView>(trans_awardGrid, item);
        }
    }
}
