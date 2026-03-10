using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Framework.Data;
using UnityEngine.UI;

public class SingleEconomyView : SingleViewBase
{
    public Text txt_danFarmName;//丹田名
    public Text txt_studentNum;//弟子量
    public Text txt_workStatus;//工作状态
    public Text txt_efficient;//效率
    public Transform trans_efficientBar;//效率
    public Image img_efficientBar;//效率
    public Transform trans_product;//产品父物体
    public Image img_product;//产品
    public Text txt_moonProductNum;//月产量
    public int itemId;//物品 
    public SingleDanFarmData singleDanfarmData;
    public DanFarmSetting singleDanFarmSetting;
    public Button btn;
    public GameObject kuohao;

    public override void Init(params object[] args)
    {
        base.Init(args);
        
        singleDanfarmData = args[0] as SingleDanFarmData;
        singleDanFarmSetting = DataTable.FindDanFarmSetting(singleDanfarmData.SettingId);

        addBtnListener(btn, () =>
        {
            if(singleDanfarmData.Status==(int)DanFarmStatusType.Idling
            || singleDanfarmData.Status == (int)DanFarmStatusType.Working)
            {
                LianDanManager.Instance.OnClickedFarm(singleDanfarmData);

            }
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        
        txt_danFarmName.SetText(singleDanFarmSetting.Name);
        int studentNum = 0;
        for(int i = 0; i < singleDanfarmData.ZuoZhenStudentIdList.Count; i++)
        {
            if (singleDanfarmData.ZuoZhenStudentIdList[i] > 0)
            {
                studentNum++;
            }
        }
        
        txt_studentNum.SetText(studentNum + "/" + LianDanManager.Instance.SingleFarmDataUnlockedStudentPosCount(singleDanfarmData));
        string workstatus = LianDanManager.Instance.DanFarmStatusShow((DanFarmStatusType)singleDanfarmData.Status);
        txt_workStatus.SetText(workstatus);

        if (singleDanfarmData.DanFarmWorkType == (int)DanFarmWorkType.Common)
        {
            if (singleDanfarmData.Status == (int)DanFarmStatusType.Working)
            {
                txt_efficient.SetText(LianDanManager.Instance.CalcDanFarmEfficient(singleDanfarmData) + "%");
                img_efficientBar.fillAmount = (LianDanManager.Instance.CalcDanFarmEfficient(singleDanfarmData) / (LianDanManager.Instance.SingleFarmDataUnlockedStudentPosCount(singleDanfarmData) * 100f));
                txt_efficient.gameObject.SetActive(true);
                trans_efficientBar.gameObject.SetActive(true);
            }
            else
            {
                txt_efficient.gameObject.SetActive(false);
                trans_efficientBar.gameObject.SetActive(false);
            }


        }
        else
        {
            txt_efficient.gameObject.SetActive(false);
            trans_efficientBar.gameObject.SetActive(false);
        }

        if (singleDanfarmData.DanFarmWorkType == (int)DanFarmWorkType.Common
            && singleDanfarmData.Status != (int)DanFarmStatusType.Building)
        {
            trans_product.gameObject.SetActive(true);
            img_product.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + DataTable.FindItemSetting(singleDanfarmData.ProductSettingId).UiName);
            txt_moonProductNum.SetText(LianDanManager.Instance.CalcDanFarmProducePerMonth(singleDanfarmData).ToString());
        }
        else
        {
            trans_product.gameObject.SetActive(false);

        }

    }




}
