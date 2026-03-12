using cfg;
using Framework.Data;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StudentManager : CommonInstance<StudentManager>
{
     

    /// <summary>
    /// 弟子已满
    /// </summary>
    /// <returns></returns>
    public bool AllStudentReachLimit()
    {
        if (RoleManager.Instance._CurGameInfo.studentData.CurStudentNum >= RoleManager.Instance._CurGameInfo.studentData.MaxStudentNum)
            return true;
        return false;
    }

    /// <summary>
    /// 生成炼丹学生
    /// </summary>
    public PeopleData GenerateLianDanStudent(PeopleData p)
    {
      

        p.talent = (int)StudentTalent.LianJing;
        p.studentCurEnergy = 100;

        string proStr = ConstantVal.baseLianDanStudentProId;
        //switch ((Rarity)rarity)
        //{
        //    case Rarity.Huang:
        //        proStr = ConstantVal.baseLianDanStudentPro_huang;
        //        break;
        //    case Rarity.Xuan:
        //        proStr = ConstantVal.baseLianDanStudentPro_xuan;
        //        break;
        //    case Rarity.Di:
        //        proStr = ConstantVal.baseLianDanStudentPro_di;
        //        break;
        //    case Rarity.Tian:
        //        proStr = ConstantVal.baseLianDanStudentPro_tian;
        //        break;
        //}
        List<int> proIdList = CommonUtil.SplitCfgOneDepth(proStr);
        for (int i = 0; i < proIdList.Count; i++)
        {
            //List<int> singlePro = proChange[i];
            int theId = proIdList[i];
            //int theNum = singlePro[1];
            SinglePropertyData pro = new SinglePropertyData();
            pro.id = theId;
            pro.num = (int)ConstantVal.workStudentInitProperty;
            int proQuality = RandomManager.Next(1, 6);

            pro.quality = proQuality;
            p.propertyIdList.Add(theId);
            p.propertyList.Add(pro);
            //p.ProQualityList.Add(proQuality);
        }
        return p;
    }

    /// <summary>
    /// 一年开始生成弟子们
    /// </summary>
    public void YearStartGenerateStudents()
    {  
        //山门招募上限就不刷了
        if (RoleManager.Instance._CurGameInfo.studentData.todayRecruitStudentNum >= ConstantVal.maxStudentRecruitNumPerDay)
        {
             return;
        }
        RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Clear();
        AuditionManager.Instance.PlayVoice(AudioClipType.NewStudentInfo);
        RdmGenerate3CandidateStudents(GenerateCandidateStudentType.AD);
        RoleManager.Instance._CurGameInfo.studentData.thisYearBrushStudentNum = 0;
        RoleManager.Instance._CurGameInfo.studentData.thisYearRecruitedStudentNum = 0;
        RoleManager.Instance._CurGameInfo.studentData.thisYearRemainCanRecruitStudentNum = 3;
        RoleManager.Instance._CurGameInfo.studentData.thisYearWatchedADNum = false;
    }

    /// <summary>
    /// 随机生成3个弟子
    /// </summary>
    public void RdmGenerate3CandidateStudents(GenerateCandidateStudentType type)
    {
        for (int i = 0; i < 3; i++)
        {
            PeopleData p = GenerateCandidateStudentByZongMenLevel(type,Gender.None);
            RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Add(p);
        }

        EventCenter.Broadcast(TheEventType.GeneratedNewStudent);
    }
    /// <summary>
    /// 设置头像
    /// </summary>
    public void SetTouxiang(Image icon, PeopleData peopledate) {
        if (icon == null)
        {
            Debug.LogError("[SetTouxiang] icon is null!");
            return;
        }
        icon.gameObject.SetActive(true);
        if (peopledate.isPlayer)
        {
            if (peopledate.gender == (int)Gender.Male)
            {
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.PeopleTouxiang + (int)StudentTalent.ManKing);
            }
            else if (peopledate.gender == (int)Gender.Female)
            {
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.PeopleTouxiang + (int)StudentTalent.FemaleKing);
            }
        }
        else
        {
            //如果有enemysetting 则用enemysetting 的icon 如果没有 则用自己的icon 如果都没有 则用默认icon
            EnemySetting enemySetting = DataTable.FindEnemySetting(peopledate.enemySettingId);
            if (enemySetting != null)
            {
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + enemySetting.SpecialPortrait);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                if (peopledate.talent == 3)
                {
                    sb.Append(peopledate.talent);
                    sb.Append('_');
                    sb.Append(peopledate.yuanSu);
                }
                else
                {
                    sb.Append(peopledate.talent);
                }
                string iconPath = ConstantVal.PeopleTouxiang + sb.ToString();
                Sprite talentSprite = ResourceManager.Instance.GetObj<Sprite>(iconPath);
                if (talentSprite == null)
                {
                    talentSprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + ConstantVal.defaultPortraitName);
                }
                icon.sprite = talentSprite;
            }
        }
    }

    /// <summary>
    /// 根据宗门等级生成一个候选弟子
    /// </summary>
    /// <returns></returns>
    public PeopleData GenerateCandidateStudentByZongMenLevel(GenerateCandidateStudentType type, Gender gender=Gender.None)
    {
        List<Rarity> rarityList = new List<Rarity>();
        rarityList = new List<Rarity> { Rarity.Fan, Rarity.Huang, Rarity.Xuan, Rarity.Di, Rarity.Tian };

        List<int> weightList = new List<int>();

        int zongMenBigLevel = (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel - 1) / 10 + 1;
        //switch (zongMenBigLevel)
        //{
        //    case 1:

        //        weightList = new List<int> { 40, 30, 15, 10, 5 };

        //        break;
        //    case 2:

        //        weightList = new List<int> { 20, 20, 30, 20, 10 };

        //        break;
        //    case 3:

        //        weightList = new List<int> { 10, 10, 30, 30, 20 };

        //        break;
        //    case 4:

        //        weightList = new List<int> { 0, 5, 25, 30, 40 };

        //        break;
        //    case 5:
        //        weightList = new List<int> { 0, 0, 10, 30, 60 };

        //        break;
        //}
        if(type==GenerateCandidateStudentType.AD)
        weightList = new List<int> { 30, 30, 28, 12, 0 };
        else
        {
            weightList = new List<int> { 30, 36, 30, 6, 0 };

        }
        int rarityIndex = CommonUtil.GetIndexByWeight(weightList);
        int rarity = (int)rarityList[rarityIndex];
        PeopleData p = RdmGenerateStudent(rarity, rarity, gender);
        return p;
    }
    /// <summary>
    /// 用广告刷新弟子
    /// </summary>
    public void ADBrushStudents()
    {  
        //山门招募上限就不刷了
        if (RoleManager.Instance._CurGameInfo.studentData.todayRecruitStudentNum >= ConstantVal.maxStudentRecruitNumPerDay)
        {
            PanelManager.Instance.OpenFloatWindow("已达上限，请等待刷新");
            return;
        }
        if (RoleManager.Instance._CurGameInfo.studentData.thisYearBrushStudentNum >= 3)
        {
            PanelManager.Instance.OpenFloatWindow("本年刷新次数已达上限");
            return;
        }
        if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.ZhaoMuLing, 1))
        {
            ItemSetting itemSetting=DataTable.table.TbItem.Get(((int)ItemIdType.ZhaoMuLing).ToString());
            PanelManager.Instance.OpenCommonHint(itemSetting.Name+"不够，是否前往获取？", () =>
            {
                PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2,ShopTag.LiBao);

            }, null);
            return;
        }

       StudentManager.Instance.OnSuccessfulADBrushStudent();
                  

        ADManager.Instance.WatchAD(ADType.ADBrushStudent);

    }
    /// <summary>
    /// 广告成功增加弟子
    /// </summary>
    public void OnSuccessfulADBrushStudent()
    {
        if (ItemManager.Instance.LoseItem((int)ItemIdType.ZhaoMuLing,1))
        {

            RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Clear();
            RoleManager.Instance._CurGameInfo.studentData.thisYearBrushStudentNum++;
            RoleManager.Instance._CurGameInfo.studentData.lastNewStudentYear = RoleManager.Instance._CurGameInfo.timeData.Year;
            RdmGenerate3CandidateStudents(GenerateCandidateStudentType.AD);
            //从大到小排序 如果最小不是地级则地级
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Count - 1; i++)
            {
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Count - 1 - i; j++)
                {
                    //前面的大于后面的，则二者交换
                    if (RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[j].studentRarity
                        > (RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[j + 1].studentRarity))
                    {
                        var temp = RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[j];
                        RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[j] = RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[j + 1];
                        RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[j + 1] = temp;

                    }
                }
            }
            if (RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[0].studentRarity < (int)Rarity.Xuan)
            {
                RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[0].studentRarity = (int)Rarity.Xuan;
            }
            EventCenter.Broadcast(TheEventType.GeneratedNewStudent);
#if !UNITY_EDITOR
        ArchiveManager.Instance.SaveArchive();
#endif
 
        }


    }

    /// <summary>
    /// 用钱刷新弟子
    /// </summary>
    public void MoneyBrushStudents()
    {
        //山门招募上限就不刷了
        if (RoleManager.Instance._CurGameInfo.studentData.todayRecruitStudentNum >= ConstantVal.maxStudentRecruitNumPerDay)
        {
            PanelManager.Instance.OpenFloatWindow("已达上限，请等待刷新");
            return;
        }

        if (RoleManager.Instance._CurGameInfo.studentData.thisYearBrushStudentNum >= 3)
        {
            PanelManager.Instance.OpenFloatWindow("本年刷新次数已达上限");
            return;
        }

        int zongMenBigLevel = (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel - 1) / 10 + 1;
        ulong cost = 10000 * (ulong)zongMenBigLevel;
        if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, cost))
        {
                    ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
            return;
        }
        ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, cost);
        RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Clear();
        RoleManager.Instance._CurGameInfo.studentData.thisYearBrushStudentNum++;
        RoleManager.Instance._CurGameInfo.studentData.lastNewStudentYear = RoleManager.Instance._CurGameInfo.timeData.Year;
        RdmGenerate3CandidateStudents(GenerateCandidateStudentType.ByMoney);

#if !UNITY_EDITOR
        ArchiveManager.Instance.SaveArchive();
