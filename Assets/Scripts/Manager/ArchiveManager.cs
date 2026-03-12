using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using cfg;
using Framework.Data;

/// <summary>
/// 存档管理器 - 使用 Easy Save 3 进行序列化
/// </summary>
public class ArchiveManager : CommonInstance<ArchiveManager>
{
    /// <summary>最大存档数量</summary>
    public int maxArchiveNum = 5;
    
    /// <summary>当前选择的存档索引</summary>
    public int curArchiveIndex = -1;
    
    /// <summary>所有存档列表</summary>
    public List<GameInfo> allArchiveList = new List<GameInfo>();
    
    /// <summary>最近的存档</summary>
    public GameInfo recentArchive = null;
    
    /// <summary>最近存档的索引</summary>
    public int recentArchiveIndex = -1;

    /// <summary>
    /// ES3 加密设置
    /// </summary>
    private ES3Settings GetES3Settings()
    {
        return new ES3Settings(ES3.EncryptionType.AES, ConstantVal.mm);
    }

    /// <summary>
    /// 保存存档
    /// </summary>
    /// <param name="archiveIndex">存档索引，-1 表示使用当前索引</param>
    public void SaveArchive(int archiveIndex = -1)
    {
        if (MatchManager.Instance.generateAllZongMenThread != null 
            && MatchManager.Instance.generateAllZongMenThread.ThreadState != System.Threading.ThreadState.Stopped)
        {
            return;
        }
        
        if (archiveIndex == -1)
            archiveIndex = curArchiveIndex;

        if (RoleManager.Instance._CurGameInfo == null)
        {
            Debug.LogError("_CurGameInfo 为空，跳过保存");
            return;
        }

        GameInfo gameInfo = RoleManager.Instance._CurGameInfo;
        gameInfo.SaveTime = CGameTime.Instance.GetTimeStamp();

        if (gameInfo.AllBuildingData == null)
        {
            Debug.Log("测试模式：自动修改存档数据");
            ApplyTestModifications(gameInfo);
        }

        // 确保目录存在
        DirectoryInfo destination = new DirectoryInfo(ConstantVal.GetArchiveSaveFolder(archiveIndex));
        if (!destination.Exists)
        {
            destination.Create();
        }

        var settings = GetES3Settings();
        string savePath = GetES3SavePath(archiveIndex);
        
        // 存档前处理：将 gemList 转换为 gemSaveList（避免循环引用）
        PrepareEquipDataForSave(gameInfo);
        
        ES3.Save<GameInfo>(ConstantVal.mm, gameInfo, savePath, settings);
        
        Debug.Log("保存gameInfo成功: " + savePath);
        
        // 验证保存的数据
        var verifyGameInfo = ES3.Load<GameInfo>(ConstantVal.mm, savePath, settings);
        if (verifyGameInfo != null)
        {
            if (verifyGameInfo.AllMapData != null && verifyGameInfo.AllMapData.MapList != null)
            {
                int unlockedMapCount = 0;
                int totalLevelCount = 0;
                int unlockedLevelCount = 0;
                foreach (var map in verifyGameInfo.AllMapData.MapList)
                {
                    if (map.MapStatus == 2) unlockedMapCount++;
                    if (map.LevelList != null)
                    {
                        totalLevelCount += map.LevelList.Count;
                        foreach (var level in map.LevelList)
                        {
                            if (level.LevelStatus == 2) unlockedLevelCount++;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("[ArchiveManager] 验证存档 - 地图数据为空");
            }
        }
        
        // 如果被封了就弹窗
        if (gameInfo.IsFeng)
            PanelManager.Instance.OpenOnlyOkHint("检测到账号异常。", null, true);
    }

    /// <summary>
    /// 载入存档
    /// </summary>
    /// <param name="archiveIndex">存档索引</param>
    /// <returns>GameInfo 对象，失败返回 null</returns>
    public GameInfo LoadArchive(int archiveIndex)
    {
        GameInfo gameInfo = null;
        var settings = GetES3Settings();
        string savePath = GetES3SavePath(archiveIndex);
        
        try
        {
            gameInfo = ES3.Load<GameInfo>(ConstantVal.mm, savePath, settings);
            
            Debug.Log($"[LoadArchive] 加载存档 - 玩家等级: {gameInfo.playerPeople?.studentLevel}, 主城等级: {gameInfo.AllBuildingData?.MountainLevel}");
            if (gameInfo.AllMapData != null && gameInfo.AllMapData.MapList != null)
            {
                int unlockedMapCount = 0;
                int totalLevelCount = 0;
                int unlockedLevelCount = 0;
                foreach (var map in gameInfo.AllMapData.MapList)
                {
                    if (map.MapStatus == 2) unlockedMapCount++;
                    if (map.LevelList != null)
                    {
                        totalLevelCount += map.LevelList.Count;
                        foreach (var level in map.LevelList)
                        {
                            if (level.LevelStatus == 2) unlockedLevelCount++;
                        }
                    }
                }
                Debug.Log($"[LoadArchive] 地图解锁: {unlockedMapCount}/{gameInfo.AllMapData.MapList.Count}, 关卡解锁: {unlockedLevelCount}/{totalLevelCount}");
            }
            else
            {
                Debug.Log("[LoadArchive] 地图数据为空");
            }
            
            // 删除 SettingId=70001 的丹田数据
            if (gameInfo.allDanFarmData != null && gameInfo.allDanFarmData.DanFarmList != null)
            {
                gameInfo.allDanFarmData.DanFarmList.RemoveAll(data => data.SettingId == 70001);
            }
            // 加载后处理：将 gemSaveList 还原为 gemList
            RestoreEquipDataAfterLoad(gameInfo);
            // 把存档存到备份位置
            DirectoryInfo destination = new DirectoryInfo(ConstantVal.GetArchiveBeiFenSaveFolder(archiveIndex));
            if (!destination.Exists)
            {
                destination.Create();
            }
            
            string backupPath = GetES3BackupPath(archiveIndex);
            ES3.Save<GameInfo>(ConstantVal.mm, gameInfo, backupPath, settings);
        }
        catch (Exception e)
        {
            Debug.LogError("加载存档失败: " + e.Message);
            
            // 尝试恢复备份存档
            PanelManager.Instance.OpenCommonHint("出于未知的原因，坏档了，是否恢复备份存档？", () =>
            {
                try
                {
                    string backupPath = GetES3BackupPath(archiveIndex);
                    gameInfo = ES3.Load<GameInfo>(ConstantVal.mm, backupPath, settings);
                    
                    // 把备份存档存到对应位置
                    DirectoryInfo destination = new DirectoryInfo(ConstantVal.GetArchiveSaveFolder(archiveIndex));
                    if (!destination.Exists)
                    {
                        destination.Create();
                    }
                    
                    ES3.Save<GameInfo>(ConstantVal.mm, gameInfo, savePath, settings);
                    LoadAllArchive();
                }
                catch (Exception backupEx)
                {
                    Debug.LogError("恢复备份存档失败: " + backupEx.Message);
                }
            }, null);
        }
        
        // 检查并自动装备（如果还没有装备）
        CheckAndAutoEquip(gameInfo);
        
        return gameInfo;
    }
    
    private void CheckAndAutoEquip(GameInfo gameInfo)
    {
        if (gameInfo == null) return;
        
        var allEquipSettings = DataTable.table?.TbEquipment?.DataList;
        if (allEquipSettings == null || allEquipSettings.Count == 0)
        {
            Debug.LogWarning("[TestMod] 装备配置表为空，无法自动装备");
            return;
        }

        // 确保玩家有装备槽位
        if (gameInfo.playerPeople == null)
        {
            Debug.LogWarning("[TestMod] 玩家数据为空，无法自动装备");
            return;
        }

        // 初始化玩家的装备槽位
        if (gameInfo.playerPeople.curEquipItemList == null)
        {
            gameInfo.playerPeople.curEquipItemList = new List<ItemData> { null, null, null, null };
        }
        else if (gameInfo.playerPeople.curEquipItemList.Count < 4)
        {
            while (gameInfo.playerPeople.curEquipItemList.Count < 4)
            {
                gameInfo.playerPeople.curEquipItemList.Add(null);
            }
        }
        
        // 检查玩家是否有装备
        bool playerHasEquip = false;
        for (int i = 0; i < gameInfo.playerPeople.curEquipItemList.Count; i++)
        {
            if (gameInfo.playerPeople.curEquipItemList[i] != null && gameInfo.playerPeople.curEquipItemList[i].settingId > 0)
            {
                playerHasEquip = true;
                break;
            }
        }
        
        // 如果没有装备，自动添加 - 使用正常装备方式
        if (!playerHasEquip)
        {
            // 确保技能数据已初始化 - 使用正常流程
            InitPlayerSkillsFullLevel(gameInfo.playerPeople);

            Debug.Log("[TestMod] 玩家没有装备，正在自动装备...");
            for (int i = 0; i < 4; i++)
            {
                var bestEquip = FindBestEquipmentForSlot(i, allEquipSettings);
                Debug.Log($"[TestMod] 槽位 {i} 找到最佳装备: {(bestEquip != null ? bestEquip.Name : "null")}");
                if (bestEquip != null)
                {
                    // 创建装备
                    ItemData item = CreateBestEquipItem(bestEquip, gameInfo);
                    
                    // 添加到背包
                    if (gameInfo.ItemModel == null)
                    {
                        gameInfo.ItemModel = new ItemModel();
                    }
                    gameInfo.ItemModel.itemIdList.Add(item.settingId);
                    gameInfo.ItemModel.itemDataList.Add(item);
                    gameInfo.ItemModel.onlyIdList.Add(item.onlyId);

                    // 使用正常装备方式装备
                    EquipmentManager.Instance.OnEquip(gameInfo.playerPeople, item, i);
                }
            }
            Debug.Log("[TestMod] 玩家自动装备完成");
        }
        
        // 检查弟子是否有装备
        if (gameInfo.studentData?.allStudentList != null)
        {
            foreach (var student in gameInfo.studentData.allStudentList)
            {
                if (student == null) continue;

                // 初始化弟子的装备槽位
                if (student.curEquipItemList == null)
                {
                    student.curEquipItemList = new List<ItemData> { null, null, null, null };
                }
                else if (student.curEquipItemList.Count < 4)
                {
                    while (student.curEquipItemList.Count < 4)
                    {
                        student.curEquipItemList.Add(null);
                    }
                }

                // 确保技能数据已初始化
                if (student.allSkillData == null)
                {
                    student.allSkillData = new AllSkillData();
                }
                
                // 使用正常流程初始化弟子技能
                InitPlayerSkillsFullLevel(student);
                
                bool studentHasEquip = false;
                for (int i = 0; i < student.curEquipItemList.Count; i++)
                {
                    if (student.curEquipItemList[i] != null && student.curEquipItemList[i].settingId > 0)
                    {
                        studentHasEquip = true;
                        break;
                    }
                }
                
                if (!studentHasEquip)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var bestEquip = FindBestEquipmentForSlot(i, allEquipSettings);
                        if (bestEquip != null)
                        {
                            // 创建装备
                            ItemData item = CreateBestEquipItem(bestEquip, gameInfo);
                            
                            // 添加到背包
                            if (gameInfo.ItemModel == null)
                            {
                                gameInfo.ItemModel = new ItemModel();
                            }
                            gameInfo.ItemModel.itemIdList.Add(item.settingId);
                            gameInfo.ItemModel.itemDataList.Add(item);
                            gameInfo.ItemModel.onlyIdList.Add(item.onlyId);

                            // 使用正常装备方式装备
                            EquipmentManager.Instance.OnEquip(student, item, i);
                        }
                    }
                    Debug.Log($"[TestMod] 弟子 {student.name} 自动装备完成");
                }
            }
        }
    }

    /// <summary>
    /// 载入所有存档
    /// </summary>
    /// <returns>所有存档列表</returns>
    public List<GameInfo> LoadAllArchive()
    {
        Debug.Log("加载所有存档");
        allArchiveList = new List<GameInfo>();
        
        for (int i = 0; i < maxArchiveNum; i++)
        {
            // 检查StreamingAssets中是否有存档文件，如果有就复制到持久化目录
            Debug.Log("检查并复制存档" + i);
            CheckAndCopyStreamingArchive(i);

            string savePath = GetES3SavePath(i);
            if (!ES3.FileExists(savePath))
            {
                allArchiveList.Add(null);
            }
            else
            {
                GameInfo gameInfo = LoadArchive(i);

#if !UNITY_EDITOR
                if (gameInfo != null
                    && !string.IsNullOrWhiteSpace(gameInfo.versionName)
                    && ConstantVal.VersionNum(gameInfo.versionName) < 3000)
                {
                    // 删档测试
                    Debug.Log("删除存档" + i);
                    DeleteArchive(i);
                    allArchiveList.Add(null);
                }
                else
                {
                    allArchiveList.Add(gameInfo);
                }
#else
                allArchiveList.Add(gameInfo);
#endif
            }
        }
        
        // 找到最近的存档
        long recentSaveTime = long.MinValue;
        recentArchive = null;
        recentArchiveIndex = -1;
        
        for (int i = 0; i < allArchiveList.Count; i++)
        {
            GameInfo gameInfo = allArchiveList[i];
            if (gameInfo == null)
                continue;
            if (gameInfo.SaveTime > recentSaveTime)
            {
                recentArchive = gameInfo;
                recentSaveTime = gameInfo.SaveTime;
                recentArchiveIndex = i;
            }
        }
        
        return allArchiveList;
    }

    /// <summary>
    /// 获取存档上传名称（带包名前缀）
    /// </summary>
    public string ArchiveUploadName()
    {
        string packageName = Application.identifier;
        string archiveName = "";
        if (!string.IsNullOrEmpty(MyHttpServer.Instance.curAccount))
        {
            archiveName = $"{packageName}_{MyHttpServer.Instance.curAccount}_{curArchiveIndex}";
            Debug.Log($"使用banhaomode存档格式: {archiveName}");
        }
        else
        {
            archiveName = $"{packageName}_{RoleManager.Instance._CurGameInfo.TheGuid}";
            Debug.Log($"使用普通存档格式: {archiveName}");
        }
        return archiveName;
    }

    /// <summary>
    /// 获取存档下载名称（带包名前缀）
    /// </summary>
    public string ArchiveDownloadName(string originalName)
    {
        string packageName = Application.identifier;
        return $"{packageName}_{originalName}";
    }

    /// <summary>
    /// 创建上传快照
    /// </summary>
    public string CreateUploadSnapshot()
    {
        string src = GetES3SavePath(curArchiveIndex);
        string temp = Path.Combine(Application.temporaryCachePath,
                                  $"upload_{DateTime.Now:yyyyMMddHHmmssfff}.tmp");

        File.Copy(src, temp, overwrite: true);
        File.SetAttributes(temp, FileAttributes.Temporary | FileAttributes.NotContentIndexed);

        return temp;
    }

    /// <summary>
    /// 删除存档
    /// </summary>
    /// <param name="index">存档索引</param>
    public void DeleteArchive(int index)
    {
        string savePath = GetES3SavePath(index);
        if (ES3.FileExists(savePath))
        {
            ES3.DeleteFile(savePath);
            LoadAllArchive();
        }

        EventCenter.Broadcast(TheEventType.OnDeleteArchive);
    }

    /// <summary>
    /// 查找最近的存档
    /// </summary>
    public GameInfo FindRecentGameInfo()
    {
        List<GameInfo> archiveList = LoadAllArchive();

        long recentSaveTime = long.MinValue;
        GameInfo recent = null;
        
        for (int i = 0; i < archiveList.Count; i++)
        {
            GameInfo gameInfo = archiveList[i];
            if (gameInfo == null)
                continue;
            if (gameInfo.SaveTime > recentSaveTime)
            {
                recent = gameInfo;
                recentSaveTime = gameInfo.SaveTime;
            }
        }
        return recent;
    }

    /// <summary>
    /// 检查并复制StreamingAssets中的存档到持久化目录
    /// </summary>
    private void CheckAndCopyStreamingArchive(int archiveIndex)
    {
        string streamingArchivePath = ConstantVal.GetSpecialArchiveStreamPath(archiveIndex);
        string persistentArchivePath = GetES3SavePath(archiveIndex);

        // 如果持久化目录中已经有存档文件，则跳过
        if (ES3.FileExists(persistentArchivePath))
        {
            return;
        }

        CheckAndCopyStreamingArchiveSync(streamingArchivePath, persistentArchivePath, archiveIndex);
    }

    /// <summary>
    /// 同步检查并复制StreamingAssets中的存档
    /// </summary>
    private void CheckAndCopyStreamingArchiveSync(string streamingArchivePath, string persistentArchivePath, int archiveIndex)
    {
        try
        {
            using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(streamingArchivePath))
            {
                var asyncOp = www.SendWebRequest();
                while (!asyncOp.isDone)
                {
                    System.Threading.Thread.Sleep(10);
                }

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    DirectoryInfo destination = new DirectoryInfo(ConstantVal.GetArchiveSaveFolder(archiveIndex));
                    if (!destination.Exists)
                    {
                        destination.Create();
                    }

                    File.WriteAllBytes(persistentArchivePath, www.downloadHandler.data);
                    Debug.Log($"已从StreamingAssets复制存档到持久化目录: {persistentArchivePath}");
                }
                else
                {
                    Debug.Log($"StreamingAssets中没有找到存档文件: {streamingArchivePath}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"复制StreamingAssets存档失败: {e.Message}");
        }
    }

    /// <summary>
    ///     /// 获取 ES3 存档路径
    /// </summary>
    private string GetES3SavePath(int archiveIndex)
    {
        return $"New2Archives/archive_{archiveIndex}/GameInfo.es3";
    }

    /// <summary>
    ///    ES3 备份存档路径
    /// </summary>
    private string GetES3BackupPath(int archiveIndex)
    {
        return $"New2BeiFenArchives/archive_{archiveIndex}/GameInfo.es3";
    }

    #region
    /// <summary>
    /// 存档前处理：遍历所有装备，将 gemList 转换为 gemSaveList
    /// </summary>
    private void PrepareEquipDataForSave(GameInfo gameInfo)
    {
        // 处理背包中的装备
        if (gameInfo.ItemModel?.itemDataList != null)
        {
            foreach (var item in gameInfo.ItemModel.itemDataList)
            {
                item?.equipProtoData?.PrepareForSave();
            }
        }
        
        // 处理仓库中的装备
        if (gameInfo.ItemModel?.cangKuItemDataList != null)
        {
            foreach (var item in gameInfo.ItemModel.cangKuItemDataList)
            {
                item?.equipProtoData?.PrepareForSave();
            }
        }
        
        // 处理已装备的装备
        if (gameInfo.AllEquipmentData?.curEquippedEquipList != null)
        {
            foreach (var equip in gameInfo.AllEquipmentData.curEquippedEquipList)
            {
                equip?.PrepareForSave();
            }
        }
        
        // 处理玩家身上的装备
        if (gameInfo.playerPeople?.curEquipItemList != null)
        {
            foreach (var item in gameInfo.playerPeople.curEquipItemList)
            {
                item?.equipProtoData?.PrepareForSave();
            }
        }
        
        // 处理弟子身上的装备
        if (gameInfo.studentData?.allStudentList != null)
        {
            foreach (var student in gameInfo.studentData.allStudentList)
            {
                if (student?.curEquipItemList != null)
                {
                    foreach (var item in student.curEquipItemList)
                    {
                        item?.equipProtoData?.PrepareForSave();
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 加载后处理：遍历所有装备，将 gemSaveList 还原为 gemList
    /// 注意：setting 赋值需要在配置表加载完成后调用 RestoreAllSettings
    /// </summary>
    private void RestoreEquipDataAfterLoad(GameInfo gameInfo)
    {
        // 处理背包中的装备
        if (gameInfo.ItemModel?.itemDataList != null)
        {
            foreach (var item in gameInfo.ItemModel.itemDataList)
            {
                item?.equipProtoData?.RestoreAfterLoad();
            }
        }
        
        // 处理仓库中的装备
        if (gameInfo.ItemModel?.cangKuItemDataList != null)
        {
            foreach (var item in gameInfo.ItemModel.cangKuItemDataList)
            {
                item?.equipProtoData?.RestoreAfterLoad();
            }
        }
        
        // 处理已装备的装备
        if (gameInfo.AllEquipmentData?.curEquippedEquipList != null)
        {
            foreach (var equip in gameInfo.AllEquipmentData.curEquippedEquipList)
            {
                equip?.RestoreAfterLoad();
            }
        }
        
        // 处理玩家身上的装备
        if (gameInfo.playerPeople?.curEquipItemList != null)
        {
            foreach (var item in gameInfo.playerPeople.curEquipItemList)
            {
                item?.equipProtoData?.RestoreAfterLoad();
            }
        }
        
        // 处理弟子身上的装备
        if (gameInfo.studentData?.allStudentList != null)
        {
            foreach (var student in gameInfo.studentData.allStudentList)
            {
                if (student?.curEquipItemList != null)
                {
                    foreach (var item in student.curEquipItemList)
                    {
                        item?.equipProtoData?.RestoreAfterLoad();
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 配置表加载完成后调用：为所有 ItemData 和 EquipProtoData 重新赋值 setting
    /// </summary>
    public void RestoreAllSettings(GameInfo gameInfo)
    {
        if (gameInfo == null) return;

        // 处理背包中的物品
        if (gameInfo.ItemModel?.itemDataList != null)
        {
            foreach (var item in gameInfo.ItemModel.itemDataList)
            {
                if (item == null) continue;
                item.setting = DataTable.FindItemSetting(item.settingId);
                if (item.equipProtoData != null)
                {
                    item.equipProtoData.setting = DataTable.FindEquipSetting(item.equipProtoData.settingId);
                }
            }
        }

        // 处理仓库中的物品
        if (gameInfo.ItemModel?.cangKuItemDataList != null)
        {
            foreach (var item in gameInfo.ItemModel.cangKuItemDataList)
            {
                if (item == null) continue;
                item.setting = DataTable.FindItemSetting(item.settingId);
                if (item.equipProtoData != null)
                {
                    item.equipProtoData.setting = DataTable.FindEquipSetting(item.equipProtoData.settingId);
                }
            }
        }

        // 处理已装备的装备
        if (gameInfo.AllEquipmentData?.curEquippedEquipList != null)
        {
            foreach (var equip in gameInfo.AllEquipmentData.curEquippedEquipList)
            {
                if (equip == null) continue;
                equip.setting = DataTable.FindEquipSetting(equip.settingId);
            }
        }

        // 处理玩家身上的装备
        if (gameInfo.playerPeople?.curEquipItemList != null)
        {
            for (int i = 0; i < gameInfo.playerPeople.curEquipItemList.Count; i++)
            {
                var item = gameInfo.playerPeople.curEquipItemList[i];
                if (item == null) continue;
                // 清理无效的装备数据（ES3反序列化可能创建空对象）
                if (item.settingId <= 0)
                {
                    gameInfo.playerPeople.curEquipItemList[i] = null;
                    continue;
                }
                item.setting = DataTable.FindItemSetting(item.settingId);
                if (item.equipProtoData != null)
                {
                    item.equipProtoData.setting = DataTable.FindEquipSetting(item.equipProtoData.settingId);
                }
            }
        }

        // 处理弟子身上的装备
        if (gameInfo.studentData?.allStudentList != null)
        {
            foreach (var student in gameInfo.studentData.allStudentList)
            {
                if (student?.curEquipItemList == null) continue;
                for (int i = 0; i < student.curEquipItemList.Count; i++)
                {
                    var item = student.curEquipItemList[i];
                    if (item == null) continue;
                    // 清理无效的装备数据（ES3反序列化可能创建空对象）
                    if (item.settingId <= 0)
                    {
                        student.curEquipItemList[i] = null;
                        continue;
                    }
                    item.setting = DataTable.FindItemSetting(item.settingId);
                    if (item.equipProtoData != null)
                    {
                        item.equipProtoData.setting = DataTable.FindEquipSetting(item.equipProtoData.settingId);
                    }
                }
            }
        }

    }
    /// <summary>
    /// 测试模式：自动修改存档数据
    /// 将玩家等级、属性、建筑、新手教程、地图关卡全部设为最高/完成状态
    /// </summary>
    private void ApplyTestModifications(GameInfo gameInfo)
    {
        if (gameInfo == null) return;
        
        Debug.Log("[TestMod] 开始应用测试修改...");

        // 2.1 设置所有丹炉建筑，解锁位置，排序好
        if (gameInfo.allDanFarmData != null && gameInfo.allDanFarmData.DanFarmList != null)
        {
            // 先解锁丹炉位置
            gameInfo.allDanFarmData.DanFarmZuoZhenStudentLimit = 4;
            gameInfo.allDanFarmData.UnlockedDanFarmNumLimit = 100;
            gameInfo.allDanFarmData.UnlockedFarmNum = 100;
            
            // 解锁所有丹炉类型
            if (DataTable.table != null && DataTable.table.TbDanFarm != null)
            {
                var allDanFarms = DataTable.table.TbDanFarm.DataList;
                if (allDanFarms != null)
                {
                    foreach (var danFarmSetting in allDanFarms)
                    {
                        if (danFarmSetting != null && !gameInfo.allDanFarmData.UnlockedDanFarmId.Contains(danFarmSetting.Id.ToInt32()))
                        {
                            gameInfo.allDanFarmData.UnlockedDanFarmId.Add(danFarmSetting.Id.ToInt32());
                        }
                    }
                }
            }
            
            // 解锁所有丹炉位置
            foreach (var danFarm in gameInfo.allDanFarmData.DanFarmList)
            {
                if (danFarm != null)
                {
                    danFarm.PosUnlockStatusList.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        danFarm.PosUnlockStatusList.Add(true);
                    }
                }
            }
            
            // 使用正确的配置表ID创建丹炉建筑
            CreateAllDanFarms(gameInfo);
            
            Debug.Log($"[TestMod] 丹炉建筑已创建，解锁位置，空地全开");
        }
        
        // 3. 设置所有新手教程为已完成
        if (gameInfo.NewGuideData != null)
        {
            gameInfo.NewGuideData.finishedGuideIdList.Clear();
            gameInfo.NewGuideData.IdList.Clear();
            gameInfo.NewGuideData.AccomplishStatus.Clear();
            
            for (int i = 1; i <= 100; i++)
            {
                gameInfo.NewGuideData.finishedGuideIdList.Add(i);
                gameInfo.NewGuideData.IdList.Add(i);
                gameInfo.NewGuideData.AccomplishStatus.Add(2);
            }
            gameInfo.NewGuideData.curGuideId = 0;
            gameInfo.NewGuideData.curGuideStep = 0;
            Debug.Log($"[TestMod] 新手教程已完成 ({gameInfo.NewGuideData.finishedGuideIdList.Count} 个)");
        }
        
        // 4. 设置所有地图和关卡为解锁/通关状态（通过正常流程）
        if (gameInfo.AllMapData != null && gameInfo.AllMapData.MapList != null)
        {
            foreach (var map in gameInfo.AllMapData.MapList)
            {
                map.MapStatus = 4;
                map.LieXiMapStatus = 4;
                
                if (map.LevelList != null)
                {
                    foreach (var level in map.LevelList)
                    {
                        level.LevelStatus = 4;
                        level.HaveAccomplished = true;
                    }
                }
                if (map.FixedLevelList != null)
                {
                    foreach (var level in map.FixedLevelList)
                    {
                        level.LevelStatus = 4;
                        level.HaveAccomplished = true;
                    }
                }
            }
            // 触发任务检查
            TaskManager.Instance.TryAccomplishAllTask();
            Debug.Log($"[TestMod] 地图和关卡已全部解锁/通关 ({gameInfo.AllMapData.MapList.Count} 个地图)");
        }
        
        // 5. 宗门等级满（调用正常升级方法，触发建筑数量变化）
        if (gameInfo.allZongMenData != null)
        {
            int maxZongMenLevel = DataTable._zongMenUpgradeList.Count;
            
            // 先设置为1级
            gameInfo.allZongMenData.ZongMenLevel = 1;
            
            // 逐步升级到满级，使用正常升级方法触发建筑解锁、空地数量、体力上限、弟子数量等变化
            for (int level = 1; level < maxZongMenLevel; level++)
            {
                if (RoleManager.Instance != null && ZongMenManager.Instance != null)
                {
                    ZongMenManager.Instance.UpgradeZongMenLevel();
                }
                else
                {
                    // 降级处理：如果 Manager 未初始化，直接设置数据
                    ZongMenUpgradeSetting curSetting = DataTable._zongMenUpgradeList[level - 1];
                    ZongMenUpgradeSetting afterSetting = DataTable._zongMenUpgradeList[level];
                    
                    List<List<int>> beforeBuilding = CommonUtil.SplitCfg(curSetting.UnlockedBuilding);
                    List<List<int>> afterBuilding = CommonUtil.SplitCfg(afterSetting.UnlockedBuilding);
                    
                    for (int i = 0; i < beforeBuilding.Count; i++)
                    {
                        List<int> before = beforeBuilding[i];
                        List<int> after = afterBuilding[i];
                        int buildId = before[0];
                        int beforeNum = before[1];
                        int afterNum = after[1];
                        
                        int existedNum = 0;
                        for (int j = 0; j < gameInfo.allDanFarmData.UnlockedDanFarmId.Count; j++)
                        {
                            int id = gameInfo.allDanFarmData.UnlockedDanFarmId[j];
                            if (id == buildId)
                            {
                                existedNum++;
                            }
                        }
                        while (afterNum > existedNum)
                        {
                            LianDanManager.Instance.UnlockDanFarm(buildId);
                            existedNum++;
                        }
                    }
                    
                    int farmNumBefore = curSetting.FarmNumLimit.ToInt32();
                    int farmNumAfter = afterSetting.FarmNumLimit.ToInt32();
                    if (farmNumAfter > farmNumBefore)
                    {
                        gameInfo.allDanFarmData.UnlockedDanFarmNumLimit = farmNumAfter + gameInfo.allZongMenData.SendFarmNumLimitAddNum;
                    }
                    
                    gameInfo.allZongMenData.ZongMenLevel++;
                }
            }
            
            Debug.Log($"[TestMod] 宗门等级已设为满级 ({maxZongMenLevel})，建筑已按等级解锁");
        }
        
        // 6. 探索数据全解锁（直接设置数据）
        if (gameInfo.AllExploreData != null && gameInfo.AllExploreData.ExploreList != null)
        {
            foreach (var exploreData in gameInfo.AllExploreData.ExploreList)
            {
                if (exploreData != null)
                {
                    exploreData.Unlocked = true;
                }
            }
            Debug.Log($"[TestMod] 探索已全部解锁 ({gameInfo.AllExploreData.ExploreList.Count} 个)");
        }
        
        // 7. 成就数据全完成（通过正常流程）
        if (gameInfo.AllAchievementData != null && gameInfo.AllAchievementData.achievementList != null)
        {
            foreach (var achievement in gameInfo.AllAchievementData.achievementList)
            {
                if (achievement != null)
                {
                    achievement.accomplishStatus = 2;
                    achievement.curProgress = 9999;
                }
            }
            // 触发任务检查
            TaskManager.Instance.TryAccomplishAllTask();
            TaskManager.Instance.TryAccomplishAllGuideBook();
            Debug.Log($"[TestMod] 成就已全部完成 ({gameInfo.AllAchievementData.achievementList.Count} 个)");
        }
        
        // 8. 空地全开 - 解锁所有丹田位置
        if (gameInfo.allDanFarmData != null && gameInfo.allDanFarmData.DanFarmList != null)
        {
            foreach (var danFarm in gameInfo.allDanFarmData.DanFarmList)
            {
                danFarm.PosUnlockStatusList.Clear();
                for (int i = 0; i < 4; i++)
                {
                    danFarm.PosUnlockStatusList.Add(true);
                }
                Debug.Log($"[TestMod] 丹田空地已全开");
            }
        }

        // 9.5 玩家升级到999级（与修武弟子一样的升级流程）
        if (gameInfo.playerPeople != null)
        {
            UpgradePlayerTo999Level(gameInfo.playerPeople);
        }

        // 9. 设置现有弟子只增加经验不修改等级 (不修改)
        // if (gameInfo.studentData?.allStudentList != null)
        // {
        //     foreach (var student in gameInfo.studentData.allStudentList)
        //     {
        //         if (student != null)
        //         {
        //             student.studentCurExp = 0;
        //             student.curXiuwei = 0;
        //         }
        //     }
        //     Debug.Log($"[TestMod] 现有弟子经验已设为满 ({gameInfo.studentData.allStudentList.Count} 个)");
        // }

        // 10. 创建最高品质的各职业随从各4个
        CreateMaxQualityStudents(gameInfo);

        // 11. 获得所有物品，数量为99999
        AddAllItems(gameInfo);
        
        // 12. 为玩家和所有弟子自动装备最好的装备
        AutoEquipBestGear(gameInfo);
        // 13. 初始化历练数据 - 解锁所有历练关卡
        if (gameInfo.timeData != null)
        {
            // 初始化今日参与历练状态
            gameInfo.timeData.TodayParticipatedLiLianStatus.Clear();
            gameInfo.timeData.LastParticipatedLiLianTime.Clear();

            if (DataTable._liLianList != null)
            {
                for (int i = 0; i < DataTable._liLianList.Count; i++)
                {
                    gameInfo.timeData.TodayParticipatedLiLianStatus.Add(0);
                    gameInfo.timeData.LastParticipatedLiLianTime.Add(0);
                }
            }

            // 设置每日最大历练次数
            gameInfo.timeData.MaxLiLianTimePerDay = 2;

            Debug.Log($"[TestMod] 历练数据已初始化，解锁所有历练关卡");
        }
        // 14. 开启历练功能
        if (LiLianManager.Instance != null)
        {
            LiLianManager.Instance.SetLiLianEnabled(true);
            Debug.Log("[TestMod] 历练功能已开启");
        }
        Debug.Log("[TestMod] 测试修改应用完成！");
    }
    
    private void AddAllItems(GameInfo gameInfo)
    {
        if (gameInfo.ItemModel == null)
        {
            gameInfo.ItemModel = new ItemModel();
        }
        
        if (DataTable.table != null && DataTable.table.TbItem != null)
        {
            var allItems = DataTable.table.TbItem.DataList;
            if (allItems != null)
            {
                foreach (var itemSetting in allItems)
                {
                    if (itemSetting == null) continue;
                    
                    int itemId = itemSetting.Id.ToInt32();
                    
                    ItemData newItem = new ItemData();
                    newItem.settingId = itemId;
                    newItem.count = 9999999999999999;
                    newItem.onlyId = ConstantVal.SetId;
                    
                    if (itemSetting.Quality != "-1")
                    {
                        newItem.quality = itemSetting.Quality.ToInt32();
                    }
                    newItem.setting = itemSetting;
                    
                    gameInfo.ItemModel.itemIdList.Add(itemId);
                    gameInfo.ItemModel.itemDataList.Add(newItem);
                    gameInfo.ItemModel.onlyIdList.Add(newItem.onlyId);
                }
                Debug.Log($"[TestMod] 已添加 {allItems.Count} 种物品，每种 99999999999999 个（直接添加数据）");
            }
        }
    }
    
    private void CreateMaxQualityStudents(GameInfo gameInfo)
    {
        if (gameInfo == null) return;
        
        if (gameInfo.studentData == null)
        {
            gameInfo.studentData = new StudentData();
        }
        
        int maxQuality = (int)Quality.Gold;
        int maxRarity = (int)cfg.Rarity.orange; // 资源图片只有1-5，红色(6)没有图片
        
        var talents = new List<StudentTalent>
        {
            StudentTalent.LianJing,
            StudentTalent.DuanZhao,
            StudentTalent.LianGong,
            StudentTalent.CaiKuang,
            StudentTalent.ChaoYao,
            StudentTalent.JingWen,
            StudentTalent.BaoShi,
            StudentTalent.JingShang
        };
        
        int studentCount = 0;
        
        foreach (var talent in talents)
        {
            for (int i = 0; i < 4; i++)
            {
                PeopleData student = CreateMaxQualityStudent(talent, maxQuality, maxRarity, gameInfo);
                // 使用正常招募流程添加弟子
                StudentManager.Instance.AddStudent(student);
                studentCount++;
                
                // 调用升级流程将弟子升级到999级
                UpgradeStudentTo999Level(student);
                
                if (talent == StudentTalent.LianGong)
                {
                    SetupLianGongStudentMax(student, gameInfo);
                }
            }
        }
        
        gameInfo.studentData.CurStudentNum = gameInfo.studentData.allStudentList.Count;
        gameInfo.studentData.MaxStudentNum = 1000;
        
        Debug.Log($"[TestMod] 已创建 {studentCount} 个各职业随从（每职业4个）");
    }

    private void UpgradeStudentTo999Level(PeopleData p)
    {
        if (p == null) return;
        
        bool isLianGong = (p.talent == (int)StudentTalent.LianGong);
        int targetLevel = 999;
        
        if (isLianGong)
        {
            Debug.Log($"[TestMod] 开始将修武弟子 {p.name} 从 trainIndex {p.trainIndex} 升级到 {targetLevel}");
            
            while (p.trainIndex < targetLevel)
            {
                int curLevelLimit = StudentManager.Instance.GetStudentLevelLimit(p);
                
                if (p.trainIndex >= curLevelLimit)
                {
                    Debug.Log($"[TestMod] 修武弟子 {p.name} 达到境界上限 {curLevelLimit}，停止升级");
                    break;
                }
                
                if (p.trainIndex >= DataTable._trainList.Count - 1)
                {
                    Debug.Log($"[TestMod] 修武弟子 {p.name} 达到配置表上限，停止升级");
                    break;
                }
                
                TrainSetting curTrainSetting = DataTable._trainList[p.trainIndex];
                ulong xiuweiNeed = curTrainSetting.XiuWeiNeed.ToUInt64();
                
                if (p.curXiuwei < xiuweiNeed)
                {
                    p.curXiuwei += (ulong)(xiuweiNeed - p.curXiuwei + 10000000);
                }
                
                int originalNextBreak = p.nextBreakThroughAdd;
                p.nextBreakThroughAdd = 100;
                
                StudentManager.Instance.OnBreakThrough(p);
                
                p.nextBreakThroughAdd = originalNextBreak;
            }
            
            Debug.Log($"[TestMod] 修武弟子 {p.name} 升级完成，当前 trainIndex {p.trainIndex}");
        }
        else
        {
            Debug.Log($"[TestMod] 开始将弟子 {p.name} 从等级 {p.studentLevel} 升级到 {targetLevel}");
            
            while (p.studentLevel < targetLevel)
            {
                int curLevelLimit = StudentManager.Instance.GetStudentLevelLimit(p);
                
                if (p.studentLevel >= curLevelLimit)
                {
                    Debug.Log($"[TestMod] 弟子 {p.name} 达到等级上限 {curLevelLimit}，停止升级");
                    break;
                }
                
                if (p.studentLevel > 0 && p.studentLevel <= DataTable._studentUpgradeList.Count)
                {
                    StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                    int needExp = setting.NeedExp.ToInt32();
                    p.studentCurExp += needExp;
                }
                else
                {
                    p.studentCurExp += 1000000;
                }
                
                while (p.studentLevel < targetLevel)
                {
                    curLevelLimit = StudentManager.Instance.GetStudentLevelLimit(p);
                    
                    if (p.studentLevel >= curLevelLimit)
                    {
                        break;
                    }
                    
                    if (p.studentLevel > 0 && p.studentLevel <= DataTable._studentUpgradeList.Count)
                    {
                        StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                        int needExp = setting.NeedExp.ToInt32();
                        
                        if (p.studentCurExp < needExp)
                        {
                            Debug.Log($"[TestMod] 弟子 {p.name} 经验不足，停止升级，当前等级 {p.studentLevel}");
                            return;
                        }
                        
                        p.studentLevel++;
                        p.studentCurExp -= needExp;
                    }
                    else
                    {
                        if (p.studentCurExp < 1000000)
                        {
                            return;
                        }
                        p.studentLevel++;
                        p.studentCurExp -= 1000000;
                    }
                    
                    for (int i = 0; i < p.propertyList.Count; i++)
                    {
                        Quality proQuality = (Quality)(int)p.propertyList[i].quality;
                        int valEquip = StudentManager.Instance.StudentBreakThroughAdd((StudentTalent)(int)p.talent, proQuality);
                        p.propertyList[i].num += valEquip;
                        if (p.propertyList[i].num >= 300)
                        {
                            p.propertyList[i].num = 300;
                        }
                    }
                    
                    for (int i = 0; i < p.curBattleProList.Count; i++)
                    {
                        Quality proQuality = (Quality)(int)p.curBattleProList[i].quality;
                        int valEquip = StudentManager.Instance.StudentBreakThroughAdd((StudentTalent)(int)p.talent, proQuality);
                        p.curBattleProList[i].num += valEquip;
                        if (p.curBattleProList[i].num >= 300)
                        {
                            p.curBattleProList[i].num = 300;
                        }
                    }
                }
            }
            
            Debug.Log($"[TestMod] 弟子 {p.name} 升级完成，当前等级 {p.studentLevel}");
        }
    }

    private void UpgradePlayerTo999Level(PeopleData p)
    {
        if (p == null) return;

        int targetLevel = 999;

        if (p.trainIndex == 0)
        {
            p.trainIndex = 0;
        }
        if (p.curXiuwei == 0)
        {
            p.curXiuwei = 0;
        }

        Debug.Log($"[TestMod] 开始将玩家 {p.name} 从 trainIndex {p.trainIndex} 升级到 {targetLevel}");

        while (p.trainIndex < targetLevel)
        {
            int curLevelLimit = StudentManager.Instance.GetStudentLevelLimit(p);

            if (p.trainIndex >= curLevelLimit)
            {
                Debug.Log($"[TestMod] 玩家 {p.name} 达到境界上限 {curLevelLimit}，停止升级");
                break;
            }

            if (p.trainIndex >= DataTable._trainList.Count - 1)
            {
                Debug.Log($"[TestMod] 玩家 {p.name} 达到配置表上限，停止升级");
                break;
            }

            TrainSetting curTrainSetting = DataTable._trainList[p.trainIndex];
            ulong xiuweiNeed = curTrainSetting.XiuWeiNeed.ToUInt64();

            if (p.curXiuwei < xiuweiNeed)
            {
                p.curXiuwei += (ulong)(xiuweiNeed - p.curXiuwei + 10000000);
            }

            int originalNextBreak = p.nextBreakThroughAdd;
            int originalEatedDanNum = p.curEatedDanNum;
            p.nextBreakThroughAdd = 100;
            p.curEatedDanNum = 10;

            StudentManager.Instance.OnBreakThrough(p);

            p.nextBreakThroughAdd = originalNextBreak;
            p.curEatedDanNum = originalEatedDanNum;
        }

        Debug.Log($"[TestMod] 玩家 {p.name} 升级完成，当前 trainIndex {p.trainIndex}");
    }
    
    private PeopleData CreateMaxQualityStudent(StudentTalent talent, int quality, int rarity, GameInfo gameInfo)
    {
        PeopleData p = new PeopleData();
        p.onlyId = gameInfo.TheId++;
        p.name = GetStudentNameByTalent(talent);
        p.studentType = (int)StudentType.WaiMen;
        p.talent = (int)talent;
        p.studentQuality = quality;
        p.studentRarity = rarity;
        
        // 设置默认等级和经验（以便可以使用经验丹）
        p.studentLevel = 1;
        p.studentCurExp = 0;
        
        p.propertyIdList = new List<int>();
        p.propertyList = new List<SinglePropertyData>();
        p.curBattleProIdList = new List<int>();
        p.curBattleProList = new List<SinglePropertyData>();
        
        p.portraitIndexList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        p.portraitType = (int)PortraitType.ChangeFace;
        
        string proStr = ConstantVal.baseLianGongStudentPro;
        List<List<int>> baseBattleProList = CommonUtil.SplitCfg(ConstantVal.baseBattleProperty);
        List<List<int>> proList = CommonUtil.SplitCfg(proStr);
        
        List<int> haveValIdList = new List<int>();
        List<int> haveValValList = new List<int>();
        
        for (int i = 0; i < proList.Count; i++)
        {
            List<int> thePro = proList[i];
            haveValIdList.Add(thePro[0]);
            haveValValList.Add(thePro[1]);
        }
        
        for (int i = 0; i < baseBattleProList.Count; i++)
        {
            List<int> singlePro = baseBattleProList[i];
            int theId = singlePro[0];
            int theNum = singlePro[1];
            
            if (haveValIdList.Contains(theId))
            {
                int index = haveValIdList.IndexOf(theId);
                theNum = haveValValList[index];
            }
            
            SinglePropertyData pro = new SinglePropertyData();
            pro.id = theId;
            pro.num = theNum;
            pro.quality = 5;
            if (theId == (int)PropertyIdType.MpNum)
            {
                pro.limit = 100;
            }
            else if (theId == (int)PropertyIdType.Hp)
            {
                pro.limit = theNum;
            }
            
            p.propertyIdList.Add(theId);
            p.propertyList.Add(pro);
            
            SinglePropertyData battlePro = new SinglePropertyData();
            battlePro.id = theId;
            battlePro.num = theNum;
            battlePro.limit = pro.limit;
            battlePro.quality = 5;
            
            p.curBattleProIdList.Add(theId);
            p.curBattleProList.Add(battlePro);
        }

        // 使用正常流程初始化技能
        p.allSkillData = new AllSkillData();
        p.allSkillData.unlockedSkillPos = 3;
        p.allSkillData.unlockedTypeList = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            p.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);
        }
        
        // 添加基础技能（根据元素类型）
        SingleSkillData singleSkill = new SingleSkillData();
        singleSkill.skillId = (int)BattleManager.Instance.PuGongIdByYuanSu((YuanSuType)p.yuanSu);
        singleSkill.skillLevel = 1;
        p.allSkillData.skillList.Add(singleSkill);
        p.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);

        p.curEquipItemList = new List<ItemData> { null, null, null, null };
        
        p.gender = UnityEngine.Random.Range(0, 2);
        p.yuanSu = UnityEngine.Random.Range(1, 6);
        p.enemySettingId = 0;
        p.curPhase = 1;
        p.totalPhase = 10;
        p.xiSuiRate = 100;
        p.talentRarity = 5;
        
        // 设置随机头像（和正常招募流程一致）
        RoleManager.Instance.RdmFace(p);
        
        p.xueMai = new XueMaiData();
        p.xueMai.xueMaiTypeList = new List<XueMaiType>();
        p.xueMai.xueMaiLevelList = new List<int>();
        for (int i = 1; i < 6; i++)
        {
            p.xueMai.xueMaiTypeList.Add((XueMaiType)i);
            p.xueMai.xueMaiLevelList.Add(0);
        }
        
        p.socializationData = new SocializationData();
        p.socializationData.knowPeopleList = new List<ulong>();
        p.socializationData.haoGanDu = new List<int>();
        p.socializationData.socialRecordList = new List<SocializationRecordData>();
        
        return p;
    }
    
    private string GetStudentNameByTalent(StudentTalent talent)
    {
        switch (talent)
        {
            case StudentTalent.LianJing: return "炼丹弟子";
            case StudentTalent.DuanZhao: return "炼器弟子";
            case StudentTalent.LianGong: return "修武弟子";
            case StudentTalent.CaiKuang: return "采矿弟子";
            case StudentTalent.ChaoYao: return "灵田弟子";
            case StudentTalent.JingWen: return "经文弟子";
            case StudentTalent.BaoShi: return "宝石弟子";
            case StudentTalent.JingShang: return "经商弟子";
            default: return "弟子";
        }
    }
    
    private void SetupLianGongStudentMax(PeopleData p, GameInfo gameInfo)
    {
        if (p.curEquipItemList != null && DataTable.table.TbEquipment != null)
        {
            var allEquipSettings = DataTable.table.TbEquipment.DataList;
            if (allEquipSettings != null && allEquipSettings.Count > 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    var bestEquip = FindBestEquipmentForSlot(i, allEquipSettings);
                    if (bestEquip != null)
                    {
                        ItemData item = new ItemData();
                        item.settingId = bestEquip.Id.ToInt32();
                        item.onlyId = gameInfo.TheId++;
                        item.quality = (int)Quality.Gold;
                        item.count = 1;
                        
                        EquipProtoData equipProto = new EquipProtoData();
                        equipProto.settingId = item.settingId;
                        equipProto.onlyId = item.onlyId;
                        equipProto.curLevel = 100;
                        equipProto.curExp = 999999;
                        equipProto.curDurability = 100;
                        equipProto.jingLianLv = 10;
                        equipProto.propertyList = new List<SinglePropertyData>();
                        
                        item.equipProtoData = equipProto;
                        p.curEquipItemList[i] = item;
                    }
                }
            }
        }
        
        Debug.Log($"[TestMod] 修武弟子已设置：装备 {p.curEquipItemList.FindAll(x => x != null).Count} 件");
    }
    
    private EquipmentSetting FindBestEquipmentForSlot(int slotIndex, List<EquipmentSetting> allEquipSettings)
    {
        EquipmentSetting best = null;
        int bestRarity = 0;
        
        string[] slotNames = { "法器", "锦衣", "鞋子", "璎珞", "饰品", "腰带" };
        string slotName = slotIndex < slotNames.Length ? slotNames[slotIndex] : "未知";
        
        foreach (var equip in allEquipSettings)
        {
            if (equip == null) continue;
            
            int equipType = equip.Pos.ToInt32();
            bool matchesSlot = false;
            
            switch (slotIndex)
            {
                case 0: matchesSlot = (equipType == 1); break;
                case 1: matchesSlot = (equipType == 2); break;
                case 2: matchesSlot = (equipType == 3); break;
                case 3: matchesSlot = (equipType == 4); break;
                case 4: matchesSlot = (equipType == 5); break;
                case 5: matchesSlot = (equipType == 6); break;
            }
            
            if (matchesSlot)
            {
                int rarity = equip.Rarity.ToInt32();
                Debug.Log($"[TestMod] 槽位 {slotIndex}({slotName}) 找到装备: {equip.Name}, 类型={equipType}, 稀有度={rarity}");
                if (rarity > bestRarity)
                {
                    bestRarity = rarity;
                    best = equip;
                }
            }
        }
        
        if (best == null)
        {
            Debug.LogWarning($"[TestMod] 槽位 {slotIndex}({slotName}) 没有找到可用装备");
        }
        
        return best;
    }

    private void InitPlayerSkillsFullLevel(PeopleData p)
    {
        if (p == null) return;

        // 初始化技能数据
        if (p.allSkillData == null)
        {
            p.allSkillData = new AllSkillData();
        }
        if (p.allSkillData.equippedSkillIdList == null)
        {
            p.allSkillData.equippedSkillIdList = new List<int>();
        }
        if (p.allSkillData.skillList == null)
        {
            p.allSkillData.skillList = new List<SingleSkillData>();
        }

        // 解锁所有技能槽位
        p.allSkillData.unlockedSkillPos = 3;
        if (p.allSkillData.unlockedTypeList == null)
        {
            p.allSkillData.unlockedTypeList = new List<int>();
        }
        else
        {
            p.allSkillData.unlockedTypeList.Clear();
        }
        for (int i = 0; i < 8; i++)
        {
            p.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);
        }

        // 获取所有可用的技能ID列表
        List<int> skillIdList = new List<int>();
        if (DataTable.table != null && DataTable.table.TbSkill != null)
        {
            var allSkills = DataTable.table.TbSkill.DataList;
            if (allSkills != null)
            {
                // 添加一些基础技能
                skillIdList.Add(1); //  LingDan
                skillIdList.Add(2); //  PuGong
                skillIdList.Add(3); //  FangYu
                skillIdList.Add(4); //  ZhiLiao
                
                // 获取弟子天赋对应的技能
                if (p.talent == (int)StudentTalent.LianGong)
                {
                    skillIdList.Add(5); // XiuLian
                }
            }
        }

        // 清空现有技能并添加满级技能
        p.allSkillData.skillList.Clear();
        p.allSkillData.equippedSkillIdList.Clear();

        // 获取技能升级配置表的最大等级
        int maxSkillLevel = 50; // 默认最大等级
        if (DataTable._skillUpgradeList != null && DataTable._skillUpgradeList.Count > 0)
        {
            maxSkillLevel = DataTable._skillUpgradeList.Count;
        }

        // 添加技能并设置为满级
        foreach (int skillId in skillIdList)
        {
            SingleSkillData skillData = new SingleSkillData();
            skillData.skillId = skillId;
            skillData.skillLevel = maxSkillLevel;
            p.allSkillData.skillList.Add(skillData);
            
            // 装备这个技能
            p.allSkillData.equippedSkillIdList.Add(skillId);
        }

        Debug.Log($"[TestMod] 技能已设置为满级 {maxSkillLevel}，共 {p.allSkillData.skillList.Count} 个技能");
    }
    
    private void CreateAllDanFarms(GameInfo gameInfo)
    {
        if (DataTable.table == null || DataTable.table.TbDanFarm == null)
        {
            Debug.LogWarning("[TestMod] 配置表未加载，跳过创建丹炉建筑");
            return;
        }
        
        var allDanFarms = DataTable.table.TbDanFarm.DataList;
        if (allDanFarms == null || allDanFarms.Count == 0)
        {
            Debug.LogWarning("[TestMod] 没有丹炉配置，跳过创建");
            return;
        }
        
        Debug.Log($"[TestMod] 开始创建丹炉建筑，共 {allDanFarms.Count} 种");
        
        // 清空现有丹炉列表，重新创建
        gameInfo.allDanFarmData.DanFarmList.Clear();
        
        // 第一个建筑位置
        float startX = -1975f;
        float startY = 1835f;
        
        // 每个建筑之间的间距
        float spacingX = 250f;
        float spacingY = -250f;
        
        // 每排最多6个
        int perRow = 6;
        
        int index = 0;
        
        foreach (var danFarmSetting in allDanFarms)
        {
            if (danFarmSetting == null) continue;
            
            int settingId = danFarmSetting.Id.ToInt32();
            
            // 每个类型的丹炉创建1个
            SingleDanFarmData danFarm = new SingleDanFarmData();
            danFarm.OnlyId = (ulong)(gameInfo.TheId++);
            danFarm.SettingId = settingId;
            danFarm.IsEmpty = false;
            danFarm.Index = index;
            danFarm.DanFarmType = danFarmSetting.Type.ToInt32();
            
            // 计算位置：每排6个，超过换排
            int row = index / perRow;
            int col = index % perRow;
            danFarm.LocalPos = new Vector2(
                startX + col * spacingX,
                startY + row * spacingY
            );
            
            // 获取丹炉满级
            List<int> upgradeCostList = CommonUtil.SplitCfgOneDepth(danFarmSetting.UpgradeCost);
            int maxLevel = upgradeCostList.Count;
            
            // 状态为Idle（空闲状态，可以直接使用）
            danFarm.Status = 1; // Idling状态
            danFarm.RemainTime = 0;
            danFarm.ProcessDanTimer = 0;
            danFarm.RebuildTotalTime = 0;
            danFarm.OpenQuanLi = false;
            danFarm.QuanLiTotalTime = 0;
            danFarm.QuanliRemainTime = 0;
            danFarm.ProcessSpeed = 0;
            // 从配置表获取产品ID，如果无效则设置为灵识(10001)
            int productId = danFarmSetting.Param.ToInt32();
            if (productId <= 0)
            {
                productId = (int)ItemIdType.LingShi;
            }
            danFarm.ProductSettingId = productId;
            danFarm.ProductRemainNum = 0;
            danFarm.ProductTotalNum = 0;
            danFarm.HandleStop = false;
            danFarm.NeedForeItemId = 0;
            danFarm.SingleDanPrice = 0;
            danFarm.Unlocked = true;
            danFarm.TalentType = 0;
            
            // 先初始化坐镇位置列表，确保有足够的元素
            for (int j = 0; j < 4; j++)
            {
                danFarm.ZuoZhenStudentIdList.Add(0);
            }
            
            danFarm.PosUnlockStatusList.Clear();
            for (int j = 0; j < 4; j++)
            {
                danFarm.PosUnlockStatusList.Add(false);
            }
            
            // 初始等级为1，然后调用升级逻辑到满级
            danFarm.CurLevel = 1;
            
            // 调用同步升级方法到满级
            LianDanManager.Instance.DanFarmUpgradeToMaxForTest(danFarm);
            
            danFarm.Status = 1; // Idling状态
            
            danFarm.StudentUseCangKuDataList = new List<SingleStudentUseCangKuData>();
            danFarm.ProductItemList = new List<ItemData>();
            danFarm.UnlockedProductIdList = new List<int>();
            
            gameInfo.allDanFarmData.DanFarmList.Add(danFarm);
            index++;
        }
        
        Debug.Log($"[TestMod] 已创建 {index} 个丹炉建筑，每种1个，每排6个按顺序排列");
    }
    
    private void AutoEquipBestGear(GameInfo gameInfo)
    {
        if (gameInfo == null) return;
        
        var allEquipSettings = DataTable.table?.TbEquipment?.DataList;
        if (allEquipSettings == null || allEquipSettings.Count == 0)
        {
            Debug.Log($"[TestMod] 没有找到装备配置表，无法自动装备");
            return;
        }
        
        Debug.Log($"[TestMod] 装备配置表加载成功，共 {allEquipSettings.Count} 个装备");
        
        int equipCount = 0;
        
        // 为玩家装备 - 使用正常装备方式
        if (gameInfo.playerPeople != null)
        {
            // 确保技能数据已初始化 - 使用正常流程
            InitPlayerSkillsFullLevel(gameInfo.playerPeople);

            for (int i = 0; i < 4; i++)
            {
                var bestEquip = FindBestEquipmentForSlot(i, allEquipSettings);
                Debug.Log($"[TestMod] 槽位 {i} 找到最佳装备: {(bestEquip != null ? bestEquip.Name : "null")}");
                if (bestEquip != null)
                {
                    // 先从背包移除该物品（如果已存在）
                    ItemData existingItem = gameInfo.playerPeople.curEquipItemList[i];
                    if (existingItem != null && existingItem.settingId > 0)
                    {
                        // 卸下现有装备
                        existingItem.equipProtoData.isEquipped = false;
                        existingItem.equipProtoData.belongP = 0;
                    }

                    // 创建新装备
                    ItemData item = CreateBestEquipItem(bestEquip, gameInfo);
                    
                    // 添加到背包
                    if (gameInfo.ItemModel == null)
                    {
                        gameInfo.ItemModel = new ItemModel();
                    }
                    gameInfo.ItemModel.itemIdList.Add(item.settingId);
                    gameInfo.ItemModel.itemDataList.Add(item);
                    gameInfo.ItemModel.onlyIdList.Add(item.onlyId);

                    // 使用正常装备方式装备
                    EquipmentManager.Instance.OnEquip(gameInfo.playerPeople, item, i);
                    equipCount++;
                }
            }
            Debug.Log($"[TestMod] 玩家已自动装备 {equipCount} 件装备");
        }
        
        // 为所有弟子装备 - 使用正常装备方式
        if (gameInfo.studentData?.allStudentList != null)
        {
            foreach (var student in gameInfo.studentData.allStudentList)
            {
                if (student == null) continue;

                // 使用正常流程初始化弟子技能
                InitPlayerSkillsFullLevel(student);
                
                int studentEquipCount = 0;
                for (int i = 0; i < 4; i++)
                {
                    var bestEquip = FindBestEquipmentForSlot(i, allEquipSettings);
                    if (bestEquip != null)
                    {
                        // 先从背包移除该物品（如果已存在）
                        ItemData existingItem = student.curEquipItemList[i];
                        if (existingItem != null && existingItem.settingId > 0)
                        {
                            existingItem.equipProtoData.isEquipped = false;
                            existingItem.equipProtoData.belongP = 0;
                        }

                        // 创建新装备
                        ItemData item = CreateBestEquipItem(bestEquip, gameInfo);
                        
                        // 添加到背包
                        if (gameInfo.ItemModel == null)
                        {
                            gameInfo.ItemModel = new ItemModel();
                        }
                        gameInfo.ItemModel.itemIdList.Add(item.settingId);
                        gameInfo.ItemModel.itemDataList.Add(item);
                        gameInfo.ItemModel.onlyIdList.Add(item.onlyId);

                        // 使用正常装备方式装备
                        EquipmentManager.Instance.OnEquip(student, item, i);
                        studentEquipCount++;
                    }
                }
                Debug.Log($"[TestMod] 弟子 {student.name} 已自动装备 {studentEquipCount} 件装备");
            }
        }
        
        Debug.Log($"[TestMod] 自动装备完成，共装备 {equipCount} 件");
    }
    
    private ItemData CreateBestEquipItem(EquipmentSetting bestEquip, GameInfo gameInfo)
    {
        ItemData item = new ItemData();
        item.settingId = bestEquip.Id.ToInt32();
        item.onlyId = gameInfo.TheId++;
        item.quality = bestEquip.Rarity.ToInt32();
        item.count = 1;
        item.setting = DataTable.FindItemSetting(item.settingId);
        
        EquipProtoData equipProto = new EquipProtoData();
        equipProto.settingId = item.settingId;
        equipProto.onlyId = item.onlyId;
        equipProto.curLevel = 100;
        equipProto.curExp = 999999;
        equipProto.curDurability = 100;
        equipProto.jingLianLv = 10;
        equipProto.propertyList = new List<SinglePropertyData>();
        
        // 从装备配置表中解析基础属性
        List<List<int>> baseProList = CommonUtil.SplitCfg(bestEquip.BasePro);
        for (int i = 0; i < baseProList.Count; i++)
        {
            List<int> singlePro = baseProList[i];
            if (singlePro.Count >= 2)
            {
                SinglePropertyData proData = new SinglePropertyData();
                proData.id = singlePro[0];
                proData.num = singlePro[1];
                proData.quality = (int)Quality.None;
                equipProto.propertyList.Add(proData);
            }
        }
        
        equipProto.setting = bestEquip;
        
        item.equipProtoData = equipProto;
        
        return item;
    }
    #endregion
}