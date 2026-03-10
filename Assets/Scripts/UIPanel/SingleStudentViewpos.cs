using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleStudentViewpos : SingleStudentView
{
    public override void OnOpenIng() {
        base.OnOpenIng();
        if (peopleData.isPlayer)
        {
            int rarity = peopleData.studentRarity;
            if (rarity == 0)
                rarity = 1;
            if (peopleData.talent == (int)StudentTalent.LianGong)
                img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_diziknew_" + rarity);
            else
                img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_diziknew_" + rarity);
        }
    }

    public override void ShowStudentKuang() {
        if (img_bg != null)
        {
            img_bg.sprite = CommonUtil.StudentKuangNew(peopleData);
        }
    }
}
