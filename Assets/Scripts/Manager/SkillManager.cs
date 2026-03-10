using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Data;
using cfg;

public class SkillManager : CommonInstance<SkillManager>
{
    /// <summary>
    /// 加一个技能到内存
    /// </summary>
    /// <param name="skillId"></param>
    public SingleSkillData AddProtoSkill(int skillId, PeopleData p)
    {
        bool haveSameSkill = false;
        for (int i = 0; i < p.allSkillData.skillList.Count; i++)
        {
            SingleSkillData data = p.allSkillData.skillList[i];
            if (data.skillId == skillId)
            {
                haveSameSkill = true;
                return null;
            }
        }
        SingleSkillData singleSkillData = new SingleSkillData();
        singleSkillData.skillId = skillId;
        singleSkillData.skillLevel = 1;
        p.allSkillData.skillList.Add(singleSkillData);
        SkillSetting skillSetting = DataTable.FindSkillSetting(singleSkillData.skillId);
        //PanelManager.Instance.OpenOnlyOkHint("已学会技能：" + skillSetting.name,null);
        RefreshRedPointShow(skillId);
        return singleSkillData;
    }
    public void OnStudySkill(int skillId,PeopleData p)
    {
        if (!ItemManager.Instance.CheckIfItemEnough(skillId, 1))
        {
            PanelManager.Instance.OpenFloatWindow("材料不够");
            return;
        }
        if (CheckIfHaveSkill(skillId, p))
        {
            PanelManager.Instance.OpenFloatWindow("已经学习了");
            return;
        }

            ItemManager.Instance.LoseItem(skillId, 1);
            SingleSkillData data= SkillManager.Instance.AddSkill(skillId, p);
            EventCenter.Broadcast(TheEventType.StudySkill, data);
        
    }
    public SingleSkillData FindSkillById(int skillId, PeopleData p)
    {
        for (int i = 0; i < p.allSkillData.skillList.Count; i++)
        {
            SingleSkillData data = p.allSkillData.skillList[i];
            if (data.skillId == skillId)
            {
                return data;
            }
        }
        return null;

    }
    public bool CheckIfHaveSkill(int skillId,PeopleData p)
    {
        for(int i = 0; i < p.allSkillData.skillList.Count; i++)
        {
            SingleSkillData data = p.allSkillData.skillList[i];
            if (data.skillId == skillId)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 加一个技能
    /// </summary>
    /// <param name="skillId"></param>
    public SingleSkillData AddSkill(int skillId,PeopleData p)
    {
        bool haveSameSkill = false;
        for(int i = 0; i < p.allSkillData.skillList.Count; i++)
        {
            SingleSkillData data = p.allSkillData.skillList[i];
            if (data.skillId == skillId)
            {
                haveSameSkill = true;
                return null;
            }
        }
        SingleSkillData singleSkillData = new SingleSkillData();
        singleSkillData.skillId = skillId;
        singleSkillData.skillLevel = 1;
        p.allSkillData.skillList.Add(singleSkillData);
        SkillSetting skillSetting = DataTable.FindSkillSetting(singleSkillData.skillId);
        //PanelManager.Instance.OpenOnlyOkHint("已学会技能：" + skillSetting.name,null);
        RefreshRedPointShow(skillId);
        return singleSkillData;
    }
    /// <summary>
    /// 装备技能 TODO战斗力变化
    /// </summary>
    /// <param name="skillId"></param>
    public void EquipProtoSkill(PeopleData p, SingleSkillData singleSkillData)
    {
        bool haveSameSkill = false;
        SkillSetting theSkillSetting = DataTable.FindSkillSetting(singleSkillData.skillId);

        for (int i = 0; i < p.allSkillData.equippedSkillIdList.Count; i++)
        {
            SingleSkillData data = SkillManager.Instance.GetSingleSkillProtoDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[i], p.allSkillData);
            if (data.skillId == singleSkillData.skillId)
            {
                haveSameSkill = true;
                return;
            }
            SkillSetting existedSkillSetting = DataTable.FindSkillSetting(data.skillId);
            if (i >= 1
                && theSkillSetting.YuanSu == existedSkillSetting.YuanSu)
            {
              
                PanelManager.Instance.OpenFloatWindow("无法携带同类"+LanguageUtil.GetLanguageText((int)LanguageIdType.功法));
                return;
            }

        }
        ////装备上限
        //if(p.AllSkillData.EquippedSkillIdList.Count-1>= RoleManager.Instance._CurGameInfo.playerPeople.AllSkillData.UnlockedSkillPos)
        //{
        //    PanelManager.Instance.OpenFloatWindow("技能携带已达上限");
        //    return;
        //}
        //RoleManager.Instance._CurGameInfo.playerPeople.AllSkillData.
        singleSkillData.isEquipped = true;
        p.allSkillData.equippedSkillIdList.Add(singleSkillData.skillId);
        EventCenter.Broadcast(TheEventType.OnEquipSkill, singleSkillData);
        TaskManager.Instance.TryAccomplishAllTask();

        //TalkingDataGA.OnEvent("携带技能", new Dictionary<string, object>() { { theSkillSetting.name, 1 } });

    }

    /// <summary>
    /// 装备技能 TODO战斗力变化
    /// </summary>
    /// <param name="skillId"></param>
    public void EquipSkill(PeopleData p, SingleSkillData singleSkillData)
    {
        bool haveSameSkill = false;
        SkillSetting theSkillSetting = DataTable.FindSkillSetting(singleSkillData.skillId);
        if (p.yuanSu != theSkillSetting.YuanSu.ToInt32())
        {
            PanelManager.Instance.OpenCommonHint("该"+LanguageUtil.GetLanguageText((int)LanguageIdType.功法)+"与您当前属性不符,是否切换属性？", () =>
            {
                JumpPageManager.Instance.JumpToChangeYuanSu();
            }, null);
            return;
        }
        for (int i = 0; i < p.allSkillData.equippedSkillIdList.Count; i++)
        {
            SingleSkillData data =SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[i], p.allSkillData);
            if (data.skillId == singleSkillData.skillId)
            {
                haveSameSkill = true;
                return;
            }
            SkillSetting existedSkillSetting = DataTable.FindSkillSetting(data.skillId);
            if (i>=1
                &&theSkillSetting.YuanSu == existedSkillSetting.YuanSu)
            {
                PanelManager.Instance.OpenFloatWindow("无法携带同类"+LanguageUtil.GetLanguageText((int)LanguageIdType.功法));
                return;
            }

        }
        ////装备上限
        //if(p.AllSkillData.EquippedSkillIdList.Count-1>= RoleManager.Instance._CurGameInfo.playerPeople.AllSkillData.UnlockedSkillPos)
        //{
        //    PanelManager.Instance.OpenFloatWindow("技能携带已达上限");
        //    return;
        //}
        //RoleManager.Instance._CurGameInfo.playerPeople.AllSkillData.
        singleSkillData.isEquipped = true;
        p.allSkillData.equippedSkillIdList.Add(singleSkillData.skillId);
        EventCenter.Broadcast(TheEventType.OnEquipSkill, singleSkillData);
        TaskManager.Instance.TryAccomplishAllTask();

        //TalkingDataGA.OnEvent("携带技能", new Dictionary<string, object>() { { theSkillSetting.name, 1 } });

    }

