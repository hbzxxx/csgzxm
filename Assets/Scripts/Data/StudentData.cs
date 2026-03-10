using System;
using System.Collections.Generic;

/// <summary>
/// 弟子数据
/// </summary>
[Serializable]
public class StudentData
{
    /// <summary>最大弟子数量</summary>
    public int MaxStudentNum;
    
    /// <summary>当前弟子数量</summary>
    public int CurStudentNum;
    
    /// <summary>当前空闲弟子数量</summary>
    public int CurFreeStudentNum;
    
    /// <summary>所有弟子列表</summary>
    public List<PeopleData> allStudentList = new List<PeopleData>();
    
    /// <summary>招募候选弟子</summary>
    public List<PeopleData> recruitCandidateStudent = new List<PeopleData>();
    
    /// <summary>今年剩余可招募弟子数量</summary>
    public int thisYearRemainCanRecruitStudentNum;
    
    /// <summary>今年已招募弟子数量</summary>
    public int thisYearRecruitedStudentNum;
    
    /// <summary>今年刷新弟子次数</summary>
    public int thisYearBrushStudentNum;
    
    /// <summary>今年是否已观看广告扩招</summary>
    public bool thisYearWatchedADNum;
    
    /// <summary>上次新弟子年份</summary>
    public int lastNewStudentYear;
    
    /// <summary>今日招募弟子数量</summary>
    public int todayRecruitStudentNum;
    
    /// <summary>上次招募弟子时间</summary>
    public long lastRecruitStudentTime;
    
    /// <summary>总招募弟子数量</summary>
    public int totalRecruitStudentNum;
}
