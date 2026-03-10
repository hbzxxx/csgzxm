using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailManager : CommonInstance<MailManager>
{

 
    public override void Init()
    {
        base.Init();
        EventCenter.Remove(TheEventType.OnSearchedMyAllMail, OnSearchedMyAllMail);
        EventCenter.Register(TheEventType.OnSearchedMyAllMail, OnSearchedMyAllMail);

    }

    void OnSearchedMyAllMail(object[] args)
    {
 
    }
}
