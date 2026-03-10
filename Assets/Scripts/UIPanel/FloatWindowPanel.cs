using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 飘窗
/// </summary>
public class FloatWindowPanel : PanelBase
{
    public string str_content;
    public Text txt_content;//内容
    public Image img_bg;//背景
    // 原先协程参数: showTime=0.5s 后开始淡出，淡出耗时0.5s。改为计时器控制。
    [SerializeField] float stayDuration = 0.5f;   // 停留时间
    [SerializeField] float fadeDuration = 0.5f;   // 淡出时间

    float timer = 0f;
    bool fading = false;
    bool finished = false;

    public override void Init(object[] args)
    {
        base.Init(args);
        str_content = args[0] as string;
    }

    /// <summary>
    /// 给组件赋值
    /// </summary>
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_content.text = str_content;
        // 还原透明度（确保复用时不残留上次淡出状态）
        if (img_bg != null)
        {
            var c = img_bg.color; c.a = 1f; img_bg.color = c;
        }
        if (txt_content != null)
        {
            var c2 = txt_content.color; c2.a = 1f; txt_content.color = c2;
        }
        timer = 0f;
        fading = false;
        finished = false;
    }

    private void Update()
    {
        if (finished) return;

        timer += Time.deltaTime;

        // 尚未开始淡出
        if (!fading)
        {
            if (timer >= stayDuration)
            {
                // 进入淡出
                fading = true;
                timer = 0f; // 复用 timer 记录淡出已进行时间
            }
        }
        else
        {
            float t = fadeDuration <= 0f ? 1f : Mathf.Clamp01(timer / fadeDuration);
            float alpha = 1f - t;
            if (img_bg != null)
            {
                var c = img_bg.color; c.a = alpha; img_bg.color = c;
            }
            if (txt_content != null)
            {
                var c2 = txt_content.color; c2.a = alpha; txt_content.color = c2;
            }
            if (t >= 1f)
            {
                finished = true;
                PanelManager.Instance.ClosePanel(this);
            }
        }
    }


  
}
