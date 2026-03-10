using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class OtherZongMenManager : CommonInstance<OtherZongMenManager>
{
    

    /// <summary>
    /// 初始化一组宗门
    /// </summary>
    /// <param name="index"></param>
    public void InitGroupZongMen(int index)
    {
        //string fieldName = DataTable._mapList[index].name;
        //SingleOtherZongMenData singleOtherZongMenData = new SingleOtherZongMenData();
        //int zongMenNum = 100 * index;
        //for(int i = 0; i < index; i++)
        //{
        //    PeopleData PeopleData = new PeopleData();
        //    PeopleData.OnlyId = ConstantVal.SetId;

        //    List<List<int>> baseBattleProList = CommonUtil.SplitCfg(ConstantVal.baseBattleProperty);

        //    List<List<int>> proList = CommonUtil.SplitCfg(enemySetting.property);
        //    List<int> haveValIdList = new List<int>();
        //    List<int> haveValValList = new List<int>();
        //    for (int i = 0; i < proList.Count; i++)
        //    {
        //        List<int> thePro = proList[i];
        //        haveValIdList.Add(thePro[0]);
        //        haveValValList.Add(thePro[1]);
        //    }
        //    for (int i = 0; i < baseBattleProList.Count; i++)
        //    {
        //        List<int> singlePro = baseBattleProList[i];
        //        int id = singlePro[0];
        //        int num = singlePro[1];

        //        if (haveValIdList.Contains(id))
        //        {
        //            int index = haveValIdList.IndexOf(id);
        //            num = haveValValList[index];
        //        }

        //        SinglePropertyData singlePropertyData = new SinglePropertyData();
        //        singlePropertyData.Id = id;
        //        singlePropertyData.Num = num;
        //        singlePropertyData.Quality = 1;

        //        if (id == (int)PropertyIdType.MpNum)
        //        {
        //            singlePropertyData.Limit = 100;
        //        }
        //        else if (id == (int)PropertyIdType.Hp)
        //        {
        //            singlePropertyData.Limit = singlePropertyData.Num;
        //        }


        //        PeopleData.PropertyList.Add(singlePropertyData);
        //        PeopleData.PropertyIdList.Add(singlePropertyData.Id);

        //        SinglePropertyData battle_singlePropertyData = new SinglePropertyData();
        //        battle_singlePropertyData.Id = id;
        //        battle_singlePropertyData.Num = singlePropertyData.Num;
        //        battle_singlePropertyData.Limit = singlePropertyData.Limit;
        //        battle_singlePropertyData.Quality = 1;

        //        PeopleData.CurBattleProIdList.Add(id);
        //        PeopleData.CurBattleProList.Add(battle_singlePropertyData);
        //        //PeopleData.ProQualityList.Add(proQuality);
        //    }
        //    //性别
        //    int gender = RandomManager.Next(1, 3);
        //    string name = enemySetting.name;
        //    PeopleData.Name = name;
        //    PeopleData.Gender = gender;
        //    RoleManager.Instance.RdmFace(PeopleData);

        //    //PeopleData.StudentRarity = matchLevel;
        //    //PeopleData.StudentQuality = enemyLevel;
        //    PeopleData.EnemySettingId = enemySetting.id.ToInt32();
        //    PeopleData.StudentType = (int)StudentType.Enemy;
        //    //初始技能暂定灵弹
        //    PeopleData.AllSkillData = new AllSkillData();
        //    if (!string.IsNullOrWhiteSpace(enemySetting.skill))
        //    {
        //        List<List<int>> theSkill = CommonUtil.SplitCfg(enemySetting.skill);
        //        for (int i = 0; i < theSkill.Count; i++)
        //        {
        //            List<int> settingSkill = theSkill[i];
        //            SingleSkillData singleSkill = new SingleSkillData();
        //            singleSkill.SkillId = settingSkill[0];
        //            singleSkill.SkillLevel = settingSkill[1];
        //            PeopleData.AllSkillData.SkillList.Add(singleSkill);
        //            PeopleData.AllSkillData.EquippedSkillIdList.Add(singleSkill.SkillId);
        //        }
        //    }
        //    else
        //    {
        //        SingleSkillData singleSkill = new SingleSkillData();
        //        singleSkill.SkillId = (int)SkillIdType.LingDan;
        //        singleSkill.SkillLevel = 1;
        //        PeopleData.AllSkillData.SkillList.Add(singleSkill);
        //        PeopleData.AllSkillData.EquippedSkillIdList.Add(singleSkill.SkillId);
        //    }
        //}
    }
}
