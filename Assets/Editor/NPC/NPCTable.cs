#if UNITY_EDITOR

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class NPCTalbe
{
    [FormerlySerializedAs("allNPCs")]
    [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
    private readonly List<NPCWrapper> allNPCs;

    public NPC this[int index]
    {
        get { return this.allNPCs[index].NPC; }
    }

    public NPCTalbe(IEnumerable<NPC> npcs)
    {
        this.allNPCs = npcs.Select(x => new NPCWrapper(x)).ToList();
    }

    private class NPCWrapper
    {
        private NPC npc; // npc is a ScriptableObject and would render a unity object
                                     // field if drawn in the inspector, which is not what we want.

        public NPC NPC
        {
            get { return this.npc; }
        }

        public NPCWrapper(NPC npc)
        {
            this.npc = npc;
        }
        [TableColumnWidth(120)]
        [ShowInInspector]
        public int id { get { return this.npc.id; } set { this.npc.id = value; EditorUtility.SetDirty(this.npc); } }
         
        [TableColumnWidth(50, false)]
        [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
        public Texture Icon { get { return this.npc.Icon; } set { this.npc.Icon = value; EditorUtility.SetDirty(this.npc); } }

        [TableColumnWidth(120)]
        [ShowInInspector]
        public string Name { get { return this.npc.Name; } set { this.npc.Name = value; EditorUtility.SetDirty(this.npc); } }

        [ShowInInspector, ProgressBar(0, 100)]
        public Gender gender { get { return this.npc.gender; } set { this.npc.gender = value; EditorUtility.SetDirty(this.npc); } }

        [TableColumnWidth(120)]
        [ShowInInspector]
        public string des { get { return this.npc.des; } set { this.npc.des = value; EditorUtility.SetDirty(this.npc); } }

        [TableColumnWidth(120)]
        [ShowInInspector]
        public List<SingleTask> tasks { get { return this.npc.tasks; } set { this.npc.tasks = value; EditorUtility.SetDirty(this.npc); } }
    }
}
#endif