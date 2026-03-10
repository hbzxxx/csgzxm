using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemTagScrollView : MultipleTagScrollView
{
    public LianDanBuildingPanel parentPanel;
    public List<GemPictureView> curShowPictureViewList = new List<GemPictureView>();
    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[0] as LianDanBuildingPanel; 
    }

    public override void OnTagBtnClick(int index)
    {
        base.OnTagBtnClick(index);
        PanelManager.Instance.CloseAllSingle(trans_scrollGrid);
        //List<int> settingIdList = unlockedGemPictureId((Rarity)(index+1));
        //curShowPictureViewList.Clear();
        //for (int i = 0; i < settingIdList.Count; i++)
        //{
        //    ItemData item = new ItemData();
        //    item.SettingId = settingIdList[i];
        //    GemPictureView view= PanelManager.Instance.OpenSingle<GemPictureView>(trans_scrollGrid, item, parentPanel);
        //    curShowPictureViewList.Add(view);
        //}
        //if (curShowPictureViewList.Count > 0)
        //{
        //    curShowPictureViewList[0].btn.onClick.Invoke();
        //}
    }
 
}