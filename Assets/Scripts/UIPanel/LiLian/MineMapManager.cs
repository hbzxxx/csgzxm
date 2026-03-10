using System.Collections.Generic;
using UnityEngine;
using Framework.Data;

/// <summary>
/// 矿洞地图管理器
/// 负责地图生成、遮罩计算、寻路等逻辑
/// </summary>
public class MineMapManager : CommonInstance<MineMapManager>
{
    public MineMapData mapData;
    public List<PeopleData> participantList; // 参与历练的弟子列表
    public List<ItemData> collectedItems;    // 收集到的物品列表

    /// <summary>
    /// 初始化新的矿洞地图
    /// </summary>
    public void InitNewMap(List<PeopleData> pList)
    {
        participantList = pList;
        collectedItems = new List<ItemData>();
        mapData = new MineMapData();
        GenerateMap();
    }

    /// <summary>
    /// 生成地图
    /// </summary>
    public void GenerateMap()
    {
        // 1. 先将所有格子设为普通砖块
        for (int x = 0; x < MineMapData.MAP_WIDTH; x++)
        {
            for (int y = 0; y < MineMapData.MAP_HEIGHT; y++)
            {
                mapData.grids[x, y].blockType = MineBlockType.Normal;
                mapData.grids[x, y].isMasked = true;
                mapData.grids[x, y].isRevealed = false;
                mapData.grids[x, y].rewardItem = null;
                mapData.grids[x, y].hasStairEntrance = false;
            }
        }

        // 2. 在中心区域挖出初始空地（至少2个相邻空地）
        CreateInitialEmptySpaces();

        // 3. 放置角色到空地上
        PlaceCharacters();

        // 4. 随机放置特殊砖块
        PlaceSpecialBlocks();

        // 5. 计算遮罩
        UpdateMask();
    }

    /// <summary>
    /// 在中心区域创建初始空地
    /// </summary>
    void CreateInitialEmptySpaces()
    {
        mapData.emptyPositions.Clear();

        // 中心区域范围 (大约在地图中心)
        int centerX = MineMapData.MAP_WIDTH / 2;
        int centerY = MineMapData.MAP_HEIGHT / 2;

        // 创建第一个空地（中心位置）
        Vector2Int firstEmpty = new Vector2Int(centerX, centerY);
        SetGridEmpty(firstEmpty);

        // 创建剩余的空地，使用配置的数量
        int emptyCount = 1; // 已经创建了第一个空地
        List<Vector2Int> frontier = new List<Vector2Int> { firstEmpty };
        HashSet<Vector2Int> visited = new HashSet<Vector2Int> { firstEmpty };

        while (emptyCount < MineConfig.MIN_EMPTY_COUNT && frontier.Count > 0)
        {
            // 随机选择一个已存在的空地作为起点
            Vector2Int currentEmpty = frontier[RandomManager.Next(0, frontier.Count)];
            
            // 获取相邻位置
            Vector2Int[] directions = MineMapData.GetAdjacentDirections();
            List<Vector2Int> validDirections = new List<Vector2Int>();
            foreach (var dir in directions)
            {
                Vector2Int newPos = currentEmpty + dir;
                if (mapData.IsValidPosition(newPos) && !visited.Contains(newPos))
                {
                    validDirections.Add(newPos);
                }
            }

            if (validDirections.Count > 0)
            {
                // 随机选择一个有效方向创建新空地
                Vector2Int newEmpty = validDirections[RandomManager.Next(0, validDirections.Count)];
                SetGridEmpty(newEmpty);
                visited.Add(newEmpty);
                frontier.Add(newEmpty);
                emptyCount++;
            }
            else
            {
                // 如果当前位置没有有效方向，从frontier中移除
                frontier.Remove(currentEmpty);
            }
        }

        Debug.Log($"CreateInitialEmptySpaces: 创建了 {emptyCount} 个初始空地，配置要求: {MineConfig.MIN_EMPTY_COUNT}");
    }

    /// <summary>
    /// 将格子设为空地
    /// </summary>
    void SetGridEmpty(Vector2Int pos)
    {
        MineGridData grid = mapData.GetGrid(pos);
        if (grid != null)
        {
            grid.blockType = MineBlockType.Empty;
            grid.isRevealed = true;
            grid.isMasked = false;
            if (!mapData.emptyPositions.Contains(pos))
            {
                mapData.emptyPositions.Add(pos);
            }
        }
    }

