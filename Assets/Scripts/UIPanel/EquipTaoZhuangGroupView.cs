using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipTaoZhuangGroupView : SingleViewBase
{
    EquipTaoZhuangType type;
    List<SingleEquipPictureData> pictureDataList;

    public Text txt_taoZhuangName;
    public Text txt_taoZhuangDes;
    public Transform grid;
    EquipTaoZhuangSetting taoZhuangSetting;
    public List<EquipPictureView> pictureViewList = new List<EquipPictureView>();
    public NewEquipPreparePanel parentPanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        type = (EquipTaoZhuangType)args[0];
        pictureDataList = args[1] as List<SingleEquipPictureData>;
        parentPanel = args[2] as NewEquipPreparePanel;
        if (type!=EquipTaoZhuangType.None)
        taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)type);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        string taoZhuangName = "";
        string taoZhuangDes = "";
        if (type == EquipTaoZhuangType.None)
        {
         
            taoZhuangName =  LanguageUtil.GetLanguageText((int)LanguageIdType.普通法器);
            taoZhuangDes = "无套装效果";
        }
        else
        {
            taoZhuangName = taoZhuangSetting.Name;
            string des1 = EquipmentManager.Instance.TaoZhuangDes(type, false);
            string des2 = EquipmentManager.Instance.TaoZhuangDes(type, true);
            taoZhuangDes = des1 + "\n" + des2;
        }

        txt_taoZhuangName.SetText(taoZhuangName);
        txt_taoZhuangDes.SetText(taoZhuangDes);
        for(int i = 0; i < pictureDataList.Count; i++)
        {
            pictureViewList.Add(PanelManager.Instance.OpenSingle<EquipPictureView>(grid, pictureDataList[i],parentPanel));
        }
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(grid);
        pictureViewList.Clear();
    }
}
