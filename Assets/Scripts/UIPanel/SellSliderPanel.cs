using cfg;
using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellSliderPanel : PanelBase
{
    public ItemData itemData;

    #region slider
    public int chooseNum;//选择数量
    public Text txt_choosedNum;//已选数量
    public Text txt_minNum;//最小
    public Text txt_maxNum;//最大
    public Slider slider;//滑动条
    public ItemSetting itemSetting;
    public Transform trans_matGrid;
    public List<WithCountItemView> matViewList = new List<WithCountItemView>();

    public Button btn_confirm;//确定
    #endregion


    public override void Init(params object[] args)
    {
        base.Init(args);
        itemData = args[0] as ItemData;
        itemSetting = DataTable.FindItemSetting(itemData.settingId);
        addBtnListener(btn_confirm, () =>
             {
                 if (chooseNum <= 0)
                 {
                     PanelManager.Instance.OpenFloatWindow("未选择数量");
                     return;
                 }
                 else
                 {
                     ItemManager.Instance.SellItem(itemData, (ulong)chooseNum);
                     PanelManager.Instance.ClosePanel(this);
                 }

             });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_choosedNum.SetText("0");


        ClearCertainParentAllSingle<WithCountItemView>(trans_matGrid);
        matViewList.Clear();
        ItemData item = new ItemData();
        item.settingId = (int)ItemIdType.LingShi;
        item.count = 0;
        WithCountItemView view = AddSingle<WithCountItemView>(trans_matGrid, item);
        matViewList.Add(view);


        int maxCount = (int)(ulong)itemData.count;
        int minCount = 0;
        txt_minNum.SetText(minCount.ToString());
        txt_maxNum.SetText(maxCount.ToString());

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
            OnChoosedNum(num);
        });
    }


    /// <summary>
    /// 选择了数量
    /// </summary>
    public void OnChoosedNum(int num)
    {
        matViewList[0].txt_count.SetText((chooseNum * itemSetting.Price.ToInt32()).ToString());

        chooseNum = num;
        txt_choosedNum.SetText(chooseNum.ToString());
    }
}
