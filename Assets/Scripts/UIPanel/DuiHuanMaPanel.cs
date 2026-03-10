 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuiHuanMaPanel : PanelBase
{
    public Button btn_sendMa;
    public InputField input;
    public override void Init(params object[] args)
    {
        base.Init(args);
         input.text = "";
        //频繁判断
        addBtnListener(btn_sendMa, () =>
        {
    
            if (Game.Instance.isRightEdition
            &&GameTimeManager.Instance.connectedToFuWuQiTime
            &&Game.Instance.isLogin)
            {
                AllDuiHuanMaData allData = RoleManager.Instance._CurGameInfo.AllDuiHuanMaData;
                if (allData.getDuiHuanMaList.Contains(input.text))
                {
                    PanelManager.Instance.OpenFloatWindow("无效兑换码");
                    return;
                }
          
 
                ////永久激活码
                //string str = input.text;
                //if (ConstantVal.jiHuoMaList.Contains(str)
                //&& !RoleManager.Instance._CurGameInfo.onceJiHuoedList.Contains(str))
                //{
                //    ItemData item = new ItemData();
                //    item.settingId = (int)ItemIdType.ZuanShi;
                //    item.num = 80;
                //    ItemManager.Instance.GetItemWithAwardPanel(new List<ItemData> { item });
                //    RoleManager.Instance._CurGameInfo.onceJiHuoedList.Add(str);
                //}
                //else if (str.Length == 17
                // && !RoleManager.Instance._CurGameInfo.onceJiHuoedList.Contains(str))
                //{
                //    if (Game.Instance.isRightEdition)
                //        Game.Instance.clientManager.SendRT(NetCmd.EntityRpc, "GetFixedLiBao", str);
                //}
                //else
                //{
                //}
            }
            else
            {
                PanelManager.Instance.OpenFloatWindow("请检查网络状况");
            }
    

        });
    }
  
}
