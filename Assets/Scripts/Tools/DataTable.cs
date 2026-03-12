using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using cfg;
using LitJson;

/// <summary>
/// 配置表加载（Luban 架构）
/// </summary>
namespace Framework.Data
{
    public class DataTable
    {
        public static Dictionary<string, string> languageDic = new Dictionary<string, string>();
        public static Tables table;

        #region 加载

        private static JSONNode Loader(string fileName)
        {
            var path = GetDataTablePath(fileName + ".json");
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Debug.LogError($"[DataTable] 配置文件缺失: {fileName}");
                return null;
            }
            string content = File.ReadAllText(path);
            content = CommonUtil.DecryptDES(content, ConstantVal.mm);
            return JSON.Parse(content);
        }

        private static string GetDataTablePath(string relativeFile)
        {
#if UNITY_EDITOR
            var root = Application.streamingAssetsPath;
#else
            var root = Application.persistentDataPath;
#endif
            return Path.Combine(root, "res/DataTable", relativeFile).Replace("\\", "/");
        }

        public static void LoadTableData()
        {
            JsonMapper.RegisterImporter<int, long>((int value) => (long)value);
            table = new Tables(Loader);
            //Debug.Log(table.TbDanFarm);
        }

        /// <summary>
        /// 从配置表加载 NPC 数据（替代原 SO 加载）
        /// </summary>
        public static void LoadSOData()
        {
            LoadNPCDataFromTable();
        }

        #endregion

        #region NPC（从配置表加载）
        
        public static NPC[] NPCArr;
        
        /// <summary>
        /// 根据 NPC ID 查找 NPC 配置
        /// </summary>
        public static NPC FindNPCArrById(int id)
        {
            if (NPCArr == null) return null;
            foreach (var npc in NPCArr) { if (npc.id == id) return npc; }
            return null;
        }

        /// <summary>
        /// 从配置表加载 NPC 数据，构建 NPC 对象数组
        /// </summary>
        private static void LoadNPCDataFromTable()
        {
            var npcList = table.TbNPC.DataList;
            NPCArr = new NPC[npcList.Count];

            for (int i = 0; i < npcList.Count; i++)
            {
                var npcSetting = npcList[i];
                NPC npc = ScriptableObject.CreateInstance<NPC>();
                
                // 基础信息
                npc.id = npcSetting.Id;
                npc.Name = npcSetting.Name;
                npc.initAdd = npcSetting.InitAdd;
                npc.gender = (Gender)npcSetting.Gender;
                npc.porTraitName = npcSetting.PorTraitName;
                npc.smallPeopleTextureName = npcSetting.SmallPeopleTextureName;
                npc.des = npcSetting.Des;
                npc.npcType = (NPCType)npcSetting.NpcType;
                npc.enemyId = (EnemyIdType)npcSetting.EnemyId;

                // 加载该 NPC 的任务列表
                npc.tasks = LoadNPCTasks(npcSetting.Id);

                NPCArr[i] = npc;
            }
        }

