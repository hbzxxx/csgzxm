using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;

/// <summary>
/// 装备槽位属性显示视图（简化版，用于显示爆伤等属性）
/// </summary>
public class EquipPosPropertyView : SingleViewBase
{
    public Text txt_name;
    public Text txt_num;
    
    private int id;
    private int num;
    private Quality quality;
    private PropertySetting propertySetting;

    public override void Init(params object[] args)
    {
        base.Init(args);
        id = (int)args[0];
        num = (int)args[1];
        quality = (Quality)args[2];
        
        propertySetting = DataTable.FindPropertySetting(id);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        Show();
    }

    void Show()
    {
        if (propertySetting == null)
            return;
            
        if (txt_name != null)
            txt_name.SetText(propertySetting.Name);

        if (txt_num != null)
        {
            switch ((PropertyIdType)propertySetting.Id.ToInt32())
            {
                case PropertyIdType.CritNum:
                case PropertyIdType.CritRate:
                    txt_num.SetText(num + "%");
                    break;
                case PropertyIdType.RdmProDamageAdd:
                case PropertyIdType.WaterDamageAdd:
                case PropertyIdType.FireDamageAdd:
                case PropertyIdType.StormDamageAdd:
                case PropertyIdType.IceDamageAdd:
                case PropertyIdType.YangProDamageAdd:
                case PropertyIdType.YinProDamageAdd:
                case PropertyIdType.TotalProDamageAdd:
                    txt_num.SetText(num * 0.01 + "%");
                    break;
                default:
                    txt_num.SetText(num.ToString());
                    break;
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        propertySetting = null;
    }
}
