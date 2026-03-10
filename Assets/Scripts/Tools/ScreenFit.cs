using UnityEngine;
using UnityEngine.UI;

public class ScreenFit : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public float width;
    public float height;
    public bool testMode;
    // Start is called before the first frame update
    bool initOk = false;

    private void Awake()
    {
   
    }
    //public void Init()
    //{
    //    width = Screen.width;
    //    height = Screen.height;

    //    //高屏
    //    if (height / width > 16 / 9)
    //    {
    //        canvasScaler.matchWidthOrHeight = 0;
    //    }
    //    else
    //    {
    //        canvasScaler.matchWidthOrHeight = 1;

    //    }
    //    initOk = true;
    //}
    void Start()
    {
        //width = Screen.width;
        //height = Screen.height;

        ////高屏
        //if (height / width > 16 / 9)
        //{
        //    GetComponent<RectTransform>().sizeDelta = new Vector2(1080, 1080 * height / width);
        //    canvasScaler.matchWidthOrHeight = 0;
        //}
        //else
        //{
        //    GetComponent<RectTransform>().sizeDelta = new Vector2(1080 * width / height, 1920);
        //    canvasScaler.matchWidthOrHeight = 1;
        //}
        //initOk = true;
        Refresh();
    }

    void Refresh()
    {
#if UNITY_STANDALONE && !UNITY_EDITOR
        Screen.SetResolution(1080, 1920, true);
#else
        width = Screen.width;
        height = Screen.height;

        //高屏
        if (height / width > 16 / 9)
        {
            //Debug.Log("高宽比大于16/9 为" + height / width);

            //GetComponent<RectTransform>().sizeDelta = new Vector2(1080, 1080 * height / width);
            if(canvasScaler.matchWidthOrHeight!=0)
                canvasScaler.matchWidthOrHeight = 0;
        }
        else
        {
            //Debug.Log("高宽比小于等于16/9 为" + height / width);
            //GetComponent<RectTransform>().sizeDelta = new Vector2(1080 * width / height, 1920);
            if (canvasScaler.matchWidthOrHeight != 1)
                canvasScaler.matchWidthOrHeight = 1;
        }
#endif
        //initOk = true;
        //Debug.Log(GetComponent<RectTransform>().sizeDelta);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!testMode)
        //return;
        //if (initOk)
        //    return;
        Refresh();
        return;
        //width = Screen.width;
        //height = Screen.height;
        
        ////高屏
        //if (height / width > 16 / 9)
        //{
        //    if (canvasScaler.matchWidthOrHeight != 0)
        //    {
        //        initOk = false;
        //    }
        //    else
        //    {
        //        initOk = true;
        //    }
        //}
        //else
        //{
        //    if (canvasScaler.matchWidthOrHeight != 1)
        //    {
        //        initOk = false;
        //    }
        //    else
        //    {
        //        initOk = true;
        //    }
        //}
        ////如非测试阶段 则打开
        //if (!initOk)
        //{
        //    Refresh();
     
        //}
         
    }
}
