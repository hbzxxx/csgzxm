using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;
 
public class PeopleProPanel : PanelBase
{
    public Transform trans_proGrid;
    public Image img_hpBar;//血条
    public Image img_mpBar;//蓝条
    public Text txt_hpNum;//血量

    public override void Init(params object[] args)
    {
        base.Init(args);
    
        RegisterEvent(TheEventType.AddBattleProperty, RefreshPro);
        RegisterEvent(TheEventType.DeBattleProperty, RefreshPro);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowPro();
    }

    /// <summary>
    /// 显示属性
    /// </summary>
    public void ShowPro()
    {
        PanelManager.Instance.CloseAllSingle(trans_proGrid);
        List<SinglePropertyData> proList = RoleManager.Instance.GetTotalBattlePro(RoleManager.Instance._CurGameInfo.playerPeople);
        for(int i=0;i< proList.Count; i++)
        {
            SinglePropertyData thePro = proList[i];
            int theId = thePro.id;
            int theNum = thePro.num;
            if (thePro.id != (int)PropertyIdType.MpNum)
            {
                SinglePropertyView proView = PanelManager.Instance.OpenSingle<SinglePropertyView>(trans_proGrid, theId, theNum, (Quality)(int)thePro.quality);

            }
        }
        RefreshHPShow(false);
        RefreshMPShow(false);
    }

    void RefreshPro()
    {
        RefreshHPShow(true);
        RefreshMPShow(true);
    }

    /// <summary>
    /// 刷新血量
    /// </summary>
    void RefreshHPShow(bool tween)
    {
        SinglePropertyData singlePropertyData = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, RoleManager.Instance._CurGameInfo.playerPeople);
        int targetNum = singlePropertyData.num;
        if (!tween)
        {
            txt_hpNum.SetText(targetNum + "/" + singlePropertyData.limit);
            img_hpBar.fillAmount = targetNum / (float)singlePropertyData.limit;
        }
        else
        {
            string[] arr = txt_hpNum.text.Split('/');
            int beforeNum = arr[0].ToInt32();
            DOTween.To(() => beforeNum, x =>
            {
                beforeNum = x;
                txt_hpNum.SetText(x+"/"+singlePropertyData.limit);
            }, targetNum, 1);
            img_hpBar.DOFillAmount(targetNum / (float)singlePropertyData.limit, 1f);
        }
    }

    /// <summary>
    /// 刷新蓝量
    /// </summary>
    void RefreshMPShow(bool tween)
    {
        SinglePropertyData singlePropertyData = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, RoleManager.Instance._CurGameInfo.playerPeople);
        int targetNum = singlePropertyData.num;
        if (!tween)
        {
            img_mpBar.fillAmount = targetNum / (float)singlePropertyData.limit;
        }
        else
        {

            img_mpBar.DOFillAmount(targetNum / (float)singlePropertyData.limit, 1f);
        }
    }

}
