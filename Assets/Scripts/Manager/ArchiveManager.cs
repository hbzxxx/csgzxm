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

        //if (RoleManager.Instance._CurGameInfo == null)
        //{
        //    Debug.LogError("_CurGameInfo 为空，跳过保存");
        //    return;
        //}

        //GameInfo gameInfo = RoleManager.Instance._CurGameInfo;
        // 确保 ArchiveGenerator 实例存在
        if (ArchiveGenerator.Instance == null)
        {
            var go = new GameObject("ArchiveGenerator_Auto");
            go.AddComponent<ArchiveGenerator>();
        }
        GameInfo gameInfo = ArchiveGenerator.Instance.GetGameInfo();
        gameInfo.SaveTime = CGameTime.Instance.GetTimeStamp();
        
#if UNITY_EDITOR
        // 测试模式：自动修改存档数据
        ApplyTestModifications(gameInfo);
#endif
        
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
            Debug.Log($"[ArchiveManager] 验证存档 - 玩家等级: {verifyGameInfo.playerPeople?.studentLevel}, 主城等级: {verifyGameInfo.AllBuildingData?.MountainLevel}");
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
                Debug.Log($"[ArchiveManager] 验证存档 - 地图解锁: {unlockedMapCount}/{verifyGameInfo.AllMapData.MapList.Count}, 关卡解锁: {unlockedLevelCount}/{totalLevelCount}");
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
        
        return gameInfo;
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
                    Debug.LogError($"[存档异常] 玩家槽位 {i} 有空的 ItemData 对象，已清理");
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
                        Debug.LogError($"[存档异常] 弟子 {student.name} (onlyId={student.onlyId}) 槽位 {i} 有空的 ItemData 对象，已清理");
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

        Debug.Log("RestoreAllSettings 完成：所有 setting 已重新赋值");
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// 测试模式：自动修改存档数据
    /// 将玩家等级、属性、建筑、新手教程、地图关卡全部设为最高/完成状态
    /// </summary>
    private void ApplyTestModifications(GameInfo gameInfo)
    {
        if (gameInfo == null) return;
        
        Debug.Log("[TestMod] 开始应用测试修改...");
        
        // 1. 设置玩家等级为最高
        int maxStudentLevel = DataTable._studentUpgradeList.Count;
        if (gameInfo.playerPeople != null)
        {
            gameInfo.playerPeople.studentLevel = maxStudentLevel;
            gameInfo.playerPeople.studentCurExp = int.MaxValue;
            gameInfo.playerPeople.curXiuwei = ulong.MaxValue;
            Debug.Log($"[TestMod] 玩家等级已设为: {maxStudentLevel}");
            
            // 设置所有属性为最大值
            if (gameInfo.playerPeople.propertyList != null)
            {
                foreach (var prop in gameInfo.playerPeople.propertyList)
                {
                    prop.num = int.MaxValue;
                    prop.limit = long.MaxValue;
                }
            }
            if (gameInfo.playerPeople.curBattleProList != null)
            {
                foreach (var prop in gameInfo.playerPeople.curBattleProList)
                {
                    prop.num = int.MaxValue;
                    prop.limit = long.MaxValue;
                }
            }
            Debug.Log("[TestMod] 玩家属性已设为最大值");
        }
        
        // 2. 设置山门等级为最高
        if (gameInfo.AllBuildingData != null)
        {
            gameInfo.AllBuildingData.MountainLevel = 100; // 设置一个较高的山门等级
            
            // 设置所有建筑满级
            if (gameInfo.AllBuildingData.BuildList != null)
            {
                int equipMaxLevel = DataTable._equipBuildingUpgradeList.Count;
                int lianDanMaxLevel = DataTable._lianDanBuildingUpgradeList.Count;
                int maxBuildingLevel = Mathf.Max(equipMaxLevel, lianDanMaxLevel, 50);
                
                foreach (var building in gameInfo.AllBuildingData.BuildList)
                {
                    building.CurBuildLevel = maxBuildingLevel;
                    building.MaxStudentNum = 100;
                    building.StudentNum = 0;
                }
                Debug.Log($"[TestMod] 建筑已设为满级 (等级 {maxBuildingLevel})");
            }
        }
        
        // 3. 设置所有新手教程为已完成
        if (gameInfo.NewGuideData != null)
        {
            // 获取所有配置的新手引导ID
            if (DataTable._newGuideList != null)
            {
                gameInfo.NewGuideData.finishedGuideIdList.Clear();
                gameInfo.NewGuideData.IdList.Clear();
                gameInfo.NewGuideData.AccomplishStatus.Clear();
                
                foreach (var guide in DataTable._newGuideList)
                {
                    gameInfo.NewGuideData.finishedGuideIdList.Add(guide.Id);
                    gameInfo.NewGuideData.IdList.Add(guide.Id);
                    gameInfo.NewGuideData.AccomplishStatus.Add(2); // 2 = 已完成
                }
                gameInfo.NewGuideData.curGuideId = 0;
                gameInfo.NewGuideData.curGuideStep = 0;
                Debug.Log($"[TestMod] 新手教程已完成 ({gameInfo.NewGuideData.finishedGuideIdList.Count} 个)");
            }
        }
        
        // 4. 设置所有地图和关卡为解锁/通关状态
        if (gameInfo.AllMapData != null && gameInfo.AllMapData.MapList != null)
        {
            foreach (var map in gameInfo.AllMapData.MapList)
            {
                map.MapStatus = 2; // 2 = 已解锁
                
                if (map.LevelList != null)
                {
                    foreach (var level in map.LevelList)
                    {
                        level.LevelStatus = 2; // 2 = 已通关
                        level.HaveAccomplished = true;
                    }
                }
                if (map.FixedLevelList != null)
                {
                    foreach (var level in map.FixedLevelList)
                    {
                        level.LevelStatus = 2;
                        level.HaveAccomplished = true;
                    }
                }
            }
            Debug.Log($"[TestMod] 地图和关卡已全部解锁/通关 ({gameInfo.AllMapData.MapList.Count} 个地图)");
        }
        
        // 5. 尝试解锁更多内容（如果存在）
        // 宗门等级
        if (gameInfo.allZongMenData != null)
        {
            gameInfo.allZongMenData.ZongMenLevel = 100;
            Debug.Log("[TestMod] 宗门等级已设为满级");
        }
        
        // 探索数据
        if (gameInfo.AllExploreData != null && gameInfo.AllExploreData.ExploreList != null)
        {
            foreach (var explore in gameInfo.AllExploreData.ExploreList)
            {
                explore.Unlocked = true;
                explore.AllEventNum = 999;
            }
            Debug.Log("[TestMod] 探索已全部解锁");
        }
        
        // 成就数据
        if (gameInfo.AllAchievementData != null && gameInfo.AllAchievementData.achievementList != null)
        {
            foreach (var achievement in gameInfo.AllAchievementData.achievementList)
            {
                if (achievement != null)
                {
                    achievement.accomplishStatus = 2; // 2 = 已完成
                    achievement.curProgress = 9999;
                }
            }
            Debug.Log($"[TestMod] 成就已全部完成 ({gameInfo.AllAchievementData.achievementList.Count} 个)");
        }
        
        Debug.Log("[TestMod] 测试修改应用完成！");
    }
#endif
}