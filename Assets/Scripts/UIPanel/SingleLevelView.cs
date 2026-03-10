using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Spine.Unity;

public class SingleLevelView : SingleViewBase
{
    public Button btn; 
    public GameObject imagePrefab; // 脚步图片
    public GameObject footstepPanel; // 脚步图片父节点
    private List<GameObject> spawnedFootsteps = new List<GameObject>();
    public Text txt_name;
    public Transform trans_lock;
    public string settingId;
    public SingleLevelData singleLevelData;
    public LevelSetting levelSetting;
    public MapSceneType mapSceneType;
    //public Image img;
    public SkeletonGraphic ske;
    public List<Sprite> guankaStatus;
    public Image img_gkstatu;

    public GameObject obj_accomplished;

    public override void Init(params object[] args)
    {
        base.Init(args);
        mapSceneType = (MapSceneType)args[0];
        guankaStatus.Clear();
        for (int i = 0; i < 3; i++)
        {
            guankaStatus.Add(ResourceManager.Instance.GetObj<Sprite>(ConstantVal.guankStatus + (i+1)));
        }

        levelSetting = DataTable.FindLevelSetting(settingId);
        if (mapSceneType == MapSceneType.XianMen)
            singleLevelData = MapManager.Instance.FindLevelById(settingId);
        else
            singleLevelData = MapManager.Instance.FindFixedLevelById(settingId);

        addBtnListener(btn, () =>
        {
            if (singleLevelData.LevelStatus == (int)AccomplishStatus.Accomplished)
            {
                PanelManager.Instance.OpenFloatWindow("该关卡已通关");
            }
            else
            {
                //新手引导限制
                if (levelSetting.Level == "0-6")
                {
                    if (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel < 6)
                    {
                        PanelManager.Instance.OpenFloatWindow(LanguageIdType.宗门+ "6级后可继续挑战");
                        return;
                    }
                }
   
                if (singleLevelData.LevelStatus == (int)AccomplishStatus.Locked)
                {
                    PanelManager.Instance.OpenFloatWindow("请先通过前面的关卡");
                    return;
                }
                switch ((LevelType)levelSetting.Leveltype.ToInt32())
                {
                    case LevelType.ZhongZhuanZhan:
                         MapManager.Instance.ZhongZhuanZhanRest(settingId);
                        break;
                    default:
                                               OpenBattlePreparePanel();

                        break;

                }

            }


        });
        RegisterEvent(TheEventType.OnSaoDang, OnSaoDang);
    }
 

    public void OpenBattlePreparePanel()
    {
        BattleType battleType;

        MapManager.Instance.curChoosedLevelId = settingId;
        if (levelSetting.IsFixed == "1")
        {
            battleType = BattleType.FixedLevelBattle;

        }
        else
            battleType = BattleType.LevelBattle;

        List<PeopleData> myList = RoleManager.Instance.FindMyBattleTeamList(false, 0);
        List<PeopleData> enemyList = new List<PeopleData>();
        for (int i = 0; i < singleLevelData.Enemy.Count; i++)
        {
            enemyList.Add(singleLevelData.Enemy[i]);
        }
        PanelManager.Instance.OpenPanel<BattlePreparePanel>(PanelManager.Instance.trans_layer2,
            battleType,
            myList,
            enemyList);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void InitShow()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        LevelSetting setting = DataTable.FindLevelSetting(settingId);
        txt_name.SetText(setting.Level);



        bool unlock;
        if (setting.IsFixed=="1")
        {
            unlock = MapManager.Instance.CheckIfUnlockFixedLevel(settingId);
        }
        else
        {
            unlock = MapManager.Instance.CheckIfUnlockLevel(settingId);

        }
        //switch ((LevelType)setting.leveltype.ToInt32())
        //{
        //    case LevelType.Battle:
        //        ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.mapFolderPath + ConstantVal.commonBattleLevelSke);
        //        break;
        //    case LevelType.JingYingBattle:
        //        ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.mapFolderPath + ConstantVal.jingYingBattleLevelSke);
        //        break;
        //    case LevelType.ZhongZhuanZhan:
        //        ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.mapFolderPath + ConstantVal.commonBattleLevelSke);
        //        break;
        //    case LevelType.BossBattle:
        //        ske.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.mapFolderPath + ConstantVal.bossBattleLevelSke);
        //        break;
        //}
        //ske.Initialize(true);

        if(singleLevelData.LevelStatus == (int)AccomplishStatus.Accomplished)
        {
            //obj_accomplished.SetActive(true);
            //switch ((LevelType)setting.Leveltype.ToInt32())
            //{
            //    case LevelType.Battle:
            //        ske.AnimationState.SetAnimation(0, "fu2", false);
            //        break;
            //    case LevelType.JingYingBattle:
            //        ske.AnimationState.SetAnimation(0, "fu2", false);
            //        break;
            //    case LevelType.ZhongZhuanZhan:
            //        ske.AnimationState.SetAnimation(0, "zhongzhuanzhan2", false);
            //        break;
            //    case LevelType.BossBattle:
            //        ske.AnimationState.SetAnimation(0, "fu2", false);
            //        break;
            //}
        }
        else
        {
          
            //obj_accomplished.SetActive(false);
            //switch ((LevelType)setting.Leveltype.ToInt32())
            //{
            //    case LevelType.Battle:
            //        ske.AnimationState.SetAnimation(0, "xiaoguai", false);
            //        break;
            //    case LevelType.JingYingBattle:
            //        ske.AnimationState.SetAnimation(0, "jyg_2", false);
            //        break;
            //    case LevelType.ZhongZhuanZhan:
            //        ske.AnimationState.SetAnimation(0, "zhongzhuanzhan", false);
            //        break;
            //    case LevelType.BossBattle:
            //        ske.AnimationState.SetAnimation(0, "jyg_2", false);
            //        break;
            //}
        }
        Showgkstatus((AccomplishStatus)singleLevelData.LevelStatus);
    }

