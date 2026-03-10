using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//单个社交事件
public class SingleSocializationRecordVew : SingleViewBase
{
    public Text txt_time;//时间
    public Text txt_record;//记录

    public override void Init(params object[] args)
    {
        base.Init(args);
        SocializationRecordData data = args[0] as SocializationRecordData;

        txt_time.SetText(data.time[0] + "年" + data.time[1] + "月" + data.time[2] + "周");
        txt_record.SetText(data.content);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }
}
