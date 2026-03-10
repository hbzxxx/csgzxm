using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemProTxtView : SingleViewBase
{
    public int proNum;
    public int compareProNum;//与之相比

    public Text txt_proNum;
    public Text txt_compareProNum;
    public override void Init(params object[] args)
    {
        base.Init(args);
        int proId = (int)args[0];
        proNum = (int)args[1];
        PropertySetting setting = DataTable.FindPropertySetting(proId);
        string baiFen = "%";
        if (proId == (int)PropertyIdType.MPSpeed
            || proId == (int)PropertyIdType.JingTong)
        {
            baiFen = "";
        }
        txt_proNum.SetText(setting.Name+"+"+ proNum + baiFen);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }
}
