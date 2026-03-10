using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDesPanel : PanelBase
{
    public Transform trans_jingJieDes;
    public Transform grid_jingJieDes;//境界描述格子

    public Transform trans_rankDes;
    public Transform grid_rankDes;//排位

    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    /// <summary>
    /// 显示境界
    /// </summary>
    public void ShowJingJie()
    {
        trans_rankDes.gameObject.SetActive(false);
        trans_jingJieDes.gameObject.SetActive(true);
        ClearCertainParentAllSingle<SingleTopDesView>(grid_jingJieDes);
        
    }

    /// <summary>
    /// 显示排位
    /// </summary>
    public void ShowRank()
    {
        trans_rankDes.gameObject.SetActive(true);
        trans_jingJieDes.gameObject.SetActive(false);
        ClearCertainParentAllSingle<SingleTopDesView>(grid_rankDes);
        List<string> phaseNameList = new List<string>();
        for (int i = 1; i < 16; i++)
        {
            string centerName = ConstantVal.MatchRankName(i);
            if (!phaseNameList.Contains(centerName))
            {
                phaseNameList.Add(centerName);
            }
        }
        int curRank = (RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel);
        for (int i = 0; i < phaseNameList.Count; i++)
        {
            bool highLight = false;
            if (i == curRank-1)
            {
                highLight = true;
            }
            AddSingle<SingleTopDesView>(grid_rankDes, phaseNameList[i], highLight);
        }
    }

    public override void Clear()
    {
        base.Clear();
        trans_rankDes.gameObject.SetActive(false);
        trans_jingJieDes.gameObject.SetActive(false);
    }
}