    /// <summary>
    /// 放置角色到空地上
    /// </summary>
    void PlaceCharacters()
    {
        mapData.characterPositions.Clear();

        if (mapData.emptyPositions.Count >= 1)
        {
            // 第一个角色放在第一个空地
            mapData.characterPositions.Add(mapData.emptyPositions[0]);
        }

        if (participantList.Count >= 2 && mapData.emptyPositions.Count >= 2)
        {
            // 第二个角色放在第二个空地
            mapData.characterPositions.Add(mapData.emptyPositions[1]);
        }
        else if (participantList.Count >= 2 && mapData.emptyPositions.Count == 1)
        {
            // 如果只有一个空地，两个角色都在同一位置
            mapData.characterPositions.Add(mapData.emptyPositions[0]);
        }
    }

    /// <summary>
    /// 放置特殊砖块
    /// </summary>
    void PlaceSpecialBlocks()
    {
        // 获取所有可用的砖块位置（非空地）
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int x = 0; x < MineMapData.MAP_WIDTH; x++)
        {
            for (int y = 0; y < MineMapData.MAP_HEIGHT; y++)
            {
                MineGridData grid = mapData.grids[x, y];
                if (grid.blockType == MineBlockType.Normal)
                {
                    availablePositions.Add(new Vector2Int(x, y));
                }
            }
        }

        // 打乱顺序
        ShuffleList(availablePositions);

        int index = 0;

        // 放置宝藏砖块
        int treasureCount = RandomManager.Next(MineConfig.MIN_TREASURE_COUNT, MineConfig.MAX_TREASURE_COUNT + 1);
        for (int i = 0; i < treasureCount && index < availablePositions.Count; i++)
        {
            Vector2Int pos = availablePositions[index++];
            MineGridData grid = mapData.GetGrid(pos);
            grid.blockType = MineBlockType.Treasure;
            grid.rewardItem = GenerateRewardItem();
        }

        // 放置炸药砖块
        int bombCount = RandomManager.Next(MineConfig.MIN_BOMB_COUNT, MineConfig.MAX_BOMB_COUNT + 1);
        for (int i = 0; i < bombCount && index < availablePositions.Count; i++)
        {
            Vector2Int pos = availablePositions[index++];
            MineGridData grid = mapData.GetGrid(pos);
            grid.blockType = MineBlockType.Bomb;
        }

