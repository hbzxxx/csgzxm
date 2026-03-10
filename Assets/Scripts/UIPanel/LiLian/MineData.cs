using System.Collections.Generic;
using UnityEngine;
using Framework.Data;

/// <summary>
/// 矿洞砖块类型
/// </summary>
public enum MineBlockType
{
    Empty = 0,      // 空地
    Normal = 1,     // 普通砖块
    Treasure = 2,   // 宝藏砖块
    Bomb = 3,       // 炸药砖块
    Stair = 4,      // 楼梯砖块
}

/// <summary>
/// 单个格子数据
/// </summary>
public class MineGridData
{
    public int x;                       // 列坐标 (0-5)
    public int y;                       // 行坐标 (0-6)
    public MineBlockType blockType;     // 砖块类型
    public bool isMasked;               // 是否被遮罩（不可达）
    public bool isRevealed;             // 是否已被开采/显露
    public ItemData rewardItem;         // 奖励物品（宝藏砖块用）
    public bool hasStairEntrance;       // 是否有楼梯入口（楼梯被开采后显示）

    public MineGridData(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.blockType = MineBlockType.Normal;
        this.isMasked = true;
        this.isRevealed = false;
        this.rewardItem = null;
        this.hasStairEntrance = false;
    }

    /// <summary>
    /// 是否是砖块（非空地）
    /// </summary>
    public bool IsBrick()
    {
        return blockType != MineBlockType.Empty && !isRevealed;
    }

    /// <summary>
    /// 是否可以被点击开采
    /// </summary>
    public bool CanBeMined()
    {
        return IsBrick() && !isMasked;
    }
}

/// <summary>
/// 矿洞地图数据
/// </summary>
public class MineMapData
{
    public const int MAP_WIDTH = 6;     // 地图宽度（列数）
    public const int MAP_HEIGHT = 7;    // 地图高度（行数）

    public MineGridData[,] grids;       // 格子数据
    public List<Vector2Int> emptyPositions;  // 空地位置列表
    public List<Vector2Int> characterPositions; // 角色位置列表
    public int remainingSteps;          // 剩余步数
    public int currentFloor;            // 当前层数

    public MineMapData()
    {
        grids = new MineGridData[MAP_WIDTH, MAP_HEIGHT];
        emptyPositions = new List<Vector2Int>();
        characterPositions = new List<Vector2Int>();
        remainingSteps = MineConfig.INITIAL_STEPS;
        currentFloor = 1;

        // 初始化所有格子
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                grids[x, y] = new MineGridData(x, y);
            }
        }
    }

    /// <summary>
    /// 获取指定位置的格子
    /// </summary>
    public MineGridData GetGrid(int x, int y)
    {
        if (x < 0 || x >= MAP_WIDTH || y < 0 || y >= MAP_HEIGHT)
            return null;
        return grids[x, y];
    }

    /// <summary>
    /// 获取指定位置的格子
    /// </summary>
    public MineGridData GetGrid(Vector2Int pos)
    {
        return GetGrid(pos.x, pos.y);
    }

    /// <summary>
    /// 检查坐标是否有效
    /// </summary>
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < MAP_WIDTH && y >= 0 && y < MAP_HEIGHT;
    }

    /// <summary>
    /// 检查坐标是否有效
    /// </summary>
    public bool IsValidPosition(Vector2Int pos)
    {
        return IsValidPosition(pos.x, pos.y);
    }

    /// <summary>
    /// 获取相邻的四个方向
    /// </summary>
    public static Vector2Int[] GetAdjacentDirections()
    {
        return new Vector2Int[]
        {
            new Vector2Int(0, 1),   // 上
            new Vector2Int(0, -1),  // 下
            new Vector2Int(-1, 0),  // 左
            new Vector2Int(1, 0)    // 右
        };
    }

    /// <summary>
    /// 获取相邻格子位置列表
    /// </summary>
    public List<Vector2Int> GetAdjacentPositions(int x, int y)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        Vector2Int[] directions = GetAdjacentDirections();
        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            if (IsValidPosition(nx, ny))
            {
                result.Add(new Vector2Int(nx, ny));
            }
        }
        return result;
    }

    /// <summary>
    /// 获取相邻格子位置列表
    /// </summary>
    public List<Vector2Int> GetAdjacentPositions(Vector2Int pos)
    {
        return GetAdjacentPositions(pos.x, pos.y);
    }
}

/// <summary>
/// 矿洞配置
/// </summary>
public static class MineConfig
{
    public const int INITIAL_STEPS = 10;            // 初始步数
    public const int MIN_TREASURE_COUNT = 15;       // 最少宝藏砖块数量
    public const int MAX_TREASURE_COUNT = 20;       // 最多宝藏砖块数量
    public const int MIN_BOMB_COUNT = 9;            // 最少炸药砖块数量
    public const int MAX_BOMB_COUNT = 10;            // 最多炸药砖块数量
    public const int STAIR_COUNT = 1;               // 楼梯砖块数量
    public const int BOMB_CHAIN_COUNT = 5;          // 炸药连锁次数
    public const int MIN_EMPTY_COUNT = 2;           // 最少初始空地数量
    public const float MOVE_SPEED = 0.15f;          // 每格移动时长（秒/格）
    public const float MINE_DURATION = 0.5f;        // 开采动画时长（Sprite序列动画时长）
    public const float BLOCK_ANIMATION_DURATION = 0.5f; // 砖块碎裂动画时长（Sprite序列）
    public const float EXPLOSION_DELAY = 0.3f;      // 爆炸延迟时间
    public const float BOMB_CHAIN_INTERVAL = 0.3f;  // 连锁爆炸间隔时间（秒）
    public const int STAIR_MAX_DISTANCE = 300;       // 楼梯最大交互距离
}
