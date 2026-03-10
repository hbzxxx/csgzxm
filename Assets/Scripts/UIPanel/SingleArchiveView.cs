
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UPLoadFTP;

public class SingleArchiveView : SingleViewBase
{
    public bool haveArchive;
    public bool recent = false;
    public int index;
    public GameInfo gameInfo;

    public GameObject obj_icon;
    public Portrait portrait;
    public Text txt_name;
    public Text txt_zongMenName;//宗门名
    public Text txt_gameTime;//游戏时间
    public Text txt_archiveTime;//存档时间
    public Text txt_zongMenLevel;//宗门等级
    public Text txt_quIndex;//第几区

    public GameObject obj_recent;//最近存档

    public Transform trans_haveArchive;//有存档

    public Transform trans_noArchive;//没存档

    public Button btn;//

    public Button btn_upload;
    public ArchivePanel parentPanel;
 
    public override void Init(params object[] args)
    {
        base.Init(args);
        haveArchive = (bool)args[0];
        index = (int)args[1];
        if (haveArchive)
        {
            gameInfo = args[2] as GameInfo;
            recent = (bool)args[3];
        }
        parentPanel = args[4] as ArchivePanel;

        addBtnListener(btn, () =>
        {
            if (haveArchive)
            {
             
                PanelManager.Instance.OpenCommonHint("掌门：" + gameInfo.playerPeople.name + "\n" + txt_archiveTime.text,()=>
                {    
                    //直接进入该区服
                    PanelManager.Instance.OpenPanel<LoadingPanel>(PanelManager.Instance.trans_layer3);
                    EventCenter.Broadcast(TheEventType.ChooseQu, index);
                    PanelManager.Instance.ClosePanel(parentPanel);
                    //Game.Instance.StartGame(index);
               
                },
                ()=> 
                {
                    PanelManager.Instance.OpenCommonHint("是否删除存档:" + "掌门：" + gameInfo.playerPeople.name + "?（此存档为本地存档，一旦删除将无法找回，包括充值、养成数据。非坏档情况不建议删除此存档，如果您有疑问，可及时咨询客服。）", () =>
                    {
                        PanelManager.Instance.OpenCommonHint("真的确定删除吗，选择后不能后悔哦。", () =>
                        {
                            ArchiveManager.Instance.DeleteArchive(index);

                        }, null);
                    }, null);


                }, "进入", "删除");
            }
            else
            {
                PanelManager.Instance.OpenPanel<LoadingPanel>(PanelManager.Instance.trans_layer3);
                PanelManager.Instance.ClosePanel(parentPanel);

                EventCenter.Broadcast(TheEventType.ChooseQu, index);
                //PanelManager.Instance.OpenCommonHint("确定在该位置开新档进行游戏吗?", () =>
                //{
                //    Game.Instance.StartGame(index);

                //}, null);
            }
        });

        addBtnListener(btn_upload, () =>
        {

            using (var input = File.OpenRead(ConstantVal.GetArchiveSavePath(index)))
            {
                //GameInfo gameInfo = GameInfo.Parser.ParseFrom(input);
                UpLoadFiles.OnUploadArchive( ConstantVal.GetArchiveSavePath(index),  ArchiveManager.Instance.ArchiveUploadName());
                
            }
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (haveArchive)
        {
            trans_haveArchive.gameObject.SetActive(true);
            trans_noArchive.gameObject.SetActive(false);
            
            //已捏脸和名字
           if(gameInfo.playerPeople.portraitIndexList.Count > 0)
            {
                txt_name.SetText(gameInfo.playerPeople.name);
                txt_zongMenName.SetText(gameInfo.allZongMenData.ZongMenName);
                obj_icon.gameObject.SetActive(false);
                portrait.gameObject.SetActive(true);
                portrait.Refresh(gameInfo.playerPeople);
                
            }
            else
            {
                txt_name.SetText(gameInfo.playerPeople.name);
                txt_zongMenName.SetText(gameInfo.allZongMenData.ZongMenName);
                obj_icon.gameObject.SetActive(true);
                portrait.gameObject.SetActive(false);
            }
            txt_zongMenLevel.SetText(LanguageUtil.GetLanguageText((int)LanguageIdType.宗门)+"等级" + gameInfo.allZongMenData.ZongMenLevel);
            txt_gameTime.SetText("游戏时间：" + gameInfo.timeData.Year + "年" + gameInfo.timeData.Month + "月" + gameInfo.timeData.Week + "周");
            txt_archiveTime.SetText("存档时间:" + CGameTime.Instance.GetDateTimeByTimeStamp(gameInfo.SaveTime));
            obj_recent.gameObject.SetActive(recent);
        }
        else
        {
            trans_haveArchive.gameObject.SetActive(false);
            trans_noArchive.gameObject.SetActive(true);
        }
        txt_quIndex.SetText((index-2) + "区");
    }
}