        // 放置楼梯砖块
        for (int i = 0; i < MineConfig.STAIR_COUNT && index < availablePositions.Count; i++)
        {
            Vector2Int pos = availablePositions[index++];
            MineGridData grid = mapData.GetGrid(pos);
            grid.blockType = MineBlockType.Stair;
        }
    }

    /// <summary>
    /// 生成奖励物品
    /// </summary>
    ItemData GenerateRewardItem()
    {
        // 使用历练管理器的权重配置系统
        LiLianManager.LiLianItemConfig itemConfig = GetRandomItemConfig();
        
        ItemData item = new ItemData();
        item.settingId = itemConfig.itemId;
        item.count = (ulong)RandomManager.Next(10, 50); // 挖矿中的物品数量可以独立配置
        item.setting = DataTable.FindItemSetting(item.settingId); // 设置物品配置
        
        Debug.Log($"GenerateRewardItem: 生成物品，ID: {item.settingId}, 数量: {item.count}");
        return item;
    }

    /// <summary>
    /// 根据权重随机选择物品配置（从LiLianManager复制）
    /// </summary>
    LiLianManager.LiLianItemConfig GetRandomItemConfig()
    {
        int totalWeight = 0;
        foreach (var config in LiLianManager.liLianItemConfigs)
        {
            totalWeight += config.weight;
        }

        int randomWeight = RandomManager.Next(0, totalWeight);
        int currentWeight = 0;
        
        Debug.Log($"GetRandomItemConfig: 总权重={totalWeight}, 随机权重={randomWeight}");
        
        foreach (var config in LiLianManager.liLianItemConfigs)
        {
            currentWeight += config.weight;
            Debug.Log($"GetRandomItemConfig: 检查物品ID={config.itemId}, 权重={config.weight}, 当前累计权重={currentWeight}");
            if (randomWeight < currentWeight)
            {
                Debug.Log($"GetRandomItemConfig: 选择物品ID={config.itemId}");
                return config;
            }
        }
        
        Debug.LogWarning("GetRandomItemConfig: 未找到匹配的物品，返回默认物品");
        return LiLianManager.liLianItemConfigs[0]; // 默认返回第一个
    }

    /// <summary>
    /// 打乱列表顺序
    /// </summary>
    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = RandomManager.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    /// <summary>
    /// 更新遮罩（计算可达区域）
    /// </summary>
    public void UpdateMask()
    {
        // 先将所有砖块设为遮罩状态
        for (int x = 0; x < MineMapData.MAP_WIDTH; x++)
        {
            for (int y = 0; y < MineMapData.MAP_HEIGHT; y++)
            {
                MineGridData grid = mapData.grids[x, y];
                if (grid.blockType != MineBlockType.Empty && !grid.isRevealed)
                {
                    grid.isMasked = true;
                }
                else
                {
                    grid.isMasked = false;
                }
            }
        }

        // 从所有空地出发，标记相邻的砖块为可点击（不遮罩）
        foreach (Vector2Int emptyPos in mapData.emptyPositions)
        {
            List<Vector2Int> adjacentPositions = mapData.GetAdjacentPositions(emptyPos);
            foreach (Vector2Int adjPos in adjacentPositions)
            {
                MineGridData adjGrid = mapData.GetGrid(adjPos);
                if (adjGrid != null && adjGrid.IsBrick())
                {
                    adjGrid.isMasked = false;
                }
            }
        }
    }

    /// <summary>
    /// 检查砖块是否可以被开采（与空地相邻）
    /// </summary>
    public bool CanMineBrick(Vector2Int pos)
    {
        MineGridData grid = mapData.GetGrid(pos);
        if (grid == null) return false;
        
        // 检查是否与空地相邻
        if (!IsAdjacentToEmpty(pos)) return false;
        
        return grid.CanBeMined() && mapData.remainingSteps > 0;
    }

    /// <summary>
    /// 检查位置是否与空地相邻
    /// </summary>
    public bool IsAdjacentToEmpty(Vector2Int pos)
    {
        List<Vector2Int> adjacentPositions = mapData.GetAdjacentPositions(pos);
        
        foreach (Vector2Int adjPos in adjacentPositions)
        {
            MineGridData adjGrid = mapData.GetGrid(adjPos);
            if (adjGrid != null && adjGrid.blockType == MineBlockType.Empty)
            {
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// 获取角色移动到目标砖块旁边的最佳空地位置
    /// </summary>
    public Vector2Int? GetBestEmptyPositionNearBrick(Vector2Int brickPos)
    {
        List<Vector2Int> adjacentPositions = mapData.GetAdjacentPositions(brickPos);
        Vector2Int? bestPos = null;
        int minDistance = int.MaxValue;

        // 获取主角色当前位置
        Vector2Int charPos = mapData.characterPositions.Count > 0 ? mapData.characterPositions[0] : Vector2Int.zero;

        foreach (Vector2Int adjPos in adjacentPositions)
        {
            MineGridData adjGrid = mapData.GetGrid(adjPos);
            if (adjGrid != null && adjGrid.blockType == MineBlockType.Empty)
            {
                // 计算到角色当前位置的曼哈顿距离
                int distance = Mathf.Abs(adjPos.x - charPos.x) + Mathf.Abs(adjPos.y - charPos.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestPos = adjPos;
                }
            }
        }

        return bestPos;
    }

    /// <summary>
    /// 使用BFS寻找从起点到终点的路径（限制最大距离）
    /// </summary>
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int maxDistance = -1)
    {
        return FindPath(start, end, maxDistance, false);
    }

    /// <summary>
    /// 使用BFS寻找从起点到终点的路径（限制最大距离，可选择是否允许到达楼梯）
    /// </summary>
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int maxDistance, bool allowStair)
    {
        if (start == end)
        {
            return new List<Vector2Int> { start };
        }

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();

        frontier.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = start;
        distance[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == end)
            {
                // 重建路径
                List<Vector2Int> path = new List<Vector2Int>();
                Vector2Int node = end;
                while (node != start)
                {
                    path.Add(node);
                    node = cameFrom[node];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            List<Vector2Int> neighbors = mapData.GetAdjacentPositions(current);
            foreach (Vector2Int next in neighbors)
            {
                MineGridData nextGrid = mapData.GetGrid(next);
                
                // 检查是否可以移动到此位置
                bool canMove = false;
                if (nextGrid != null)
                {
                    if (nextGrid.blockType == MineBlockType.Empty)
                    {
                        canMove = true; // 空地总是可以移动
                    }
                    else if (allowStair && next == end && nextGrid.hasStairEntrance)
                    {
                        canMove = true; // 允许到达楼梯位置
                    }
                }
                
                if (canMove && !visited.Contains(next))
                {
                    // 检查距离限制
                    int nextDistance = distance[current] + 1;
                    if (maxDistance > 0 && nextDistance > maxDistance)
                    {
                        continue; // 跳过超过最大距离的位置
                    }
                    
                    frontier.Enqueue(next);
                    visited.Add(next);
                    cameFrom[next] = current;
                    distance[next] = nextDistance;
                }
            }
        }

        // 没有找到路径
        return null;
    }

    /// <summary>
    /// 开采砖块
    /// </summary>
    public MineResult MineBrick(Vector2Int pos)
    {
        MineResult result = new MineResult();
        result.minedPositions = new List<Vector2Int>();
        result.rewards = new List<ItemData>();
        result.collectedItems = new List<ItemData>();
        result.explodedPositions = new List<Vector2Int>();
        result.positionRewards = new Dictionary<Vector2Int, ItemData>();

        MineGridData grid = mapData.GetGrid(pos);
        if (grid == null || !grid.CanBeMined())
        {
            result.success = false;
            return result;
        }

        result.success = true;
        result.blockType = grid.blockType;

        // 根据砖块类型处理
        switch (grid.blockType)
        {
            case MineBlockType.Normal:
                RevealBrick(pos, result);
                break;

            case MineBlockType.Treasure:
                RevealBrick(pos, result);
                if (grid.rewardItem != null)
                {
                    result.rewards.Add(grid.rewardItem);
                    result.positionRewards[pos] = grid.rewardItem; // 保存位置奖励
                    collectedItems.Add(grid.rewardItem);
                }
                break;

            case MineBlockType.Bomb:
                // 先爆炸炸弹本身
                RevealBrick(pos, result);
                // 更新遮罩（相当于一次开采）
                UpdateMask();
                // 触发连锁爆炸（从炸弹位置开始，剩余4次）
                TriggerBombChain(pos, result, MineConfig.BOMB_CHAIN_COUNT - 1);
                break;

            case MineBlockType.Stair:
                RevealBrick(pos, result);
                grid.hasStairEntrance = true;
                result.hasStair = true;
                break;
        }

        // 消耗步数
        mapData.remainingSteps--;

        // 更新遮罩
        UpdateMask();

        return result;
    }

    /// <summary>
    /// 揭示砖块（变为空地）
    /// </summary>
    void RevealBrick(Vector2Int pos, MineResult result)
    {
        MineGridData grid = mapData.GetGrid(pos);
        if (grid == null) return;

        grid.isRevealed = true;
        grid.blockType = MineBlockType.Empty;
        result.minedPositions.Add(pos);

        if (!mapData.emptyPositions.Contains(pos))
        {
            mapData.emptyPositions.Add(pos);
        }
    }

    /// <summary>
    /// 触发炸弹连锁（固定4次爆炸）
    /// </summary>
    void TriggerBombChain(Vector2Int lastExplodedPos, MineResult result, int remainingChains)
    {
        if (remainingChains <= 0) return;

        // 获取上一次爆炸位置的相邻砖块
        List<Vector2Int> adjacentPositions = mapData.GetAdjacentPositions(lastExplodedPos);
        List<Vector2Int> validTargets = new List<Vector2Int>();

        foreach (Vector2Int adjPos in adjacentPositions)
        {
            MineGridData adjGrid = mapData.GetGrid(adjPos);
            if (adjGrid != null && adjGrid.IsBrick())
            {
                // 所有砖块都可以被爆炸（包括宝藏和楼梯）
                validTargets.Add(adjPos);
                
                if (adjGrid.blockType == MineBlockType.Stair)
                {
                    Debug.Log($"TriggerBombChain: 楼梯砖块 {adjPos} 可以被连锁爆炸");
                }
            }
        }

        if (validTargets.Count == 0) return;

        // 随机选择一个方向炸开
        Vector2Int targetPos = validTargets[RandomManager.Next(0, validTargets.Count)];
        MineGridData targetGrid = mapData.GetGrid(targetPos);

        // 保存原始砖块类型（因为ExplodeBrick会修改为Empty）
        MineBlockType originalBlockType = targetGrid.blockType;
        
        // 爆炸目标砖块
        ExplodeBrick(targetPos, result);
        
        // 每次爆炸后更新遮罩（相当于一次开采）
        UpdateMask();

        // 检查爆炸的砖块是否是炸弹
        if (originalBlockType == MineBlockType.Bomb)
        {
            Debug.Log($"TriggerBombChain: 爆炸到炸弹砖块 {targetPos}，重新开始连锁，剩余次数：{MineConfig.BOMB_CHAIN_COUNT - 1}");
            // 如果是炸弹，从当前位置重新开始完整的连锁（5次）
            TriggerBombChain(targetPos, result, MineConfig.BOMB_CHAIN_COUNT - 1);
        }
        else
        {
            Debug.Log($"TriggerBombChain: 爆炸普通砖块 {targetPos}，继续连锁，剩余次数：{remainingChains - 1}");
            // 如果不是炸弹，继续正常连锁
            TriggerBombChain(targetPos, result, remainingChains - 1);
        }
    }

    /// <summary>
    /// 爆炸砖块（直接消失，如果是宝藏直接获得）
    /// </summary>
    void ExplodeBrick(Vector2Int pos, MineResult result)
    {
        MineGridData grid = mapData.GetGrid(pos);
        if (grid == null) return;

        // 如果是宝藏，直接获得
        if (grid.blockType == MineBlockType.Treasure && grid.rewardItem != null)
        {
            result.rewards.Add(grid.rewardItem);
            result.positionRewards[pos] = grid.rewardItem; // 保存位置奖励
            result.collectedItems.Add(grid.rewardItem);
        }

        // 如果是楼梯砖块，设置楼梯入口
        if (grid.blockType == MineBlockType.Stair)
        {
            grid.hasStairEntrance = true;
            result.hasStair = true;
            Debug.Log($"ExplodeBrick: 楼梯砖块 {pos} 被炸开，显示楼梯入口");
        }

        // 砖块直接消失变成空地
        grid.isRevealed = true;
        grid.blockType = MineBlockType.Empty;
        grid.isMasked = false;

        // 添加到爆炸位置和空地位置
        result.explodedPositions.Add(pos);
        mapData.emptyPositions.Add(pos);
    }

    /// <summary>
    /// 移动角色到新位置（第二个角色跟随第一个角色的前一个位置）
    /// </summary>
    public void MoveCharacters(Vector2Int newPosition)
    {
        if (mapData.characterPositions.Count == 0) return;

        Vector2Int oldMainPos = mapData.characterPositions[0];
        
        // 处理跟随角色（第二个角色）
        if (mapData.characterPositions.Count >= 2)
        {
            Vector2Int followPos = mapData.characterPositions[1];
            
            // 第二个角色移动到第一个角色的前一个位置
            if (IsValidPosition(oldMainPos))
            {
                mapData.characterPositions[1] = oldMainPos;
            }
            else
            {
                // 如果前一个位置不可用，找其他相邻位置
                Vector2Int? adjacentPos = FindBestAdjacentPosition(newPosition, followPos);
                if (adjacentPos.HasValue)
                {
                    mapData.characterPositions[1] = adjacentPos.Value;
                }
            }
        }
        
        // 第一个角色移动到新位置
        mapData.characterPositions[0] = newPosition;
        
        // 确保两个角色不重叠
        EnsureCharactersNotOverlapping();
    }

    /// <summary>
    /// 确保角色不重叠，如果重叠则交换位置
    /// </summary>
    void EnsureCharactersNotOverlapping()
    {
        if (mapData.characterPositions.Count < 2) return;
        
        Vector2Int mainPos = mapData.characterPositions[0];
        Vector2Int followPos = mapData.characterPositions[1];
        
        // 如果两个角色重叠，交换位置
        if (mainPos == followPos)
        {
            Debug.Log("EnsureCharactersNotOverlapping: 角色重叠，交换位置");
            
            // 找到主角色附近的空地
            List<Vector2Int> adjacentPositions = mapData.GetAdjacentPositions(mainPos);
            List<Vector2Int> validPositions = new List<Vector2Int>();
            
            foreach (Vector2Int adjPos in adjacentPositions)
            {
                if (IsValidPosition(adjPos))
                {
                    validPositions.Add(adjPos);
                }
            }
            
            if (validPositions.Count > 0)
            {
                // 随机选择一个有效位置给跟随角色
                Vector2Int newPos = validPositions[RandomManager.Next(0, validPositions.Count)];
                mapData.characterPositions[1] = newPos;
                Debug.Log($"EnsureCharactersNotOverlapping: 跟随角色移动到 {newPos}");
            }
        }
        
        // 确保两个角色距离不超过一个格子
        int distance = GetDistance(mainPos, mapData.characterPositions[1]);
        if (distance > 1)
        {
            Debug.Log($"EnsureCharactersNotOverlapping: 角色距离过远({distance})，调整跟随角色位置");
            
            // 找到主角色附近的最佳位置
            Vector2Int? bestPos = FindBestAdjacentPosition(mainPos, mapData.characterPositions[1]);
            if (bestPos.HasValue)
            {
                mapData.characterPositions[1] = bestPos.Value;
                Debug.Log($"EnsureCharactersNotOverlapping: 跟随角色调整到 {bestPos.Value}");
            }
        }
    }

    /// <summary>
    /// 检查位置是否有效（空地）
    /// </summary>
    bool IsValidPosition(Vector2Int pos)
    {
        MineGridData grid = mapData.GetGrid(pos);
        return grid != null && grid.blockType == MineBlockType.Empty;
    }

    /// <summary>
    /// 计算两个位置的曼哈顿距离
    /// </summary>
    int GetDistance(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    /// <summary>
    /// 找到目标位置附近最适合跟随角色的相邻位置
    /// </summary>
    Vector2Int? FindBestAdjacentPosition(Vector2Int targetPos, Vector2Int currentFollowPos)
    {
        List<Vector2Int> adjacentPositions = mapData.GetAdjacentPositions(targetPos);
        Vector2Int? bestPos = null;
        int minDistance = int.MaxValue;

        foreach (Vector2Int adjPos in adjacentPositions)
        {
            MineGridData grid = mapData.GetGrid(adjPos);
            // 只考虑空地位置
            if (grid != null && grid.blockType == MineBlockType.Empty)
            {
                // 选择距离当前跟随角色最近的位置
                int distance = GetDistance(adjPos, currentFollowPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestPos = adjPos;
                }
            }
        }

        return bestPos;
    }

    /// <summary>
    /// 进入下一层
    /// </summary>
    public void EnterNextFloor()
    {
        mapData.currentFloor++;
        GenerateMap();
    }

    /// <summary>
    /// 检查游戏是否结束
    /// </summary>
    public bool IsGameOver()
    {
        return mapData.remainingSteps <= 0;
    }

    /// <summary>
    /// 获取总收集物品
    /// </summary>
    public List<ItemData> GetCollectedItems()
    {
        return collectedItems;
    }
}

/// <summary>
/// 开采结果
/// </summary>
public class MineResult
{
    public bool success;
    public MineBlockType blockType;
    public List<Vector2Int> minedPositions;
    public List<ItemData> rewards;
    public List<ItemData> collectedItems;
    public List<Vector2Int> explodedPositions;
    public bool hasStair;
    
    // 新增：按位置保存奖励，用于按顺序播放
    public Dictionary<Vector2Int, ItemData> positionRewards;
}
