using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Framework.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyHttpServer : CommonInstance<MyHttpServer>
{
    // 服务器URL配置 - 使用HTTPS和3443端口
      private string serverUrl = "https://101.35.87.105:3443";
    
    private void Awake()
    {
        // 使用HTTPS连接，设置证书验证（如果需要的话）
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        
        Debug.Log("HTTP服务器初始化完成，使用HTTPS连接：" + serverUrl);
    }
    [SerializeField]
    private string packageName
    {
        get
        {
            return Application.identifier;
        }
    }
    
    // UI引用 (如果您使用UI)
    [SerializeField] private InputField usernameInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private InputField realNameInput;
    [SerializeField] private InputField idCardInput;
    [SerializeField] private Text responseText;
    
    public string curSessionId = "";
    public string curAccount = "";
    public string curIdCard;
    private Coroutine heartbeatCoroutine;
    private Coroutine timeCheckCoroutine;  // 时间检查协程

    // 注册按钮点击事件
    public void OnRegisterButtonClick(string userName, string passWord, string realName, string idCard)
    {
        Game.Instance. StartCoroutine(Register(
            userName,
            passWord,
            realName,
            idCard,
            (success, message) =>
            {
                if (success)
                {
                    Debug.Log("注册成功");
                    // 处理注册成功逻辑
                }
                else
                {
                    Debug.LogError("注册失败: " + message);
                    // 处理注册失败逻辑
                }
            }
        ));
    }



    public void RechargeBtnClick(int num)
    {
          Game.Instance. StartCoroutine(Recharge(
            curSessionId,
            curAccount,
            num,
            response =>
            {
                Debug.Log("充值成功: " + response.message);
                // 处理充值成功逻辑
            },
            errorMessage =>
            {
                Debug.LogError("充值失败: " + errorMessage);
                // 处理充值失败逻辑
            }
        ));
    }


    
    /// <summary>
    /// 开始发送心跳
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="deviceId">当前设备的唯一ID</param>
    /// <param name="intervalSeconds">心跳间隔时间 (秒)</param>
    public void StartHeartbeat(string username, string deviceId, float intervalSeconds = 5.0f)
    {
        if (heartbeatCoroutine != null)
        {
             Game.Instance.  StopCoroutine(heartbeatCoroutine);
        }
        heartbeatCoroutine =   Game.Instance.StartCoroutine(HeartbeatCoroutine(username, deviceId, intervalSeconds));
        Debug.Log($"心跳已启动，用户: {username}, 设备ID: {deviceId}");
    }
    
    /// <summary>
    /// 开始时间限制检查 (仅用于banhaomode)
    /// </summary>
    /// <param name="intervalSeconds">检查间隔时间 (秒)</param>
    public void StartTimeCheck(float intervalSeconds = 5.0f)
    {
        if (!Game.Instance.banHaoMode) return;
        
        if (timeCheckCoroutine != null)
        {
              Game.Instance. StopCoroutine(timeCheckCoroutine);
        }
        timeCheckCoroutine =   Game.Instance. StartCoroutine(TimeCheckCoroutine(intervalSeconds));
        Debug.Log("时间限制检查已启动");
    }
    
    /// <summary>
    /// 时间限制检查协程
    /// </summary>
    private IEnumerator TimeCheckCoroutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            
            // 检查是否在允许的游戏时间
            if (!IsAllowedGameTime())
            {
                Debug.LogWarning("当前时间不在允许的游戏时间范围内");
                ShowTimeRestrictionMessage();
                yield break; // 停止检查
            }
        }
    }
    
    /// <summary>
    /// 检查当前时间是否在允许的游戏时间内
    /// </summary>
    /// <returns>是否在允许时间内</returns>
    private bool IsAllowedGameTime()
    {
        if (!Game.Instance.banHaoMode) return true;
        if (string.IsNullOrEmpty(curIdCard)) return true;
        
        // 计算年龄
        int age = GetAgeFromIdCard(curIdCard);
        if (age >= 18) return true; // 成年人无限制
        
        DateTime now = DateTime.Now;
        DayOfWeek dayOfWeek = now.DayOfWeek;
        int hour = now.Hour;
        
        // 未成年人只能在周五、周六、周日的20-21点游戏
        bool isWeekend = (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday);
        bool isValidHour = (hour >= 20 && hour < 21);
        
        return isWeekend && isValidHour;
    }
    
    /// <summary>
    /// 从身份证号获取年龄
    /// </summary>
    /// <param name="idCard">身份证号</param>
    /// <returns>年龄</returns>
    private int GetAgeFromIdCard(string idCard)
    {
        if (string.IsNullOrEmpty(idCard) || idCard.Length != 18) return 18; // 默认成年
        
        try
        {
            int year = int.Parse(idCard.Substring(6, 4));
            int month = int.Parse(idCard.Substring(10, 2));
            int day = int.Parse(idCard.Substring(12, 2));
            
            DateTime birthDate = new DateTime(year, month, day);
            return CalculateAge(birthDate);
        }
        catch (Exception ex)
        {
            Debug.LogError($"解析身份证年龄失败: {ex.Message}");
            return 18; // 解析失败默认成年
        }
    }
    
    /// <summary>
    /// 显示时间限制提示并强制退出
    /// </summary>
    private void ShowTimeRestrictionMessage()
    {
        int age = GetAgeFromIdCard(curIdCard);
        string message = $"根据国家相关规定，未成年人只能在周五、周六、周日的20:00-21:00时间段内进行游戏。当前时间不在允许范围内，游戏将自动关闭。";
        
        PanelManager.Instance.OpenOnlyOkHint(message, () =>
        {
            Debug.Log("未成年人时间限制，强制退出游戏");
            Application.Quit();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
    }
    
    /// <summary>
    /// 停止时间检查
    /// </summary>
    public void StopTimeCheck()
    {
        if (timeCheckCoroutine != null)
        {
              Game.Instance. StopCoroutine(timeCheckCoroutine);
            timeCheckCoroutine = null;
            Debug.Log("时间限制检查已停止");
        }
    }
     private IEnumerator HeartbeatCoroutine(string username, string deviceId, float interval)
    {
        while (true)
        {
          yield return new WaitForSeconds(interval);

string url = $"{serverUrl}/api/iAmAlive";

// 使用与登录相同的JSON库和数据结构
var requestData = new Dictionary<string, object>
{
    { "username", username },
    { "deviceId", deviceId }
};

// 统一使用 JsonConvert 进行序列化
string json = JsonConvert.SerializeObject(requestData);

using (UnityWebRequest request = UnityWebRequest.Put(url, json))
{
    request.method = "POST";
    request.SetRequestHeader("Content-Type", "application/json");
    
    // HTTPS连接的证书处理
    request.certificateHandler = new AcceptAllCertificates();
    request.disposeCertificateHandlerOnDispose = true;

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        string responseJson = request.downloadHandler.text;
        
        // 统一使用 JsonConvert 进行反序列化
        HeartbeatResponse response = JsonConvert.DeserializeObject<HeartbeatResponse>(responseJson);

        if (response.success)
        {
            if (response.status == "DEVICE_ID_MISMATCH")
            {
                Debug.LogWarning($"需要被踢下线! 消息: {response.message}. 新设备ID: {response.newDeviceId}");
                // 在这里处理强制下线逻辑
                HandleForceLogout();
                yield break; // 停止心跳
            }
            else if (response.status == "OK")
            {
                Debug.Log("心跳正常。");
            }
        }
        else
        {
            Debug.LogError($"心跳失败: {response.message}");
            // 如果服务器告知未登录或会话过期，也应处理下线
            if (response.status == "NOT_LOGGED_IN")
            {
                HandleForceLogout();
                yield break;
            }
        }
    }
    else
    {
        // 改进错误处理，显示更详细的错误信息
        string errorMsg = $"心跳请求错误 - 状态码: {request.responseCode}";
        if (!string.IsNullOrEmpty(request.downloadHandler.text))
        {
            errorMsg += $" - 响应: {request.downloadHandler.text}";
        }
        else
        {
            errorMsg += $" - 网络错误: {request.error}";
        }
        Debug.LogError(errorMsg);
        // 网络错误等情况，可以考虑增加重试逻辑或直接下线
    }
}
        }
    }

    /// <summary>
    /// 处理强制下线
    /// </summary>
    private void HandleForceLogout()
    {
        Debug.LogError("强制下线！您的账号已在别处登录或会话已失效。");
        StopHeartbeat();
        StopTimeCheck();
        // 在这里添加返回登录界面、清理用户数据等操作
        // 例如:
        // UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }

    /// <summary>
    /// 停止发送心跳
    /// </summary>
    public void StopHeartbeat()
    {
        if (heartbeatCoroutine != null)
        {
             Game.Instance.  StopCoroutine(heartbeatCoroutine);
            heartbeatCoroutine = null;
            Debug.Log("心跳已停止。");
        }
    }


    /// <summary>
    /// 发起充值请求
    /// </summary>
    /// <param name="sessionId">用户会话ID</param>
    /// <param name="username">用户名</param>
    /// <param name="amount">充值金额 (整数)</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public IEnumerator Recharge(string sessionId, string username, int amount, Action<RechargeResponse> onSuccess, Action<string> onError)
    {
   // 修改后的充值代码
string url = $"{serverUrl}/api/recharge";

// 使用与登录相同的JSON库和数据结构
var requestData = new Dictionary<string, object>
{
    { "sessionId", curSessionId },
    { "username", curAccount },
    { "amount", (float)amount }
};

string json = JsonConvert.SerializeObject(requestData);

using (UnityWebRequest request = UnityWebRequest.Put(url, json))
{
    request.method = "POST";
    request.SetRequestHeader("Content-Type", "application/json");
    
    // HTTPS连接的证书处理
    request.certificateHandler = new AcceptAllCertificates();
    request.disposeCertificateHandlerOnDispose = true;

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        var response = JsonConvert.DeserializeObject<RechargeResponse>(request.downloadHandler.text);
        if (response.success)
        {
            // 清洗提示文本
            response.message = SanitizeHint(response.message, "充值成功");
            onSuccess?.Invoke(response);
        }
        else
        {
            string clean = SanitizeHint(response.message, "充值失败");
            onError?.Invoke(clean);
        }
    }
    else
    {
        // 不把状态码直接传给用户，只在日志中记录
        string rawDetail = $"状态码:{request.responseCode} 原始:{request.downloadHandler.text}";
        Debug.LogError($"Recharge error detail => {rawDetail}");
        string serverMsg = null;
        if (!string.IsNullOrEmpty(request.downloadHandler.text))
        {
            try
            {
                var resp = JsonConvert.DeserializeObject<RechargeResponse>(request.downloadHandler.text);
                serverMsg = resp?.message;
            }
            catch { }
        }
        string clean = SanitizeHint(serverMsg, "充值失败");
        onError?.Invoke(clean);
    }
}
    }

    // 统一清洗提示：只保留主要中文语句，去除技术细节，补全结尾标点
    private string SanitizeHint(string raw, string fallback)
    {
        if (string.IsNullOrWhiteSpace(raw)) raw = fallback;
        raw = raw.Trim();
        // 抓取第一段中文/数字/常用符号
        var reg = new System.Text.RegularExpressions.Regex(@"[\u4e00-\u9fa50-9A-Za-z：:，,。！？!%（）()·—_《》“”‘’\s]+");
        var m = reg.Match(raw);
        if (m.Success) raw = m.Value.Trim();
        int nl = raw.IndexOf('\n');
        if (nl >= 0) raw = raw.Substring(0, nl).Trim();
        if (!(raw.EndsWith("。") || raw.EndsWith("！") || raw.EndsWith("？"))) raw += "。";
        return raw;
    }

    /// <summary>
    /// 显示未成年人充值限制提示
    /// </summary>
    private void ShowRechargeRestrictionMessage(string message)
    {
        // 在游戏中显示一个提示框或UI元素
        Debug.LogWarning("未成年人充值限制提示: " + message);
        // 例如:
        // UIManager.Instance.ShowPopup("充值限制", message +
        //     "\n\n根据国家相关规定，未成年人充值有以下限制：\n" +
        //     "- 8周岁以上未满16周岁的用户：单次充值不超过50元，每月累计不超过200元\n" +
        //     "- 16周岁以上未满18周岁的用户：单次充值不超过100元，每月累计不超过400元");
    }





    // 获取设备唯一ID
    public string GetDeviceId()
    {
        // 在实际应用中，应使用更可靠的方法获取设备唯一标识符
        // 例如：SystemInfo.deviceUniqueIdentifier
        return SystemInfo.deviceUniqueIdentifier;
    }
    
    // 用户登录
    public IEnumerator Login(string username, string password, System.Action<bool, string> callback = null)
    {
         
        // 创建登录数据
        var loginData = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password },
            { "deviceId", GetDeviceId() },
            { "packageName", packageName }
        };
        
        // 转换为JSON
        string jsonData = JsonConvert.SerializeObject(loginData);
        string url = $"{serverUrl}/api/login";
        // 创建请求
        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            
            // HTTPS连接的证书处理
            request.certificateHandler = new AcceptAllCertificates();
            request.disposeCertificateHandlerOnDispose = true;
            
            // 发送请求
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
                
                if (response.success)
                {
                    // 保存会话信息
                    //sessionId = response.sessionId;
                    //isLoggedIn = true;
                    curSessionId= response.sessionId;
                    curAccount = username;
                    curIdCard = response.user.idCard; // 保存身份证号
                    Debug.Log("登录成功: " + JsonConvert.SerializeObject(response.user));
                    
                    // banhaomode下启动时间限制检查
                    if (Game.Instance.banHaoMode)
                    {
                        StartTimeCheck();
                    }
                    
                    // 登录成功后启动心跳
                    StartHeartbeat(username, GetDeviceId());
                    
                    // 处理成功登录
                    callback?.Invoke(true, null);
                }
                else
                {
                    // 检查是否是未成年人限制
                    if (response.restrictionReason == "AGE_RESTRICTION")
                    {
                        Debug.LogWarning("未成年人登录限制: " + response.message);
                        // 显示未成年人限制提示
                        ShowAgeRestrictionMessage(response.message);
                    }
                    else
                    {
                        Debug.LogError("登录失败: " + response.message);
                    }
                    
                   // isLoggedIn = false;
                    callback?.Invoke(false, response.message);
                }
            }
            else
            {
                // 尝试解析错误响应
                string errorMessage = request.error;
                try 
                {
                    // 尝试解析响应内容，可能包含更详细的错误信息
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        var errorResponse = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.message))
                        {
                            errorMessage = errorResponse.message;
                            
                            // 检查是否是未成年人限制
                            if (errorResponse.restrictionReason == "AGE_RESTRICTION")
                            {
                                Debug.LogWarning("未成年人登录限制: " + errorMessage);
                                // 显示未成年人限制提示
                                ShowAgeRestrictionMessage(errorMessage);
                                callback?.Invoke(false, errorMessage);
                                //isLoggedIn = false;
                                yield break;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("解析错误响应失败: " + ex.Message);
                }
                
                Debug.LogError("登录请求失败: " + errorMessage);
               // isLoggedIn = false;
                callback?.Invoke(false, errorMessage);
            }
        }
    }
    
    // 显示未成年人限制提示
    private void ShowAgeRestrictionMessage(string message)
    {
        // 在实际应用中，这里应该显示一个UI提示，告知用户未成年人登录时间限制
        // 例如：弹出对话框、显示提示面板等
        Debug.LogWarning("未成年人登录限制: " + message);
        
        // 示例：如果使用Unity UI系统，可以激活一个预先准备好的提示面板
        // if (ageRestrictionPanel != null)
        // {
        //     ageRestrictionPanel.SetActive(true);
        //     Text messageText = ageRestrictionPanel.GetComponentInChildren<Text>();
        //     if (messageText != null)
        //     {
        //         messageText.text = message;
        //     }
        // }
    }




    // 注册方法
    public IEnumerator Register(string username, string password, string realName, string idCard, System.Action<bool, string> callback = null)
    {
        // 验证输入
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(realName) || string.IsNullOrEmpty(idCard))
        {
            string errorMsg = "所有字段都是必填的";
            if (responseText != null)
                responseText.text = errorMsg;
            Debug.LogError(errorMsg);
            callback?.Invoke(false, errorMsg);
            yield break;
        }

        if (!CheckRegisterParamValid(username, password, realName, idCard).Item1)
        {
            string errorMsg = CheckRegisterParamValid(username, password, realName, idCard).Item2;
            callback?.Invoke(false, errorMsg);
            yield break; // 如果验证不通过，直接返回
        }

        // 创建注册数据
        var registrationData = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password },
            { "realName", realName },
            { "idCard", idCard },
            { "packageName", packageName } // 添加游戏包名
        };

        // 转换为JSON
        string jsonData = JsonUtility.ToJson(new Serializable(registrationData));
