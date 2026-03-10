using DG.Tweening;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 出城地图选择面板
/// 点击下边栏出城按钮弹出，包含三个小地图按钮
/// </summary>
public class OutCityMapView : PanelBase
{
    public Button btn_worldMap;//地图
    public Button btn_wanFaDaTing;//历练
    public Button btn_third;//副本按钮
    public Button btn_close;
    
    private Vector3 map1OriginalScale;
    private Vector3 map2OriginalScale;
    private Vector3 map3OriginalScale;
    
    public override void Init(params object[] args)
    {
        base.Init(args);

        map1OriginalScale = Vector3.one;
        map2OriginalScale = Vector3.one;
        map3OriginalScale = Vector3.one;

        addBtnListener(btn_worldMap, OnClickWorldMap);
        addBtnListener(btn_wanFaDaTing, OnClickWanFaDaTing);
        addBtnListener(btn_third, OnClickThird);
        if (btn_close != null)
        {
            addBtnListener(btn_close, OnClickClose);
        }
    }
    
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        PlayOpenAnim();
    }
    
    void PlayOpenAnim()
    {
        if (btn_worldMap != null)
        {
            btn_worldMap.transform.localScale = Vector3.zero;
            btn_worldMap.transform.DOScale(map1OriginalScale, 0.3f).SetEase(Ease.OutBack).SetDelay(0f);
        }
        if (btn_wanFaDaTing != null)
        {
            btn_wanFaDaTing.transform.localScale = Vector3.zero;
            btn_wanFaDaTing.transform.DOScale(map2OriginalScale, 0.3f).SetEase(Ease.OutBack).SetDelay(0.1f);
        }
        if (btn_third != null)
        {
            btn_third.transform.localScale = Vector3.zero;
            btn_third.transform.DOScale(map3OriginalScale, 0.3f).SetEase(Ease.OutBack).SetDelay(0.2f);
        }
    }
    
    void OnClickWorldMap()
    {
        GameSceneManager.Instance.GoToScene(SceneType.WorldMap);
        CloseView();
    }
    
    void OnClickWanFaDaTing()
    {
        PanelManager.Instance.OpenPanel<WanfaDatingPanel>(PanelManager.Instance.trans_layer2);
        CloseView();
    }
    
    void OnClickThird()
    {
        //GameSceneManager.Instance.GoToScene(SceneType.OutsideMap);
        PanelManager.Instance.OpenPanel<CopyPanel>(PanelManager.Instance.trans_layer2);
        CloseView();
    }
    
    void OnClickClose()
    {
        CloseView();
    }
    
    void CloseView()
    {
        PanelManager.Instance.ClosePanel(this);
    }
    
    public override void Clear()
    {
        base.Clear();
        KillAllTweens();
        
        if (btn_worldMap != null) btn_worldMap.transform.localScale = map1OriginalScale;
        if (btn_wanFaDaTing != null) btn_wanFaDaTing.transform.localScale = map2OriginalScale;
        if (btn_third != null) btn_third.transform.localScale = map3OriginalScale;
    }
    
    void KillAllTweens()
    {
        if (btn_worldMap != null) DOTween.Kill(btn_worldMap.transform);
        if (btn_wanFaDaTing != null) DOTween.Kill(btn_wanFaDaTing.transform);
        if (btn_third != null) DOTween.Kill(btn_third.transform);
    }
}
