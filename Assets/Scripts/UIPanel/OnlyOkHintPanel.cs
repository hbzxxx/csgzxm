using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlyOkHintPanel : PanelBase
{
    public HintData hintData;
    // public Transform trans_blockRange;//这个范围内点击不会关闭一般用bg
    public Button btn_ok;//右边的按钮 ok
    //public Button btn_cancel;//左边的按钮 取消
    public Text txt_content;//内容
    public bool cannotClick = false;
    public override void Init(object[] args)
    {
        base.Init(args);
        this.hintData = args[0] as HintData;
        if (args.Length >= 2)
        {
            cannotClick =(bool) args[1];
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        // SetBlockParent(trans_blockRange);
        RegisterBtnClick();
        ShowDetail();
        if (cannotClick)
            btn_ok.enabled = false;
        else
            btn_ok.enabled = true;
    }

    /// <summary>
    /// 按钮点击事件
    /// </summary>
    void RegisterBtnClick()
    {
        addBtnListener(btn_ok, OnOkClick);
     
    }

    void ShowDetail()
    {
        this.txt_content.text = hintData.content;

        string okBtnTxt = "确定";

        if (hintData.str_okBtn != "")
            okBtnTxt = hintData.str_okBtn;


        //this.btn_ok.GetComponentInChildren<Text>().SetText(okBtnTxt);

    }

    /// <summary>
    /// 点击ok
    /// </summary>
    void OnOkClick()
    {
        if (hintData.okCallBack != null)
            hintData.okCallBack.Invoke();
        PanelManager.Instance.ClosePanel(this);
    }

    public override void Clear()
    {
        base.Clear();
        cannotClick = false;
    }
}
