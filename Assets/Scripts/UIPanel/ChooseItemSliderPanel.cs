
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseItemSliderPanel : PanelBase
{

    public int chooseNum;//选择数量
    public Text txt_choosedNum;//已选数量
    public Text txt_minNum;//最小
    public Text txt_maxNum;//最大
    public Slider slider;//滑动条

    //public ItemSetting itemSetting;
    public Transform trans_matGrid;
    public List<WithCountItemView> matViewList = new List<WithCountItemView>();

    public Button btn_confirm;//确定加入仓库
    public Action<int> callBack;
    public ItemData itemData;
    public override void Init(params object[] args)
    {
        base.Init(args);
        itemData = args[0] as ItemData;
        callBack = args[1] as Action<int>;
        addBtnListener(btn_confirm, () =>
        {
            callBack(chooseNum);
            PanelManager.Instance.ClosePanel(this);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        OnOpenChoosePanel(itemData);
    }



    #region slider
    /// <summary>
    /// 打开选择面板
    /// </summary>
    /// <param name="itemId"></param>
    public virtual void OnOpenChoosePanel(ItemData item)
    {


        ClearCertainParentAllSingle<WithCountItemView>(trans_matGrid);

        matViewList.Clear();
        matViewList.Add(AddSingle<WithCountItemView>(trans_matGrid, item));


        //可以做的最大数量
        int maxCount = (int)(ulong)item.count;
        int minCount = 1;
        txt_minNum.SetText(minCount.ToString());
        txt_maxNum.SetText(maxCount.ToString());
        //txt_choosedNum.SetText("0");
        //int divideNum=
        slider.value = 0;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((x) =>
        {
            //slider.normalizedValue
            int num = Mathf.RoundToInt(maxCount * x);
            if (maxCount > 0)
            {
                //这里要divide一下
                //int intNum=Mathf.RoundToInt()
                slider.value = num / (float)maxCount;
            }
            else
            {
                slider.value = 0;
            }
            if (num < minCount)
                num = minCount;
            OnChoosedNum(num);
        });
        slider.onValueChanged.Invoke(slider.value);
    }

    /// <summary>
    /// 选择了数量
    /// </summary>
    public virtual void OnChoosedNum(int num)
    {

        chooseNum = num;
        txt_choosedNum.SetText(chooseNum.ToString());
    }

    #endregion

}
