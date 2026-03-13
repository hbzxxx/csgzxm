using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;

public class SkillPanel : PanelBase
{
    public PeopleData p;
    //public List<GameObject> maiObjList = new List<GameObject>();
    public List<Button> yuanSuBtnList = new List<Button>();
    //public List<GameObject> maiRedPointList = new List<GameObject>();

    //public List<GameObject> lockObjList = new List<GameObject>();

    public List<Transform> curEquippedSkillGridList;//当前装备的技能格子
    public List<GameObject> equipSkillLockList;//当前装备的技能格子锁

    //public Button btn_equip;//携带
    //public Button btn_unEquip;//卸下
    //public Button btn_upgrade;//升级
    //public Button btn_study;//学习

    public SingleSkillData curChoosedSkill;//当前选择的技能

    public Transform trans_commonPanel;//技能普通页面

    #region 选择技能页面
    //public JingMaiIDType curChoosedJingMaiIdType;
    public YuanSuType curChoosedYuanSuType;
    public Transform trans_chooseSkillPanel;//选择技能页面
    //public Button btn_closeChooseSkillPanel;//关闭选择技能页面
    public Transform trans_skillGrid;//技能格子
    public List<BigSkillView> bigSkillViewList = new List<BigSkillView>();
    public Image img_curYuanSu;//当前什么类型技能
    public Button btn_left;
    public Button btn_right;
    public GameObject obj_upgradeRedPoint;
    public Transform trans_studyConsumeGrid;//学习消耗

    #endregion

    #region 技能升级
    public Transform trans_upgrade;//升级页面
    public Text txt_nameInUpgradePanel;//升级页面的名字
    public Text txt_desInUpgradePanel;//升级页面的描述
    public List<GameObject> upgradeMaiList;//升级页面的所有筋脉
    public List<GameObject> upgradeFireList;//火焰
    public Transform trans_upgradeMaiEffectParent;//筋脉闪电链特效
    public Text txt_beforeUpgradeLv;//升级前
    public Text txt_afterUpgradeLv;//升级后
    public Text txt_beforeUpgradeDes;//升级前效果
    public Text txt_afterUpgradeDes;//升级后效果
    public Button btn_backUpgrade;//返回升级
    public Button btn_confirmUpgrade;//升级确认
    //public Button btn_equipInUpgradePanel;//携带
    //public Button btn_unEquipInUpgradePanel;//卸下
    public Image upgradeProcessBar;//进度
    public Text txt_beforeUpgradeBigLevelTag;//升级前大境界
    public Text txt_afterUpgradeBigLevelTag;//升级后大境界
    public Transform trans_upgradeConsumeGrid;//升级消耗

    #endregion
    public ScrollViewNevigation nevigation;

    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;

        for(int i = 0; i < yuanSuBtnList.Count; i++)
        {
            Button btn = yuanSuBtnList[i];
            YuanSuType type = (YuanSuType)(i+1);
            addBtnListener(btn, () =>
            {
                if(p.curUnlockedYuanSuList.Contains((int)type))
                {
                    ShowMySkill(type);
                }
                else
                {
                    if (p.isPlayer)
                    {
                        PanelManager.Instance.OpenFloatWindow("未解锁，请提升等级");
                    }
                    else
                    {
                        if (p.yuanSu == (int)type)
                        {
                            ShowMySkill(type);

                        }
                        else
                        {
                            PanelManager.Instance.OpenFloatWindow("无法切换");

                        }
                    }
                }
            });
    
        }
       

        addBtnListener(btn_left, () =>
        {
            if (curChoosedYuanSuType-1== 0)
            {
                return;
            }
            ShowMySkill((YuanSuType)(curChoosedYuanSuType - 1));
        });
        addBtnListener(btn_right, () =>
        {
            if (curChoosedYuanSuType == YuanSuType.End - 1)
            {
                return;
            }
            ShowMySkill((YuanSuType)(curChoosedYuanSuType + 1));

        });

        
        addBtnListener(btn_confirmUpgrade, () =>
        {
            TaskManager.Instance.guide_upgradeSkill = false;
            PanelManager.Instance.CloseTaskGuidePanel();

            List<SkillUpgradeSetting> settingList = DataTable.FindSkillUpgradeListBySkillId(curChoosedSkill.skillId);

            //消耗
            SkillUpgradeSetting curSetting = settingList[curChoosedSkill.skillLevel - 1];

            List<ItemData> consumeList = SkillManager.Instance.GetSkillUpgradeConsume(curChoosedSkill);
            for (int i = 0; i < consumeList.Count; i++)
            {
                ItemData single = consumeList[i];
              
                if (!ItemManager.Instance.CheckIfItemEnough(single.settingId, (ulong)single.count))
                {
                    ItemSetting setting = DataTable.FindItemSetting(single.settingId);
                    PanelManager.Instance.OpenFloatWindow(setting.Name + "不够");
                    return;

                }

            }
            //消耗

  
            for (int i = 0; i < consumeList.Count; i++)
            {
                ItemData item = consumeList[i];

                ItemManager.Instance.LoseItem(item.settingId, (ulong)item.count);

            }
            SkillManager.Instance. OnUpgradeSkill(curChoosedSkill);
        });

