 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

public class Skill : MonoBehaviour, ISkill
{
    
    public float foreRollTime { get; set ; }
    public SkillSetting skillSetting { get; set; }

    public void ForeRoll()
    {
    }

    public void Init()
    {
    }

    public void Init(SkillSetting skillSetting, PeopleData belong, PeopleData be)
    {
        this.skillSetting = skillSetting;
    }

    public void Run()
    {
    }
}
