using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherFunctionFarmDetailPanel : PanelBase
{
    public Image img_icon;
    public Text txt_name;
    public Text txt_des;
    public DanFarmSetting danFarmSetting;

    public Text txt_taoZhuangLabel;
    public Text txt_unlockedTaoZhuang;
    public Text txt_taoZhuangDes;
    public Text txt_needTao;

    public override void Init(params object[] args)
    {
        base.Init(args);
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        danFarmSetting = DataTable.FindDanFarmSetting(data.SettingId);
        txt_name.SetText(danFarmSetting.Name);
        txt_des.SetText(danFarmSetting.Des);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        img_icon.sprite =  ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + danFarmSetting.UiName);

        if (!string.IsNullOrWhiteSpace(danFarmSetting.Param))
        {
            string[] arr = danFarmSetting.Param.Split('|');
            txt_taoZhuangLabel.gameObject.SetActive(true);

            string taoZhuangLabel = arr[0];
            string taoZhuangDes = arr[1];

            bool allActivated = true;
            string needTao = "";
            //是否激活
            for(int i = 2; i < arr.Length; i++)
            {
                int farmId = arr[i].ToInt32();
                DanFarmSetting farmSetting = DataTable.FindDanFarmSetting(farmId);

                SingleDanFarmData farm = LianDanManager.Instance.FindMyFarm(farmId);
                if (farm != null)
                {
                    needTao += "<color=green><b>" + farmSetting.Name + "</b></color>" + "\n";
                }
                else
                {
                    needTao += farmSetting.Name+"\n";
                    allActivated = false;
                }
            }

            txt_needTao.SetText(needTao);

            if (allActivated)
            {
                txt_taoZhuangLabel.SetText("<color=green><b>" + taoZhuangLabel + "</b></color>"  );
                txt_taoZhuangDes.SetText("<color=green><b>" + taoZhuangDes + "</b></color>"  );
                txt_unlockedTaoZhuang.SetText("<color=green><b>" + "（已激活）"+ "</b></color>");

            }
            else
            {
                txt_taoZhuangLabel.SetText(  taoZhuangLabel );
                txt_taoZhuangDes.SetText( taoZhuangDes );
                txt_unlockedTaoZhuang.SetText(  "（未激活）" );
            }

        }
        else
        {
            txt_taoZhuangLabel.gameObject.SetActive(false);
        }
    }

}