#endif
 
    }


    /// <summary>
    /// 随机来个弟子
    /// </summary>
    /// <param name="rarity"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public PeopleData RdmGenerateStudent(int rarity,int quality,Gender gender=Gender.None)
    {
        int index = RandomManager.Next(1, 4);
        PeopleData p = new PeopleData();
        p.studentLevel = 1;

        p.onlyId = ConstantVal.SetId;
        if (gender == Gender.None)
            p.gender = RandomManager.Next(1, 3);
        else
            p.gender = (int)gender;
        p.name = RoleManager.Instance.rdmName((Gender)(int)p.gender);
        p.studentRarity = rarity;
        p.studentQuality = quality;
        RoleManager.Instance.RdmFace(p);
        //switch (index)
        //{
        //    //练丹师
        //    case 1:
        //        p = GenerateLianDanStudent(rarity, 1);
        //        break;
        //    //练器师
        //    case 2:
        //        p = GenerateEquipMakeStudent(rarity, 1);
        //        break;
        //    //练功师
        //    case 3:
        //        p = GenerateLianGongStudent(rarity, 1);
        //        break;
        //}
        p.socializationData = new SocializationData();
        p.socializationData.socialActivity = RandomManager.Next(0, 99);
        p.socializationData.xingGe = RdmXingGe();
        return p;
    }
    /// <summary>
    /// 生成内门弟子
    /// </summary>
    public PeopleData GenerateLianGongStudent(PeopleData p,Quality quality,YuanSuType yuanSuType)
    {
        p.propertyIdList.Clear();
        p.propertyList.Clear();
        p.curBattleProIdList.Clear();
        p.curBattleProList.Clear();
        List<int> weightList = new List<int>();
        switch (quality)
        {
            case Quality.Green:
                weightList = new List<int> { 50, 30, 20, 0, 0 };
                break;
            case Quality.Blue:
                weightList = new List<int> { 40, 30, 20, 10, 0 };
                break;
            case Quality.Purple:
                weightList = new List<int> { 20, 20, 20, 20, 20 };
                break;
            case Quality.Orange:
                weightList = new List<int> { 10, 15, 20, 25, 30 };
                break;
            case Quality.Gold:
                weightList = new List<int> { 0, 0, 20, 30, 50 };
                break;
        }
        List<int> qualityList = new List<int> { 1, 2, 3, 4, 5 };

        p.talent = (int)StudentTalent.LianGong;
        p.talentRarity = (int)quality;
        string proStr = ConstantVal.baseLianGongStudentPro;
        List<List<int>> baseBattleProList = CommonUtil.SplitCfg(ConstantVal.baseBattleProperty);

        List<List<int>> proList = CommonUtil.SplitCfg(proStr);
        List<int> haveValIdList = new List<int>();
        List<int> haveValValList = new List<int>();

        for (int i = 0; i < proList.Count; i++)
        {
            List<int> thePro = proList[i];
            haveValIdList.Add(thePro[0]);
            haveValValList.Add(thePro[1]);
        }
        for (int i = 0; i < baseBattleProList.Count; i++)
        {
            List<int> singlePro = baseBattleProList[i];
            int theId = singlePro[0];
            int theNum = singlePro[1];

            if (haveValIdList.Contains(theId))
            {
                int index = haveValIdList.IndexOf(theId);
                theNum = haveValValList[index];
            }
            int qualityIndex = CommonUtil.GetIndexByWeight(weightList);
            int proQuality = qualityList[qualityIndex];
                //RandomManager.Next(1, 6);
            //p.ProQualityList.Add(proQuality);

            theNum = Mathf.RoundToInt(theNum * StudentQualityAdd((Quality)proQuality));

            SinglePropertyData pro = new SinglePropertyData();
            pro.id = theId;
            pro.num = theNum;
            pro.quality = proQuality;
            if (theId == (int)PropertyIdType.MpNum)
            {
                pro.limit = 100;
            }
            else if (theId == (int)PropertyIdType.Hp)
            {
                pro.limit = pro.num;
            }

            p.propertyIdList.Add(theId);
            p.propertyList.Add(pro);

            SinglePropertyData battlePro = new SinglePropertyData();
            battlePro.id = theId;
            battlePro.num = theNum;
            battlePro.limit = pro.limit;
            battlePro.quality = pro.quality;
           

            p.curBattleProIdList.Add(theId);
            p.curBattleProList.Add(battlePro);
        }
  
        //初始技能暂定灵弹
        p.allSkillData = new AllSkillData();
        if(p.studentRarity==3 )
        {
            p.allSkillData.unlockedSkillPos = RandomManager.Next(1, 3);
        }
        else if (p.studentRarity > 3)
        {
            p.allSkillData.unlockedSkillPos = RandomManager.Next(1, 4);
        }
        else
        {
            p.allSkillData.unlockedSkillPos = 1;
        }
        SingleSkillData singleSkill = new SingleSkillData();
        singleSkill.skillId = (int)BattleManager.Instance.PuGongIdByYuanSu(yuanSuType);
        singleSkill.skillLevel = 1;
        p.allSkillData.skillList.Add(singleSkill);
        p.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);

        p.yuanSu = (int)yuanSuType;
        p.xueMai = new XueMaiData();
        for (int i = 1; i < 6; i++)
        {
            p.xueMai.xueMaiTypeList.Add((XueMaiType)i);
            p.xueMai.xueMaiLevelList.Add(0);
        }
        return p;
    }
 
    /// <summary>
    /// 招募弟子
    /// </summary>
    public void RecruitStudent(PeopleData p,RecruitStudentType type)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData existP = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (existP.onlyId == p.onlyId)
            {
                Debug.LogError("存在相同");
                return;
            }
        }
        if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, ConstantVal.cruitStudentNeedLingShiNum))
        {
            ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
            return;
        }

  

        //如果达到了最大弟子限制
        if (BuildingManager.Instance.CheckIfReachBuildingMaxNeiMenNumLimit())
        {
            
            PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.当前弟子人数已达上限请提升宗门等级));
            return;
            //打开剔除弟子面板
            //EventCenter.Broadcast(TheEventType.OpenReplaceStudentPanel, p);
        }
        //今年已增加足够弟子
        else if(RoleManager.Instance._CurGameInfo.studentData.thisYearRecruitedStudentNum>= RoleManager.Instance._CurGameInfo.studentData.thisYearRemainCanRecruitStudentNum)
        {
            
            PanelManager.Instance.OpenPanel<AddStudentRecruitNumADPanel>(PanelManager.Instance.trans_layer2);
            return;
        }
        else
        {
            ReallyRecruitStudent(p,type);
        }

        

    }
    /// <summary>
    /// 获取某类学生的数据
    /// </summary>
    public List<PeopleData> GetTypeStudent(StudentTalent studentTalentType)
    {
        List<PeopleData> res = new List<PeopleData>();
        //RepeatedField<PeopleData> fieldList = null;
        //switch (studentTalentType)
        //{
        //    case StudentType.LianDan:
        //        fieldList = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList;
        //        break;
        //    case StudentType.EquipMake:
        //        fieldList = RoleManager.Instance._CurGameInfo.StudentData.EquipMakeStudentList;
        //        break;
        //    case StudentType.LianGong:
        //        fieldList = RoleManager.Instance._CurGameInfo.StudentData.LianGongStudentList;
        //        break;
        //}
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if(p.talent==(int)studentTalentType)
            res.Add(p);
        }
        return res;
    }

    /// <summary>
    /// 找学生
    /// </summary>
    /// <param name="studentType"></param>
    /// <returns></returns>
    public PeopleData FindStudent(ulong onlyId)
    {
        //RepeatedField<PeopleData> fieldList = new RepeatedField<PeopleData>();

        //switch (talentType)
        //{
        //    case StudentTalent.LianGong:
        //        fieldList = RoleManager.Instance._CurGameInfo.StudentData.LianGongStudentList;
        //        break;
        //    case StudentTalent.LianDan:
        //        fieldList = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList;
        //        break;
        //    case StudentTalent.EquipMake:
        //        fieldList = RoleManager.Instance._CurGameInfo.StudentData.EquipMakeStudentList;
        //        break;
        //}
        if (RoleManager.Instance._CurGameInfo.playerPeople.onlyId == onlyId)
            return RoleManager.Instance._CurGameInfo.playerPeople;

        for (int i = 0; i <RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.onlyId == onlyId)
                return p;
        }
        return null;
    }


    /// <summary>
    /// 踢掉学生
    /// </summary>
    public void RemoveStudent(PeopleData toReplaced)
    {

        StudentStatusType studentStatusType = (StudentStatusType)(int)toReplaced.studentStatusType;
         
        if(studentStatusType== StudentStatusType.AtTeam)
        {
            PanelManager.Instance.OpenFloatWindow("已上阵，请先下阵");
            return;
  
        }
        else if(studentStatusType==StudentStatusType.DanFarmQuanLi
            || studentStatusType==StudentStatusType.DanFarmRelax
            || studentStatusType == StudentStatusType.DanFarmWork)
        {
            SingleDanFarmData danFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(toReplaced.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[toReplaced.zuoZhenDanFarmIndex];
            for(int i = 0; i < danFarmData.ZuoZhenStudentIdList.Count; i++)
            {
                ulong theId = danFarmData.ZuoZhenStudentIdList[i];
                if (theId == toReplaced.onlyId)
                {
                    danFarmData.ZuoZhenStudentIdList[i] = 0;
                    break;
                }
            }
        }
        else if(studentStatusType==StudentStatusType.AtExplore)
        {
            PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.正在秘境探险请先等待回来));
            return;
        }
        RoleManager.Instance._CurGameInfo.studentData.allStudentList.Remove(toReplaced);

        //所有弟子认识的人变化
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.socializationData.knowPeopleList.Contains(toReplaced.onlyId))
            {
                int index = p.socializationData.knowPeopleList.IndexOf(toReplaced.onlyId);
                int haoGan = p.socializationData.haoGanDu[index];
                string str = "";
              
     

                p.socializationData.knowPeopleList.RemoveAt(index);
                p.socializationData.haoGanDu.RemoveAt(index);
            }
        }
        RoleManager.Instance._CurGameInfo.studentData.allStudentList.Remove(toReplaced);
        ////在固定关卡 不用管
        //case StudentStatusType.AtFixedWorld:
        //    RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Remove(toReplaced);
        //    break;
        //case StudentStatusType.DanFarmWork:
        //     toReplaced.ZuoZhenDanFarmIndex



        //switch ((StudentType)toReplaced.StudentType)
        //{
        //    case StudentType.LianDan:
        //        RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Remove(toReplaced);
        //        break;
        //    case StudentType.EquipMake:
        //        RoleManager.Instance._CurGameInfo.StudentData.EquipMakeStudentList.Remove(toReplaced);
        //        break;
        //    case StudentType.LianGong:
        //        RoleManager.Instance._CurGameInfo.StudentData.LianGongStudentList.Remove(toReplaced);
        //        break;
        //}
        EventCenter.Broadcast(TheEventType.OnRemoveStudent);
        InitRedPoint();
    }

    /// <summary>
    /// 增加学生
    /// </summary>
    public void AddStudent(PeopleData p)
    {
        switch ((StudentTalent)(int)p.talent)
        {
          
            case StudentTalent.LianGong:
                p.studentCurEnergy = 100;
                break;
        }
        RoleManager.Instance._CurGameInfo.studentData.allStudentList.Add(p);
        InitRedPoint();

    }

    /// <summary>
    /// 真正招募
    /// </summary>
    void ReallyRecruitStudent(PeopleData p,RecruitStudentType type)
    {
        ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, ConstantVal.cruitStudentNeedLingShiNum);
        //ItemManager.Instance.LoseItem((int)ItemIdType.ZhaoJiLing, 1);
            RoleManager.Instance._CurGameInfo.studentData.thisYearRecruitedStudentNum++;

        AddStudent(p);

 
        //所有人遣散
        RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Remove(p);

        if (type == RecruitStudentType.ShanMen)
            RoleManager.Instance._CurGameInfo.studentData.todayRecruitStudentNum++;

        EventCenter.Broadcast(TheEventType.SuccessRecruit,p);

        PanelManager.Instance.OpenFloatWindow("招募成功");

   
        RoleManager.Instance._CurGameInfo.studentData.todayRecruitStudentNum++;
        TaskManager.Instance.TryAccomplishAllTask();
        TaskManager.Instance.GetDailyAchievement(TaskType.ZhaoMuDiZi, "1");
 
    }

    /// <summary>
    /// 看广告增加招募弟子人数限制
    /// </summary>
    public void ADAddRecruitStudentLimit()
    {

        RoleManager.Instance._CurGameInfo.studentData.thisYearWatchedADNum = true;
        RoleManager.Instance._CurGameInfo.studentData.thisYearRemainCanRecruitStudentNum+=2;
        EventCenter.Broadcast(TheEventType.AddedRecruitStudentNumLimit);
     }

 

    /// <summary>
    /// 得到经验值
    /// </summary>
    public void OnGetStudentExp(PeopleData p,int num)
    {
        int levelLimit = GetStudentLevelLimit(p);
        bool haveCanTestTalentStudent = false;
        bool haveCanBreakThroughStudent = false;
        //没满级
        if (p.studentLevel < levelLimit)
        {
            //int totalLimit= StudentManager.Instance.GetStudentExpTotalLimit((Rarity)p.StudentRarity);
            if (p.talent ==(int)StudentTalent.LianGong)
            {
                ulong beforeXiuWei = p.curXiuwei;
                TrainSetting curTrainSetting = DataTable._trainList[p.trainIndex];
                ulong xiuweiNeed = curTrainSetting.XiuWeiNeed.ToUInt64();
                p.curXiuwei += (ulong)num;
                //经验满了
                if (p.curXiuwei >= xiuweiNeed)
                {
                    //p.CurXiuwei = xiuweiNeed;
                    haveCanBreakThroughStudent = true;
                    //本来就满了
                    if (beforeXiuWei < xiuweiNeed)
                    {
                        ////可能领悟技能 可能强化装备
                        //if (p.allSkillData.equippedSkillIdList.Count == 1)
                        //{
                        //    int lingWuSkillRate = 30;
                        //    int lingWuSkillIndex = RandomManager.Next(0, 100);
                        //    if (lingWuSkillIndex < lingWuSkillRate)
                        //    {
                        //         //如果已在练功房看中了功法
                        //        List<SingleDanFarmData> cangKuList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.CangKu);
                        //        for (int i = 0; i < cangKuList.Count; i++)
                        //        {
                        //            SingleDanFarmData cangKu = cangKuList[i];
                        //            for (int j = 0; j < cangKu.StudentUseCangKuDataList.Count; j++)
                        //            {
                        //                SingleStudentUseCangKuData useCangKuData = cangKu.StudentUseCangKuDataList[j];
                        //                if (useCangKuData.StudentOnlyId == p.onlyId
                        //                && useCangKuData.StudentGoCangKuNeedType == (int)StudentGoCangKuNeedType.StudySkill)
                        //                {
                        //                    return;
                        //                }
                        //            }
                        //        }
                             
                        //        List<SkillIdType> candidateList= ConstantVal.BigSkillIdListByYuanSu((YuanSuType)p.yuanSu);
                        //        //领悟到了技能
                        //        int skillIndex = RandomManager.Next(0, candidateList.Count);
                        //        int skillId = (int)candidateList[skillIndex];
                        //        SingleSkillData skill= SkillManager.Instance.AddSkill(skillId,p);
                        //        SkillManager.Instance.EquipSkill(p, skill);
                        //        //弹窗通知
                        //        PanelManager.Instance.OpenPanel<StudentGetNewSkillPanel>(PanelManager.Instance.trans_layer2, p, skill);


                        //        List<DialogSetting> candidateContent2 = DataTable.FindDialogSettingListByType((int)DialogContentType.StudentStudySkill);
                        //        string str = candidateContent2[RandomManager.Next(0, candidateContent2.Count)].content;
                        //        str = str.Replace("x", DataTable.FindSkillSetting(skill.skillId).name);
                        //        ChatManager.Instance.AddChat(p, str);
                             
                        //    }
                        //}
                        //else
                        //{
                        //    //否则 可能升级技能
                        //    SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[1], p.allSkillData);
                        //    int upgradeRate = GetSkillUpgradeRate(p);
                        //    int upgradeSkillIndex = RandomManager.Next(0, 100);
                        //    if (upgradeSkillIndex < upgradeRate)
                        //    {
                        //        //通知
                        //        if ((singleSkillData.skillLevel + 1) % 5 != 0)
                        //        {
                        //            singleSkillData.skillLevel++;
                        //            SkillSetting setting = DataTable.FindSkillSetting(singleSkillData.skillId);
                        //            PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "潜心修炼，悟有所得，将" + setting.name + "功法等级提升至Lv" + singleSkillData.skillLevel);
                        //        }

                        //    }
                        //}

                        //if (p.curEquipItem != null)
                        //{
                        //    int equipUpgradeRate = GetEquipIntenseRate(p);
                        //    int equipUpgradeIndex = RandomManager.Next(0, 100);
                        //    if (equipUpgradeIndex < equipUpgradeRate
                        //        &&p.curEquipItem.equipProtoData.curLevel<25
                        //        &&string.IsNullOrWhiteSpace(EquipmentManager.Instance.JudgeIfEquipIntenseSatisfyLevelCondition(p.curEquipItem.equipProtoData.curLevel)))
                        //    {
                        //        EquipmentManager.Instance.OnIntenseEquip(p.curEquipItem.onlyId, null, p);
                        //        EquipmentSetting equipmentSetting = DataTable.FindEquipSetting(p.curEquipItem.equipProtoData.settingId);
                        //        PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "将" + equipmentSetting.name + "强化至Lv" + p.curEquipItem.equipProtoData.curLevel);

                        //    }
                        //}

                   
                    }
                }
               
            }
            else
            {
                if (p.studentLevel > 0 && p.studentLevel <= DataTable._studentUpgradeList.Count)
                {
                    int limit = DataTable._studentUpgradeList[p.studentLevel-1].NeedExp.ToInt32();
                    p.studentCurExp += num;
                    if (p.studentCurExp >= limit)
                    {
                        p.studentCurExp = limit;

                        if (p.talent == (int)StudentTalent.None)
                        {
                            haveCanTestTalentStudent = true;
                        }
                        else
                        {
                            haveCanBreakThroughStudent = true;
                        }
                    }
                }
            }
            
        }
        //练功弟子
        if (haveCanTestTalentStudent)
        {

            //显示已满
            EventCenter.Broadcast(TheEventType.RareStudentExpFull);
        }
        if (haveCanBreakThroughStudent)
        {

            //显示已满
            EventCenter.Broadcast(TheEventType.TalentStudentExpFull);
        }

        RefreshRedPointShow(p);

        EventCenter.Broadcast(TheEventType.OnGetStudentExp);
    }
    /// <summary>
    /// 练功弟子能否突破
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool CheckIfLianGongStudentCanBreakThrough(PeopleData p)
    {
        if (p.talent == (int)StudentTalent.LianGong)
        {
            if(p.trainIndex < StudentManager.Instance.GetStudentLevelLimit(p))
            {
                ulong beforeXiuWei = p.curXiuwei;
                TrainSetting curTrainSetting = DataTable._trainList[p.trainIndex];
                ulong xiuweiNeed = curTrainSetting.XiuWeiNeed.ToUInt64();
                if (beforeXiuWei >= xiuweiNeed)
                    return true;
            }
            return false;
        }
        return false;
    }
    public void RefreshRedPointShow(PeopleData p)
    {
    
        int levelLimit = GetStudentLevelLimit(p);
        int curLevel = 0;
        bool show = false;
        if (p.talent == (int)StudentTalent.LianGong)
            curLevel = p.trainIndex;
        else
            curLevel = p.studentLevel;
        //没满级
        if (curLevel < levelLimit)
        {
            //int totalLimit= StudentManager.Instance.GetStudentExpTotalLimit((Rarity)p.StudentRarity);
            if (p.talent == (int)StudentTalent.LianGong)
            {
                ulong beforeXiuWei = p.curXiuwei;
                TrainSetting curTrainSetting = DataTable._trainList[p.trainIndex];
                ulong xiuweiNeed = curTrainSetting.XiuWeiNeed.ToUInt64();
                //经验满了
                if (p.curXiuwei >= xiuweiNeed)
                {
                    show = true;
                }

            }else if (p.talent == (int)StudentTalent.None)
            {
                if (p.studentLevel > 0 && p.studentLevel <= DataTable._studentUpgradeList.Count)
                {
                    StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                    int expLimit = setting.NeedExp.ToInt32();
                    if (p.studentCurExp >= expLimit)
                    {
                        int cost = 1500;//基础是1500
                        cost = Mathf.RoundToInt(cost * ConstantVal.GetValAddByRarity((Rarity)(int)p.studentRarity));
                        if (ItemManager.Instance.FindLingShiCount() >= cost)
                        {
                            show = true;
                        }
                    }
                }
            }
            else
            {
                if (p.studentLevel > 0 && p.studentLevel <= DataTable._studentUpgradeList.Count)
                {
                    bool itemEnough = true;
                    int limit = DataTable._studentUpgradeList[p.studentLevel - 1].NeedExp.ToInt32();
                    List<List<int>> matList = CommonUtil.SplitCfg(DataTable._studentUpgradeList[p.studentLevel - 1].NeedMat);
                    for (int i = 0; i < matList.Count; i++)
                    {
                        List<int> singleMat = matList[i];
                        int id = singleMat[0];
                        int num = singleMat[1];
                        if (!ItemManager.Instance.CheckIfItemEnough(id, (ulong)num))
                        {
                            itemEnough = false;
                            break;
                        }
                    }
                    if (p.studentCurExp >= limit&&itemEnough)
                    {  
                        show = true;
                    }
                }
            }
        }
        if (p.talent == (int)StudentTalent.LianGong)
        {
            bool canUpgradeSkill = false;

            if (p.allSkillData != null && p.allSkillData.equippedSkillIdList.Count >= 2)
            {
                int skillId = p.allSkillData.equippedSkillIdList[1];
                SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(skillId, p.allSkillData);
                List<ItemData> consumeList = SkillManager.Instance.GetSkillUpgradeConsume(singleSkillData);
                bool itemEnough = true;
                for (int i = 0; i < consumeList.Count; i++)
                {
                    ItemData single = consumeList[i];

                    if (!ItemManager.Instance.CheckIfItemEnough(single.settingId, (ulong)single.count))
                    {

                        itemEnough = false;
                        break;
                    }

                }
                if (itemEnough)
                {
                    List<SkillUpgradeSetting> settingList = DataTable.FindSkillUpgradeListBySkillId(singleSkillData.skillId);

                    if (singleSkillData.skillLevel < settingList.Count)
                    {
                        canUpgradeSkill = true;
                    }
                }
            }

            RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Student_InfoBigStudentView_SkillUpgrade, (int)(ulong)p.onlyId, canUpgradeSkill);

            bool canUpgradeXueMai = false;
            int xueMailimit = XueMaiManager.Instance.limitLevel(p);
            for (int i = 1; i < (int)XueMaiType.End; i++)
            {

                XueMaiType type = (XueMaiType)i;
                XueMaiUpgradeSetting xueMaiUpgradeSetting = DataTable.FindXueMaiUpgradeSettingByType((int)type);

                int index = p.xueMai.xueMaiTypeList.IndexOf(type);
                int curXueMaiLevel = p.xueMai.xueMaiLevelList[index];
                if (curXueMaiLevel < xueMailimit)
                {
                    List<List<List<int>>> itemNeedParam = CommonUtil.SplitThreeCfg(xueMaiUpgradeSetting.NeedItem);
                    List<List<int>> itemNeed = itemNeedParam[curXueMaiLevel];
                    bool singleItemEnough = true;
                    for (int j = 0; j < itemNeed.Count; j++)
                    {
                        List<int> single = itemNeed[j];
                        int id = single[0];
                        int num = single[1];
                        if (!ItemManager.Instance.CheckIfItemEnough(id, (ulong)num))
                        {
                            singleItemEnough = false;
                            break;
                        }
                    }
                    if (singleItemEnough)
                    {
                        canUpgradeXueMai = true;
                        break;

                    }

                }



            }
            RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Student_InfoBigStudentView_XueMai, (int)(ulong)p.onlyId, canUpgradeXueMai);

        }


        RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Student_InfoBigStudentView, (int)(ulong)p.onlyId, show);

        EventCenter.Broadcast(TheEventType.RefreshStudentRedPoint,p);
    }

    ///// <summary>
    ///// 获取装备得到强化的概率
    ///// </summary>
    ///// <param name="p"></param>
    ///// <returns></returns>
    //public int GetEquipIntenseRate(PeopleData p)
    //{
    //    ItemData item= p.curEquipItem;
    //    int rate = 0;

    //    if (item.equipProtoData.curLevel < 25)
    //    {
    //        ItemSetting setting = DataTable.FindItemSetting(item.settingId);
    //        Rarity rarity = (Rarity)setting.rarity.ToInt32();
    //        int jingJieLevel = p.trainIndex / 30 + 1;//境界等级
    //        int maxLevel = 0;
    //        //境界1 弟子只能强化小于等于自己境界的装备 凡级最高为25 升级概率15% TODO测试最高级

    //        if ((int)rarity <= jingJieLevel)
    //        {
    //            rate = 80 / (int)rarity;
    //            maxLevel = 25;
    //        }
    //        else
    //        {
    //            rate = 0;
    //            maxLevel = 1;
    //        }

    //    }
    //    else
    //    {
    //        rate = 0;
    //    }
   
    //    return rate;
    //}
    /// <summary>
    /// 获取技能升级概率
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public int GetSkillUpgradeRate(PeopleData p)
    {
        int maxSkillLevel = (p.trainIndex / 30 + 1) * 10;
        SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[1], p.allSkillData);
        int rate = 0;
        List<SkillUpgradeSetting> upgradeList = DataTable.FindSkillUpgradeListBySkillId(singleSkillData.skillId);
        if (singleSkillData.skillLevel < upgradeList.Count)
        {
            rate = 50 / (maxSkillLevel / 10);
        }
        return rate;
    }

    /// <summary>
    /// 弟子突破
    /// </summary>
    /// <param name="p"></param>
    public void BreakThrough(PeopleData p)
    {
        p = FindStudent(p.onlyId);
        int curLevel = 0;
        if (p.talent == (int)StudentTalent.LianGong
            ||p.onlyId==RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
            curLevel = p.trainIndex;
        else
            curLevel = p.studentLevel;
        if (curLevel >= GetStudentLevelLimit(p))
        {
             return;
        }
        if (p.talent != (int)StudentTalent.LianGong)
        {
            if (p.studentLevel <= 0 || p.studentLevel > DataTable._studentUpgradeList.Count)
            {
                return;
            }

            StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
            int needExp = setting.NeedExp.ToInt32();
            if (p.studentCurExp < needExp)
            {
                 return;
            }
           
            p.studentLevel++;
            p.studentCurExp -= needExp;
            if (p.studentCurExp <= 0)
                p.studentCurExp = 0;

            for (int i = 0; i < p.propertyList.Count; i++)
            {
                Quality proQuality = (Quality)(int)p.propertyList[i].quality;
                int valEquip = StudentBreakThroughAdd((StudentTalent)(int)p.talent, proQuality);
                p.propertyList[i].num += valEquip;
                if (p.propertyList[i].num >= 300)
                {
                    p.propertyList[i].num = 300;
                }
            }
            TaskManager.Instance.GetAchievement(AchievementType.StudentUpgradeCount, "");
            TaskManager.Instance.GetDailyAchievement(TaskType.StudentUpgradeCount, "1");
            TaskManager.Instance.TryAccomplishAllTask();
            EventCenter.Broadcast(TheEventType.StudentBreakthroughSuccess, p);

             AuditionManager.Instance.PlayVoice(AudioClipType.BreakThrough);
          }
        else
        {
            OnBreakThrough(p);
        }



        ////修武
        //if (p.Talent ==(int)StudentTalent.LianGong)
        //{
        //}
        //else
        //{


        //}
        RefreshRedPointShow(p);
        EventCenter.Broadcast(TheEventType.RefreshStudentRedPoint, p);
        EventCenter.Broadcast(TheEventType.StudentStatusChange, p);
     
#if !UNITY_EDITOR
                ArchiveManager.Instance.SaveArchive();
#endif
    }

    /// <summary>
    /// 修武弟子突破
    /// </summary>
    public void OnBreakThrough(PeopleData p)
    {
    
        long before =RoleManager.Instance.CalcZhanDouLi(p);
        int trainIndex = p.trainIndex;
        //当前等级小于上限 可以突破
        if(p.trainIndex>= GetStudentLevelLimit(p))
        {
            return;
        }
        if (trainIndex < DataTable._trainList.Count - 1)
        {
            TrainSetting curTrainSetting = DataTable._trainList[trainIndex];
            TrainSetting nextTrainSetting = DataTable._trainList[trainIndex + 1];

            // ulong lingShiNeed = curTrainSetting.lingShiNeed.ToUInt64();
            ulong xiuweiNeed = curTrainSetting.XiuWeiNeed.ToUInt64();

            if (p.curXiuwei < xiuweiNeed)
            {
                //PanelManager.Instance.OpenFloatWindow("修为不够");
                return;
            }
            //成功与否
            int rate =RoleManager.Instance.GetSuccessRate(p);
            int curRdmIndex = RandomManager.Next(0, 101);
            //消耗材料
            //ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, lingShiNeed);
            //DeProperty(PropertyIdType.Tili, -30);
            //服药上限归零
             p.curEatedDanNum = 0;
            //调息丹服用数量归零
            p.curEatedTiaoXiDanNum = 0;
            //成功
            if (curRdmIndex <= rate)
            {
                AuditionManager.Instance.PlayVoice(AudioClipType.BreakThrough);
                List<List<int>> proChange = CommonUtil.SplitCfg(nextTrainSetting.ProChange);


                List<SinglePropertyData> addedProList = new List<SinglePropertyData>();//加了哪些属性
                //体力够不够


                p.trainIndex++;
                p.nextBreakThroughAdd = 0;
                for (int i = 0; i < proChange.Count; i++)
                {
                    List<int> singlePro = proChange[i];
                    int proId = singlePro[0];
                    int proNum = singlePro[1];
                  
                    //根据品质
                    SinglePropertyData mypro = RoleManager.Instance.FindProperty(proId, p);
                    if (p.isPlayer)
                        mypro.quality = 5;
                    proNum = Mathf.RoundToInt(ConstantVal.GetValAddByRarity((Rarity)(int)mypro.quality) * proNum/1.8f);
                    if (proNum > 0)
                    {
                    
                        RoleManager.Instance.AddProperty((PropertyIdType)proId, proNum,p);
                        SinglePropertyData pro = new SinglePropertyData();
                        pro.id = proId;
                        pro.num = proNum;
                        
                        addedProList.Add(pro);
                    }
                }
                RoleManager.Instance.RefreshBattlePro(p);

                //发消息给UI
                long after =RoleManager.Instance.CalcZhanDouLi(p);
                EventCenter.Broadcast(TheEventType.SuccessBreakThrough,p, addedProList, before, after);

                TaskManager.Instance.GetDailyAchievement(TaskType.StudentUpgradeCount, "1");

                TaskManager.Instance.GetAchievement(AchievementType.StudentUpgradeCount, "");


         


                TaskManager.Instance.TryAccomplishAllTask();
                EventCenter.Broadcast(TheEventType.StudentBreakthroughSuccess, p);

 

               }
            else
            {
                //curTrainSetting.xiuWeiNeed
                //如果是当前大境界第一阶 则无需损失修为
                //if ((p.CurTrainIndex+1) / 10 != 1)
                //{
                //    TrainSetting foreTrainSetting = DataTable._trainList[p.CurTrainIndex-1];

                //    int offset = curTrainSetting.xiuWeiNeed.ToInt32() - foreTrainSetting.xiuWeiNeed.ToInt32();
                //    p.CurXiuwei -= (ulong)offset;
                //    p.CurTrainIndex--;

                //}
                ////else
                ////{

                ////}
               
                TrainSetting foreTrainSetting = DataTable._trainList[p.trainIndex - 1];
                int offset = curTrainSetting.XiuWeiNeed.ToInt32() - foreTrainSetting.XiuWeiNeed.ToInt32();
                p.curXiuwei -= (ulong)offset;
                int giantLevel = RoleManager.Instance.GiantLevel(p);// (p.CurTrainIndex / 30 + 1);
                int rateAdd = 25 - giantLevel * 4;
                PanelManager.Instance.OpenFloatWindow("突破失败 损失修为，下次突破成功率增加"+ rateAdd+"%");
                p.nextBreakThroughAdd += rateAdd;
                EventCenter.Broadcast(TheEventType.FailStudentBreakThrough);
 
 
            }




        }
        //long after = CalcZhanDouLi();
        //PanelManager.Instance.OpenZhanDouLiChangePanel(before, after);

    }

    /// <summary>
    /// 元神受损时间 最低9分钟 最高
    /// </summary>
    /// <returns></returns>
    public long YuanShenShouSunTime(PeopleData p)
    {
        long second = 0;
        if (p.trainIndex < 20)
            second = 0;
        else if(p.trainIndex<60)
            second = (long)(60 * 60 * 0.15f * (p.trainIndex) / 10);
        else if (p.trainIndex < 90)
            second = (long)(60 * 60 * 0.3f * (p.trainIndex) / 10);
        else if (p.trainIndex < 120)
            second = (long)(60 * 60 * 1f * (p.trainIndex) / 10);
        else
            second = (long)(60 * 60 * 2f * (p.trainIndex) / 10);
        float deBaiFen = 0;
        //for (int i = 0; i < curGameInfo.itemList.Count; i++)
        //{
        //    ItemData data = curGameInfo.itemList[i];
        //    if (data.oldXinWuData != null && data.oldXinWuData.oldXinWuSetting.Function.Type == OldXinWuFunctionType.DeYuanShenShouSunTime)
        //    {
        //        deBaiFen += data.oldXinWuData.longParam;
        //    }
        //}
        if (LianDanManager.Instance.FindMyFarmNum((int)DanFarmIdType.BiXieJinLian)>0)
        {
            deBaiFen = ConstantVal.biXieJinLianDeBaiFen;

            if(LianDanManager.Instance.FindMyFarmNum((int)DanFarmIdType.YouBiBaiLian)>0
                && LianDanManager.Instance.FindMyFarmNum((int)DanFarmIdType.JingShiQingLian) > 0)
            {
                deBaiFen +=ConstantVal.sanLianBingDiDeBaiFen;
            }

        }
        second = (long)Mathf.Round(second * (1 - deBaiFen * 0.01f));

        return second;
    }


    /// <summary>
    /// 学生突破战斗属性增加
    /// </summary>
    /// <returns></returns>
    public List<SinglePropertyData> StudentBreakThroughBattleProAdd()
    {
        List<SinglePropertyData> proList = new List<SinglePropertyData>();
        //string theStr = "";
        //switch (quality)
        //{
        //    case Rarity.Huang:
        //        theStr = ConstantVal.LianGongUpgradeAdd_huang;
        //        break;
        //    case Rarity.Xuan:
        //        theStr = ConstantVal.LianGongUpgradeAdd_xuan;
        //        break;
        //    case Rarity.Di:
        //        theStr = ConstantVal.LianGongUpgradeAdd_di;
        //        break;
        //    case Rarity.Tian:
        //        theStr = ConstantVal.LianGongUpgradeAdd_tian;
        //        break;
        //}
        List<List<int>> theList = CommonUtil.SplitCfg(ConstantVal.LianGongStudentPropertyPerLevelAdd);
        for(int i = 0; i < theList.Count; i++)
        {
            SinglePropertyData pro = new SinglePropertyData();
            pro.id = theList[i][0];
            int theNum = theList[i][1];
            pro.num = theNum;
            proList.Add(pro);
        }
        return proList;
    }

    /// <summary>
    /// 学生突破属性增加
    /// </summary>
    /// <returns></returns>
    public int StudentBreakThroughAdd(StudentTalent studentType,Quality quality)
    {
        //switch (studentType)
        //{
        //    case StudentTalent.EquipMake:
        //        return Mathf.RoundToInt(StudentQualityAdd(quality) * ConstantVal.workStudentPropertyPerLevelAdd);
        //    case StudentTalent.LianDan:

        //}
        if(studentType!=StudentTalent.LianGong)
        return Mathf.RoundToInt(StudentQualityAdd(quality) * ConstantVal.workStudentPropertyPerLevelAdd);

        return 0;
    }

    /// <summary>
    /// 学生颜色 颜色加成应该只影响单条属性 单条属性颜色加成是不一样的
    /// </summary>
    /// <returns></returns>
    public float StudentQualityAdd(Quality quality)
    {
        float res = (1 + ((int)quality - 1) * 0.2f);
        return res;
    }

    /// <summary>
    /// 弟子经验和经验上限
    /// </summary>
    /// <returns></returns>
    public List<int> FindStudentExpAndLimit(PeopleData p)
    {
        List<int> res = new List<int>();
        if(p.studentLevel > 0 && p.studentLevel <= DataTable._studentUpgradeList.Count)
        {
            StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
            int expLimit = setting.NeedExp.ToInt32();
            int curExp = p.studentCurExp;
            res.Add(curExp);
            res.Add(expLimit);
        }
        else
        {
            res.Add(0);
            res.Add(0);
        }
        return res;
    }

    ///// <summary>
    ///// 当前带进主线的弟子
    ///// </summary>
    ///// <returns></returns>
    //public List<PeopleData> GetCurMainWorldStudentList()
    //{
    //    List<PeopleData> res = new List<PeopleData>();
    //    for(int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Count; i++)
    //    {
    //        PeopleData p = RoleManager.Instance._CurGameInfo.StudentData.AllStudentList[i];
    //        if (p.StudentPrepareToWorld)
    //        {
    //            res.Add(p);
    //        }
    //    }
    //    return res;
    //}

   
  

    /// <summary>
    /// 弟子等级上限
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public int GetStudentLevelLimit(PeopleData p)
    {
        int res = 0;
        if (p.onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
            res = 149;
        else
        {
            if (p.talent == (int)StudentTalent.LianGong)
            {
                switch ((Rarity)(int)p.studentRarity)
                {
                    case Rarity.Fan:
                        res = 29;
                        break;
                    case Rarity.Huang:
                        res = 59;
                        break;
                    case Rarity.Xuan:
                        res = 89;
                        break;
                    case Rarity.Di:
                        res = 119;
                        break;
                    case Rarity.Tian:
                        res = 149;
                        break;
                }
            }
            else
            {
                switch ((Rarity)(int)p.studentRarity)
                {
                    case Rarity.Fan:
                        res = 10;
                        break;
                    case Rarity.Huang:
                        res = 20;
                        break;
                    case Rarity.Xuan:
                        res = 30;
                        break;
                    case Rarity.Di:
                        res = 40;
                        break;
                    case Rarity.Tian:
                        res = 50;
                        break;
                }
            }
        }
       
        res += p.trainIndex * 10;
        if (res >= DataTable._trainList.Count)
            res = DataTable._trainList.Count - 1;
        return res;
    }




    /// <summary>
    /// 裸弟子增加每周经验
    /// </summary>
    public void NakedStudentAddExp()
    {

        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent == (int)StudentTalent.None)
            {
                int val = RandomManager.Next(2, 6);

                OnGetStudentExp(p, val);
                //if(p.studentcu )

                //if (p.StudentCurExp >= 120)
                //{
                //    p.StudentCurExp = 120;                    
                //}
            }
            //生产弟子增加生产经验
            else if(p.talent!=(int)StudentTalent.LianGong)
            {
                //int maxLevel = DataTable._studentUpgradeList.Count;
                //if (p.StudentLevel < maxLevel)
                //{
                int val = RandomManager.Next(10, 20);
                //如果坐镇 则经验更多
                if (p.studentStatusType == (int)StudentStatusType.DanFarmWork
                    || p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi
                   || p.studentStatusType == (int)StudentStatusType.DanFarmRelax)
                {
                    SingleDanFarmData danFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);
                    if (danFarmData != null && p.talent == danFarmData.TalentType)
                    {
                        val = RandomManager.Next(20, 40);
                    }
                }
                OnGetStudentExp(p, val);
            }
            else if (p.talent == (int)(StudentTalent.LianGong))
            {
                //int xiuWei= RandomManager.Next(2, 4)*RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel;
                ////TrainSetting curTrainSetting = DataTable._trainList[p.CurTrainIndex];
                ////ulong xiuweiNeed = curTrainSetting.xiuWeiNeed.ToUInt64();

                //OnGetStudentExp(p, xiuWei);

         
            }


        }
  

    }
    /// <summary>
    /// 天赋测试 TODO防止读档 cost在这里不阻挡 真正endow的时候消耗cost
    /// </summary>
    /// <param name="p"></param>
    public void OnTalentTest(PeopleData p,bool ad=false,bool tianJi=false)
    {
   
        

        //觉醒一个天赋
        List<int> candidateTalents = new List<int>
        {
            1,2,3,4,5,6,7,8
        };
        //天赋权重
        List<int> talentWeight = new List<int> { 1, 1, 1, 1, 1,1,1,1 };

        //觉醒一个天赋
        List<int> candidateTalentQuality = new List<int>
        {
            1,2,3,4,5
        };
        List<int> qualityweightList = new List<int> { 15, 25, 30, 20, 10 };

        List<StudentTalent> choosedCandidateList = new List<StudentTalent>();
        List<Quality> choosedCandidateQualityList = new List<Quality>();
        List<YuanSuType> choosedYuanSuList = new List<YuanSuType>();//元素

        if (p.lastAppearTalentList.Count == 3)
        {
            for(int i=0;i< 3; i++)
            {
                choosedCandidateList.Add((StudentTalent)(int)p.lastAppearTalentList[i]);
                choosedCandidateQualityList.Add((Quality)(int)p.lastAppearTalentQualityList[i]);
                choosedYuanSuList.Add((YuanSuType)(int)p.lastAppearYuanSuList[i]);
            }
        }
        else
        {
            //随机3个天赋
            for (int i = 0; i < 3; i++)
            {
                int index = CommonUtil.GetIndexByWeight(talentWeight);
                StudentTalent choosedTalent = (StudentTalent)candidateTalents[index];
                choosedCandidateList.Add(choosedTalent);
                talentWeight.RemoveAt(index);
                candidateTalents.RemoveAt(index);

                int qualityIndex = CommonUtil.GetIndexByWeight(qualityweightList); //RandomManager.Next(0, candidateTalentQuality.Count);
                choosedCandidateQualityList.Add((Quality)candidateTalentQuality[qualityIndex]);

                YuanSuType yuanSuType = YuanSuType.None;
                if (choosedTalent == StudentTalent.LianGong)
                {
                    yuanSuType = (YuanSuType)RandomManager.Next(1, (int)YuanSuType.End);
                }
                choosedYuanSuList.Add(yuanSuType);
            }
        }
        if(tianJi&&
            !choosedCandidateQualityList.Contains(Quality.Gold))
        {
            choosedCandidateQualityList[0] = Quality.Gold;
        }
   
        StudentTalent guideTalent = StudentTalent.None;
        //没测试过
        if (TaskManager.Instance.FindAchievement(AchievementType.TianFuJueXingNum).ToInt32() == 0)
        {
            guideTalent = StudentTalent.DuanZhao;
       
        }
        else if (TaskManager.Instance.FindAchievement(AchievementType.TianFuJueXingNum).ToInt32() == 1)
        {
            guideTalent = StudentTalent.LianGong;

        }
        bool haveValid = false;
        if (guideTalent != StudentTalent.None)
        {
            for (int i = 0; i < choosedCandidateList.Count; i++)
            {
                StudentTalent talent = choosedCandidateList[i];
                if (talent == guideTalent)
                {
                    haveValid = true;
                    
                }
                choosedCandidateQualityList[i] = Quality.Gold;
            }
            //if (!haveValid)
            //{
            //    choosedCandidateList[0] = StudentTalent.EquipMake;
            //}
        }
        if (guideTalent != StudentTalent.None)
        {
            choosedCandidateList[0] = guideTalent;
            choosedYuanSuList[0]= (YuanSuType)RandomManager.Next(1, (int)YuanSuType.End);
        }
        //for(int i = 0; i < choosedCandidateList.Count; i++)
        //{
        //    StudentTalent talent = choosedCandidateList[i];
        //    YuanSuType yuanSuType = YuanSuType.None;
        //    if (talent == StudentTalent.LianGong)
        //    {
        //        yuanSuType = (YuanSuType)RandomManager.Next(1, (int)YuanSuType.End);
        //    }
        //    choosedYuanSuList.Add(yuanSuType);
        //}

        EventCenter.Broadcast(TheEventType.Appear3Talent, choosedCandidateList, choosedCandidateQualityList, choosedYuanSuList);
        //引导
        if (guideTalent == StudentTalent.DuanZhao)
        {
            PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting(10002));

        }else if(guideTalent == StudentTalent.LianGong)
        {
            PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting(10003));
        }
        // StudentTalent choosedTalent = (StudentTalent)candidateTalents[index];
        //EndowTalentToStudent(p, choosedTalent);

        //反作弊存档：
        p.lastAppearTalentList.Clear();
        p.lastAppearTalentQualityList.Clear();
        p.lastAppearYuanSuList.Clear();
        for (int i = 0; i < choosedCandidateList.Count; i++)
        {
            p.lastAppearTalentList.Add((int)choosedCandidateList[i]);
            p.lastAppearTalentQualityList.Add((int)choosedCandidateQualityList[i]);
            p.lastAppearYuanSuList.Add((int)choosedYuanSuList[i]);
        }
