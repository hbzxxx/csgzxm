using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 天赋飞过去的动画
/// </summary>
public class FlyTalentView : SingleViewBase
{
    public Transform trans_animParent;//特效父物体
    public Quality quality;
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.position = (Vector3)args[0];
        quality = (Quality)args[1];
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        GameObject prefab = null;
        switch (quality)
        {
            case Quality.Green:
                 prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_green_fly);
                break;
            case Quality.Blue:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_blue_fly);
                break;
            case Quality.Purple:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_purple_fly);
                break;
            case Quality.Orange:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_orange_fly);
                break;
            case Quality.Gold:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_gold_fly);
                break;

        }
        GameObject obj = Instantiate(prefab);
        obj.transform.SetParent(this.transform, false);


    }

    /// <summary>
    /// 炸
    /// </summary>
    public void OnBoom()
    {
        GameObject prefab = null;

        switch (quality)
        {
            case Quality.Green:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_green_boom);
                break;
            case Quality.Blue:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_blue_boom);
                break;
            case Quality.Purple:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_purple_boom);
                break;
            case Quality.Orange:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_orange_boom);
                break;
            case Quality.Gold:
                prefab = ResourceManager.Instance.GetObj<GameObject>(ConstantVal.talentTestPath + ConstantVal.talent_gold_boom);
                break;

        }
        GameObject obj = Instantiate(prefab);
        obj.transform.SetParent(this.transform, false);
    }
    public override void Clear()
    {
        base.Clear();
        int num = transform.childCount;
        for(int i= num - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
