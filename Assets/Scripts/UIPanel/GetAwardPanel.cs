
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAwardPanel : PanelBase
{
    public Button btn_fanAll;//翻开所有物品
    public List<GetAwardItemView> viewList = new List<GetAwardItemView>();
    public List<ItemData> itemList;
    public Transform trans_grid;
    public Vector2 startGridPos = new Vector2(0, 167);
    public float delayTime = 0.06f;
    float delayTimer = 0;
    int curShowIndex = 0;
    bool show = false;

    public override void Init(params object[] args)
    {
        base.Init(args);
        itemList = args[0] as List<ItemData>;

        addBtnListener(btn_fanAll, () =>
         {
             show = false;
             for (int i=0;i< itemList.Count; i++)
             {
                 GetAwardItemView view = viewList[i];
                 if (!view.startFade)
                 {
                     view.StartAnim();
                 }
             }
             btn_fanAll.gameObject.SetActive(false);
         });
    }


    public override void OnOpenIng()
    {
        base.OnOpenIng();
        trans_grid.localPosition = startGridPos;
        if (itemList != null&&itemList.Count>0)
        {
            curShowIndex = 0;

            for(int i = 0; i < itemList.Count; i++)
            {
               GetAwardItemView view= PanelManager.Instance.OpenSingle<GetAwardItemView>(trans_grid, itemList[i]);
                viewList.Add(view);
            }
            //viewList[0].StartAnim();
            //show = true;
            //delayTimer = 0;
            //curShowIndex = 1;
            for (int j = 0; j < viewList.Count; j++)
            {
                viewList[j].StartAnim();
            }
            CheckIfShowFanAll();
        }
        AuditionManager.Instance.PlayVoice(AudioClipType.MakeFinish);
    }

    /// <summary>
    /// 是否显示所有牌子
    /// </summary>
    void CheckIfShowFanAll()
    {
        if (curShowIndex < itemList.Count)
            btn_fanAll.gameObject.SetActive(true);
        else
            btn_fanAll.gameObject.SetActive(false);
    }

    //private void Update()
    //{
    //    if (show)
    //    {
    //        delayTimer += Time.deltaTime ;
    //        if (delayTimer >= delayTime)
    //        {
    //            if (curShowIndex < itemList.Count)
    //            {
    //                viewList[curShowIndex].StartAnim();
    //                //PanelManager.Instance.OpenSingle<GetAwardItemView>(trans_grid, itemList[curShowIndex]);
    //                delayTimer = 0;
    //                curShowIndex++;
    //            }
    //            else
    //            {
    //                show = false;
    //            }
    //            CheckIfShowFanAll();
    //
    //        }
    //    }
    //}

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_grid);
        show = false;
        delayTimer = 0;
        viewList.Clear();
    }
    public override void OnClose()
    {
        base.OnClose();
    }
}
