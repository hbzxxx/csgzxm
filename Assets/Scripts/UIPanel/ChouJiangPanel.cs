using DG.Tweening;
using Framework.Data;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using cfg;

public class ChouJiangPanel : PanelBase
{
    public Text txt_tianJingNum;
    public Text txt_tianJiFuNum;

    public Button btn_danChou;
    public Transform trans_idle;

    public Transform trans_danChouItemGrid;

    public Button btn_shiLian;
    public Transform trans_shiLianItemGrid;

    public Transform trans_jinRes;
    public SkeletonGraphic ske_jinRes;

    public Transform trans_ziRes;
    public SkeletonGraphic ske_ziRes;

    public Transform trans_commonRes;
    public SkeletonGraphic ske_commonRes;

    public PostProcessVolume postProcessVolume;

    bool waitBloom = false;
    float waitBloomTimer = 0;
    float waitBloomTime = 2.1f;

    bool startVoice = false;
    float voiceTimer = 0;
    float voiceTime = 1.5f;

    List<ItemData> resList;

    public Button btn_wenHao;

    public Text txt_baoDiDes;

    public Transform trans_gaiLvDes;
    public Text txt_gaiLvDes;
    public Button btn_closeGaiLvDes;

    public Text txt_todayRemainDes;

    public override void Init(params object[] args)
    {
        base.Init(args);

        addBtnListener(btn_danChou, () =>
        {
             ChouJiangManager.Instance.OnChou(RoleManager.Instance._CurGameInfo.ChouJiangData, 1);
        });
        addBtnListener(btn_shiLian, () =>
        {
             ChouJiangManager.Instance.OnChou(RoleManager.Instance._CurGameInfo.ChouJiangData, 10);
        });

        addBtnListener(btn_wenHao, () =>
        {
            string des = "";
            for(int i = 0; i < DataTable.table.TbChouJiang.DataList.Count; i++)
            {
                ChouJiangSetting setting = DataTable.table.TbChouJiang.DataList[i];
                string itemName= DataTable.table.TbItem.GetOrDefault(CommonUtil.SplitCfgOneDepth(setting.Item)[0].ToString())?.Name;
                des += itemName + ":" + setting.Weight + "%"+"\n";
            }
            trans_gaiLvDes.gameObject.SetActive(true);
            txt_gaiLvDes.SetText(des);

            //PanelManager.Instance.OpenPanel<TipsPanel>(PanelManager.Instance.trans_layer2, des,(Vector2)btn_wenHao.transform.position);
        });
        addBtnListener(btn_closeGaiLvDes, () =>
         {
             trans_gaiLvDes.gameObject.SetActive(false);
         });

        RegisterEvent(TheEventType.OnChou, OnReceivedChou);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        ItemData danChouData = new ItemData();
        danChouData.settingId = (int)ItemIdType.TianJiFu;
        danChouData.count = 1;
        AddSingle<NoKuangItemView>(trans_danChouItemGrid, danChouData);


        ItemData shiLianData = new ItemData();
        shiLianData.settingId = (int)ItemIdType.TianJiFu;
        shiLianData.count = 9;
        AddSingle<NoKuangItemView>(trans_shiLianItemGrid, shiLianData);
        ShowBaoDiDes();
    }
    /// <summary>
    /// 保底描述
    /// </summary>
    void ShowBaoDiDes()
    {
        
        int baoDiBefore10 = 10 - RoleManager.Instance._CurGameInfo.ChouJiangData.baoDi10Num;
        int baoDiBefore50 = 50 - RoleManager.Instance._CurGameInfo.ChouJiangData.baoDi50Num;
        txt_baoDiDes.SetText(baoDiBefore10 
        + LanguageUtil.GetLanguageText((int)LanguageIdType.抽后必得地级弟子接引令) + "\n" 
        + baoDiBefore50 + LanguageUtil.GetLanguageText((int)LanguageIdType.抽后必得天级弟子接引令));
        
        // 显示今日剩余抽奖次数
        ShowTodayRemainDes();
    }
    
