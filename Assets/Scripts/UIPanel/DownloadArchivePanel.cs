using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UPLoadFTP;

public class DownloadArchivePanel : PanelBase
{
    public InputField input;
    public Button btn;
    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn, OnDownload);
    }
    public void OnDownload()
    {
        bool havePos = false;
        int posIndex = 0;
        string path = "";
        int maxArchiveNum =5;
        for (int i = 0; i < maxArchiveNum; i++)
        {
            if (!Directory.Exists(ConstantVal.GetArchiveSaveFolder(i)))
            {
                Directory.CreateDirectory(ConstantVal.GetArchiveSaveFolder(i));
            }
            if (!File.Exists(ConstantVal.GetArchiveSavePath(i)))
            {
                posIndex = i;
                path = ConstantVal.GetArchiveSaveFolder(i);
                havePos = true;
                break;
            }
        }
        if (!havePos)
        {
            PanelManager.Instance.OpenFloatWindow("当前无空位，请清理存档位");
        }
        else
        {
            PanelManager.Instance.OpenFloatWindow("下载中，请等待10秒再操作");

            UpLoadFiles.OnDownloadArchive(path, ArchiveManager.Instance.ArchiveDownloadName(input.text));

            PanelManager.Instance.ClosePanel(this);

        }

    }
}
