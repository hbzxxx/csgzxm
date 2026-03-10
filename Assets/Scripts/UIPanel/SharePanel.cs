using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharePanel : PanelBase
{
    public Text txt;
    public Button btn_share;

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_share, () =>
        {
            PanelManager.Instance.ClosePanel(this);
            return;
            TextEditor te = new TextEditor();
            te.text = txt.text;
            te.SelectAll();
            te.Copy();
            PanelManager.Instance.OpenFloatWindow("已复制到剪贴板");
#if !UNITY_EDITOR
            AddQQManager.Instance.CallAndroidMethod("OnShare");
#else
            Application.OpenURL("https://tap.cn/X6N3QCUV?channel=rep-rep_zn0vt3faoh1");
#endif
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt.SetText(ShareManager.Instance.ShareTxt());


    }
}
