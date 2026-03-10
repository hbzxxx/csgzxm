 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 记录面板
/// </summary>
public class RecordManager : CommonInstance<RecordManager>
{
   
    public void AddTongZhi(string content)
    {
      //  EventCenter.Broadcast(TheEventType.AddTongZhi,TongZhiType.Common, content);
        PanelManager.Instance.AddTongZhi(TongZhiType.Common, content);
    }

    public void AddTongZhi(string content,ConsumeType consumeType,int id,int num)
    {
        int theId = id;
        int theNum =  num;
        PanelManager.Instance.AddTongZhi(TongZhiType.Consume,content,consumeType, theId, theNum);
    }

    /// <summary>
    /// 增加顶部事件记录
    /// </summary>
    /// <param name="record"></param>
    public void AddTopRecord(string record)
    {
        EventCenter.Broadcast(TheEventType.AddTopRecord, record);
    }
}
