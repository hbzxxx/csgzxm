using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleDetailPropertyView : SinglePropertyView
{
    public Text txt_des;
    public Image tubiao;

    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_des.SetText(propertySetting.Des);

        if (tubiao != null)
        {
            //Debug.Log(propertySetting.Id+" : "+propertySetting.Name);
            if (propertySetting != null)
            {
                Debug.Log(propertySetting.Name+":"+propertySetting.Id);
                tubiao.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.propertyIconFolderPath + propertySetting.Id);
            }
        }
    }
}
