using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using cfg;

public class NewGuideCanvas : MonoBehaviour,UnityEngine.EventSystems.IPointerClickHandler
{
    public GraphicRaycaster RaycastInCanvas;
    EventSystem eventSystem;

    public GameObject highLightedObj;//高亮obj
    public NewGuideSetting setting;

    public GameObject obj_focus;//聚焦手指
    public GameObject obj_circle;//手指圈

    public GameObject obj_focuseBtn;//点哪个位置
    public GuidePointType curGuideType;//当前的指引

    public int curIndex;//当前指示的下标
    public List<string> highLightedPathList;//高亮的位置
    public List<string> focuseBtnPathList;//指向按钮的位置
    public List<string> txtList;//文字
    public List<int> guideTypeList;//是指还是移动地图后指
    bool startSearchingPath;//找聚焦物品的路径
    public Text txt_guide;//指引说明

    bool notFind = false;
    float notFindTimer = 0;
    float notFindTime = 1;
    Action callBack = null;
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(NewGuideSetting theSetting,Action theCallBack=null)
    {
        startSearchingPath = false;
        this.GetComponent<Canvas>().worldCamera = Camera.main;
        eventSystem = EventSystem.current;
        obj_focus.gameObject.SetActive(false);

        GameTimeManager.Instance.AddTimeBlockCount();
        Debug.Log("NewGuideCanvas调用时停" + theSetting.Id);
        setting = theSetting;


        eventSystem = EventSystem.current;
        if (RaycastInCanvas == null)
            RaycastInCanvas = GetComponentInParent<GraphicRaycaster>();


        //highLightedPathList = setting.highLightObjPath.Split('|');
        highLightedPathList = CommonUtil.SplitCfgStringOneDepth(setting.HighLightObjPath);
        focuseBtnPathList = CommonUtil.SplitCfgStringOneDepth(setting.HighLightObjPath);
       guideTypeList= CommonUtil.SplitCfgOneDepth(setting.GuideType);
        if (!string.IsNullOrWhiteSpace(setting.Txt))
            txtList = CommonUtil.SplitCfgStringOneDepth(setting.Txt);
        else
            txtList = new List<string>();

        //guideTypeList = new List<GuideType>();
        //for (int i = 0; i < typeList.Count; i++)
        //{
        //    guideTypeList
        //}

        for (int i= highLightedPathList.Count - 1; i >= 0; i--)
        {
            string theStr = highLightedPathList[i];
            if (string.IsNullOrWhiteSpace(theStr))
            {
                highLightedPathList.RemoveAt(i);
            }
        }
        for (int i = focuseBtnPathList.Count - 1; i >= 0; i--)
        {
            string theStr = focuseBtnPathList[i];
            if (string.IsNullOrWhiteSpace(theStr))
            {
                focuseBtnPathList.RemoveAt(i);
            }
        }
        for (int i = txtList.Count - 1; i >= 0; i--)
        {
            string theStr = txtList[i];
            if (string.IsNullOrWhiteSpace(theStr))
            {
                txtList.RemoveAt(i);
            }
        }
        curIndex = 0;


        //obj_finger.transform.position = obj_focuseBtn.transform.position;
        EventCenter.Remove(TheEventType.FinishTargetSightToCertainObj, FinishTargetSightToCertainObj);
        EventCenter.Register(TheEventType.FinishTargetSightToCertainObj, FinishTargetSightToCertainObj);

        EventCenter.Remove(TheEventType.OnBtnClick, CheckGuiRaycastObjects);
        EventCenter.Register(TheEventType.OnBtnClick, CheckGuiRaycastObjects);

        StartCoroutine(WaitCanvasInit());
        txt_guide.gameObject.SetActive(false);
        notFind = false;
        notFindTimer = 0;
        callBack = theCallBack;

    }

    /// <summary>
    /// 结束指引
    /// </summary>
    void FinishTargetSightToCertainObj(object[] args)
    {
        GameObject obj = args[0] as GameObject;
        if (obj.GetInstanceID() == obj_focuseBtn.GetInstanceID())
        {
            Point();
        }
    }
    /// <summary>
    /// 开始指
    /// </summary>
    void StartFocuse()
    {
       
        if(highLightedObj.gameObject.GetComponent<Canvas>()==null)
        highLightedObj.gameObject.AddComponent<Canvas>().overrideSorting = true;
        highLightedObj.gameObject.GetComponent<Canvas>().sortingOrder = 3;
        highLightedObj.gameObject.AddComponent<GraphicRaycaster>();


        if(curGuideType== GuidePointType.DirectPoint)
        {
            Point();
        }
        else
        {
            EventCenter.Broadcast(TheEventType.TargetSightToCertainObj, obj_focuseBtn,true);
        }

    }

