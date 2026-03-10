using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TesRdmStudentGaiLv : MonoBehaviour
{
    public List<Text> pRarityList;

    public List<Text> tianFuList;

    public List<Text> totalList;

    public Button btn_rdmP;
    public Button btn_rdmTianFu;

    private void Awake()
    {
        DataTable.LoadTableData();
    }
    // Start is called before the first frame update
    void Start()
    {
        btn_rdmP.onClick.AddListener(() =>
        {
            List<Rarity> rarityList = new List<Rarity> { Rarity.Fan, Rarity.Huang, Rarity.Xuan, Rarity.Di, Rarity.Tian };

            List<int> weightList = new List<int> { 30, 40, 18, 10, 2 };
            for(int i = 0; i < 3; i++)
            {
                int rarityIndex = CommonUtil.GetIndexByWeight(weightList);
                int rarity = (int)rarityList[rarityIndex];
                pRarityList[i].SetText(CommonUtil.RarityName((Rarity)rarity));
                pRarityList[i].color = CommonUtil.QualityColor((Quality)rarity);
                totalList[rarity - 1].SetText((totalList[rarity - 1].text.ToInt32() + 1).ToString());
            }
  
        });
        btn_rdmTianFu.onClick.AddListener(() =>
        {
            //觉醒一个天赋
            List<int> candidateTalents = new List<int>
        {
            1,2,3,4,5,6,7,8
        };
            //天赋权重
            List<int> talentWeight = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1 };

            //觉醒一个天赋
            List<int> candidateTalentQuality = new List<int>
        {
            1,2,3,4,5
        };
            List<int> weightList = new List<int> { 15, 25, 30, 20, 10 };
            for (int i = 0; i < 3; i++)
            {
                int rarityIndex = CommonUtil.GetIndexByWeight(weightList);
                int rarity = (int)candidateTalentQuality[rarityIndex];
                int talent = candidateTalents[RandomManager.Next(0, candidateTalents.Count)];
                tianFuList[i].SetText(CommonUtil.RarityName((Rarity)rarity)+":"+StudentManager.Instance.TalentNameByTalent((StudentTalent)talent));
                tianFuList[i].color = CommonUtil.QualityColor((Quality)rarity);
            }

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
