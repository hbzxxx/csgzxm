using cfg;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class BattleSkillView : SingleViewBase
{
    public Image bg_img;
    public Image icon;
    public Image bg_kuang;
    public PeopleData peopleData;
    public SkillSetting setting;
    public SkillViewType skillViewType;
    public Sprite bgkp;
    public Sprite bgknp;

    public Text txt_dengji;
    public bool isScale;

    public override void Init(params object[] args)
    {
        base.Init(args);
        
        if (args.Length >= 2)
        {
            peopleData = args[0] as PeopleData;
            setting = args[1] as SkillSetting;
            
            if (args.Length >= 3)
            {
                skillViewType = (SkillViewType)args[2];
            }
            else
            {
                skillViewType = SkillViewType.Show;
            }
            isScale = (bool)args[3];
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        
        if (setting != null)
        {
            // 设置技能图标
            if (icon != null)
            {
                icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.skillFolderPath + setting.IconName);
            }
            
            // 设置技能等级
            if (txt_dengji != null && peopleData != null && peopleData.allSkillData != null)
            {
                // 通过技能ID查找装备的技能数据
                SingleSkillData skillData = null;
                int settingSkillId = int.Parse(setting.Id);
                for (int i = 0; i < peopleData.allSkillData.equippedSkillIdList.Count; i++)
                {
                    SingleSkillData tempSkill = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(
                        peopleData.allSkillData.equippedSkillIdList[i], peopleData.allSkillData);
                    if (tempSkill != null && tempSkill.skillId == settingSkillId)
                    {
                        skillData = tempSkill;
                        break;
                    }
                }
                
                if (skillData != null)
                {
                    txt_dengji.SetText("Lv." + skillData.skillLevel);
                }
                else
                {
                    txt_dengji.SetText("");
                }
            }
            
            // 设置缩放：玩家角色缩放为0.4，其他队友缩放为1
            SetScale();
        }
    }
    
    /// <summary>
    /// 根据角色类型设置缩放
    /// </summary>
    void SetScale()
    {
        if (peopleData == null) return;
        // 判断是否是玩家角色
        float scale = isScale ? 0.4f : 1f;
        Sprite sprite = isScale ? bgknp : bgkp;
        
        // 设置 bg_kuang 的缩放
        if (bg_kuang != null)
        {
            bg_kuang.transform.localScale = new Vector3(scale, scale, scale);
            bg_kuang.sprite = sprite;
        }
        if (bg_kuang != null) bg_kuang.SetNativeSize();
        // 同时设置 bg_img 的缩放以保持一致
        //if (bg_img != null)
        //{
        //    bg_img.transform.localScale = new Vector3(scale, scale, scale);
        //}
    }

    public override void Clear()
    {
        base.Clear();
        peopleData = null;
        setting = null;
    }
}