using Framework.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

public class PanelManager : MonoInstance<PanelManager>
{
    Dictionary<ObjectPoolSingle, PanelBase> panelDic = new Dictionary<ObjectPoolSingle, PanelBase>();
    List<PanelBase> panelList = new List<PanelBase>();

    public Transform trans_commonPanelParent;//场景等适用于当前游戏模块的
    public Transform trans_layer2;//状态栏 菜单栏等即使切换场景也常显的
    public Transform trans_layer3;//提示框等需要置顶的

    public Transform trans_sceneLayer;//场景层
    public Material mat_grey;

    public YieldShowInMainPanelType curYieldShowInMainPanelType;//跳回到主界面应该显示的

    public TaskGuidePanel curTaskGuidePanel;//当前任务指引

    public ZhanDouLiChangeShowPanel curZhanDouLiChangeShowPanel;

    public TongZhiPanel tongZhiPanel;//通知
    public DanMuPanel danmuPanel;//弹幕
    public OfflineIncomePanel offlineIncomePanel;

    public ThirtyDayQianDaoPanel thirtyDayQianDaoPanel;//30天签到panel

    public bool blockHintPanel = false;//是否屏蔽提示弹窗（挖矿玩法等特殊场景使用）

    public bool dialogDirectionLeft = true;
    public StartPanel loginPanel;
    public override void Init()
    {
        base.Init();
        trans_commonPanelParent = GameObject.Find("Canvas/Panel/Layer1").transform;
        trans_layer2 = GameObject.Find("Canvas/Panel/Layer2").transform;
        trans_layer3 = GameObject.Find("Canvas/Panel/Layer3").transform;
        trans_sceneLayer = GameObject.Find("SceneCanvas/Panel").transform;

        mat_grey = ResourceManager.Instance.GetObj<Material>("Material/UI_ImageGreyShader");

    }

    /// <summary>
    /// 清掉layer2所有panel
    /// </summary>
    public void ClearLayer2Panel()
    {
        PanelManager.Instance.CloseAllPanel(trans_layer2);

    }

    /// <summary>
    /// 直接获取面板 最好是用事件系统
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public PanelBase GetPanel(ObjectPoolSingle singleType)
    {
        if (panelDic.ContainsKey(singleType))
            return panelDic[singleType];
        return null;
    }
    /// <summary>
    /// 直接获取面板 最好是用事件系统
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public T GetPanel<T>() where T:PanelBase
    {
        for(int i = 0; i < panelList.Count; i++)
        {
            PanelBase panel = panelList[i];
            if(panel is T)
            {
                return panel as T;
            }
        }
        return null;
        //if (panelDic.ContainsValue(T))
        //    return panelDic[singleType];
        //return null;
    }

