using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChapterProcessView : SingleViewBase
{
    SingleChapterGuideBookData singleChapterGuideBookData;
    public Button btn;
    public int index;
    public GameObject obj_gotAward;
    public Text txt;
    GuideBookPanel parentPanel;
    public GameObject obj_redPoint;
    public override void Init(params object[] args)
    {
        base.Init(args);
         singleChapterGuideBookData = args[0] as SingleChapterGuideBookData;
         index = (int)args[1];
        parentPanel = args[2] as GuideBookPanel;
        addBtnListener(btn, () =>
        {
            parentPanel.OnChoosedProcess(index);
        });
        RegisterEvent(TheEventType.NevigateScrollAutoPoint, OnNevigated);
        RegisterEvent(TheEventType.RefreshGuideBookRedPoint, RefreshRedPoint);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (singleChapterGuideBookData.processAccomplishStatus[index]==(int)AccomplishStatus.GetAward)
            obj_gotAward.SetActive(true);
        else
            obj_gotAward.SetActive(false);
        txt.SetText((index + 1) * 10 + "%");

        RefreshRedPoint();
    }

    void RefreshRedPoint()
    {
        RedPointManager.Instance.SetRedPointUI(obj_redPoint, RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleChapterProcessTask, 100 * singleChapterGuideBookData.chapter + index);
    }



    void OnNevigated(object[] args)
    {
        GameObject obj = args[0] as GameObject;
        if (obj == gameObject)
            btn.onClick.Invoke();
    }
}
