using System;
using System.Collections.Generic;

/// <summary>
/// 所有NPC数据
/// </summary>
[Serializable]
public class AllNPCData
{
    /// <summary>所有NPC列表</summary>
    public List<SingleNPCData> AllNPCList = new List<SingleNPCData>();
    
    /// <summary>当前显示的NPC唯一ID列表</summary>
    public List<ulong> CurShowNPCOnlyIdList = new List<ulong>();
    
    /// <summary>已移除的NPC配置ID列表</summary>
    public List<int> RemovedNPCSettingIdList = new List<int>();
}

/// <summary>
/// 单个NPC数据
/// </summary>
[Serializable]
public class SingleNPCData
{
    /// <summary>NPC ID</summary>
    public int Id;
    
    /// <summary>唯一ID</summary>
    public ulong OnlyId;
    
    /// <summary>名称</summary>
    public string Name = "";
    
    /// <summary>已完成任务索引</summary>
    public int AccomplishedTaskIndex;
    
    /// <summary>所有任务列表</summary>
    public List<SingleTaskProtoData> AllTaskList = new List<SingleTaskProtoData>();
    
    /// <summary>人物数据</summary>
    public PeopleData PeopleData;
    
    /// <summary>当前任务ID</summary>
    public ulong CurTaskId;
    
    /// <summary>当前显示场景</summary>
    public int CurShowScene;
    
    /// <summary>本地位置</summary>
    public List<float> LocalPos = new List<float>();
    
    /// <summary>下次出现时间</summary>
    public long NextAppearTime;
    
    /// <summary>小人物贴图名称</summary>
    public string SmallPeopleTextureName = "";
}

/// <summary>
/// 单个任务数据
/// </summary>
[Serializable]
public class SingleTaskProtoData
{
    /// <summary>任务ID</summary>
    public ulong TheId;
    
    /// <summary>任务索引</summary>
    public int TaskIndex;
    
    /// <summary>完成状态</summary>
    public int AccomplishStatus;
    
    /// <summary>上次完成时间</summary>
    public long LastAccomplishTime;
    
    /// <summary>是否开始引导</summary>
    public bool StartGuide;
    
    /// <summary>当前进度</summary>
    public int CurNum;
    
    /// <summary>NPC ID</summary>
    public int NpcId;
}

/// <summary>
/// 标记的人物数据
/// </summary>
[Serializable]
public class NotedPeopleData
{
    /// <summary>标记的人物唯一ID列表</summary>
    public List<ulong> notedPeopleOnlyIdList = new List<ulong>();
    
    /// <summary>标记的人物列表</summary>
    public List<SingleNotedPeople> NotedList = new List<SingleNotedPeople>();
}

/// <summary>
/// 单个标记人物数据
/// </summary>
[Serializable]
public class SingleNotedPeople
{
    /// <summary>唯一ID</summary>
    public ulong OnlyId;
    
    /// <summary>名称</summary>
    public string Name = "";
    
    /// <summary>记录列表</summary>
    public List<SocializationRecordData> Record = new List<SocializationRecordData>();
    
    /// <summary>标记类型</summary>
    public int NotedType;
    
    /// <summary>人物数据</summary>
    public PeopleData P;
}
