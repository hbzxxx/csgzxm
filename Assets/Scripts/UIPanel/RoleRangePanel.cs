  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPLoadFTP;

public class RoleRangePanel : PanelBase
{
     public Transform grid;
     public List<Button> pageBtns;
    public Transform trans_myGrid;
    public Text txt_des;

    //public Image 
    public List<Transform> grid_tops;
    public override void Init(params object[] args)
    {
        base.Init(args);

        for(int i = 0; i < pageBtns.Count; i++)
        {
            int index = i;
            Button btn = pageBtns[i];
      
        }

     }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RangeManager.Instance.UploadRange();
        pageBtns[0].onClick.Invoke();
    }

 
 
    public override void Clear()
    {
        base.Clear();
        for(int i=0;i< grid_tops.Count; i++)
        {
            ClearCertainParentAllSingle<SingleViewBase>(grid_tops[i]);
        }
    }
}
