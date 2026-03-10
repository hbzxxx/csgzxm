using Framework.Data;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

public class SendManager : CommonInstance<SendManager>
{ 

    public void OnSend(PeopleData p,ItemData itemData,ItemSetting setting)
    {
        if (!ItemManager.Instance.CheckIfHaveItemBySettingId(itemData.settingId))
        {
            return;
        }
       
    

    }

}
