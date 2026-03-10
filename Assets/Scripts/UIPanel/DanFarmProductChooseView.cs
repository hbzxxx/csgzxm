using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class DanFarmProductChooseView : SingleViewBase
{
    public Color color_choosed = new Color32(152, 206, 229, 255);
    public Image img_bg;
    public ItemSetting itemSetting;
    public Image img_icon;
    public Text txt_name;
    public Button btn;
    public bool locked;//锁定
    public string lockedTxt;//锁定字样
    public Button btn_locked;//锁定
    public Text txt_locked;//锁定
    public ChooseFarmProductMatPanel parentPanel;
    public Transform trans_matGrid;
    public override void Init(params object[] args)
    {
        base.Init(args);
        int itemId = (int)args[0];
        parentPanel = args[1] as ChooseFarmProductMatPanel;
        locked = (bool)args[2];
        if (locked)
        {
            lockedTxt = (string)args[3];
        }

        itemSetting = DataTable.FindItemSetting(itemId);

        addBtnListener(btn, () =>
        {
            parentPanel.OnChoosed(this);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (itemSetting == null) return;
        img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + itemSetting.UiName);
        txt_name.SetText(itemSetting.Name);
        if (locked)
        {
            btn_locked.gameObject.SetActive(true);
            txt_locked.SetText(lockedTxt);
        }
        else
        {
            btn_locked.gameObject.SetActive(false);

        }

        PanelManager.Instance.CloseAllSingle(trans_matGrid);
        List<List<int>> matList = CommonUtil.SplitCfg(itemSetting.Param2);

        for (int i = 0; i < matList.Count; i++)
        {
            List<int> singleMat = matList[i];
            int needNum = singleMat[1];
            SingleConsumeView view = PanelManager.Instance.OpenSingle<SingleConsumeView>(trans_matGrid, singleMat[0], needNum, ConsumeType.Item);
         }

    }

    public void OnChoose(bool choose)
    {
        if (choose)
        {
            img_bg.color = color_choosed;

        }
        else
        {
            img_bg.color = Color.white;
        }
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_matGrid);
    }


}
