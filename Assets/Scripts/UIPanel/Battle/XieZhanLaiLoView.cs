using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 协战来咯提示UI
/// 当协战队友出场时显示，使用DOTween实现弹出动画
/// </summary>
public class XieZhanLaiLoView : SingleViewBase
{
    public Text txt_xieZhanLaiLo;     // "协战来咯！"文字
    public Text txt_warning;          // "WARNING"文字
    public Image img_background;      // 背景图
    public Image img_leftLine;        // 左侧装饰线
    public Image img_rightLine;       // 右侧装饰线
    public CanvasGroup canvasGroup;   // 用于整体淡入淡出

    public Transform trans_content;   // 内容容器

    // 动画参数
    private float showDuration = 0.4f;
    private float stayDuration = 1.5f;
    private float hideDuration = 0.3f;

    public override void Init(params object[] args)
    {
        base.Init(args);

        // 初始化状态
        InitState();

        // 播放弹出动画
        PlayShowAnim();
    }

    /// <summary>
    /// 初始化状态
    /// </summary>
    void InitState()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }

        if (trans_content != null)
        {
            trans_content.localScale = new Vector3(0.3f, 0.3f, 1f);
        }

        // 文字初始状态
        if (txt_xieZhanLaiLo != null)
        {
            txt_xieZhanLaiLo.transform.localScale = Vector3.zero;
        }

        if (txt_warning != null)
        {
            txt_warning.transform.localScale = Vector3.zero;
            txt_warning.color = new Color(txt_warning.color.r, txt_warning.color.g, txt_warning.color.b, 0);
        }

        // 装饰线初始状态
        if (img_leftLine != null)
        {
            img_leftLine.transform.localScale = new Vector3(0, 1, 1);
        }

        if (img_rightLine != null)
        {
            img_rightLine.transform.localScale = new Vector3(0, 1, 1);
        }
    }

    /// <summary>
    /// 播放显示动画
    /// </summary>
    void PlayShowAnim()
    {
        Sequence seq = DOTween.Sequence();

        // 整体淡入
        if (canvasGroup != null)
        {
            seq.Append(canvasGroup.DOFade(1, showDuration * 0.5f));
        }

        // 内容容器弹出
        if (trans_content != null)
        {
            seq.Join(trans_content.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack));
        }

        // 装饰线展开
        if (img_leftLine != null)
        {
            seq.Join(img_leftLine.transform.DOScaleX(1, showDuration * 0.8f).SetEase(Ease.OutQuad));
        }

        if (img_rightLine != null)
        {
            seq.Join(img_rightLine.transform.DOScaleX(1, showDuration * 0.8f).SetEase(Ease.OutQuad));
        }

        // "协战来咯！"文字弹出
        if (txt_xieZhanLaiLo != null)
        {
            seq.Append(txt_xieZhanLaiLo.transform.DOScale(Vector3.one, showDuration * 0.6f).SetEase(Ease.OutBack));

            // 文字抖动强调
            seq.Append(txt_xieZhanLaiLo.transform.DOShakeScale(0.2f, 0.1f, 10));
        }

        // "WARNING"文字淡入
        if (txt_warning != null)
        {
            seq.Join(txt_warning.transform.DOScale(Vector3.one, showDuration * 0.4f).SetEase(Ease.OutQuad));
            seq.Join(txt_warning.DOFade(1, showDuration * 0.4f));

            // WARNING闪烁效果
            seq.Append(txt_warning.DOFade(0.5f, 0.15f).SetLoops(4, LoopType.Yoyo));
        }

        // 停留一段时间后自动隐藏
        seq.AppendInterval(stayDuration);
        seq.AppendCallback(() => PlayHideAnim());
    }

    /// <summary>
    /// 播放隐藏动画
    /// </summary>
    void PlayHideAnim()
    {
        Sequence seq = DOTween.Sequence();

        // 整体缩小并淡出
        if (trans_content != null)
        {
            seq.Append(trans_content.DOScale(new Vector3(1.2f, 0.1f, 1f), hideDuration).SetEase(Ease.InBack));
        }

        if (canvasGroup != null)
        {
            seq.Join(canvasGroup.DOFade(0, hideDuration));
        }

        // 动画完成后关闭
        seq.OnComplete(() =>
        {
            PanelManager.Instance.CloseSingle(this);
        });
    }

    /// <summary>
    /// 立即隐藏（跳过动画）
    /// </summary>
    public void HideImmediately()
    {
        DOTween.Kill(transform);
        if (canvasGroup != null)
            DOTween.Kill(canvasGroup);
        if (trans_content != null)
            DOTween.Kill(trans_content);
        if (txt_xieZhanLaiLo != null)
            DOTween.Kill(txt_xieZhanLaiLo.transform);
        if (txt_warning != null)
            DOTween.Kill(txt_warning);

        PanelManager.Instance.CloseSingle(this);
    }

    public override void Clear()
    {
        base.Clear();

        // 停止所有动画
        DOTween.Kill(transform);
        if (canvasGroup != null)
            DOTween.Kill(canvasGroup);
        if (trans_content != null)
            DOTween.Kill(trans_content);
        if (txt_xieZhanLaiLo != null)
            DOTween.Kill(txt_xieZhanLaiLo.transform);
        if (txt_warning != null)
            DOTween.Kill(txt_warning);
        if (img_leftLine != null)
            DOTween.Kill(img_leftLine.transform);
        if (img_rightLine != null)
            DOTween.Kill(img_rightLine.transform);
    }
}
