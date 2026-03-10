using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 历练描述
/// </summary>
public class SingleLiLianDesView : SingleViewBase
{
    public Text txt_content;
    public string str;
    public float txtOffset;
    public override void Init(params object[] args)
    {
        base.Init(args);
        str = (string)args[0];
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowContent();
    }

    /// <summary>
    /// 显示内容
    /// </summary>
    public void ShowContent()
    {
       txt_content.SetText(str);
     

        //RectTransform txtRect = txt_content.GetComponent<RectTransform>();
        //txtRect.sizeDelta = new Vector2(txtRect.sizeDelta.x, txt_content.preferredHeight + 5);
        //RectTransform theRect = GetComponent<RectTransform>();

        //float theHeight = txtRect.sizeDelta.y + txtOffset;
        //theRect.sizeDelta = new Vector2(theRect.sizeDelta.x, theHeight);

    }
}