        /// <summary>
        /// 加载指定 NPC 的任务列表
        /// </summary>
        private static List<SingleTask> LoadNPCTasks(int npcId)
        {
            var tasks = new List<SingleTask>();
            
            // 筛选该 NPC 的所有任务，按 taskIndex 排序
            var taskSettings = new List<cfg.NPCTaskSetting>();
            foreach (var task in table.TbNPCTask.DataList)
            {
                if (task.NpcId == npcId)
                    taskSettings.Add(task);
            }
            taskSettings.Sort((a, b) => a.TaskIndex.CompareTo(b.TaskIndex));

            foreach (var taskSetting in taskSettings)
            {
                SingleTask task = new SingleTask();
                
                // 基础信息
                task.theId = ulong.Parse(taskSetting.Id);
                task.index = taskSetting.TaskIndex;
                task.block = taskSetting.Block;
                task.tagId = taskSetting.TagId;
                task.trackingName = taskSetting.TrackingName;
                task.NPCPos = (NPCAppearPosType)taskSetting.NpcPos;
                task.TaskGuideId = taskSetting.TaskGuideId;
                task.taskType = (TaskType)taskSetting.TaskType;
                task.taskRepeatType = (TaskRepeatType)taskSetting.TaskRepeatType;
                task.repeatTime = taskSetting.RepeatTime;
                
                // 物品需求
                task.isNeedItem = taskSetting.IsNeedItem;
                task.needReceiveItem = taskSetting.NeedReceiveItem;
                task.needItem = (ItemIdType)taskSetting.NeedItem;
                task.needNum = taskSetting.NeedNum;
                
                // 杀敌需求
                task.isNeedKillEnemy = taskSetting.IsNeedKillEnemy;
                task.needKillEnemy = (EnemyIdType)taskSetting.NeedKillEnemy;
                task.needKillEnemyNum = taskSetting.NeedKillEnemyNum;
                
                // 丹炉需求
                task.isNeedDanFarmNum = taskSetting.IsNeedDanFarmNum;
                task.needDanFarmId = (DanFarmIdType)taskSetting.NeedDanFarmId;
                task.needDanFarmNum = taskSetting.NeedDanFarmNum;
                
                // 其他需求
                task.needPlayerLevel = taskSetting.NeedPlayerLevel;
                task.commonAccomplishCondition = taskSetting.CommonAccomplishCondition;
                task.des = taskSetting.Des;
                
                // 奖励列表（分号分隔）
                task.awardList = ParseStringList(taskSetting.AwardList);
                
                // 提示信息
                task.ifAppearInform = taskSetting.IfAppearInform;
                task.appearTxt = taskSetting.AppearTxt;
                
                // 对话标记
                task.ifDialogBefore = taskSetting.IfDialogBefore;
                task.ifDialogAfter = taskSetting.IfDialogAfter;
                
                // 加载任务条件
                task.condition = LoadTaskConditions(taskSetting.Id);
                
                // 加载对话列表
                task.dialogList = LoadTaskDialogs(taskSetting.Id, "before");
                task.dialogListAfter = LoadTaskDialogs(taskSetting.Id, "after");

                tasks.Add(task);
            }

            return tasks;
        }

        /// <summary>
        /// 加载任务条件列表
        /// </summary>
        private static List<TaskCondition> LoadTaskConditions(string taskId)
        {
            var conditions = new List<TaskCondition>();
            
            // 筛选该任务的所有条件，按 conditionIndex 排序
            var conditionSettings = new List<cfg.NPCTaskConditionSetting>();
            foreach (var cond in table.TbNPCTaskCondition.DataList)
            {
                if (cond.TaskId == taskId)
                    conditionSettings.Add(cond);
            }
            conditionSettings.Sort((a, b) => a.ConditionIndex.CompareTo(b.ConditionIndex));

            foreach (var condSetting in conditionSettings)
            {
                TaskCondition condition = new TaskCondition();
                condition.taskConditionType = (TaskConditionType)condSetting.ConditionType;
                condition.param = condSetting.Param;
                conditions.Add(condition);
            }

            return conditions;
        }

        /// <summary>
        /// 加载任务对话列表
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="phase">对话阶段：before/after</param>
        private static List<DialogEditorData> LoadTaskDialogs(string taskId, string phase)
        {
            var dialogs = new List<DialogEditorData>();
            
            // 筛选该任务指定阶段的对话，按 dialogIndex 排序
            var dialogSettings = new List<cfg.NPCTaskDialogSetting>();
            foreach (var dialog in table.TbNPCTaskDialog.DataList)
            {
                if (dialog.TaskId == taskId && dialog.DialogPhase == phase)
                    dialogSettings.Add(dialog);
            }
            dialogSettings.Sort((a, b) => a.DialogIndex.CompareTo(b.DialogIndex));

            foreach (var dialogSetting in dialogSettings)
            {
                DialogEditorData dialog = new DialogEditorData();
                dialog.type = (TaskDialogBelongType)dialogSetting.BelongType;
                dialog.content = dialogSetting.Content;
                dialog.ifPlayerAnswer = dialogSetting.IfPlayerAnswer;
                dialog.answerList = ParseStringList(dialogSetting.AnswerList);
                dialogs.Add(dialog);
            }

            return dialogs;
        }

