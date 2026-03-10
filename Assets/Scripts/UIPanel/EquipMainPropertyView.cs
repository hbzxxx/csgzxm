using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;
/// <summary>
/// 装备主属性面板
/// </summary>
public class EquipMainPropertyView : SingleViewBase
{
    SinglePropertyData pro = new SinglePropertyData();

    public Text txt_proName;
    public Text txt_proNum;
    public override void Init(params object[] args)
    {
        base.Init(args);
        pro = (SinglePropertyData)args[0];

        PropertySetting propertySetting = DataTable.FindPropertySetting(pro.id);

        txt_proName.SetText(propertySetting.Name);
        txt_proNum.SetText(RoleManager.Instance.GetPropertyShow(pro.id, pro.num));
    }

}
