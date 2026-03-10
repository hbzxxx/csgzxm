using DG.Tweening;
using Framework.Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗场景中协战队友显示View
/// 显示在主战斗角色后面，死亡后变灰
/// </summary>
public class BattleXieZhanView : SingleViewBase
{
    public Image img_character;      // 角色图片（后续替换为骨骼动画）
    public Image img_grayMask;       // 死亡灰色遮罩
    public CanvasGroup canvasGroup;  // 用于淡入淡出动画

    public PeopleData peopleData;
    public bool isPlayer;            // 是否是玩家方
    public bool isDead;              // 是否死亡
    public int posIndex;             // 在协战队伍中的位置索引

    // 动画相关
    private Vector3 originalPos;
    private float enterAnimDuration = 0.5f;

    public override void Init(params object[] args)
    {
        base.Init(args);

        peopleData = args[0] as PeopleData;
        isPlayer = (bool)args[1];
        posIndex = (int)args[2];

        isDead = false;

        // 设置角色图片
        RefreshCharacterImage();

        // 初始化状态
        if (img_grayMask != null)
            img_grayMask.gameObject.SetActive(false);

        // 记录原始位置
        originalPos = transform.localPosition;

        // 播放入场动画
        PlayEnterAnim();
    }

    /// <summary>
    /// 刷新角色图片显示
    /// 后续这里会替换为骨骼动画
    /// </summary>
    void RefreshCharacterImage()
    {
        if (img_character != null)
        {
            // 使用头像作为临时图片，后续替换为角色立绘或骨骼动画
            StudentManager.Instance.SetTouxiang(img_character, peopleData);
        }
    }

    /// <summary>
    /// 播放入场动画
    /// </summary>
    public void PlayEnterAnim()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, enterAnimDuration);
        }

        // 从下方滑入
        Vector3 startPos = originalPos + new Vector3(0, -100, 0);
        transform.localPosition = startPos;
        transform.DOLocalMove(originalPos, enterAnimDuration).SetEase(Ease.OutBack);

        // 缩放动画
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, enterAnimDuration).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// 播放退场动画
    /// </summary>
    public void PlayExitAnim(System.Action onComplete = null)
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0, enterAnimDuration * 0.5f);
        }

        transform.DOScale(Vector3.zero, enterAnimDuration * 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// 设置死亡状态（变灰）
    /// </summary>
    public void SetDead(bool dead)
    {
        isDead = dead;

        if (img_grayMask != null)
        {
            img_grayMask.gameObject.SetActive(dead);
        }

        // 角色图片变灰
        if (img_character != null)
        {
            if (dead)
            {
                // 变灰效果
                img_character.color = new Color(0.4f, 0.4f, 0.4f, 1f);

                // 播放死亡动画
                PlayDeadAnim();
            }
            else
            {
                img_character.color = Color.white;
            }
        }
    }

    /// <summary>
    /// 播放死亡动画
    /// </summary>
    void PlayDeadAnim()
    {
        // 抖动效果
        transform.DOShakePosition(0.3f, 10f, 20);

        // 下沉效果
        transform.DOLocalMoveY(originalPos.y - 30f, 0.5f)
            .SetDelay(0.3f)
            .SetEase(Ease.InQuad);
    }

    /// <summary>
    /// 播放攻击动画（协战攻击时调用）
    /// </summary>
    public void PlayAttackAnim()
    {
        // 向前冲刺
        float direction = isPlayer ? 1 : -1;
        Vector3 attackPos = originalPos + new Vector3(direction * 50f, 0, 0);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(attackPos, 0.2f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOLocalMove(originalPos, 0.3f).SetEase(Ease.InOutQuad));
    }

    /// <summary>
    /// 播放待机动画（后续替换为骨骼动画）
    /// </summary>
    public void PlayIdleAnim()
    {
        // 简单的呼吸效果
        transform.DOScale(new Vector3(1.02f, 0.98f, 1f), 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// 停止所有动画
    /// </summary>
    public void StopAllAnims()
    {
        transform.DOKill();
        if (canvasGroup != null)
            canvasGroup.DOKill();
    }

    public override void Clear()
    {
        base.Clear();
        StopAllAnims();
        peopleData = null;
        isDead = false;
    }
}
