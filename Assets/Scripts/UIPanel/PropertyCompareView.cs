using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class PropertyCompareView : SingleViewBase
{
    public int id;
    public string num1;
    public string num2;

    public Text txt_name;
    public Text txt_num1;
    public Text txt_num2;

    public override void Init(params object[] args)
    {
        base.Init(args);
        id = (int)args[0];
        num1= (string)args[1];
        num2= (string)args[2];
        PropertySetting setting = DataTable.FindPropertySetting(id);
        txt_name.SetText(setting.Name);
        txt_num1.SetText(num1.ToString());
        txt_num2.SetText(num2.ToString());
    }
}
