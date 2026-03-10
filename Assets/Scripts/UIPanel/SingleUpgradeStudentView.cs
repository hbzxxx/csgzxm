using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;
using DG.Tweening;

public class SingleUpgradeStudentView : SingleViewBase
{

    public Transform trans_studentGrid;

    public Image imgBar;

    public Text txt_exp;

    public Transform trans_max;

    public int beforeExp;

    public int expLimit;

    PeopleData p;

    StudentUpgradeSetting setting;
    TrainSetting trainSetting;
    bool canAdd = false;

    int curVal = 0;
    float barBeforeVal = 0;
    float barFinishVal = 0;
    public Text txt_studentName;
    public Text txt_lv;
    public GameObject obj_canTuPo;//可以突破了
    public override void Init(params object[] args)
    {
        base.Init(args);
        beforeExp = (int)args[0];
        p = args[1] as PeopleData;

        if (p.talent == (int)StudentTalent.LianGong)
        {  
            //忽隐忽现
            int lastXiuWeiNeed = 0;

            if (p.trainIndex >= 1)
            {
                TrainSetting lastTrainSetting = DataTable._trainList[p.trainIndex - 1];
                lastXiuWeiNeed = lastTrainSetting.XiuWeiNeed.ToInt32();
            }

            if (p.trainIndex < DataTable._trainList.Count-1)
            {
                trainSetting = DataTable._trainList[p.trainIndex];
                expLimit = trainSetting.XiuWeiNeed.ToInt32();
                canAdd = true;
                curVal =(int)(ulong)p.curXiuwei;
                barBeforeVal = (beforeExp - lastXiuWeiNeed)/ (float)(expLimit - lastXiuWeiNeed);


                barFinishVal =(curVal-lastXiuWeiNeed)/ (float)(expLimit - lastXiuWeiNeed);
            }
            else
            {
                canAdd = false;
            }

        }
        else
        {
            if (p.studentLevel < DataTable._studentUpgradeList.Count)
            {
                setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                expLimit = setting.NeedExp.ToInt32();
                canAdd = true;
                curVal = p.studentCurExp;
                barBeforeVal = beforeExp / (float)expLimit;
                barFinishVal = p.studentCurExp / (float)expLimit;
            }
            else
            {
                canAdd = false;
            }
        }
  
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        SingleStudentView view= PanelManager.Instance.OpenSingle<SingleStudentView>(trans_studentGrid, p);
        view.obj_nameBg.gameObject.SetActive(false);
        txt_studentName.SetText(view.txt_name.text);
        txt_lv.SetText(view.txt_lv.text);

        trans_max.gameObject.SetActive(false);
        
        imgBar.DOKill();
        txt_exp.DOKill();
        imgBar.fillAmount = barBeforeVal;

        if (canAdd)
        {
            txt_exp.gameObject.SetActive(true);
            txt_exp.SetText(beforeExp + "/" + expLimit);
            imgBar.DOFillAmount(barFinishVal, 1f);
            DOTween.To(() => beforeExp, x =>
            {
                beforeExp = x;
                txt_exp.SetText(x + "/" + expLimit);
            
            }
            , curVal, 1f);

            if (beforeExp >= expLimit)
            {
                trans_max.gameObject.SetActive(true);
            }
            else
            {
                trans_max.gameObject.SetActive(false);
            }
        }
        else
        {
            txt_exp.gameObject.SetActive(false);
        }

        if (StudentManager.Instance.CheckIfCanTuPo(p))
        {
            obj_canTuPo.SetActive(true);
        }
        else
        {
            obj_canTuPo.SetActive(false);
        }
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_studentGrid);
    }
}