#if !UNITY_EDITOR
        ArchiveManager.Instance.SaveArchive();
#endif
    }

    /// <summary>
    /// 给弟子天赋赋值
    /// </summary>
    public void EndowTalentToStudent(PeopleData p, StudentTalent talent,Quality quality,YuanSuType yuanSuType)
    {
        int cost = 1500;//基础是1500

        cost = Mathf.RoundToInt(cost * ConstantVal.GetValAddByRarity((Rarity)(int)p.studentRarity));

        if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, (ulong)cost))
        {
                     ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
            return;
        }
        ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)cost);
        p.studentLevel = 1;
        p.studentCurExp = 0;
        p.curXiuwei = 0;
        p.trainIndex = 0;
        p.propertyIdList.Clear();
        p.propertyList.Clear();
        p.curEatedDanNum = 0;
        p.talentRarity = (int)quality;
        p.talent = (int)talent;
        List<int> proIdList = new List<int>();
        List<int> proNumList = new List<int>();
        List<int> qualityList = new List<int>();

        int id = -1;
        int num = (int)ConstantVal.workStudentInitProperty;
        int proQuality = (int)quality;
        num = Mathf.RoundToInt(num * ConstantVal.GetValAddByRarity((Rarity)proQuality));

        switch (talent)
        {
            case StudentTalent.LianJing://炼丹师 影响炼丹品质
                id=(int)PropertyIdType.JieDan;
                proIdList.Add(id);
                proNumList.Add(num);
                qualityList.Add(proQuality);
                break;
            case StudentTalent.DuanZhao://炼器师 影响炼器品质
                id = (int)PropertyIdType.ShiWu;
                proIdList.Add(id);
                proNumList.Add(num);
                qualityList.Add(proQuality);
                break;
            case StudentTalent.LianGong://内门弟子
                 GenerateLianGongStudent(p, quality, yuanSuType);
                break;
            case StudentTalent.CaiKuang://采矿
                id = (int)PropertyIdType.CaiKuang;
                proIdList.Add(id);
                proNumList.Add(num);
                qualityList.Add(proQuality);
                break;
            case StudentTalent.ChaoYao://造化 影响灵树产量
                id = (int)PropertyIdType.ZaoHua;
                proIdList.Add(id);
                proNumList.Add(num);
                qualityList.Add(proQuality);
                break;
            case StudentTalent.JingWen://经文 影响藏经阁产量
                id = (int)PropertyIdType.JingWen;
                proIdList.Add(id);
                proNumList.Add(num);
                qualityList.Add(proQuality);
                break;
            case StudentTalent.JingShang://种田 影响灵田产量
                id = (int)PropertyIdType.LingShi;
                proIdList.Add(id);
                proNumList.Add(num);
                qualityList.Add(proQuality);
                break;
            case StudentTalent.BaoShi://宝石 影响宝石属性
                id = (int)PropertyIdType.LianShi;
                proIdList.Add(id);
                proNumList.Add(num);
                qualityList.Add(proQuality);
                break;
        }
        if (p.talent != (int)StudentTalent.LianGong)
        {
            for(int i = 0; i < proIdList.Count; i++)
            {
                int theId = proIdList[i];
                SinglePropertyData pro = new SinglePropertyData();
                pro.id = theId;
                pro.num = proNumList[i];
                pro.quality = qualityList[i];
                
                p.propertyIdList.Add(theId);
                p.propertyList.Add(pro);
            }
            
        }
        p.studentCurExp = 0;
        RoleManager.Instance.RefreshBattlePro(p);
        EventCenter.Broadcast(TheEventType.EndowTalent, p);
        TaskManager.Instance.GetAchievement(AchievementType.TianFuJueXingNum, "");
        InitRedPoint();
        //反作弊存档：
        p.lastAppearTalentList.Clear();
        p.lastAppearTalentQualityList.Clear();
        p.lastAppearYuanSuList.Clear();
        TaskManager.Instance.TryAccomplishGuideBook(TaskType.TianFuJueXingNum);
        TaskManager.Instance.GetDailyAchievement(TaskType.TianFuJueXingNum,"1");

        TaskManager.Instance.TryAccomplishGuideBook(TaskType.HaveATalentBStudent);
        //TalkingDataGA.OnEvent("选择天赋", new Dictionary<string, object>() { { TalentNameByTalent(talent), 1 } });

    }

    /// <summary>
    /// 放弃天赋
    /// </summary>
    public void RemoveTianFu(PeopleData p)
    {
        for(int i = 0; i < p.curEquipItemList.Count; i++)
        {
            p.curEquipItemList[i] = null;
        }
        p.propertyIdList.Clear();
        p.propertyList.Clear();
        p.curEatedDanNum = 0;
        p.talent = (int)StudentTalent.None;
        p.lastAppearTalentList.Clear();
        p.lastAppearTalentQualityList.Clear();
        p.lastAppearYuanSuList.Clear();
        p.allSkillData = new AllSkillData();
        p.studentCurExp = 0;
        p.curXiuwei = 0;
        p.studentLevel = 1;
        p.trainIndex = 0;
        p.changeTalentNum = 0;
        InitRedPoint();
        //天赋移除
        EventCenter.Broadcast(TheEventType.RemoveTalent, p);
    }

    /// <summary>
    /// 洗髓 提升品级
    /// </summary>
    public void OnXiSui(PeopleData p,ItemData xiSuiDan)
    {
        return;

        //有概率失败
        int rate = p.xiSuiRate + xiSuiDan.setting.Param2.ToInt32();
        int rdmNum = RandomManager.Next(0, 100);
        if (rdmNum < rate)
        {

            p.studentRarity++;
            p.studentQuality++;
            if (p.studentRarity >= (int)Rarity.End)
            {
                p.studentRarity = (int)Rarity.Tian;
            }
            if (p.studentQuality >= (int)Quality.End)
            {
                p.studentQuality = (int)Quality.Gold;
            }
            if (p.talent != (int)StudentTalent.None)
            {
                //生产弟子重新分配属性
            }
            YuanSuType yuanSuType = YuanSuType.None;

            if (p.talent == (int)StudentTalent.LianGong)
            {
                yuanSuType = (YuanSuType)p.yuanSu;
            }

            EndowTalentToStudent(p, (StudentTalent)(int)p.talent, (Quality)(int)p.talentRarity, yuanSuType);
 
        }
        else
        {
            p.xiSuiRate += xiSuiDan.setting.Param3.ToInt32();
            if (p.xiSuiRate >= 100)
                p.xiSuiRate = 100;
         }


    }
    /// <summary>
    /// 改变性格
    /// </summary>
    /// <param name="p"></param>
    public void RdmChangeXingGe(PeopleData p)
    {
        p.socializationData.socialActivity = RandomManager.Next(0, 99);
        p.socializationData.xingGe = RdmXingGe();
        EventCenter.Broadcast(TheEventType.ChangeXingGe);
    }
    /// <summary>
    /// 整容
    /// </summary>
    /// <param name="p"></param>
    public void RdmZhengRong(PeopleData p)
    {
        RoleManager.Instance.RdmFace(p);

    }
    /// <summary>
    /// 通过天赋得到天赋名
    /// </summary>
    /// <returns></returns>
    public string TalentNameByTalent(StudentTalent talent)
    { 
        string res = "";
        if (talent == StudentTalent.LianGong)
        {
            res =  LanguageUtil.GetLanguageText((int)LanguageIdType.修武);
        }else if (talent == StudentTalent.None)
        {
            res = "新人";
        }
        else
        {
            PropertySetting setting = DataTable.FindPropertySettingByTalent((int)talent);
            if (setting == null) return "修武";
            res = setting.Name;
        }
        return res;
    }

    /// <summary>
    /// 找大标签的弟子
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public List<PeopleData> FindBigTagStudentList(StudentBigTag tag)
    {
        List<PeopleData> res = new List<PeopleData>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            switch (tag)
            {
                case StudentBigTag.None:
                    res.Add(p);
                    break;
                case StudentBigTag.New:
                    if(p.talent==(int)StudentTalent.None)
                    res.Add(p);
                    break;
                case StudentBigTag.Product:
                    if (p.talent !=(int) StudentTalent.None
                        &&p.talent!= (int)StudentTalent.LianGong)
                        res.Add(p);
                    break;
                case StudentBigTag.LianGong:
                    if (p.talent == (int)StudentTalent.LianGong)
                        res.Add(p);
                    break;
            }
        }
        return res;
    }
    /// <summary>
    /// 找带某个天赋的弟子
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public List<PeopleData> FindTalentStudentList(List<PeopleData> studentList, StudentTalent talent)
    {
        List<PeopleData> res = new List<PeopleData>();
        for (int i = 0; i < studentList.Count; i++)
        {
            PeopleData p = studentList[i];
            if (p.talent == (int)talent
                || talent==StudentTalent.All)
            {
                res.Add(p);
            }
        }
        return res;
    }
    /// <summary>
    /// 找带某个天赋的弟子
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public List<PeopleData> FindAllMyTalentStudentList(StudentTalent talent)
    {
        List<PeopleData> res = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent == (int)talent)
            {
                res.Add(p);
            }
        }
        return res;
    }
    /// <summary>
    /// 随机一个性格
    /// </summary>
    /// <returns></returns>
    public int RdmXingGe()
    {

       int xingGeType = RandomManager.Next(1, (int)XingGeType.End);
        return xingGeType;
    }