string url = $"{serverUrl}/api/register";
        // 创建请求 - 使用HTTPS POST方法
        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            
            // HTTPS连接的证书处理
            request.certificateHandler = new AcceptAllCertificates();
            request.disposeCertificateHandlerOnDispose = true;

            // 发送请求
            yield return request.SendWebRequest();

            // 处理响应
            if (request.result == UnityWebRequest.Result.Success)
            {
                string successMsg = "注册成功: " + request.downloadHandler.text;
                Debug.Log(successMsg);
                if (responseText != null)
                    responseText.text = "注册成功!";

                // 调用成功回调
                callback?.Invoke(true, successMsg);
                
                // 可以在这里添加注册成功后的逻辑
            }
            else
            {
                string errorMessage = "注册失败: " + request.error;

                // 尝试解析服务器返回的错误信息
                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    try
                    {
                        var response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
                        errorMessage = response.message;
                    }
                    catch
                    {
                        // 解析失败，使用默认错误信息
                    }
                }

                Debug.LogError(errorMessage);
                if (responseText != null)
                    responseText.text = errorMessage;
                
                // 调用失败回调
                callback?.Invoke(false, errorMessage);
            }
        }
    }

    public (bool, string) CheckRegisterParamValid(string account, string password, string name, string shenFenZheng)
    {
        string errorMessage = "";
        
        if (!Regex.IsMatch(account, "^[a-zA-Z0-9]*$"))
        {
            errorMessage = "账号只可由字母和数字组成";
            return (false, errorMessage);
        }
        if (password.Length < 3)
        {
            errorMessage = "您的密码少于3位";
            return (false, errorMessage);
        }
        if (account.Length == 0 || password.Trim().Length == 0)
        {
            errorMessage = "请输入账号和密码";
            return (false, errorMessage);
        }
        if (name.Trim().Length == 0)
        {
            errorMessage = "请检查您的身份证信息是否正确";
            return (false, errorMessage);
        }
        if (shenFenZheng.Trim().Length == 0)
        {
            errorMessage = "请检查您的身份证信息是否正确";
            return (false, errorMessage);
        }
        if (!CheckChinaIDCardNumberFormat(shenFenZheng))
        {
            errorMessage = "请检查您的身份证信息是否正确";
            return (false, errorMessage);
        }

        string value2 = name;
        if (value2.Contains(" "))
        {
            errorMessage = "您输入的内容含有敏感词,请重新输入。";
            return (false, errorMessage);
        }
        if (DataTable.IsScreening(value2))
        {
            errorMessage = "您输入的内容含有敏感词,请重新输入。";
            return (false, errorMessage);
        }
     
        // //判断年龄
        // int old = CalculateAge(
        //             new DateTime(int.Parse(shenFenZheng.Substring(6, 4)), int.Parse(shenFenZheng.Substring(10, 2)), int.Parse(shenFenZheng.Substring(12, 2))));
        // if (old < 8)
        // {
        //     errorMessage = "您的年龄小于8岁，无法注册。";
        //     return (false, errorMessage);
        // }
        
        // 所有验证都通过
        return (true, "验证成功");
    }

    

    public static int CalculateAge(DateTime birthDate)
    {
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;

        // 调整年龄，如果生日还没到
        if (birthDate > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }


    //验证合法性
    public static bool CheckChinaIDCardNumberFormat(string idCardNumber)
    {
        string idNumber = idCardNumber;
        bool result = true;
        try
        {
            if (idNumber.Length != 18)
            {
                return false;
            }
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            int year = int.Parse(idNumber.Substring(6, 4)); // 从索引6开始，截取4个字符
            if (year >= 1900 && year <= 2023)
            {

            }
            else
            {
                return false;
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }

            return true;//符合GB11643-1999标准 

        }
        catch (Exception)
        {

            result = false;
        }
        return result;
    }
    // 用于序列化Dictionary的辅助类
    [System.Serializable]
    private class Serializable
    {
        public string username;
        public string password;
        public string realName;
        public string idCard;
        public string packageName;
        
        public Serializable(Dictionary<string, string> data)
        {
            username = data["username"];
            password = data["password"];
            realName = data["realName"];
            idCard = data["idCard"];
            packageName = data["packageName"];
        }
    }
    
    // 响应数据类
    [System.Serializable]
    private class ResponseData
    {
        public bool success;
        public string message;
    }
}


// 响应数据类
[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public string sessionId;
    public UserInfo user;
    public string restrictionReason; // 添加限制原因字段，用于标识未成年人限制
}
[System.Serializable]
public class UserInfo
{
    public string username;
    public string realName;
    public string packageName;
    public string deviceId;
    public string loginTime;
    public string idCard; // 新增身份证号字段
}
[Serializable]
public class RechargeResponse
{
    public bool success;
    public string message;
    public string restrictionReason; // "RECHARGE_RESTRICTION"
}


[Serializable]
public class HeartbeatResponse
{
    public bool success;
    public string status; // "OK", "DEVICE_ID_MISMATCH", "NOT_LOGGED_IN"
    public string message;
    public string newDeviceId;
}

// 用于允许所有证书的证书处理器
public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 总是返回true，接受所有证书（仅用于开发环境）
        return true;
    }
}