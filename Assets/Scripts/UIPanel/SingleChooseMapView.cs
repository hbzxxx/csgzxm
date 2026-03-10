using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SingleChooseMapView : SingleViewBase
{

    public SingleMapData singleMapData;
    MapSetting mapSetting;
    public Text txt_mapName;
    public Image img_bg;
    public Button btn;
    public Transform trans_lock;


    public override void Init(params object[] args)
    {
        base.Init(args);
        singleMapData = args[0] as SingleMapData;
 
        mapSetting = DataTable.FindMapSetting(singleMapData.MapId);

        if (singleMapData.MapStatus == (int)AccomplishStatus.Locked)
        {
             trans_lock.gameObject.SetActive(true);
        }
        else
        {
            trans_lock.gameObject.SetActive(false);
        }
        addBtnListener(btn, () =>
        {
            //未解锁
            if (singleMapData.MapStatus == (int)AccomplishStatus.Locked)
            {
                PanelManager.Instance.OpenFloatWindow("通过上一个地图后解锁");
                return;
            }
            else
            {
                //if (parentPanel != null)
                //    parentPanel.ChooseMap(this);
                //else
                //    fixedParentPanel.ChooseMap(this);
            }

        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_mapName.SetText(mapSetting.Name);
    }

    public void OnChoose(bool choose)
    {
        if (choose)
        {
            img_bg.color = ConstantVal.color_choosed;
        }
        else
        {
            img_bg.color = Color.white;
        }
    }
}
