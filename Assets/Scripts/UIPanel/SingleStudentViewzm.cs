using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleStudentViewzm : SingleStudentView
{
    public override void OnOpenIng() {
        ShowPortrait();
        if (img_bg != null)
        {
            int rarity = peopleData.studentRarity;
            if (rarity == 0)
                rarity = 1;
            //if (peopleData.talent == (int)StudentTalent.LianGong)
                //img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_dizik_" + rarity);
            //else
                img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_dizikn_pz" + rarity);
        }
    }
}
