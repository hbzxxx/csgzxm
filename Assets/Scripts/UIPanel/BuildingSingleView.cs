using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using cfg;

public class BuildingSingleView : SingleViewBase,IDragHandler
{
    public RectTransform rt;
    public Image icon;
    public Image img_range;
    public Button btn_confirm;//确定
    public Button btn_cancel;//取消
    public int farmId;
    public MountainPanel parentPanel;
    public DanFarmSetting farmSetting;
    bool available = false;
    public override void Init(params object[] args)
    {
        base.Init(args);

        farmId = (int)args[0];
        parentPanel = args[1] as MountainPanel;

        farmSetting = DataTable.FindDanFarmSetting(farmId);

        RegisterEvent(TheEventType.Building_overlapRes, OverlapRes);
        RegisterEvent(TheEventType.OnMountainScrollMove, OnMountainScrollMove);
        RegisterEvent(TheEventType.QuitBuildingMode, OnQuitBuildingMode);
        addBtnListener(btn_confirm, () =>
         {
             //允许吗

             //确定建造
             if (available)
             {
                 LianDanManager.Instance.OnBuildNewDanFarm(farmId, (Vector2)transform.localPosition);
                 PanelManager.Instance.CloseSingle(this);
                 BuildingManager.Instance.QuitBuildingMode();
             }
             else
             {
                 PanelManager.Instance.OpenFloatWindow("请将建筑拖动到合适的位置");
             }
         
         });

        addBtnListener(btn_cancel, () =>
         {
             PanelManager.Instance.CloseSingle(this);
             BuildingManager.Instance.QuitBuildingMode();
         });
        
    }



    public override void OnOpenIng()
    {
        base.OnOpenIng();
        icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + farmSetting.UiName);
        icon.SetNativeSize();
        //img_range.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + farmSetting.RangeIconName);
        img_range.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + ConstantVal.bg_lingshu_5);
        //img_range.SetNativeSize();
        rt =img_range.GetComponent<RectTransform>();
        transform.localPosition =  - (Vector2)parentPanel.trans_content.localPosition;  //new Vector2(Screen.width/2,Screen.height/2);
        EventCenter.Broadcast(TheEventType.Building_overlapSearch, rt);

    }

    /// <summary>
    /// 是否冲突结果
    /// </summary>
    /// <param name="args"></param>
    void OverlapRes(object[] args)
    {
        bool overlap = (bool)args[0];
        if (overlap)
        {
            img_range.color = ConstantVal.unAvailableRed;
        }
        else
        {
            img_range.color = ConstantVal.availableGreen;
        }
        available = !overlap;
    }

    void OnMountainScrollMove(object[] args)
    {
        Transform landTrans = (Transform)args[0];
        ShowPos();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = eventData.position- new Vector2(Screen.width / 2, Screen.height / 2) - (Vector2)parentPanel.trans_content.localPosition;  //new Vector2(Screen.width/2,Screen.height/2);
        //山门的所有ui是否重叠

        //Debug.Log("当前是否重叠：" + RectTransToScreenPos(rt, null).Overlaps(RectTransToScreenPos(TargetRt, null)));
        EventCenter.Broadcast(TheEventType.Building_overlapSearch, rt);
    }

    /// <summary>
    /// 显示位置 如果这里出现出框bug 那就是锚点设置有问题content的锚点为x0.5y1
    /// </summary>
    public void ShowPos()
    {
         
        //世界坐标右上角
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f,
         Mathf.Abs(-Camera.main.transform.position.z)));
        //世界坐标左边界
       float leftBorder = Camera.main.transform.position.x - (cornerPos.x - Camera.main.transform.position.x);
        //世界坐标右边界
        float rightBorder = cornerPos.x;
        //世界坐标上边界
        float topBorder = cornerPos.y;
        //世界坐标下边界
        float downBorder = Camera.main.transform.position.y - (cornerPos.y - Camera.main.transform.position.y);



        //index从0到3分别为 左下 左上 右上 右下的世界坐标
        Vector3[] corners = new Vector3[4];
        transform.GetComponent<RectTransform>().GetWorldCorners(corners);
        //width = rightBorder - leftBorder;
        //height = topBorder - downBorder;

        Vector3 leftDownVec = corners[0];//左下
        Vector3 leftUpVec = corners[1];//左上
        Vector3 rightUpVec = corners[2];//右上
        Vector3 rightDownVec = corners[3];//右下

        float myWidth = rightUpVec.x - leftUpVec.x;
        float myHeight = leftUpVec.y - leftDownVec.y;

        if (leftUpVec.y >= topBorder)
        {
            Debug.Log("到达上边界");
            transform.position = new Vector3(transform.position.x, topBorder-myHeight/2, 0);
        }
        //下
        if (leftDownVec.y <= downBorder)
        {
            Debug.Log("到达下边界");

            transform.position = new Vector3(transform.position.x, downBorder+myHeight/2, 0);

        }
        //左
        if (leftDownVec.x <= leftBorder)
        {
            Debug.Log("到达左边界");

            transform.position = new Vector3(leftBorder + myWidth / 2, transform.position.y, 0);
        }
        //右
        if (rightDownVec.x >= rightBorder)
        {
            Debug.Log("到达右边界");
            transform.position = new Vector3(rightBorder - myWidth / 2, transform.position.y, 0);

        }
        EventCenter.Broadcast(TheEventType.Building_overlapSearch, rt);
    }

    /// <summary>
    /// 相机传入主要取决于rt都画布，如果画布使用camera模式，则传入对应的相机,反之为null，
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="cam"></param>
    /// <returns></returns>
    public static Rect RectTransToScreenPos(RectTransform rt, Camera cam)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 v0 = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 v1 = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);
        Rect rect = new Rect(v0, v1 - v0);
        return rect;
    }
    public override void Clear()
    {
        base.Clear();
        available = false;
    }

    void OnQuitBuildingMode()
    {
        PanelManager.Instance.CloseSingle(this);
    }
}
