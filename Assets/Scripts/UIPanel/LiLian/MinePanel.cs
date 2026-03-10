using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Framework.Data;
using System;

/// <summary>
/// 矿洞挖掘面板
/// 弟子历练新玩法主面板
/// </summary>
public class MinePanel : PanelBase
{
    #region UI引用
    public Transform trans_mapGrid;         // 地图格子父节点
    public Transform trans_characterParent; // 角色父节点
    public Text txt_steps;                  // 剩余步数文本
    public Text txt_floor;                  // 当前层数文本
    public Button btn_close;                // 关闭按钮
    public Transform trans_rewardFlyTarget; // 奖励飞行目标位置
    public Image img_background;            // 背景图
    #endregion

    #region 数据
    public List<PeopleData> participantList;                // 参与的弟子列表
    public MineBlockView[,] blockViews;                     // 格子视图数组
    public List<MineCharacterView> characterViews;          // 角色视图列表
    public float blockSize = 100f;                          // 格子大小
    public bool isProcessing = false;                       // 是否正在处理中（防止重复点击）
    public int liLianId;                                    // 历练ID

    // 完全对齐 MiniGameUI 的变量
    private bool _isAnimationPlaying = false;               // 全局动画锁
    #endregion

    public override void Init(params object[] args)
    {
        base.Init(args);

        participantList = args[0] as List<PeopleData>;
        if (args.Length > 1)
        {
            liLianId = (int)args[1];
        }

        blockViews = new MineBlockView[MineMapData.MAP_WIDTH, MineMapData.MAP_HEIGHT];
        characterViews = new List<MineCharacterView>();

        addBtnListener(btn_close, OnClickClose);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        // 屏蔽其他提示弹窗
        PanelManager.Instance.blockHintPanel = true;

        // 初始化地图
        MineMapManager.Instance.InitNewMap(participantList);

        // 创建地图视图
        CreateMapView();

        // 创建角色视图
        CreateCharacterViews();

        // 刷新UI
        RefreshUI();

        // 重置动画锁（对齐 MiniGameUI）
        _isAnimationPlaying = false;
    }

    /// <summary>
    /// 创建地图视图
    /// </summary>
    void CreateMapView()
    {
        // 清理旧的格子
        ClearMapView();

        if (trans_mapGrid == null)
        {
            Debug.LogError("MinePanel: trans_mapGrid 为空，请在预制体中绑定地图格子父节点");
            return;
        }

        MineMapData mapData = MineMapManager.Instance.mapData;
        Debug.Log($"MinePanel: 开始创建地图视图，地图大小 {MineMapData.MAP_WIDTH}x{MineMapData.MAP_HEIGHT}，父节点: {trans_mapGrid.name}");

        for (int x = 0; x < MineMapData.MAP_WIDTH; x++)
        {
            for (int y = 0; y < MineMapData.MAP_HEIGHT; y++)
            {
                MineBlockView blockView = AddSingle<MineBlockView>(trans_mapGrid, x, y, this, (Action<MineBlockView>)OnBlockClicked);
                if (blockView == null)
                {
                    Debug.LogError($"MineBlockView 创建失败，请检查预制体是否存在: MineBlockView");
                    continue;
                }
                blockView.transform.localPosition = blockView.GetTopLeftPosition();
                blockView.SetGridData(mapData.GetGrid(x, y));
                blockViews[x, y] = blockView;
            }
        }
        Debug.Log($"MinePanel: 地图视图创建完成，共创建 {trans_mapGrid.childCount} 个格子");
    }

    /// <summary>
    /// 清理地图视图
    /// </summary>
    void ClearMapView()
    {
        if (trans_mapGrid != null)
        {
            PanelManager.Instance.CloseAllSingle(trans_mapGrid);
        }

        if (blockViews != null)
        {
            for (int x = 0; x < MineMapData.MAP_WIDTH; x++)
            {
                for (int y = 0; y < MineMapData.MAP_HEIGHT; y++)
                {
                    blockViews[x, y] = null;
                }
            }
        }
    }