        addBtnListener(btn_backUpgrade, () =>
        {
            PanelManager.Instance.CloseTaskGuidePanel();
            curChoosedSkill = null;
            ShowCommonPanel();
            //ShowBottomBtn();

        });
        //addBtnListener(btn_closeChooseSkillPanel, () =>
        //{
        //    PanelManager.Instance.ClosePanel(this);

        //    //if (p.curUnlockedYuanSuList.Count <= 1)
        //    //{
        //    //    PanelManager.Instance.ClosePanel(this);
        //    //}
        //    //else
        //    //{
        //    //    trans_chooseSkillPanel.gameObject.SetActive(false);
        //    //    curChoosedSkill = null;
        //    //    ShowBottomBtn();
        //    //}
   
        //});


        RegisterEvent(TheEventType.RefreshSkillRedPoint, RefreshRedPoint);

        RegisterEvent(TheEventType.SkillUpgrade, OnUpgradeSkil);
        RegisterEvent(TheEventType.OnEquipSkill, OnEquipOrUnEquipSkill);
        RegisterEvent(TheEventType.OnUnEquipSkill, OnEquipOrUnEquipSkill);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();


        trans_upgrade.gameObject.SetActive(false);
        trans_chooseSkillPanel.gameObject.SetActive(false);
        ShowEquippedSkill();
        //ShowBottomBtn();
        ShowCommonPanel();
        ShowGuide();
        RefreshRedPoint();

