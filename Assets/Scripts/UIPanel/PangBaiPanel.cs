using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PangBaiPanel : PanelBase
{
    public float txtTimer;//计时器
    public float TxtSpeed;//文字速度
    public string content;

    int curIndex = 0;
    public Text txt;
    bool finishPangBai = false;
    public Button btn;
    public Action closeCallBack;
    public override void Init(params object[] args)
    {
        base.Init(args);
        content = (string)args[0];
        closeCallBack = args[1] as Action;
        txtTimer = 0;
        curIndex = 0;
        finishPangBai = false;
        txt.SetText(string.Empty);
        addBtnListener(btn, () =>
        {
            if (!finishPangBai)
            {
                finishPangBai = true;
                txt.SetText(content);
            }
            else
            {
                PanelManager.Instance.ClosePanel(this);

                if (closeCallBack != null)
                    closeCallBack();
            }
        
          
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        
    }


    private void Update()
    {
        if (!finishPangBai)
        {
            if (curIndex > content.Length)
            {
                finishPangBai = true;
                return;

            }
            txtTimer += Time.deltaTime;
            if (txtTimer >= TxtSpeed)
            {
                txt.SetText(content.Substring(0, curIndex));
                curIndex++;
                txtTimer = 0;
            }
        }
    
    }
}
