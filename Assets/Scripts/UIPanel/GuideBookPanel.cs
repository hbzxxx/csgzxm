using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Framework.Data;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

public class GuideBookPanel : PanelBase
{
    public GuideBookPanelShowType curShowType;
    public Button btn_dailiTaskTag;//每日tag
    public Image img_dailiTaskTag;
    public GameObject obj_dailiTaskRedPoint;//每日tag红点
    public Button btn_guideBookTaskTag;//手札tag
    public Image img_guideBookTaskTag;
    public GameObject obj_guideBookTaskRedPoint;//手札tag红点

    #region 每日任务页面
    public Transform trans_dailyTask;
    public Transform trans_canGetDailyAward;//活跃度达到xxx可领取
    public Text txt_canGetDailyAwardTxt;//活跃度达到xx可领取标题
    public Transform trans_canGetDailyAwardGrid;//活跃度达到xx可领取的具体奖励

    public Image img_activeBar;//活跃度
    public List<SkeletonGraphic> boxSkeList;//宝箱按钮
    public Transform trans_dailyTaskViewGrid;//每日任务格子
    public List<SingleDailyTaskView> singleDailyTaskViewList;//每日任务
    public List<Button> btn_baoxiangs;//宝箱按钮列表
    public List<Image> img_baoxiangs;//宝箱图片列表（用于切换Sprite）
    public List<Sprite> sprite_baoxiangopen;//宝箱打开图片
    public List<Sprite> sprite_baoxiangclose;//宝箱关闭图片
    public List<Image> ths;//领取奖励提醒图标
    private Dictionary<int, Tweener> thsTweenerDict = new Dictionary<int, Tweener>();//ths动画缓存
    #endregion

    #region 手札页面
    public Transform trans_guideBookPage;//手札

    public Text txt_chapterName;
    public Image curChapterProcess;
    public Text txt_curChapterProcess;
    public Text txt_curProsess;
    public ScrollAutoPoint  chapterProcessScroll;
    public List<ChapterProcessView> chapterProcessViewList = new List<ChapterProcessView>();
    public Transform trans_chapterProcessGrid;

    public ScrollViewNevigation taskScrollNevigation;//具体任务
    public Transform trans_taskGrid;//具体单个任务
    public List<SingleGuideBookView> singleGuideBookViewList = new List<SingleGuideBookView>();
    public Transform trans_chapterProcessAwardGrid;//当前选的章节进度奖励

    public Transform trans_chapterProcessAccomplished;//当前进度已完成
    public Transform trans_chapterProcessGetAwarded;//当前进度已领奖
    public Transform trans_chapterProcessUnAccomplished;//当前进度未领奖
    public Button btn_getCurProcessAward;

    public SingleChapterGuideBookData choosedChapter;//选择的章节
    public int curChapterProcessIndex;//当前是哪个进程

    public int minChapter = 0;
    public int maxChapter = 0;


    public int curChapter = 0;