        for (int i = 0; i < yuanSuBtnList.Count; i++)
        {
            Button btn = yuanSuBtnList[i];
            YuanSuType type = (YuanSuType)(i + 1);
            if (p.isPlayer)
            {
                if (p.curUnlockedYuanSuList.Contains((int)type))
                {
                    btn.onClick.Invoke();
                    break;
                }
            }
            else
            {
                if (p.yuanSu ==(int)type)
                {
                    btn.onClick.Invoke();
                    break;
                }
            }
       

        }
    }

    void RefreshRedPoint()
    {
        //for(int i = 0; i < maiRedPointList.Count; i++)
        //{
        //    RedPointManager.Instance.SetRedPointUI(maiRedPointList[i], RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai, i);
        //}
        //RefreshUpgradeRedPoint();
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_upgradeSkill)
        {
            if (TaskManager.Instance.guide_upgradeSkillId != 0)
            {
                for(int i = 0; i < bigSkillViewList.Count; i++)
                {
                    BigSkillView theView = bigSkillViewList[i];
                    //必须是已学习的技能（skillLevel > 0）才能升级
                    if(theView.skillData.skillId== TaskManager.Instance.guide_upgradeSkillId
                        && theView.skillData.skillLevel > 0)
                    {
                        PanelManager.Instance.LocateScrollAndTaskPoint(nevigation, theView.btn_upgrade.gameObject,true);
                        break;
                    }
                }
       
            }
        }

        //}
        else if (TaskManager.Instance.guide_equipSkill)
        {
            if (TaskManager.Instance.guide_equipSkillId != 0)
            {
                SkillSetting skillSetting = DataTable.FindSkillSetting(TaskManager.Instance.guide_equipSkillId);
                if (!string.IsNullOrWhiteSpace(skillSetting.YuanSu))
                {
                    int yuanSu = skillSetting.YuanSu.ToInt32();
                    //int index = jingMai[0];
                    PanelManager.Instance.ShowTaskGuidePanel(yuanSuBtnList[yuanSu - 1].gameObject);
                }

            }

        }
    }

        void ShowCommonPanel()
    {
        trans_upgrade.gameObject.SetActive(false);
        trans_commonPanel.gameObject.SetActive(true);
        for(int i = 0; i < yuanSuBtnList.Count; i++)
        {
            Image img = yuanSuBtnList[i].GetComponent<Image>();
            if (p.curUnlockedYuanSuList.Contains( (i + 1)))
            {
                img.material = null;
            }
            else
            {
                img.material = PanelManager.Instance.mat_grey;

            }

        }
        if (p.curUnlockedYuanSuList.Count <= 1)
        {
            ShowMySkill((YuanSuType)p.yuanSu);
        }
    }

    /// <summary>
    /// 显示升级页面
    /// </summary>
    public void ShowUpgradePanel(SingleSkillData skill)
    {
        curChoosedSkill = skill;

        if (curChoosedSkill == null)
            return;
        ClearCertainParentAllSingle<MaiLineView>(trans_upgradeMaiEffectParent);
        trans_commonPanel.gameObject.SetActive(false);
        trans_upgrade.gameObject.SetActive(true);
        SkillSetting setting = DataTable.FindSkillSetting(curChoosedSkill.skillId);
        txt_nameInUpgradePanel.SetText(setting.Name);
        txt_desInUpgradePanel.SetText(setting.Des);

        List<int> skillMaiList = CommonUtil.SplitCfgOneDepth(setting.JingMai);
        //符合条件的脉亮起来
        for(int i=0;i< upgradeMaiList.Count; i++)
        {
            int maiType = i;
            if (!skillMaiList.Contains(maiType))
            {
                upgradeMaiList[i].SetActive(false);
            }
            else
            {
                upgradeMaiList[i].SetActive(true);
            }
            upgradeFireList[i].SetActive(false);
        }
        //根据等级亮火焰
        
        int level = curChoosedSkill.skillLevel;
        int bigLevel = (level - 1) / 10+1;
        for(int i = 0; i < bigLevel + 1; i++)
        {
            if (i >= skillMaiList.Count)
                break;
            int index = skillMaiList[i];
            upgradeFireList[index].SetActive(true);
            //闪电链子
            if (i > 0)
            {
                AddSingle<MaiLineView>(trans_upgradeMaiEffectParent, upgradeFireList[index].transform, upgradeFireList[index-1].transform);
            }
        }

        List<SkillUpgradeSetting> upgradeList = DataTable.FindSkillUpgradeListBySkillId(curChoosedSkill.skillId);

        if (upgradeList == null || upgradeList.Count == 0 || curChoosedSkill.skillLevel <= 0 || curChoosedSkill.skillLevel > upgradeList.Count)
        {
            Debug.LogError($"[SkillPanel] 技能升级配置异常: skillId={curChoosedSkill.skillId}, skillLevel={curChoosedSkill.skillLevel}, upgradeList.Count={upgradeList?.Count ?? 0}");
            btn_confirmUpgrade.gameObject.SetActive(false);
            return;
        }

        SkillUpgradeSetting curSetting = upgradeList[curChoosedSkill.skillLevel - 1];
        txt_beforeUpgradeLv.SetText("Lv" + curChoosedSkill.skillLevel);
        txt_beforeUpgradeDes.SetText(SkillManager.Instance. ShowSkillFunctionDes(curSetting));
        txt_beforeUpgradeBigLevelTag.SetText(SkillManager.Instance.LevelTagStr(curChoosedSkill.skillLevel));
        if (curChoosedSkill.skillLevel < upgradeList.Count)
        {
            SkillUpgradeSetting afterSetting = upgradeList[curChoosedSkill.skillLevel];

            txt_afterUpgradeLv.SetText("Lv" + (curChoosedSkill.skillLevel + 1));
            txt_afterUpgradeDes.SetText(SkillManager.Instance.ShowSkillFunctionDes(afterSetting));

            upgradeProcessBar.fillAmount = (curChoosedSkill.skillLevel-(bigLevel-1)*10)  / 10f;
            txt_afterUpgradeLv.gameObject.SetActive(true);
            btn_confirmUpgrade.gameObject.SetActive(true);
            //显示消耗
            ClearCertainParentAllSingle<SingleConsumeView>(trans_upgradeConsumeGrid);
            List<ItemData> consumeList = SkillManager.Instance.GetSkillUpgradeConsume(curChoosedSkill);
            for(int i = 0; i < consumeList.Count; i++)
            {
                ItemData consume = consumeList[i];
                AddSingle<SingleConsumeView>(trans_upgradeConsumeGrid, (int)consume.settingId, (int)(ulong)consume.count, ConsumeType.Item);
            }
            trans_upgradeConsumeGrid.gameObject.SetActive(true);

            txt_afterUpgradeBigLevelTag.SetText(SkillManager.Instance.LevelTagStr(curChoosedSkill.skillLevel+10));
            txt_afterUpgradeBigLevelTag.gameObject.SetActive(true);

            //引导
            if (TaskManager.Instance.guide_upgradeSkill
                && TaskManager.Instance.guide_upgradeSkillId == curChoosedSkill.skillId)
            {
                PanelManager.Instance.ShowTaskGuidePanel(btn_confirmUpgrade.gameObject);
            }


        }
        else
        {
            upgradeProcessBar.fillAmount = 1;
            txt_afterUpgradeLv.gameObject.SetActive(false);
            btn_confirmUpgrade.gameObject.SetActive(false);
            trans_upgradeConsumeGrid.gameObject.SetActive(false);
            txt_afterUpgradeBigLevelTag.gameObject.SetActive(false);
        }
        ShowUpgradeBottomBtn();
    }




    /// <summary>
    /// 显示当前装配的技能
    /// </summary>
    void ShowEquippedSkill()
    {
         int unlockedPos = p.allSkillData.unlockedSkillPos;
        for (int i = 0; i < equipSkillLockList.Count; i++)
        {
            GameObject lockObj = equipSkillLockList[i];
            if (i < unlockedPos)
            {
                lockObj.SetActive(false);
            }
            else
            {
                lockObj.SetActive(true);
            }
        }

        for(int i=0;i< curEquippedSkillGridList.Count; i++)
        {
            Transform trans = curEquippedSkillGridList[i];
            ClearCertainParentAllSingle<SingleSkillView>(trans);
        }
        if (p.allSkillData != null && p.allSkillData.equippedSkillIdList != null)
        {
            for(int i = 1; i < p.allSkillData.equippedSkillIdList.Count; i++)
            {
                if (i - 1 >= curEquippedSkillGridList.Count)
                    break;
                SingleSkillData singleSkillData =SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[i], p.allSkillData);
                Transform trans = curEquippedSkillGridList[i-1];

                AddSingle<SingleSkillView>(trans, singleSkillData, SkillViewType.EquippedSkill,this);
            }
        }
    }

    /// <summary>
    /// 显示我的技能
    /// </summary>
    public void ShowMySkill(YuanSuType yuanSu)
    {
        for(int i = 0; i < yuanSuBtnList.Count; i++)
        {
            Button btn = yuanSuBtnList[i];
            if (i+1 == (int)yuanSu)
            {
                btn.GetComponent<Image>().enabled = true;
            }
            else
            {
                btn.GetComponent<Image>().enabled = false;
            }
        }

        curChoosedYuanSuType = yuanSu;
        trans_chooseSkillPanel.gameObject.SetActive(true);
        img_curYuanSu.sprite = ConstantVal.YuanSuIcon(curChoosedYuanSuType);
        bigSkillViewList.Clear();
        ClearCertainParentAllSingle<BigSkillView>(trans_skillGrid);

        List<SkillSetting> candidateList = new List<SkillSetting>();
        for (int i = 0; i < DataTable._skillList.Count; i++)
        {
            SkillSetting setting = DataTable._skillList[i];
            if(!string.IsNullOrWhiteSpace(setting.CanStudy))
            candidateList.Add(setting);
        }
        //for (int i = 0; i < ConstantVal.canLingWuSkillIdList2.Count; i++)
        //{
        //    candidateList.Add((int)ConstantVal.canLingWuSkillIdList2[i]);
        //}

        List<int> mSkillIdList = new List<int>();
        for (int i = 0; i < p.allSkillData.skillList.Count; i++)
        {
            mSkillIdList.Add(p.allSkillData.skillList[i].skillId);
        }
        List<SingleSkillData> candidateDataList = new List<SingleSkillData>();
        for (int i = 0; i < candidateList.Count; i++)
        {
            //int skillId = candidateList[i];
            SkillSetting skillSetting = candidateList[i];// DataTable.FindSkillSetting(skillId);
            if (string.IsNullOrWhiteSpace(skillSetting.YuanSu))
                continue;
            //List<int> jingMai = CommonUtil.SplitCfgOneDepth(skillSetting.JingMai);
            YuanSuType theYuanSu = (YuanSuType)skillSetting.YuanSu.ToInt32();
            SingleSkillData singleSkillData = null;
            if (theYuanSu == yuanSu)
            {
                //要显示
                if (mSkillIdList.Contains(skillSetting.Id.ToInt32()))
                {
                    int index = mSkillIdList.IndexOf(skillSetting.Id.ToInt32());
                    singleSkillData = p.allSkillData.skillList[index];
                }
                else
                {
                    singleSkillData = new SingleSkillData();
                    singleSkillData.skillLevel = 0;
                    singleSkillData.skillId = skillSetting.Id.ToInt32();
                 }
                candidateDataList.Add(singleSkillData);

            }

        }
        if (candidateDataList.Count > 0)
        {
            //排序
            for (int i = 0; i < candidateDataList.Count - 1; i++)
            {
                for (int j = 0; j < candidateDataList.Count - 1 - i; j++)
                {
                    //后面的我学了 则二者交换
                    if (mSkillIdList.Contains(candidateDataList[j + 1].skillId))
                    {
                        var temp = candidateDataList[j];
                        candidateDataList[j] = candidateDataList[j + 1];
                        candidateDataList[j + 1] = temp;
                    }
                }
            }
            for (int i = 0; i < candidateDataList.Count - 1; i++)
            {
                for (int j = 0; j < candidateDataList.Count - 1 - i; j++)
                {
                    //后面的我装配了 则二者交换
                    if (RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.equippedSkillIdList.Contains(candidateDataList[j + 1].skillId))
                    {
                        var temp = candidateDataList[j];
                        candidateDataList[j] = candidateDataList[j + 1];
                        candidateDataList[j + 1] = temp;
                    }
                }
            }
            for (int i = 0; i < candidateDataList.Count; i++)
            {
                BigSkillView bigSkillView = AddSingle<BigSkillView>(trans_skillGrid, candidateDataList[i], this);
                bigSkillViewList.Add(bigSkillView);
            }

            if (bigSkillViewList.Count > 0 && mSkillIdList.Contains(bigSkillViewList[0].skillData.skillId))
            {
                //判定是否当前属性
                if (bigSkillViewList[0].skillSetting.YuanSu.ToInt32() == RoleManager.Instance._CurGameInfo.playerPeople.yuanSu)
                {
                    bigSkillViewList[0].btn.onClick.Invoke();
                }
            }

            if (TaskManager.Instance.guide_upgradeSkill)
            {
                if (curChoosedSkill != null)
                {
                    if (curChoosedSkill.skillId != TaskManager.Instance.guide_upgradeSkillId)
                    {
                        for (int i = 0; i < bigSkillViewList.Count; i++)
                        {
                            BigSkillView bigSkillView = bigSkillViewList[i];
                            //必须是已学习的技能（skillLevel > 0）才能升级
                            if (bigSkillView.skillData.skillId == TaskManager.Instance.guide_upgradeSkillId
                                && bigSkillView.skillData.skillLevel > 0)
                            {
                                PanelManager.Instance.ShowTaskGuidePanel(bigSkillView.gameObject);
                                break;
                            }
                        }
                    }
                    else
                    {
                        //选择底部升级按钮
                        //PanelManager.Instance.ShowTaskGuidePanel(btn_upgrade.gameObject);

                    }
                }
           
            }
            else if (TaskManager.Instance.guide_equipSkill)
            {
                if (curChoosedSkill.skillId != TaskManager.Instance.guide_equipSkillId)
                {
                    for (int i = 0; i < bigSkillViewList.Count; i++)
                    {
                        BigSkillView bigSkillView = bigSkillViewList[i];
                        if (bigSkillView.skillData.skillId == TaskManager.Instance.guide_equipSkillId)
                        {
                            PanelManager.Instance.ShowTaskGuidePanel(bigSkillView.gameObject);
                            break;
                        }
                    }
                }
                else
                {
                    //选择底部升级按钮
                    //PanelManager.Instance.ShowTaskGuidePanel(btn_equip.gameObject);

                }
            }
        }
        if (bigSkillViewList.Count > 0)
        {
            bigSkillViewList[0].btn.onClick.Invoke();
        }

        if (TaskManager.Instance.guide_equipSkill)
        {
            if (TaskManager.Instance.guide_equipSkillId != 0)
            {
                SkillSetting skillSetting = DataTable.FindSkillSetting(TaskManager.Instance.guide_equipSkillId);
                for(int i = 0; i < bigSkillViewList.Count; i++)
                {
                    BigSkillView theView = bigSkillViewList[i];
                    if(theView.skillSetting.Id.ToInt32()== TaskManager.Instance.guide_equipSkillId)
                    {
                        PanelManager.Instance.ShowTaskGuidePanel(theView.btn_equip.gameObject);
                        break;
                    }
                }
    
            }

        }
    }

    public void OnChoosedSkill(SingleSkillData singleSkillData)
    {
        //ClearCertainParentAllSingle<SingleSkillView>(trans_curChoosedSkillGrid);
        //SingleSkillView view= AddSingle<SingleSkillView>(trans_curChoosedSkillGrid, singleSkillData,SkillViewType.None);
        for(int i = 0; i < bigSkillViewList.Count; i++)
        {
            BigSkillView theView = bigSkillViewList[i];
            if (theView.skillData.skillId == singleSkillData.skillId)
            {
                theView.OnChoose(true);
            }
            else
            {
                theView.OnChoose(false);
            }
        }
        curChoosedSkill = singleSkillData;
        //ShowChoosedSkillDetail();
        //ShowBottomBtn();


        if (TaskManager.Instance.guide_upgradeSkill)
        {
            if (curChoosedSkill.skillId != TaskManager.Instance.guide_upgradeSkillId)
            {
                for (int i = 0; i < bigSkillViewList.Count; i++)
                {
                    BigSkillView bigSkillView = bigSkillViewList[i];
                    //必须是已学习的技能（skillLevel > 0）才能升级
                    if (bigSkillView.skillData.skillId == TaskManager.Instance.guide_upgradeSkillId
                        && bigSkillView.skillData.skillLevel > 0)
                    {
                        PanelManager.Instance.ShowTaskGuidePanel(bigSkillView.gameObject);
                        break;
                    }
                }
            }
            else
            {
                //选择底部升级按钮
                //PanelManager.Instance.ShowTaskGuidePanel(btn_upgrade.gameObject);

            }
        }
        else if (TaskManager.Instance.guide_equipSkill)
        {
            if (curChoosedSkill.skillId != TaskManager.Instance.guide_equipSkillId)
            {
                for (int i = 0; i < bigSkillViewList.Count; i++)
                {
                    BigSkillView bigSkillView = bigSkillViewList[i];
                    if (bigSkillView.skillData.skillId == TaskManager.Instance.guide_equipSkillId)
                    {
                        PanelManager.Instance.ShowTaskGuidePanel(bigSkillView.gameObject);
                        break;
                    }
                }
            }
            else
            {
                //选择底部升级按钮
                //PanelManager.Instance.ShowTaskGuidePanel(btn_equip.gameObject);

            }
        }
 
        else if (TaskManager.Instance.guide_studySkill)
        {
            //PanelManager.Instance.ShowTaskGuidePanel(btn_study.gameObject);


        }
    
    }




    void ShowUpgradeBottomBtn()
    {
        if (curChoosedSkill!= null)
        {
            //if (!curChoosedSkill.isEquipped)
            //{
            //    btn_equipInUpgradePanel.gameObject.SetActive(true);
            //    btn_unEquipInUpgradePanel.gameObject.SetActive(false);
            //}
            //else
            //{
            //    btn_equipInUpgradePanel.gameObject.SetActive(false);
            //    btn_unEquipInUpgradePanel.gameObject.SetActive(true);
            //}
        }
    }

    void OnUpgradeSkil()
    {
        ShowUpgradePanel(curChoosedSkill);
    }

    void OnEquipOrUnEquipSkill(object[] args)
    {
        ShowEquippedSkill();
    }

    public override void Clear()
    {
        base.Clear();
        curChoosedSkill = null;
        //trans_curJingMaiType.gameObject.SetActive(false);
    }
    public override void OnClose()
    {
        base.OnClose();
        TaskManager.Instance.guide_equipSkill = false;
        TaskManager.Instance.guide_upgradeSkill = false;
        PanelManager.Instance.CloseTaskGuidePanel();
    }
}
