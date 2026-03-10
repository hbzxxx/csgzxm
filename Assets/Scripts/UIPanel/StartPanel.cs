
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UPLoadFTP;

public class StartPanel : PanelBase
{  
     // 账号登录相关
    public GameObject obj_login; // 登录界面容器
    public InputField input_LoginAccount;    // 登录账号输入框
    public InputField input_LoginPassword;   // 登录密码输入框
    public Button btn_Login;
    public Button btn_logout;
    public Button btn_GoToRegister;          // 前往注册按钮

    public GameObject obj_successfulLogin;

    // 自动登录相关
    private bool isAutoLogging = false;      // 是否正在自动登录




    public Button btn_shiLing;//适龄提示
    public Button btn_downloadArchive;
    public Button btn_newGame;     
    public Button btn_continueGame;
    public Button btn_archiveManager;//存档管理
    public Button btn_uploadArchive;//上传存档
    bool haveArchive;//有存档
    public Text txt_editionTxt;
    public Text txt_playerName;
    public Text txt_quName;
    int choosedQuIndex=-1;
    public Button btn_jianKang;
    public override void Init(params object[] args)
    {
        base.Init(args);

        // 确保密码输入框始终以密文显示，但不影响其真实内容（InputField.text 保留原始值）
        if (input_LoginPassword != null)
        {
            // 设置为密码类型（Unity 会自动用 asteriskChar 显示遮罩）
            input_LoginPassword.contentType = InputField.ContentType.Password;
            // 统一设置掩码字符（可按需要改为'*'或其它符号）
            input_LoginPassword.asteriskChar = '●';
            // 立即刷新显示
            input_LoginPassword.ForceLabelUpdate();
        }

        addBtnListener(btn_jianKang, () =>
        {
            PanelManager.Instance.OpenPanel(ObjectPoolSingle.JianKangPanel, PanelManager.Instance.trans_layer2);
        });

        if (ArchiveManager.Instance.recentArchive != null)
            haveArchive = true;
        else
            haveArchive = false;

        addBtnListener(btn_shiLing, () =>
         {
             PanelManager.Instance.OpenPanel<ShiLingTiShiPanel>(PanelManager.Instance.trans_layer2);
         });
        if (btn_GoToRegister != null)
        {
            addBtnListener(btn_GoToRegister, () =>
            {
                // 关闭登录面板，打开注册面板
                //PanelManager.Instance.ClosePanel(this);
                PanelManager.Instance.OpenPanel(ObjectPoolSingle.RegisterPanel, PanelManager.Instance.trans_layer2);
            });
        }
        // 添加登出按钮事件
        if (btn_logout != null)
        {
            addBtnListener(btn_logout, OnLogoutButtonClick);
        }
        addBtnListener(btn_newGame, () =>
        {
            FangChenMiManager.Instance.canEnter = true;
            if (!FangChenMiManager.Instance.canEnter)
            {
                FangChenMiManager.Instance.Init();
                return;
            }

            if (haveArchive)
            {
                PanelManager.Instance.OpenCommonHint("确定要建立新存档吗，老存档会被覆盖。", () =>
                {
                    Game.Instance.StartGame(Game.Instance.curServerIndex);
                    PanelManager.Instance.ClosePanel(this);

                }, null);
            }
            else
            {

                Game.Instance.StartGame(Game.Instance.curServerIndex);
                PanelManager.Instance.ClosePanel(this);

            }
        });

        addBtnListener(btn_continueGame, () =>
        {
            //if (!FangChenMiManager.Instance.canEnter)
            //{
            //    FangChenMiManager.Instance.Init();
            //    return;
            //}

            //旧存档处理
            GameInfo gameInfo = ArchiveManager.Instance.allArchiveList[Game.Instance.curServerIndex];
            string[] version = gameInfo.VersionName.Split('.');
            int bigVersion = version[0].ToInt32();
            // if (bigVersion < 2)
            // {
            //     PanelManager.Instance.OpenOnlyOkHint("该存档为旧版本存档，与新版本不兼容，请在存档管理中删除。", null);
            // }
            // else
            // {
    
            // }
            PanelManager.Instance.ClosePanel(this);
                Game.Instance.StartGame(Game.Instance.curServerIndex);

        });
        addBtnListener(btn_archiveManager, () =>
        {
            PanelManager.Instance.OpenPanel<ArchivePanel>(PanelManager.Instance.trans_layer2);

        });
        addBtnListener(btn_uploadArchive, () =>
        {
#if UNITY_EDITOR
            PanelManager.Instance.OpenPanel<YunCunDangPanel>(PanelManager.Instance.trans_layer2);

#else

            if (Game.Instance.isLogin)
            {
                PanelManager.Instance.OpenPanel<YunCunDangPanel>(PanelManager.Instance.trans_layer2);
            }
            else
            {
                PanelManager.Instance.OpenFloatWindow("请先登录后使用");
            }
#endif

            //if (haveArchive)
            //{
            //    using (var input = File.OpenRead(ConstantVal.GetArchiveSavePath(ArchiveManager.Instance.recentArchiveIndex)))
            //    {
            //        GameInfo gameInfo = GameInfo.Parser.ParseFrom(input);
            //        UpLoadFiles.OnUploadArchive(ConstantVal.GetArchiveSavePath(ArchiveManager.Instance.recentArchiveIndex), gameInfo.theGuid);

            //    }

            //}

        });

        addBtnListener(btn_downloadArchive, () =>
        {
            //PanelManager.Instance.OpenPanel<DownloadArchivePanel>(PanelManager.Instance.trans_layer2);
        });
        // 添加登录相关按钮事件
        if (btn_Login != null)
        {
            addBtnListener(btn_Login, OnLoginButtonClick);
        }

        RegisterEvent(TheEventType.OnDeleteArchive, OnDeleteArchive);
        RegisterEvent(TheEventType.FinishCheckEdition, OnFinishCheckEdition);
        RegisterEvent(TheEventType.SuccessLogIn, SuccessLogIn);
        RegisterEvent(TheEventType.SuccessSDKLogin, SuccessSDKLogin);
        RegisterEvent(TheEventType.TableOK, InitShow);
        RegisterEvent(TheEventType.StartLogin, OnStartLogin);
        RegisterEvent(TheEventType.ChooseQu, OnRequestChooseQu);
        RegisterEvent(TheEventType.SuccessDownloadArchive, InitShow);

        if (ArchiveManager.Instance.recentArchive != null)
        {
            for (int i = 0; i < ArchiveManager.Instance.allArchiveList.Count; i++)
            {

                if (ArchiveManager.Instance.recentArchive == ArchiveManager.Instance.allArchiveList[i])
                    choosedQuIndex = i;
            }
        }
        else
        {
            // banhaomode 固定选择 1 服
            if (Game.Instance.banHaoMode)
            {
                choosedQuIndex = 1;
            }
            else if (Game.Instance.neiCeFu)
            {
                choosedQuIndex = 1;
            }
            else
            {
             }
        }

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        txt_editionTxt.SetText(Application.version);
        
        // 尝试自动登录
        TryAutoLogin();
        
        InitShow();
        //btn_login.gameObject.SetActive(false);
    }
 /// <summary>
    /// 登出按钮点击事件
    /// </summary>
    private void OnLogoutButtonClick()
    {
        if (!Game.Instance.banHaoMode) return;
        
        // 删除所有本地存档，方便切换账号
        RoleManager.Instance.ClearAllLocalArchives();
        
        // 清除保存的登录账号
        PlayerPrefs.DeleteKey("LastLoginAccount");
        PlayerPrefs.Save();
        Debug.Log("已清除保存的登录账号");
        
        // 清除当前登录账号
        MyHttpServer.Instance.curAccount = "";
        
        // 显示登录界面，隐藏登录成功界面
        obj_login.SetActive(true);
        if (obj_successfulLogin != null)
        {
            obj_successfulLogin.SetActive(false);
        }
        
        PanelManager.Instance.OpenOnlyOkHint("已退出登录。", null);
    }
    void OnFinishCheckEdition()
    {
        txt_playerName.SetText("版本检查完毕");

        if (!Game.Instance.tableLoaded)
        {
            Game.Instance.LoadTable();
        }
        else
        {
            InitShow();
        }
        //btn_login.gameObject.SetActive(true);
        


    }

