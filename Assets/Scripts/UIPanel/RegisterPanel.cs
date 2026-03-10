using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public class RegisterPanel : PanelBase
{
    [Header("输入框")]
    public InputField input_Account;      // 账号输入框
    public InputField input_Password;     // 密码输入框
    public InputField input_Name;         // 姓名输入框
    public InputField input_IdCard;       // 身份证号输入框
    
    [Header("按钮")]
    public Button btn_Register;           // 注册按钮
    public Button btn_Cancel;             // 取消按钮
    
    [Header("提示文本")]
    public Text txt_Tips;                 // 提示信息文本
    
    public override void Init(params object[] args)
    {
        base.Init(args);
        
        // 绑定按钮事件
       addBtnListener(btn_Register, OnRegisterClick);
       if(btn_Cancel != null)
       addBtnListener(btn_Cancel, OnCancelClick);

        // 清空输入框
        ClearInputs();
        
        // 隐藏提示信息
        if (txt_Tips != null)
            txt_Tips.text = "";
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ClearInputs();
        if (txt_Tips != null)
            txt_Tips.text = "";
    }
    
    /// <summary>
    /// 清空所有输入框
    /// </summary>
    private void ClearInputs()
    {
        if (input_Account != null) input_Account.text = "";
        if (input_Password != null) input_Password.text = "";
        if (input_Name != null) input_Name.text = "";
        if (input_IdCard != null) input_IdCard.text = "";
    }
    
    /// <summary>
    /// 注册按钮点击事件
    /// </summary>
    private void OnRegisterClick()
    {
        string account = input_Account?.text?.Trim() ?? "";
        string password = input_Password?.text?.Trim() ?? "";
        string name = input_Name?.text?.Trim() ?? "";
        string idCard = input_IdCard?.text?.Trim() ?? "";
        
        // 验证输入
        if (!ValidateInputs(account, password, name, idCard))
            return;
            
        // 验证身份证号
        var idCardResult = ValidateIdCard(idCard);
        if (!idCardResult.isValid)
        {
            ShowTips(idCardResult.errorMsg);
            return;
        }
        
        // // 检查是否成年
        // if (!idCardResult.isAdult)
        // {
        //     ShowTips("注册失败：未成年用户不允许注册");
        //     return;
        // }
        
        // 执行注册逻辑
        DoRegister(account, password, name, idCard, idCardResult);
    }
    
    /// <summary>
    /// 取消按钮点击事件
    /// </summary>
    private void OnCancelClick()
    {
        PanelManager.Instance.ClosePanel(this);
    }
    
    /// <summary>
    /// 验证基本输入信息
    /// </summary>
    private bool ValidateInputs(string account, string password, string name, string idCard)
    {
        if (string.IsNullOrEmpty(account))
        {
            ShowTips("请输入账号");
            return false;
        }
        
        if (account.Length < 3 || account.Length > 20)
        {
            ShowTips("账号长度应为3-20个字符");
            return false;
        }
        
        if (string.IsNullOrEmpty(password))
        {
            ShowTips("请输入密码");
            return false;
        }
        
        if (password.Length < 6 || password.Length > 20)
        {
            ShowTips("密码长度应为6-20个字符");
            return false;
        }
        
        if (string.IsNullOrEmpty(name))
        {
            ShowTips("请输入姓名");
            return false;
        }
        
        if (name.Length < 2 || name.Length > 10)
        {
            ShowTips("姓名长度应为2-10个字符");
            return false;
        }
        
        if (string.IsNullOrEmpty(idCard))
        {
            ShowTips("请输入身份证号");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 身份证验证结果
    /// </summary>
    public struct IdCardValidationResult
    {
        public bool isValid;        // 是否有效
        public bool isAdult;        // 是否成年
        public int age;             // 年龄
        public string gender;       // 性别
        public string birthDate;    // 出生日期
        public string errorMsg;     // 错误信息
    }
    
    /// <summary>
    /// 验证身份证号码
    /// </summary>
    private IdCardValidationResult ValidateIdCard(string idCard)
    {
        var result = new IdCardValidationResult();
        
        // 基本格式验证
        if (string.IsNullOrEmpty(idCard))
        {
            result.errorMsg = "身份证号不能为空";
            return result;
        }
        
        // 长度验证
        if (idCard.Length != 18)
        {
            result.errorMsg = "身份证号必须为18位";
            return result;
        }
        
        // 格式验证（前17位为数字，最后一位为数字或X）
        if (!Regex.IsMatch(idCard, @"^\d{17}[\dX]$"))
        {
            result.errorMsg = "身份证号格式不正确";
            return result;
        }
        
        // 提取出生日期
        string birthStr = idCard.Substring(6, 8);
        if (!DateTime.TryParseExact(birthStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime birthDate))
        {
            result.errorMsg = "身份证号中的出生日期无效";
            return result;
        }
        
        // 检查出生日期是否合理
        DateTime now = DateTime.Now;
        if (birthDate > now || birthDate < new DateTime(1900, 1, 1))
        {
            result.errorMsg = "出生日期不合理";
            return result;
        }
        
        // 计算年龄
        int age = now.Year - birthDate.Year;
        if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
        {
            age--;
        }
        
        // 获取性别（倒数第二位，奇数为男，偶数为女）
        int genderCode = int.Parse(idCard.Substring(16, 1));
        string gender = genderCode % 2 == 1 ? "男" : "女";
        
        // 校验码验证
        if (!ValidateIdCardCheckCode(idCard))
        {
            result.errorMsg = "身份证号校验码错误";
            return result;
        }
        
        result.isValid = true;
        result.age = age;
        result.isAdult = age >= 18;
        result.gender = gender;
        result.birthDate = birthDate.ToString("yyyy年MM月dd日");
        
        return result;
    }
    
    /// <summary>
    /// 验证身份证校验码
    /// </summary>
    private bool ValidateIdCardCheckCode(string idCard)
    {
        // 加权因子
        int[] weights = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
        // 校验码对应表
        char[] checkCodes = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
        
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            sum += int.Parse(idCard[i].ToString()) * weights[i];
        }
        
        int mod = sum % 11;
        char expectedCheckCode = checkCodes[mod];
        
        return idCard[17] == expectedCheckCode;
    }
    
    /// <summary>
    /// 执行注册
    /// </summary>
    private   void DoRegister(string account, string password, string name, string idCard, IdCardValidationResult idCardInfo)
    {
        // 显示正在注册的提示
        ShowTips("正在注册，请稍等...");
        
        // 禁用注册按钮，防止重复点击
        if (btn_Register != null)
            btn_Register.interactable = false;
            
        try
        {
            // 调用HTTP注册接口
            StartCoroutine(MyHttpServer.Instance.Register(account, password, name, idCard,
            (result, msg) =>
            {
                if (result)
                {
                    HandleRegisterSuccess(account, name,idCardInfo);

                }
                else
                {
                    HandleRegisterFailed(msg);
                }
            }));

            // 重新启用注册按钮
                if (btn_Register != null)
                    btn_Register.interactable = true;
            
           
        }
        catch (Exception ex)
        {
            // 重新启用注册按钮
            if (btn_Register != null)
                btn_Register.interactable = true;
                
            HandleRegisterFailed("注册失败：网络异常 - " + ex.Message);
            Debug.LogError($"注册异常: {ex}");
        }
    }
    
    /// <summary>
    /// 处理注册成功
    /// </summary>
    private void HandleRegisterSuccess(string account, string name, IdCardValidationResult idCardInfo)
    {
        string successMsg = $"注册成功！\n" +
                           $"账号：{account}\n" +
                           $"姓名：{name}\n" +
                           $"性别：{idCardInfo.gender}\n" +
                           $"出生日期：{idCardInfo.birthDate}\n" +
                           $"年龄：{idCardInfo.age}岁\n\n" +
                           $"点击确定跳转到登录界面";
        
        Debug.Log($"用户注册成功：账号={account}, 姓名={name}, 年龄={idCardInfo.age}");
        
        // 弹出成功提示，点击确定后跳转到登录界面
        PanelManager.Instance.OpenOnlyOkHint(successMsg, new System.Action(() =>
        {
            // 关闭注册面板
            PanelManager.Instance.ClosePanel(this);
            
            // 打开登录面板
            //PanelManager.Instance.OpenPanel(ObjectPoolSingle.StartPanel, PanelManager.Instance.trans_layer2);
            
            Debug.Log("注册成功，跳转到登录界面");
        }));
    }
    
    /// <summary>
    /// 处理注册失败
    /// </summary>
    private void HandleRegisterFailed(string errorMsg)
    {
        Debug.LogError($"注册失败: {errorMsg}");
        
        // 弹出失败提示
        PanelManager.Instance.OpenOnlyOkHint(errorMsg, new System.Action(() =>
        {
            // 失败后不做特殊处理，用户可以重新尝试注册
            Debug.Log("注册失败提示已确认");
        }));
        
        // 同时在界面上显示错误信息
        ShowTips(errorMsg);
    }
    
  
    
    /// <summary>
    /// 显示提示信息
    /// </summary>
    private void ShowTips(string message)
    {
        if (txt_Tips != null)
        {
            txt_Tips.text = message;
        }
        Debug.Log($"RegisterPanel Tips: {message}");
    }
    
    public override void Clear()
    {
        base.Clear();
         
    }
}