    /// <summary>
    /// 创建角色视图
    /// </summary>
    void CreateCharacterViews()
    {
        // 清理旧的角色
        ClearCharacterViews();

        MineMapData mapData = MineMapManager.Instance.mapData;

        for (int i = 0; i < participantList.Count && i < mapData.characterPositions.Count; i++)
        {
            MineCharacterView charView = AddSingle<MineCharacterView>(trans_characterParent, i, participantList[i], blockSize);
            charView.SetGridPosition(mapData.characterPositions[i]);
            characterViews.Add(charView);
        }
    }

    /// <summary>
    /// 清理角色视图
    /// </summary>
    void ClearCharacterViews()
    {
        if (trans_characterParent != null)
        {
            PanelManager.Instance.CloseAllSingle(trans_characterParent);
        }
        if (characterViews != null)
        {
            characterViews.Clear();
        }
    }

    /// <summary>
    /// 刷新UI显示
    /// </summary>
    void RefreshUI()
    {
        MineMapData mapData = MineMapManager.Instance.mapData;

        // 刷新步数
        if (txt_steps != null)
        {
            txt_steps.text = mapData.remainingSteps.ToString();
        }

        // 刷新层数
        if (txt_floor != null)
        {
            txt_floor.text = "第" + mapData.currentFloor + "层";
        }

        // 刷新所有格子
        RefreshAllBlocks();
    }

    /// <summary>
    /// 刷新所有格子显示
    /// </summary>
    void RefreshAllBlocks()
    {
        MineMapData mapData = MineMapManager.Instance.mapData;

        for (int x = 0; x < MineMapData.MAP_WIDTH; x++)
        {
            for (int y = 0; y < MineMapData.MAP_HEIGHT; y++)
            {
                if (blockViews[x, y] != null)
                {
                    blockViews[x, y].SetGridData(mapData.GetGrid(x, y));
                }
            }
        }
    }

    /// <summary>
    /// 格子点击事件（完全对齐 MiniGameUI 的锁逻辑）
    /// </summary>
    void OnBlockClicked(MineBlockView blockView)
    {
        // 完全对齐 MiniGameUI：动画播放时直接拦截所有点击
        if (_isAnimationPlaying) return;
        if (isProcessing) return;
        if (blockView == null || blockView.gridData == null) return;

        Vector2Int clickPos = new Vector2Int(blockView.gridX, blockView.gridY);

        // 检查是否是楼梯入口
        if (blockView.gridData.hasStairEntrance)
        {
            OnStairEntranceClicked();
            return;
        }

        // 检查是否可以开采
        if (!MineMapManager.Instance.CanMineBrick(clickPos))
        {
            return;
        }

        // 开始处理
        isProcessing = true;

        // 获取砖块旁边的空地位置
        Vector2Int? targetPos = MineMapManager.Instance.GetBestEmptyPositionNearBrick(clickPos);
        if (targetPos == null)
        {
            isProcessing = false;
            return;
        }

        // 获取移动路径
        Vector2Int currentPos = MineMapManager.Instance.mapData.characterPositions[0];
        List<Vector2Int> path = MineMapManager.Instance.FindPath(currentPos, targetPos.Value);

        if (path == null || path.Count == 0)
        {
            isProcessing = false;
            return;
        }

        // 执行移动和开采（调用 MiniGameUI 风格的平滑移动）
        StartCoroutine(MoveAndMineCoroutine(path, clickPos, targetPos.Value));
    }

