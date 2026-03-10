using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleHaoGanDesView : SingleViewBase
{
    int index;
    public Text txt_name;
    public Text txt_pro;
    public override void Init(params object[] args)
    {
        base.Init(args);
        index = (int)args[0];
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        string nameStr = "";
        string str = "";
        switch (index)
        {
            case 0:
                nameStr = "好感31-60";
                str = "攻击触发连携攻击，造成50%伤害，触发冷却回合数为5";
                break;
            case 1:
                nameStr = "好感61-90";
                str = "攻击触发连携攻击，造成70%伤害，触发冷却回合数为4";
                break;
            case 2:
                nameStr = "好感91-99";
                str = "攻击触发连携攻击，造成90%伤害，触发冷却回合数为3";
                break;
            case 3:
                nameStr = "好感100";
                str = "攻击触发连携攻击，造成100%伤害，触发冷却回合数为3";
                break;
            

        }
        txt_name.SetText(nameStr);
        txt_pro.SetText(str);
    }
}
