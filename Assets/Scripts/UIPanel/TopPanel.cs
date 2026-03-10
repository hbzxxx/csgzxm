using DG.Tweening;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
 
public class TopPanel : PanelBase
{
   // public Transform trans_propertyGrid;
   // public Text txt_time;

    public Text txt_year;
    public Text txt_moon;
    public Text txt_week;

    MatchForecastView matchForecastView;
    public Transform trans_matchForecastParent;

    public Image img_tianJing;//天晶
    public Text txt_tianJing;

    public Text txt_lingShiNum;//灵石数量
    public Text txt_tili;//体力
    public Image img_tilibar;//体力条
    public Text txt_tiliCd;//体力恢复cd
    public Button btn_addTi;//加体力按钮

    public Image img_hpBar;//血条
    public Image img_mpBar;//蓝条
    public Text txt_hpNum;//血量
    public Text txt_zhanDouLi;//战斗力
    public Button btn_headIcon;//头像
    public Text txt_name;//名字
    public Portrait porTrait_head;//头像
    public Image img_touXiangKuangDi;//头像框的底
    public Image img_touXiangKuang;//头像框
    public Image img_head;//头像

    public float secondTimer = 0;

    public Transform trans_role;
    public Transform trans_time;

    public GameObject obj_redPoint;

    public Button btn_tianJingAdd;//加天晶
    public Button btn_lingShiAdd;//加灵石
 