#region 弟子社交

    public List<SocialPlan> socialPlanList = new List<SocialPlan>();//当前社交

    /// <summary>
    /// 弟子进行社交
    /// </summary>
    public void OnStudentSocialization()
    {
        return;
        

        
    }

    /// <summary>
    /// 记录社交事件（保存一年）
    /// </summary>
    public void RecordSocialEvent(SocializationData data,string str)
    {
        SocializationRecordData recordData = new SocializationRecordData();
        recordData.time.Add(RoleManager.Instance._CurGameInfo.timeData.Year);
        recordData.time.Add(RoleManager.Instance._CurGameInfo.timeData.Month);
        recordData.time.Add(RoleManager.Instance._CurGameInfo.timeData.Week);
        recordData.content = str;
        data.socialRecordList.Add(recordData);
    }

    /// <summary>
    /// 改变 p1对p2的好感度
    /// </summary>
    public void ChangeHaoGanDu(PeopleData p1, PeopleData p2,int val)
    {
        int index = p1.socializationData.knowPeopleList.IndexOf(p2.onlyId);
        float percent = 1;
   

        p1.socializationData.haoGanDu[index] += val;
        if (p1.socializationData.haoGanDu[index] >= 100)
            p1.socializationData.haoGanDu[index] = 100;
        else if (p1.socializationData.haoGanDu[index] <= -100)
            p1.socializationData.haoGanDu[index] = -100;


    }

    /// <summary>
    /// 改变好感度的记录
    /// </summary>
    /// <returns></returns>
    public string HaoGanDuChangeRecord(int val)
    {
        if (val == 1)
        {
            return "交谈了一番，好感度略有提升";
        }else if (val == 2)
        {
            return "交谈甚欢，好感度提升了不少";

        }
        else if (val == 3)
        {
            return "志趣相投，畅聊许久，好感度大幅提升";

        }
        else if (val == -1)
        {
            return "话不投机，仇恨值略有提升";

        }
        else if (val == -2)
        {
            return "冷言冷语，仇恨值提升了不少";

        }
        else if (val == -3)
        {
            return "针锋相对，互相看对方不顺眼，仇恨值大幅提升";

        }
        return "";
    }

    /// <summary>
    /// 查找 p1对p2的好感度
    /// </summary>
    public int FindHaoGanDu(PeopleData p1, PeopleData p2)
    {
       
            int index = p1.socializationData.knowPeopleList.IndexOf(p2.onlyId);
        if (index == -1)
        {
            p1.socializationData.knowPeopleList.Add(p2.onlyId);
            p1.socializationData.haoGanDu.Add(0);
            p2.socializationData.knowPeopleList.Add(p1.onlyId);
            p2.socializationData.haoGanDu.Add(0);
            index = p1.socializationData.knowPeopleList.IndexOf(p2.onlyId);
        }
            return p1.socializationData.haoGanDu[index];

     }

    /// <summary>
    /// 清理掉弟子10年前的社交数据
    /// </summary>
    public void ClearSocializationData10YearBefore()
    {
        int count = RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count;
        for(int i = 0; i < count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            for(int j = p.socializationData.socialRecordList.Count-1; j >= 0; j--)
            {
                SocializationRecordData recordData = p.socializationData.socialRecordList[j];
                if(RoleManager.Instance._CurGameInfo.timeData.Year- recordData.time[0] >= 10)
                {
                    p.socializationData.socialRecordList.Remove(recordData);
                }
            }
        }
    }


    /// <summary>
    /// 得到好感度等级
    /// </summary>
    /// <returns></returns>
    public HaoGanLevelType GetStudentHaoGanLevelType(PeopleData p1,PeopleData p2)
    {
        HaoGanLevelType res = HaoGanLevelType.None;
        if (p1.socializationData.knowPeopleList.Contains(p2.onlyId))
        {
            int index = p1.socializationData.knowPeopleList.IndexOf(p2.onlyId);
            int haoGanDu = p1.socializationData.haoGanDu[index];
            //小好感
            if (haoGanDu > ConstantVal.littleHaoGanDu && haoGanDu <= ConstantVal.middleHaoGanDu)
            {
                res= HaoGanLevelType.Good;
            }
            else if (haoGanDu > ConstantVal.middleHaoGanDu && haoGanDu <= ConstantVal.bigHaoGanDu)
            {
                res = HaoGanLevelType.Great;


            }
            else if (haoGanDu > ConstantVal.bigHaoGanDu && haoGanDu < ConstantVal.fullHaoGanDu)
            {
                res = HaoGanLevelType.VeryGreat;
            }
            else if (haoGanDu >= ConstantVal.fullHaoGanDu)
            {
                if(p1.gender==p2.gender)
                res = HaoGanLevelType.Daolv;
                else
                    res = HaoGanLevelType.DaoYou;
            }
        }
        return res;
    }

