using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SingleHaoGanDuStudentView : SingleViewBase
{
    public PeopleData me;
    public PeopleData other;

    public Transform trans_peopleGrid;
    public Text txt_label;
    public Image img_haoGanDuBar;

    public override void Init(params object[] args)
    {
        base.Init(args);
        me = args[0] as PeopleData;
        other = args[1] as PeopleData;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();


        PanelManager.Instance.OpenSingle<SingleStudentHaoganduView>(trans_peopleGrid, other);

        int index = me.socializationData.knowPeopleList.IndexOf(other.onlyId);
        int haoGanDu = me.socializationData.haoGanDu[index];
        if (haoGanDu >= 0)
        {
            txt_label.SetText("好感度");
            img_haoGanDuBar.color = Color.green;
        }
        else
        {
            txt_label.SetText("仇恨值");
            img_haoGanDuBar.color = Color.red;

        }
        int fillVal = Mathf.Abs(haoGanDu);
        img_haoGanDuBar.fillAmount = fillVal / 100f;
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_peopleGrid);
    }
}