    public Button btn_left;
    public Button btn_right;
    #endregion
    // 按钮文字颜色
    private Color selectedTextColor = new Color(0.408f, 0.310f, 0.212f, 1f); // #684F36
    private Color unselectedTextColor = new Color(0.757f, 0.741f, 0.702f, 1f); // #C1BDB3
    public override void Init(params object[] args)
    {
        base.Init(args);

        addBtnListener(btn_dailiTaskTag, () =>
         {
             curShowType = GuideBookPanelShowType.DailyTask;
             UpdateTagButtonTextColors(curShowType);
             ShowDailyTaskPage();
         });

        addBtnListener(btn_guideBookTaskTag, () =>
        {
            curShowType = GuideBookPanelShowType.GuideBook;
            UpdateTagButtonTextColors(curShowType);
            ShowGuideBookPage();
        });

        //for (int i = 0; i < boxSkeList.Count; i++)
        //{
        //    Button btn = boxSkeList[i].GetComponent<Button>();
        //    int index = i;
        //    addBtnListener(btn, () =>
        //    {
        //        if (RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[index] == (int)AccomplishStatus.Accomplished)
        //        {
        //            boxSkeList[index].AnimationState.SetAnimation(0, "baoxiang_dakai", false).Complete += delegate
        //            {
        //                TaskManager.Instance.OnGetDailyTaskProcessAward(index);
        //            };
        //        }
        //        else if (RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[index] == (int)AccomplishStatus.Processing)
        //        {
        //            ShowSingleDailyProcessAward(index);
        //        }
        //    });
        //}

        //for (int i = 0; i < ths.Count; i++)
        //{
        //    ths[i].gameObject.SetActive(false);
        //}
        for (int i = 0; i < btn_baoxiangs.Count; i++)
        {
            int index = i;
            addBtnListener(btn_baoxiangs[i], () =>
            {
                OnBaoxiangClick(index);
            });
        }
 

        RefreshChapter();

        addBtnListener(btn_left, () =>
         {
             if (curChapter <= 1)
                 return;
             else
             {
                 ShowChapter(curChapter - 1);
             }
         });
        addBtnListener(btn_right, () =>
        {

            if (curChapter >= maxChapter)
                return;

            if (curChapter == RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList.Count)
                return;

            if (!TaskManager.Instance.FindChapterGuideBookData(curChapter + 1).reveal)
                return;

                ShowChapter(curChapter + 1);
            
        });

        addBtnListener(btn_getCurProcessAward, () =>
         {
             TaskManager.Instance.OnGetGuideBookChapterProcessAward(TaskManager.Instance.FindChapterGuideBookData(curChapter), curChapterProcessIndex);
         });



        RegisterEvent(TheEventType.OnGetDailyTaskAward, OnGetDailyTaskAward);

        RegisterEvent(TheEventType.OnGetDailyTaskProcessAward, OnGetDailyTaskProcessAward);


        RegisterEvent(TheEventType.OnGetGuideBookAward, OnGetAward);
    }
    /// <summary>
    /// 更新标签按钮文字颜色
    /// </summary>
    void UpdateTagButtonTextColors(GuideBookPanelShowType selectedTag)
    {
        btn_dailiTaskTag.GetComponentInChildren<Text>().color = (selectedTag == GuideBookPanelShowType.DailyTask) ? selectedTextColor : unselectedTextColor;
        btn_guideBookTaskTag.GetComponentInChildren<Text>().color = (selectedTag == GuideBookPanelShowType.GuideBook) ? selectedTextColor : unselectedTextColor;
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        //选阶段
        RefreshShow();
        btn_dailiTaskTag.onClick.Invoke();
        RefreshRedPointShow();
        img_dailiTaskTag.gameObject.SetActive(true);
        img_guideBookTaskTag.gameObject.SetActive(false);
        //ShowDailyTaskPage();
    }

    /// <summary>
    /// 刷新红点
    /// </summary>
    void RefreshRedPointShow()
    {
        RedPointManager.Instance.SetRedPointUI(obj_dailiTaskRedPoint, RedPointType.MainPanel_Btn_Task_DailyTask, 0);
        RedPointManager.Instance.SetRedPointUI(obj_guideBookTaskRedPoint, RedPointType.MainPanel_Btn_Task_GuideBookTask, 0);

    }

    #region 每日任务页面
    /// <summary>
    /// 每日任务领奖
    /// </summary>
    void OnGetDailyTaskAward()
    {
        if (curShowType == GuideBookPanelShowType.DailyTask)
            ShowDailyTaskPage();

        RefreshRedPointShow();
    }

    /// <summary>
    /// 每日任务进程
    /// </summary>
    void OnGetDailyTaskProcessAward()
    {
        if(curShowType==GuideBookPanelShowType.DailyTask)
            ShowDailyTaskPage();
        RefreshRedPointShow();

    }

