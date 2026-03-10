using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class OdinUtil
{
    private static ItemIdType TestDrawEnumByChinese(ItemIdType value, GUIContent label)
    {
        Type targetEnumType = typeof(ItemIdType);
        LabelTextAttribute targetEnumLabelTextAttribute = targetEnumType.GetAttribute<LabelTextAttribute>();

        FieldInfo[] fieldInfos = targetEnumType.GetFields();

        string[] targetItemNames = Enum.GetNames(targetEnumType);

        string finalShowName = "";
        for (int i = 0; i < targetItemNames.Length; i++)
        {
            if (targetItemNames[i] == value.ToString())
            {
                finalShowName = value.ToString();
                break;
            }
        }

        for (int i = 0; i < fieldInfos.Length; i++)
        {
            if (fieldInfos[i].Name == finalShowName)
            {
                finalShowName = fieldInfos[i].GetAttribute<LabelTextAttribute>().Text;
                break;
            }
        }

        return EnumSelector<ItemIdType>.DrawEnumField(new GUIContent(targetEnumLabelTextAttribute.Text), new GUIContent(finalShowName), value);
    }

    [CustomValueDrawer("TestDrawEnumByChinese")]
    [LabelText("测试中文枚举")]
    public enum TestEnum
    {
        [LabelText("测试1")]
        Test1,

        [LabelText("测试2")]
        Test2
    }
    private static TestEnum TestDrawEnumByChinese(TestEnum value, GUIContent label)
    {
        Type targetEnumType = typeof(TestEnum);
        LabelTextAttribute targetEnumLabelTextAttribute = targetEnumType.GetAttribute<LabelTextAttribute>();

        FieldInfo[] fieldInfos = targetEnumType.GetFields();

        string[] targetItemNames = Enum.GetNames(targetEnumType);

        string finalShowName = "";
        for (int i = 0; i < targetItemNames.Length; i++)
        {
            if (targetItemNames[i] == value.ToString())
            {
                finalShowName = value.ToString();
                break;
            }
        }

        for (int i = 0; i < fieldInfos.Length; i++)
        {
            if (fieldInfos[i].Name == finalShowName)
            {
                finalShowName = fieldInfos[i].GetAttribute<LabelTextAttribute>().Text;
                break;
            }
        }

        return EnumSelector<TestEnum>.DrawEnumField(new GUIContent(targetEnumLabelTextAttribute.Text), new GUIContent(finalShowName), value);
    }
}
