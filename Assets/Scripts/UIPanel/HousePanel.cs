using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HousePanel : PanelBase
{
    public Button btn_tree;
    public Button btn_train;

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_tree, () =>
        {
            EventCenter.Broadcast(TheEventType.CloseMainPanel);
            GameSceneManager.Instance.GoToScene(SceneType.Tree);

        });

   
    }

}