    /// <summary>
    /// 显示今日剩余抽奖次数
    /// </summary>
    void ShowTodayRemainDes()
    {
        int dailyLimit = 50;
        int todayUsed = (int)RoleManager.Instance._CurGameInfo.timeData.TodayChouNum;
        int remainTimes = dailyLimit - todayUsed;
        if (remainTimes < 0) remainTimes = 0;
        
        txt_todayRemainDes.SetText($"今日剩余抽奖次数：{remainTimes}/{dailyLimit}");
    }
    private void Update()
    {
        if (waitBloom)
        {
            waitBloomTimer += Time.deltaTime;
            if (waitBloomTimer >= waitBloomTime)
            {

                waitBloom = false;
                
                //img_black.color = Color.white;
                Bloom b;
                postProcessVolume.profile.TryGetSettings<Bloom>(out b);
                //AuditionManager.Instance.PlayVoice(AudioClipType.ZhuanChang);
                DOTween.To(() => b.intensity, x =>
                {
                    b.intensity.Override(x);
                }, 80, 1).OnComplete(() =>
                {
                    //AuditionManager.Instance.PlayBGM(AudioClipType.BGM_Mountain);

                    b.intensity.Override(0);
                    PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, resList);
                    ShowIdle();
                     
                    //img_black.gameObject.SetActive(true);
                    //img_black.DOKill();
                    //img_black.color = new Color(1, 1, 1, 1);
                    //img_black.DOColor(Color.black, 1).OnComplete(() =>
                    //{

                    //    RoleManager.Instance._CurGameInfo.playerPeople.yuanSu = (int)curChoosedXueMai;
                    //    RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Add((int)curChoosedXueMai);
                    //    RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[0].skillId = (int)BattleManager.Instance.PuGongIdByYuanSu((YuanSuType)RoleManager.Instance._CurGameInfo.playerPeople.yuanSu);
                    //    RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.equippedSkillIdList[0] = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[0].skillId;
                    //    PanelManager.Instance.ClosePanel(this);


                    //    string content1 = "你走进一个小树林，在潺潺的溪水中看见自己的脸，似乎想起了自己的名字和长相……";
                    //    Action openSetNameAction = OpenSetName;
                    //    PanelManager.Instance.OpenPanel<PangBaiPanel>(PanelManager.Instance.trans_layer2, content1, openSetNameAction);
                    //});
                });
            }
        }

        if (startVoice)
        {
            voiceTimer += Time.deltaTime;
            if (voiceTimer >= voiceTime)
            {
                voiceTimer = 0;
                AuditionManager.Instance.PlayVoice(AudioClipType.ChouKa);
                startVoice = false;
            }
        }

    }

    void ShowIdle()
    {
        txt_tianJingNum.SetText(UIUtil.ShowBigCount((long)ItemManager.Instance.FindItemCount((int)ItemIdType.TianJing)));
        txt_tianJiFuNum.SetText(UIUtil.ShowBigCount((long)ItemManager.Instance.FindItemCount((int)ItemIdType.TianJiFu)));

        trans_idle.gameObject.SetActive(true);
        trans_commonRes.gameObject.SetActive(false);
        trans_ziRes.gameObject.SetActive(false);
        trans_jinRes.gameObject.SetActive(false);
        ShowBaoDiDes();
    }
    /// <summary>
    /// 抽卡结束
    /// </summary>
    void OnReceivedChou(object[] args)
    {
        trans_idle.gameObject.SetActive(false);

        ChouJiangResData res = args[0] as ChouJiangResData;
        resList = res.resList;
        if (res.resRarity == Rarity.Tian)
        {
            trans_ziRes.gameObject.SetActive(false);
            trans_commonRes.gameObject.SetActive(false);
            trans_jinRes.gameObject.SetActive(true);

            ske_jinRes.Initialize(true);
            ske_jinRes.AnimationState.SetAnimation(0,"jing", false);
        }
        else if (res.resRarity == Rarity.Di)
        {
            trans_ziRes.gameObject.SetActive(true);
            trans_commonRes.gameObject.SetActive(false);
            trans_jinRes.gameObject.SetActive(false);

            ske_ziRes.Initialize(true);
            ske_ziRes.AnimationState.SetAnimation(0, "zi", false);
        }
        else
        {
            trans_ziRes.gameObject.SetActive(false);
            trans_commonRes.gameObject.SetActive(true);
            trans_jinRes.gameObject.SetActive(false);

            ske_commonRes.Initialize(true);
            ske_commonRes.AnimationState.SetAnimation(0, "lan", false);
        }
        waitBloomTimer = 0;
        waitBloom = true;

        voiceTimer = 0;
        startVoice = true;
    }
    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(trans_danChouItemGrid);
        ClearCertainParentAllSingle<SingleViewBase>(trans_shiLianItemGrid);
        ShowIdle();

        trans_idle.gameObject.SetActive(true);
        waitBloom = false;
        waitBloomTimer = 0;

        startVoice = false;
        voiceTimer = 0;

        trans_gaiLvDes.gameObject.SetActive(false);
    }
}
