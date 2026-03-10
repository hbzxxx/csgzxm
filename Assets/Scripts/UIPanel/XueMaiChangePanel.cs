using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 改变血脉
/// </summary>
public class XueMaiChangePanel : PanelBase
{
    public Button btn_change;//改变
     public YuanSuType curChoosedYuanSu;//当前选择的元素
    public List<SingleChangeXueMaiView> singleChangeXueMaiViewList;
    public Image img_center;
    public GameObject obj_canUnlockNewXueMai;
    public Button btn_wenHao;
    public override void Init(params object[] args)
    {
        base.Init(args);
        for (int i = 0; i < singleChangeXueMaiViewList.Count; i++)
        {
            singleChangeXueMaiViewList[i].Init(this);
            singleChangeXueMaiViewList[i].OnOpenIng();
        }
        addBtnListener(btn_change, () =>
         {
             PanelManager.Instance.OpenCommonHint("改变属性后，"+LanguageUtil.GetLanguageText((int)LanguageIdType.功法)+"需要重新装配，确定改变吗？", () =>
             {
                 RoleManager.Instance.ChangeYuanSu(RoleManager.Instance._CurGameInfo.playerPeople, curChoosedYuanSu);
                 PanelManager.Instance.OpenFloatWindow("改变属性成功");
             }, null);
         });
        addBtnListener(btn_wenHao, () =>
        {
 
        });
        RegisterEvent(TheEventType.UnlockYuanSu, RefreshShow);
        RegisterEvent(TheEventType.ChangeYuanSu, RefreshShow);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
    }
    void RefreshShow()
    {
        for (int i = 0; i < singleChangeXueMaiViewList.Count; i++)
        {
            singleChangeXueMaiViewList[i].Init(this);
            singleChangeXueMaiViewList[i].OnOpenIng();
        }
        img_center.sprite = ConstantVal.YuanSuIcon((YuanSuType)RoleManager.Instance._CurGameInfo.playerPeople.yuanSu);
        btn_change.gameObject.SetActive(false);
        if (RoleManager.Instance.CandidateYuanSuNum() > RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Count)
        {
            obj_canUnlockNewXueMai.gameObject.SetActive(true);
        }
        else
        {
            obj_canUnlockNewXueMai.gameObject.SetActive(false);

        }
    }
    /// <summary>
    /// 元素选择
    /// </summary>
    public void OnChoosedYuanSu(SingleChangeXueMaiView view)
    {
        for(int i=0;i< singleChangeXueMaiViewList.Count; i++)
        {
            SingleChangeXueMaiView theView = singleChangeXueMaiViewList[i];
            if (theView == view)
            {
                theView.OnChoosed(true);
            }
            else
            {
                theView.OnChoosed(false);
            }
        }

        if (view.yuanSuType == (YuanSuType)RoleManager.Instance._CurGameInfo.playerPeople.yuanSu)
        {
            btn_change.gameObject.SetActive(false);
        }
        else
        {
            curChoosedYuanSu = view.yuanSuType;
            //如果是已解锁的
            if (RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Contains((int)view.yuanSuType))
            {
                btn_change.gameObject.SetActive(true);
            }
            //如果是未解锁的
            else
            {
                if (RoleManager.Instance.CandidateYuanSuNum() > RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Count)
                {
                    PanelManager.Instance.OpenCommonHint("确定解锁" + ConstantVal.YuanSuName(view.yuanSuType) + "属性吗？", () =>
                    {
                        RoleManager.Instance.UnlockYuanSu(RoleManager.Instance._CurGameInfo.playerPeople, curChoosedYuanSu);

                    }, null);
                }
                else
                {
                    PanelManager.Instance.OpenFloatWindow("掌门修为达到下个大境界可解锁新的属性");
                    btn_change.gameObject.SetActive(false);

                }
            }

        }
    }
    
    public override void Clear()
    {
        base.Clear();
        for(int i = 0; i < singleChangeXueMaiViewList.Count; i++)
        {
            singleChangeXueMaiViewList[i].Clear();
        }
    }
}
