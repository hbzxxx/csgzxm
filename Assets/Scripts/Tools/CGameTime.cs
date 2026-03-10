using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

/// <summary>
/// mao 时间类
/// </summary>
public class CGameTime
{
    static CGameTime inst;

    public static CGameTime Instance
    {
        get
        {
            if (inst == null)
                inst = new CGameTime();
            return inst;
        }
    }

    public void GetTimeStampFromApplicationStart()
    {
        //Time.realtimeSinceStartup
    }

    /// <summary>
    /// 时间戳转日期
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public DateTime GetDateTimeByTimeStamp(long timeStamp)
    {
        DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
        DateTime dt = startTime.AddSeconds(timeStamp);
        return dt;
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public long GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
    /// <summary>
    /// 获取时间戳 毫秒
    /// </summary>
    /// <returns></returns>
    public long GetTimeStampMiliSecond()
    {
        TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }


    /// <summary>
    /// 得到某天24点时间戳
    /// </summary>
    /// <returns></returns>
    public long GetTo24TimeStamp(DateTime date)
    {
       DateTime the24Time= Convert.ToDateTime(date.AddDays(1).ToString("D").ToString()).AddSeconds(-1);
        TimeSpan ts = the24Time - date;
        return (long)ts.TotalSeconds;
    }

    /// <summary>
    /// 传入当前时间戳，得到24点时间戳
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public long GetTo24TimeStampByTimeStamp(long timeStamp)
    {
        DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
        DateTime dt = startTime.AddSeconds(timeStamp);
        long res = GetTo24TimeStamp(dt);
        
        return res;
    }

    /// <summary>
    /// 传入当前时间戳，得到星期几
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public DayOfWeek GetDayOfWeekByTimeStamp(long timeStamp)
    {
        DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
        DateTime dt = startTime.AddSeconds(timeStamp);
        return dt.DayOfWeek;

    }



    public IEnumerator GetServiceTimeSyn(Action<long> callback)
    {
        string url = "https://www.baidu.com";
        //Debug.Log("开始获取"+url +"的服务器时间（GMT DATE）");
        UnityWebRequest WebRequest = new UnityWebRequest(url);
        WebRequest.timeout = 3;
        yield return WebRequest.SendWebRequest();
        decimal dec = 0;
        //网页加载完成  并且下载过程中没有错误   string.IsNullOrEmpty 判断字符串是否是null 或者是" ",如果是返回true
        //WebRequest.error  下载过程中如果出现下载错误  会返回错误信息 如果下载没有完成那么将会阻塞到下载完成
        if (WebRequest.isDone && string.IsNullOrEmpty(WebRequest.error))
        {
            Dictionary<string, string> resHeaders = WebRequest.GetResponseHeaders();
            string key = "DATE";
            string value = null;
            if (resHeaders != null && resHeaders.ContainsKey(key))
            {
                resHeaders.TryGetValue(key, out value);
            }
            if (value == null)
            {
                Debug.Log("DATE is null");
                yield break;
            }

            //存储当前时间戳
            //Timer_t._Action_SaveCurrentTime(GetTimeSqite(FormattingGMT(value).ToString()));

            //北京时间
            //Debug.Log(FormattingGMT(value).ToString());
            dec = GetTimeSqite(FormattingGMT(value));
            callback((long)dec);
        }
        else
        {
            //PanelManager.Instance.OpenFloatWindow("获取服务器时间出错！");
            callback(-1);
        }
    }

    /// <summary>
    /// GMT(格林威治时间)时间转成本地时间
    /// </summary>
    /// <param name="gmt">字符串形式的GMT时间</param>
    /// <returns></returns>
    private DateTime FormattingGMT(string gmt)
    {
        DateTime dt = DateTime.MinValue;
        try
        {
            string pattern = "";
            if (gmt.IndexOf("+0") != -1)
            {
                gmt = gmt.Replace("GMT", "");
                pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
            }
            if (gmt.ToUpper().IndexOf("GMT") != -1)
            {
                pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
            }
            if (pattern != "")
            {
                dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                dt = dt.ToLocalTime();
            }
            else
            {
                dt = Convert.ToDateTime(gmt);
            }
        }
        catch
        {
        }
        return dt;
    }
    /// <summary>
    /// 返回的是一个时间数组,数组下标从0到最大代表了--- 年月日时分秒
    /// </summary>
    /// <param name="_str">传入一个时间的字符串</param>
    /// <returns></returns>
    private decimal GetTimeSqite(DateTime dateTime)
    {
        //        string pattern4 = @"[ ]|[/]|[:]";
        //        //存储在数组中的顺序是年月日时分秒
        //        string[] timerArr = Regex.Split(_str, pattern4);

        //#if UNITY_IOS
        //       int[] timerNum = new int[timerArr.Length-1];
        //       for (int i = 0; i < timerArr.Length-1; i++)
        //       {
        //         timerNum[i] = int.Parse(timerArr[i]);
        //       }
        //#else
        //        int[] timerNum = new int[timerArr.Length];
        //        for (int i = 0; i < timerArr.Length; i++)
        //        {
        //            timerNum[i] = int.Parse(timerArr[i]);
        //        }
        //#endif
        dateTime = dateTime.ToUniversalTime();
        DateTime startTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);//这是历史最早的时间戳
        decimal t = (dateTime.Ticks - startTime.Ticks) / 10000000;//得到了那个点的时间戳(这里用long好像会丢精度   建议decimal)
        long thetest = GetTimeStamp();

        return t;

        return 11;
        //return GetTimeStamp(timerNum);
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    private decimal GetTimeStamp(int[] time)
    {
        DateTime today = new DateTime(time[0], time[1], time[2], time[3], time[4], time[5]);
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));//这是历史最早的时间戳
        decimal t = (today.Ticks - startTime.Ticks) / 10000000;//得到了那个点的时间戳(这里用long好像会丢精度   建议decimal)
        return t;
    }

}
