
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchivePanel : PanelBase
{

    public Transform grid;
    public ScrollViewNevigation scrollViewNevigation;
    public List<SingleArchiveView> archiveViewList = new List<SingleArchiveView>();

    public Transform grid_choosedFu;//选择的服务器父物体

    public override void Init(params object[] args)
    {
        base.Init(args);
        RegisterEvent(TheEventType.OnDeleteArchive, OnDeleteArchive);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        InitShow();
    }
    void InitShow()
    {
        ClearCertainParentAllSingle<SingleViewBase>(grid);
        ClearCertainParentAllSingle<SingleViewBase>(grid_choosedFu);

        // List<int> validFuIndex = M_ServerData.ClientServerIndexList(Game.Instance.neiCeFu);
        // for (int i = 0; i < ArchiveManager.Instance.allArchiveList.Count; i++)
        // {
        //     if (!validFuIndex.Contains(i))
        //     {
        //         continue;
        //     }
        //     //GameInfo gameInfo = ArchiveManager.Instance.allArchiveList[i];
        //     //bool recent = false;
        //     //if (i == ArchiveManager.Instance.recentArchiveIndex)
        //     //{
        //     //    recent = true;
        //     //}

        //     AddSingle<SingleFuView>(grid, i, this);
            
        //     //if (gameInfo == null)
        //     //    archiveViewList.Add(AddSingle<SingleArchiveView>(grid, false,i,null,false,this));
        //     //else
        //     //    archiveViewList.Add(AddSingle<SingleArchiveView>(grid, true, i,gameInfo, recent,this));

        // }
        //定位到最近存档
        //if(ArchiveManager.Instance.recentArchiveIndex>0)
        //PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, archiveViewList[ArchiveManager.Instance.recentArchiveIndex].gameObject, false);
    }

    public void OnChoosedFu(int fuIndex)
    {
        ClearCertainParentAllSingle<SingleViewBase>(grid_choosedFu);

        GameInfo gameInfo = ArchiveManager.Instance.allArchiveList[fuIndex];
        bool recent = false;
        if (fuIndex == ArchiveManager.Instance.recentArchiveIndex)
        {
            recent = true;
        }

        if (gameInfo == null)
            archiveViewList.Add(AddSingle<SingleArchiveView>(grid_choosedFu, false, fuIndex, null, false, this));
        else
            archiveViewList.Add(AddSingle<SingleArchiveView>(grid_choosedFu, true, fuIndex, gameInfo, recent, this));
    }

    /// <summary>
    /// 删除了存档
    /// </summary>
    void OnDeleteArchive()
    {
        archiveViewList.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
        ClearCertainParentAllSingle<SingleViewBase>(grid_choosedFu);

        InitShow();
    }

    public override void Clear()
    {
        base.Clear();
        archiveViewList.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
        ClearCertainParentAllSingle<SingleViewBase>(grid_choosedFu);

    }
}
