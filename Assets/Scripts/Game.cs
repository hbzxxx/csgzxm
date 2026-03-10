using DG.Tweening;
using Framework.Data;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
 using System.Threading;
    

public class Game : MonoBehaviour
{
    string loginKey = "VnyRLcBhGpJ0fGX6";
    public string lechenGuid="";
 
    public bool openGame;
    public bool gameSpeedAdd;//gm加速游戏
    public static Game Instance;
    public ScenePanel scenePanel;//场景面板
    public bool openNewGuide;//打开新手引导
    public bool jiHuoPackage;//激活码的包

    float miliTimer = 0;
    float miliTime = 60;//60秒刷新一次时间相关
    float secondTimer = 0;
    float secondTime = 1;//1秒刷新一次
     string talkingDataAppId = "55144104208640ADA70C8574F80692F6";

    public bool errBeng;//errobeng
    public bool startErrBengCountDown;
    public float errBengTimer;
    public float errBengTime;

    public bool gameEntered;
    public bool useFuWuQi;//用服务器
    public bool isRightEdition;//是对的版本
    public bool canEnterGame;//能进游戏
    public int initPort = 6670;//todo配表初始服
    public bool onSuccessSDKLogin;
    public int curServerIndex;//当前区服
    public bool tableLoaded = false;
     public bool isLogin = false;//判断是否
    public bool neiCeFu = false;//是内测服
    float conveyTime = 0;
    float conveyTimer = 0;
    bool startConvey;
    int readyToConnectServerIndex;
    
     public bool banHaoMode;

    //public Text txt;
    private void Awake()
    {
        if (!openGame)
            return;
        Instance = this;
        RoleManager.Instance.CheckArchive();
        SDKManager.Instance.Init();
        PanelManager.Instance.Init();

        // 配置表加载提前到注册之前，否则敏感词检查会报错
        if (!tableLoaded)
        {
            LoadTable();
        }

        PanelManager.Instance.OpenLoginPanel();

        //if (openGame)
        //StartCoroutine(CopyFromStreamToPersist(TableOK));

        //int num = 4;
        //int targetnum = 30;

        //DOTween.To(()=>num, (x) =>

        //{
        //    num=x ;
        //    txt.text = x.ToString();
        //} , targetnum, 4);
        miliTimer = 0;
 
    }

 

    //int initNum = 10;//内存中的值
    //int verifyCode = 3;//验证

    //int DeItemNum(int num)
    //{
    //    initNum -= num;
    //    int res = initNum + verifyCode;
    //    Verify(num);
    //    return res;
    //}

    //void Verify(int num)
    //{
    //    int theNum = initNum - num + verifyCode;
    //    if (theNum != initNum)
    //    {
    //        Debug.Log("作弊了");
    //    }
    //}

    /// <summary>
    /// 开始登录
    /// </summary>
    public void StartLogin(string guid="",int archiveIndex=0)
    {
        //检查有无存档
        EventCenter.Broadcast(TheEventType.StartLogin);
        curServerIndex = archiveIndex;
       
         
        if (!useFuWuQi)
        {
            if (RoleManager.Instance._CurGameInfo.IsFeng)
            {
                PanelManager.Instance.OpenFloatWindow("已被封号，无法进入游戏");
            }
            else
            {
                canEnterGame = true;
                EventCenter.Broadcast(TheEventType.SuccessLogIn);
            }

            //StartCoroutine(CopyFromStreamToPersist(TableOK));
        }
        else
        {

            //#if TOPONSDK
            //           TapSDKManager.Instance.Init();
            //#else
            //           clientManager.SendRT("Login", RoleManager.Instance._CurGameInfo.theGuid, RoleManager.Instance._CurGameInfo.nickName, "", false);

            //#endif
            if (guid != "")
            {
                RoleManager.Instance._CurGameInfo.TheGuid = guid;
            }
            else
            {
                RoleManager.Instance._CurGameInfo.TheGuid = Guid.NewGuid().ToString();
            }
            Debug.Log("guid是" + RoleManager.Instance._CurGameInfo.TheGuid+"用该guid登录maofuwuqi"); 
             
        }
    }

    //public void TheTest()
    //{
    //    string txt="";
    //    StreamReader sr = new StreamReader(@"C:\Users\Administrator\Documents\Tencent Files\370365688\FileRecv\文本.txt");

    //    while (!sr.EndOfStream)
    //    {
    //        string str = sr.ReadLine();
    //        str.Replace(Convert.ToChar(10).ToString(), "\\n");
    //        str.Replace(Convert.ToChar(13).ToString(), "\\n");

    //        txt += str;
    //    }
    //    Debug.Log(txt);
    //    System.IO.File.WriteAllText(@"C:\Users\Administrator\Documents\Tencent Files\370365688\FileRecv\文本转换.txt", txt, Encoding.UTF8);


