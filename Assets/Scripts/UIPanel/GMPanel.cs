using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
 

public class GMPanel : MonoBehaviour
{
    public Button btn_addPro;

    public InputField input_itemId;
    public InputField input_itemCount;
    public Button btn_getItem;

    public Button btn_buji;

    public bool battleWin;//战斗必胜
    public Button btn_studentExpFull;//学生经验全满

    public InputField input_jumpToMonth;//跳到该月
    public Button btn_jumpToMonth;//跳到该月

    public Button btn_save;

    public Button btn_leavBigMap;//离开大地图


    public InputField input_loseItemId;
    public InputField input_loseItemCount;
    public Button btn_loseItem;

    public Button btn_addXiuWei;//增加修为

    public Button btn_unlockAllFarm;//解锁所有房

    public InputField input_buildId;
    public Button btn_unlockBuild;//解锁建筑id

    public Toggle hide;
    public Transform content;

    public Button btn_testCharge;

    public Button btn_haoGan;

    public Button btn_uploadArchive;

    private void Awake()
    {
        btn_addPro.onClick.AddListener(() =>
        {
             RoleManager.Instance.AddProperty(PropertyIdType.MPSpeed, 8);
         

        });

        btn_getItem.onClick.AddListener(() =>
        {
            int id = input_itemId.text.ToInt32();
            int count = input_itemCount.text.ToInt32();

            ItemSetting setting = DataTable.FindItemSetting(id);

            if (setting == null)
            {
                Debug.LogError("没有id为" + id + "的物品");
            }
            else
            {
                if (setting.ItemType.ToInt32() == (int)ItemType.Gem)
                {
                    ItemManager.Instance.AddANewGem(id, null,false,Quality.Purple);
                }
                else if (setting.ItemType.ToInt32() == (int)ItemType.Equip)
                {
                    EquipmentManager.Instance.AddEquipPicture(setting.Id.ToInt32());
                }
                else
                {
                    ItemManager.Instance.GetItem(id, (ulong)count);

                }
            }
        });

        btn_buji.onClick.AddListener(() =>
        {
            ItemManager.Instance.GetItem((int)ItemIdType.LingShi, 55555555);
 
            ItemManager.Instance.GetItem(91001, 555555);
            ItemManager.Instance.GetItem(92001, 555555);
            ItemManager.Instance.GetItem(93001, 555555);
            ItemManager.Instance.GetItem(94001, 555555);
            ItemManager.Instance.GetItem(95001, 555555);
            ItemManager.Instance.GetItem(111001, 555555);
            ItemManager.Instance.GetItem(112001, 555555);
            ItemManager.Instance.GetItem(113001, 555555);
            ItemManager.Instance.GetItem(114001, 555555);
            ItemManager.Instance.GetItem(115001, 555555);

            ItemManager.Instance.GetItem(121001, 555555);
            ItemManager.Instance.GetItem(121002, 555555);
            ItemManager.Instance.GetItem(121003, 555555);
            ItemManager.Instance.GetItem(121004, 555555);
            ItemManager.Instance.GetItem(121005, 555555);
            ItemManager.Instance.GetItem(121006, 555555);
            ItemManager.Instance.GetItem(121007, 555555);

            ItemManager.Instance.GetItem(122001, 555555);
            ItemManager.Instance.GetItem(122002, 555555);
            ItemManager.Instance.GetItem(122003, 555555);
            ItemManager.Instance.GetItem(122004, 555555);
            ItemManager.Instance.GetItem(122005, 555555);
            ItemManager.Instance.GetItem(122006, 555555);
            ItemManager.Instance.GetItem(122007, 555555);

            ItemManager.Instance.GetItem(123001, 555555);
            ItemManager.Instance.GetItem(123002, 555555);
            ItemManager.Instance.GetItem(123003, 555555);
            ItemManager.Instance.GetItem(123004, 555555);
            ItemManager.Instance.GetItem(123005, 555555);
            ItemManager.Instance.GetItem(123006, 555555);
            ItemManager.Instance.GetItem(123007, 555555);

            ItemManager.Instance.GetItem(124001, 555555);
            ItemManager.Instance.GetItem(124002, 555555);
            ItemManager.Instance.GetItem(124003, 555555);
            ItemManager.Instance.GetItem(124004, 555555);
            ItemManager.Instance.GetItem(124005, 555555);
            ItemManager.Instance.GetItem(124006, 555555);
            ItemManager.Instance.GetItem(124007, 555555);

            ItemManager.Instance.GetItem(125001, 555555);
            ItemManager.Instance.GetItem(125002, 555555);
            ItemManager.Instance.GetItem(125003, 555555);
            ItemManager.Instance.GetItem(125004, 555555);
            ItemManager.Instance.GetItem(125005, 555555);
            ItemManager.Instance.GetItem(125006, 555555);
            ItemManager.Instance.GetItem(125007, 555555);

            ItemManager.Instance.GetItem(131001, 555555);
            ItemManager.Instance.GetItem(132001, 555555);
            ItemManager.Instance.GetItem(133001, 555555);
            ItemManager.Instance.GetItem(134001, 555555);
            ItemManager.Instance.GetItem(135001, 555555);

            ItemManager.Instance.GetItem(131101, 555555);
            ItemManager.Instance.GetItem(132101, 555555);
            ItemManager.Instance.GetItem(133101, 555555);
            ItemManager.Instance.GetItem(134101, 555555);
            ItemManager.Instance.GetItem(135101, 555555);

            ItemManager.Instance.GetItem(131201, 555555);
            ItemManager.Instance.GetItem(132201, 555555);
            ItemManager.Instance.GetItem(133201, 555555);
            ItemManager.Instance.GetItem(134201, 555555);
            ItemManager.Instance.GetItem(135201, 555555);

            ItemManager.Instance.GetItem(131301, 555555);
            ItemManager.Instance.GetItem(132301, 555555);
            ItemManager.Instance.GetItem(133301, 555555);
            ItemManager.Instance.GetItem(134301, 555555);
            ItemManager.Instance.GetItem(135301, 555555);


            ItemManager.Instance.GetItem(131401, 555555);
            ItemManager.Instance.GetItem(132401, 555555);
            ItemManager.Instance.GetItem(133401, 555555);
            ItemManager.Instance.GetItem(134401, 555555);
            ItemManager.Instance.GetItem(135401, 555555);


            ItemManager.Instance.GetItem(131501, 555555);
            ItemManager.Instance.GetItem(132501, 555555);
            ItemManager.Instance.GetItem(133501, 555555);
            ItemManager.Instance.GetItem(134501, 555555);
            ItemManager.Instance.GetItem(135501, 555555);

            ItemManager.Instance.GetItem(131601, 555555);
            ItemManager.Instance.GetItem(132601, 555555);
            ItemManager.Instance.GetItem(133601, 555555);
            ItemManager.Instance.GetItem(134601, 555555);
            ItemManager.Instance.GetItem(135601, 555555);

            ItemManager.Instance.GetItem(161001, 10000);
            ItemManager.Instance.GetItem(162001, 10000);
            ItemManager.Instance.GetItem(163001, 10000);
            ItemManager.Instance.GetItem(164001, 10000);
            ItemManager.Instance.GetItem(165001, 10000);


            ItemManager.Instance.GetItem(171001, 1);
            ItemManager.Instance.GetItem(172001, 1);
            ItemManager.Instance.GetItem(173001, 1);
            ItemManager.Instance.GetItem(174001, 1);
            ItemManager.Instance.GetItem(175001, 1);

            RoleManager.Instance.AddProperty(PropertyIdType.Tili, 1000);

        });

        btn_studentExpFull.onClick.AddListener(() =>
        {
            //for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Count; i++)
            //{
            //    StudentManager.Instance.OnGetStudentExp(RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList[i], 140);
            //}
            //for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.EquipMakeStudentList.Count; i++)
            //{
            //    StudentManager.Instance.OnGetStudentExp(RoleManager.Instance._CurGameInfo.StudentData.EquipMakeStudentList[i], 140);
            //}
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
            {
                PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
                StudentManager.Instance.OnGetStudentExp(p, 14000);
                if (p.talent == (int)StudentTalent.LianGong)
                {
                    RoleManager.Instance._CurGameInfo.studentData.allStudentList[i].curXiuwei += 9999999;

                }
            }
        });

        btn_jumpToMonth.onClick.AddListener(() =>
        {
            int target = input_jumpToMonth.text.ToInt32();
            if (target > RoleManager.Instance._CurGameInfo.timeData.Month)
            {
                GameTimeManager.Instance._CurTimeData.Week = 1;
                GameTimeManager.Instance._CurTimeData.TheWeekDay = 1;
                GameTimeManager.Instance._CurTimeData.Day = 1;

                GameTimeManager.Instance._CurTimeData.Month = target;
                GameTimeManager.Instance.DayStartEvent();
            }
        });

        btn_save.onClick.AddListener(() =>
        {
            ArchiveManager.Instance.SaveArchive();
            PanelManager.Instance.OpenOnlyOkHint("保存成功！", null);

        });

        btn_leavBigMap.onClick.AddListener(() =>
        {
            if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType ==(int) SceneType.SingleMap)
            {
                MapManager.Instance.LeaveMap();
            }
        });

