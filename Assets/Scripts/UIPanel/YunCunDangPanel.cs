 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPLoadFTP;

public class YunCunDangPanel : PanelBase
{
    bool inGame;
    bool connectedFuWuQi = false;
    public int todayUploadArchiveNum;
    public int todayDownloadArchiveNum;
    public Text txt_todayDownloadRemain;
    public Text txt_todayUploadRemain;

    public Button btn_upload;
    public Button btn_download;

    public InputField input_handleId;
    public InputField input_code;

    public Transform trans_input;

    public override void Init(params object[] args)
    {
        base.Init(args);
 
        if (!PlayerPrefs.HasKey("todayUploadArchiveNum"))
        {
            PlayerPrefs.SetInt("todayUploadArchiveNum", 0);
        }
        if (!PlayerPrefs.HasKey("todayDownloadArchiveNum"))
        {
            PlayerPrefs.SetInt("todayDownloadArchiveNum", 0);
        }
        todayUploadArchiveNum = PlayerPrefs.GetInt("todayUploadArchiveNum");
        todayDownloadArchiveNum = PlayerPrefs.GetInt("todayDownloadArchiveNum");

        GameTimeManager.Instance.GetServiceTime((x) => 
        {
            if (x > 0)
            {
                connectedFuWuQi = true;
                todayUploadArchiveNum = PlayerPrefs.GetInt("todayUploadArchiveNum");
                todayDownloadArchiveNum = PlayerPrefs.GetInt("todayDownloadArchiveNum");
                Debug.Log("今日上传次数：todayUploadArchiveNum" + todayUploadArchiveNum);
                Debug.Log("今日下载次数：todayDownloadArchiveNum" + todayDownloadArchiveNum);
                long lastRecordedTime = 0;
                if (RoleManager.Instance._CurGameInfo.timeData != null)
                {
                    lastRecordedTime = RoleManager.Instance._CurGameInfo.timeData.LastRecordCloudArchiveTime; //这里lastrecordedtime没刷新 所以不刷新次数 要单独记录lastrecord的上传时间
                }
                //Debug.Log("上次记录的时间:" + RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime);
                //Debug.Log("上次记录的时间-24小时:" + CGameTime.Instance.GetTo24TimeStampByTimeStamp(RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime));
                long judge = x - (lastRecordedTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(lastRecordedTime));
                //Debug.Log("今天距离昨天过了多久x-的结果：" + judge);
                RoleManager.Instance._CurGameInfo.timeData.LastRecordCloudArchiveTime = x;
                if (judge > 0)
                {
                    todayUploadArchiveNum = 0;
                    todayDownloadArchiveNum = 0;
                    PlayerPrefs.SetInt("todayUploadArchiveNum", todayUploadArchiveNum);
                    PlayerPrefs.SetInt("todayDownloadArchiveNum", todayDownloadArchiveNum);

                    Debug.Log("judge大于零到了1天刷新次数");

                }
                else
                {
                    Debug.Log("judge小于零没到1天不刷新次数"  );

                }

#if UNITY_EDITOR
                //todayUploadArchiveNum = 0;
                //todayDownloadArchiveNum = 0;
#endif
                txt_todayDownloadRemain.SetText("今日剩余(" + (ConstantVal.maxUploadArchiveNum - todayDownloadArchiveNum + "/" + ConstantVal.maxUploadArchiveNum + ")"));

                txt_todayUploadRemain.SetText("今日剩余(" + (ConstantVal.maxUploadArchiveNum - todayUploadArchiveNum + "/" + ConstantVal.maxUploadArchiveNum + ")"));

            }

        });

        addBtnListener(btn_upload, () =>
        {
            //Game.Instance.isLogin = true;
            //connectedFuWuQi = true;
            if (Game.Instance.isLogin
            &&connectedFuWuQi
            &&todayUploadArchiveNum<ConstantVal.maxUploadArchiveNum)
            {
                todayUploadArchiveNum++;
                PlayerPrefs.SetInt("todayUploadArchiveNum", todayUploadArchiveNum);
                PanelManager.Instance.OpenFloatWindow("正在上传，请勿操作");
                UpLoadFiles.OnUploadArchive(ConstantVal.GetArchiveSavePath(Game.Instance.curServerIndex),ArchiveManager.Instance.ArchiveUploadName());
            }
        });
        addBtnListener(btn_download, () =>
        {
#if UNITY_EDITOR
            UpLoadFiles.OnDownloadArchive(ConstantVal.GetArchiveSaveFolder(1), ArchiveManager.Instance.ArchiveDownloadName(input_handleId.text));

#else
           //是不是gm下载存档
            if (Game.Instance.isLogin
            && connectedFuWuQi
            && todayDownloadArchiveNum < ConstantVal.maxUploadArchiveNum)
            {
                if (!string.IsNullOrWhiteSpace(input_handleId.text)
                &&!string.IsNullOrWhiteSpace(input_code.text))
                {
                    Game.Instance.clientManager.SendRT(NetCmd.EntityRpc, "DownloadArchive", input_handleId.text,input_code.text);

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TheGuid))
                    {
                        todayDownloadArchiveNum++;
                        PlayerPrefs.SetInt("todayDownloadArchiveNum", todayDownloadArchiveNum);
                        PanelManager.Instance.OpenFloatWindow("正在下载，请勿操作");
                        UpLoadFiles.OnDownloadArchive(ConstantVal.GetArchiveSaveFolder(Game.Instance.curServerIndex), ArchiveManager.Instance.ArchiveDownloadName(RoleManager.Instance._CurGameInfo.TheGuid));
                     }
                }
            }
#endif


        });
        //RegisterEvent(TheEventType.SuccessDownloadArchive, OnSuccessfulDownload);
        RegisterEvent(TheEventType.OnRequestGMDownload, OnRequestGMDownload);
        RegisterEvent(TheEventType.SuccessDownloadArchive, RefreshShow);
        RegisterEvent(TheEventType.SuccessUploadArchive, RefreshShow);

    }
    void RefreshShow()
    {
        txt_todayDownloadRemain.SetText("今日剩余(" + (ConstantVal.maxUploadArchiveNum - todayDownloadArchiveNum + "/" + ConstantVal.maxUploadArchiveNum + ")"));

        txt_todayUploadRemain.SetText("今日剩余(" + (ConstantVal.maxUploadArchiveNum - todayUploadArchiveNum + "/" + ConstantVal.maxUploadArchiveNum + ")"));

#if UNITY_EDITOR
        trans_input.gameObject.SetActive(true);
#else
        trans_input.gameObject.SetActive(false);
#endif
    }
    void OnSuccessfulDownload()
    {
        //Game.Instance.StartGame
        //重新选区
    }

    void OnRequestGMDownload()
    {
        PanelManager.Instance.OpenFloatWindow("正在下载，请勿操作");
        UpLoadFiles.OnDownloadArchive(ConstantVal.GetArchiveSaveFolder(Game.Instance.curServerIndex), ArchiveManager.Instance.ArchiveDownloadName(input_handleId.text));
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
    }
}
