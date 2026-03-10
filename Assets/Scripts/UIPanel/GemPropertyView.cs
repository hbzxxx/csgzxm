using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Framework.Data;
using DG.Tweening;
using System;
using cfg;
/// <summary>
/// 宝石属性
/// </summary>
public class GemPropertyView : SingleViewBase
{
    public GemData GemData;
    public SinglePropertyData propertyData;
    public PropertySetting propertySetting;
    public Text txt_name;
    public Text txt_num;
    public Image img_bar;
    public Image img_studentBar;//弟子加成
    public bool havePro;

    public Transform trans_havePro;
    public Transform trans_noPro;

    public int theInitNum;//tween用的初始数字
    public override void Init(params object[] args)
    {
        base.Init(args);
        havePro = (bool)args[0];
        if (havePro)
        {
            propertyData = args[1] as SinglePropertyData;
            propertySetting = DataTable.FindPropertySetting(propertyData.id);
            GemData = args[2] as GemData;
        }
      

    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (havePro)
        {
            txt_num.DOKill();
            img_bar.DOKill();
            img_studentBar.DOKill();
            txt_name.SetText(propertySetting.Name.ToCharArray()[0].ToString());
            string baiFen = "%";
            if(propertyData.id==(int)PropertyIdType.MPSpeed
                ||propertyData.id== (int)PropertyIdType.JingTong)
            {
                baiFen = "";
            }
            txt_num.SetText(propertyData.num + baiFen);
            int limit = ItemManager.Instance.GetGemProLimitById(GemData.gemSettingId);
            img_bar.fillAmount = propertyData.num / (float)limit;
            img_studentBar.fillAmount = 0;
            trans_havePro.gameObject.SetActive(true);
            trans_noPro.gameObject.SetActive(false);

        }
        else
        {
            trans_havePro.gameObject.SetActive(false);
            trans_noPro.gameObject.SetActive(true);
        }
     
    }

    /// <summary>
    /// 缓动 
    /// </summary>
    public void TweenVal(SinglePropertyData singlePropertyData,Action callBack)
    {
        int theNum = 0;
        img_bar.fillAmount = 0;
        string baiFen = "%";
        if (propertyData.id == (int)PropertyIdType.MPSpeed
            || propertyData.id == (int)PropertyIdType.JingTong)
        {
            baiFen = "";
        }
        txt_num.SetText("0"+baiFen);
        theInitNum = singlePropertyData.num;
        int limit = ItemManager.Instance.GetGemProLimitById(GemData.gemSettingId);

        DOTween.To(() => theNum, x =>
        {
            theNum = x;
            txt_num.SetText(x + baiFen);
        }
       , singlePropertyData.num, 1f);
        img_bar.DOFillAmount(singlePropertyData.num / (float)limit, 1).OnComplete(() =>
        {
            callBack?.Invoke();
        });
    }

    /// <summary>
    /// 缓动弟子加成属性
    /// </summary>
    public void TweenStudentVal(SinglePropertyData singlePropertyData)
    {
        int theNum = theInitNum;
        img_studentBar.fillAmount = 0;
        //txt_num.SetText("0%");
        string baiFen = "%";
        if (propertyData.id == (int)PropertyIdType.MPSpeed
            || propertyData.id == (int)PropertyIdType.JingTong)
        {
            baiFen = "";
        }
        int limit = ItemManager.Instance.GetGemProLimitById(GemData.gemSettingId);

        DOTween.To(() => theNum, x =>
        {
            theNum = x;
            txt_num.SetText(x + baiFen);
        }
       , singlePropertyData.num, 1f);
        img_studentBar.DOFillAmount(singlePropertyData.num / (float)limit, 1);
    }

    public override void Clear()
    {
        base.Clear();

    }
}
