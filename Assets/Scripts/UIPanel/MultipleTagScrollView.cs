using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleTagScrollView : SingleViewBase
{
    public List<Button> tagBtnList;

    public RectTransform rectTrans_scroll;
    /// <summary>
    /// 小格子
    /// </summary>
    public Transform trans_scrollGrid;


    public override void Init(params object[] args)
    {
        base.Init(args);

        for(int i = 0; i < tagBtnList.Count; i++)
        {
            Button btn = tagBtnList[i];
            int index = i;
            addBtnListener(btn, () =>
            {
                OnTagBtnClick(index);
            });
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        OnTagBtnClick(0);
    }


    /// <summary>
    /// 点了某个标签
    /// </summary>
    /// <param name="index"></param>
    public virtual void OnTagBtnClick(int index)
    {
        
       rectTrans_scroll.SetSiblingIndex(index+1);
        
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_scrollGrid);
    }


}
