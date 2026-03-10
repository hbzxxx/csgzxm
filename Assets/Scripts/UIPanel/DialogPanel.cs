using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class DialogPanel : PanelBase
{
    public Button btn_next;
    public Transform trans_leftPeopleStartPos;//左边人物起点
    public Transform trans_rightPeopleStartPos;//右边人物起点
    public Transform trans_imgLeftPeople;
    public Transform trans_imgRightPeople;
    //public Transform trans_txtLeftPeople;
    //public Transform trans_txtRightPeople;
    public Transform trans_verticalDraw;
    public Transform trans_portraitParent;//立绘父物体 旋转他
    public Text txt;
    public Text txt_name;
   // People people;
    bool left;

    List<DialogData> dialogList = new List<DialogData>();
    Action endCallBack;
    int curIndex = -1;
    PeopleData curSpeekPeople;

    public DialogType dialogType;

    public Transform trans_choose;//对话完后弹出选项
    //public Button btn1;//选项1
    //public Text txt_btn1;//选项1txt
    //public Button btn2;//选项2
    //public Text txt_btn2;//选项2txt

    //public Action btn1ChooseCallback;//选择了选项1
    //public Action btn2ChooseCallback;//选择了选项2

    public Transform trans_chooseBtnGrid;//选择按钮
   
   List<Func<bool>> success;
    public Portrait portrait;
    public Image img_specialPortrait;
    public string curContent;//当前内容
    public float txtSpeed;
    public int curTxtWordIndex;
    public float txtWordTimer;
    public bool startFlowTxt;
    public bool portraitMove;
    public override void Init(params object[] args)
    {
        base.Init(args);

        RegisterEvent(TheEventType.CloseCurDialog, () =>
        {
            PanelManager.Instance.ClosePanel(this);
        });
        dialogType = (DialogType)args[0];
        dialogList = args[1] as List<DialogData>;
        trans_choose.gameObject.SetActive(false);

        if (dialogType == DialogType.Common)
        {
            endCallBack = args[2] as Action;
        }
        else
        {
            List<string> btnStrList = args[2] as List<string>;

            List<Action> callBackList = args[3] as List<Action>;
            if (callBackList == null)
            {
                success = args[3] as List<Func<bool>>;
            }
          
            //if(theCB.Invoke())

            for (int i = 0; i < btnStrList.Count; i++)
            {
                if (callBackList != null)
                {
                    PanelManager.Instance.OpenSingle<ChooseBtnView>(trans_chooseBtnGrid, btnStrList[i], callBackList[i], this);

                    //callBackList[i] += delegate
                    //{
                    //    PanelManager.Instance.ClosePanel(this);
                    //};
                }
                else
                {
                    PanelManager.Instance.OpenSingle<ChooseBtnView>(trans_chooseBtnGrid, btnStrList[i], success[i], this);

                }



            }

       
        }


        addBtnListener(btn_next, () =>
        {
            if (startFlowTxt)
            {
                //流完
                txt.SetText(dialogList[curIndex].content);
                startFlowTxt = false;
                return;
            }
            if (curIndex < dialogList.Count-1)
            {
                curIndex++;
                //if (dialogList[curIndex].belong != curSpeekPeople)
                //{
                //    left = !left;
                //}
                if (dialogList[curIndex].belong != null &&
                 RoleManager.Instance.CheckIfMeOrStudent(dialogList[curIndex].belong.onlyId))
                {
              
                    left = true;
                }
                else
                {    
                
                    left = false;
                }
                if (PanelManager.Instance.dialogDirectionLeft != left)
                {
                    portraitMove = true;
                }
                else
                {
                    portraitMove = false;
                }
                PanelManager.Instance.dialogDirectionLeft = left;

                NextDialog();
            }
            //结束对话
            else
            {
                if (dialogType == DialogType.Common)
                {
                    PanelManager.Instance.ClosePanel(this);

                    if (endCallBack != null)
                    {
                        endCallBack();

                    }
                }
                else
                {
                    trans_choose.gameObject.SetActive(true);
                }

            }
        });
    }

    public override void Clear()
    {
        base.Clear();
        startFlowTxt = false;
         txt.SetText("");
        PanelManager.Instance.CloseAllSingle(trans_chooseBtnGrid);
    }

    public override void OnClose()
    {
        base.OnClose();
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        
        curIndex = 0;

        left = true;
        if (dialogList[curIndex].belong!=null&&
            RoleManager.Instance.CheckIfMeOrStudent(dialogList[curIndex].belong.onlyId))
        {
            left = true;
        }
        else
        {
            left = false;
        }
        PanelManager.Instance.dialogDirectionLeft = !left;
        if (PanelManager.Instance.dialogDirectionLeft != left)
        {
            portraitMove = true;
        }
        else
        {
            portraitMove = false;

        }

        PanelManager.Instance.dialogDirectionLeft = left;

        NextDialog();

    }


    void NextDialog()
    {
        curSpeekPeople= dialogList[curIndex].belong;

        if (curSpeekPeople != null)
        {
            //img_verticalDraw.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.verticalDrawFolderPath + curSpeekPeople.verticalDrawName);
            trans_verticalDraw.gameObject.SetActive(true);
            if (curSpeekPeople.specialPortrait)
            {
                portrait.gameObject.SetActive(false);
                img_specialPortrait.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + curSpeekPeople.specialPortraitName);
                img_specialPortrait.gameObject.SetActive(true);

            }
            else
            {
                portrait.gameObject.SetActive(true);
                img_specialPortrait.gameObject.SetActive(false);
                if (curSpeekPeople.portraitIndexList.Count == 0)
                {
                    RoleManager.Instance.RdmFace(curSpeekPeople);
                }
                portrait.Refresh(curSpeekPeople);

            }
            txt_name.SetText(curSpeekPeople.name);
            
        }
        else
        {
            trans_verticalDraw.gameObject.SetActive(false);
            //txt.transform.position = trans_txtRightPeople.position;

        }
        if (left)
        {
            trans_portraitParent.localEulerAngles = new Vector3(0, 180, 0);
            trans_verticalDraw.DOKill();
            if (portraitMove)
            {
                trans_verticalDraw.position = trans_leftPeopleStartPos.position;
                trans_verticalDraw.DOMoveX(trans_imgLeftPeople.transform.position.x, .5f);
            }
            else
            {
                trans_verticalDraw.position = trans_imgLeftPeople.transform.position;

            }

            //txt.transform.position = trans_txtLeftPeople.position;
        }
        else
        {
            trans_portraitParent.localEulerAngles = Vector3.zero;
            trans_verticalDraw.DOKill();

            if (portraitMove)
            {
                trans_verticalDraw.position = trans_rightPeopleStartPos.position;
                trans_verticalDraw.DOMoveX(trans_imgRightPeople.transform.position.x, .5f);
            }
            else
            {
                trans_verticalDraw.position = trans_imgRightPeople.transform.position;
            }


            //trans_verticalDraw.transform.position = trans_imgRightPeople.transform.position;
            //txt.transform.position = trans_txtRightPeople.position;
        }
        curContent = dialogList[curIndex].content;
        curTxtWordIndex = 0;
        txtWordTimer = 0;
        startFlowTxt = true;
    }

    private void Update()
    {
        if (startFlowTxt)
        {
            if (curTxtWordIndex > curContent.Length)
            {
                startFlowTxt = false;
                return;

            }
            txtWordTimer += Time.deltaTime;
            if (txtWordTimer >= txtSpeed)
            {
                txt.SetText(curContent.Substring(0, curTxtWordIndex));
                curTxtWordIndex++;
                txtWordTimer = 0;
            }
        }

    }

}

/// <summary>
/// 对话类型
/// </summary>
public enum DialogType
{
    None=0,
    Common,
    Choose,//有选项的对话
}
