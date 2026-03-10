using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TuPoShiBaiSingle : SingleViewBase
{
    public Button btn_close;
    public Transform trans_content;
    public Text txt_loseNum;
    public Text txt_successAdd;
    public float showContentTime = 1.5f;
    public float showContentTimer = 0;
    public bool showContent = false;
    public override void Init(params object[] args)
    {
        base.Init(args);
        int loseNum = (int)args[0];
        int add = (int)args[1];
        trans_content.gameObject.SetActive(false);
        showContentTimer = 0;
        showContent = true;
        txt_loseNum.SetText(loseNum.ToString());
        txt_successAdd.SetText("下次突破成功率+" + add + "%");

        addBtnListener(btn_close, () =>
        {
            PanelManager.Instance.CloseSingle(this);
        });
    }
    private void Update()
    {
        if (showContent)
        {
            showContentTimer += Time.deltaTime;
            if (showContentTimer >= showContentTime)
            {
                showContent = false;
                trans_content.gameObject.SetActive(true);
            }
        }

    }

    public override void Clear()
    {
        base.Clear();
        showContent = false;
    }
}