    //}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadTable()
    {
        Debug.Log("开始游戏");
        StartCoroutine(CopyFromStreamToPersist(TableOK));
    }
 
    // Update is called once per frame
    void Update()
    {
            #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            // 调试用，按W键可以触发注册
           MyHttpServer.Instance.RechargeBtnClick(1);
        }
#endif
        //时间相关每分钟刷一次
        if (RoleManager.Instance.initOk)
        {
            miliTimer += Time.deltaTime;
            if (miliTimer >= miliTime)
            {
                GameTimeManager.Instance.RealityTimeProcess();
                miliTimer = 0;
            }
            secondTimer += Time.deltaTime;
            if (secondTimer >= secondTime)
            {
                GameTimeManager.Instance.RealitySecondProcess();
                secondTimer = 0;

            }
        
        }
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    Connect(1);
        //}
        if (isLogin)
        {
            if (conveyTime <= 0)
            {
                conveyTime = RandomManager.Next(60, 180);
            }
            conveyTimer += Time.deltaTime;
            if (conveyTimer >= conveyTime)
            {
                conveyTime = RandomManager.Next(60, 180);
                conveyTimer = 0;
             }
        }
#if !UNITY_EDITOR

        //if (errBeng)
        //{
        //    startErrBengCountDown = true;
        //    errBengTime = RandomManager.Next(100, 1000)*0.01f;
        //    errBeng = false;
        //}
        //if (startErrBengCountDown)
        //{
        //    errBengTimer += Time.deltaTime;
        //    if (errBengTimer >= errBengTime)
        //    {
        //        int theI=0;
        //        while (true)
        //        {
        //            theI++;
        //        }
        //    }
        //}
#endif

