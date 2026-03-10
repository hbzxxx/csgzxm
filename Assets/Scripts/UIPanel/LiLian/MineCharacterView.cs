using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Framework.Data;
using Spine.Unity;

/// <summary>
/// 矿洞角色视图
/// </summary>
public class MineCharacterView : SingleStudentView
{
    public Image img_character;         // 角色图片（备用）
    public SkeletonGraphic ske;         // 角色骨骼动画
    public int characterIndex;          // 角色索引（0=主角色，1=跟随角色）
    public Vector2Int currentGridPos;   // 当前格子位置
    public float blockSize = 100f;      // 格子大小

    public override void Init(params object[] args)
    {
        base.Init(args);

        characterIndex = (int)args[0];
        peopleData = args[1] as PeopleData;

        if (args.Length > 2)
        {
            blockSize = (float)args[2];
        }

        InitCharacterSpine();
    }

    /// <summary>
    /// 初始化角色骨骼动画
    /// </summary>
    void InitCharacterSpine()
    {
        if (ske == null) return;

        // 根据性别加载不同的骨骼
        if (peopleData.gender == (int)Gender.Male)
        {
            ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(
                ConstantVal.battlePeoplePath + ConstantVal.liLianMaleSke);
        }
        else
        {
            ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(
                ConstantVal.battlePeoplePath + ConstantVal.liLianFemaleSke);
        }

        //ske.Initialize(true);
        //PlayIdleAnimation();
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    /// <summary>
    /// 设置位置（格子坐标）
    /// </summary>
    public void SetGridPosition(Vector2Int gridPos)
    {
        currentGridPos = gridPos;
        transform.localPosition = GridToLocalPosition(gridPos);
    }

    /// <summary>
    /// 格子坐标转换为本地坐标（左上角锚点）
    /// </summary>
    Vector3 GridToLocalPosition(Vector2Int gridPos)
    {
        // 格子坐标转换为UI坐标
        // 左上角锚点：Y轴向下为正，需要翻转
        float x = gridPos.x * blockSize + 75;  // 与砖块对齐的偏移
        float y = -gridPos.y * blockSize - 75; // Y轴翻转并偏移
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// 移动到目标格子位置
    /// </summary>
    public void MoveTo(Vector2Int targetPos, Action onComplete = null)
    {
        currentGridPos = targetPos;
        Vector3 targetLocalPos = GridToLocalPosition(targetPos);

        //PlayWalkAnimation();

        transform.DOKill();
        transform.DOLocalMove(targetLocalPos, MineConfig.MOVE_SPEED).OnComplete(() =>
        {
            //PlayIdleAnimation();
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 沿路径移动
    /// </summary>
    public void MoveAlongPath(List<Vector2Int> path, Action onComplete = null)
    {
        if (path == null || path.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        //PlayWalkAnimation();

        Sequence sequence = DOTween.Sequence();
        foreach (Vector2Int pos in path)
        {
            Vector3 targetLocalPos = GridToLocalPosition(pos);
            sequence.Append(transform.DOLocalMove(targetLocalPos, MineConfig.MOVE_SPEED));
        }

        sequence.OnComplete(() =>
        {
            currentGridPos = path[path.Count - 1];
            //PlayIdleAnimation();
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 播放待机动画
    /// </summary>
    public void PlayIdleAnimation()
    {
        if (ske != null && ske.AnimationState != null)
        {
            ske.AnimationState.SetAnimation(0, "jingzhi", true);
        }
    }

    /// <summary>
    /// 播放行走动画
    /// </summary>
    public void PlayWalkAnimation()
    {
        if (ske != null && ske.AnimationState != null)
        {
            ske.AnimationState.SetAnimation(0, "feixing", true);
        }
    }

    /// <summary>
    /// 播放挖掘动画
    /// </summary>
    public void PlayMineAnimation(Action onComplete = null)
    {
        //if (ske != null && ske.AnimationState != null)
        //{
        //    // 使用现有的动画，如果没有专门的挖掘动画可以用攻击动画代替
        //    ske.AnimationState.SetAnimation(0, "jingzhi", false);
        //}

        // 延迟完成回调
        DOVirtual.DelayedCall(MineConfig.MINE_DURATION, () =>
        {
            //PlayIdleAnimation();
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 设置朝向
    /// </summary>
    public void SetFacing(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public override void Clear()
    {
        base.Clear();
        transform.DOKill();
        peopleData = null;
    }
}
