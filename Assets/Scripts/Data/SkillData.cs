using System;
using System.Collections.Generic;

/// <summary>
/// 所有技能数据
/// </summary>
[Serializable]
public class AllSkillData
{
    /// <summary>技能列表</summary>
      public List<SingleSkillData> skillList = new List<SingleSkillData>();
    
    /// <summary>已解锁类型列表</summary>
     public List<int> unlockedTypeList = new List<int>();
    
    /// <summary>已解锁技能位置</summary>
    public int unlockedSkillPos;
    
    /// <summary>已装备技能ID列表</summary>
    public List<int> equippedSkillIdList = new List<int>();
    
    /// <summary>上次使用技能索引</summary>
    public int lastUseSkillIndex;
}

/// <summary>
/// 单个技能数据
/// </summary>
[Serializable]
public class SingleSkillData
{
    /// <summary>技能ID</summary>
    public int skillId;
    
    /// <summary>技能等级</summary>
    public int skillLevel;
    
    /// <summary>伤害百分比列表</summary>
    public List<float> damagePercentList = new List<float>();
    
    /// <summary>是否已装备</summary>
    public bool isEquipped;
    
    /// <summary>冷却时间（剩余回合数）</summary>
    public int cd;
    
    /// <summary>是否查看过（影响红点）</summary>
    public bool show;
    
    public YuanSuType yuanSuType;
}
