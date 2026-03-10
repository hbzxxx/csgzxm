using cfg;
using DG.Tweening;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleTongZhiView : SingleViewBase
{
    public Image bg;
    public Text txt;
    public float stayTime = 3f;
    public float stayTimer = 0;
    public Transform trans_initPos;
    public Transform trans_endPos;

    public Transform trans_common;//普通
    public Transform trans_consume;//消耗
    public Image img_consumeIcon;
    public Text txt_consumeTxt;//消耗
    public Text txt_consumeNumTxt;//消耗量
    public ConsumeType consumeType;
    public TongZhiType tongZhiType;
    
    public override void Init(params object[] args)
    {
        base.Init(args);
        tongZhiType = (TongZhiType)args[0];
        int consumeNum = 0;
        switch (tongZhiType)
        {
            case TongZhiType.Common:
                trans_common.gameObject.SetActive(true);
                trans_consume.gameObject.SetActive(false);
                txt.SetText((string)args[1]);
                break;
            case TongZhiType.Consume:
                trans_common.gameObject.SetActive(false);
                trans_consume.gameObject.SetActive(true);

                consumeType = (ConsumeType)args[2];
                int id = (int)args[3];
                consumeNum = (int)args[4];

                txt_consumeTxt.SetText((string)args[1]);
                txt_consumeTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(txt_consumeTxt.preferredWidth, txt_consumeTxt.GetComponent<RectTransform>().sizeDelta.y);
                string name = "";
                switch (consumeType)
                {
                    case ConsumeType.Item:
                        ItemSetting itemSetting = DataTable.FindItemSetting(id);
                        name = itemSetting.Name;
                        img_consumeIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + itemSetting.UiName);
                        break;
                    case ConsumeType.Property:
                        PropertySetting propertySetting = DataTable.FindPropertySetting(id);
                        name = propertySetting.Name;
                        img_consumeIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.propertyIconFolderPath + propertySetting.UiName);
                        break;

                }

                string addStr = "";
                if (consumeNum > 0)
                {
                    txt.SetText("获得");
                    addStr = name + "+";

                }
                else
                {
                    txt.SetText("失去");
                    addStr = name;
                }
                txt_consumeNumTxt.SetText(addStr+consumeNum.ToString());
                break;
        }

        bg.DOKill();
        txt.DOKill();
        txt_consumeTxt.DOKill();
        img_consumeIcon.DOKill();
        txt_consumeNumTxt.DOKill();

        bg.transform.position = trans_initPos.position;
        bg.color = new Color32(0, 0, 0, 115);
        txt.color = Color.white;
        txt_consumeTxt.color = Color.white;
        img_consumeIcon.color = Color.white;

        if (consumeNum > 0)
            txt_consumeNumTxt.color = Color.green;
        else
            txt_consumeNumTxt.color = Color.red;


        stayTimer = 0;

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

  

       

    }

    private void Update()
    {
        stayTimer += Time.deltaTime;
        if (stayTimer >= stayTime)
        {
            bg.transform.DOLocalMoveY(trans_endPos.localPosition.y, .8f).OnComplete(() =>
            {
                PanelManager.Instance.CloseSingle(this);
            });
            bg.DOFade(0, .8f);
            txt.DOFade(0, .8f);
            txt_consumeTxt.DOFade(0, .8f);
            img_consumeIcon.DOFade(0, .8f);

            stayTimer = 0;
        }
    }

    public override void Clear()
    {
        base.Clear();
        bg.DOKill();
        txt.DOKill();
        txt_consumeTxt.DOKill();
        img_consumeIcon.DOKill();
        txt_consumeNumTxt.DOKill();
    }
}
/// <summary>
/// 通知类型
/// </summary>
public enum TongZhiType
{
    None=0,
    Common,
    Consume,//有消耗图标的
}