        //if (Input.GetKeyDown(KeyCode.Q)) 
        //{ 
        //    var field = typeof(Net.Common.ObscuredPropertyObserver<ulong>).GetField("valueAtkKey", BindingFlags.Instance | BindingFlags.NonPublic);
        //    field.SetValue(NeiCunModel.Instance._ItemNeiCunModel.itemDataList[0].theCount, (long)123);
        //    var v = NeiCunModel.Instance._ItemNeiCunModel.itemDataList[0].theCount.Value;
        //}
    }

    //void TestThread()
    //{
    //    lock (this)
    //    {
    //        for (int i = 0; i < 100; i++)
    //    {
           
    //            MatchManager.Instance.Mimic();



    //        }
    //    }
    //}


    /// <summary>
    /// tableok了 这里是初始化总入口
    /// </summary>
    public void TableOK()
    {
        DataTable.LoadTableData();
        DataTable.LoadSOData();
        //RedPointManager.Instance.Init();
        //RoleManager.Instance.Init(-1);
        ////SocializationManager.Instance.Init();
        ////GameModuleManager.Instance.Init();
        ////GameTimeManager.Instance.Init();
        ////AchievementManager.Instance.Init();
        //GameTimeManager.Instance.Init();
        Debug.Log("TalkingData init begin ");
        TalkingDataGA.OnStart(talkingDataAppId, "TalkingData");
        Debug.Log("TalkingData  init completed ");
        //UnitManager.Instance.Init();
 
 
 
        scenePanel.Init();
 
        //FangChenMiManager.Instance.Init();
        //Test();

        if (jiHuoPackage)
        {

        }
        EventCenter.Broadcast(TheEventType.TableOK);
    }

    /// <summary>
    /// 游戏入口
    /// </summary>
    public void StartGame(int archiveIndex)
    {
        //if(archiveIndex!=-1)
        //    ArchiveManager.Instance.LoadArchive(archiveIndex);
        bool newPlayer = false;
        if (ArchiveManager.Instance.allArchiveList[archiveIndex] == null)
        {
            newPlayer = true;
        }
// #if UNITY_EDITOR
//         if(!newPlayer&& ArchiveManager.Instance.allArchiveList[archiveIndex].QuIndex != archiveIndex)
//         {
//             PanelManager.Instance.OpenFloatWindow("区服信息与该存档不一致！");
//             return;
//         }
// #else
//         RoleManager.Instance._CurGameInfo.QuIndex = archiveIndex;
// #endif
        RoleManager.Instance._CurGameInfo.quIndex = archiveIndex;

        ArchiveManager.Instance.curArchiveIndex = archiveIndex;

        RoleManager.Instance.Init(archiveIndex);
        GameTimeManager.Instance.Init();

        ItemData item = new ItemData();
        item.settingId = 10001;
        item.count = 2;
 
        SceneType initSceneType = SceneType.Mountain; 
        switch ((SceneType)RoleManager.Instance._CurGameInfo.SceneData.CurSceneType)
        {
            case SceneType.MiJingExplore:
                break;
            case SceneType.Battle:
                initSceneType = SceneType.Mountain;
                break;
            default:
                initSceneType = (SceneType)RoleManager.Instance._CurGameInfo.SceneData.CurSceneType;
                break;
        }

        GameObject videoObj = GameObject.Find("SceneCanvas/fengMian/RawImage");
        if (videoObj != null)
            GameObject.Destroy(videoObj);

        // 新存档时跳过打开TopPanel和MainPanel，等SetNamePanel完成后再打开
        bool isNewArchive = ArchiveManager.Instance.allArchiveList[archiveIndex] == null;
        if (isNewArchive)
        {
            GameSceneManager.Instance.skipOpenTopMainPanel = true;
        }

        GameSceneManager.Instance.GoToScene(initSceneType, false);
        //任务
        if (isNewArchive)
        {
            // 新存档：先打开 SetNamePanel，由 SetNamePanel.OnConfirm 调用 CheckIfNPCAppear
            PanelManager.Instance.OpenPanel<SetNamePanel>(PanelManager.Instance.trans_layer2);
        }
        else
        {
            // 老存档：直接检查 NPC 出现
            TaskManager.Instance.CheckIfNPCAppear();
        }
        RoleManager.Instance.InitOffLineIncome();
        RoleManager.Instance.SendTouXiangAndKuangInfoToServer();
 
        if (isLogin)
        {
#if !UNITY_EDITOR
            AddQQManager.Instance.CallAndroidMethod("OnSendRoleData", "enterServer", RoleManager.Instance._CurGameInfo.TheGuid, RoleManager.Instance._CurGameInfo.AllZongMenData.ZongMenName,
                RoleManager.Instance._CurGameInfo.AllZongMenData.ZongMenLevel,  (curServerIndex - 2).ToString(), M_ServerData.fuNameList[curServerIndex], (int)ItemManager.Instance.FindItemCount((int)ItemIdType.TianJing));
#endif
        }

        gameEntered = true;
        //TaskManager.Instance.CheckIfNPCAppear();
    }

    /// <summary>
    /// 判断持久化目录有没有version，如果没有说明还没复制过， 把streamingasset的文件放到持久化目录
    /// </summary>
    public IEnumerator CopyFromStreamToPersist(Action okCallBack)
    {
        //对比 流目录version/服务器version 若一样 则拷贝version和所有table到持久化目录，然后不需要任何下载 进游戏完事了


        //若流目录version和服务器version不一样 则直接对比持久化目录和服务器version（这里先做判断，如果持久化目录没有version，则将流目录的version和table全部放到持久化目录，再对比持久化目录version和服务器version下载））


 
        //FileInfo fileInfo = new FileInfo(PathConstant.GetVersionPersistentPath());
        //持久化目录里面没有，需要复制 
        //if (!fileInfo.Exists)
        //{
        //先复制version 再复制version里面提到的
        FileInfo StreamfileInfo = new FileInfo(ConstantVal.GetVersionStreamPath());

        List<string> streamResPathList = new List<string>();//流目录下所有资源的位置
        List<string> codeList = new List<string>();//流目录下所有资源的code

        //Debug.Log("尝试从流目录加载资源" + PathConstant.GetVersionStreamPath());
        WWW www = new WWW(ConstantVal.GetVersionStreamPath());
        yield return www;

        if (www.isDone)
        {
            //Debug.Log("流目录加载version成功！");

            using (var reader = new StringReader(www.text))
            {
                //s = reader.ReadToEnd();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string theKey = ConstantVal.mm;

                    //解密
                    line = CommonUtil.DecryptDES(line, theKey);
                    var fields = line.Split(':');
                 
                    if (fields.Length > 1)
                    {
                        streamResPathList.Add(fields[0]);
                        codeList.Add(fields[1]);
                    }
                }
            }
            //拷贝数据到指定路径
            File.WriteAllBytes(ConstantVal.GetVersionPersistentPath(), www.bytes);
            //Debug.Log("拷贝version到指定路径成功");
        }
        System.Security.Cryptography.MD5CryptoServiceProvider md5Generator = new System.Security.Cryptography.MD5CryptoServiceProvider();

        for (int i = 0; i < streamResPathList.Count; i++)
        {
            //这些path从version里面取
            string path = ConstantVal.GetFileInStreamPath(streamResPathList[i]);
            //"file://"+ Application.streamingAssetsPath + "/" + streamResPathList[i];
            string targetPath = Application.persistentDataPath + "/" + streamResPathList[i];

            DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath + "/res/DataTable");
            if (!info.Exists)
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/res/DataTable");

                // Debug.Log("创建datatable到持久化目录成功"+ Application.persistentDataPath + "/res/DataTable");

            }

            // Debug.Log("尝试从流目录加载" + path);

            WWW www2 = new WWW(path);
            //Debug.Log("流目录加载文件成功" + path);
            yield return www2;

            if (www2.isDone)
            {
                // 检查 WWW 请求是否成功
                if (!string.IsNullOrEmpty(www2.error) || www2.bytes == null || www2.bytes.Length == 0)
                {
                    Debug.LogError("文件加载失败: " + path + " 错误: " + www2.error);
                    Game.Instance.errBeng = true;
                    continue;
                }
                byte[] hash = md5Generator.ComputeHash(www2.bytes);
                string strMD5 = System.BitConverter.ToString(hash);
                strMD5 = strMD5.Replace("-", ""); //把“-”去掉
                if (strMD5 != codeList[i])
                {
                    Debug.LogError("文件出错！" + path);
                    Game.Instance.errBeng = true;
                }
                else
                {
                    File.WriteAllBytes(targetPath, www2.bytes);

                }

                //对比
            }
        }
        Debug.Log("所有文件加载成功！");

        ////高级存档
        //WWW www3 = new WWW(ConstantVal.GetSpecialArchiveStreamPath());
        //yield return www3;
        //if (www3.isDone)
        //{
        //    //Debug.Log("流目录加载version成功！");
        //    DirectoryInfo destination = new DirectoryInfo(ConstantVal.GetArchiveSaveFolder(0));
        //    if (!destination.Exists)
        //    {
        //        destination.Create();
        //    }

        //    //拷贝数据到指定路径
        //    File.WriteAllBytes(ConstantVal.GetSpecialArchivePersistentPath(), www3.bytes);

        //}


        //}
        //持久化目录有，不复制，直接用持久化目录的version
        //else
        //{
        //    FileInfo fileInfo3 = new FileInfo(PathConstant.GetVersionPersistentPath());
        //    if (fileInfo3.Exists)
        //    {
        //        StreamReader r = new StreamReader(PathConstant.GetVersionPersistentPath());
        //        s = r.ReadToEnd();
        //    }
        //}

        //暂时不用加载服务器versino
        // StartCoroutine(LoadVersions(s));
        tableLoaded = true;
        if (okCallBack != null)
            okCallBack();
    }
    /// <summary>
    /// sdk登录成功
    /// </summary>
    /// <param name="msg"></param>
    public void OnSuccessSDKLogin(string msg )
    {
         //登录成功
        lechenGuid = msg;
 
        onSuccessSDKLogin = true;
        //EventCenter.Broadcast(TheEventType.SuccessSDKLogin);
        bool haveArchive = false;

 
        if (Game.Instance.banHaoMode)
        {
            curServerIndex = 1;
        }
        if (ArchiveManager.Instance.recentArchive != null)
        {
            for (int i = 0; i < ArchiveManager.Instance.allArchiveList.Count; i++)
            {

                if (ArchiveManager.Instance.recentArchive != null
                    && ArchiveManager.Instance.recentArchive == ArchiveManager.Instance.allArchiveList[i])
                {
                    //编辑器 用老存档的guid
#if UNITY_EDITOR
                    if (!string.IsNullOrWhiteSpace(ArchiveManager.Instance.recentArchive.TheGuid))
                    {
                        string[] theGuidArr = ArchiveManager.Instance.recentArchive.TheGuid.Split('_');
                        if (theGuidArr.Length == 1)
                            lechenGuid = theGuidArr[0];
                        else
                            lechenGuid = theGuidArr[1];
                    }

#endif
                    curServerIndex = i;
                }

            }
        }
           
                 FinishCkeckEdition();

        //clientManager.port = initPort + curServerIndex;
        //clientManager.Connect();
        //如果有号 则显示最近的号 如果没号 则默认最新服务器 作为存档index

        //StartLogin(msg);
    }

    public void RequestBack(string keyData)
    {
        Debug.Log(keyData);
        string[] res = keyData.Split('%');
        if(res[0]== "GMOrder_ClientGetItem")
        {
            Debug.Log(res[1]);
        }
    }
     
    /// <summary>
    /// 下载新版本
    /// </summary>
    public async void GoToDLNewEdition()
    {
        Application.OpenURL("https://www.taptap.cn/app/378861");
        //bool isSuccess = await TapCommon.UpdateGameAndFailToWebInTapTap("378861");
    }


    /// <summary>
    /// 结束版本更新 可以进入游戏
    /// </summary>
    void FinishCkeckEdition()
    {
        EventCenter.Broadcast(TheEventType.FinishCheckEdition);
    }

 
 

    /// <summary>
    /// 安卓端命令floatwindow
    /// </summary>
    public void OpenFloatWindowFromAndroid(string str)
    {
        PanelManager.Instance.OpenFloatWindow(str);
    }
 
    /// <summary>
    /// 得到广告奖励
    /// </summary>
    public void OnGetADReward(string str)
    {
        
        ADManager.Instance.finishWathAD = true;
    }

 

}