        /// <summary>
        /// 解析分号分隔的字符串列表
        /// </summary>
        private static List<string> ParseStringList(string str)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(str)) return list;
            
            var parts = str.Split(';');
            foreach (var part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                    list.Add(part);
            }
            return list;
        }

        #endregion

        #region Item
        public static ItemSetting FindItemSetting(string id)
        {
            var setting = table.TbItem.GetOrDefault(id);
            if (setting == null)
            {
                Debug.LogWarning($"[配置表] 找不到物品配置, id={id}，自动修改成金币！");
            }
            return setting;
        }
        public static ItemSetting FindItemSetting(int id) => FindItemSetting(id.ToString());
        public static List<ItemSetting> FindRarityTypeItemSetting(string itemType, int rarity = 0)
        {
            var res = new List<ItemSetting>();
            foreach (var s in table.TbItem.DataList)
                if (s.ItemType == itemType && (rarity == 0 || s.Rarity.ToInt32() == rarity)) res.Add(s);
            return res;
        }
        public static ItemSetting FindHeroSuiPianSettingByHeroId(int id)
        {
            foreach (var setting in table.TbItem.DataList)
            {
                if (setting.Param.ToInt32() == id)
                    return setting;
            }
            return null;
        }

        public static List<ItemSetting> FindItemSettingListByType(ItemType type)
        {
            List<ItemSetting> res = new List<ItemSetting>();
            foreach (var setting in table.TbItem.DataList)
            {
                if (setting.ItemType.ToInt32() == (int)type)
                    res.Add(setting);
            }
            return res;
        }

        /// <summary>
        /// 根据物品类型和稀有度查找物品配置列表
        /// </summary>
        public static List<ItemSetting> FindItemSettingListByType(ItemType type, int rarity)
        {
            List<ItemSetting> res = new List<ItemSetting>();
            foreach (var setting in table.TbItem.DataList)
            {
                if (setting.ItemType.ToInt32() == (int)type && setting.Rarity.ToInt32() == rarity)
                    res.Add(setting);
            }
            return res;
        }

        public static ItemSetting FindValidXiSuiDanSetting(int rarity)
        {
            foreach (var setting in table.TbItem.DataList)
            {
                // XiSuiDan 类型需要根据实际 ItemType 枚举值调整
                if (setting.Param.ToInt32() == rarity)
                    return setting;
            }
            return null;
        }

        #endregion

        #region Screening 屏蔽词

        public static bool IsScreening(string str)
        {
            foreach (var setting in table.TbScreening.DataList)
            {
                if (str.Contains(setting.ScreeningStr))
                    return true;
            }
            return false;
        }

        #endregion

        #region Property 属性

        public static PropertySetting FindPropertySetting(string id)
        {
            return table.TbProperty.GetOrDefault(id);
        }
        public static PropertySetting FindPropertySetting(int id)
        {
            return FindPropertySetting(id.ToString());
        }

        #endregion

        #region Train 修炼

        public static TrainSetting FindTrainSetting(string id)
        {
            return table.TbTrain.GetOrDefault(id);
        }
        public static TrainSetting FindTrainSetting(int id)
        {
            return FindTrainSetting(id.ToString());
        }

        public static TrainSetting FindTrainSettingByCenterLevel(int centerLevel)
        {
            // 直接通过 Id 查找，centerLevel 实际上是 trainIndex
            return table.TbTrain.GetOrDefault(centerLevel.ToString());
        }

        #endregion

        #region Equipment 装备

        public static EquipmentSetting FindEquipmentSetting(string id)
        {
            return table.TbEquipment.GetOrDefault(id);
        }
        public static EquipmentSetting FindEquipmentSetting(int id)
        {
            return FindEquipmentSetting(id.ToString());
        }

        /// <summary>
        /// 根据稀有度获取装备图纸分组列表（按套装类型分组）
        /// </summary>
        public static Dictionary<int, List<int>> FindEquipPictureGroupList(Rarity rarity)
        {
            Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
            foreach (var setting in table.TbEquipment.DataList)
            {
                if (setting.Rarity.ToInt32() == (int)rarity)
                {
                    int taoZhuangType = setting.TaoZhuang.ToInt32();
                    if (!result.ContainsKey(taoZhuangType))
                    {
                        result[taoZhuangType] = new List<int>();
                    }
                    result[taoZhuangType].Add(setting.Id.ToInt32());
                }
            }
            return result;
        }

        #endregion

        #region Skill 技能

        public static SkillSetting FindSkillSetting(string id)
        {
            return table.TbSkill.GetOrDefault(id);
        }
        public static SkillSetting FindSkillSetting(int id)
        {
            return FindSkillSetting(id.ToString());
        }

        /// <summary>
        /// 根据元素类型获取可学习的技能列表
        /// </summary>
        public static List<SkillSetting> FindCanStudySkillListByYuanSu(YuanSuType yuanSu)
        {
            List<SkillSetting> res = new List<SkillSetting>();
            foreach (var setting in table.TbSkill.DataList)
            {
                // 必须同时满足：元素匹配 且 CanStudy 非空（有对应功法书物品）
                if (setting.YuanSu.ToInt32() == (int)yuanSu && !string.IsNullOrWhiteSpace(setting.CanStudy))
                    res.Add(setting);
            }
            return res;
        }

        /// <summary>
        /// 获取所有可学习的技能列表（不限元素）
        /// </summary>
        public static List<SkillSetting> FindCanStudySkillListByYuanSu()
        {
            List<SkillSetting> res = new List<SkillSetting>();
            foreach (var setting in table.TbSkill.DataList)
            {
                // 只返回 CanStudy 非空的技能（有对应功法书物品）
                if (!string.IsNullOrWhiteSpace(setting.CanStudy))
                    res.Add(setting);
            }
            return res;
        }

        public static SkillUpgradeSetting FindSkillUpgradeSetting(string id)
        {
            return table.TbSkillUpgrade.GetOrDefault(id);
        }
        public static SkillUpgradeSetting FindSkillUpgradeSetting(int id)
        {
            return FindSkillUpgradeSetting(id.ToString());
        }

        #endregion

        #region Level 关卡

        public static LevelSetting FindLevelSetting(string id)
        {
            return table.TbLevel.GetOrDefault(id);
        }
        public static LevelSetting FindLevelSetting(int id)
        {
            return FindLevelSetting(id.ToString());
        }

        #endregion

        #region Map 地图

        public static MapSetting FindMapSetting(string id)
        {
            return table.TbMap.GetOrDefault(id);
        }
        public static MapSetting FindMapSetting(int id)
        {
            return FindMapSetting(id.ToString());
        }

        public static MapEventSetting FindMapEventSetting(string id)
        {
            return table.TbMapEvent.GetOrDefault(id);
        }
        public static MapEventSetting FindMapEventSetting(int id)
        {
            return FindMapEventSetting(id.ToString());
        }

        #endregion

        #region Building 建筑

 

        public static BuildingUpgradeSetting FindBuildingUpgradeSetting(string id)
        {
            return table.TbBuildingUpgrade.GetOrDefault(id);
        }
        public static BuildingUpgradeSetting FindBuildingUpgradeSetting(int id)
        {
            return FindBuildingUpgradeSetting(id.ToString());
        }

        #endregion

        #region NewGuide 新手引导

        public static NewGuideSetting FindNewGuideSetting(string id)
        {
            return table.TbNewGuide.GetOrDefault(id);
        }
        public static NewGuideSetting FindNewGuideSetting(int id)
        {
            return FindNewGuideSetting(id.ToString());
        }

        #endregion

        #region Dialog 对话
 
        #endregion

        #region Enemy 敌人

        public static EnemySetting FindEnemySetting(string id)
        {
            return table.TbEnemy.GetOrDefault(id);
        }
        public static EnemySetting FindEnemySetting(int id)
        {
            return FindEnemySetting(id.ToString());
        }

        #endregion

        #region Gem 宝石

        public static GemSetting FindGemSetting(string id)
        {
            return table.TbGem.GetOrDefault(id);
        }
        public static GemSetting FindGemSetting(int id)
        {
            return FindGemSetting(id.ToString());
        }

        public static List<GemSetting> FindGemSettingList(int rarity, int pos)
        {
            List<GemSetting> res = new List<GemSetting>();
            foreach (var setting in table.TbGem.DataList)
            {
                if (setting.Rarity.ToInt32() == rarity)
                    res.Add(setting);
            }
            return res;
        }

        #endregion

        #region Shop 商店

        public static ShopSetting FindShopSetting(string id)
        {
            return table.TbShop.GetOrDefault(id);
        }
        public static ShopSetting FindShopSetting(int id)
        {
            return FindShopSetting(id.ToString());
        }

        public static List<ShopSetting> FindShopSettingList(ShopType type)
        {
            List<ShopSetting> res = new List<ShopSetting>();
            foreach (var setting in table.TbShop.DataList)
            {
                if (setting.Type.ToInt32() == (int)type)
                    res.Add(setting);
            }
            return res;
        }

        #endregion

        #region Task 任务

        public static TaskSetting FindTaskSetting(string id)
        {
            return table.TbTask.GetOrDefault(id);
        }
        public static TaskSetting FindTaskSetting(int id)
        {
            return FindTaskSetting(id.ToString());
        }

        public static List<TaskSetting> FindTypeTaskSettingList(TaskType type)
        {
            List<TaskSetting> res = new List<TaskSetting>();
            foreach (var setting in table.TbTask.DataList)
            {
                if (setting.Type.ToInt32() == (int)type)
                    res.Add(setting);
            }
            return res;
        }

        #endregion

        #region Buff

 

        public static BattleBuffSetting FindBattleBuffSetting(string id)
        {
            return table.TbBattleBuff.GetOrDefault(id);
        }
        public static BattleBuffSetting FindBattleBuffSetting(int id)
        {
            return FindBattleBuffSetting(id.ToString());
        }

        #endregion

        #region XingGe 性格
 

        #endregion

        #region XueMaiUpgrade 血脉升级

        public static XueMaiUpgradeSetting FindXueMaiUpgradeSetting(string id)
        {
            return table.TbXueMaiUpgrade.GetOrDefault(id);
        }
        public static XueMaiUpgradeSetting FindXueMaiUpgradeSetting(int id)
        {
            return FindXueMaiUpgradeSetting(id.ToString());
        }

        public static XueMaiUpgradeSetting FindXueMaiUpgradeSettingByType(int type)
        {
            foreach (var setting in table.TbXueMaiUpgrade.DataList)
            {
                if (setting.Type.ToInt32() == type)
                    return setting;
            }
            return null;
        }

        #endregion

        #region 兼容旧 API（DataList 别名）

        // Train
        public static List<TrainSetting> _trainList => table.TbTrain.DataList;

        // StudentUpgrade
        public static List<StudentUpgradeSetting> _studentUpgradeList => table.TbStudentUpgrade.DataList;

        // GuideBook
        public static List<GuideBookSetting> _guideBookList => table.TbGuideBook.DataList;
        public static GuideBookSetting FindGuideBookSetting(string id) => table.TbGuideBook.GetOrDefault(id);
        public static GuideBookSetting FindGuideBookSetting(int id) => FindGuideBookSetting(id.ToString());

        // Task
        public static List<TaskSetting> _taskList => table.TbTask.DataList;

        // ZongMenUpgrade
        public static List<ZongMenUpgradeSetting> _zongMenUpgradeList => table.TbZongMenUpgrade.DataList;

        // MapEvent
        public static List<MapEventSetting> _mapEventList => table.TbMapEvent.DataList;

        // SingleResearch
 
        // LiLian
        public static List<LiLianSetting> _liLianList => table.TbLiLian.DataList;
        public static LiLianSetting FindLiLianSetting(string id) => table.TbLiLian.GetOrDefault(id);
        public static LiLianSetting FindLiLianSetting(int id) => FindLiLianSetting(id.ToString()); 
        // ExploreMap
        public static List<ExploreMapSetting> _exploreMapList => table.TbExploreMap.DataList;
        public static ExploreMapSetting FindExploreMapSetting(string id) => table.TbExploreMap.GetOrDefault(id);
        public static ExploreMapSetting FindExploreMapSetting(int id) => FindExploreMapSetting(id.ToString());

        // DanFarm
        public static DanFarmSetting FindDanFarmSetting(string id) => table.TbDanFarm.GetOrDefault(id);
        public static DanFarmSetting FindDanFarmSetting(int id) => FindDanFarmSetting(id.ToString());

        // ZongMenProduce
 
        // MiJing
        public static List<MiJingSetting> _miJingList => table.TbMiJing.DataList;
        public static MiJingSetting FindMiJingSetting(string id) => table.TbMiJing.GetOrDefault(id);
        public static MiJingSetting FindMiJingSetting(int id) => FindMiJingSetting(id.ToString());
        public static MiJingLevelSetting FindMiJingLevelSetting(string id) => table.TbMiJingLevel.GetOrDefault(id);
        public static MiJingLevelSetting FindMiJingLevelSetting(int id) => FindMiJingLevelSetting(id.ToString());

        // Map
        public static List<MapSetting> _mapList => table.TbMap.DataList;
        public static bool CheckIfHaveIdMap(string id) => table.TbMap.GetOrDefault(id) != null;
        public static bool CheckIfHaveIdMap(int id) => CheckIfHaveIdMap(id.ToString());

        // NewGuide
        public static List<NewGuideSetting> _newGuideList => table.TbNewGuide.DataList;
        public static Dictionary<string, NewGuideSetting> newGuideDic
        {
            get
            {
                var dic = new Dictionary<string, NewGuideSetting>();
                foreach (var s in table.TbNewGuide.DataList)
                    dic[s.Id] = s;
                return dic;
            }
        }

        // Building
 
        // Match
        public static List<MatchSetting> _matchList => table.TbMatch.DataList;

        // Shop
        public static List<ShopSetting> _shopList => table.TbShop.DataList;
        public static Dictionary<string, ShopSetting> shopDic
        {
            get
            {
                var dic = new Dictionary<string, ShopSetting>();
                foreach (var s in table.TbShop.DataList)
                    dic[s.Id] = s;
                return dic;
            }
        }
        public static List<ShopSetting> FindShopSettingListByType(int type)
        {
            var res = new List<ShopSetting>();
            foreach (var s in table.TbShop.DataList)
                if (s.Type.ToInt32() == type) res.Add(s);
            return res;
        }

        // EquipMakeTeam
 
        // RdmName
        public static List<RdmNameSetting> _rdmNameList => table.TbRdmName.DataList;

        // Property
        public static List<PropertySetting> _propertyList => table.TbProperty.DataList;
        public static PropertySetting FindPropertySettingByTalent(int talent)
        {
            foreach (var s in table.TbProperty.DataList)
                if (s.TalentType.ToInt32() == talent) return s;
            return null;
        }

        // EquipTaoZhuang
        public static EquipTaoZhuangSetting FindEquipTaoZhuangSetting(string id) => table.TbEquipTaoZhuang.GetOrDefault(id);
        public static EquipTaoZhuangSetting FindEquipTaoZhuangSetting(int id) => FindEquipTaoZhuangSetting(id.ToString());

        // SkillUpgrade
        public static List<SkillUpgradeSetting> FindSkillUpgradeListBySkillId(int skillId)
        {
            var res = new List<SkillUpgradeSetting>();
            foreach (var s in table.TbSkillUpgrade.DataList)
                if (s.SkillId.ToInt32() == skillId) res.Add(s);
            return res;
        }

        // TestStudentSingleProInfluence
        public static List<TestStudentSingleProInfluenceSetting> _testStudentSingleProInfluenceList => table.TbTestStudentSingleProInfluence.DataList;
        public static TestStudentSingleProInfluenceSetting FindTestStudentSingleProInfluenceByPro(int proNum)
        {
            foreach (var s in table.TbTestStudentSingleProInfluence.DataList)
            {
                // ProRange 格式为 "min-max"
                var range = s.ProRange.Split('-');
                if (range.Length == 2)
                {
                    int min = int.Parse(range[0]);
                    int max = int.Parse(range[1]);
                    if (proNum >= min && proNum <= max)
                        return s;
                }
            }
            // 返回最后一个作为默认值
            var list = table.TbTestStudentSingleProInfluence.DataList;
            return list.Count > 0 ? list[list.Count - 1] : null;
        }

        // TestStudentTotalProInfluence
        public static List<TestStudentTotalProInfluenceSetting> _testStudentTotalProInfluenceList => table.TbTestStudentTotalProInfluence.DataList;
        public static TestStudentTotalProInfluenceSetting FindTestStudentTotalProInfluenceByPro(int proNum)
        {
            foreach (var s in table.TbTestStudentTotalProInfluence.DataList)
            {
                // ProRange 格式为 "min-max"
                var range = s.ProRange.Split('-');
                if (range.Length == 2)
                {
                    int min = int.Parse(range[0]);
                    int max = int.Parse(range[1]);
                    if (proNum >= min && proNum <= max)
                        return s;
                }
            }
            // 返回最后一个作为默认值
            var list = table.TbTestStudentTotalProInfluence.DataList;
            return list.Count > 0 ? list[list.Count - 1] : null;
        }

        // LianDanBuildingUpgrade
        public static List<LianDanBuildingUpgradeSetting> _lianDanBuildingUpgradeList => table.TbLianDanBuildingUpgrade.DataList;

        // Level
        public static List<LevelSetting> _levelList => table.TbLevel.DataList;

        // Activity
 
        // RdmZongMenName
        public static List<RdmZongMenNameSetting> _rdmZongMenNameList => table.TbRdmZongMenName.DataList;

        // MapSetting - 根据MapLevel查找
        public static MapSetting FindMapSettingByMapLevel(int mapLevel)
        {
            foreach (var setting in table.TbMap.DataList)
            {
                if (setting.MapLevel.ToInt32() == mapLevel)
                    return setting;
            }
            return null;
        }

        // GuideBook - 根据Chapter查找任意一个
        public static GuideBookSetting FindAnyGuideBookSettingByChapter(int chapter)
        {
            foreach (var setting in table.TbGuideBook.DataList)
            {
                if (setting.Chapter.ToInt32() == chapter)
                    return setting;
            }
            return null;
        }

        // EquipBuildingUpgrade
        public static List<EquipBuildingUpgradeSetting> _equipBuildingUpgradeList => table.TbEquipBuildingUpgrade.DataList;

        // BuildingUpgrade
        public static List<BuildingUpgradeSetting> _buildingUpgradeList => table.TbBuildingUpgrade.DataList;

        // EquipUpgrade
        public static List<EquipUpgradeSetting> _equipUpgradeList => table.TbEquipUpgrade.DataList;

        // XueMaiUpgrade - 重载方法支持 XueMaiType 参数
        public static XueMaiUpgradeSetting FindXueMaiUpgradeSettingByType(XueMaiType type)
        {
            return FindXueMaiUpgradeSettingByType((int)type);
        }

        // 兼容旧 API：FindEquipSetting -> FindEquipmentSetting
        public static EquipmentSetting FindEquipSetting(string id) => FindEquipmentSetting(id);
        public static EquipmentSetting FindEquipSetting(int id) => FindEquipSetting(id.ToString());

        // 兼容旧 API：FindtestStudentTotalProInfluenceByPro（小写t）
        public static TestStudentTotalProInfluenceSetting FindtestStudentTotalProInfluenceByPro(int proNum) => FindTestStudentTotalProInfluenceByPro(proNum);

        // FindItemSettingByType - 单参数版本
        public static List<ItemSetting> FindItemSettingByType(ItemType type)
        {
            return FindItemSettingListByType(type);
        }

        // FindItemSettingByType - 双参数版本（按类型和稀有度查找）
        public static ItemSetting FindItemSettingByType(ItemType type, int rarity)
        {
            foreach (var setting in table.TbItem.DataList)
            {
                if (setting.ItemType.ToInt32() == (int)type && setting.Rarity.ToInt32() == rarity)
                    return setting;
            }
            return null;
        }

        // Skill
        public static List<SkillSetting> _skillList => table.TbSkill.DataList;

        // LeiChong
        public static List<LeiChongSetting> _leiChongList => table.TbLeiChong.DataList;
        public static LeiChongSetting FindLeiChongSetting(string id) => table.TbLeiChong.GetOrDefault(id);
        public static LeiChongSetting FindLeiChongSetting(int id) => FindLeiChongSetting(id.ToString());

        // Gem - 根据稀有度查找破碎宝石的 ItemId 列表
        public static List<int> FindRarityPoSuiGem(Rarity rarity)
        {
            List<int> res = new List<int>();
            foreach (var setting in table.TbGem.DataList)
            {
                if (setting.Rarity.ToInt32() == (int)rarity && setting.Level.ToInt32() == 1)
                {
                    res.Add(setting.ItemId.ToInt32());
                }
            }
            return res;
        }

        // ChouJiang
        public static List<ChouJiangSetting> _chouJiangList => table.TbChouJiang.DataList;
        public static ChouJiangSetting FindChouJiangSetting(string id) => table.TbChouJiang.GetOrDefault(id);
        public static ChouJiangSetting FindChouJiangSetting(int id) => FindChouJiangSetting(id.ToString());

        // UpgradeMode
        public static List<UpgradeModeSetting> _upgradeModeList => table.TbUpgradeMode.DataList;
        public static UpgradeModeSetting FindUpgradeModeSetting(string id) => table.TbUpgradeMode.GetOrDefault(id);
        public static UpgradeModeSetting FindUpgradeModeSetting(int id) => FindUpgradeModeSetting(id.ToString());

        // SkillUpgrade
        public static List<SkillUpgradeSetting> _skillUpgradeList => table.TbSkillUpgrade.DataList;

        // LevelPress
        public static List<LevelPressSetting> _levelPressList => table.TbLevelPress.DataList;
        public static LevelPressSetting FindLevelPressSetting(string id) => table.TbLevelPress.GetOrDefault(id);
        public static LevelPressSetting FindLevelPressSetting(int id) => FindLevelPressSetting(id.ToString());

        // TotalADAward
        public static List<TotalADAwardSetting> _totalADAwardList => table.TbTotalADAward.DataList;

 
        // QianDao - 签到
        public static List<QianDaoSetting> _qianDaoList => table.TbQianDao.DataList;
        public static QianDaoSetting FindQianDaoSetting(string id) => table.TbQianDao.GetOrDefault(id);
        public static QianDaoSetting FindQianDaoSetting(int id) => FindQianDaoSetting(id.ToString());
        
        /// <summary>
        /// 根据签到类型和天数查找签到配置
        /// </summary>
        /// <param name="type">签到类型：1=7日签到，2=30日签到</param>
        /// <param name="day">第几天</param>
        public static QianDaoSetting FindQianDaoSettingByTypeAndDay(int type, int day)
        {
            foreach (var setting in table.TbQianDao.DataList)
            {
                if (setting.Type.ToInt32() == type && setting.Day.ToInt32() == day)
                    return setting;
            }
            return null;
        }

        // EquipTaoZhuang - 根据套装类型和稀有度查找原配装备ID列表
        // 注意：EquipTaoZhuangSetting 只有 Id, Name, Des, Param, Param2 属性
        // 如需按套装类型和稀有度查找，需要根据实际配置表结构调整
        public static List<int> FindTaoZhuangYuanPeiIdListByTaoZhuangType(int taoZhuangType, Rarity rarity)
        {
            List<int> res = new List<int>();
            // 当前 EquipTaoZhuangSetting 没有 TaoZhuangType/Rarity/YuanPeiId 字段
            // 如果需要此功能，请根据实际配置表结构重新实现
            return res;
        }

        #endregion
    }
}