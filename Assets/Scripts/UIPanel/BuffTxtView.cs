using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffTxtView : SingleViewBase
{
    public Image img;
    public Text txt;
    public RectTransform rect;
    public Button btn;
    public Color32 goodTxtColor = new Color32(23, 49, 53, 255);
    public Color32 badTxtColor = new Color32(38, 11, 5, 255);
    public override void Init(params object[] args)
    {
        base.Init(args);
        BattleBuff battleBuff = args[0] as BattleBuff;

        string huihe = "";
        if (battleBuff.remainHuiHe < 999)
        {
            huihe = battleBuff.remainHuiHe.ToString();
        }
        if (battleBuff.buffSetting.Bad == "1")
        {
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.battleBuffBadBg);
            txt.color = badTxtColor;
        }
        else
        {
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.battleBuffGoodBg);
            txt.color = goodTxtColor;

        }
        txt.SetText(battleBuff.buffSetting.Name+ huihe);
        //txt.rectTransform.sizeDelta = new Vector2(txt.preferredWidth, rect.sizeDelta.y);
        //img.rectTransform.sizeDelta = new Vector2(txt.preferredWidth, rect.sizeDelta.y);
        //rect.sizeDelta = new Vector2(txt.preferredWidth, rect.sizeDelta.y);

        addBtnListener(btn, () =>
        {
            PanelManager.Instance.OpenPanel<NoTransparentTipsPanel>(PanelManager.Instance.trans_layer2, battleBuff.buffSetting.Des,(Vector2)transform.position);
        });
    }
}