    /// <summary>
    /// 扫荡
    /// </summary>
    /// <param name="args"></param>
    void OnSaoDang(object[] args)
    {
        BattleType battleType = (BattleType)args[0];
        if (battleType == BattleType.LevelBattle)
        {
            string levelId = (string)args[1];
            if (levelId == singleLevelData.LevelId
                &&levelSetting.Leveltype.ToInt32()!=(int)LevelType.ZhongZhuanZhan)
            {
                ShowAccomplishLevelAnim();
            }
        }
    }
    /// <summary>
    /// 完成关卡动画显示
    /// </summary>
    public void ShowAccomplishLevelAnim()
    {
        //ske.AnimationState.SetAnimation(0, "fu", false);
        img_gkstatu.sprite = guankaStatus[2];
    }
    /// <summary>
    /// 生成脚步预制体
    /// </summary>
    public void CompletedLevelStepDisplay(SingleLevelView prevLevel)
    {
        ClearFootstepPrefabs(); // 先清理自身已生成的脚步
        RectTransform prevBtnRect = prevLevel.GetComponent<RectTransform>();
        RectTransform currBtnRect = GetComponent<RectTransform>();
        Vector3 posA = prevBtnRect.anchoredPosition;
        Vector3 posB = currBtnRect.anchoredPosition;

        footstepPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(-posB.x, -posB.y, posB.z);

        SpawnThreeFootstepsBetween(posA, posB); // 生成3个均匀分布的脚步
        Debug.Log($"posA={posA} | posB={posB}");
    }

    /// <summary>
    /// 固定生成3个100x100的脚步，均匀分布在两关卡之间且逐步倾斜，朝向当前关卡
    /// </summary>
    /// <param name="posA">上一关卡坐标</param>
    /// <param name="posB">当前关卡坐标</param>
    private void SpawnThreeFootstepsBetween(Vector2 posA, Vector2 posB)
    {
        // 1. 计算朝向当前关卡的方向向量（当前关卡 - 上一关卡）
        Vector2 directionToCurrentLevel = posB - posA;
        // 2. 计算基础角度（朝向当前关卡），并适配UI坐标系的旋转规则
        float baseAngle = Mathf.Atan2(directionToCurrentLevel.y, directionToCurrentLevel.x) * Mathf.Rad2Deg -90;
        // 补充：若脚步图片本身是「向上/向右」的默认朝向，需根据实际素材调整偏移（比如+90°）
        // 示例：如果脚步素材默认是向上（UI坐标系上），则 baseAngle += 90f;

        // 3. 定义每个脚步的倾斜偏移（逐步向当前关卡方向倾斜，幅度可调整）
        // 第一个脚步略左倾，第二个正朝向，第三个略右倾，模拟「逐步转向当前关卡」
        float[] angleOffsets = new float[] { -2f, 0f, 2f };

        // 4. 4等份间距，3个脚步分别在1/4、2/4、3/4位置，避免重叠
        float intervalRatio = 1f / 4f;

        for (int i = 1; i <= 2; i++)
        {
            float t = intervalRatio * i;
            // 线性插值计算每个脚步的坐标
            Vector2 spawnPos = Vector2.Lerp(posA, posB, t);

            GameObject footstep = Instantiate(imagePrefab, footstepPanel.transform);
            RectTransform footstepRect = footstep.GetComponent<RectTransform>();

            // 设置脚步坐标和大小
            footstepRect.anchoredPosition = spawnPos;
            footstepRect.sizeDelta = new Vector2(100, 100);

            // 5. 计算最终角度（基础朝向 + 逐步倾斜偏移）
            float finalAngle = baseAngle + angleOffsets[i - 1];
            // 应用旋转（UGUI绕Z轴旋转，确保朝向当前关卡）
            footstepRect.rotation = Quaternion.Euler(0, 0, finalAngle);

            spawnedFootsteps.Add(footstep);
        }
    }

    /// <summary>
    /// 清理当前关卡生成的所有脚步预制体
    /// </summary>
    public void ClearFootstepPrefabs()
    {
        foreach (var footstep in spawnedFootsteps)
        {
            if (footstep != null)
                Destroy(footstep);
        }
        spawnedFootsteps.Clear();
    }
    public void Showgkstatus(AccomplishStatus accomplishStatus) {
        switch (accomplishStatus)
        {
            case AccomplishStatus.None:
                break;
            case AccomplishStatus.Locked:
                img_gkstatu.sprite = guankaStatus[0];
                break;
            case AccomplishStatus.UnAccomplished:
                //img_gkstatu.sprite = guankaStatus[0];
                break;
            case AccomplishStatus.Processing:  
                img_gkstatu.sprite = guankaStatus[1];
                break;
            case AccomplishStatus.Accomplished:
                img_gkstatu.sprite = guankaStatus[2];
                break;
            case AccomplishStatus.GetAward:
                break;
            default:
                break;
        }
    }
}