    /// <summary>
    /// 打开一个小面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent">父对象</param>
    /// <param name="args">参数</param>
    public T OpenSingle<T>(Transform parent, params object[] args) where T : SingleViewBase
    {
        if (parent == null)
        {
            Debug.Log("在无父物体的位置创建single！！！"+ typeof(T).ToString());
        }
            //return null;

       

        string typeName = typeof(T).ToString();

        string path = ConstantVal.GetPanelPath(typeName);//mao 获取panel路径
        // GameObject plobj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        bool available = Enum.TryParse<ObjectPoolSingle>(typeName, out ObjectPoolSingle singleType);
        if (!available)
        {
            Debug.LogError("没有定义该类型的对象池枚举" + typeName);
        }
        GameObject plobj = ObjectPoolManager.Instance.GetObjcectFromPool(singleType, path, true);
        plobj.transform.SetParent(parent, false);
        plobj.name = typeName;
        T t = plobj.GetComponent<T>();
        if (null == t)
            t = plobj.AddComponent<T>();
        t.objType = singleType;
        t.isTmpObj = true;
        t.obj = plobj;
        t.Clear();
        t.Init(args);
        t.OnOpenIng();
        return t;
    }
    /// <summary>
    /// 打开一个小面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent">父对象</param>
    /// <param name="args">参数</param>
    public SingleViewBase OpenSingle(ObjectPoolSingle type, Transform parent, params object[] args)
    {
        if (parent == null)
        {
            Debug.Log("在无父物体的位置创建single！！！" + typeof(SingleViewBase).ToString());
        }
        //return null;



        string typeName = Enum.GetName(typeof(ObjectPoolSingle), type);

        string path = ConstantVal.GetPanelPath(typeName);//mao 获取panel路径
        // GameObject plobj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        bool available = Enum.TryParse<ObjectPoolSingle>(typeName, out ObjectPoolSingle singleType);
        if (!available)
        {
            Debug.LogError("没有定义该类型的对象池枚举" + typeName);
        }
        GameObject plobj = ObjectPoolManager.Instance.GetObjcectFromPool(singleType, path, true);
        plobj.transform.SetParent(parent, false);
        plobj.name = typeName;
        SingleViewBase t = plobj.GetComponent<SingleViewBase>();
        if (null == t)
            t = plobj.AddComponent<SingleViewBase>();
        t.objType = singleType;
        t.isTmpObj = true;
        t.obj = plobj;
        t.Clear();
        t.Init(args);
        t.OnOpenIng();
        return t;
    }
    /// <summary>
    /// 打开一个面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent">父对象</param>
    /// <param name="args">参数</param>
    public PanelBase OpenPanel(ObjectPoolSingle type, Transform parent, params object[] args)
    {

        string path = ConstantVal.GetPanelPath(type.ToString());//mao 获取panel路径
        // GameObject plobj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        //ObjectPoolSingle res=ObjectPoolSingle.None;



        GameObject plobj = ObjectPoolManager.Instance.GetObjcectFromPool(type, path, true);
        plobj.transform.SetParent(parent, false);
        plobj.name = type.ToString();
        PanelBase t = plobj.GetComponent<PanelBase>();
        if (null == t)
            t = plobj.AddComponent<PanelBase>();
        t.objType = type;
        t.isTmpObj = true;
        t.obj = plobj;
        t.Clear();
        t.Init(args);
        t.OnOpenIng();

        if (!panelDic.ContainsKey(type))
        {
            panelDic.Add(type, t);
        }

        if (!panelList.Contains(t))
        {
            panelList.Add(t);
        }
        ArrangeAllPanel(parent);
        return t;
    }


    /// <summary>
    /// 打开一个面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent">父对象</param>
    /// <param name="args">参数</param>
    public T OpenPanel<T>(Transform parent, params object[] args) where T : PanelBase
    {
        if (parent == null)
            return null;

        string typeName = typeof(T).ToString();

        string path = ConstantVal.GetPanelPath(typeName);//mao 获取panel路径
        // GameObject plobj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        //ObjectPoolSingle res=ObjectPoolSingle.None;
        bool available = Enum.TryParse<ObjectPoolSingle>( typeName,out ObjectPoolSingle singleType);
        //res
        //ObjectPoolSingle singleType = (ObjectPoolSingle)Enum.Parse(typeof(ObjectPoolSingle), typeName);
        if (!available)
        {
            Debug.LogError("没有定义该类型的对象池枚举" + typeName);
        }
        GameObject plobj = ObjectPoolManager.Instance.GetObjcectFromPool(singleType, path, true);
        plobj.transform.SetParent(parent, false);
        plobj.name = typeName;
        T t = plobj.GetComponent<T>();
        if (null == t)
            t = plobj.AddComponent<T>();
        t.objType = singleType;
        t.isTmpObj = true;
        t.obj = plobj;
        t.Clear();
        t.Init(args);
        t.OnOpenIng();

        if (!panelDic.ContainsKey(singleType))
        {
            panelDic.Add(singleType,t);
        }

        if (!panelList.Contains(t))
        {
            panelList.Add(t);
        }
        ArrangeAllPanel(parent);
        return t;
    }


