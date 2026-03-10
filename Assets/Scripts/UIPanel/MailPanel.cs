using Framework.Data;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailPanel : PanelBase
{
     public List<SingleMailView> mailViewList = new List<SingleMailView>();
    public Transform grid;
    public SingleMailView curChoosedView;

    public Transform trans_singleMailContent;
    public Text txt_mailName;
    public Text txt_mailContent;
    public Text txt_time;
    public Transform trans_itemGrid;
    public Button btn_getSingleMailItem;
    public Transform trans_haveGetSingleMailItem;
    public Button btn_deleteSingleMail;
    public Button btn_getAllMailAward;
    public Button btn_deleteAllReadedMail;
    public override void Init(params object[] args)
    {
        base.Init(args);
  
        RegisterEvent(TheEventType.OnSearchedMyAllMail, OnSearchedMyAllMail);
        RegisterEvent(TheEventType.OnReadMail, OnReadMail);
        RegisterEvent(TheEventType.OnGetMailAward, OnGetMailAward);
        //RegisterEvent(TheEventType.ReceiveMail, OnReceiveMail);
        RegisterEvent(TheEventType.OnGetAllMailAward, OnGetAllMailAward);
        RegisterEvent(TheEventType.OnDeleteMail, OnDeleteMail);

 

        addBtnListener(btn_getAllMailAward, () =>
        {
   

        });
        //删除
        addBtnListener(btn_deleteSingleMail, () =>
        {
       

        });
        //删除所有已读
        addBtnListener(btn_deleteAllReadedMail, () =>
        {
 
        });

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    /// <summary>
    /// 选择了邮件
    /// </summary>
    /// <param name="view"></param>
    public void OnChoosedMail(SingleMailView view)
    {
        for(int i = 0; i < mailViewList.Count; i++)
        {
            SingleMailView theView = mailViewList[i];
            if (theView == view)
            {
                curChoosedView = theView;
                ShowContent();
            }
        }
    }
    void ShowAllMail()
    {
        mailViewList.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
 
        if (mailViewList.Count > 0)
        {
            mailViewList[0].btn.onClick.Invoke();
        }
        else
        {
            trans_singleMailContent.gameObject.SetActive(false);
        }

    }


    public void ShowContent()
    {
        if (curChoosedView != null)
        {
            trans_singleMailContent.gameObject.SetActive(true);
            ClearCertainParentAllSingle<SingleViewBase>(trans_itemGrid);
           
   
        }

    }
    void OnSearchedMyAllMail(object[] args)
    {
          ShowAllMail();
    }
    void OnReadMail(object[] args)
    {
        
    }

    void OnGetMailAward(object[] args)
    {
         
    }
    /// <summary>
    /// 领所有奖励
    /// </summary>
    void OnGetAllMailAward(object[] args)
    {
 
        if (curChoosedView != null)
        {
            curChoosedView.RefreshShow();
        }
        ShowContent();
    }

    ///// <summary>
    ///// 收到新邮件
    ///// </summary>
    //void OnReceiveMail()
    //{
    //    Game.Instance.clientManager.SendRT(NetCmd.EntityRpc, "RequestMailList");
    //}
    /// <summary>
    /// 删除邮件
    /// </summary>
    void OnDeleteMail(object[] args)
    {
         ShowAllMail();
    }
    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
         ClearCertainParentAllSingle<SingleViewBase>(trans_itemGrid);

        trans_singleMailContent.gameObject.SetActive(false);
    }
}
