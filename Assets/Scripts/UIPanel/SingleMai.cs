using DG.Tweening;
using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleMai : MonoBehaviour
{
    public JingMaiIDType jingMaiIDType;
    public Image img_self;//自己
    public Image img_txt;//文字
    public Image img_timeBar;//时间bar
    public Button btn;

    public bool clicked = false;//点到了
    public BattlePanel parentPanel;
    public GameObject obj_fire;
    public bool light;//点亮
    bool isCD;//正在cd
    int totalCD;//总cd
    int remainCD;//剩余cd
    public Image img_cd;
    
    public void Init(BattlePanel parentPanel)
    {
        this.parentPanel = parentPanel;

        img_self.DOKill();
        img_txt.DOKill();
        img_timeBar.DOKill();
        img_self.color = new Color(img_self.color.r, img_self.color.g, img_self.color.b, 0);
        img_txt.color = new Color(img_txt.color.r, img_txt.color.g, img_txt.color.b, 0);
        img_txt.transform.localScale = new Vector3(1, 1, 1);
        img_timeBar.color = new Color(img_txt.color.r, img_txt.color.g, img_txt.color.b, 0);
        img_timeBar.fillAmount = 1;
        btn.enabled = false;
        img_txt.raycastTarget = false;

        obj_fire.gameObject.SetActive(false);
        clicked = false;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);
        light = false;
        isCD = false;
        img_cd.gameObject.SetActive(false);
    }

    /// <summary>
    /// 点亮
    /// </summary>
    public void OnLight(bool first)
    {
        btn.enabled = true;
        img_txt.raycastTarget = true;
        img_self.DOFade(1, 1f);
        img_txt.DOFade(1, 1f);
        if (!first)
        {
            img_timeBar.DOFade(1, 1f).OnComplete(() =>
            {
                img_timeBar.DOFillAmount(0, 2f).OnComplete(() =>
                {

                    if (!clicked)
                    {
                        //消失
                        btn.enabled = false;
                        img_txt.raycastTarget = false;

                        img_self.DOFade(0, .1f);
                        img_txt.DOFade(0, .1f);

                        img_timeBar.DOFade(0, .1f);
                        parentPanel.OnLoseClickMai();
                    }
                });

            });
        }
        else
        {
   
        }
        light = true;
    }

    /// <summary>
    /// 点击了
    /// </summary>
    void OnClicked()
    {
        if (isCD)
        {
            PanelManager.Instance.OpenFloatWindow("冷却中，还需" + remainCD + "回合");
            return;
        }
        img_self.DOKill();
        img_timeBar.DOKill();
        img_txt.DOKill();

        img_txt.transform.DOScale(3f, .4f).SetEase(parentPanel.qteTxtEase);
        img_txt.DOFade(0, .8f);
        img_self.DOFade(0, .5f);
     
        img_timeBar.DOFade(0, .5f);
        clicked = true;
        btn.enabled = false;
        img_txt.raycastTarget = false;
        //点到 发消息
        parentPanel.OnClickedMai(this);
        obj_fire.gameObject.SetActive(true);

    }

    /// <summary>
    /// 熄灭
    /// </summary>
    public void Disappear()
    {
        btn.enabled = false;
        img_txt.raycastTarget = false;
        img_txt.DOKill();
        img_self.DOKill();
        img_timeBar.DOKill();

        obj_fire.gameObject.SetActive(false);
        img_txt.DOFade(0, .2f).OnComplete(() =>
        {
            img_txt.transform.localScale = new Vector3(1, 1, 1);
        });
        img_self.DOFade(0, .2f);
        img_timeBar.DOFade(0, .2f);
        img_cd.gameObject.SetActive(false);
        light = false;
    }

    public void ShowCD(SingleSkillData singleSkillData)
    {
        if (singleSkillData.cd > 0)
        {
            isCD = true;
            totalCD = DataTable.FindSkillSetting(singleSkillData.skillId).Cd.ToInt32();
            remainCD = singleSkillData.cd;

            img_cd.gameObject.SetActive(true);
            img_cd.fillAmount = remainCD / (float)totalCD;
        }
    }
}
