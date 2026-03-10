using Framework.Data;
using Google.Protobuf.Collections;
using System;
 using System.Reflection;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// 语言管理类
/// </summary>
public static class LanguageUtil
{
    private static string curLanguage = "Chinese";

    public static string CurLanguage
    {
        get { return curLanguage; }
        set
        {
            curLanguage = value;
            if (OnLanguageChange != null)
                OnLanguageChange();
        }
    }
    public static event Action OnLanguageChange;


    /// <summary>
    /// 文本值完全是表格里的文本时使用这个
    /// </summary>
    /// <param name="targetText"></param>
    /// <param name="textId">表格id</param>
    public static void SetText(this Text targetText, string textId, bool withFitter = false)
    {

        string res = textId;
        res = res.Replace("\\n", "\n")
        .Replace("Lv", "等级")
        .Replace("lv", "等级")
.Replace("洗髓", "升华")
.Replace("教化","调谐")
.Replace("整容","捏脸")
.Replace("转魂","天赋")
.Replace("开光", "洞幽")
.Replace("击杀", "击败")
.Replace("讨伐","清理")
.Replace("酒","甘")
.Replace("墓地","遗迹")
.Replace("邪恶","宵人")
.Replace("仇恨","坏感")
.Replace("元婴","凝形")
.Replace("合体","归真")
.Replace("渡劫","冲霄")
.Replace("牛逼","达人")
        ;
        targetText.text = res;
 

    }
    public static void SetPos(this RepeatedField<float> savePos, Vector2 pos)
    {
        if (savePos.Count == 2)
        {
            savePos[0] = pos.x;
            savePos[1] = pos.y;
        }
        else
        {
            savePos.Clear();
            savePos.Add(pos[0]);
            savePos.Add(pos[1]);
        }

    }

    public static string GetLanguageText(int textId)
    {
        string res = DataTable.table.TbLanguage.Get(textId).Des;
        return res;
    }
}

public enum LanguageIdType
{
    None=0,
    当前武备等级已达上限请先升级人物等级=1,
您还没有炼器房请先建造一个=2,
您还没有八卦炉请先建造一个=4,
您还没有炼丹炉请先建造一个=5,
炼丹=6,
炼丹天赋可让弟子驻守炼丹房时提高炼制高品质丹药的概率=7,
炼器=8,
炼器天赋可让弟子驻守炼器房时提高炼制法器的属性强化法器的属性以及分解法器返还材料的数量=9,
采矿=10,
采矿天赋可让随从驻守矿场时提高产量=11,
灵树=12,
灵树天赋可让弟子驻守灵树时提高产量=13,
经文=14,
经文天赋可让弟子驻守藏经阁时提高产量=15,
种田=16,
种田天赋可让弟子驻守灵田时提高产量=17,

宝石=18,
宝石天赋可让弟子驻守八卦炉时提高炼制合成宝石的属性=19,

不符合境界需求=20,

不能再携带更多功法了=21,
抽后必得地级弟子接引令=22,
抽后必得天级弟子接引令=23,

当前弟子人数已达上限请提升宗门等级=24,
该建筑数量已达上限请提升宗门等级=25,
当前无任何修为丹=26,
普通法器=27,
界域动荡一年一度的界域裂隙已经打开=28,

修武=29,

正在秘境探险请先等待回来=30,
弟子=31,

秘境=32,
已被封脉无法使用功法=33,

功法=34,

当前正在界域裂隙请掌门回到宗门后再强化吧=35,

界域裂隙=36,

灵石=37,
宗门=38,

宗门大比=39,
历练=40,

法器=41,
锦衣=42,
鞋子=43,
璎珞=44,

不能将自己逐出宗门=45,

逐出宗门=46,    
}