    /// <summary>
    /// 卸下技能
    /// </summary>
    public void UnEquipSkill(PeopleData p, SingleSkillData singleSkillData)
    {
        SkillSetting skillSetting = DataTable.FindSkillSetting(singleSkillData.skillId);
        for (int i = p.allSkillData.equippedSkillIdList.Count-1; i >=0; i--)
        {
            SingleSkillData data =SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[i], p.allSkillData);
            if (data.skillId == singleSkillData.skillId)
            {
                data.isEquipped = false;
                p.allSkillData.equippedSkillIdList.Remove(data.skillId);
                EventCenter.Broadcast(TheEventType.OnUnEquipSkill, singleSkillData);

                //TalkingDataGA.OnEvent("卸下技能", new Dictionary<string, object>() { { skillSetting.name, 1 } });

                break;
            }
        }
    }



    /// <summary>
    /// 技能升级
    /// </summary>
    public void OnUpgradeSkill(SingleSkillData singleSkillData)
    {
        List<SkillUpgradeSetting> settingList = DataTable.FindSkillUpgradeListBySkillId(singleSkillData.skillId);

        if (singleSkillData.skillLevel < settingList.Count)
        {
          
            singleSkillData.skillLevel++;
            EventCenter.Broadcast(TheEventType.SkillUpgrade, singleSkillData);
            TaskManager.Instance.TryAccomplishAllTask();
            SkillSetting skillSetting = DataTable.FindSkillSetting(singleSkillData.skillId);
            //TalkingDataGA.OnEvent("技能等级", new Dictionary<string, object>() { { skillSetting.name, singleSkillData.skillLevel } });
            SkillManager.Instance.RefreshRedPointShow(singleSkillData.skillId);
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
            {
                StudentManager.Instance.RefreshRedPointShow(RoleManager.Instance._CurGameInfo.studentData.allStudentList[i]);

            }
        }
    


        //singleSkillData.SkillLevel++;
    }
    //public int SkillBigLevelBySmallLevel()
    //{

    //}
    /// <summary>
    /// 标签
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public string LevelTagStr(int skillLevel)
    {
        string res = "";
        int bigLevel = skillLevel / 10 + 1;
        if (skillLevel >= 1 && skillLevel <= 10)
        {
            res = "初学";
        }else if (skillLevel >= 11 && skillLevel <= 20)
        {
            res = "小成";

        }
        else if (skillLevel >= 21 && skillLevel <= 30)
        {
            res = "娴熟";

        }
        else if (skillLevel >= 31 && skillLevel <= 40)
        {
            res = "精通";

        }
        else
        {
            res = "完美";

        }

        return res;
    }
    public SingleSkillData GetSingleSkillProtoDataByEquippedSkillId(int id, AllSkillData allSkillData)
    {
        for (int i = 0; i < allSkillData.skillList.Count; i++)
        {
            SingleSkillData data = allSkillData.skillList[i];
            if (data.skillId == id)
            {
                return data;
            }

        }
        return null;

    }

    public SingleSkillData GetSingleSkillDataByEquippedSkillId(int id,AllSkillData allSkillData)
    {
        for(int i = 0; i < allSkillData.skillList.Count; i++)
        {
            SingleSkillData data = allSkillData.skillList[i];
            if (data.skillId == id)
            {
                return data;
            }

        }
        return null;

    }

    /// <summary>
    /// 技能升级材料
    /// </summary>
    /// <returns></returns>
    public List<ItemData> GetSkillUpgradeConsume(SingleSkillData singleSkillData)
    {
        List<ItemData> res = new List<ItemData>();
        List<SkillUpgradeSetting> upgradeList = DataTable.FindSkillUpgradeListBySkillId(singleSkillData.skillId);
        if (singleSkillData.skillLevel < upgradeList.Count)
        {
            SkillUpgradeSetting curSetting = upgradeList[singleSkillData.skillLevel - 1];
            //显示消耗
            List<List<int>> consume = CommonUtil.SplitCfg(curSetting.Consume);
            for (int i = 0; i < consume.Count; i++)
            {
                List<int> singleConsume = consume[i];
                ItemData data = new ItemData();
                data.settingId = singleConsume[0];
                data.count = (ulong)singleConsume[1];
                res.Add(data);
                //AddSingle<SingleConsumeView>(trans_upgradeConsumeGrid, singleConsume[0], singleConsume[1], ConsumeType.Item);
            }
            int needBookNum = 0;
            if ((singleSkillData.skillLevel + 1) % 5 == 0)
            {
                List<int> bookNumList = new List<int> { 1, 3, 5, 8, 11, 15, 19, 23, 27, 32 };
                int needBookNumIndex = (singleSkillData.skillLevel + 1) / 5-1;
                needBookNum = bookNumList[needBookNumIndex];
                ItemData bookItemData = new ItemData();
                bookItemData.settingId = curSetting.SkillId.ToInt32();
                bookItemData.count = (ulong)needBookNum;
                res.Add(bookItemData);
            }
  
        }
        return res;
        
    }


    public void InitRedPoint()
    {
        ClearRedPoint();
        RedPoint MainPanel_Btn_Knapsack = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Knapsack, 0);

        RedPoint MainPanel_Btn_Knapsack_SkillTag = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Knapsack_SkillTag, 0);
        RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Knapsack, MainPanel_Btn_Knapsack_SkillTag);


        for (int i = 0; i < (int)JingMaiIDType.End; i++)
        {
            RedPoint MainPanel_Btn_Knapsack_SkillTag_Mai = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai, i);
            //绑定
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Knapsack_SkillTag, MainPanel_Btn_Knapsack_SkillTag_Mai);
        }
        List<int> candidateList = new List<int>();
        for (int i = 0; i < DataTable._skillList.Count; i++)
        {
            SkillSetting setting = DataTable._skillList[i];
            if(!string.IsNullOrWhiteSpace(setting.CanStudy))
            candidateList.Add(DataTable._skillList[i].Id.ToInt32());
        }
        //for (int i = 0; i < ConstantVal.canLingWuSkillIdList2.Count; i++)
        //{
        //    candidateList.Add((int)ConstantVal.canLingWuSkillIdList2[i]);
        //}

        for (int i = 0; i < candidateList.Count; i++)
        {
            int id = candidateList[i];
             //他们都绑定全部
            RedPoint MainPanel_Btn_Knapsack_SkillTag_Mai_Skill = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill, id);
            RedPoint MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Upgrade = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Upgrade, id);
            RedPoint MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Find = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Find, id);

            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Knapsack_SkillTag_Mai_Skill, MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Upgrade);
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Knapsack_SkillTag_Mai_Skill, MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Find);



            SkillSetting skillSetting = DataTable.FindSkillSetting(id);
            List<int> maiList = CommonUtil.SplitCfgOneDepth(skillSetting.JingMai);
            RedPoint MainPanel_Btn_Knapsack_SkillTag_Mai= RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai, maiList[0]);
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Knapsack_SkillTag_Mai, MainPanel_Btn_Knapsack_SkillTag_Mai_Skill);

            RefreshRedPointShow(id);
        }

    }

    /// <summary>
    /// 刷新所有技能红点
    /// </summary>
    /// <param name="skillId"></param>
    public void RefreshAllRedPointShow()
    {
        List<int> candidateList = new List<int>();
        for (int i = 0; i <DataTable._skillList.Count; i++)
        {
            SkillSetting setting = DataTable._skillList[i];
            if (!string.IsNullOrWhiteSpace(setting.CanStudy))
            {
                candidateList.Add(setting.Id.ToInt32());

            }
        }
        //for (int i = 0; i < ConstantVal.canLingWuSkillIdList2.Count; i++)
        //{
        //    candidateList.Add((int)ConstantVal.canLingWuSkillIdList2[i]);
        //}

        for (int i = 0; i < candidateList.Count; i++)
        {
 
            RefreshRedPointShow(candidateList[i]);
        }


    }
    /// <summary>
    /// 升级材料满足可升级 或者新发现的
    /// </summary>
    /// <param name="skillId"></param>
    public void RefreshRedPointShow(int skillId)
    {
        if (!RedPointManager.Instance.initOk)
            return;
        bool show = false;
         SingleSkillData mSkill = null;
        for(int i = 0; i <RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count; i++)
        {
            SingleSkillData skill = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[i];
            if (skill.skillId == skillId)
            {
                mSkill = skill;
                break;
            }
        }
        if (mSkill == null)
        {
            RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Find, skillId, false);
            RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Upgrade, skillId, false);

        }

        else
        {
            //如果没查看过，显示红点
            if (!mSkill.show)
            {
                RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Find, skillId, true);
            }
            else
            {
                RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Find, skillId, false);

            }
            // 如果可以升级，显示红点


            bool consumeEnough = true;
                List<SkillUpgradeSetting> settingList = DataTable.FindSkillUpgradeListBySkillId(mSkill.skillId);
 
                List<ItemData> consumeList = SkillManager.Instance.GetSkillUpgradeConsume(mSkill);
                for (int i = 0; i < consumeList.Count; i++)
                {
                ItemData single = consumeList[i];

                    if (!ItemManager.Instance.CheckIfItemEnough(single.settingId, (ulong)single.count))
                    {
                        consumeEnough = false;
                        break;
                    }
                }
                //消耗
                if (consumeEnough)
                {

                    show = true;

                }
                else
                {
                    show = false;
                }
                RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Upgrade, skillId, show);

            

        }
 
         EventCenter.Broadcast(TheEventType.RefreshSkillRedPoint);
    }
    /// <summary>
    /// 清掉所有红点
    /// </summary>
    public void ClearRedPoint()
    {
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Knapsack);

        RedPoint knapsackPoint = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Knapsack, 0);
        for (int i= knapsackPoint.sunList.Count-1; i>=0; i--)
        {
            if(knapsackPoint.sunList[i].redPointType== RedPointType.MainPanel_Btn_Knapsack_SkillTag)
            {
                knapsackPoint.sunList.RemoveAt(i);
            }
        }
        
       RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Knapsack_SkillTag);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Upgrade);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Find);


    }

    /// <summary>
    /// 通过元素得到普攻功法id
    /// </summary>
    /// <param name="yuanSuType"></param>
    /// <returns></returns>
    public SkillIdType SkillIdByYuanSu(YuanSuType yuanSuType)
    {
        switch (yuanSuType)
        {
            case YuanSuType.Water:
                return SkillIdType.ShuiPuGong;
            case YuanSuType.Fire:
                return SkillIdType.HuoPuGong;
            case YuanSuType.Storm:
                return SkillIdType.LeiPuGong;
            case YuanSuType.Ice:
                return SkillIdType.BingPuGong;
            case YuanSuType.Light:
                return SkillIdType.LingDan;
            case YuanSuType.Dark:
                return SkillIdType.DarkPuGong;
        }

        return SkillIdType.None;
    }

    /// <summary>
    /// 显示技能作用描述
    /// </summary>
    /// <returns></returns>
    public string ShowSkillFunctionDes(SkillUpgradeSetting skillUpgradeSetting)
    {
        string res = "";
        if (!string.IsNullOrWhiteSpace(skillUpgradeSetting.UpgradeDes))
            res = skillUpgradeSetting.UpgradeDes;

        List<string> replaceStrList = CommonUtil.SplitCfgStringOneDepth(skillUpgradeSetting.DesReplace);
        for (int i = 0; i < replaceStrList.Count; i++)
        {
            string signal = "[" + i + "]";
            string str = replaceStrList[i];

            res = res.Replace(signal, str);
        }


        return res;
        //string damage = skillUpgradeSetting.damage;
        //List<int> damageList = CommonUtil.SplitCfgOneDepth(damage);

        //for (int i = 0; i < damageList.Count; i++)
        //{
        //    int singleDamage = damageList[0];
        //    string singleStr = (i + 1) + "段伤害：" + (singleDamage + 100) + "%\n";
        //    res += singleStr;
        //}
        //return res;
    }
}
