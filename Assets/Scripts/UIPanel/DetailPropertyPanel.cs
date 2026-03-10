 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailPropertyPanel : PanelBase
{
    public Transform trans_grid;
    PeopleData p;
    public Text txt_shuxin;

    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (p.isPlayer) {
            txt_shuxin.SetText("当前领主属性为：" + ConstantVal.YuanSuName((YuanSuType)p.yuanSu));
        }
        else
        {
            txt_shuxin.SetText("当前属下属性为：" + ConstantVal.YuanSuName((YuanSuType)p.yuanSu));
        }
        List<SinglePropertyData> proList = RoleManager.Instance.GetTotalBattlePro(p);
        for (int i = 0; i < proList.Count; i++)
        {
            SinglePropertyData thePro = proList[i];
            int theId = thePro.id;
            int theNum =(int) thePro.num;
            if (thePro.id != (int)PropertyIdType.MpNum)
            {
                SingleDetailPropertyView proView = AddSingle<SingleDetailPropertyView>(trans_grid, theId, theNum, (Quality)(int)thePro.quality);

            }
        }
        //for (int i=0;i<p.propertyList)
        
    }

    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(trans_grid);
    }


}