    public override void Init(params object[] args)
    {
        base.Init(args);
        EventCenter.Register(TheEventType.PropertyAdd, RefreshPro);
        EventCenter.Register(TheEventType.PropertyDecrease, RefreshPro);
        EventCenter.Register(TheEventType.WeekPlus, ShowTime);
        EventCenter.Register(TheEventType.MoonPlus, ShowTime);
        EventCenter.Register(TheEventType.YearPlus, ShowTime);

        EventCenter.Register(TheEventType.OnEnrolledMatch, ShowMatchForecast);
        EventCenter.Register(TheEventType.GetItem, ShowLingShi);
        EventCenter.Register(TheEventType.LoseItem, ShowLingShi);
        RegisterEvent(TheEventType.AddBattleProperty, RefreshPro);
        RegisterEvent(TheEventType.DeBattleProperty, RefreshPro);
        RegisterEvent(TheEventType.SetFaceAndName, OnSetFaceAndName);
        RegisterEvent(TheEventType.RealitySecondPassed, OnSecondPassed);
        RegisterEvent(TheEventType.FinishCloudMove, ShowBtns);
        RegisterEvent(TheEventType.ChangeZhangMenName, ShowName);
        RegisterEvent(TheEventType.RefreshZhangMenRedPoint, RefreshRedPoint);
        RegisterEvent(TheEventType.EnterBuildingMode, EnterBuildingMode);
        RegisterEvent(TheEventType.EnterOnlyMoveBuildingMode, EnterBuildingMode);
        RegisterEvent(TheEventType.ChangeTouXiang, OnSetFaceAndName);
        addBtnListener(btn_headIcon, () =>
        {
            PanelManager.Instance.OpenPanel<TrainPanel>(PanelManager.Instance.trans_layer2);

           // PanelManager.Instance.OpenPanel<PlayerPeoplePanel>(PanelManager.Instance.trans_layer2, 0);
        });

        addBtnListener(btn_addTi, () =>
        {
            PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);
        });
        addBtnListener(btn_tianJingAdd, () =>
        {
            PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2,ShopTag.ChongZhi );
        });
        addBtnListener(btn_lingShiAdd, () =>
        {
            PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2, ShopTag.LiBao);
        });
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
       // ShowProperty();
        RefreshLingShiShow(false);
        RefreshTianJingShow();
        ShowTime();
        RefreshTiliShow(false);
        RefreshHPShow(false);
        RefreshMPShow(false);
        RefreshZhanDouLiShow();
 
        OnSetFaceAndName();
        ShowBtns();
        RefreshRedPoint();
    }

    /// <summary>
    /// 红点刷新
    /// </summary>
    void RefreshRedPoint()
    {
        //RedPointManager.Instance.SetRedPointUI(obj_redPoint, RedPointType.MainPanel_Btn_ZhangMen, 0);
    }

    void ShowBtns()
    {
        trans_role.gameObject.SetActive(true);
        trans_time.gameObject.SetActive(true);
    }
    void ShowName()
    {
        txt_name.SetText(RoleManager.Instance._CurGameInfo.playerPeople.name);

    }

    void EnterBuildingMode()
    {
        PanelManager.Instance.ClosePanel(this);
    }
    /// <summary>
    /// 捏脸成功
    /// </summary>
    void OnSetFaceAndName()
    {
        ShowName();
        if (!string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.playerPeople.name))
        {
            //是否设置了头像框
            string str = RoleManager.Instance._CurGameInfo.TouXiang;
            //设置了头像
            if (!string.IsNullOrWhiteSpace(str))
            {
                string[] strArr = str.Split('|');
                string touXiang = strArr[0];
                string touXiangKuang = strArr[1];
                //设置了头像
                //if (!string.IsNullOrWhiteSpace(touXiang))
                //{
                //    img_head.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath +RoleManager.Instance.TouXiangPath(touXiang.ToInt32()));
                //    img_head.gameObject.SetActive(true);
                //    porTrait_head.gameObject.SetActive(false);
                //}
                ////没设置头像
                //else
                //{
                //    porTrait_head.Refresh(RoleManager.Instance._CurGameInfo.playerPeople);
                //    porTrait_head.gameObject.SetActive(true);
                //    img_head.gameObject.SetActive(false);
                //}
                porTrait_head.gameObject.SetActive(false);
                StudentManager.Instance.SetTouxiang(img_head, RoleManager.Instance._CurGameInfo.playerPeople);
                //设置了头像框
                if (!string.IsNullOrWhiteSpace(touXiangKuang))
                {
                    ItemSetting kuangSetting = DataTable.FindItemSetting(touXiangKuang.ToInt32());
                    string[] kuangArr = kuangSetting.Param.Split('|');
                    img_touXiangKuangDi.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + kuangArr[0]);
                    img_touXiangKuang.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + kuangArr[1]);
                }
                //没设置头像框
                else
                {
                    img_touXiangKuangDi.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuangDi);

                    img_touXiangKuang.sprite= ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuang);
                }

            }
            else
            {
                //没设置头像也没设置头像框
                //porTrait_head.Refresh(RoleManager.Instance._CurGameInfo.playerPeople);
                //porTrait_head.gameObject.SetActive(true);
                //img_head.gameObject.SetActive(false);
                porTrait_head.gameObject.SetActive(false);
                StudentManager.Instance.SetTouxiang(img_head, RoleManager.Instance._CurGameInfo.playerPeople);

                img_touXiangKuangDi.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuangDi);

                img_touXiangKuang.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuang);
            }
         
        }
        else
        {
            porTrait_head.gameObject.SetActive(false);
            StudentManager.Instance.SetTouxiang(img_head, RoleManager.Instance._CurGameInfo.playerPeople);
            img_touXiangKuangDi.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuangDi);
            img_touXiangKuang.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuang);
        }
    }

    void ShowTime()
    {
        txt_year.SetText(RoleManager.Instance._CurGameInfo.timeData.Year.ToString());
        txt_moon.SetText(RoleManager.Instance._CurGameInfo.timeData.Month.ToString());
        txt_week.SetText(RoleManager.Instance._CurGameInfo.timeData.Week.ToString());
        //更新下次比赛
        if (matchForecastView != null)
        {
            matchForecastView.RefreshShow();
            //trans_matchForecastParent.GetChild(0).GetComponent<MatchForecastView>().
        }
    }

    void RefreshZhanDouLiShow()
    {
        long zhanDouLi = RoleManager.Instance.CalcZhanDouLi(RoleManager.Instance._CurGameInfo.playerPeople);
        txt_zhanDouLi.SetText("战力：" + zhanDouLi);
    }
    /// <summary>
    /// 刷新血量
    /// </summary>
    void RefreshHPShow(bool tween)
    {
        SinglePropertyData singlePropertyData = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, RoleManager.Instance._CurGameInfo.playerPeople);
        int targetNum = singlePropertyData.num;
        if (!tween)
        {
            txt_hpNum.SetText(targetNum + "/"+ singlePropertyData.limit);
            img_hpBar.fillAmount = targetNum / (float)singlePropertyData.limit;
        }
        else
        {
            string[] arr = txt_hpNum.text.Split('/');
            int beforeNum = arr[0].ToInt32();
            DOTween.To(() => beforeNum, x =>
            {
                beforeNum = x;
                txt_hpNum.SetText(x+"/" + singlePropertyData.limit);
            }, targetNum, 1);
            img_hpBar.DOFillAmount(targetNum / (float)singlePropertyData.limit, 1f);
        }
    }
    /// <summary>
    /// 刷新蓝量
    /// </summary>
    void RefreshMPShow(bool tween)
    {
        SinglePropertyData singlePropertyData = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, RoleManager.Instance._CurGameInfo.playerPeople);
        int targetNum = singlePropertyData.num;
        if (!tween)
        {
            img_mpBar.fillAmount = targetNum / (float)singlePropertyData.limit;
        }
        else
        {

            img_mpBar.DOFillAmount(targetNum / (float)singlePropertyData.limit, 1f);
        }
    }

    private void Update()
    {
    
    }
    /// <summary>
    /// 每秒钟过去
    /// </summary>
    void OnSecondPassed()
    {
         
        if (RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime <= 0
            || (RoleManager.Instance.FindMyProperty((int)PropertyIdType.Tili).num >= RoleManager.Instance._CurGameInfo.allZongMenData.TiliLimit))
        {
            txt_tiliCd.gameObject.SetActive(false);
            return;
        }
        else
        {
            txt_tiliCd.gameObject.SetActive(true);
            long timeNow = CGameTime.Instance.GetTimeStamp();
            long lastReviveTime = RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime;
            long remainSecond = (ConstantVal.tiliReviveMinute * 60 - (timeNow - lastReviveTime));
            long remainMin = remainSecond / 60;
            long remainSec = remainSecond % 60;
            txt_tiliCd.SetText("下次恢复：" + remainMin + "分" + remainSec + "秒");
            //if (remainSecond <= 0)
            //{
              
            //        remainSecond = ConstantVal.tiliReviveMinute * 60- (CGameTime.Instance.GetTimeStamp() - RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime);
            //        if (remainSecond <= 0)
            //        {
            //            RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime = x;
            //            RoleManager.Instance.AddProperty(PropertyIdType.Tili, 1);
            //            remainMin = ConstantVal.tiliReviveMinute;
            //            remainSec =0;
            //            txt_tiliCd.SetText("下次恢复：" + remainMin + "分" + remainSec + "秒");

            //        }
             
            //}
        }

    }

    /// <summary>
    /// 刷新体力显示
    /// </summary>
    void RefreshTiliShow(bool tween)
    {
        SinglePropertyData tili = RoleManager.Instance.FindMyProperty((int)PropertyIdType.Tili);
        int targetNum = tili.num;

        txt_tili.SetText(targetNum + "/" + RoleManager.Instance._CurGameInfo.allZongMenData.TiliLimit);
        img_tilibar.fillAmount = targetNum / (float)tili.limit;
    }

    /// <summary>
    /// 刷新天晶显示
    /// </summary>
    void RefreshTianJingShow()
    {
        txt_tianJing.SetText(UIUtil.ShowBigCount((long)ItemManager.Instance.FindItemCount((int)ItemIdType.LingJing)));
    }

    /// <summary>
    /// 刷新灵石显示
    /// </summary>
    void RefreshLingShiShow(bool tween)
    {
        tween = false;
        if (!tween)
        {
            txt_lingShiNum.SetText(UIUtil.ShowBigCount(ItemManager.Instance.FindLingShiCount()));
        }
        else
        {
            int beforeNum = txt_lingShiNum.text.ToInt32();
            int targetNum =(int)ItemManager.Instance.FindLingShiCount();
            DOTween.To(() => beforeNum, x =>
            {
                beforeNum = x;
                txt_lingShiNum.SetText(UIUtil.ShowBigCount(x));
            }, targetNum, 2);
        }
    }


    /// <summary>
    /// 刷新属性显示
    /// </summary>
    void RefreshPro()
    {
        RefreshTiliShow(true);
        //for (int i = 0; i < singlePropertyViewList.Count; i++)
        //{
        //    singlePropertyViewList[i].RefreshProShow();
        //}
        RefreshHPShow(true);
        RefreshMPShow(true);
        RefreshZhanDouLiShow();
    }

    void ShowLingShi(object[] param)
    {
        int theId = (int)param[0];
        if (theId == (int)ItemIdType.LingShi)
        {
            RefreshLingShiShow(true);
        }else if (theId == (int)ItemIdType.LingJing)
        {
            RefreshTianJingShow();
        }

    }

    ///// <summary>
    ///// 显示属性
    ///// </summary>
    //void ShowProperty()
    //{
    //    //PanelManager.Instance.CloseAllSingle(trans_propertyGrid);
    //    //for (int i = 0; i < DataTable._propertyList.Count; i++)
    //    //{
    //    //    PropertySetting setting = DataTable._propertyList[i];
    //    //    if (setting.showInMain == "1")
    //    //    {
    //    //        SinglePropertyView view = PanelManager.Instance.OpenSingle<SinglePropertyView>(trans_propertyGrid, setting.id.ToInt32(), 0, true);
    //    //        singlePropertyViewList.Add(view);
    //    //    }
    //    //}
    //    ////显示物品
    //    //ItemShowView itemShowView = PanelManager.Instance.OpenSingle<ItemShowView>(trans_propertyGrid, ItemManager.Instance.FindItemBySettingId((int)ItemIdType.LingShi));
    //}
    /// <summary>
    /// 显示比赛预告
    /// </summary>
    void ShowMatchForecast()
    {
        //SingleMatchData singleMatchData = RoleManager.Instance._CurGameInfo.MatchData.EnrolledSingleMatchData;
        //MatchSetting setting = DataTable.FindMatchSetting(singleMatchData.SettingId);
        matchForecastView = PanelManager.Instance.OpenSingle<MatchForecastView>(trans_matchForecastParent);
    }

    public override void Clear()
    {
        base.Clear();
        //PanelManager.Instance.CloseAllSingle(trans_propertyGrid);
        PanelManager.Instance.CloseAllSingle(trans_matchForecastParent);


        EventCenter.Remove(TheEventType.PropertyAdd, RefreshPro);
        EventCenter.Remove(TheEventType.PropertyDecrease, RefreshPro);
        EventCenter.Remove(TheEventType.WeekPlus, ShowTime);

        EventCenter.Remove(TheEventType.MoonPlus, ShowTime);
        EventCenter.Remove(TheEventType.YearPlus, ShowTime);
        EventCenter.Remove(TheEventType.OnEnrolledMatch, ShowMatchForecast);

        EventCenter.Remove(TheEventType.GetItem, ShowLingShi);
        EventCenter.Remove(TheEventType.LoseItem, ShowLingShi);
        PanelManager.Instance.CloseAllSingle(trans_matchForecastParent);
        matchForecastView = null;
        
    }

}