#endregion

    /// <summary>
    /// 经验值满
    /// </summary>
    public void FullExp(PeopleData p)
    {
        if (p.talent != (int)StudentTalent.LianGong)
        {
            if (p.studentLevel >= DataTable._studentUpgradeList.Count)
                return;
            StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
            int needExp = setting.NeedExp.ToInt32();
            int curExp = p.studentCurExp;
            int val= needExp - curExp;
            OnGetStudentExp(p, val);
        }
    }

    /// <summary>
    /// 弟子准备上下阵
    /// </summary>
    /// <param name="onlyId"></param>
    /// <param name="up"></param>
    public void StudentPrepareTeam(ulong onlyId,bool up,int teamPosIndex, int teamIndex=0)
    {
        PeopleData p = StudentManager.Instance.FindStudent(onlyId);
        List<ulong> choosedTeam = null;
        if (teamIndex == 0)
            choosedTeam = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1;
        else
            choosedTeam = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList2;

        if (up)
        {
            if (p.studentStatusType == (int)StudentStatusType.AtTeam)
            {
                PanelManager.Instance.OpenFloatWindow("该弟子已上阵");
                return;
            }
            if (p.studentStatusType == (int)StudentStatusType.AtExplore)
            {
                PanelManager.Instance.OpenFloatWindow("该弟子正在秘境派遣中，无法出战");
                //TODO老存档弟子出不来的问题 以后删
                SingleExploreData exploreData = MapManager.Instance.FindSingleExploreDataById(p.curAtExploreId);
                if (exploreData.ExploreTeamData == null)
                {
                    p.studentStatusType = (int)StudentStatusType.None;
                    p.curAtExploreId = 0;
                }
                return;
            }
            if (p.seriousInjury)
            {
                PanelManager.Instance.OpenFloatWindow("该弟子伤势未痊愈，请掌门见谅");
                return;
            }
            if (p.studentStatusType == (int)StudentStatusType.DanFarmWork
           || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
           || p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi)
            {
                SingleDanFarmData studentFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmIndex];

                DanFarmSetting studentFarmSetting = DataTable.FindDanFarmSetting(studentFarmData.SettingId);


                if (studentFarmData.DanFarmWorkType == (int)DanFarmWorkType.Special
               && p.studentStatusType == (int)DanFarmStatusType.Working)
                {
                    DanFarmSetting setting = DataTable.FindDanFarmSetting(studentFarmData.SettingId);
                    PanelManager.Instance.OpenFloatWindow(p.name + "正在" + setting.Name + "工作，请等待工作完成");
                    return;

                }
                else
                {
                    PanelManager.Instance.OpenCommonHint("该弟子正在" + studentFarmSetting.Name + "驻守，是否取消其驻守?", () =>
                    {
                        LianDanManager.Instance.StopZuoZhen(p.onlyId);

                    }, null);
                    return;
                }
            }
            p.isMyTeam = true;
            p.studentStatusType = (int)StudentStatusType.AtTeam;
            p.atTeamIndex = teamIndex;
            p.teamPosIndex = teamPosIndex;
            if (choosedTeam[teamPosIndex]!=(p.onlyId))
            {
                choosedTeam[teamPosIndex]=(p.onlyId);
            }
        }
        else
        {
            p.studentStatusType = (int)StudentStatusType.None;
            p.atTeamIndex = -1;
            p.teamPosIndex = -1;

            if (choosedTeam.Contains(p.onlyId))
            {
                int index = choosedTeam.IndexOf(p.onlyId);
                choosedTeam[index]=0;
            }
        }
        EventCenter.Broadcast(TheEventType.StudentPrepareTeam, p);
        TaskManager.Instance.TryAccomplishGuideBook(TaskType.ShangZhen);

    }

    public void InitRedPoint()
    {
        ClearRedPoint();
           RedPoint MainPanel_Btn_Student = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student, 0);
        for(int i = 0; i < (int)StudentBigTag.End; i++)
        {
            RedPoint MainPanel_Btn_Student_BigTag = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_BigTag, i);
            //绑定
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student, MainPanel_Btn_Student_BigTag);

        }

        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            //他们都绑定全部
            RedPoint MainPanel_Btn_Student_InfoBigStudentView = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_InfoBigStudentView, (int)(ulong)p.onlyId);
            RedPoint MainPanel_Btn_Student_InfoBigStudentViewInfo = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_InfoBigStudentViewInfo, (int)(ulong)p.onlyId);
            RedPoint MainPanel_Btn_Student_InfoPanelStudentViewInfo = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_InfoPanelStudentViewInfo, (int)(ulong)p.onlyId);
            
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_InfoPanelStudentViewInfo, MainPanel_Btn_Student_InfoBigStudentView);
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_InfoPanelStudentViewInfo, MainPanel_Btn_Student_InfoBigStudentViewInfo);


            //技能
            RedPoint MainPanel_Btn_Student_InfoBigStudentView_SkillUpgrade = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_InfoBigStudentView_SkillUpgrade, (int)(ulong)p.onlyId);
            //血脉
            RedPoint MainPanel_Btn_Student_InfoBigStudentView_XueMai = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_InfoBigStudentView_XueMai, (int)(ulong)p.onlyId);

            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_InfoBigStudentViewInfo, MainPanel_Btn_Student_InfoBigStudentView_SkillUpgrade);
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_InfoBigStudentViewInfo, MainPanel_Btn_Student_InfoBigStudentView_XueMai);


            //新人
            if (p.talent == (int)StudentTalent.None)
            {
                //新人大标签
                RedPoint MainPanel_Btn_Student_BigTag2 = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_BigTag, (int)StudentBigTag.New);
                RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_BigTag2, MainPanel_Btn_Student_InfoPanelStudentViewInfo);
 
            }
            //修武大标签
            else if(p.talent == (int)StudentTalent.LianGong)
            {
                RedPoint MainPanel_Btn_Student_BigTag2 = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_BigTag, (int)StudentBigTag.LianGong);
                RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_BigTag2, MainPanel_Btn_Student_InfoPanelStudentViewInfo);
             }
            //生产
            else
            {
                //生产大标签
                RedPoint MainPanel_Btn_Student_BigTag2 = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_BigTag, (int)StudentBigTag.Product);
                //生产小标签
                RedPoint MainPanel_Btn_Student_BigTag_SmallTag = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_BigTag_SmallTag, p.talent);
                //生产全部小标签
                RedPoint MainPanel_Btn_Student_BigTag_SmallTag2 = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_BigTag_SmallTag, 0);

                RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_BigTag2, MainPanel_Btn_Student_BigTag_SmallTag);
                RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_BigTag_SmallTag, MainPanel_Btn_Student_InfoPanelStudentViewInfo);
                RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_BigTag_SmallTag2, MainPanel_Btn_Student_InfoPanelStudentViewInfo);
 

            }

            //全部大标签
            RedPoint MainPanel_Btn_Student_BigTag = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Student_BigTag, (int)StudentBigTag.None);
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_Student_BigTag, MainPanel_Btn_Student_InfoPanelStudentViewInfo);
 
            RefreshRedPointShow(p);
        }
        //修武的绑定在修武bigtag和全部bigtag 新人绑定在新人bigtag和全部bigtag 生产绑定在生产bigtag和天赋smalltag

    }
    /// <summary>
    /// 找某个稀有度 的生产弟子
    /// </summary>
    /// <param name="talent"></param>
    /// <returns></returns>
    public List<PeopleData> FindProductRarityStudent(Rarity rarity)
    {
        List<PeopleData> res = new List<PeopleData>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
         
            if (p.talent!=(int)StudentTalent.None
                &&p.talent!= (int)StudentTalent.LianGong
                &&p.studentRarity==(int)rarity)
            {
                res.Add(p);
            }
        }
        return res;
    }
    /// <summary>
    /// 找某个稀有度 的练功弟子
    /// </summary>
    /// <param name="talent"></param>
    /// <returns></returns>
    public List<PeopleData> FindLianGongRarityStudent(Rarity rarity)
    {
        List<PeopleData> res = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];

            if (p.talent == (int)StudentTalent.LianGong
                && p.studentRarity == (int)rarity
                &&!CheckIfStudentAtCangKu(p))
            {
                res.Add(p);
            }
        }
        return res;
    }
    /// <summary>
    /// 找某个稀有度 的 弟子
    /// </summary>
    /// <param name="talent"></param>
    /// <returns></returns>
    public List<PeopleData> FindRarityStudent(Rarity rarity)
    {
        List<PeopleData> res = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];

            if (  p.studentRarity == (int)rarity)
            {
                res.Add(p);
            }
        }
        return res;
    }
    //找是否有经验值满且没在仓库的生产弟子
    public bool CheckIfExpFullProductAndNotAtCangKuStudent()
    {
        List<PeopleData> res = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];

            if (p.talent != (int)StudentTalent.LianGong
                && p.talent != (int)StudentTalent.None
                 && !CheckIfStudentAtCangKu(p)
                 && p.studentLevel < StudentManager.Instance.GetStudentLevelLimit(p))
            {
                StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];

                if (p.studentCurExp >= setting.NeedExp.ToInt32())
                {
                    return true;
                }

            }
        }
        return false;
    }

    /// <summary>
    /// 生产弟子是否能突破
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool CheckIfProductStudentCanBreakThrough(PeopleData p)
    {

        if (p.talent != (int)StudentTalent.LianGong
            && p.talent != (int)StudentTalent.None
             && p.studentLevel < StudentManager.Instance.GetStudentLevelLimit(p))
        {
            StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];

            if (p.studentCurExp >= setting.NeedExp.ToInt32())
            {
                return true;
            }

        }

        return false;
    }
    /// <summary>
    /// 找某个稀有度 能进仓库 的生产弟子
    /// </summary>
    /// <param name="talent"></param>
    /// <returns></returns>
    public List<PeopleData> FindCanEnterCangKuProductRarityStudent(Rarity rarity)
    {
        List<PeopleData> res = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];

            if (p.talent != (int)StudentTalent.LianGong
                && p.talent!=(int)StudentTalent.None
                && p.studentRarity == (int)rarity
                && !CheckIfStudentAtCangKu(p))
            {
                StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];

                res.Add(p);
            }
        }
        return res;
    }