    /// <summary>
    /// <param name="path">完整路径 [起点, 点1, 点2, ..., 终点]</param>
    /// <returns>移动协程</returns>
    private IEnumerator SmoothMoveAlongPath(List<Vector2Int> path)
    {
        // 1. 完全对齐 MiniGameUI：移动动画开始，加全局锁
        _isAnimationPlaying = true;

        if (characterViews.Count == 0 || path.Count <= 1)
        {
            _isAnimationPlaying = false;
            yield break;
        }

        // 2. 强制角色1为领头者，角色2为跟随者
        MineCharacterView leaderChar = characterViews[0];
        MineCharacterView followerChar = characterViews.Count > 1 ? characterViews[1] : null;

        int leaderIndex = GetLinearIndexFromGrid(MineMapManager.Instance.mapData.characterPositions[0]);
        int followerIndex = followerChar != null ? GetLinearIndexFromGrid(MineMapManager.Instance.mapData.characterPositions[1]) : -1;

        // 3. 主角色实际移动路径
        List<Vector2Int> mainPath = new List<Vector2Int>();
        for (int i = 1; i < path.Count; i++)
        {
            mainPath.Add(path[i]);
        }

        if (mainPath.Count == 0)
        {
            _isAnimationPlaying = false;
            yield break;
        }

        // 4. 逐帧移动
        for (int i = 0; i < mainPath.Count; i++)
        {
            Vector2Int nextGridPos = mainPath[i];
            int nextLinearIndex = GetLinearIndexFromGrid(nextGridPos);
            int oldLeaderIndex = leaderIndex;
            leaderIndex = nextLinearIndex;

            // 4.1 获取格子的世界位置
            Vector3 leaderEnd = GetBlockWorldPosition(nextGridPos);
            Vector3 followerEnd = GetBlockWorldPosition(GetGridFromLinearIndex(oldLeaderIndex));

            // 4.2 处理朝向
            int oldLeaderCol = GetColumnFromLinearIndex(oldLeaderIndex);
            int newLeaderCol = GetColumnFromLinearIndex(nextLinearIndex);

            bool isFacingRight = newLeaderCol < oldLeaderCol;
            leaderChar.SetFacing(isFacingRight);
            followerChar?.SetFacing(isFacingRight);

            // 4.3 平滑插值移动
            float duration = 0.3f;
            float elapsed = 0f;

            Vector3 leaderStart = leaderChar.transform.position;
            Vector3 followerStart = followerChar != null ? followerChar.transform.position : Vector3.zero;

            // 逐帧插值
            while (elapsed < duration)
            {
                leaderChar.transform.position = Vector3.Lerp(leaderStart, leaderEnd, elapsed / duration);
                if (followerChar != null)
                {
                    followerChar.transform.position = Vector3.Lerp(followerStart, followerEnd, elapsed / duration);
                }

                elapsed += Time.deltaTime;
                yield return null; // 等待下一帧
            }

            // 4.4 强制精准到位（对齐 MiniGameUI）
            leaderChar.transform.position = leaderEnd;
            if (followerChar != null)
            {
                followerChar.transform.position = followerEnd;
            }

            // 4.5 强制更新角色索引（完全对齐 MiniGameUI 的索引更新逻辑）
            MineMapManager.Instance.mapData.characterPositions[0] = nextGridPos;
            if (followerChar != null)
            {
                MineMapManager.Instance.mapData.characterPositions[1] = GetGridFromLinearIndex(oldLeaderIndex);
                followerIndex = oldLeaderIndex;
            }
        }

        // 5. 移动完成解锁（对齐 MiniGameUI）
        _isAnimationPlaying = false;
    }

    /// <summary>
    /// 获取格子的世界位置
    /// </summary>
    private Vector3 GetBlockWorldPosition(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= MineMapData.MAP_WIDTH ||
            gridPos.y < 0 || gridPos.y >= MineMapData.MAP_HEIGHT)
        {
            return Vector3.zero;
        }

