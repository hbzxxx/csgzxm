using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedSingleMapScenePanel_3 : SingleMapScenePanel
{
    public Transform trans_layer1;
    public Transform trans_layer2;

    public Button btn_enter;//进入
    public Button btn_out;//出城
    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_enter, () =>
        {
            PanelManager.Instance.PingPongBlackMask(() =>
            {
                EnterLayer(2);
            }, null);
        });
        addBtnListener(btn_out, () =>
        {
            PanelManager.Instance.PingPongBlackMask(() =>
            {
                EnterLayer(1);
            }, null);
        });
    }
    public void EnterLayer(int layer)
    {
        MapManager.Instance.curMapIndex = layer;
        if (layer == 1)
        {
            trans_layer1.gameObject.SetActive(true);
            trans_layer2.gameObject.SetActive(false);
        }
        else
        {
            trans_layer2.gameObject.SetActive(true);
            trans_layer1.gameObject.SetActive(false);

        }

        for (int i = 0; i < levelViewList.Count; i++)
        {
 
            levelViewList[i].InitShow();
        }
    }
}
