using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
 
public interface ISkill
{
     SkillSetting skillSetting { get; set; }

     void Init(SkillSetting skillSetting, PeopleData belong, PeopleData be);//初始化
     void ForeRoll();//技能前摇

     void Run();//技能发动
}