    /// <summary>
    /// 打开一个永久面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent">父对象</param>
    /// <param name="args">参数</param>
    public T OpenPersistentPanel<T>(Transform parent, params object[] args) where T : PanelBase
    {
        if (parent == null)
            return null;

        string typeName = typeof(T).ToString();

        string path = ConstantVal.GetPanelPath(typeName);//mao 获取panel路径
        // GameObject plobj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        //ObjectPoolSingle res=ObjectPoolSingle.None;
        bool available = Enum.TryParse<ObjectPoolSingle>(typeName, out ObjectPoolSingle singleType);
        //res
        //ObjectPoolSingle singleType = (ObjectPoolSingle)Enum.Parse(typeof(ObjectPoolSingle), typeName);
        if (!available)
        {
            Debug.LogError("没有定义该类型的对象池枚举" + typeName);
        }
        GameObject plobj = ObjectPoolManager.Instance.GetObjcectFromPool(singleType, path, false);
        plobj.transform.SetParent(parent, false);
        plobj.name = typeName;
        T t = plobj.GetComponent<T>();
        if (null == t)
            t = plobj.AddComponent<T>();
        t.objType = singleType;
        t.isTmpObj = false;
        t.obj = plobj;
        t.Clear();
        t.Init(args);
        t.OnOpenIng();

        if (!panelDic.ContainsKey(singleType))
        {
            panelDic.Add(singleType, t);
        }

        if (!panelList.Contains(t))
        {
            panelList.Add(t);
        }
        return t;
    }



    /// <summary>
    /// 关闭面板
    /// </summary>
    /// <param name="panelBase"></param>
    public void ClosePanel(PanelBase panelBase)
    {
 
        panelBase.Clear();
        panelBase.RemoveBtnClick();
        panelBase.RemoveEventRegister();
        panelBase.ClearAllSingle();
        if (panelDic.ContainsKey(panelBase.objType))
        {
            panelDic.Remove(panelBase.objType);
        }

        if (panelList.Contains(panelBase))
        {
            panelList.Remove(panelBase);
        }
        EntityManager.Instance.CloseEntity(panelBase);
    }

