using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SingleMatchZongMenView : SingleViewBase
{
    public Portrait portrait;
    public Text zongMenName;
    public Text txt_rank;
    public Image img_gou;

    public Vector2Int singleTimeRange = new Vector2Int(0, 400);
    public float prepareTimer = 0;
    public float prepareTime = 0;
    public bool startPrepare;

    public bool prepared = false;
    public SingleOtherZongMenData data;
    MatchPanel parentPanel;

    public override void Init(params object[] args)
    {
        base.Init(args);
        data = args[0] as SingleOtherZongMenData;
        parentPanel = args[1] as MatchPanel;

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        PeopleData p = null;
      
            p = data.pList[0];
        portrait.Refresh(p);
        zongMenName.SetText(data.zongMenName);
        txt_rank.SetText(ConstantVal.MatchRankName(data.curRankLevel) + data.curStar);

        prepareTime = RandomManager.Next(singleTimeRange[0], singleTimeRange[1]) * 0.01f;
        prepareTimer = 0;
        if(!data.isPlayerZongMen)
            startPrepare = true;
        img_gou.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (startPrepare)
        {
            prepareTimer += Time.deltaTime;
            if (prepareTimer >= prepareTime)
            {
                startPrepare = false;
                Prepare(true);
            }
        }
    }

    public void Prepare(bool prepare)
    {
        if (prepare)
        {
            img_gou.gameObject.SetActive(true);
            parentPanel.Prepared(true);

        }
        else
        {
            img_gou.gameObject.SetActive(false);
            parentPanel.Prepared(false);

        }
    }


    public override void Clear()
    {
        base.Clear();
        prepared = false;
        startPrepare = false;
    }
}