    void InitShow()
    {

        if (Game.Instance.onSuccessSDKLogin)
        {
            txt_playerName.SetText("sdk登录成功，开始登录服务器");

            haveArchive = false;
            for (int i = 0; i < ArchiveManager.Instance.allArchiveList.Count; i++)
            {
                Debug.Log("存档"+i+"是"+ ArchiveManager.Instance.allArchiveList[i]);
                if (i == choosedQuIndex && ArchiveManager.Instance.allArchiveList[i] != null)
                {
                    haveArchive = true;
                }
 
            }

            //登录成功
            if (Game.Instance.canEnterGame)
            {
                if (haveArchive)
                {
                    txt_playerName.SetText("[掌门：" + ArchiveManager.Instance.allArchiveList[Game.Instance.curServerIndex].playerPeople.name + "]");

                    btn_continueGame.gameObject.SetActive(true);
                    if(!Game.Instance.banHaoMode)
                    btn_archiveManager.gameObject.SetActive(true);
                    btn_newGame.gameObject.SetActive(false);
                }
                //没有任何存档
                else
                {
                    //recentArchiveIndex = ArchiveManager.Instance.maxArchiveNum - 1;//最新的区
                    txt_playerName.SetText("无角色");

                    btn_newGame.gameObject.SetActive(true);
                    if(!Game.Instance.banHaoMode)
                    btn_archiveManager.gameObject.SetActive(true);
                    btn_continueGame.gameObject.SetActive(false);
                }
            }
            //还没登录
            else
            {
                txt_playerName.SetText("服务器登陆中……");

                if (haveArchive)
                {
                    //btn_continueGame.gameObject.SetActive(true);
                    //btn_archiveManager.gameObject.SetActive(true);
                    //btn_newGame.gameObject.SetActive(false);
                }
                else
                {
                    // banhaomode 固定选择 1 服
                    if (Game.Instance.banHaoMode)
                    {
                        choosedQuIndex = 1;
                    }
                    else if (Game.Instance.neiCeFu)
                    {
                        choosedQuIndex = 1;
                    }
                    else
                    {
                     }

                    //btn_newGame.gameObject.SetActive(true);
                    //btn_archiveManager.gameObject.SetActive(false);
                    //btn_continueGame.gameObject.SetActive(false);
                }
                if(!Game.Instance.isLogin)
                    OnChooseQu(choosedQuIndex);
                //这里直接登录最近的区
                //ArchiveManager.Instance.curArchiveIndex = recentArchiveIndex;
                //Game.Instance.Connect(recentArchiveIndex);
                //btn_login.gameObject.SetActive(false);
            }
            txt_quName.SetText((choosedQuIndex - 2) + "区·"  );

        }
        //显示一个登录按钮
        else
        {
            //btn_login.gameObject.SetActive(true);
            btn_newGame.gameObject.SetActive(false);
            btn_archiveManager.gameObject.SetActive(false);
            btn_continueGame.gameObject.SetActive(false);
        }
      
    }
    void OnRequestChooseQu(object[] args)
    {
        OnChooseQu((int)args[0]);
    }
    /// <summary>
    /// 选区
    /// </summary>
    void OnChooseQu(int quIndex)
    {
        // banhaomode 固定使用 1 服
        if (Game.Instance.banHaoMode)
        {
            choosedQuIndex = 1;
            quIndex = 1; // 强制修改为 1 服
        }
        else
        {
            choosedQuIndex = quIndex;
        }
        
        txt_quName.SetText((quIndex - 2) + "区·"  );

        txt_playerName.SetText("服务器登陆中……");
        //这里直接登录最近的区
        ArchiveManager.Instance.curArchiveIndex = quIndex;

        if (Game.Instance.useFuWuQi)
        {
         }
        else
        {
            Game.Instance.StartLogin(Game.Instance.lechenGuid, quIndex);
        }
        //btn_login.gameObject.SetActive(false);

    }
    /// <summary>
    /// 删除了存档
    /// </summary>
    void OnDeleteArchive()
    {
        if (ArchiveManager.Instance.recentArchive != null)
            haveArchive = true;
        else
            haveArchive = false;
        InitShow();
    }

