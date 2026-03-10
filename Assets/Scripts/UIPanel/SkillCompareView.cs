using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
/// <summary>
/// 技能升级比较
/// </summary>
public class SkillCompareView : SingleViewBase
{
    SkillSetting skillSetting;
    
    int curLevel;

    public Text txt_label;
    public Text txt_curLevelDes;
    public Text txt_afterLevelDes;
    public RectTransform rect;
    public RectTransform rectTrans_label;//标题
    public Transform trans_centerPos;
    public Transform trans_leftPos;

    public float iconHeight = 135;//icon高度
    public override void Init(params object[] args)
    {
        base.Init(args);
        skillSetting = args[0] as SkillSetting;
        curLevel = (int)args[1];

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        string upgradeDesStr = skillSetting.UpgradeDesArr;
        string[] desArr = null;
        //多段伤害

        txt_label.SetText(skillSetting.Name);
        desArr = upgradeDesStr.Split('|');


        int curIndex = curLevel / 5;
        int nextIndex = (curLevel + 1) / 5;
        
        //因为可能技能到某个阶段就不升级了 未免报错 暂时如此处理 TODO后续技能升级的节点需要设定在装备Setting上

        txt_curLevelDes.SetText(desArr[curIndex]);

        //如果技能不升级
        if (nextIndex == curIndex)
        {
            txt_afterLevelDes.gameObject.SetActive(false);
            txt_curLevelDes.transform.position = trans_centerPos.position;
        }
        else
        {
            txt_afterLevelDes.gameObject.SetActive(true);
            txt_curLevelDes.transform.position = trans_leftPos.position;
            txt_afterLevelDes.SetText(desArr[nextIndex]);
            txt_afterLevelDes.GetComponent<RectTransform>().sizeDelta = new Vector2(txt_afterLevelDes.GetComponent<RectTransform>().sizeDelta.x,
    txt_afterLevelDes.preferredHeight);
        }

        txt_curLevelDes.GetComponent<RectTransform>().sizeDelta = new Vector2(txt_curLevelDes.GetComponent<RectTransform>().sizeDelta.x,
         txt_curLevelDes.preferredHeight);

       
        float height = Mathf.Max(iconHeight, txt_curLevelDes.GetComponent<RectTransform>().sizeDelta.y);

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);


    }




}
