#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector.Demos.RPGEditor;

public class StoryEditor : OdinMenuEditorWindow
{
    [MenuItem("剧情编辑器/新剧情")]
    private static void Open()
    {
        StoryEditor test = GetWindow<StoryEditor>();
        test.Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;

        // Adds the character overview table.
        //CharacterOverview.Instance.UpdateCharacterOverview();
        //tree.Add("Characters", new CharacterTable(CharacterOverview.Instance.AllCharacters));

        // Adds all characters.
        tree.AddAllAssetsAtPath("NPCs", "Assets/Resources/NPCs", typeof(NPC), true, true);



        // Add icons to characters and items.
        tree.EnumerateTree().AddIcons<NPC>(x => x.Icon);
        tree.SortMenuItemsByName();
        return tree;
    }

    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }

    protected override void OnBeginDrawEditors()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create NPC")))
            {
                 
                ScriptableObjectCreator.ShowDialog<NPC>("Assets/Resources/NPCs", obj =>
                {
                    obj.Name = obj.name;
                    base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                });
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}
#endif