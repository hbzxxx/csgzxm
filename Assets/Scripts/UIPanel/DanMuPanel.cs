using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanMuPanel : PanelBase
{
    public Transform trans_up;
    public Transform trans_down;
    public Transform trans_end;

    public Transform trans_danMuParent;
    public List<DanMuTxtView> curShowDanMuTxtViewList = new List<DanMuTxtView>();
    int yOffset = 100;
    public List<int> candidateYPos = new List<int>();

    public List<string> waitDanMuContentQueue = new List<string>();
    public List<bool> waitDanMuImportantQueue = new List<bool>();

    public int maxSpeed = 1000;
    public int minSpeed = 400;
    public override void Init(params object[] args)
    {
        base.Init(args);
        string content = (string)args[0];
        bool important = (bool)args[1];
        int maxCount = (int)(trans_up.localPosition.y - trans_down.localPosition.y) / 100;
        for(int i = 0; i < maxCount; i++)
        {
            candidateYPos.Add((int)trans_up.localPosition.y - yOffset * i);
        }
        AddDanMu(content, important);
    }

    public void AddDanMu(string content,bool important)
    {
        //进入等待区
        if (candidateYPos.Count == 0)
        {
            waitDanMuContentQueue.Add(content);
            waitDanMuImportantQueue.Add(important);
            return;
        }

        int yIndex = RandomManager.Next(0, candidateYPos.Count);
        int yPos = candidateYPos[yIndex];

        DanMuTxtView view = AddSingle<DanMuTxtView>(trans_danMuParent, content, important, yPos);
        view.transform.DOKill();
        view.transform.localPosition = new Vector2(trans_up.localPosition.x, yPos);
        float speed = 0.01f* RandomManager.Next(minSpeed, maxSpeed);
        curShowDanMuTxtViewList.Add(view);
        candidateYPos.Remove(yPos);

        view.transform.DOLocalMoveX(trans_end.localPosition.x, speed).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (!candidateYPos.Contains(view.logicPosY))
            {
                candidateYPos.Add(view.logicPosY);
             
            }
            PanelManager.Instance.CloseSingle(view);

            if (waitDanMuContentQueue.Count > 0)
            {
                string newContent = waitDanMuContentQueue[0];
                bool newImportant = waitDanMuImportantQueue[0];
                waitDanMuContentQueue.RemoveAt(0);
                waitDanMuImportantQueue.RemoveAt(0);
                AddDanMu(newContent, newImportant);
            }
        });

    }


    public override void Clear()
    {
        base.Clear();
        for(int i = 0; i < curShowDanMuTxtViewList.Count; i++)
        {
            curShowDanMuTxtViewList[i].DOKill();
        }
        curShowDanMuTxtViewList.Clear();
        candidateYPos.Clear();
        waitDanMuContentQueue.Clear();
        waitDanMuImportantQueue.Clear();
        ClearCertainParentAllSingle<DanMuTxtView>(trans_danMuParent);
    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.danmuPanel = null;

    }
}
