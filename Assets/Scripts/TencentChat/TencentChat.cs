using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Nlp.V20190408;
using TencentCloud.Nlp.V20190408.Models;
using UnityEngine;

public class TencentChat : CommonInstance<TencentChat>
{

    public Thread theAnswerThread;
    public Action<float> theCallBack;
    public bool isZhangMen;


    public void Answer(string str, bool isZhangMen = true)
    {
        if (theAnswerThread != null && theAnswerThread.ThreadState != ThreadState.Stopped)
        {
            //PanelManager.Instance.OpenFloatWindow("您说话太频繁啦");
            return;
        }
        this.isZhangMen = isZhangMen;
        theAnswerThread = new Thread(new ParameterizedThreadStart(AnswerSync));
        theAnswerThread.Start(str);
        //string theAnswer =SearchForStr(str);
        //ChatManager.Instance.GetStudentAnswer(new List<string> { theAnswer }, isZhangMen);

    }


    void AnswerSync(object str)
    {
        lock (this)
        {
            string theAnswer ="";
            string theStr = (string)str;
            theAnswer= SearchForStr(theStr);
            theAnswer = JsonDecode(theAnswer);
            Debug.Log("回复" + theAnswer);
         }

    }

    public string SearchForStr(string str)
    {
        try
        {
            Credential cred = new Credential
            {
                SecretId = "AKIDxkHYYHitnQmrfCSRAcmfIeiV2IHyPZR2",
                SecretKey = "LQElQqzrSnreAPO6XhaDZeT0GFxPaU7T"
            };

            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ("nlp.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;

            NlpClient client = new NlpClient(cred, "ap-guangzhou", clientProfile);
            ChatBotRequest req = new ChatBotRequest();
            req.Query = str;
           ChatBotResponse resp = client.ChatBotSync(req);
            //Console.WriteLine(AbstractModel.ToJsonString(resp));
            return AbstractModel.ToJsonString(resp);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return "";

        }
    }

    /// <summary>
    /// 对NLP的回复进行Json解析
    /// </summary>
    /// <param name="js"></param>
    /// <returns></returns>
    public string JsonDecode(string js)
    {
        var json = JsonConvert.DeserializeObject<AnswerModel>(js);

        //if (json.Keys.Contains("Reply"))
        //{
        //    var result = (string)json["Reply"];
        //    return result;
        //}
        return json.Reply;
        //return "";
    }
}

public class AnswerModel
{
    public string Reply;
    public string Confidence;
    public string RequestId;
}
