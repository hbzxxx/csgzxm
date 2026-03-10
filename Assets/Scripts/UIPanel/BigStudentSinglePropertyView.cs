using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigStudentSinglePropertyView : SinglePropertyView
{
    public Image img_quality;
    public Image tubiao;

    public override void Show()
    {
        base.Show();
        if (img_quality is not null)
        {
            if (quality != Quality.None)
            {
                img_quality.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.artResPath + ConstantVal.QualityUIName((int)quality));
                img_quality.gameObject.SetActive(true);
            }
            else
            {
                img_quality.gameObject.SetActive(false);
            }
        }
        if (tubiao != null) {
            //Debug.Log(propertySetting.Id+" : "+propertySetting.Name);
            if (propertySetting != null) {
                tubiao.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.propertyIconFolderPath + propertySetting.Id);
            }
        }
     
    }
}
