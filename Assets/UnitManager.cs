using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class UnitManager : CommonInstance<UnitManager>
{  

    public string api_key= "3WIaL6wB9ndQ8ZohNEwyBCPI";
    public string secret_key= "hmVKduVytEgOSfoKyzlbfg3CuuEKlRcs ";
    string accessToken = string.Empty;

    InputField chatDialog;
    Button speechToggle; //选择当前发送信息的类型（语音或者文字）

    //public ChatUI chat; //聊天界面
    public bool isChooseSpeech = false; //判断当前的发送信息类型状态

    //语音识别模块
    string resultStr = string.Empty;
    int recordFrequency = 8000;
    AudioClip saveAudioClip; //存储语音
    string currentDeviceName = string.Empty; //当前录音设备名称
    AudioSource source;

    int recordMaxTime = 20;
    public Sprite[] _sp;
    public GameObject speechButton;

    //NLP
    AudioSource tts_source;
    string result = string.Empty;

    //发送录音保存字典
    public int myindex = 0;
    public Dictionary<int, AudioClip> myclipDic = new Dictionary<int, AudioClip>();
    //收到录音保存字典
    public int robotindex = 0;
    public Dictionary<int, AudioClip> robotclipDic = new Dictionary<int, AudioClip>();


    public Thread theAnswerThread;
    public Action<float> theCallBack;
    public bool isZhangMen;
    public override void Init()
    {
        base.Init();
        Game.Instance.StartCoroutine(_GetAccessToken());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    Unit_NLP("你吃饭了吗");
        //    JsonDecode(result);
        //}

    }

    public void Answer(string str,bool isZhangMen=true)
    {
        if (theAnswerThread != null && theAnswerThread.ThreadState != ThreadState.Stopped)
        {
            //PanelManager.Instance.OpenFloatWindow("您说话太频繁啦");
            return;
        }
        this.isZhangMen = isZhangMen;
        theAnswerThread = new Thread(new ParameterizedThreadStart(AnswerSync));
        theAnswerThread.Start(str);

    }


    void AnswerSync(object str)
    {
        lock(this)
        {
            List<string> theAnswer = new List<string>();
            string theStr = (string)str;
            Unit_NLP(theStr);

            theAnswer = JsonDecode(result);
         }
 
    }

    /// <summary>
    /// 当前处于发语音还是文字，如果是语音则AI回复语音，如果是文字则AI回复文字
    /// </summary>
    void ToChangeSpeechToggle()
    {
        if (!this.isChooseSpeech)
        {
            this.isChooseSpeech = true;
            speechToggle.GetComponent<Image>().sprite = _sp[0];
            speechButton.SetActive(true);
        }
        else
        {
            this.isChooseSpeech = false;
            speechToggle.GetComponent<Image>().sprite = _sp[1];
            speechButton.SetActive(false);
        }
    }


    /// <summary>
    /// 开始录音
    /// </summary>
    public void StartRecord()
    {
        saveAudioClip = Microphone.Start(currentDeviceName, false, recordMaxTime, recordFrequency);
    }

    /// <summary>
    /// 结束录音
    /// </summary>
    public void EndRecord()
    {
        Microphone.End(currentDeviceName);
        myclipDic.Add(myindex, saveAudioClip);
        myindex++;
        //source.PlayOneShot(saveAudioClip);
        //StartCoroutine(RequestASR());//请求语音识别
    }

    /// <summary>
    /// 请求语音识别
    /// </summary>
    /// <returns></returns>
    public IEnumerator RequestASR()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            yield return _GetAccessToken();
        }
        resultStr = string.Empty;
        //处理当前录音数据为PCM16
        float[] samples = new float[recordFrequency * 10 * saveAudioClip.channels];
        saveAudioClip.GetData(samples, 0);
        var samplesShort = new short[samples.Length];
        for (var index = 0; index < samples.Length; index++)
        {
            samplesShort[index] = (short)(samples[index] * short.MaxValue);
        }
        byte[] datas = new byte[samplesShort.Length * 2];
        Buffer.BlockCopy(samplesShort, 0, datas, 0, datas.Length);

        string url = string.Format("{0}?cuid={1}&token={2}", "https://vop.baidu.com/server_api", SystemInfo.deviceUniqueIdentifier, accessToken);

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddBinaryData("audio", datas);

        UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, wwwForm);

        unityWebRequest.SetRequestHeader("Content-Type", "audio/pcm;rate=" + recordFrequency);

        yield return unityWebRequest.SendWebRequest();

        if (string.IsNullOrEmpty(unityWebRequest.error))
        {
            resultStr = unityWebRequest.downloadHandler.text;
            if (Regex.IsMatch(resultStr, @"err_msg.:.success"))
            {
                Match match = Regex.Match(resultStr, "result.:..(.*?)..]");
                if (match.Success)
                {
                    resultStr = match.Groups[1].ToString();//语音识别的结果
                    Unit_NLP(resultStr);
                    JsonDecode(result);
                }
            }
            else
            {
                resultStr = "识别结果为空";
            }
        }
    }


    /// <summary>
    /// 返回的语音合成结果
    /// </summary>

    [System.Serializable]
    public class TtsResponse
    {
        public int error_index;
        public string error_msg;
        public string sn;
        public int idx;
        public bool Success
        {
            get { return error_index == 0; }
        }
        public AudioClip clip;
    }
    /// <summary>
    /// 请求语音合成
    /// </summary>
    /// <param name="text"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator StratTTS(string text, Action<TtsResponse> callback)
    {
        var url = "http://tsn.baidu.com/text2audio";

        var param = new Dictionary<string, string>();
        param.Add("tex", text);
        param.Add("tok", accessToken);
        param.Add("cuid", SystemInfo.deviceUniqueIdentifier);
        param.Add("ctp", "1");
        param.Add("lan", "zh");
        param.Add("spd", "5");
        param.Add("pit", "5");
        param.Add("vol", "10");
        param.Add("per", "1");
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_UWP
        param.Add("aue", "6"); //Windows设置为wav格式，移动端需要mp3格式
#endif
        int i = 0;
        foreach (var p in param)
        {
            url += i != 0 ? "&" : "?";
            url += p.Key + "=" + p.Value;
            i++;
        }
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_UWP  //根据不同平台，获取不同类型的音频格式
        var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
#else
        var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
#endif
        Debug.Log("[WitBaiduAip]" + www.url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError(www.error);
        else
        {
            var type = www.GetResponseHeader("Content-Type");
            Debug.Log("[WitBaiduAip]response type: " + type);
            if (type.Contains("audio"))
            {
                var response = new TtsResponse { clip = DownloadHandlerAudioClip.GetContent(www) };
                callback(response);
            }
            else
            {
                var textBytes = www.downloadHandler.data;
                var errorText = Encoding.UTF8.GetString(textBytes);
            }
        }
    }


    /// <summary>
    /// NLP的人工智能对话功能
    /// </summary>
    /// <param name="mysay"></param>
    /// <param name="session_id"></param>
    /// <param name="action_id"></param>
    public void Unit_NLP(string mysay, string session_id = "", string action_id = "")
    {
        string token = accessToken;
        string host = "https://unit-api.baidu.com/rpc/2.0/unit/service/v3/chat?access_token=" + token;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
        request.Method = "post";
        request.ContentType = "application/json";
        request.KeepAlive = true;
        
        JsonData send = new JsonData();
        send["version"] = "3.0";
        send["service_id"] = "S64432";
        send["log_id"] = "home";
        send["session_id"] = "home";
        send["action_id"] = "1136976";
        send["request"] = new JsonData();
        send["request"]["user_id"] = "88888";
        send["request"]["terminal_id"] = "UNIT_DEV_"+RoleManager.Instance._CurGameInfo.TheGuid;
        send["request"]["query"] = mysay;
        send["request"]["query_info"] = new JsonData();
        send["request"]["query_info"]["type"] = "TEXT";
        JsonData bot_session = new JsonData();
        bot_session["session_id"] = "";
        send["bot_session"] = JsonMapper.ToJson(bot_session);
        string sendStr = JsonMapper.ToJson(send);
        byte[] buffer = Encoding.UTF8.GetBytes(sendStr);
        request.ContentLength = buffer.Length;
        request.GetRequestStream().Write(buffer, 0, buffer.Length);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        result = reader.ReadToEnd();
    }

    /// <summary>
    /// 对NLP的回复进行Json解析
    /// </summary>
    /// <param name="js"></param>
    /// <returns></returns>
    public List<string> JsonDecode(string js)
    {
        List<string> Says = new List<string>();
        var json = JsonMapper.ToObject(js);
       
        if (json.Keys.Contains("result"))
        {
            var result =  json["result"];
            if (result.Keys.Contains("responses"))
            {
                var resArray =  result["responses"];
                var res = resArray[0];
                if (res.Keys.Contains("actions"))
                {
                    var actArray =  res["actions"];
                    var act = actArray[0];
                    if (act.Keys.Contains("say"))
                    {
                        var say = (string)act["say"];

                        if (!isChooseSpeech)
                        {
                            Debug.Log("shuole" + say);
                        }
                        else
                        {
                  
                        }
                        Says.Add(say);
                    }
                }
            }
        }
        return Says;
    }

    /// <summary>
    /// 获取accessToken请求令牌
    /// </summary>
    /// <returns></returns>
    IEnumerator _GetAccessToken()
    {
        var uri =
            string.Format(
                "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}",
                api_key, secret_key);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(uri);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.isDone)
        {
            Match match = Regex.Match(unityWebRequest.downloadHandler.text, @"access_token.:.(.*?).,");
            if (match.Success)
            {
                Debug.Log("Token已经匹配");
                accessToken = match.Groups[1].ToString();
            }
            else
            {
                Debug.Log("验证错误,获取AccessToken失败!!!");
            }
        }
    }
}
