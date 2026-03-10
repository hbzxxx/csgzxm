
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleMatchPeopleView : SingleViewBase
{
    public Portrait portrait;
    public Text txt_name;
    public PeopleData peopleData;
    public SingleOtherZongMenData data;
    public override void Init(params object[] args)
    {
        base.Init(args);

        data = args[0] as SingleOtherZongMenData;
        //PeopleData = args[0] as PeopleData;
        peopleData = data.pList[0];
        txt_name.SetText(data.zongMenName);
        portrait.Refresh(peopleData);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

}