    public void ShowDailyTaskPage()
    {
        trans_dailyTask.gameObject.SetActive(true);
        trans_guideBookPage.gameObject.SetActive(false);
        img_dailiTaskTag.gameObject.SetActive(true);
        img_guideBookTaskTag.gameObject.SetActive(false);

        img_activeBar.fillAmount = RoleManager.Instance._CurGameInfo.AllDailyTaskData.curActiveNum / (float)100;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList.Count; i++)
        {
            // SkeletonGraphic ske = boxSkeList[i];
            // AccomplishStatus status =(AccomplishStatus)RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[i];
            // if (status == AccomplishStatus.Processing)
            // {
            //     ske.AnimationState.SetAnimation(0, "baoxiang_1", true);
            // }else if (status == AccomplishStatus.Accomplished)
            // {
            //     ske.AnimationState.SetAnimation(0, "baoxiang_huangdong", true);

            // }else if (status == AccomplishStatus.GetAward)
            // {
            //     ske.AnimationState.SetAnimation(0, "baoxiang_2", true);

            // }
            if (i >= img_baoxiangs.Count) break;
            AccomplishStatus status = (AccomplishStatus)RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[i];
            // 根据状态设置宝箱图片：关闭=未完成/进行中，打开=已领奖
            if (status == AccomplishStatus.Processing)
            {
                img_baoxiangs[i].sprite = sprite_baoxiangclose[i];
                StopThsAnim(i);
            }
            else if (status == AccomplishStatus.Accomplished)
            {
                img_baoxiangs[i].sprite = sprite_baoxiangclose[i];
                PlayThsShakeAnim(i);
            }
            else if (status == AccomplishStatus.GetAward)
            {
                img_baoxiangs[i].sprite = sprite_baoxiangopen[i];
                StopThsAnim(i);
            }
        }
        //显示下一个奖励
        int nextAwardIndex = RoleManager.Instance._CurGameInfo.AllDailyTaskData.curActiveNum / 20;
        ShowSingleDailyProcessAward(nextAwardIndex);
        //显示所有任务
        ClearCertainParentAllSingle<SingleDailyTaskView>(trans_dailyTaskViewGrid);
        List<SingleDailyTaskData> showList = new List<SingleDailyTaskData>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList.Count; i++)
        {
            SingleDailyTaskData data = RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList[i];
            showList.Add(data);
            //AddSingle<SingleDailyTaskView>(trans_dailyTaskViewGrid);
        }

        //排序
        //排序 先排进行中 再排已完成
        for (int i = 0; i < showList.Count - 1; i++)
        {
            for (int j = 0; j < showList.Count - 1 - i; j++)
            {
                //后面的进行中 则二者交换
                if (showList[j + 1].accomplishStatus == (int)AccomplishStatus.Processing)
                {
                    var temp = showList[j];
                    showList[j] = showList[j + 1];
                    showList[j + 1] = temp;
  
                }
            }
        }
        for (int i = 0; i < showList.Count - 1; i++)
        {
            for (int j = 0; j < showList.Count - 1 - i; j++)
            {
                //后面的已完成 则二者交换
                if (showList[j + 1].accomplishStatus == (int)AccomplishStatus.Accomplished)
                {
                    var temp = showList[j];
                    showList[j] = showList[j + 1];
                    showList[j + 1] = temp;
  
                }
            }
        }
        for(int i = 0; i < showList.Count; i++)
        {
            AddSingle<SingleDailyTaskView>(trans_dailyTaskViewGrid,showList[i]);
        }
    }
    /// <summary>
    /// 显示某个日常进程奖励
    /// </summary>
    void ShowSingleDailyProcessAward(int index)
    {
        List<List<List<int>>> processAward = CommonUtil.SplitThreeCfg(ConstantVal.dailyTaskProcessAward);

        if (index < processAward.Count)
        {
            trans_canGetDailyAward.gameObject.SetActive(true);
            ClearCertainParentAllSingle<WithCountItemView>(trans_canGetDailyAwardGrid);
            List<List<int>> showAward = processAward[index];
            for (int i = 0; i < showAward.Count; i++)
            {
                List<int> single = showAward[i];
                ItemData item = new ItemData();
                item.settingId = single[0];
                item.count = (ulong)single[1];
                AddSingle<WithCountItemView>(trans_canGetDailyAwardGrid, item);
            }
            txt_canGetDailyAwardTxt.SetText("活跃度达到" + (index + 1) * 20 + "可领取");
        }
        else
        {
            trans_canGetDailyAward.gameObject.SetActive(false);
        }

    }
    
    /// <summary>
    /// 宝箱点击处理
    /// </summary>
    void OnBaoxiangClick(int index)
    {
        AccomplishStatus status = (AccomplishStatus)RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[index];
        if (status == AccomplishStatus.Accomplished)
        {
            // 可领取状态：播放DOTween开箱动画后领奖
            PlayBaoxiangOpenAnim(index, () =>
            {
                TaskManager.Instance.OnGetDailyTaskProcessAward(index);
            });
        }
        else if (status == AccomplishStatus.Processing)
        {
            // 进行中状态：显示奖励预览
            ShowSingleDailyProcessAward(index);
        }
    }

    /// <summary>
    /// 播放宝箱打开动画
    /// </summary>
    void PlayBaoxiangOpenAnim(int index, System.Action onComplete)
    {
        if (index >= img_baoxiangs.Count) return;
        
        // 停止并隐藏 ths 动画
        StopThsAnim(index);
        
        Image img = img_baoxiangs[index];
        Transform trans = img.transform;
        
        // 晃动 + 缩放动画
        Sequence seq = DOTween.Sequence();
        seq.Append(trans.DOShakeRotation(0.3f, new Vector3(0, 0, 15), 10, 90));
        seq.Append(trans.DOScale(1.2f, 0.1f));
        seq.Append(trans.DOScale(1f, 0.1f));
        seq.AppendCallback(() =>
        {
            // 切换为打开状态图片
            img.sprite = sprite_baoxiangopen[index];
        });
        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 播放 ths 领取提醒图标左右晃动动画（循环）
    /// </summary>
    void PlayThsShakeAnim(int index)
    {
        if (index >= ths.Count) return;
        
        // 如果已有动画在播放，不重复创建
        if (thsTweenerDict.ContainsKey(index) && thsTweenerDict[index] != null && thsTweenerDict[index].IsActive())
            return;
        
        Image thImg = ths[index];
        thImg.gameObject.SetActive(true);
        thImg.transform.localRotation = Quaternion.identity;
        
        // 左右晃动动画，循环播放
        Tweener tweener = thImg.transform.DORotate(new Vector3(0, 0, 15), 0.15f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
        thsTweenerDict[index] = tweener;
    }

    /// <summary>
    /// 停止 ths 动画并隐藏
    /// </summary>
    void StopThsAnim(int index)
    {
        if (index >= ths.Count) return;
        
        // 杀死动画
        if (thsTweenerDict.ContainsKey(index) && thsTweenerDict[index] != null)
        {
            thsTweenerDict[index].Kill();
            thsTweenerDict.Remove(index);
        }
        
        // 隐藏并重置旋转
        ths[index].gameObject.SetActive(false);
        ths[index].transform.localRotation = Quaternion.identity;
    }

    #endregion

    /// <summary>
    /// 显示手札页面
    /// </summary>
    public void ShowGuideBookPage()
    {
        trans_dailyTask.gameObject.SetActive(false);
        trans_guideBookPage.gameObject.SetActive(true);
        img_dailiTaskTag.gameObject.SetActive(false);
        img_guideBookTaskTag.gameObject.SetActive(true);

        RefreshChapter();

        ShowChapter(minChapter);
    }

    void RefreshShow()
    {
        RefreshChapter();

        ShowChapter(minChapter);
    }
    void OnGetAward()
    {
        RefreshChapter();

        ShowChapter(curChapter);
        RefreshRedPointShow();

    }

    void RefreshChapter()
    {
        minChapter = int.MaxValue;
        maxChapter = int.MinValue;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Count; i++)
        {
            SingleGuideBookTaskData data = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList[i];
            GuideBookSetting setting = DataTable.FindGuideBookSetting(data.settingId);
            int chapter = setting.Chapter.ToInt32();
            if (chapter < minChapter
                && data.accomplishStatus != (int)AccomplishStatus.GetAward)
            {
                minChapter = chapter;
            }
            if (chapter > maxChapter)
            {
                maxChapter = chapter;
            }
        }
        if (minChapter > maxChapter)
            minChapter = maxChapter;
    }

    void ShowChapter(int chapter)
    {
        choosedChapter = TaskManager.Instance.FindChapterGuideBookData(chapter);
        curChapter = chapter;
        string zh_chapter = NumberToChinese(chapter);
        txt_chapterName.SetText("卷" + zh_chapter);

        List<SingleGuideBookTaskData> chapterGuideBookList = TaskManager.Instance.FindChapterGuideBookList(chapter);
        int total = chapterGuideBookList.Count;

        int awardedNum = 0;
        for (int i = 0; i < chapterGuideBookList.Count; i++)
        {
            SingleGuideBookTaskData data = chapterGuideBookList[i];
            if(data.accomplishStatus == (int)AccomplishStatus.GetAward)
            {
                awardedNum++;
            }
        }
        float rate = awardedNum / (float)total;
        curChapterProcess.fillAmount = rate;
        txt_curChapterProcess.SetText((rate * 100).ToString("0") + "%");
        //显示当前任务完成进度
        txt_curProsess.SetText("完成度达到" + (rate * 100).ToString("0") + "%");
        ClearCertainParentAllSingle<ChapterProcessView>(trans_chapterProcessGrid);
        chapterProcessViewList.Clear();
        int curIndex = 0;
        SingleChapterGuideBookData chapterGuideData = TaskManager.Instance.FindSingleChapterBookDataByTaskId(chapterGuideBookList[0].settingId);

        for(int i = 0; i < chapterGuideData.processAccomplishStatus.Count; i++)
        {
            AccomplishStatus status = (AccomplishStatus)chapterGuideData.processAccomplishStatus[i];
            if (status==AccomplishStatus.GetAward)
                curIndex = i+1;
        }
        if (curIndex >= 9)
            curIndex = 9;

        //定位到已领过的
        for (int i = 0; i < 10; i++)
        {
            GuideBookSetting setting = DataTable.FindGuideBookSetting(chapterGuideBookList[0].settingId);
            chapterProcessViewList.Add(AddSingle<ChapterProcessView>(trans_chapterProcessGrid, chapterGuideData, i,this));
            //设置进度
            //Debug.Log(i);
            //if (rate * 10 >= i + 1
            //    && rate * i <= i + 2)
            //{
            //    curIndex = i;
            //}
        }
        chapterProcessScroll.Init();

        chapterProcessScroll.OnLocate(chapterProcessViewList[curIndex].GetComponent<RectTransform>());
        chapterProcessViewList[curIndex].btn.onClick.Invoke();

        ClearCertainParentAllSingle<SingleGuideBookView>(trans_taskGrid);
        singleGuideBookViewList.Clear();
        List<SingleGuideBookTaskData> showList = new List<SingleGuideBookTaskData>();
        List<GuideBookSetting> showSetting = new List<GuideBookSetting>();
        //显示所有任务
        for (int i = 0; i < chapterGuideBookList.Count; i++)
        {
            SingleGuideBookTaskData data = chapterGuideBookList[i];
            GuideBookSetting setting = DataTable.FindGuideBookSetting(data.settingId);
            if (setting.Chapter.ToInt32() != curChapter)
                continue;
            //如果已完成 观察有没有后置任务，如果没有后置任务 则显示 如果有 则跳过
            if (data.accomplishStatus == (int)AccomplishStatus.GetAward)
            {
                if (!string.IsNullOrWhiteSpace(setting.NextTask))
                {
                    continue;
                }
            }
            else if (data.accomplishStatus == (int)AccomplishStatus.Locked)
            {
                continue;
            }
            showList.Add(data);
            showSetting.Add(setting);
    
        }
        //排序 先排进行中 再排已完成
        for (int i = 0; i < showList.Count - 1; i++)
        {
            for (int j = 0; j < showList.Count - 1 - i; j++)
            {
                //后面的进行中 则二者交换
                if (showList[j + 1].accomplishStatus == (int)AccomplishStatus.Processing)
                {
                    var temp = showList[j];
                    showList[j] = showList[j + 1];
                    showList[j + 1] = temp;

                    var temp2 = showSetting[j];
                    showSetting[j] = showSetting[j + 1];
                    showSetting[j + 1] = temp2;
                }
            }
        }
        for (int i = 0; i < showList.Count - 1; i++)
        {
            for (int j = 0; j < showList.Count - 1 - i; j++)
            {
                //后面的已完成 则二者交换
                if (showList[j + 1].accomplishStatus == (int)AccomplishStatus.Accomplished)
                {
                    var temp = showList[j];
                    showList[j] = showList[j + 1];
                    showList[j + 1] = temp;

                    var temp2 = showSetting[j];
                    showSetting[j] = showSetting[j + 1];
                    showSetting[j + 1] = temp2;
                }
            }
        }
        for(int i = 0; i < showList.Count; i++)
        {
            SingleGuideBookTaskData data = showList[i];
            GuideBookSetting setting = showSetting[i];
            //如果锁定 跳过
            singleGuideBookViewList.Add(AddSingle<SingleGuideBookView>(trans_taskGrid, data, setting));
        }

        //定位到第一个
        if(showList.Count>0)
        PanelManager.Instance.LocateScrollAndTaskPoint(taskScrollNevigation, singleGuideBookViewList[0].gameObject, false);
    }

    /// <summary>
    /// 选择了进度
    /// </summary>
    public void OnChoosedProcess(int index)
    {
        curChapterProcessIndex = index;
        ClearCertainParentAllSingle<WithCountItemView>(trans_chapterProcessAwardGrid);
        GuideBookSetting setting= DataTable.FindAnyGuideBookSettingByChapter(choosedChapter.chapter);
        if (!string.IsNullOrWhiteSpace(setting.ChapterAward))
        {
           List<List<List<int>>> award = CommonUtil.SplitThreeCfg(setting.ChapterAward);
            for (int i = 0; i < award.Count; i++)
            {
                if (i == index)
                {
                    List<List<int>> singleProcessAward = award[i];
                    for(int j = 0; j < singleProcessAward.Count; j++)
                    {
                        List<int> single = singleProcessAward[j];
                        if (single.Count == 2)
                        {
                            ItemData item = new ItemData();
                            item.settingId = single[0];
                            item.count = (ulong)single[1];
                            if (item.count <= 0)
                                continue;
                            AddSingle<WithCountItemView>(trans_chapterProcessAwardGrid, item);
                        }
                    }
                  
                }

            }
        }

        SingleChapterGuideBookData singleChapterGuideBookData = TaskManager.Instance.FindChapterGuideBookData(curChapter);
        if (singleChapterGuideBookData.processAccomplishStatus[index] ==(int)AccomplishStatus.Accomplished)
        {
            trans_chapterProcessAccomplished.gameObject.SetActive(true);
            trans_chapterProcessGetAwarded.gameObject.SetActive(false);
        }else if(singleChapterGuideBookData.processAccomplishStatus[index] == (int)AccomplishStatus.UnAccomplished)
        {
            trans_chapterProcessAccomplished.gameObject.SetActive(false);
            trans_chapterProcessGetAwarded.gameObject.SetActive(false);
        }
        else if (singleChapterGuideBookData.processAccomplishStatus[index] == (int)AccomplishStatus.GetAward)
        {
            trans_chapterProcessAccomplished.gameObject.SetActive(false);
            trans_chapterProcessGetAwarded.gameObject.SetActive(true);
        }


    }

    public override void OnClose()
    {
        base.OnClose();
        chapterProcessScroll.OnClose();
    }

    /// <summary>
    /// 数字转中文大写
    /// </summary>
    string NumberToChinese(int number)
    {
        string[] chineseNumbers = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
        if (number >= 0 && number <= 10)
            return chineseNumbers[number];
        else if (number > 10 && number < 20)
            return "十" + chineseNumbers[number - 10];
        else if (number >= 20 && number < 100)
        {
            int tens = number / 10;
            int ones = number % 10;
            return chineseNumbers[tens] + "十" + (ones > 0 ? chineseNumbers[ones] : "");
        }
        return number.ToString();
    }

}
/// <summary>
/// 任务类型
/// </summary>
public enum GuideBookPanelShowType
{
    None=0,
    DailyTask,
    GuideBook,
}