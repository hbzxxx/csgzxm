using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SingleLiLianView : SingleViewBase
{
    public int id;//id
    public Image img_bg;//玩法背景图
    public Image img_icon;//玩法图标
    public Text txt_title;//玩法标题
    public Button btn;
    //public Text txt_name;
    public LeLiLianData data;

    public override void Init(params object[] args)
    {
        base.Init(args);
        data = args[0] as LeLiLianData;
        id = data.settingId;
        //图
        img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + data.iconName);
        //标题
        txt_title.SetText(data.title);

        //根据关卡是否解锁来设置背景
        if (!LiLianManager.Instance.liLianEnabled)
            if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex < 10)
            {

            }
            else
            {
                img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + data.bgName);
            }
        

        LiLianSetting setting = DataTable.FindLiLianSetting(id);
        int timeIndex = DataTable._liLianList.IndexOf(setting);
        int curParticipatedNum = RoleManager.Instance._CurGameInfo.timeData.TodayParticipatedLiLianStatus[timeIndex];
        //txt_name.SetText((RoleManager.Instance._CurGameInfo.timeData.MaxLiLianTimePerDay - curParticipatedNum)+"/"+ RoleManager.Instance._CurGameInfo.timeData.MaxLiLianTimePerDay);

        addBtnListener(btn, () =>
        {
            OnClickedView();
          
        });
    }

    void OnClickedView()
    {
        // 检查历练功能开关
        //if (!LiLianManager.Instance.liLianEnabled)
        //{
        //    PanelManager.Instance.OpenFloatWindow("历练功能已关闭");
        //    return;
        //}
        if (!LiLianManager.Instance.liLianEnabled) {
            if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex < 10)
            {
                PanelManager.Instance.OpenFloatWindow((RoleManager.Instance._CurGameInfo.playerPeople.trainIndex + 1) + "级开启");
                return;
            }
        }
        LiLianSetting setting = DataTable.FindLiLianSetting(id);
        int timeIndex = DataTable._liLianList.IndexOf(setting);
        int curParticipatedNum = RoleManager.Instance._CurGameInfo.timeData.TodayParticipatedLiLianStatus[timeIndex];
        if (!LiLianManager.Instance.liLianEnabled)
            if (curParticipatedNum >= RoleManager.Instance._CurGameInfo.timeData.MaxLiLianTimePerDay)
            {
                PanelManager.Instance.OpenFloatWindow("今日参加历练次数已达上限");
            }
            else
            {
                PanelManager.Instance.OpenPanel<LiLianPanel>(PanelManager.Instance.trans_layer2, id);
            }
        else{
            PanelManager.Instance.OpenPanel<LiLianPanel>(PanelManager.Instance.trans_layer2, id);
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    public override void Clear()
    {
        base.Clear();
    }
}
