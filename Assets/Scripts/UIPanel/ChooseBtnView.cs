using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBtnView : SingleViewBase
{

    public Button btn;
    public Text txt;
    Action callBack; 
    Func<bool> function;
    public DialogPanel parentPanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        string content = (string)args[0];
        txt.SetText(content);
        callBack = args[1] as Action;
        if (callBack == null)
        {
            function = args[1] as Func<bool>;
        }
        parentPanel = args[2] as DialogPanel;

        addBtnListener(btn,
            ()=> 
            {

                if (callBack != null)
                {         
                    //如果父物体已经在callback里面关闭了，则不用关 因为可能出现这样的情况：回调中又生成一个父物体，然后该按钮又是儿子，这样把刚生成的父物体也关闭了
                    if(parentPanel!=null)
                        PanelManager.Instance.ClosePanel(parentPanel);
                    callBack();
          
                }
                else
                {
                    if (function())
                    {
                        PanelManager.Instance.ClosePanel(parentPanel);
                    }
                }
            }
            
            );
    }

    public override void Clear()
    {
        base.Clear();
        //callBack = null;
        //function = null;
        //parentPanel = null;
    }


}
