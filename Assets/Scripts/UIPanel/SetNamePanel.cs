using Framework.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置名字和血脉选择面板
/// 合并了原 XueMaiChoosePanel 的血脉选择功能
/// </summary>
public class SetNamePanel : PanelBase
{
    public Portrait portrait;
    public int maxRoleNameLength = 12;

    public InputField input_name;
    public InputField input_zongMenName;

    public Button btn_rdmName;
    public Button btn_rdmZongMenName;

    public Button btn_confirm;

    public Toggle toggle_male;
    public Toggle toggle_female;
    public Gender gender = Gender.Male;

    public Image Gender_img;

    // 血脉选择相关
    public List<Button> xueMaiBtnList;
    public List<YuanSuType> typeList;
    YuanSuType curChoosedXueMai = YuanSuType.None;
 
    public override void Init(params object[] args)
    {
        base.Init(args);
        input_name.text = "";
        input_zongMenName.text = "";

        input_name.onValidateInput += _OnValidateInput;
        input_zongMenName.onValidateInput += _OnValidateZongMenInput;

        // 血脉按钮监听
        for (int i = 0; i < xueMaiBtnList.Count; i++)
        {
            int index = i;
            Button btn = xueMaiBtnList[i];
            addBtnListener(btn, () =>
            {
                OnChoosedSingleXueMai(index);
            });
        }

        addBtnListener(btn_confirm, OnConfirm);
        addBtnListener(btn_rdmName, RdmName);
        addBtnListener(btn_rdmZongMenName, RdmZongMenName);

        toggle_female.onValueChanged.RemoveAllListeners();
        toggle_female.onValueChanged.AddListener((x) =>
        {
            SetGender(x);
        });
        toggle_male.onValueChanged.RemoveAllListeners();
        toggle_male.onValueChanged.AddListener((x) =>
        {
            SetGender(!x);
        });

        toggle_male.onValueChanged.Invoke(true);
    }

    void OnConfirm()
    {
        // 检查是否选择了血脉
        if (curChoosedXueMai == YuanSuType.None)
        {
            PanelManager.Instance.OpenFloatWindow("请选择一个属性");
            return;
        }

        if (DataTable.IsScreening(input_name.text))
        {
            PanelManager.Instance.OpenFloatWindow("名字包含敏感字\n请重新输入");
            return;
        }
        if (DataTable.IsScreening(input_zongMenName.text))
        {
            PanelManager.Instance.OpenFloatWindow("名字包含敏感字\n请重新输入");
            return;
        }

        // 设置血脉
        RoleManager.Instance._CurGameInfo.playerPeople.yuanSu = (int)curChoosedXueMai;
        RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Add((int)curChoosedXueMai);
        RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[0].skillId = (int)BattleManager.Instance.PuGongIdByYuanSu((YuanSuType)RoleManager.Instance._CurGameInfo.playerPeople.yuanSu);
        RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.equippedSkillIdList[0] = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[0].skillId;

        // 设置名字
        RoleManager.Instance._CurGameInfo.playerPeople.name = input_name.text;
        ZongMenManager.Instance.SetInitZongMenName();
        RoleManager.Instance.profile.SetProfileName(input_name.text);

        // 随机一个脸
        RoleManager.Instance.RdmFace(RoleManager.Instance._CurGameInfo.playerPeople);

        // 关闭SetNamePanel，打开TopPanel和MainPanel
        PanelManager.Instance.ClosePanel(this);

        // 新存档设置完成后，打开TopPanel和MainPanel
        if (GameSceneManager.Instance.skipOpenTopMainPanel)
        {
            GameSceneManager.Instance.skipOpenTopMainPanel = false;
            PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);
            PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);
        }

        // 触发神秘少女出现
        TaskManager.Instance.CheckIfNPCAppear();
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RdmName();
        toggle_male.onValueChanged.Invoke(true);

 
    }

    void SetGender(bool isFemale)
    {
        int initGender = RoleManager.Instance._CurGameInfo.playerPeople.gender;
        bool genderChange = false;
        if (isFemale)
        {
            gender = Gender.Female;
            if (initGender != (int)gender)
                genderChange = true;
            Gender_img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.PeopleTouxiang + (int)StudentTalent.FemaleKing);
        }
        else
        {
            gender = Gender.Male;
            if (initGender != (int)gender)
                genderChange = true;
            Gender_img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.PeopleTouxiang + (int)StudentTalent.ManKing);
        }
        RoleManager.Instance._CurGameInfo.playerPeople.gender = (int)gender;
        if (genderChange)
        {
            RdmName();
        }
    }

    /// <summary>
    /// 选择了某个血脉
    /// </summary>
    void OnChoosedSingleXueMai(int index)
    {
        curChoosedXueMai = typeList[index];
   
    }

    public void RdmName()
    {
        input_name.text = RoleManager.Instance.rdmName((Gender)(int)RoleManager.Instance._CurGameInfo.playerPeople.gender);
    }

    public void RdmZongMenName()
    {
        input_zongMenName.text = ZongMenManager.Instance.RdmZongMenName();
    }

    char _OnValidateInput(string text, int charIndex, char addedChar)
    {
        if (System.Text.Encoding.UTF8.GetBytes(text + addedChar).Length > maxRoleNameLength)
        {
            Debug.Log("字数过多");
            return '\0';
        }
        return addedChar;
    }

    char _OnValidateZongMenInput(string text, int charIndex, char addedChar)
    {
        if (System.Text.Encoding.UTF8.GetBytes(text + addedChar).Length > maxRoleNameLength)
        {
            Debug.Log("字数过多");
            return '\0';
        }
        return addedChar;
    }

    public override void Clear()
    {
        base.Clear();
        curChoosedXueMai = YuanSuType.None;
    }
}
