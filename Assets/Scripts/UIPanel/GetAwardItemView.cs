using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAwardItemView : WithCountItemView
{
    public SkeletonGraphic ske;
    public float fadeDelay = 0.06f;
    float fadeDelayTimer = 0;
    public bool startFade = false;
    public float fadeDuration = 0.1f;
    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        //img_icon.DOKill();
        //img_icon.DOFade(0, 0);
        ske.gameObject.SetActive(false);
        img_icon.gameObject.SetActive(false);
        txt_count.gameObject.SetActive(false);
    }

    /// <summary>
    /// 开始动作
    /// </summary>
    public void StartAnim()
    {
        //ske.gameObject.SetActive(true);
        img_icon.gameObject.SetActive(true);

        //if (setting == null)
        //{
        //    Debug.LogError($"没有{itemData.settingId}该物品请更新物品");
        //    return;
        //}
        //string animName = ConstantVal.awardItemAnimName((Rarity)setting.Rarity.ToInt32());
        //img_icon.DOKill();
        startFade = true;
        txt_count.gameObject.SetActive(true);
        //ske.AnimationState.SetAnimation(0, animName, false);
        //ske.AnimationState.AddAnimation(0, animName + "2", false, 0);
    }

    private void Update()
    {
        //if (startFade)
        //{
        //    fadeDelayTimer += Time.deltaTime;
        //    if (fadeDelayTimer >= fadeDelay)
        //    {
        //        img_icon.DOFade(1, fadeDuration).OnComplete(() =>
        //        {
        //            txt_count.gameObject.SetActive(true);
        //        });
        //    }
        //}
    }
    public override void RefreshShow()
    {
        base.RefreshShow();
        if (itemData.equipProtoData == null)
            txt_count.SetText(UIUtil.ShowBigCount((long)(ulong)itemData.count));
        else
            txt_count.SetText("Lv." + itemData.equipProtoData.curLevel);

        addBtnListener(btn, () =>
        {
            PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData,true);
        });
    }

    public override void Clear()
    {
        base.Clear();
        fadeDelayTimer = 0;
        startFade = false;
    }
}
