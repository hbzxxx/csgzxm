using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 矿洞单个格子视图
/// </summary>
public class MineBlockView : SingleViewBase
{
    public Image img_block;         // 砖块图片
    public Image img_mask;          // 遮罩图片
    public Image img_highlight;     // 高亮框
    public Image img_treasure;      // 宝藏图标（宝藏砖块显示）
    public Image img_bomb;          // 炸弹图标（炸药砖块显示）
    public Image img_stair;         // 楼梯图标（楼梯砖块显示）
    public Image img_stairEntrance; // 楼梯入口（开采后显示）
    public Image img_bombEffect;     // 炸弹爆炸特效
    public Button btn_click;        // 点击按钮
    
    // 砖块碎裂动画Sprite序列
    public Sprite[] blockBreakSprites; // 砖块碎裂动画帧序列（1.png -> 5.png）
    public Sprite originalBlockSprite;  // 初始砖块Sprite（石砖.png）

    public int gridX;               // 格子X坐标
    public int gridY;               // 格子Y坐标
    public MineGridData gridData;   // 格子数据
    public MinePanel parentPanel;   // 父面板引用

    public Action<MineBlockView> onClickCallback; // 点击回调

    public override void Init(params object[] args)
    {
        base.Init(args);

        gridX = (int)args[0];
        gridY = (int)args[1];
        parentPanel = args[2] as MinePanel;

        if (args.Length > 3)
        {
            onClickCallback = args[3] as Action<MineBlockView>;
        }

        if (btn_click != null)
        {
            addBtnListener(btn_click, OnClick);
        }
        
        // 保存初始砖块Sprite
        if (img_block != null && img_block.sprite != null)
        {
            originalBlockSprite = img_block.sprite;
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshView();
        
        // 重置砖块到初始状态
        ResetBlockSprite();
    }

    /// <summary>
    /// 重置砖块到初始Sprite
    /// </summary>
    void ResetBlockSprite()
    {
        if (img_block != null && originalBlockSprite != null)
        {
            img_block.sprite = originalBlockSprite;
        }
    }

    /// <summary>
    /// 播放砖块碎裂Sprite序列动画
    /// </summary>
    void PlayBlockBreakAnimation(Action onComplete = null)
    {
        if (img_block == null || blockBreakSprites == null || blockBreakSprites.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 确保砖块可见并隐藏遮罩（避免遮罩覆盖碎裂动画）
        img_block.gameObject.SetActive(true);
        if (img_mask != null)
        {
            img_mask.gameObject.SetActive(false);
        }

        float frameDuration = MineConfig.BLOCK_ANIMATION_DURATION / blockBreakSprites.Length;

        // 使用DOTween序列播放Sprite动画
        Sequence sequence = DOTween.Sequence();
        
        for (int i = 0; i < blockBreakSprites.Length; i++)
        {
            int frameIndex = i;
            sequence.AppendCallback(() =>
            {
                if (img_block != null && frameIndex < blockBreakSprites.Length)
                {
                    img_block.sprite = blockBreakSprites[frameIndex];
                }
            });
            sequence.AppendInterval(frameDuration);
        }

        sequence.OnComplete(() =>
        {
            // 动画完成后隐藏砖块并重置为初始Sprite
            if (img_block != null)
            {
                img_block.gameObject.SetActive(false);
                ResetBlockSprite();
            }
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 设置格子数据
    /// </summary>
    public void SetGridData(MineGridData data)
    {
        gridData = data;
        RefreshView();
    }

    /// <summary>
    /// 刷新视图显示
    /// </summary>
    public void RefreshView()
    {
        if (gridData == null) return;

        // 隐藏所有特殊图标
        if (img_treasure != null) img_treasure.gameObject.SetActive(false);
        if (img_bomb != null) img_bomb.gameObject.SetActive(false);
        if (img_bombEffect != null) 
        {
            img_bombEffect.gameObject.SetActive(false);
            Debug.Log($"MineBlockView[{gridX},{gridY}]: 隐藏炸弹特效");
        }
        if (img_stair != null) img_stair.gameObject.SetActive(false);
        if (img_stairEntrance != null) img_stairEntrance.gameObject.SetActive(false);
        if (img_highlight != null) img_highlight.gameObject.SetActive(false);

        // 根据格子状态显示
        if (gridData.blockType == MineBlockType.Empty || gridData.isRevealed)
        {
            // 空地或已开采
            if (img_block != null) img_block.gameObject.SetActive(false);
            if (img_mask != null) img_mask.gameObject.SetActive(false);

            // 如果有楼梯入口
            if (gridData.hasStairEntrance && img_stairEntrance != null)
            {
                img_stairEntrance.gameObject.SetActive(true);
            }
        }
        else
        {
            // 砖块
            if (img_block != null) img_block.gameObject.SetActive(true);

            // 遮罩状态
            if (img_mask != null)
            {
                img_mask.gameObject.SetActive(gridData.isMasked);
            }

            // 显示特殊砖块图标（包括遮罩状态）
            ShowBlockTypeIcon();
        }
    }

    /// <summary>
    /// 显示砖块类型图标
    /// </summary>
    void ShowBlockTypeIcon()
    {
        switch (gridData.blockType)
        {
            case MineBlockType.Treasure:
                if (img_treasure != null) 
                {
                    img_treasure.gameObject.SetActive(true);
                    // 显示具体物品图标而不是宝箱图标
                    if (gridData.rewardItem != null)
                    {
                        Debug.Log($"MineBlockView[{gridX},{gridY}]: 显示物品图标，物品ID: {gridData.rewardItem.settingId}, 数量: {gridData.rewardItem.count}");
                        img_treasure.ShowItemIcon(gridData.rewardItem);
                    }
                    else
                    {
                        Debug.LogWarning($"MineBlockView[{gridX},{gridY}]: rewardItem 为空，无法显示物品图标");
                    }
                }
                break;
            case MineBlockType.Bomb:
                if (img_bomb != null) img_bomb.gameObject.SetActive(true);
                break;
            case MineBlockType.Stair:
                if (img_stair != null) img_stair.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    void OnClick()
    {
        if (gridData == null) return;

        // 遮罩状态下不响应点击
        if (gridData.isMasked)
        {
            return;
        }

        // 检查是否是楼梯入口（楼梯入口不受处理状态限制）
        if (gridData.hasStairEntrance)
        {
            onClickCallback?.Invoke(this);
            return;
        }

        // 检查父面板是否正在处理中（避免重复点击）
        if (parentPanel != null && parentPanel.isProcessing)
        {
            return;
        }

        // 检查是否可以开采（空地不能开采）
        if (!gridData.CanBeMined())
        {
            // 无效点击反馈
            PlayInvalidClickFeedback();
            return;
        }

        // 触发点击回调
        onClickCallback?.Invoke(this);
    }

    /// <summary>
    /// 播放无效点击反馈
    /// </summary>
    void PlayInvalidClickFeedback()
    {
        // 抖动效果
        transform.DOKill();
        transform.DOShakePosition(0.2f, 5f).OnComplete(() =>
        {
            transform.localPosition = GetTopLeftPosition();
        });

        // 可以添加音效
        // AuditionManager.Instance.PlayVoice(AudioClipType.InvalidClick);
    }

    /// <summary>
    /// 播放开采动画
    /// </summary>
    /// <param name="blockType">原始砖块类型（因为 gridData.blockType 在调用前已被修改为 Empty）</param>
    /// <param name="onComplete">动画完成回调</param>
    public void PlayMineAnimation(MineBlockType blockType, Action onComplete = null)
    {
        // 根据砖块类型播放不同动画
        switch (blockType)
        {
            case MineBlockType.Treasure:
                PlayTreasureMineAnimation(onComplete);
                break;
            case MineBlockType.Bomb:
                PlayBombMineAnimation(onComplete);
                break;
            default:
                PlayNormalMineAnimation(onComplete);
                break;
        }
    }

    /// <summary>
    /// 播放普通砖块开采动画
    /// </summary>
    void PlayNormalMineAnimation(Action onComplete = null)
    {
        // 使用Sprite序列动画播放砖块碎裂
        PlayBlockBreakAnimation(onComplete);
    }

    /// <summary>
    /// 播放宝箱开采动画（先消失砖块再播放宝箱动画）
    /// </summary>
    void PlayTreasureMineAnimation(Action onComplete = null)
    {
        // 确保宝藏图标在动画播放期间可见
        if (img_treasure != null)
        {
            img_treasure.gameObject.SetActive(true);
        }
        
        // 使用Sprite序列动画播放砖块碎裂
        PlayBlockBreakAnimation(() =>
        {
            // 砖块动画完成后隐藏宝藏图标
            if (img_treasure != null)
            {
                img_treasure.gameObject.SetActive(false);
            }
            onComplete?.Invoke();
        });
    }

    
    /// <summary>
    /// 播放炸弹开采动画（爆炸特效与砖块碎裂动画同时播放）
    /// </summary>
    void PlayBombMineAnimation(Action onComplete = null)
    {
        // 确保砖块在动画播放期间可见
        if (img_block != null)
        {
            img_block.gameObject.SetActive(true);
        }

        // 同时播放爆炸特效和砖块碎裂动画
        bool effectDone = false;
        bool blockDone = false;
        
        System.Action checkComplete = () =>
        {
            if (effectDone && blockDone)
            {
                onComplete?.Invoke();
            }
        };
        
        // 播放爆炸特效
        PlayBombEffectAnimation(() =>
        {
            effectDone = true;
            checkComplete();
        });
        
        // 同时播放砖块碎裂Sprite序列动画
        PlayBlockBreakAnimation(() =>
        {
            blockDone = true;
            checkComplete();
        });
    }

    /// <summary>
    /// 播放炸弹爆炸特效
    /// </summary>
    void PlayBombEffectAnimation(Action onComplete = null)
    {
        // 隐藏炸弹图标
        if (img_bomb != null)
        {
            img_bomb.gameObject.SetActive(false);
        }

        // 显示爆炸特效
        if (img_bombEffect != null)
        {
            img_bombEffect.gameObject.SetActive(true);
            img_bombEffect.transform.localScale = Vector3.zero;
            img_bombEffect.transform.DOKill();
            // 爆炸放大效果
            img_bombEffect.transform.DOScale(1.5f, 0.2f).OnComplete(() =>
            {
                // 爆炸消散
                img_bombEffect.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    img_bombEffect.gameObject.SetActive(false);
                    img_bombEffect.transform.localScale = Vector3.one;
                    img_bombEffect.color = new Color(img_bombEffect.color.r, img_bombEffect.color.g, img_bombEffect.color.b, 1f);
                    onComplete?.Invoke();
                });
            });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 播放获得奖励动画
    /// </summary>
    public void PlayRewardAnimation(ItemData reward, Transform targetPos)
    {
        if (reward == null) return;

        // 创建飞行物品动画（可以使用现有的GetItemFlyUpAnimView）
        // 这里简化处理，直接显示获得物品
    }

    /// <summary>
    /// 显示高亮
    /// </summary>
    public void ShowHighlight(bool show)
    {
        if (img_highlight != null)
        {
            img_highlight.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// 获取格子大小（从预制体实际大小获取）
    /// </summary>
    float GetBlockSize()
    {
        // 从砖块图片组件获取实际大小
        if (img_block != null)
        {
            RectTransform rectTransform = img_block.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform.rect.width;
            }
        }
        
        // 备用方案：从自身 RectTransform 获取
        RectTransform selfRect = GetComponent<RectTransform>();
        if (selfRect != null)
        {
            return selfRect.rect.width;
        }
        
        // 默认值
        return 100f;
    }

    /// <summary>
    /// 获取在网格容器中的相对位置（左上角对齐）
    /// </summary>
    public Vector3 GetTopLeftPosition()
    {
        float size = GetBlockSize();
        
        // 左上角对齐：第一个砖块(0,0)在(0,0)位置
        // 注意：Unity UI中Y轴向下为正，所以需要翻转Y坐标
        return new Vector3(gridX * size+75, -gridY * size-75, 0);
    }

    public override void Clear()
    {
        base.Clear();
        gridData = null;
        onClickCallback = null;
        parentPanel = null;

        if (img_block != null)
        {
            img_block.transform.DOKill();
            img_block.transform.localScale = Vector3.one;
            img_block.gameObject.SetActive(true);
        }

        if (img_treasure != null)
        {
            img_treasure.transform.DOKill();
            img_treasure.transform.localScale = Vector3.one;
        }

        if (img_bombEffect != null)
        {
            img_bombEffect.transform.DOKill();
            img_bombEffect.transform.localScale = Vector3.one;
            img_bombEffect.color = new Color(img_bombEffect.color.r, img_bombEffect.color.g, img_bombEffect.color.b, 1f);
            img_bombEffect.gameObject.SetActive(false);
        }

        transform.DOKill();
    }
}