        btn_loseItem.onClick.AddListener(() =>
        {
            int id = input_loseItemId.text.ToInt32();
            int count = input_loseItemCount.text.ToInt32();

            ItemSetting setting = DataTable.FindItemSetting(id);

            if (setting == null)
            {
                Debug.LogError("没有id为" + id + "的物品");
            }
            else
            {
                ItemManager.Instance.LoseItem(id, (ulong)count);

            }
        });

        btn_addXiuWei.onClick.AddListener(() =>
        {
            RoleManager.Instance._CurGameInfo.playerPeople.curXiuwei += 9999999;

        });

        btn_unlockAllFarm.onClick.AddListener(() =>
        {
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
            {
                SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                data.Unlocked = true;

                EventCenter.Broadcast(TheEventType.UnlockDanFarm, data);
            }

        });

        btn_unlockBuild.onClick.AddListener(() =>
        {
            int buildId = input_buildId.text.ToInt32();
            LianDanManager.Instance.UnlockDanFarm(buildId);
        });

        hide.onValueChanged.AddListener((x) =>
        {
            content.gameObject.SetActive(x);
        });

        btn_testCharge.onClick.AddListener(() =>
        {
            AddQQManager.Instance.CallAndroidMethod("OnCharge", RoleManager.Instance._CurGameInfo.TheGuid,
             RoleManager.Instance._CurGameInfo.NickName, 1, "50001", "60天晶测试", RoleManager.Instance._CurGameInfo.quIndex.ToString());
        });

