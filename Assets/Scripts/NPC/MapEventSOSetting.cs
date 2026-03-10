using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEventSOSetting : SerializedScriptableObject
{
    [HorizontalGroup("Split", 55, LabelWidth = 70)]
    [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
    public Texture Icon;

    [VerticalGroup("Split/Meta")]
    public int id;
    [VerticalGroup("Split/Meta")]
    public string Name;
    [VerticalGroup("Split/Meta")]
    [HideLabel, TextArea(1, 1)]
    [EnumAttirbute("面板描述")]
    public string des;

    [VerticalGroup("Split/Meta")]
    [EnumAttirbute("事件类型")]
    public MapEventType mapEventType;

    [VerticalGroup("Split/Meta")]
    [EnumAttirbute("事件场景")]
    public SceneType sceneType;

    [VerticalGroup("Split/Meta")]
    [EnumAttirbute("敌人id"),ShowIf("mapEventType",MapEventType.Enemy)]
    public EnemyIdType EnemyIdType;
}
