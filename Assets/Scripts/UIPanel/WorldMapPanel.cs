using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapPanel : PanelBase
{
    public Button btn_empty;
    public List<SingleMapView> singleMapViewList;

    public List<Image> worldImgList;//世界的图
    public List<Image> lieXiImgList;//裂隙的图

    public Button btn_switch;//切换

    public bool isWorld = true;//是世界

    public Transform trans_duJie;//渡劫

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_empty, () =>
        {
            CloseAllMapContent();
        });
        addBtnListener(btn_switch, () =>
        {
            if (isWorld)
            {
                SwitchToLieXi();
            }
            else
            {
                SwitchToWorld();
            }
        });
        isWorld = true;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        for(int i = 0; i < singleMapViewList.Count; i++)
        {
            singleMapViewList[i].Init(this);
            singleMapViewList[i].InitShow();
        }
        ShowGuide();

        for (int i = 0; i < worldImgList.Count; i++)
        {
            Image img = worldImgList[i];
            img.DOKill();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }

        for (int i = 0; i < lieXiImgList.Count; i++)
        {
            Image img = lieXiImgList[i];
            img.DOKill();

            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        }
        trans_duJie.gameObject.SetActive(false);
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_passFixLevel)
        {
            for (int i = 0; i < singleMapViewList.Count; i++)
            {
                SingleMapView singleMapView = singleMapViewList[i];
                if (singleMapView.mapSetting.MapLevel.ToInt32() ==TaskManager.Instance.guide_passFixLevelMapId)
                {
                    PanelManager.Instance.ShowTaskGuidePanel(singleMapView.btn.gameObject);
                }
            }
        }

    }



    /// <summary>
    /// 切换到世界
    /// </summary>
    public void SwitchToWorld()
    {
        for (int i = 0; i < worldImgList.Count; i++)
        {
            Image img = worldImgList[i];
            img.DOKill();
            img.DOFade(1, 1f);
        }

        for (int i = 0; i < lieXiImgList.Count; i++)
        {
            Image img = lieXiImgList[i];
            img.DOKill();

            img.DOFade(0, 1f);
        }
        isWorld = true;
        trans_duJie.gameObject.SetActive(false);
        AuditionManager.Instance.PlayBGM(AudioClipType.BGM_YunHaiZong);


    }
    /// <summary>
    /// 切换到裂隙
    /// </summary>
    public void SwitchToLieXi()
    {
        for(int i = 0; i < worldImgList.Count; i++)
        {
            Image img = worldImgList[i];
            img.DOKill();
            img.DOFade(0, 1f);
        }

        for (int i = 0; i < lieXiImgList.Count; i++)
        {
            Image img = lieXiImgList[i];
            img.DOKill();

            img.DOFade(1, 1f);
        }
        isWorld = false;
        CloseAllMapContent();
        trans_duJie.gameObject.SetActive(true);
        AuditionManager.Instance.PlayBGM(AudioClipType.BGM_WorldMap);
    }

    /// <summary>
    /// 选择了某个图
    /// </summary>
    public void OnChoosedMap(SingleMapView singleMapView)
    {
         
        if (singleMapView.settingId > 10004)
        {
            PanelManager.Instance.OpenFloatWindow("该章节尚未开放");
            return;
        }
        for (int i = 0; i < singleMapViewList.Count; i++)
        {
            SingleMapView theView = singleMapViewList[i];
            if (theView == singleMapView)
            {
                theView.OnChoose(true);
            }
            else
            {
                theView.OnChoose(false);
            }
        }
    }

    /// <summary>
    /// 关闭所有地图内容
    /// </summary>
    public void CloseAllMapContent()
    {
        for (int i = 0; i < singleMapViewList.Count; i++)
        {
            SingleMapView theView = singleMapViewList[i];
            theView.OnChoose(false);

        }
    }
}