    /// <summary>
    /// 点
    /// </summary>
    public void Point()
    {
        int scale = setting.PointScale.ToInt32();
        if (scale == 0)
            scale = 1;
        obj_focus.transform.localScale = new Vector3(scale, scale, scale);

        obj_focus.gameObject.SetActive(true);
        SetFingerPos(obj_focuseBtn);
        obj_circle.transform.localScale = new Vector3(1, 1, 1);
        obj_circle.transform.DOKill();
        obj_circle.transform.DOScale(1.5f, 0.6f).SetLoops(-1);
        if (txtList.Count > 0)
        {
            string txt = txtList[curIndex];
            if (!string.IsNullOrEmpty(txt))
            {
                txt_guide.gameObject.SetActive(true);
                txt_guide.SetText(txt);
            }
            else
            {
                txt_guide.gameObject.SetActive(false);

            }
        }

    }

    IEnumerator WaitCanvasInit()
    {
        yield return new WaitForSeconds(0.1f);
        startSearchingPath = true;
        notFind = true;
        notFindTimer = 0;
    }


    /// <summary>
    /// 设置手指位置
    /// </summary>
    void SetFingerPos(GameObject obj_target)
    {

        obj_focus.transform.position = obj_target.transform.position;
        RectTransform rect = obj_focus.GetComponent<RectTransform>();

        float width = rect.sizeDelta.x;
        float height = rect.sizeDelta.y;
        //左上
        //if(rect.anchorMin==new Vector2(0,1)
        //    &&rect.anchorMax==new Vector2(0, 1)
        //    )
        //{
        //    obj_finger.transform.localPosition += new Vector3(width / 2, -height / 2,0);
        //}
        //右上

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    GameObject obj=GameObject.Find("SceneCanvas/Panel/Mountain")
        //}
        if (startSearchingPath)
        {
     
            string highLightedObjPath = highLightedPathList[curIndex];
            string focuseBtnPath = focuseBtnPathList[curIndex];
            highLightedObj = GameObject.Find(highLightedObjPath);
            obj_focuseBtn = GameObject.Find(focuseBtnPath);
            curGuideType = (GuidePointType)guideTypeList[curIndex];
            if (highLightedObj != null && obj_focuseBtn != null)
            {
                if (highLightedObj.activeInHierarchy && obj_focuseBtn.activeInHierarchy)
                {
                    StartFocuse();
                    startSearchingPath = false;
                    notFind = false;
                }
            
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            //CheckGuiRaycastObjects();
            ////非强制聚焦点一下 该引导直接完成
            //if (setting.forceLocate == "0")
            //{
            //    EventCenter.Broadcast(TheEventType.AccomplishNewGuide, (NewGuideIdType)setting.id.ToInt32());

            //}
        }
 
        if (notFind)
        {
            notFindTimer += Time.deltaTime;
            if (notFindTimer >= notFindTime)
            {
                Close();
            }
        }
    }



    public void OnPointerClick(PointerEventData eventData)
    {
  
    }




    /// <summary>
    /// 是否点击在空白区域
    /// </summary>
    /// <returns></returns>
    public void CheckGuiRaycastObjects()
    {
        if (obj_focuseBtn == null)
            return ;
        //当执行了一次之后  本次鼠标点击就不再执行此方法
        //return false;
        //获取到所有鼠标射线检测到的UI
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, list);


        for(int i = 0; i < list.Count; i++)
        {
            if(list[i].gameObject.GetInstanceID()== obj_focuseBtn.gameObject.GetInstanceID())
            {
                //obj_focuseBtn.GetComponent<Button>().onClick.Invoke();
                DestroyImmediate(highLightedObj.GetComponent<GraphicRaycaster>());
                DestroyImmediate(highLightedObj.GetComponent<Canvas>());
                curIndex++;

                if (curIndex < focuseBtnPathList.Count)
                {
                    startSearchingPath = true;
                    notFind = true;
                    notFindTimer = 0;
                }
                //关闭
                else
                {
                    Close();
                }
                return  ;

            }
        }

        Debug.Log("应该关闭！");
        //PanelMgr.Instance.ClosePanel(this);
        //closeAction?.Invoke();
        return  ;
    }


    public void Close()
    {
        if (highLightedObj != null)
        {
            DestroyImmediate(highLightedObj.GetComponent<GraphicRaycaster>());
            DestroyImmediate(highLightedObj.GetComponent<Canvas>());
        }
   
        GameTimeManager.Instance.DeTimeBlockCount();
        Debug.Log("NewGuideCanvas取消时停"+setting.Id);
        obj_focus.transform.localScale = new Vector3(1, 1, 1);
        obj_focus.SetActive(false);
        //引导觉醒炼器技能
        if (setting.Id.ToInt32() == 10002||setting.Id.ToInt32()==10003)
        {
            PanelManager.Instance.OpenGuideSlideTalentCanvas(focuseBtnPathList[0]);
        }
        setting = null;
        ObjectPoolManager.Instance.DisappearObjectToPool(ObjectPoolSingle.NewGuideCanvas, this.gameObject, true);
        notFind = false;
        notFindTimer = 0;
        callBack?.Invoke();

        EventCenter.Remove(TheEventType.OnBtnClick, CheckGuiRaycastObjects);

    }
}

/// <summary>
/// 指引类型 是直接指还是移动地图
/// </summary>
public enum GuidePointType
{
    None=0,
    DirectPoint=1,
    MoveScroll=2,
}