using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Framework.Data;
using cfg;

public class AddExpShowStudentView : SingleStudentView
{
    public int beforeExp;

    public Image img_exp;
    public Text txt_exp;
 
    public override void Init(params object[] args)
    {
        base.Init(args);

        beforeExp = (int)args[1];
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        img_exp.DOKill();
        txt_exp.DOKill();
        txt_lv.SetText("Lv." + peopleData.studentLevel);

        if (peopleData.studentLevel == DataTable._studentUpgradeList.Count)
        {
            img_exp.fillAmount = 1;
            txt_exp.SetText("Max");
        }
        else
        {
            StudentUpgradeSetting setting = DataTable._studentUpgradeList[peopleData.studentLevel];
            int expLimit = setting.NeedExp.ToInt32();

            DOTween.To(() => beforeExp, (x) =>
            {
                beforeExp = x;
                txt_exp.text = beforeExp + "/" + expLimit;
            }, peopleData.studentCurExp, 1);
            img_exp.DOFillAmount(peopleData.studentCurExp / (float)expLimit,1);
        }

  
    }
}
