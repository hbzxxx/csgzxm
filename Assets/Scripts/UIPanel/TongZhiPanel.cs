 using System.Collections;
using System.Collections.Generic;
using Framework.Data;
using UnityEngine;

public class TongZhiPanel : PanelBase
{
    public Transform trans_grid;

    public override void Init(params object[] args)
    {
        base.Init(args);

        TongZhiType tongZhiType = (TongZhiType)args[0];
        string content = (string)args[1];

        switch (tongZhiType)
        {
            case TongZhiType.Common:
               AddSingle<SingleTongZhiView>(trans_grid, TongZhiType.Common, content);

                break;
            case TongZhiType.Consume:
                ConsumeType consumeType = (ConsumeType)args[2];
                int id = (int)args[3];
                int consumeNum = (int)args[4];
                if (DataTable.FindItemSetting(id) == null) break;
                    AddSingle<SingleTongZhiView>(trans_grid, TongZhiType.Consume, content, consumeType, id, consumeNum);
                if (consumeNum > 0)
                {
                    AuditionManager.Instance.PlayVoice(AudioClipType.GetJinBi);

                }
                break;
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.tongZhiPanel = null;
    }
}
