
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonDanTagScrollView : MultipleTagScrollView
{
    public LianDanBuildingPanel parentPanel;
    public List<CommonDanPictureView> curShowPictureViewList = new List<CommonDanPictureView>();

    //暂时只做这几种破镜丹
    public List<ItemIdType> commonDanIdList = new List<ItemIdType>() { ItemIdType.PoJingDan_Fan,
    ItemIdType.PoJingDan_Huang,
    ItemIdType.PoJingDan_Xuan,
    ItemIdType.PoJingDan_Di,
    ItemIdType.PoJingDan_Tian,};

    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[0] as LianDanBuildingPanel;
    }

    public override void OnTagBtnClick(int index)
    {
        base.OnTagBtnClick(index);
        PanelManager.Instance.CloseAllSingle(trans_scrollGrid);
        //List<int> settingIdList = unlockedGemPictureId((Rarity)(index + 1));
        curShowPictureViewList.Clear();
        //for (int i = 0; i < commonDanIdList.Count; i++)
       // {
            ItemData item = new ItemData();
            item.settingId =(int)commonDanIdList[index];
            CommonDanPictureView view = PanelManager.Instance.OpenSingle<CommonDanPictureView>(trans_scrollGrid, item, parentPanel);
            curShowPictureViewList.Add(view);
        //}
        //curShowPictureViewList[0].btn.onClick.Invoke();
    }
}
