using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleProductProView : SingleViewBase
{
    StudentTalent talent;
    public Text txt_name;
    public Text txt_pro;
    public override void Init(params object[] args)
    {
        base.Init(args);
        talent = (StudentTalent)args[0];
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        string nameStr = "";
        string str = "";
        switch (talent)
        {
            case StudentTalent.LianJing:
                nameStr = LanguageUtil.GetLanguageText((int)LanguageIdType.炼丹);
                str = LanguageUtil.GetLanguageText((int)LanguageIdType.炼丹天赋可让弟子驻守炼丹房时提高炼制高品质丹药的概率);
                break;
            case StudentTalent.DuanZhao:
                nameStr = LanguageUtil.GetLanguageText((int)LanguageIdType.炼器);

                str = LanguageUtil.GetLanguageText((int)LanguageIdType.炼器天赋可让弟子驻守炼器房时提高炼制法器的属性强化法器的属性以及分解法器返还材料的数量);
                break;
            case StudentTalent.CaiKuang:
                nameStr = LanguageUtil.GetLanguageText((int)LanguageIdType.采矿);
                str = LanguageUtil.GetLanguageText((int)LanguageIdType.采矿天赋可让随从驻守矿场时提高产量);
                break;
            case StudentTalent.ChaoYao:
                nameStr =  LanguageUtil.GetLanguageText((int)LanguageIdType.灵树);
                str = LanguageUtil.GetLanguageText((int)LanguageIdType.灵树天赋可让弟子驻守灵树时提高产量);
                break;
            case StudentTalent.JingWen:
                nameStr =  LanguageUtil.GetLanguageText((int)LanguageIdType.经文);
                str = LanguageUtil.GetLanguageText((int)LanguageIdType.经文天赋可让弟子驻守藏经阁时提高产量);
                break;
            case StudentTalent.JingShang:
                nameStr =  LanguageUtil.GetLanguageText((int)LanguageIdType.种田);
                str =  LanguageUtil.GetLanguageText((int)LanguageIdType.种田天赋可让弟子驻守灵田时提高产量);
                break;
            case StudentTalent.BaoShi:
                nameStr = LanguageUtil.GetLanguageText((int)LanguageIdType.宝石);
                str = LanguageUtil.GetLanguageText((int)LanguageIdType.宝石天赋可让弟子驻守八卦炉时提高炼制合成宝石的属性);
                break;

        }
        txt_name.SetText(nameStr);
        txt_pro.SetText(str);
    }
}
