using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 编辑器工具：查找预制体中字体丢失的文本组件，并提供修复功能
/// 使用方式：菜单 Tools -> Missing Font Finder
/// </summary>
public class MissingFontFinder : EditorWindow
{
    private Vector2 scrollPosition;
    private Vector2 fontListScrollPosition;
    private List<MissingFontInfo> missingFontResults = new List<MissingFontInfo>();
    private List<Font> unityFonts = new List<Font>();
    private List<TMP_FontAsset> tmpFonts = new List<TMP_FontAsset>();
    private int selectedUnityFontIndex = -1;
    private int selectedTmpFontIndex = -1;
    private bool hasScanned = false;
    private bool showUnityText = true;
    private bool showTmpText = true;

    [MenuItem("Tools/字体丢失检测工具")]
    public static void ShowWindow()
    {
        var window = GetWindow<MissingFontFinder>("Missing Font Finder");
        window.minSize = new Vector2(600, 400);
    }

    private void OnEnable() { LoadAllFonts(); }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("预制体字体丢失检测工具", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        DrawScanSection();
        EditorGUILayout.Space(10);
        DrawFontSelectionSection();
        EditorGUILayout.Space(10);
        DrawResultsSection();
    }

    private void DrawScanSection()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("扫描所有预制体", GUILayout.Height(30))) ScanAllPrefabs();
        if (GUILayout.Button("刷新字体列表", GUILayout.Height(30))) LoadAllFonts();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        showUnityText = EditorGUILayout.ToggleLeft("显示 Unity Text", showUnityText, GUILayout.Width(150));
        showTmpText = EditorGUILayout.ToggleLeft("显示 TextMeshPro", showTmpText, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFontSelectionSection()
    {
        EditorGUILayout.LabelField("项目中的字体资源", EditorStyles.boldLabel);
        fontListScrollPosition = EditorGUILayout.BeginScrollView(fontListScrollPosition, GUILayout.Height(150));
        if (unityFonts.Count > 0)
        {
            EditorGUILayout.LabelField("Unity Font (" + unityFonts.Count + ")", EditorStyles.miniBoldLabel);
            for (int i = 0; i < unityFonts.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                bool isSelected = selectedUnityFontIndex == i;
                bool newSelected = EditorGUILayout.ToggleLeft(unityFonts[i].name, isSelected, GUILayout.Width(200));
                if (newSelected && !isSelected) { selectedUnityFontIndex = i; selectedTmpFontIndex = -1; }
                else if (!newSelected && isSelected) { selectedUnityFontIndex = -1; }
                EditorGUILayout.ObjectField(unityFonts[i], typeof(Font), false);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.Space(5);
        if (tmpFonts.Count > 0)
        {
            EditorGUILayout.LabelField("TextMeshPro Font (" + tmpFonts.Count + ")", EditorStyles.miniBoldLabel);
            for (int i = 0; i < tmpFonts.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                bool isSelected = selectedTmpFontIndex == i;
                bool newSelected = EditorGUILayout.ToggleLeft(tmpFonts[i].name, isSelected, GUILayout.Width(200));
                if (newSelected && !isSelected) { selectedTmpFontIndex = i; selectedUnityFontIndex = -1; }
                else if (!newSelected && isSelected) { selectedTmpFontIndex = -1; }
                EditorGUILayout.ObjectField(tmpFonts[i], typeof(TMP_FontAsset), false);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }


    private void DrawResultsSection()
    {
        var filteredResults = GetFilteredResults();
        EditorGUILayout.LabelField("扫描结果 (" + filteredResults.Count + " 个字体丢失)", EditorStyles.boldLabel);
        if (!hasScanned) { EditorGUILayout.HelpBox("点击「扫描所有预制体」开始检测", MessageType.Info); return; }
        if (filteredResults.Count == 0) { EditorGUILayout.HelpBox("未发现字体丢失的文本组件", MessageType.Info); return; }
        
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = selectedUnityFontIndex >= 0 || selectedTmpFontIndex >= 0;
        if (GUILayout.Button("修复所有选中项", GUILayout.Height(25))) FixAllMissingFonts();
        GUI.enabled = true;
        if (GUILayout.Button("全选/取消全选", GUILayout.Height(25), GUILayout.Width(120)))
        {
            bool allSelected = filteredResults.All(r => r.isSelected);
            foreach (var result in filteredResults) result.isSelected = !allSelected;
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach (var result in filteredResults) DrawResultItem(result);
        EditorGUILayout.EndScrollView();
    }

    private void DrawResultItem(MissingFontInfo info)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        info.isSelected = EditorGUILayout.Toggle(info.isSelected, GUILayout.Width(20));
        if (GUILayout.Button(info.prefabPath, EditorStyles.linkLabel))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(info.prefabPath);
            if (prefab != null) { Selection.activeObject = prefab; EditorGUIUtility.PingObject(prefab); }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("  组件路径: " + info.componentPath);
        EditorGUILayout.LabelField("  类型: " + info.textType);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        bool canFix = (info.textType == TextType.UnityText && selectedUnityFontIndex >= 0) ||
                      (info.textType == TextType.TextMeshPro && selectedTmpFontIndex >= 0);
        GUI.enabled = canFix;
        if (GUILayout.Button("修复此项", GUILayout.Width(80))) { FixSingleMissingFont(info); ScanAllPrefabs(); }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void LoadAllFonts()
    {
        unityFonts.Clear();
        tmpFonts.Clear();
        string[] fontGuids = AssetDatabase.FindAssets("t:Font");
        foreach (string guid in fontGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Font font = AssetDatabase.LoadAssetAtPath<Font>(path);
            if (font != null) unityFonts.Add(font);
        }
        string[] tmpFontGuids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        foreach (string guid in tmpFontGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TMP_FontAsset tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (tmpFont != null) tmpFonts.Add(tmpFont);
        }
        Debug.Log("[MissingFontFinder] 已加载 " + unityFonts.Count + " 个 Unity Font，" + tmpFonts.Count + " 个 TMP Font");
    }


    private void ScanAllPrefabs()
    {
        missingFontResults.Clear();
        hasScanned = true;
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int totalCount = prefabGuids.Length;
        int currentIndex = 0;
        try
        {
            foreach (string guid in prefabGuids)
            {
                currentIndex++;
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EditorUtility.DisplayProgressBar("扫描预制体", "正在扫描: " + path, (float)currentIndex / totalCount);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;
                Text[] texts = prefab.GetComponentsInChildren<Text>(true);
                foreach (var text in texts)
                {
                    //if (text.font == null)
                    //{
                        missingFontResults.Add(new MissingFontInfo
                        {
                            prefabPath = path,
                            componentPath = GetGameObjectPath(text.gameObject, prefab.transform),
                            textType = TextType.UnityText,
                            gameObjectName = text.gameObject.name
                        });
                    //}
                }
                TMP_Text[] tmpTexts = prefab.GetComponentsInChildren<TMP_Text>(true);
                foreach (var tmpText in tmpTexts)
                {
                    if (tmpText.font == null)
                    {
                        missingFontResults.Add(new MissingFontInfo
                        {
                            prefabPath = path,
                            componentPath = GetGameObjectPath(tmpText.gameObject, prefab.transform),
                            textType = TextType.TextMeshPro,
                            gameObjectName = tmpText.gameObject.name
                        });
                    }
                }
            }
        }
        finally { EditorUtility.ClearProgressBar(); }
        Debug.Log("[MissingFontFinder] 扫描完成，发现 " + missingFontResults.Count + " 个字体丢失的文本组件");
    }

    private string GetGameObjectPath(GameObject obj, Transform root)
    {
        string path = obj.name;
        Transform current = obj.transform.parent;
        while (current != null && current != root) { path = current.name + "/" + path; current = current.parent; }
        return path;
    }

    private List<MissingFontInfo> GetFilteredResults()
    {
        return missingFontResults.Where(r =>
            (showUnityText && r.textType == TextType.UnityText) ||
            (showTmpText && r.textType == TextType.TextMeshPro)).ToList();
    }

    private void FixAllMissingFonts()
    {
        var selectedResults = GetFilteredResults().Where(r => r.isSelected).ToList();
        if (selectedResults.Count == 0) { EditorUtility.DisplayDialog("提示", "请先选择要修复的项目", "确定"); return; }
        int fixedCount = 0;
        try
        {
            for (int i = 0; i < selectedResults.Count; i++)
            {
                var info = selectedResults[i];
                EditorUtility.DisplayProgressBar("修复字体", "正在修复: " + info.prefabPath, (float)i / selectedResults.Count);
                if (FixSingleMissingFont(info)) fixedCount++;
            }
        }
        finally { EditorUtility.ClearProgressBar(); }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ScanAllPrefabs();
        EditorUtility.DisplayDialog("修复完成", "成功修复 " + fixedCount + " 个字体丢失的文本组件", "确定");
    }


    private bool FixSingleMissingFont(MissingFontInfo info)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(info.prefabPath);
        if (prefab == null) { Debug.LogError("[MissingFontFinder] 无法加载预制体: " + info.prefabPath); return false; }
        Transform target = FindTransformByPath(prefab.transform, info.componentPath);
        if (target == null) { Debug.LogError("[MissingFontFinder] 无法找到组件路径: " + info.componentPath); return false; }
        bool success = false;
        if (info.textType == TextType.UnityText && selectedUnityFontIndex >= 0)
        {
            Text text = target.GetComponent<Text>();
            if (text != null) { text.font = unityFonts[selectedUnityFontIndex]; EditorUtility.SetDirty(prefab); success = true; }
        }
        else if (info.textType == TextType.TextMeshPro && selectedTmpFontIndex >= 0)
        {
            TMP_Text tmpText = target.GetComponent<TMP_Text>();
            if (tmpText != null) { tmpText.font = tmpFonts[selectedTmpFontIndex]; EditorUtility.SetDirty(prefab); success = true; }
        }
        if (success) Debug.Log("[MissingFontFinder] 已修复: " + info.prefabPath + " -> " + info.componentPath);
        return success;
    }

    private Transform FindTransformByPath(Transform root, string path)
    {
        if (string.IsNullOrEmpty(path)) return root;
        string[] parts = path.Split('/');
        Transform current = root;
        int startIndex = (parts[0] == root.name) ? 1 : 0;
        for (int i = startIndex; i < parts.Length; i++)
        {
            Transform child = current.Find(parts[i]);
            if (child == null) child = FindChildRecursive(current, parts[i]);
            if (child == null) return null;
            current = child;
        }
        return current;
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}

public enum TextType { UnityText, TextMeshPro }

public class MissingFontInfo
{
    public string prefabPath;
    public string componentPath;
    public TextType textType;
    public string gameObjectName;
    public bool isSelected = true;
}
