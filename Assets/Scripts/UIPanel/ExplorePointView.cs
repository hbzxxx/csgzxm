using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;
using DG.Tweening;

public class ExplorePointView : SingleViewBase
{
    public SingleExploreData singleExploreData;
    public Button btn;
    public SingleMapEventData eventData;
    public ExploreMapSetting mapSetting;
    public MapEventSetting mapEventSetting;

    public ExplorePanel parentPanel;

    public Text txt_name;

    public Image img;
    public override void Init(params object[] args)
    {
        base.Init(args);
        eventData = args[0] as SingleMapEventData;
        parentPanel = args[1] as ExplorePanel;

        singleExploreData = parentPanel.exploreData;
        mapSetting = DataTable.FindExploreMapSetting(singleExploreData.SettingId);

        mapEventSetting = DataTable.FindMapEventSetting(eventData.SettingId);
        mapSetting = DataTable.FindExploreMapSetting(singleExploreData.SettingId);
        addBtnListener(btn, () =>
        {
            parentPanel.OnClickedSingleExplorePoint(this);
        });
    
        RegisterEvent(TheEventType.OnTeamExploreArrived, OnArrived);
        RegisterEvent(TheEventType.RevealWenHao, OnReceiveRevealWenHao);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        Vector2Int logicPos= CommonUtil.GetLogicPosByLogicIndex(eventData.PosIndex, mapSetting.LieNum.ToInt32());

        eventData.LogicPos.Clear();
        eventData.LogicPos.Add(logicPos.x);
        eventData.LogicPos.Add(logicPos.y);

        transform.localPosition = new Vector2(eventData.Pos[0], eventData.Pos[1]);
        if (eventData.WenHao)
        {
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mapEventWenHao);
            txt_name.SetText("未知");
        }
        else
        {
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mapFolderPath + mapEventSetting.UiName);
            txt_name.SetText(mapEventSetting.Name);
        }

        img.SetNativeSize();

        if (eventData.IsHide)
            Hide();
        else
        {
            btn.enabled = true;
          
        }
    
        gameObject.name = eventData.OnlyId.ToString();
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    void Hide()
    {
        btn.enabled = false;
        img.DOKill();
        img.gameObject.SetActive(false);
    }

    public void Reveal()
    {
        if (eventData.IsHide)
        {
            btn.enabled = true;
            eventData.IsHide = false;
            img.gameObject.SetActive(true);
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
            img.DOFade(1, 1);
        }

    }

    /// <summary>
    /// 揭露问号
    /// </summary>
    public void OnArrived(object[] args)
    {
        SingleExploreData data = args[0] as SingleExploreData;
        if(data.ExploreTeamData.ExploreId==parentPanel.exploreData.SettingId
            &&data.ExploreTeamData.LogicPosIndex== eventData.PosIndex)
        {
            RevealWenHao();
            btn.onClick.Invoke();
        }
    }

    public void OnReceiveRevealWenHao(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        if (onlyId == eventData.OnlyId)
        {
            RevealWenHao();
        }
    }

    /// <summary>
    /// 揭露问号
    /// </summary>
    public void RevealWenHao()
    {
        if (eventData.IsHide)
        {
            btn.enabled = true;
            eventData.IsHide = false;
            img.gameObject.SetActive(true);

        }
        //问号则有个变化的过程
        if (eventData.WenHao)
        {
            eventData.WenHao = false;
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mapFolderPath + mapEventSetting.UiName);
            txt_name.SetText(mapEventSetting.Name);
            img.DOFade(1, 1);
        }
   
    }

    //显示细节 
    public void ShowDetail()
    {
 

    }

    /// <summary>
    /// 选中
    /// </summary>
    public void OnChoose(bool choose)
    {
        if (choose)
        {

            //else
            //{
            EventCenter.Broadcast(TheEventType.ShowExploreDetail, singleExploreData, eventData,true);
                //ShowDetail();
            //}
        }
        else
        {
            EventCenter.Broadcast(TheEventType.ShowExploreDetail, singleExploreData, eventData, false);

            //trans_detail.gameObject.SetActive(false);

        }

    }

 
}