    /// <summary>
    /// 关闭某个父物体的所有面板
    /// </summary>
    public void CloseAllPanel(Transform trans)
    {
        int count = trans.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            //防套娃
            if (i > trans.childCount - 1)
                i--;
            PanelBase panel = trans.GetChild(i).GetComponent<PanelBase>();
            if (panel != null)
            {

                ClosePanel(panel);

            }
            else
            {
                Debug.LogError("关闭一个不存在的panel！！" + trans);
            }
        }
    }

    /// <summary>
    /// 关闭小面板
    /// </summary>
    /// <param name="panelBase"></param>
    public void CloseSingle(SingleViewBase singleViewBase)
    {
        singleViewBase.Clear();
        singleViewBase.RemoveBtnClick();
        singleViewBase.RemoveEventRegister();
        EntityManager.Instance.CloseEntity(singleViewBase);
    }

    /// <summary>
    /// 关闭某个父物体所有小面板
    /// </summary>
    /// <param name="trans"></param>
    public void CloseAllSingle(Transform trans)
    {
        if (trans == null)
        {
            Debug.LogWarning("CloseAllSingle: trans is null");
            return;
        }
        
        int count = trans.childCount;
        for(int i = count-1; i >= 0; i--)
        {
            Transform child = null;
            try
            {
                child = trans.GetChild(i);
            }
            catch (MissingReferenceException)
            {
                // 子对象已被销毁，跳过
                continue;
            }
            
            if (child == null)
                continue;
                
            SingleViewBase view = child.GetComponent<SingleViewBase>();
            CloseSingle(view);
        }
    }

    /// <summary>
    /// 黑幕降临
    /// </summary>
    public void BlackMask(BlackMaskType blackMaskType,Action finishCallBack)
    {
        BlackMaskPanel blackMaskPanel = GetPanel<BlackMaskPanel>();
        if (blackMaskPanel == null)
        {
            blackMaskPanel= PanelManager.Instance.OpenPanel<BlackMaskPanel>(PanelManager.Instance.trans_layer3, blackMaskType,finishCallBack);

        }
    }
    /// <summary>
    /// 黑幕暗-亮
    /// </summary>
    public void PingPongBlackMask( Action finishCallBack,Action closePanelCallBack)
    {
        BlackMaskPanel blackMaskPanel = GetPanel<BlackMaskPanel>();
        if (blackMaskPanel == null)
        {
            blackMaskPanel = PanelManager.Instance.OpenPanel<BlackMaskPanel>(PanelManager.Instance.trans_layer3, BlackMaskType.PingPong, finishCallBack, closePanelCallBack);

        }
    }

    public void OpenFloatWindow(string str)
    {
        OpenPanel<FloatWindowPanel>(trans_layer3, str);
    }

    /// <summary>
    /// 打开普通弹窗
    /// </summary>
    public void OpenCommonHint(string content, Action okCb, Action cancelCb, string okBtnTxt = "", string cancelBtnTxt = "")
    {
        // 检查并添加标点符号
        if (!string.IsNullOrEmpty(content))
        {
            string trimmedContent = content.Trim();
            if (!trimmedContent.EndsWith("。") && !trimmedContent.EndsWith("！") && !trimmedContent.EndsWith("？"))
            {
                content = trimmedContent + "。";
            }
        }
        
        HintData hintData = new HintData();
        hintData.content = content;
        hintData.okCallBack = okCb;
        hintData.cancelCallBack = cancelCb;
        hintData.str_okBtn = okBtnTxt;
        hintData.str_cancelBtn = cancelBtnTxt;
        OpenPanel<CommonHintPanel>(PanelManager.Instance.trans_layer2, hintData);
    }

    /// <summary>
    /// 打开强制确定弹窗
    /// </summary>
    /// <param name="content"></param>
    /// <param name="okCb"></param>
    public void OpenOnlyOkHint(string content,Action okCb,bool cannotClick=false)
    {
        // 如果屏蔽提示弹窗，直接执行回调并返回
        if (blockHintPanel)
        {
            okCb?.Invoke();
            return;
        }

        // 检查并添加标点符号
        if (!string.IsNullOrEmpty(content))
        {
            string trimmedContent = content.Trim();
            if (!trimmedContent.EndsWith("。") && !trimmedContent.EndsWith("！") && !trimmedContent.EndsWith("？"))
            {
                content = trimmedContent + "。";
            }
        }
        HintData hintData = new HintData();
        hintData.content = content;
        hintData.okCallBack = okCb;
        OpenPanel<OnlyOkHintPanel>(PanelManager.Instance.trans_layer3, hintData, cannotClick);
    }

    /// <summary>
    /// 打开新手引导
    /// </summary>
    /// <returns></returns>
    public void OpenNewGuideCanvas(cfg.NewGuideSetting newGuideSetting,Action callBack=null)
    {
        // DialogId 字段已从配置表移除，直接打开引导
        GameObject obj = ObjectPoolManager.Instance.GetObjcectFromPool(ObjectPoolSingle.NewGuideCanvas, ConstantVal.newGuideCanvasPath, true);
        obj.transform.SetParent(null, false);
        NewGuideManager.Instance.newGuideCanvas = obj.GetComponent<NewGuideCanvas>();
        NewGuideManager.Instance.newGuideCanvas.Init(newGuideSetting,callBack);
    }

    /// <summary>
    /// 滑动引导选择天赋
    /// </summary>
    /// <param name="path"></param>
    public void OpenGuideSlideTalentCanvas(string path)
    {
        GameObject obj = ObjectPoolManager.Instance.GetObjcectFromPool(ObjectPoolSingle.GuideSlideTalentCanvas, ConstantVal.guideSlideCanvasPath, true);
        obj.transform.SetParent(null, false);
        obj.GetComponent<GuideSlideTalentCanvas>().Init(path);
        //NewGuideManager.Instance.newGuideCanvas = obj.GetComponent<GuideSlideTalentCanvas>();
        //NewGuideManager.Instance.newGuideCanvas.Init(path);
    }

    /// <summary>
    /// 通过权重对该父物体下的所有面板排序
    /// </summary>
    public void ArrangeAllPanel(Transform trans)
    {
        int curNum = trans.childCount;
        List<PanelBase> curPanelList = new List<PanelBase>();
        for(int i=0;i< curNum; i++)
        {
            PanelBase thePanel  = trans.GetChild(i).GetComponent<PanelBase>();
            if(thePanel!=null)
            curPanelList.Add(thePanel);
        }
        curPanelList= CommonUtil.SelectSort<PanelBase>(curPanelList);
        for(int i = 0; i < curPanelList.Count; i++)
        {
            int index = i;
            curPanelList[index].transform.SetSiblingIndex(index);
        }
    }

    /// <summary>
    /// 显示指引面板
    /// </summary>
    public void ShowTaskGuidePanel(GameObject obj)
    {
        if (curTaskGuidePanel != null)
        {
            PanelManager.Instance.ClosePanel(curTaskGuidePanel);
        }
       
        curTaskGuidePanel= PanelManager.Instance.OpenPanel<TaskGuidePanel>(trans_layer2,GuideType.Common, obj.transform.position,obj);
        
    }
    /// <summary>
    /// 显示带文字的指引面板
    /// </summary>
    public void ShowWithTxtTaskGuidePanel(GameObject obj,string txt)
    {
        if (curTaskGuidePanel != null)
        {
            PanelManager.Instance.ClosePanel(curTaskGuidePanel);
        }

        curTaskGuidePanel = PanelManager.Instance.OpenPanel<TaskGuidePanel>(trans_layer2, GuideType.WithTxt, obj.transform.position,obj, txt);

    }
    /// <summary>
    /// 显示带文字带回调的指引面板
    /// </summary>
    public void ShowWithTxtAndCallbackTaskGuidePanel(GameObject obj, string txt,Action callBack)
    {
        if (curTaskGuidePanel != null)
        {
            PanelManager.Instance.ClosePanel(curTaskGuidePanel);
        }

        curTaskGuidePanel = PanelManager.Instance.OpenPanel<TaskGuidePanel>(trans_layer2, GuideType.WithTxt, obj.transform.position, obj, txt , callBack);

    }
    /// <summary>
    /// 关闭指引面板
    /// </summary>
    public void CloseTaskGuidePanel()
    {
        if (curTaskGuidePanel != null)
        {
            PanelManager.Instance.ClosePanel(curTaskGuidePanel);
        }
    }

    public void OpenZhanDouLiChangePanel(long before,long after)
    {
        if (curZhanDouLiChangeShowPanel != null)
            ClosePanel(curZhanDouLiChangeShowPanel);
        if (before == after)
            return;
        curZhanDouLiChangeShowPanel=OpenPanel<ZhanDouLiChangeShowPanel>(trans_layer2, before, after);
    }

    /// <summary>
    /// 打开30天签到面板
    /// </summary>
    public void OpenThirtyDayQianDaoPanel()
    {
        if (thirtyDayQianDaoPanel == null)
            thirtyDayQianDaoPanel = PanelManager.Instance.OpenPanel<ThirtyDayQianDaoPanel>(trans_layer2);
    }
    
    public void AddTongZhi(params object[] args)
    {
        if (tongZhiPanel == null)
        {
            tongZhiPanel = OpenPanel<TongZhiPanel>(trans_layer2, args);
        }
        else
        {
            tongZhiPanel.Init(args);
        }
    }

    /// <summary>
    /// 加弹幕
    /// </summary>
    /// <param name="args"></param>
    public void AddDanMu(params object[] args)
    {
        string content = (string)args[0];
        bool improtant = (bool)args[1];
        if (danmuPanel == null)
        {
            danmuPanel = OpenPanel<DanMuPanel>(trans_layer2, content,improtant);
        }
        else
        {
            danmuPanel.AddDanMu(content, improtant);
        }
    }
    public void ShowUnlock(GameObject obj,GameObject lockObj,UIComponentType type)
    {
        UnlockType unlockType = GetUnlockType(type);
        switch (unlockType)
        {
            case UnlockType.UnShow:
                obj.gameObject.SetActive(false);
                break;
            case UnlockType.Locked:
                obj.gameObject.SetActive(true);
                lockObj.gameObject.SetActive(false);
                break;
            case UnlockType.UnLocked:
                obj.gameObject.SetActive(true);
                lockObj.gameObject.SetActive(true);
                break;
        }
    }

    public UnlockType GetUnlockType(UIComponentType uIComponentType)
    {
        if (!Game.Instance.openNewGuide)
            return UnlockType.UnLocked;



        return UnlockType.None;

    }

    #region 新功能解锁显示
    List<string> waitToShowUnlockFunctionList = new List<string>();
    public void AddUnlockFunction(string functionName)
    {
        waitToShowUnlockFunctionList.Add(functionName);
       
    }
 
    /// <summary>
    /// 关闭新功能解锁
    /// </summary>
    public void CloseUnlockFunction(string functionName)
    {
        if (waitToShowUnlockFunctionList.Contains(functionName))
            waitToShowUnlockFunctionList.Remove(functionName);
    }

    #endregion;

    /// <summary>
    /// 指向某位置
    /// </summary>
    public void LocateScrollAndTaskPoint(ScrollViewNevigation scrollViewNevigation, GameObject obj,bool showFinger=true)
    {
        StartCoroutine(YieldLocateScrollAndTaskPoint(scrollViewNevigation, obj,showFinger));
    }

    IEnumerator YieldLocateScrollAndTaskPoint(ScrollViewNevigation scrollViewNevigation,GameObject obj,bool showFinger=true)
    {
        yield return null;
        scrollViewNevigation.NevigateImmediately(obj.GetComponent<RectTransform>());
        if(showFinger)
        PanelManager.Instance.ShowTaskGuidePanel(obj.gameObject);
    }

    public void CloseOfflineIncomePanel()
    {
        if (offlineIncomePanel != null)
        {
            PanelManager.Instance.ClosePanel(offlineIncomePanel);
            offlineIncomePanel = null;
            
        }
    }
    public void OpenOfflinePanel()
    {
        if (offlineIncomePanel == null)
        {
            offlineIncomePanel= PanelManager.Instance.OpenPanel<OfflineIncomePanel>(trans_layer2);

        }
    }

    public void OpenLoginPanel()
    {
        loginPanel = OpenPanel<StartPanel>(trans_layer2);
    }

    public void CloseLoginPanel()
    {
        if (loginPanel != null)
            ClosePanel(loginPanel);
        loginPanel = null;
    }
}

/// <summary>
/// ui类型
/// </summary>
public enum UIComponentType
{
    None=0,
    MainPanel_home=1,//洞府按钮
    MainPanel_match= 2,//参赛按钮
    MainPanel_archive=3,//存档按钮
    MainPanel_knapsack=4,//背包按钮
    MainPanel_Mountain=5,//山门按钮
    MainPanel_outside=6,//外出按钮
}

/// <summary>
/// 载入主界面后应该显示的内容
/// </summary>
public enum YieldShowInMainPanelType
{
    None=0,
    UnlockNewMatch,//解锁新赛事
    UnlockCloudPanel,//云层解锁
    AfterKillShanHaiZongZhangMen,//击败山海宗掌门后的剧情
    DiShuFirstFarewell,//帝姝第一次告别
    WinLiMaoZhangMen,//打赢狸猫掌门
    LoseLiMaoZhangMen,//输给狸猫掌门
    AfterFirstGuide,//第一个引导过了
    ShowTaoFaUnlock,//解锁讨伐

    //LosePanZongBattle,//在叛宗战中被击败
    //WinPanZongBattle,//在叛宗战中胜利
    //KillShanHanZongZhangLao,//击败山海宗
}