    void SuccessSDKLogin()
    {
        InitShow();
    }
    void OnStartLogin()
    {
        txt_playerName.SetText("登陆中");
       // btn_login.gameObject.SetActive(false);
    }
    /// <summary>
    /// 登录成功
    /// </summary>
    void SuccessLogIn()
    {
    
        //Game.Instance.StartGame();
        if (Game.Instance.tableLoaded)
         InitShow();
    }


   /// <summary>
    /// 登录按钮点击事件
    /// </summary>
    private async void OnLoginButtonClick()
    {
        if (input_LoginAccount == null || input_LoginPassword == null)
        {
            PanelManager.Instance.OpenOnlyOkHint("登录界面未正确配置！", null);
            return;
        }
        
        string account = input_LoginAccount.text?.Trim() ?? "";
        string password = input_LoginPassword.text?.Trim() ?? "";
        
        // 验证输入
        if (string.IsNullOrEmpty(account))
        {
            PanelManager.Instance.OpenOnlyOkHint("请输入账号！", null);
            return;
        }
        
        if (string.IsNullOrEmpty(password))
        {
            PanelManager.Instance.OpenOnlyOkHint("请输入密码！", null);
            return;
        }
        
        // 检查特定账号的实名认证状态
        if (account == "zzz1" || account == "zzz2" || account == "zzz3")
        {
            PanelManager.Instance.OpenOnlyOkHint("该账号未通过实名认证，无法进入。", null);
            return;
        }
        
        // 调用登录接口
        try
        {
            Debug.Log($"开始登录：账号={account}");
            //HandleLoginSuccess(account);
            StartCoroutine(MyHttpServer.Instance.Login(
            account,
            password,
            (success, message) =>
            {
                if (success)
                {
                    Debug.Log("登录成功");
                    // 处理登录成功逻辑
                    HandleLoginSuccess(account);
                }
                else
                {
                    Debug.LogError("登录失败: " + message);
                    HandleLoginFailed(message);
                }
            }
        ));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"登录异常: {ex.Message}");
            HandleLoginFailed($"登录异常: {ex.Message}");
        }
    }


 /// <summary>
    /// 处理登录成功
    /// </summary>
    private void HandleLoginSuccess(string account)
    {
        Debug.Log($"用户登录成功：{account}");
        
        // 保存登录账号到PlayerPrefs（不限制模式）
        PlayerPrefs.SetString("LastLoginAccount", account);
        
        // 默认记住密码，保存加密后的密码
        if (input_LoginPassword != null && !string.IsNullOrEmpty(input_LoginPassword.text))
        {
            string encryptedPassword = EncryptPassword(input_LoginPassword.text);
            PlayerPrefs.SetString("LastLoginPassword_" + account, encryptedPassword);
            PlayerPrefs.SetInt("RememberPassword", 1);
            Debug.Log($"已保存账号和密码: {account}");
        }
        else
        {
            Debug.Log($"已保存账号: {account}");
        }
        
        PlayerPrefs.Save();
        
        PanelManager.Instance.OpenOnlyOkHint($"登录成功！\n欢迎回来，{account}", async () =>
        {
            // 关闭登录面板，继续原有的服务器选择流程
            Debug.Log("登录成功，开始下载云端存档...");

            // 清空输入框
            if (input_LoginAccount != null) input_LoginAccount.text = "";
            if (input_LoginPassword != null) input_LoginPassword.text = "";
            obj_login.SetActive(false);

            // banhaomode下显示登录成功界面
            if (Game.Instance.banHaoMode && obj_successfulLogin != null)
            {
                obj_successfulLogin.SetActive(true);
            }

            // 如果是banhaomode，尝试下载云端存档
            if (Game.Instance.banHaoMode)
            {
                await DownloadAccountArchives(account);
            }
            Game.Instance.OnSuccessSDKLogin(account);
             //Game.Instance.StartGame
            //WebSocketClient.Instance.PCTestLogin(account);
        });
    }
    
    /// <summary>
    /// 处理登录失败
    /// </summary>
    private void HandleLoginFailed(string errorMsg)
    {
        Debug.LogError($"登录失败: {errorMsg}");
        PanelManager.Instance.OpenOnlyOkHint(errorMsg, null);
    }



    
    /// <summary>
    /// 下载账号的云端存档 (仅用于banhaomode)
    /// </summary>
    private async Task DownloadAccountArchives(string account)
    {
        try
        {
            // 检查本地是否已有存档，如果有则优先使用本地存档
            if (ArchiveManager.Instance.recentArchive != null)
            {
                Debug.Log($"banhaomode: 本地已有存档，跳过云端下载，优先使用本地存档");
                return;
            }
            
            Debug.Log($"banhaomode: 本地无存档，开始为账号 {account} 下载服务器1的存档...");
            
            // 打开loading面板
            PanelManager.Instance.OpenPanel(ObjectPoolSingle.LoadingPanel, PanelManager.Instance.trans_layer3);
            
            // banhaomode: 只下载服务器1的存档（带包名前缀）
            string archiveFileName = ArchiveManager.Instance.ArchiveDownloadName($"{account}_1");
            bool downloadSuccess = await TryDownloadSingleArchive(archiveFileName, 1);
            
            if (downloadSuccess)
            {
                Debug.Log($"成功下载存档: {archiveFileName}");
                // 重新加载存档列表
                ArchiveManager.Instance.LoadAllArchive();
            }
            else
            {
                Debug.Log("未找到云端存档，将创建新存档");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"下载云端存档异常: {ex.Message}");
        }
        finally
        {
            // 无论成功还是失败，都关闭loading面板
            EventCenter.Broadcast(TheEventType.FinishLoading);
        }
    }
    
    /// <summary>
    /// 尝试下载单个存档文件（静默失败）
    /// </summary>
    /// <param name="archiveFileName">存档文件名，格式：账号_区服</param>
    /// <param name="serverId">服务器ID</param>
    /// <returns>下载是否成功</returns>
    private async Task<bool> TryDownloadSingleArchive(string archiveFileName, int serverId)
    {
        try
        {
            // 构建本地存档保存路径
            string localArchivePath = ConstantVal.GetArchiveSaveFolder(serverId);
            
            Debug.Log($"尝试下载存档 {archiveFileName} 到 {localArchivePath}");
            
            // 记录下载前的时间戳，用于判断是否成功下载
            string expectedFilePath = Path.Combine(localArchivePath, "GameInfo.es3");
            DateTime beforeDownload = DateTime.Now;
            bool fileExistedBefore = File.Exists(expectedFilePath);
            DateTime? originalModifyTime = null;
            if (fileExistedBefore)
            {
                originalModifyTime = File.GetLastWriteTime(expectedFilePath);
            }
            
            // 使用腾讯云COS下载方法，静默处理异常
            try
            {
                // 异步调用下载，但不等待其完成，因为UpLoadFiles.OnDownloadArchive是异步的
                UpLoadFiles.OnDownloadArchive(localArchivePath, archiveFileName);
                
                // 等待一段时间让下载完成（减少等待时间）
                await Task.Delay(1500); // 等待1.5秒
                
                // 检查文件是否真的下载成功
                if (File.Exists(expectedFilePath))
                {
                    // 如果文件之前不存在，或者修改时间发生了变化，说明下载成功
                    if (!fileExistedBefore || 
                        (originalModifyTime.HasValue && File.GetLastWriteTime(expectedFilePath) > originalModifyTime.Value))
                    {
                        Debug.Log($"存档 {archiveFileName} 下载成功");
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                // 静默失败，这是预期的行为（文件可能不存在）
                Debug.Log($"存档 {archiveFileName} 下载失败（预期行为）: {ex.Message}");
            }
            
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.Log($"尝试下载存档 {archiveFileName} 时发生异常: {ex.Message}");
            return false;
        }
    }



    /// <summary>
    /// 尝试自动登录上次成功登录的账号
    /// </summary>
    private   void TryAutoLogin()
    {
        try
        {
            // 防止重复自动登录
            if (isAutoLogging) return;
            
            // 检查是否有保存的登录账号
            if (PlayerPrefs.HasKey("LastLoginAccount"))
            {
                string lastAccount = PlayerPrefs.GetString("LastLoginAccount", "");
                
                if (!string.IsNullOrEmpty(lastAccount))
                {
                    Debug.Log($"发现上次登录账号: {lastAccount}，尝试自动登录...");
                    
                    // 如果输入框存在，自动填充账号
                    if (input_LoginAccount != null)
                    {
                        input_LoginAccount.text = lastAccount;
                    }
                    
                    // 检查是否保存了密码（出于安全考虑，使用简单加密）
                    bool hasRememberedPassword = PlayerPrefs.HasKey("RememberPassword") && 
                                                PlayerPrefs.GetInt("RememberPassword", 0) == 1 &&
                                                PlayerPrefs.HasKey("LastLoginPassword_" + lastAccount);
                    
                    if (hasRememberedPassword)
                    {
                        // 自动填充密码并尝试登录
                        string encryptedPassword = PlayerPrefs.GetString("LastLoginPassword_" + lastAccount, "");
                        if (!string.IsNullOrEmpty(encryptedPassword))
                        {
                            // 简单解密（仅示例，实际应用中应使用更安全的加密方式）
                            string password = DecryptPassword(encryptedPassword);
                            
                            if (input_LoginPassword != null)
                            {
                                input_LoginPassword.text = password;
                            }
                            
                            isAutoLogging = true;
                            txt_playerName.SetText($"自动登录中: {lastAccount}");
                          
                            // 执行自动登录
                             OnLoginButtonClick();

                            
                            isAutoLogging = false;
                            return;
                        }
                    }
                    
                    // 如果没有保存密码，只填充账号
                    txt_playerName.SetText($"账号已填充: {lastAccount}，请输入密码");
                    Debug.Log($"已自动填充账号: {lastAccount}，请手动输入密码后登录");
                }
            }
            else
            {
                Debug.Log("未发现保存的登录账号");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"自动登录异常: {ex.Message}");
            isAutoLogging = false;
        }
    }

  

    /// <summary>
    /// 简单加密密码（仅示例，实际应用中应使用更安全的加密方式）
    /// </summary>
    private string EncryptPassword(string password)
    {
        try
        {
            // 使用简单的Base64 + 偏移加密
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] + 13); // 简单偏移
            }
            return System.Convert.ToBase64String(bytes);
        }
        catch
        {
            return password; // 加密失败返回原密码
        }
    }

    /// <summary>
    /// 简单解密密码
    /// </summary>
    private string DecryptPassword(string encryptedPassword)
    {
        try
        {
            // 解密过程
            byte[] bytes = System.Convert.FromBase64String(encryptedPassword);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] - 13); // 恢复偏移
            }
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return ""; // 解密失败返回空字符串
        }
    }

    public override void Clear()
    {
        base.Clear();
        txt_playerName.SetText("正在检查版本……");
        // btn_login.gameObject.SetActive(true);
        // btn_downloadArchive.gameObject.SetActive(false);
        btn_newGame.gameObject.SetActive(false);
        btn_continueGame.gameObject.SetActive(false);
        btn_archiveManager.gameObject.SetActive(false);//存档管理
        //btn_uploadArchive.gameObject.SetActive(false);//上传存档
        choosedQuIndex = -1;
        obj_login.SetActive(true);
        obj_successfulLogin.SetActive(false);
        
        // 重置自动登录状态
        isAutoLogging = false;
    }
}