        btn_haoGan.onClick.AddListener(() =>
        {
            // 遍历所有学生
            List<PeopleData> allStudents = RoleManager.Instance._CurGameInfo.studentData.allStudentList;
            for (int i = 0; i < allStudents.Count; i++)
            {
                PeopleData p1 = allStudents[i];
                for (int j = 0; j < allStudents.Count; j++)
                {
                    if (i == j) continue; // 跳过自己

                    PeopleData p2 = allStudents[j];

                    // 检查是否已经认识
                    int index = p1.socializationData.knowPeopleList.IndexOf(p2.onlyId);
                    if (index == -1)
                    {
                        // 不认识就让他们认识，并设置好感度为满值
                        p1.socializationData.knowPeopleList.Add(p2.onlyId);
                        p1.socializationData.haoGanDu.Add(ConstantVal.fullHaoGanDu);
                    }
                    else
                    {
                        // 已经认识，直接设置好感度为满值
                        p1.socializationData.haoGanDu[index] = ConstantVal.fullHaoGanDu;
                    }
                }
            }

            PanelManager.Instance.OpenOnlyOkHint("所有弟子好感度已全满！", null);
        });

        btn_uploadArchive.onClick.AddListener(() =>
        {
            // 先保存当前存档，再上传
            ArchiveManager.Instance.SaveArchive();
            PanelManager.Instance.OpenFloatWindow("正在上传，请勿操作");
            UPLoadFTP.UpLoadFiles.OnUploadArchive(
                ConstantVal.GetArchiveSavePath(ArchiveManager.Instance.curArchiveIndex),
                ArchiveManager.Instance.ArchiveUploadName());
        });

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (battleWin)
        {
            if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.Battle)
            {
                if (!BattleManager.Instance.logicPause)
                {
                    PeopleData p1 = BattleManager.Instance.p1List[BattleManager.Instance.p1Index];
                    PeopleData p2 = BattleManager.Instance.p2List[BattleManager.Instance.p2Index];
                    BattleManager.Instance.AddLogicPause();

                    BattleManager.Instance.someOndeDead = true;
                    //EventCenter.Broadcast(TheEventType.BattlePeopleDead, p2.OnlyId);

                    BattleManager.Instance.OnSingleBattleEnd(p1, p2,p1.onlyId);
                }
               
            }


        }
      
    }

    
}
