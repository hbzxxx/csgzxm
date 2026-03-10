using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockFunctionPanel : PanelBase
{
    public Text txt_functionName;//功能
    public float disappearTimer = 0;
    public float disappearTime = 1f;//多久消失

    public override void Init(params object[] args)
    {
        base.Init(args);
        string content = (string)args[0];
        txt_functionName.SetText(content);
        disappearTimer = 0;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    private void Update()
    {
       
        disappearTimer += Time.deltaTime;
        if (disappearTimer >= disappearTime)
        {
            PanelManager.Instance.CloseUnlockFunction(txt_functionName.text);

            PanelManager.Instance.ClosePanel(this);
         }
        
    }
}
