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

#if UNITY_EDITOR
        if (gameInfo.AllBuildingData == null)
        {
            Debug.Log("测试模式：自动修改存档数据");
            ApplyTestModifications(gameInfo);
        }
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
        
        // 1. 主角经验设为满
        if (gameInfo.playerPeople != null)
        {
            gameInfo.playerPeople.studentCurExp = 99999999;
            gameInfo.playerPeople.curXiuwei = 99999999;
            Debug.Log("[TestMod] 主角经验已设为 99999999");
        }
        
        // 2. 设置山门等级为最高，建筑全满
        if (gameInfo.AllBuildingData != null)
        {
            gameInfo.AllBuildingData.MountainLevel = 100;
            
            gameInfo.AllBuildingData.BuildList.Clear();
            
            int equipMaxLevel = DataTable._equipBuildingUpgradeList.Count;
            int lianDanMaxLevel = DataTable._lianDanBuildingUpgradeList.Count;
            
            // 先添加炼器房 (10002)
            for (int i = 0; i < 10; i++)
            {
                SingleBuildingData equipBuilding = new SingleBuildingData();
                equipBuilding.BuildTypeId = 10002;
                equipBuilding.SettingId = 10002;
                equipBuilding.CurBuildLevel = equipMaxLevel;
                equipBuilding.MaxStudentNum = 100;
                equipBuilding.StudentNum = 0;
                gameInfo.AllBuildingData.BuildList.Add(equipBuilding);
            }
            
            // 再添加炼丹房 (10001)
            for (int i = 0; i < 10; i++)
            {
                SingleBuildingData lianDanBuilding = new SingleBuildingData();
                lianDanBuilding.BuildTypeId = 10001;
                lianDanBuilding.SettingId = 10001;
                lianDanBuilding.CurBuildLevel = lianDanMaxLevel;
                lianDanBuilding.MaxStudentNum = 100;
                lianDanBuilding.StudentNum = 0;
                gameInfo.AllBuildingData.BuildList.Add(lianDanBuilding);
            }
            
            Debug.Log($"[TestMod] 建筑已设为满级 (炼器房 {equipMaxLevel} 级 x10, 炼丹房 {lianDanMaxLevel} 级 x10)");
        }
        
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
        
        // 4. 设置所有地图和关卡为解锁/通关状态
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
            Debug.Log($"[TestMod] 地图和关卡已全部解锁/通关 ({gameInfo.AllMapData.MapList.Count} 个地图)");
        }
        
        // 5. 宗门等级满
        if (gameInfo.allZongMenData != null)
        {
            int maxZongMenLevel = DataTable._zongMenUpgradeList.Count;
            gameInfo.allZongMenData.ZongMenLevel = maxZongMenLevel;
            Debug.Log($"[TestMod] 宗门等级已设为满级 ({maxZongMenLevel})");
        }
        
        // 6. 探索数据全解锁
        if (gameInfo.AllExploreData != null && gameInfo.AllExploreData.ExploreList != null)
        {
            foreach (var explore in gameInfo.AllExploreData.ExploreList)
            {
                explore.Unlocked = true;
                explore.AllEventNum = 999;
            }
            Debug.Log("[TestMod] 探索已全部解锁");
        }
        
        // 7. 成就数据全完成
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
        
        // 9. 创建最高品质的各职业随从各4个
        CreateMaxQualityStudents(gameInfo);
        
        // 10. 获得所有物品，数量为99
        AddAllItems(gameInfo);
        
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
                    ItemData item = new ItemData();
                    item.settingId = itemId;
                    item.onlyId = gameInfo.TheId++;
                    item.count = 999999999999999;
                    item.quality = itemSetting.Quality.ToInt32();
                    item.setting = itemSetting;
                    
                    gameInfo.ItemModel.itemIdList.Add(itemId);
                    gameInfo.ItemModel.onlyIdList.Add(item.onlyId);
                    gameInfo.ItemModel.itemDataList.Add(item);
                }
                Debug.Log($"[TestMod] 已添加 {allItems.Count} 种物品，每种 99 个");
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
        int maxRarity = (int)cfg.Rarity.red;
        
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
                gameInfo.studentData.allStudentList.Add(student);
                studentCount++;
                
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
    
    private PeopleData CreateMaxQualityStudent(StudentTalent talent, int quality, int rarity, GameInfo gameInfo)
    {
        PeopleData p = new PeopleData();
        p.onlyId = gameInfo.TheId++;
        p.name = GetStudentNameByTalent(talent);
        p.studentType = (int)StudentType.WaiMen;
        p.talent = (int)talent;
        p.studentQuality = quality;
        p.studentRarity = rarity;
        
        int maxStudentLevel = DataTable._studentUpgradeList.Count;
        p.studentLevel = maxStudentLevel;
        p.studentCurExp = 99999999;
        p.curXiuwei = 99999999;
        
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
        
        p.allSkillData = new AllSkillData();
        p.curEquipItemList = new List<ItemData> { null, null, null, null, null, null };
        
        p.gender = UnityEngine.Random.Range(0, 2);
        p.yuanSu = UnityEngine.Random.Range(1, 6);
        p.enemySettingId = 0;
        p.curPhase = 1;
        p.totalPhase = 10;
        p.xiSuiRate = 100;
        p.talentRarity = 5;
        
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
        int maxLevel = DataTable._studentUpgradeList.Count;
        int maxEquipLevel = DataTable._equipUpgradeList.Count;
        
        p.studentLevel = maxLevel;
        p.studentCurExp = 99999999;
        p.curXiuwei = 99999999;
        
        if (p.allSkillData != null)
        {
            var allSkills = DataTable.table.TbSkill.DataList;
            if (allSkills != null && allSkills.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(10, allSkills.Count); i++)
                {
                    var skillSetting = allSkills[UnityEngine.Random.Range(0, allSkills.Count)];
                    if (skillSetting != null)
                    {
                        SingleSkillData skill = new SingleSkillData();
                        skill.skillId = skillSetting.Id.ToInt32();
                        skill.skillLevel = 100;
                        skill.isEquipped = true;
                        p.allSkillData.skillList.Add(skill);
                        p.allSkillData.equippedSkillIdList.Add(skill.skillId);
                    }
                }
            }
        }
        
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
                        equipProto.curLevel = maxEquipLevel;
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
        
        Debug.Log($"[TestMod] 修武弟子已设置：等级 {p.studentLevel}，技能 {p.allSkillData.skillList.Count} 个，装备 {p.curEquipItemList.FindAll(x => x != null).Count} 件");
    }
    
    private EquipmentSetting FindBestEquipmentForSlot(int slotIndex, List<EquipmentSetting> allEquipSettings)
    {
        EquipmentSetting best = null;
        int bestRarity = 0;
        
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
                if (rarity > bestRarity)
                {
                    bestRarity = rarity;
                    best = equip;
                }
            }
        }
        
        return best;
    }
    
    private void CreateAllDanFarms(GameInfo gameInfo)
    {
        if (DataTable.table == null || DataTable.table.TbDanFarm == null) return;
        
        var allDanFarms = DataTable.table.TbDanFarm.DataList;
        if (allDanFarms == null || allDanFarms.Count == 0) return;
        
        // 第一个建筑位置
        float startX = -1975f;
        float startY = 1835f;
        
        // 每个建筑之间的间距（不重叠）
        float spacingX = 250f;
        float spacingY = -250f;
        
        // 每行4个
        int perRow = 4;
        
        int index = 0;
        int typeIndex = 0;
        
        foreach (var danFarmSetting in allDanFarms)
        {
            if (danFarmSetting == null) continue;
            
            int settingId = danFarmSetting.Id.ToInt32();
            
            // 每个类型的丹炉创建1个
            for (int i = 0; i < 1; i++)
            {
                SingleDanFarmData danFarm = new SingleDanFarmData();
                danFarm.OnlyId = (ulong)(gameInfo.TheId++);
                danFarm.SettingId = settingId;
                danFarm.IsEmpty = false;
                danFarm.Index = index;
                danFarm.DanFarmType = danFarmSetting.Type.ToInt32();
                
                // 计算位置：每种类型一行，第一个位置(-1975, 1835)
                int rowInType = i / perRow;
                int colInType = i % perRow;
                
                danFarm.LocalPos = new Vector2(
                    startX + colInType * spacingX + typeIndex * 50f,
                    startY + typeIndex * spacingY + rowInType * spacingY
                );
                
                // 状态为工作状态（不需要等待建造时间）
                danFarm.Status = 1; // Working状态
                danFarm.RemainTime = 0;
                danFarm.ProcessDanTimer = 0;
                danFarm.RebuildTotalTime = 0;
                danFarm.OpenQuanLi = false;
                danFarm.QuanLiTotalTime = 0;
                danFarm.QuanliRemainTime = 0;
                danFarm.ProcessSpeed = 0;
                // 从配置表获取产品ID
                danFarm.ProductSettingId = danFarmSetting.Param.ToInt32();
                danFarm.ProductRemainNum = 0;
                danFarm.ProductTotalNum = 0;
                danFarm.HandleStop = false;
                danFarm.NeedForeItemId = 0;
                danFarm.SingleDanPrice = 0;
                danFarm.Unlocked = true;
                danFarm.TalentType = 0;
                danFarm.CurLevel = 1;
                
                for (int j = 0; j < 4; j++)
                {
                    danFarm.ZuoZhenStudentIdList.Add(0);
                }
                
                danFarm.PosUnlockStatusList.Clear();
                for (int j = 0; j < 4; j++)
                {
                    danFarm.PosUnlockStatusList.Add(true);
                }
                
                danFarm.StudentUseCangKuDataList = new List<SingleStudentUseCangKuData>();
                danFarm.ProductItemList = new List<ItemData>();
                danFarm.UnlockedProductIdList = new List<int>();
                
                gameInfo.allDanFarmData.DanFarmList.Add(danFarm);
                index++;
            }
            typeIndex++;
        }
        
        Debug.Log($"[TestMod] 已创建 {index} 个丹炉建筑，每种1个，位置4个");
    }
#endif
}