
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : SerializedScriptableObject
{
    [HorizontalGroup("Split", 55, LabelWidth = 70)]
    [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
    public Texture Icon;

    [VerticalGroup("Split/Meta")]
    public int id;

    [VerticalGroup("Split/Meta")]
    public string Name;
    [VerticalGroup("Split/Meta")]
    public bool initAdd;
    [VerticalGroup("Split/Meta")]
    public Gender gender;

    [VerticalGroup("Split/Meta")]
    [EnumAttirbute("立绘图片名")]
    public string porTraitName;

    [VerticalGroup("Split/Meta")]
    [EnumAttirbute("小人图片名")]
    public string smallPeopleTextureName;

    [VerticalGroup("Split/Meta")]
    [HideLabel, TextArea(1, 1)]
    public string des;

    [VerticalGroup("Split/Meta")]
    [EnumAttirbute("npc类型")]
    public NPCType npcType;


    [VerticalGroup("Split/Meta")]
    [EnumAttirbute("敌人id")]
    public EnemyIdType enemyId ;


    [VerticalGroup("Split/Meta"),HideIf("npcType", NPCType.Enemy)]
    [EnumAttirbute("npc任务")]
    public List<SingleTask> tasks;

}

//[EnumAttirbute("npc类型")]
public enum NPCType
{
    [EnumAttirbute("主线")]
    None = 0,
    [EnumAttirbute("支线")]
    BranchLine=1,//支线 不影响主线
    [EnumAttirbute("敌人")]
    Enemy,//敌人
    [EnumAttirbute("神算子")]
    ShenSuanZi,//神算子
}
/// <summary>
/// npcid类型
/// </summary>
public enum NPCIDType
{
    None=0,
    PangBai=10000,//旁白
    HunHun=10001,//开局混混 打完以后 这血脉的力量仿佛
    ShenMiShaoNv=10002,//神秘少女
    HunHunTouZi=10003,//开局混混头子
    ShanHaiZongMenRen = 10004,//山海宗门人
    ShanHaiZongZhangLao=10005,//山海宗长老
    DiShu=10006,//帝姝
    ShenSuanZi=10007,//神算子
    SongJiNengShanZei=10008,//送技能的山贼
    SuMengLan=10009,//苏梦岚（
}