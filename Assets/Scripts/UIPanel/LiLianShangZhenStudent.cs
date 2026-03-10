using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiLianShangZhenStudent : SingleStudentView
{
    // Start is called before the first frame update
    public LiLianPanel parentPanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        peopleData = args[0] as PeopleData;
        //parentPanel = args[1] as LiLianPanel;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

}