        MineBlockView blockView = blockViews[gridPos.x, gridPos.y];
        return blockView != null ? blockView.transform.position : Vector3.zero;
    }

    /// <summary>
    /// 网格坐标转线性索引
    /// </summary>
    private int GetLinearIndexFromGrid(Vector2Int gridPos)
    {
        return gridPos.y * MineMapData.MAP_WIDTH + gridPos.x;
    }

    /// <summary>
    /// 线性索引转网格坐标
    /// </summary>
    private Vector2Int GetGridFromLinearIndex(int linearIndex)
    {
        int y = linearIndex / MineMapData.MAP_WIDTH;
        int x = linearIndex % MineMapData.MAP_WIDTH;
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 获取列号
    /// </summary>
    private int GetColumnFromLinearIndex(int linearIndex)
    {
        return linearIndex % MineMapData.MAP_WIDTH;
    }

    // ========== 重构移动和开采逻辑 ==========
    private IEnumerator MoveAndMineCoroutine(List<Vector2Int> path, Vector2Int minePos, Vector2Int targetEmptyPos)
    {
        // 执行 MiniGameUI 风格的平滑移动
        yield return StartCoroutine(SmoothMoveAlongPath(path));

        // 移动完成后执行开采
        PerformMine(minePos);
    }

    /// <summary>
    /// 同步移动角色到楼梯
    /// </summary>
    void MoveCharactersToStair(List<Vector2Int> path, Vector2Int stairPos)
    {
        if (characterViews.Count == 0 || path.Count == 0 || _isAnimationPlaying)
        {
            isProcessing = false;
            return;
        }

        StartCoroutine(MoveToStairCoroutine(path, stairPos));
    }

    private IEnumerator MoveToStairCoroutine(List<Vector2Int> path, Vector2Int stairPos)
    {
        // 执行 MiniGameUI 风格的平滑移动
        yield return StartCoroutine(SmoothMoveAlongPath(path));

        // 等待一小段时间
        yield return new WaitForSeconds(0.2f);

        EnterNextFloor();
    }

    /// <summary>
    /// 执行开采
    /// </summary>
    void PerformMine(Vector2Int minePos)
    {
        // 播放角色挖掘动画
        if (characterViews.Count > 0)
        {
            characterViews[0].PlayMineAnimation();
        }

        // 执行开采逻辑
        MineResult result = MineMapManager.Instance.MineBrick(minePos);

        if (!result.success)
        {
            isProcessing = false;
            _isAnimationPlaying = false;
            return;
        }

        // 播放开采动画（按顺序依次播放）
        PlayAnimationsSequentially(result, minePos);
    }

    /// <summary>
    /// 按顺序播放动画（回调驱动，每个动画完成后才播放下一个）
    /// </summary>
    void PlayAnimationsSequentially(MineResult result, Vector2Int minePos)
    {
        // 使用 result.blockType 作为原始砖块类型
        MineBlockType originalBlockType = result.blockType;

        // 创建动画序列：每个动画接受 onComplete 回调，完成后通知播放下一个
        List<System.Action<System.Action>> animationSequence = new List<System.Action<System.Action>>();

        // 添加开采位置的动画
        foreach (Vector2Int pos in result.minedPositions)
        {
            MineBlockView blockView = blockViews[pos.x, pos.y];
            if (blockView != null)
            {
                // 第一个位置使用原始砖块类型
                MineBlockType animBlockType = (pos == minePos) ? originalBlockType : MineBlockType.Normal;

                animationSequence.Add((onComplete) =>
                {
                    blockView.PlayMineAnimation(animBlockType, () =>
                    {
                        // 动画完成后再刷新视图（避免动画前隐藏img_block导致Animator重置）
                        blockView.RefreshView();
                        onComplete?.Invoke();
                    });
                });
            }
        }

        // 添加爆炸位置的动画（按爆炸顺序）
        foreach (Vector2Int pos in result.explodedPositions)
        {
            MineBlockView blockView = blockViews[pos.x, pos.y];
            if (blockView != null)
            {
                animationSequence.Add((onComplete) =>
                {
                    blockView.PlayMineAnimation(MineBlockType.Bomb, () =>
                    {
                        // 动画完成后再刷新视图（避免动画前隐藏img_block导致Animator重置）
                        blockView.RefreshView();
                        onComplete?.Invoke();
                    });
                });
            }
        }

        // 如果没有动画，直接完成
        if (animationSequence.Count == 0)
        {
            OnMineAnimationComplete(result);
            return;
        }

        // 按顺序播放动画，每个动画完成后播放下一个
        PlayAnimationSequence(animationSequence, 0, result);
    }

    /// <summary>
    /// 递归播放动画序列（回调驱动，当前动画完成后才播放下一个）
    /// </summary>
    void PlayAnimationSequence(List<System.Action<System.Action>> animations, int index, MineResult result)
    {
        if (index >= animations.Count)
        {
            // 所有动画播放完成
            OnMineAnimationComplete(result);
            return;
        }

        // 播放当前动画，动画完成后等待间隔再播放下一个
        animations[index](() =>
        {
            // 如果不是最后一个动画，添加连锁爆炸间隔
            if (index < animations.Count - 1)
            {
                DOVirtual.DelayedCall(MineConfig.BOMB_CHAIN_INTERVAL, () =>
                {
                    PlayAnimationSequence(animations, index + 1, result);
                });
            }
            else
            {
                // 最后一个动画直接完成
                PlayAnimationSequence(animations, index + 1, result);
            }
        });
    }

    /// <summary>
    /// 开采动画完成
    /// </summary>
    void OnMineAnimationComplete(MineResult result)
    {
        // 按顺序播放奖励动画（先开采后爆炸）
        PlayRewardsSequentially(result);

        // 刷新UI
        RefreshUI();

        // 检查游戏是否结束
        if (MineMapManager.Instance.IsGameOver())
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                OnGameOver();
            });
        }

        isProcessing = false;
        _isAnimationPlaying = false; // 解锁（对齐 MiniGameUI）
    }

    /// <summary>
    /// 按顺序播放奖励动画（先开采后爆炸）
    /// </summary>
    void PlayRewardsSequentially(MineResult result)
    {
        if (result == null) return;

        // 创建奖励播放序列：先开采位置，后爆炸位置
        List<Vector2Int> rewardSequence = new List<Vector2Int>();

        // 添加开采位置的奖励
        foreach (Vector2Int pos in result.minedPositions)
        {
            if (result.positionRewards != null && result.positionRewards.ContainsKey(pos))
            {
                rewardSequence.Add(pos);
            }
        }

        // 添加爆炸位置的奖励（按爆炸顺序）
        foreach (Vector2Int pos in result.explodedPositions)
        {
            if (result.positionRewards != null && result.positionRewards.ContainsKey(pos))
            {
                rewardSequence.Add(pos);
            }
        }

        // 按顺序播放奖励动画
        PlayRewardSequence(rewardSequence, result.positionRewards, 0);
    }

    /// <summary>
    /// 播放奖励序列
    /// </summary>
    void PlayRewardSequence(List<Vector2Int> positions, Dictionary<Vector2Int, ItemData> positionRewards, int index)
    {
        if (index >= positions.Count) return;

        Vector2Int pos = positions[index];
        if (positionRewards != null && positionRewards.ContainsKey(pos))
        {
            ItemData reward = positionRewards[pos];
            PlayRewardAnimation(reward);

            // 延迟播放下一个奖励
            DOVirtual.DelayedCall(0.3f, () =>
            {
                PlayRewardSequence(positions, positionRewards, index + 1);
            });
        }
        else
        {
            // 没有奖励，直接播放下一个
            PlayRewardSequence(positions, positionRewards, index + 1);
        }
    }

    /// <summary>
    /// 播放奖励动画
    /// </summary>
    void PlayRewardAnimation(ItemData reward)
    {
        if (reward == null) return;

        // 使用现有的飞行动画
        if (trans_rewardFlyTarget != null)
        {
            // GetItemFlyUpAnimView.Init 需要 (Vector3 pos, ItemData itemData) 参数
            Vector3 startPos = trans_rewardFlyTarget.position;
            GetItemFlyUpAnimView view = AddSingle<GetItemFlyUpAnimView>(trans_rewardFlyTarget, startPos, reward);
        }

        // 显示获得提示
        // string itemName = DataTable.FindItemSetting(reward.settingId)?.Name ?? "物品";
        // PanelManager.Instance.OpenFloatWindow("获得 " + itemName + " x" + reward.count);
    }

    /// <summary>
    /// 楼梯入口点击（对齐 MiniGameUI 的锁逻辑）
    /// </summary>
    void OnStairEntranceClicked()
    {
        // 完全对齐 MiniGameUI：动画中禁止操作
        if (_isAnimationPlaying) return;

        // 获取楼梯位置
        Vector2Int stairPos = Vector2Int.zero;
        for (int x = 0; x < MineMapData.MAP_WIDTH; x++)
        {
            for (int y = 0; y < MineMapData.MAP_HEIGHT; y++)
            {
                MineGridData grid = MineMapManager.Instance.mapData.GetGrid(x, y);
                if (grid != null && grid.hasStairEntrance)
                {
                    stairPos = new Vector2Int(x, y);
                    break;
                }
            }
            if (stairPos != Vector2Int.zero) break;
        }

        // 获取角色当前位置
        Vector2Int currentPos = MineMapManager.Instance.mapData.characterPositions[0];

        // 如果角色已经在楼梯位置，直接进入下一层
        if (currentPos == stairPos)
        {
            EnterNextFloor();
            return;
        }

        // 检查角色是否在楼梯附近
        int distance = Mathf.Abs(currentPos.x - stairPos.x) + Mathf.Abs(currentPos.y - stairPos.y);
        if (distance > MineConfig.STAIR_MAX_DISTANCE)
        {
            PanelManager.Instance.OpenFloatWindow($"距离楼梯太远，需要在{MineConfig.STAIR_MAX_DISTANCE}格范围内才能使用楼梯");
            return;
        }

        // 否则移动到楼梯位置（允许到达楼梯）
        List<Vector2Int> path = MineMapManager.Instance.FindPath(currentPos, stairPos, MineConfig.STAIR_MAX_DISTANCE, true);
        if (path != null && path.Count > 0)
        {
            isProcessing = true;

            // 同步移动两个角色到楼梯（使用 MiniGameUI 风格的平滑移动）
            MoveCharactersToStair(path, stairPos);
        }
        else
        {
            PanelManager.Instance.OpenFloatWindow("无法到达楼梯位置");
            isProcessing = false;
        }
    }

    /// <summary>
    /// 进入下一层
    /// </summary>
    void EnterNextFloor()
    {
        // 进入下一层
        MineMapManager.Instance.EnterNextFloor();

        // 重新创建地图和角色
        CreateMapView();
        CreateCharacterViews();
        RefreshUI();

        PanelManager.Instance.OpenFloatWindow("进入第" + MineMapManager.Instance.mapData.currentFloor + "层");
        isProcessing = false;
        _isAnimationPlaying = false; // 解锁（对齐 MiniGameUI）
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    void OnGameOver()
    {
        // 重置处理状态（对齐 MiniGameUI）
        isProcessing = false;
        _isAnimationPlaying = false;

        // 获取收集的物品
        List<ItemData> collectedItems = MineMapManager.Instance.GetCollectedItems();

        // 记录历练完成
        LiLianManager.Instance.curParticipatedPList = participantList;
        LiLianManager.Instance.receivedItemList = collectedItems;

        // 计算总物品
        ItemData totalItem = new ItemData();
        totalItem.settingId = (int)ItemIdType.LingShi;
        totalItem.count = 0;
        foreach (ItemData item in collectedItems)
        {
            if (item.settingId == (int)ItemIdType.LingShi)
            {
                totalItem.count += item.count;
            }
        }
        LiLianManager.Instance.totalItemData = totalItem;

        // 弹出结算确认对话框
        ShowGameOverConfirmDialog();
    }

    /// <summary>
    /// 显示游戏结束确认对话框
    /// </summary>
    void ShowGameOverConfirmDialog()
    {
        // 计算获得的灵石数量
        List<ItemData> collectedItems = MineMapManager.Instance.GetCollectedItems();
        int lingShiCount = 0;
        foreach (ItemData item in collectedItems)
        {
            if (item.settingId == (int)ItemIdType.LingShi)
            {
                lingShiCount += (int)item.count;
            }
        }

        // 弹出确认对话框
        HintData hintData = new HintData();
        hintData.content = "挖矿历练结束！\n共获得 " + lingShiCount + " 灵石\n是否确认结算？";
        hintData.str_okBtn = "确认";
        hintData.str_cancelBtn = "取消";
        hintData.okCallBack = () =>
        {
            // 确认后关闭挖矿面板并结算
            PanelManager.Instance.ClosePanel(this);
            OpenLiLianJieSuanPanel();
        };
        hintData.cancelCallBack = () =>
        {
            // 取消后直接关闭挖矿面板，返回历练面板
            PanelManager.Instance.ClosePanel(this);
        };

        CommonHintPanel commonHintPanel = PanelManager.Instance.OpenPanel<CommonHintPanel>(PanelManager.Instance.trans_layer2, hintData);
        commonHintPanel.btn_cancel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 打开历练结算面板
    /// </summary>
    void OpenLiLianJieSuanPanel()
    {
        // 记录任务
        TaskManager.Instance.GetDailyAchievement(TaskType.LiLian, "1");
        TaskManager.Instance.GetAchievement(AchievementType.LiLian, "1");

        // 发放奖励
        List<ItemData> collectedItems = MineMapManager.Instance.GetCollectedItems();
        foreach (ItemData item in collectedItems)
        {
            ItemManager.Instance.GetItemWithTongZhiPanel(item.settingId, item.count);
        }

        // 尝试使用旧玩法的结算面板
        UseOldSettlementPanel();
    }

    /// <summary>
    /// 使用旧玩法的结算面板
    /// </summary>
    void UseOldSettlementPanel()
    {
        // 尝试获取历练面板
        LiLianPanel liLianPanel = PanelManager.Instance.GetPanel<LiLianPanel>();
        if (liLianPanel != null)
        {
            // 如果历练面板存在，调用其结算方法
            // 由于JieSuan是私有的，我们需要通过反射或者其他方式调用
            // 或者创建一个公共的结算方法
            try
            {
                // 使用反射调用私有的JieSuan方法
                var method = typeof(LiLianPanel).GetMethod("JieSuan", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(liLianPanel, null);
                }
                else
                {
                    // 如果反射失败，显示简单结算
                    ShowSimpleSettlement();
                }
            }
            catch
            {
                // 如果反射失败，显示简单结算
                ShowSimpleSettlement();
            }
        }
        else
        {
            // 如果历练面板不存在，显示简单结算
            ShowSimpleSettlement();
        }
    }

    /// <summary>
    /// 显示简单结算
    /// </summary>
    void ShowSimpleSettlement()
    {
        List<ItemData> collectedItems = MineMapManager.Instance.GetCollectedItems();
        int lingShiCount = 0;
        foreach (ItemData item in collectedItems)
        {
            if (item.settingId == (int)ItemIdType.LingShi)
            {
                lingShiCount += (int)item.count;
            }
        }

        PanelManager.Instance.OpenFloatWindow("挖矿历练结束！共获得 " + lingShiCount + " 灵石");
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    void OnClickClose()
    {
        // 确认是否退出
        //HintData hintData = new HintData();
        //hintData.content = "确定要退出历练吗？当前进度将不会保存。";
        //hintData.okCallBack = () =>
        //{
        //    PanelManager.Instance.ClosePanel(this);
        //};
        //hintData.cancelCallBack = () => { };
        //PanelManager.Instance.OpenPanel<CommonHintPanel>(PanelManager.Instance.trans_layer2, hintData);
    }

    public override void Clear()
    {
        base.Clear();
        ClearMapView();
        ClearCharacterViews();
        isProcessing = false;
        _isAnimationPlaying = false; // 重置动画锁（对齐 MiniGameUI）

        // 恢复提示弹窗
        PanelManager.Instance.blockHintPanel = false;
    }
}