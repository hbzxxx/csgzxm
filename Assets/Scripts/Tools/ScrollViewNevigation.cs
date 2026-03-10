using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.ScrollRect;

/// <summary>
/// 循环滚动类型
/// </summary>
public enum ScrollLoopType
{
    None = 0,       // 不循环
    Vertical = 1,   // 垂直循环
    Horizontal = 2,  // 水平循环
    Default,
}

public class ScrollViewNevigation : MonoBehaviour
{

    public ScrollRect scrollRect;
    public RectTransform viewport;
    public RectTransform content;

    public RectTransform testObj;

    [Header("循环滚动设置")]
    public ScrollLoopType loopType = ScrollLoopType.None;
    public List<RectTransform> loopItemList = new List<RectTransform>();

    private float itemHeight;
    private float itemWidth;
    private int itemCount;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        EventCenter.Remove(TheEventType.TargetSightToCertainObj, OnReceiveTargetEvent);
        EventCenter.Register(TheEventType.TargetSightToCertainObj, OnReceiveTargetEvent);
    }
    // Use this for initialization
    void Start ()
	{
        Init();
    }

    // Update is called once per frame
    void Update () {
        if (loopType != ScrollLoopType.None && loopItemList.Count > 0)
        {
            UpdateLoopScroll();
        }
	}

    private void Init()
    {
        if (loopType != ScrollLoopType.None && loopItemList.Count > 0)
        {
            scrollRect.GetComponent<ScrollRect>().movementType = MovementType.Unrestricted;
            InitLoopScroll();
        }
        else if(loopType == ScrollLoopType.Default)
        {
            //scrollRect.GetComponent<ScrollRect>().movementType = MovementType.Unrestricted;
        }
        else {
            scrollRect.GetComponent<ScrollRect>().movementType = MovementType.Clamped;
        }
    }

    #region 循环滚动功能

    /// <summary>
    /// 初始化循环滚动
    /// </summary>
    public void InitLoopScroll()
    {
        if (loopItemList.Count == 0) return;

        itemCount = loopItemList.Count;

        // 获取item尺寸
        RectTransform firstItem = loopItemList[0];
        itemWidth = firstItem.rect.width;
        itemHeight = firstItem.rect.height;
    }

    /// <summary>
    /// 更新循环滚动
    /// </summary>
    private void UpdateLoopScroll()
    {
        if (loopType == ScrollLoopType.Vertical)
        {
            UpdateVerticalLoop();
        }
        else if (loopType == ScrollLoopType.Horizontal)
        {
            UpdateHorizontalLoop();
        }
    }

    /// <summary>
    /// 垂直循环滚动
    /// </summary>
    private void UpdateVerticalLoop()
    {
        if (loopItemList.Count == 0) return;

        float viewportHeight = viewport.rect.height;

        for (int i = 0; i < loopItemList.Count; i++)
        {
            RectTransform item = loopItemList[i];
            // 获取item在viewport中的位置
            Vector3 itemWorldPos = item.position;
            Vector3 itemLocalPos = viewport.InverseTransformPoint(itemWorldPos);

            // 向上滚动：item完全超出viewport顶部，移到最下面
            if (itemLocalPos.y > itemHeight)
            {
                // 找到最下面的item
                RectTransform bottomItem = GetBottomItem();
                float newY = bottomItem.anchoredPosition.y - itemHeight;
                item.anchoredPosition = new Vector2(item.anchoredPosition.x, newY);
            }
            // 向下滚动：item完全超出viewport底部，移到最上面
            else if (itemLocalPos.y < -itemHeight)
            {
                // 找到最上面的item
                RectTransform topItem = GetTopItem();
                float newY = topItem.anchoredPosition.y + itemHeight;
                item.anchoredPosition = new Vector2(item.anchoredPosition.x, newY);
            }
        }
    }

    /// <summary>
    /// 水平循环滚动
    /// </summary>
    private void UpdateHorizontalLoop()
    {
        if (loopItemList.Count == 0) return;

        float viewportWidth = viewport.rect.width;

        for (int i = 0; i < loopItemList.Count; i++)
        {
            RectTransform item = loopItemList[i];
            // 获取item在viewport中的位置
            Vector3 itemWorldPos = item.position;
            Vector3 itemLocalPos = viewport.InverseTransformPoint(itemWorldPos);

            // 向左滚动：item完全超出viewport左边，移到最右边
            if (itemLocalPos.x <  -itemWidth)
            {
                RectTransform rightItem = GetRightItem();
                float newX = rightItem.anchoredPosition.x + itemWidth;
                item.anchoredPosition = new Vector2(newX, item.anchoredPosition.y);
            }
            // 向右滚动：item完全超出viewport右边，移到最左边
            else if (itemLocalPos.x > itemWidth)
            {
                RectTransform leftItem = GetLeftItem();
                float newX = leftItem.anchoredPosition.x - itemWidth;
                item.anchoredPosition = new Vector2(newX, item.anchoredPosition.y);
            }
        }
    }

    /// <summary>
    /// 获取最上面的item
    /// </summary>
    private RectTransform GetTopItem()
    {
        RectTransform topItem = loopItemList[0];
        for (int i = 1; i < loopItemList.Count; i++)
        {
            if (loopItemList[i].anchoredPosition.y > topItem.anchoredPosition.y)
            {
                topItem = loopItemList[i];
            }
        }
        return topItem;
    }

    /// <summary>
    /// 获取最下面的item
    /// </summary>
    private RectTransform GetBottomItem()
    {
        RectTransform bottomItem = loopItemList[0];
        for (int i = 1; i < loopItemList.Count; i++)
        {
            if (loopItemList[i].anchoredPosition.y < bottomItem.anchoredPosition.y)
            {
                bottomItem = loopItemList[i];
            }
        }
        return bottomItem;
    }

    /// <summary>
    /// 获取最左边的item
    /// </summary>
    private RectTransform GetLeftItem()
    {
        RectTransform leftItem = loopItemList[0];
        for (int i = 1; i < loopItemList.Count; i++)
        {
            if (loopItemList[i].anchoredPosition.x < leftItem.anchoredPosition.x)
            {
                leftItem = loopItemList[i];
            }
        }
        return leftItem;
    }

    /// <summary>
    /// 获取最右边的item
    /// </summary>
    private RectTransform GetRightItem()
    {
        RectTransform rightItem = loopItemList[0];
        for (int i = 1; i < loopItemList.Count; i++)
        {
            if (loopItemList[i].anchoredPosition.x > rightItem.anchoredPosition.x)
            {
                rightItem = loopItemList[i];
            }
        }
        return rightItem;
    }

    /// <summary>
    /// 设置循环item列表
    /// </summary>
    public void SetLoopItems(List<RectTransform> items)
    {
        loopItemList = items;
        InitLoopScroll();
    }

    /// <summary>
    /// 设置循环类型
    /// </summary>
    public void SetLoopType(ScrollLoopType type)
    {
        loopType = type;
    }

    #endregion

    #region 原有导航功能

    public void OnReceiveTargetEvent(object[] args)
    {
        GameObject obj = args[0] as GameObject;
        bool tween =(bool) args[1];
        if (tween)
            Nevigate(obj.GetComponent<RectTransform>());
        else
            NevigateImmediately(obj.GetComponent<RectTransform>());
    }

    private Vector3 Clear_Pivot_Offset(RectTransform rec)
    {
        var offset = new Vector3((0.5f - rec.pivot.x) * rec.rect.width,
        (0.5f - rec.pivot.y) * rec.rect.height, 0.0f);
        var newPosition = rec.localPosition + offset;
        return rec.parent.TransformPoint(newPosition);
    }

    /// <summary>
    /// 直接导航过来
    /// </summary>
    public void NevigateImmediately(RectTransform item)
    {
        var newNormalizedPosition = GetNewPos(item);
        scrollRect.GetComponent<ScrollRect>().normalizedPosition = newNormalizedPosition;
    }
    public void Nevigate(RectTransform item,bool forbidMove=true,float speed=0.8f)
    {
        var newNormalizedPosition = GetNewPos(item);


        bool beforehorizontal = scrollRect.horizontal;
        bool beforeVertical = scrollRect.vertical;
        if (forbidMove)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = false;
        }

        DOTween.To(() => scrollRect.GetComponent<ScrollRect>().normalizedPosition, x => scrollRect.GetComponent<ScrollRect>().normalizedPosition = x, newNormalizedPosition, speed).OnComplete(() =>
        {
            scrollRect.horizontal = beforehorizontal;
            scrollRect.vertical = beforeVertical;
            EventCenter.Broadcast(TheEventType.FinishTargetSightToCertainObj, item.gameObject);
        });
    }

    /// <summary>
    /// 获取导航的新地点
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Vector2 GetNewPos(RectTransform item)
    {
        Vector3 itemCurrentLocalPostion = scrollRect.GetComponent<RectTransform>().InverseTransformVector(ConvertLocalPosToWorldPos(item));
        Vector3 itemTargetLocalPos = scrollRect.GetComponent<RectTransform>().InverseTransformVector(ConvertLocalPosToWorldPos(scrollRect.GetComponent<RectTransform>()));

        Vector3 diff = itemTargetLocalPos - itemCurrentLocalPostion;
        diff.z = 0.0f;

        var newNormalizedPosition = new Vector2(
            diff.x / (content.GetComponent<RectTransform>().rect.width - scrollRect.GetComponent<RectTransform>().rect.width),
            diff.y / (content.GetComponent<RectTransform>().rect.height - scrollRect.GetComponent<RectTransform>().rect.height)
            );

        newNormalizedPosition = scrollRect.GetComponent<ScrollRect>().normalizedPosition - newNormalizedPosition;

        newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
        newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
        return newNormalizedPosition;

    }

    private Vector3 ConvertLocalPosToWorldPos(RectTransform target)
    {
        var pivotOffset = new Vector3(
            (0.5f - target.pivot.x) * target.rect.size.x,
            (0.5f - target.pivot.y) * target.rect.size.y,
            0f);

        var localPosition = target.localPosition + pivotOffset;

        return target.parent.TransformPoint(localPosition);
    }

    #endregion

    private void OnDisable()
    {
        EventCenter.Remove(TheEventType.TargetSightToCertainObj, OnReceiveTargetEvent);

    }
}