#region 弟子自主强化 

    /// <summary>
    /// 弟子是否在仓库
    /// </summary>
    /// <param name="p"></param>
    public bool CheckIfStudentAtCangKu(PeopleData p)
    {
        List<SingleDanFarmData> cangKuList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.CangKu);
        for (int i = 0; i < cangKuList.Count; i++)
        {
            SingleDanFarmData cangKu = cangKuList[i];
            if(cangKu.ZuoZhenStudentIdList.Contains(p.onlyId)) 
                return true;
        }
        return false;
    }
    /// <summary>
    /// 弟子是否在仓库修炼
    /// </summary>
    /// <param name="p"></param>
    public bool FindStudentAtCangKuTarget(PeopleData p)
    {
        List<SingleDanFarmData> cangKuList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.CangKu);
        for (int i = 0; i < cangKuList.Count; i++)
        {
            SingleDanFarmData cangKu = cangKuList[i];
            if (cangKu.ZuoZhenStudentIdList.Contains(p.onlyId))
            {
                int index = cangKu.ZuoZhenStudentIdList.IndexOf(p.onlyId);
                for(int j = 0; j < cangKu.StudentUseCangKuDataList.Count; j++)
                {
                    SingleStudentUseCangKuData useData = cangKu.StudentUseCangKuDataList[j];
                    if(useData.StudentOnlyId==p.onlyId)
                        return true;
                }
                //SingleStudentUseCangKuData useData = cangKu.StudentUseCangKuDataList[index];
                //return (StudentGoCangKuNeedType)useData.StudentGoCangKuNeedType;
            }
         }
        return false;
    }
    /// <summary>
    /// 每周选一个弟子进去搞事
    /// </summary>
    public void OnChooseStudentToCangKu()
    {
        if (RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count <= 0)
            return;
        List<SingleDanFarmData> cangKuList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.CangKu);
        for(int i = 0; i < cangKuList.Count; i++)
        {
            
               SingleDanFarmData cangKu = cangKuList[i];
            if (cangKu.Status != (int)DanFarmStatusType.Idling)
                continue;
            for (int j=0;j< cangKu.ZuoZhenStudentIdList.Count;j++)
            {
                //有空位
                if (cangKu.ZuoZhenStudentIdList[j] <= 0)
                {
                    //有没有材料
                    //if(ItemManager.Instance.)
                    //优先进修武的弟子
                    Rarity studentRarity = Rarity.None;

                    int rarityIndex = RandomManager.Next(0, 100);
                    if (rarityIndex < 40)
                        studentRarity = Rarity.Tian;
                    else if (rarityIndex < 65)
                        studentRarity = Rarity.Di;
                    else if (rarityIndex < 85)
                        studentRarity = Rarity.Xuan;
                    else if (rarityIndex < 95)
                        studentRarity = Rarity.Huang;
                    else
                        studentRarity = Rarity.Fan;

                    int studentTalentIndex = RandomManager.Next(0, 100);

                    //按照rarity排序
                    List<PeopleData> candidateList = new List<PeopleData>();
                    bool chooseLianGong = false;
                    ////如果有经验值满且没在仓库的生产弟子 且仓库有草 或者有经验值没满且没在仓库的弟子
                    //if (CheckIfExpFullProductAndNotAtCangKuStudent()
                    //    &&ItemManager.Instance.FindCangKuItemListByType(ItemType.StudentBook).Count>0)
                    //{

                    //}
                    //else
                    //{
                    //    chooseLianGong = true;
                    //    candidateList = FindLianGongRarityStudent(studentRarity);
                    //}
                    candidateList = FindLianGongRarityStudent(studentRarity);

                    //if (studentTalentIndex < 50)
                    //{
                    //    chooseLianGong = true;
                    //    candidateList = FindLianGongRarityStudent(studentRarity);
                    //}
                    //else
                    //{
                    //    candidateList = FindCanEnterCangKuProductRarityStudent(studentRarity);

                    //}
                    bool find = false;
                    if (candidateList.Count == 0)
                        find = false;
                    else
                        find = true;
                    //如果candidateList为0 则往下找 如果都没找到 return
                    while (candidateList.Count == 0 &&(int)studentRarity > 1)
                    {
                        studentRarity--;
                        candidateList = FindLianGongRarityStudent(studentRarity);
                         
                        //if (chooseLianGong)
                        //    candidateList = FindLianGongRarityStudent(studentRarity);
                        //else
                        //    candidateList = FindCanEnterCangKuProductRarityStudent(studentRarity);
                        if (candidateList.Count > 0)
                            find = true;
                    }

                    if (!find)
                    {
                        return;
                    }
                    int comePIndex = RandomManager.Next(0, candidateList.Count);
                    PeopleData p = candidateList[comePIndex];
                    //p来干嘛
                    //练功的来了 
                 
                        //List<StudentGoCangKuNeedType> validTypeList = new List<StudentGoCangKuNeedType>();

                        //List<ItemData> xiuWeiDanList = ItemManager.Instance.CheckCangKuHaveValidXiuWeiDan(p);
                        //List<ItemData> equipList = ItemManager.Instance.FindCangKuEquipItem();
                        //List<ItemData> skillBookList = ItemManager.Instance.FindCangKuSkillBookItem(p);
                        //List<ItemData> poJingDanList= ItemManager.Instance.CheckCangKuHaveValidPoJingDan(p);
                        //List<ItemData> gemList = ItemManager.Instance.FindCangKuGemItem();
                        //List<ItemData> equipMatList = ItemManager.Instance.FindCangKuIntenseMatItem(p.curEquipItem);
                        //List<ItemData> skillUpgradeMatList = ItemManager.Instance.FindCangKuSkillUpgradeMatItem(p);
                        //List<ItemData> xueMaiUpgradeMatList = ItemManager.Instance.FindCangKuXueMaiUpgradeItem(p);
                        //List<ItemData> productAddExpMatList = ItemManager.Instance.FindCangKuAddProductExpItem(p);
                        //List<ItemData> productCaoMatList = ItemManager.Instance.FindCangKuStudentUpgradeMatItem(p);
                        //生产弟子草

                        //if (productCaoMatList.Count > 0)
                        //{
                        //    validTypeList.Add(StudentGoCangKuNeedType.ProductStudentUpgrade);

                        //}
                        ////生产弟子经验丹

                        //if (productAddExpMatList.Count > 0)
                        //{
                        //    validTypeList.Add(StudentGoCangKuNeedType.ProductStudentAddExp);

                        //}
                        ////强化血脉
                        //if (xueMaiUpgradeMatList.Count > 0)
                        //{
                        //    validTypeList.Add(StudentGoCangKuNeedType.UpgradeXueMai);

                        //}

                        ////强化技能
                        //if (skillUpgradeMatList.Count > 0)
                        //{
                        //    validTypeList.Add(StudentGoCangKuNeedType.UpgradeSkill);
                        //}

                  //       //强化装备
                  //      if (p.curEquipItem != null
                  //&& equipMatList.Count > 0
                  //&& string.IsNullOrWhiteSpace(EquipmentManager.Instance.JudgeIfEquipIntenseSatisfyLevelCondition(p.curEquipItem.equipProtoData.curLevel))) 
                  //      {
                  //          validTypeList.Add(StudentGoCangKuNeedType.EquipIntense);
                  //      }

                        ////有装备的装配宝石
                        //if (p.curEquipItem!=null
                        //    &&EquipmentManager.Instance.CheckIfEquipCanInlayGem(p.curEquipItem)!=-1
                        // && gemList.Count > 0)
                        //{
                        //    validTypeList.Add(StudentGoCangKuNeedType.AddGem);
                        //}

                        ////修武的突破境界
                        //if (CheckIfLianGongStudentCanBreakThrough(p)
                        //    && poJingDanList.Count > 0)
                        //{
                        //    validTypeList.Add(StudentGoCangKuNeedType.EatPoJingDan);
                        //}


                        ////学习技能
                        //if (p.allSkillData!=null
                        //&&p.allSkillData.equippedSkillIdList.Count<=1
                        //  && skillBookList.Count > 0)
                        //{
                        //    validTypeList.Add(StudentGoCangKuNeedType.StudySkill);
                        //}

                        ////能否装备
                        //if (p.talent==(int)StudentTalent.LianGong
                        //&&p.curEquipItem == null
                        //    &&equipList.Count>0)
                        //{
                        //    validTypeList.Add(StudentGoCangKuNeedType.EquipEquip);
                        //}

                        //能否吃修为丹 且有合适修为丹 TODO把修为丹选项加入random
                        if (p.trainIndex < GetStudentLevelLimit(p))
                        {
                           if(p.curXiuwei< DataTable._trainList[p.trainIndex].XiuWeiNeed.ToUInt64())
                            StudentAutoEatXiuWeiDan(p, cangKu, j);

                    }
                    //if (validTypeList.Count > 0)
                    //    {
                    //        int index = RandomManager.Next(0, validTypeList.Count);
                    //        StudentGoCangKuNeedType choosedType = validTypeList[index];
                    //        switch (choosedType)
                    //        {  
                    //            //生产弟子吃经验丹
                    //            case StudentGoCangKuNeedType.ProductStudentAddExp:
                    //                ProductStudentAutoAddExp(p, productAddExpMatList, cangKu, j);
                    //                break;
                    //            //强化血脉
                    //            case StudentGoCangKuNeedType.UpgradeXueMai:
                    //                StudentAutoUpgradeXueMai(p, xueMaiUpgradeMatList, cangKu, j);
                    //                break;
                    //            //强化技能
                    //            case StudentGoCangKuNeedType.UpgradeSkill:
                    //                StudentAutoIntenseSkill(p, skillUpgradeMatList, cangKu, j);
                    //                break;
                    //            ////强化装备

                    //            //case StudentGoCangKuNeedType.EquipIntense:
                    //            //    StudentAutoIntenseEquip(p, equipMatList, cangKu, j);
                    //            //    break;
                    //            ////有装备的装配宝石

                    //            //case StudentGoCangKuNeedType.AddGem:
                    //            //    int addGemindex = RandomManager.Next(0, gemList.Count);
                    //            //    StudentAutoInlayGem(p, gemList[addGemindex], cangKu, j);
                    //            //    break;
                    //            //修武的突破境界

                    //            case StudentGoCangKuNeedType.EatPoJingDan:
                    //                int poJingDanIndex = RandomManager.Next(0, poJingDanList.Count);
                    //                StudentAutoEatPoJingDan(p, poJingDanList[poJingDanIndex], cangKu, j);
                    //                break;
                    //            //学习技能

                    //            case StudentGoCangKuNeedType.StudySkill:

                    //                int skillStudyindex = RandomManager.Next(0, skillBookList.Count);
                    //                StudentAutoStudySkill(p, skillBookList[skillStudyindex], cangKu, j);
                    //                break;
                    //                //装备装备
                    //            case StudentGoCangKuNeedType.EquipEquip:


                    //                int equipindex = RandomManager.Next(0, equipList.Count);
                    //                StudentAutoEquip(p, equipList[equipindex], cangKu, j);
                    //                break;
                    //                //吃修为丹
                    //            case StudentGoCangKuNeedType.EatXiuWeiDan:
                    //                StudentAutoEatXiuWeiDan(p, xiuWeiDanList, cangKu, j);
                    //                break;
                    //            //生产弟子吃草
                    //            case StudentGoCangKuNeedType.ProductStudentUpgrade:
                    //                StudentAutoEatQingLingCao(p, productCaoMatList, cangKu, j);
                    //                break;
                               
                    //        }

                    //    }
                    
                  
                }
            }
        }
    }
   
    public SingleStudentUseCangKuData GenerateBasicUseCangKuData(PeopleData p, SingleDanFarmData cangKu, int zuoZhenPos)
    {
        cangKu.ZuoZhenStudentIdList[zuoZhenPos] = p.onlyId;
        SingleStudentUseCangKuData data = new SingleStudentUseCangKuData();
        data.FarmOnlyId = cangKu.OnlyId;
        data.RemainTime = 15 / p.studentRarity;
        data.TotalTime = data.RemainTime;
        data.StudentOnlyId = p.onlyId;
        cangKu.StudentUseCangKuDataList.Add(data);
        return data;
    }

    /// <summary>
    /// 弟子自动吃修为丹
    /// </summary>
    public void StudentAutoEatXiuWeiDan(PeopleData p,SingleDanFarmData cangKu,int zuoZhenPos)
    {
        //ItemData item = itemList[0];
        //ulong num =  item.count/2;
        /////最多吃20个
        //if (num>=20)
        //{
        //    num = 20;
        //}
        //else if (num <= 1)
        //{
        //    num = item.count;
        //}

        //最多用10分之一的修为
        TrainSetting trainSetting = DataTable._trainList[p.trainIndex];

        int lastXiuWeiNeed = 0;
        if (p.trainIndex >= 1)
        {
            TrainSetting lastTrainSetting = DataTable._trainList[p.trainIndex - 1];
            lastXiuWeiNeed = lastTrainSetting.XiuWeiNeed.ToInt32();
        }
        int useNum = cangKu.ProductRemainNum;
        int totalXiuWei = trainSetting.XiuWeiNeed.ToInt32() - lastXiuWeiNeed;
        int needXiuWeiToNextLevel = trainSetting.XiuWeiNeed.ToInt32() - (int)(ulong)p.curXiuwei;
        
        int maxNum = totalXiuWei / 10;
        if (needXiuWeiToNextLevel<maxNum)
        {
            maxNum = needXiuWeiToNextLevel;
        }
        if (useNum > maxNum)
            useNum = maxNum;

        cangKu.ProductRemainNum -= useNum;
  

        //ItemManager.Instance.LoseCangKuItem(item.settingId,(ulong) num);
        //ItemSetting setting = DataTable.FindItemSetting(item.settingId);

        SingleStudentUseCangKuData data = GenerateBasicUseCangKuData(p, cangKu, zuoZhenPos);
        int baseXiuWeiAdd = RandomManager.Next(2, 4) * RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel;

        data.XiuWeiNum = useNum+ baseXiuWeiAdd;
        //data.StudentGoCangKuNeedType = (int)StudentGoCangKuNeedType.EatXiuWeiDan;

        EventCenter.Broadcast(TheEventType.OnZuoZhenStudent, cangKu, zuoZhenPos);
    }

    /// <summary>
    /// 强化个东西需要3天 这里走进度
    /// </summary>
    public void OnDayPassed()
    {
        if (RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count <= 0)
            return;
        List<SingleDanFarmData> cangKuList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.CangKu);
        for (int i = 0; i < cangKuList.Count; i++)
        {
            SingleDanFarmData cangKu = cangKuList[i];
            for (int j = 0; j < cangKu.ZuoZhenStudentIdList.Count; j++)
            {
                ulong onlyId = cangKu.ZuoZhenStudentIdList[j];
                if (onlyId > 0)
                {
                    for(int k = cangKu.StudentUseCangKuDataList.Count-1; k >=0; k--)
                    {
                        SingleStudentUseCangKuData singleData = cangKu.StudentUseCangKuDataList[k];
                        if (singleData.RemainTime >= 0)
                        {
                            singleData.RemainTime--;
                            EventCenter.Broadcast(TheEventType.OnStudentCangKuProcess, singleData);
                        }
                        else
                        {
                            cangKu.StudentUseCangKuDataList.RemoveAt(k);
                            int index = cangKu.ZuoZhenStudentIdList.IndexOf(singleData.StudentOnlyId);
                            cangKu.ZuoZhenStudentIdList[index] = 0;
                            PeopleData p = StudentManager.Instance.FindStudent(singleData.StudentOnlyId);
                            ExecuteCangKuWork(p, singleData);
                            EventCenter.Broadcast(TheEventType.OnStudentCangKuStopWork, singleData);

                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 仓库工作结算
    /// </summary>
    public void ExecuteCangKuWork(PeopleData p, SingleStudentUseCangKuData useCangKuData)
    {
        ItemType achivementItemType = ItemType.None;
        p.curXiuwei += (ulong)useCangKuData.XiuWeiNum;
        achivementItemType = ItemType.Dan;
        //switch ((StudentGoCangKuNeedType)useCangKuData.StudentGoCangKuNeedType)
        //{
        //    case StudentGoCangKuNeedType.EatXiuWeiDan:
        //        p.curXiuwei += (ulong)useCangKuData.XiuWeiNum;
        //        achivementItemType = ItemType.Dan;
        //        break;
        //        //这里注意 赠送弟子装备 需要判断是不是在仓库搞装备
        //    //case StudentGoCangKuNeedType.EquipEquip:
        //    //    EquipmentManager.Instance.OnEquip(p, useCangKuData.EquipItemData);
        //    //    ItemSetting setting = DataTable.FindItemSetting(p.curEquipItem.settingId);
        //    //    PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "装备了"+ setting.name);
        //    //    achivementItemType = ItemType.Equip;

        //    //    List<DialogSetting> candidateContent3 = DataTable.FindDialogSettingListByType((int)DialogContentType.StudentEquipEquip);
        //    //    string str3 = candidateContent3[RandomManager.Next(0, candidateContent3.Count)].content;
        //    //    str3 = str3.Replace("x",setting.name);
        //    //    ChatManager.Instance.AddChat(p, str3);

        //    //    break;
        //        //学习技能
        //    case StudentGoCangKuNeedType.StudySkill:
        //        ItemData skillBookData = useCangKuData.EquipItemData;
        //        SingleSkillData skill = SkillManager.Instance.AddSkill(skillBookData.SettingId, p);
        //        SkillManager.Instance.EquipSkill(p, skill);
        //        //弹窗通知
        //        PanelManager.Instance.OpenPanel<StudentGetNewSkillPanel>(PanelManager.Instance.trans_layer2, p, skill);
        //        achivementItemType = ItemType.SkillBook;

        //        List<DialogSetting> candidateContent2 = DataTable.FindDialogSettingListByType((int)DialogContentType.StudentStudySkill);
        //        string str = candidateContent2[RandomManager.Next(0, candidateContent2.Count)].content;
        //        str = str.Replace("x", DataTable.FindSkillSetting(skill.skillId).name);
        //        ChatManager.Instance.AddChat(p, str);
        //        break;
        //        //弟子突破
        //    case StudentGoCangKuNeedType.EatPoJingDan:
        //        if(p.curEatedDanNum<2)
        //        p.curEatedDanNum += 1;
        //        OnBreakThrough(p);
        //        TrainSetting trainSetting = DataTable._trainList[p.trainIndex];
        //        PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "已突破到" + trainSetting.smallPhaseName);
        //        achivementItemType = ItemType.PoJingDan;

        //        break;
        //    ////镶嵌宝石 索要的时候记得弹窗
        //    //case StudentGoCangKuNeedType.AddGem:
        //    //    ItemSetting gemItemSetting = DataTable.FindItemSetting(useCangKuData.EquipItemData.SettingId);
        //    //    EquipmentManager.Instance.OnInlayGem(p, p.curEquipItem.equipProtoData,useCangKuData.EquipItemData, EquipmentManager.Instance.CheckIfEquipCanInlayGem(p.curEquipItem));
        //    //    PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "已成功镶嵌" + gemItemSetting.name);
        //    //    achivementItemType = ItemType.Gem;

        //    //    break;
        //    ////强化装备
        //    //case StudentGoCangKuNeedType.EquipIntense:

        //    //    EquipmentManager.Instance.OnIntenseEquip(p.curEquipItem.equipProtoData.onlyId, null,p);
        //    //    ItemSetting equipSetting = DataTable.FindItemSetting(p.curEquipItem.settingId);
        //    //    PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "已成功将" + equipSetting.name+"强化至"+p.curEquipItem.equipProtoData.curLevel);
        //    //    achivementItemType = ItemType.EquipMat;


        //    //    List<DialogSetting> candidateContent4 = DataTable.FindDialogSettingListByType((int)DialogContentType.StudentIntenseEquip);
        //    //    string str4 = candidateContent4[RandomManager.Next(0, candidateContent4.Count)].content;
        //    //     ChatManager.Instance.AddChat(p, str4);

        //    //    break;
        //    //生产弟子吃经验丹
        //    case StudentGoCangKuNeedType.ProductStudentAddExp:
        //        OnGetStudentExp(p, useCangKuData.CommonParam.ToInt32());
        //        break;
        //    //强化血脉
        //    case StudentGoCangKuNeedType.UpgradeXueMai:
        //        XueMaiManager.Instance.OnUpgrade(p, (XueMaiType)useCangKuData.CommonParam.ToInt32());
        //        XueMaiUpgradeSetting xueMaiUpgradeSetting = DataTable.FindXueMaiUpgradeSettingByType((XueMaiType)useCangKuData.CommonParam.ToInt32());
        //        PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "已成功将" + xueMaiUpgradeSetting.name + "强化至Lv." + XueMaiManager.Instance.FindXueMaiLevel(p, (XueMaiType)useCangKuData.CommonParam.ToInt32()));
        //        achivementItemType = ItemType.XueMai;

        //        break;
        //    //升级技能
        //    case StudentGoCangKuNeedType.UpgradeSkill:

        //        SingleSkillData theSkill = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[1], p.allSkillData);
        //        SkillManager.Instance.OnUpgradeSkill(theSkill);
        //        SkillSetting skillSetting = DataTable.FindSkillSetting(theSkill.skillId);
        //        PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "已成功将" + skillSetting.name + "强化至Lv." + theSkill.skillLevel);
        //        achivementItemType = ItemType.LingQuan;

        //        break;
        //    //生产弟子用草
        //    case StudentGoCangKuNeedType.ProductStudentUpgrade:
        //        StudentManager.Instance.BreakThrough(p);
        //        PanelManager.Instance.AddTongZhi(TongZhiType.Common, p.name + "已成功将技艺提升至Lv." + p.studentLevel);
        //        achivementItemType = ItemType.StudentBook;

        //        break;
        //}
        // 已废弃：CangKu相关功能已移除
    }

    /// <summary>
    /// 判断是否在仓库（已废弃）
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool JudgeIfAtCangKu(PeopleData p)
    {
        // 已废弃：CangKu功能已移除，始终返回false
        return false;
    }

#endregion


    /// <summary>
    /// 刷新现实时间今年能招募的弟子
    /// </summary>
    public void RefreshTodayRecruitStudentTime(long x)
    {
        RoleManager.Instance._CurGameInfo.studentData.todayRecruitStudentNum = 0;
        RoleManager.Instance._CurGameInfo.studentData.lastRecruitStudentTime = x;
    }

    /// <summary>
    /// 清掉所有弟子
    /// </summary>
    public void ClearRedPoint()
    {
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Student);

        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Student_BigTag);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Student_BigTag_SmallTag);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Student_InfoBigStudentView);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Student_InfoBigStudentViewInfo);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Student_InfoPanelStudentViewInfo);

        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Student_InfoBigStudentView_SkillUpgrade);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Student_InfoBigStudentView_XueMai);

    }

    /// <summary>
    /// 是否能突破
    /// </summary>
    public bool CheckIfCanTuPo(PeopleData p)
    {
        //弟子突破
        if (p.onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId
            ||p.talent==(int)StudentTalent.LianGong)
        {
            if(p.trainIndex < StudentManager.Instance.GetStudentLevelLimit(p) )
            {
                TrainSetting curTrainSetting = DataTable._trainList[p.trainIndex];

                //不能突破 只能修炼
                if (p.curXiuwei < curTrainSetting.XiuWeiNeed.ToUInt64())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }
        if (p.talent != (int)StudentTalent.LianGong)
        {

            if (p.studentLevel > 0 && p.studentLevel < StudentManager.Instance.GetStudentLevelLimit(p))
            {
                if (p.studentLevel <= DataTable._studentUpgradeList.Count)
                {
                    StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                    int expLimit = setting.NeedExp.ToInt32();
                    if (p.studentCurExp>= expLimit)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// 生产弟子吃纯灵果
    /// </summary>
    /// <param name="p"></param>
    /// <param name="item"></param>
    /// <param name="cangKu"></param>
    /// <param name="zuoZhenPos"></param>
    public void UseChunLingGuo(PeopleData p,  ItemData item)
    {
        ItemManager.Instance.LoseItem(item.settingId, 1);
         OnGetStudentExp(p, item.setting.Param.ToInt32());
    }

    /// <summary>
    /// 使用调息丹
    /// </summary>
    /// <param name="p"></param>
    /// <param name="item"></param>
    /// <param name="cangKu"></param>
    /// <param name="zuoZhenPos"></param>
    public void UseTiaoXiDan(PeopleData p, ItemData item)
    {
   
     
    }

    /// <summary>
    /// 双天弟子数
    /// </summary>
    /// <returns></returns>
    public int FindShuangTianStudentNum()
    {
        int res = 0;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.studentRarity == 5 && p.talentRarity == 5)
                res++;
        }
        return res;
    }

    /// <summary>
    /// 回复元神
    /// </summary>
    public void TimeHuiFuYuanShen(long before, long after)
    {
        long offset = (after - before);
 
    }

    /// <summary>
    /// 是否达到等级上限
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool IfPReachLevelLimit(PeopleData p)
    {
        int curLevel = 0;
        if (p.talent == (int)StudentTalent.LianGong
         || p.onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
            curLevel = p.trainIndex;
        else
            curLevel = p.studentLevel;
        if (curLevel >= GetStudentLevelLimit(p))
        {
            //PanelManager.Instance.OpenFloatWindow("该弟子潜力已达上限");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 生产弟子最大等级
    /// </summary>
    /// <returns></returns>
    public int MaxProductStudentLevel()
    {
        int maxLevel = 0;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent != (int)StudentTalent.LianGong)
            {
                if (p.studentLevel >= maxLevel)
                    maxLevel = p.studentLevel;
            }
        }
        return maxLevel;
    }

}

/// <summary>
/// 社交计划
/// </summary>
public class SocialPlan
{
    public SocialPlan(PeopleData p1, PeopleData p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }
    public PeopleData p1;
    public PeopleData p2;
}

/// <summary>
/// 好感度级别
/// </summary>
public enum HaoGanLevelType
{
    None=0,
    Good=1,
    Great=2,
    VeryGreat=3,
    Daolv=4,
    DaoYou=5,
}

/// <summary>
/// 学生类型
/// </summary>
public enum StudentType
{
    None=0,
    WaiMen=1,//外门弟子
    //EquipMake=2,//炼器师
    //LianGong=3,//内门弟子
    Enemy=4,//敌人
}

/// <summary>
/// 弟子天赋
/// </summary>
public enum StudentTalent
{
    None=0,
    LianJing=1,//炼丹师 品质
    DuanZhao=2,//炼器师
    LianGong=3,//亲传弟子
    CaiKuang=4,//采矿
    ChaoYao=5,//造化灵树
    JingWen=6,//经文 藏经阁
    BaoShi=7,//炼宝石
    JingShang=8,//种田
    All=10001,
    ManKing=100001,
    FemaleKing=100002
}

/// <summary>
/// 弟子状态
/// </summary>
public enum StudentStatusType
{
    None=0,//掌管所有丹房
    DanFarmWork=1,//丹房工作中
    DanFarmRelax=2,//丹房休息中
    DanFarmQuanLi=3,//丹房全力工作中
    //AtFixedWorld=4,//在固定关卡
    //AtXianMenWorld=5,//在仙门关卡
    AtExplore=6,//在探险
    AtTeam=7,//在队伍中（统一用此）
}

/// <summary>
/// 性格
/// </summary>
public enum XingGeType
{
    
    YiQi=1,//义气
    GuPi=2,//孤僻
    WuSi=3,//无私
    XinYe=4,//心野
    AiJia=5,//爱家
    YaZi=6,//睚眦
    HuZong=7,//护宗
    XieE=8,//邪恶
    ZhengZhi=9,//正直
    RenWo=10,//任我
    End=11,//结束
}

///// <summary>
///// 弟子到仓库干嘛来了
///// </summary>
//public enum StudentGoCangKuNeedType
//{
//    None=0,
//    EatXiuWeiDan=1,//吃修为丹 现在只有这个（练功）
//    UpgradeSkill=2,//升级技能
//    EquipIntense=3,//强化装备
//    AddGem=4,//加宝石
//    EquipEquip=5,//装备装备
//    StudySkill=6,//学习技能
//    EatPoJingDan=7,//吃破境丹
//    ProductStudentUpgrade=8,//生产弟子吃草
//    UpgradeXueMai=9,//强化血脉
//    ProductStudentAddExp=10,//生产弟子吃经验
//}

/// <summary>
/// 生成候选弟子类型
/// </summary>
public enum GenerateCandidateStudentType
{
    None=0,
    ByMoney=1,
    AD=2,
}


/// <summary>
/// 对话配置ID类型
/// </summary>
public enum DialogSettingIdType
{
    None = 0,
    CommonZhaoMu = 1,
    CommonCiTui = 2,
}

/// <summary>
/// 对话内容类型
/// </summary>
public enum DialogContentType
{
    None = 0,
    StudentBreakThrough = 1,
    SendItemToStudent = 2,
    StudentAnswerZhangMen = 3,
    BiaoQingBao = 4,
    ChouHenAfterLiLian = 5,
    HaoGanAfterLiLian = 6,
    MatchEnemyWin = 7,
    MatchEnemyLose = 8,
    MatchEnemyBeforeBattle = 9,
    PoChan = 10,
}
