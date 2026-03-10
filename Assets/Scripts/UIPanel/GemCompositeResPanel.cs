using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Google.Protobuf.Collections;
using UnityEngine.UI;

public class GemCompositeResPanel : PanelBase
{
    public Transform trans_grid_before;
    public Transform trans_grid_after;

    public List<SinglePropertyData> beforeProList;
    public List<SinglePropertyData> afterProList;
    List<int> beforeExpList = new List<int>();
    public List<PeopleData> pList;
    public Text txt_studentAdd;
    GemData GemData;
    public Transform trans_studentGrid;
    public StudentAddAnimGroup studentAddAnimGroup;
    public override void Init(params object[] args)
    {
        base.Init(args);
        beforeProList = args[0] as List<SinglePropertyData>;
        afterProList = args[1] as List<SinglePropertyData>;
        beforeExpList = args[2] as List<int>;
        pList = args[3] as List<PeopleData>;
        GemData = args[4] as GemData;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_studentAdd.gameObject.SetActive(false);
        for (int i = 0; i < beforeProList.Count; i++)
        {
            PanelManager.Instance.OpenSingle<GemPropertyView>(trans_grid_before,true, beforeProList[i],GemData);
            PanelManager.Instance.OpenSingle<GemPropertyView>(trans_grid_after,true, afterProList[i],GemData);
        }
        //int beforeEmptyCount = 3 - beforeProList.Count;
        //for(int i = 0; i < beforeEmptyCount; i++)
        //{
        //    PanelManager.Instance.OpenSingle<GemPropertyView>(trans_grid_before, false);
        //}

        //int afterEmptyCount = 3 - afterProList.Count;
        //for (int i = 0; i < afterEmptyCount; i++)
        //{
        //    PanelManager.Instance.OpenSingle<GemPropertyView>(trans_grid_after, false);
        //}

        SinglePropertyData pro = afterProList[afterProList.Count - 1];
        //最后一个做个动画
        GemPropertyView animView= PanelManager.Instance.OpenSingle<GemPropertyView>(trans_grid_after,true, pro,GemData);

        int theNumInit = Mathf.RoundToInt(pro.num / (1 + (pro.quality - 1) * 0.2f));
        SinglePropertyData initPro = new SinglePropertyData();
        initPro.id = pro.id;
        initPro.num = theNumInit;
        initPro.limit = pro.limit;
        animView.TweenVal(initPro,()=> 
        {
            txt_studentAdd.gameObject.SetActive(true);
            animView.TweenStudentVal(pro);
        });

        //ClearCertainParentAllSingle<SingleUpgradeStudentView>(trans_studentGrid);

        //for (int i = 0; i < pList.Count; i++)
        //{
        //    AddSingle<SingleUpgradeStudentView>(trans_studentGrid, beforeExpList[i], pList[i]);
        //}
        studentAddAnimGroup.Play(pro.quality);
        AuditionManager.Instance.PlayVoice(AudioClipType.MakeFinish);

    }
    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_grid_before);
        PanelManager.Instance.CloseAllSingle(trans_grid_after);

    }
    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.OpenPanel<StudentAddExpPanel>(PanelManager.Instance.trans_layer2, beforeExpList, pList);

    }
}
