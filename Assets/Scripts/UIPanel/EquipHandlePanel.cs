
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipHandlePanel : PanelBase
{
    public Transform trans_sonParent;
    ItemData item;
    public Button btn_intense;
    public Button btn_jingLian;
    public Button btn_gem;
    public List<Button> btns;

    public override void Init(params object[] args)
    {
        base.Init(args);
        item = args[0] as ItemData;

        addBtnListener(btn_intense, () =>
        {
            PanelManager.Instance.CloseAllPanel(trans_sonParent);
            List<SingleDanFarmData> farmList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.LianQi);
            if (farmList.Count > 0)
            {
                //SingleDanFarmData farmData=ZongMenManager.Instance.FindTypeFarmList(DanFarmType.LianQi)
                PanelManager.Instance.OpenPanel<EquipIntensePanel>(trans_sonParent, farmList[0], item);
            }
            else
            {
                PanelManager.Instance.OpenFloatWindow("您没有炼器房，请造一个");
            }
            //PanelManager.Instance.OpenPanel<EquipIntensePanel>(trans_sonParent, item);
            CommonUtil.BtnChooseColor(btns, btn_intense,new Color32(246, 246, 246,255),new Color32(246,224,196,255));
        });
        addBtnListener(btn_jingLian, () =>
        {
            PanelManager.Instance.CloseAllPanel(trans_sonParent);
            PanelManager.Instance.OpenPanel<EquipJingLianPanel>(trans_sonParent, item);
            CommonUtil.BtnChooseColor(btns, btn_jingLian, new Color32(246, 246, 246, 255), new Color32(246, 224, 196, 255));
        });
        addBtnListener(btn_gem, () =>
        {
            PanelManager.Instance.CloseAllPanel(trans_sonParent);
            PanelManager.Instance.OpenPanel<EquipAddGemPanel>(trans_sonParent, item);
            CommonUtil.BtnChooseColor(btns, btn_gem, new Color32(246, 246, 246, 255), new Color32(246, 224, 196, 255));
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        btn_intense.onClick.Invoke();
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllPanel(trans_sonParent);
    }
}
