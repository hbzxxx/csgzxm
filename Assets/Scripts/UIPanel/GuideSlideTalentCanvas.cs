
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 仅为滑动天赋做的canvas
/// </summary>
public class GuideSlideTalentCanvas : MonoBehaviour
{
    public GraphicRaycaster RaycastInCanvas;
    EventSystem eventSystem;

    bool startSearchingPath;//找聚焦物品的路径
    public GameObject obj_focus;//聚焦手指
    public GameObject highLightedObj;//高亮
    public string thePath;

    public void Init(string path)
    {
        thePath = path;
        startSearchingPath = false;
        this.GetComponent<Canvas>().worldCamera = Camera.main;
        eventSystem = EventSystem.current;
        obj_focus.gameObject.SetActive(false);

        GameTimeManager.Instance.AddTimeBlockCount();
        Debug.Log("GuideSlideTalentCanvas调用时停"+ path);
        StartCoroutine(WaitCanvasInit());
        EventCenter.Remove(TheEventType.EndowTalent, OnEndowTalent);
        EventCenter.Register(TheEventType.EndowTalent, OnEndowTalent);

    }


    IEnumerator WaitCanvasInit()
    {
        yield return new WaitForSeconds(0.1f);
        startSearchingPath = true;
    }

    /// <summary>
    /// 开始指
    /// </summary>
    void StartFocuse()
    {

        if (highLightedObj.gameObject.GetComponent<Canvas>() == null)
            highLightedObj.gameObject.AddComponent<Canvas>().overrideSorting = true;
        highLightedObj.gameObject.GetComponent<Canvas>().sortingOrder = 3;
        highLightedObj.gameObject.AddComponent<GraphicRaycaster>();

        Point();

    }
    /// <summary>
    /// 点
    /// </summary>
    public void Point()
    {
        int scale = 1;
        obj_focus.transform.localScale = new Vector3(scale, scale, scale);

        obj_focus.gameObject.SetActive(true);
        SetFingerPos(highLightedObj);
   
    }

    /// <summary>
    /// 设置手指位置
    /// </summary>
    void SetFingerPos(GameObject obj_target)
    {

        obj_focus.transform.position = obj_target.transform.position;

    }
    // Update is called once per frame
    void Update()
    {

        if (startSearchingPath)
        {
            highLightedObj = GameObject.Find(thePath);
            if (highLightedObj != null)
            {
                StartFocuse();
                startSearchingPath = false;
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            CheckGuiRaycastObjects();

        }

    }

    /// <summary>
    /// 是否点击在空白区域
    /// </summary>
    /// <returns></returns>
    public bool CheckGuiRaycastObjects()
    {
        if (highLightedObj == null)
            return false;
        //当执行了一次之后  本次鼠标点击就不再执行此方法
        //return false;
        //获取到所有鼠标射线检测到的UI
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, list);


        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].gameObject.GetInstanceID() == highLightedObj.gameObject.GetInstanceID())
            {
                highLightedObj.GetComponent<Button>().OnPointerDown(eventData);
 
                return true;

            }
        }


        return true;
    }

    public void Close()
    {
        Destroy(highLightedObj.GetComponent<GraphicRaycaster>());
        Destroy(highLightedObj.GetComponent<Canvas>());
        GameTimeManager.Instance.DeTimeBlockCount();
        Debug.Log("GuideSlideTalentCanvas取消时停" + thePath);
        obj_focus.transform.localScale = new Vector3(1, 1, 1);
        obj_focus.SetActive(false);
        EventCenter.Remove(TheEventType.EndowTalent, OnEndowTalent);
        ObjectPoolManager.Instance.DisappearObjectToPool(ObjectPoolSingle.GuideSlideTalentCanvas, this.gameObject, true);
    }
    void OnEndowTalent(object[] args)
    {
        Close();
    }
}
