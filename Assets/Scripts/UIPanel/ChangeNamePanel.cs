using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ChangeNamePanel : PanelBase
{
    public Text txt;
    public Button confirm;
    public ChangeNameType changeNameType;
    public InputField input;
    public int maxNameLength = 18;
    public override void Init(params object[] args)
    {
        base.Init(args);
        changeNameType = (ChangeNameType)args[0];
        input.text = "";
        switch (changeNameType)
        {
            case ChangeNameType.ZhangMenName:
                txt.SetText("修改你的名字");
                input.onValidateInput = null;
                input.onValidateInput += _OnValidateInput;
                addBtnListener(confirm, () =>
                {
                    //if (DataTable.IsScreening(input.text))
                    //{
                    //    PanelManager.Instance.OpenFloatWindow("名字包含敏感字\n请重新输入");
                    //    return;
                    //}
                    //RoleManager.Instance._CurGameInfo.playerPeople.name = input.text;
                    //PanelManager.Instance.OpenFloatWindow("修改成功");
                    //EventCenter.Broadcast(TheEventType.ChangeZhangMenName);
                    RoleManager.Instance.ChangeName(RoleManager.Instance._CurGameInfo.playerPeople, input.text);
                    PanelManager.Instance.ClosePanel(this);
                });
                break;
            case ChangeNameType.ZongMenName:
                txt.SetText("修改宗门名字");
                input.onValidateInput = null;
                input.onValidateInput += _OnValidateZongMenInput;
                addBtnListener(confirm, () =>
                {
                    ZongMenManager.Instance.ChangeName(input.text);
                    //if (DataTable.IsScreening(input.text))
                    //{
                    //    PanelManager.Instance.OpenFloatWindow("名字包含敏感字\n请重新输入");
                    //    return;
                    //}
                    //PanelManager.Instance.OpenFloatWindow("修改成功");
                    //RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName = input.text;
                    //EventCenter.Broadcast(TheEventType.ChangeZongMenName);
                    //ChatManager.Instance.OnChangeZongMenName();
                    PanelManager.Instance.ClosePanel(this);
                 });
                break;
            case ChangeNameType.StudentName:
                txt.SetText("修改角色名字");
                input.onValidateInput = null;
                input.onValidateInput += _OnValidateInput;
                PeopleData p = args[1] as PeopleData;

                addBtnListener(confirm, () =>
                {
                    RoleManager.Instance.ChangeName(p, input.text);

                    //if (DataTable.IsScreening(input.text))
                    //{
                    //    PanelManager.Instance.OpenFloatWindow("名字包含敏感字\n请重新输入");
                    //    return;
                    //}
                    //PanelManager.Instance.OpenFloatWindow("修改成功");
                    //p.name = input.text;
                    //EventCenter.Broadcast(TheEventType.ChangeStudentName);
                    //ChatManager.Instance.OnChangeStudentName(p);
                    PanelManager.Instance.ClosePanel(this);

                });
                break;
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
         
    }


    /// <summary>
    /// 屏蔽字TO DO
    /// </summary>
    /// <param name="text"></param>
    /// <param name="charIndex"></param>
    /// <param name="addedChar"></param>
    /// <returns></returns>
    char _OnValidateInput(string text, int charIndex, char addedChar)
    {
        if (System.Text.Encoding.UTF8.GetBytes(text + addedChar).Length > maxNameLength)
        {
            Debug.Log("字数过多");
            return '\0'; //返回空字符
        }

        return addedChar;
    }

    char _OnValidateZongMenInput(string text, int charIndex, char addedChar)
    {
        if (System.Text.Encoding.UTF8.GetBytes(text + addedChar).Length > maxNameLength)
        {
            Debug.Log("字数过多");
            return '\0'; //返回空字符
        }

        return addedChar;
    }

}

public enum ChangeNameType
{
    None=0,
    ZhangMenName,
    ZongMenName,
    StudentName,
}