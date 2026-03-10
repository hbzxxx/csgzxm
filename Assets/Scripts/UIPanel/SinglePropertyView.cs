using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using cfg;
 
public class SinglePropertyView : SingleViewBase
{
    public int id;
    public int num;

    public Text txt_name;
    public Text txt_num;
    public GameObject obj_add;//加号

    public PropertySetting propertySetting;

    public Transform trans_bar;
    public Image img_bar;
    public Image img_icon;
    public Quality quality;
    public override void Init(params object[] args)
    {
        base.Init(args);
        id = (int)args[0];
        num = (int)args[1];
        quality = (Quality)args[2];//属性品质

        propertySetting = DataTable.FindPropertySetting(id);

    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        Show();
    }
    public virtual void Show()
    {
        txt_name.SetText(propertySetting.Name);

        if (propertySetting.ShowIcon == "1")
        {
            if (img_icon is not null)
            {
                img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.propertyIconFolderPath + propertySetting.UiName);
                img_icon.gameObject.SetActive(true);
            }
        }
        else
        {
            if (img_icon is not null)
            {
                img_icon.gameObject.SetActive(false);
            }
            //txt_name.gameObject.SetActive(true);
        }
        if (propertySetting.ShowBar == "1")
        {
            trans_bar.gameObject.SetActive(true);

        }
        else
        {
            trans_bar.gameObject.SetActive(false);

        }

        switch ((PropertyIdType)propertySetting.Id.ToInt32())
        {
            case PropertyIdType.CritNum:
                txt_num.SetText(num + "%");
                break;
            case PropertyIdType.CritRate:
                txt_num.SetText(num + "%");
                break;
            case PropertyIdType.RdmProDamageAdd:
                txt_num.SetText(num*0.01 + "%");
                break;
            case PropertyIdType.WaterDamageAdd:
                txt_num.SetText(num * 0.01 + "%");
                break;
            case PropertyIdType.FireDamageAdd:
                txt_num.SetText(num * 0.01 + "%");
                break;
            case PropertyIdType.StormDamageAdd:
                txt_num.SetText(num * 0.01 + "%");
                break;
            case PropertyIdType.IceDamageAdd:
                txt_num.SetText(num * 0.01 + "%");
                break;
            case PropertyIdType.YangProDamageAdd:
                txt_num.SetText(num * 0.01 + "%");
                break;
            case PropertyIdType.YinProDamageAdd:
                txt_num.SetText(num * 0.01 + "%");
                break;
            case PropertyIdType.TotalProDamageAdd:
                txt_num.SetText(num * 0.01 + "%");
                break;
            default:
                txt_num.SetText(num.ToString());
                break;
        }
        //txt_name.color = CommonUtil.RarityBlackColor((Rarity)quality);
        //txt_num.color = CommonUtil.RarityBlackColor((Rarity)quality);
        ShowAdd(false);
    }

    /// <summary>
    /// 数字平滑过渡显示
    /// </summary>
    public void NumTxtLerpShow(int targetNum)
    {
        DOTween.To(() => num, (x) =>

        {
            num = x;
            txt_num.SetText(x.ToString());
        }, targetNum, 1).OnComplete(() =>
        {
            num = targetNum;
            txt_num.SetText(targetNum.ToString());
        });

    }

    /// <summary>
    /// 刷新属性显示
    /// </summary>
    public void RefreshProShow()
    {
        Show();
    }

    /// <summary>
    /// 显示加号
    /// </summary>
    public void ShowAdd(bool show)
    {
        if (show)
        {
            RectTransform rect = txt_num.GetComponent<RectTransform>();
            float localX = (rect.localPosition.x-rect.sizeDelta.x/2 + txt_num.preferredWidth + obj_add.GetComponent<RectTransform>().sizeDelta.x / 2);
            float localY = rect.localPosition.y;
            obj_add.transform.localPosition = new Vector2(localX, localY);
            obj_add.gameObject.SetActive(true);
        }
        else
        {
            obj_add.gameObject.SetActive(false);

        }

    }
}
