using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuanSuReactionManager : CommonInstance<YuanSuReactionManager>
{
 
    /// <summary>
    /// 判断是否可以反应
    /// </summary>
    /// <returns></returns>
    public ReactionType JudgeIfCanReaction(List<YuanSuType> sourceList)
    {
        if (sourceList.Count == 2)
        {
            YuanSuType y1 = sourceList[0];
            YuanSuType y2 = sourceList[1];
            //水火 爆蒸
            if ((y1 == YuanSuType.Water && y2 == YuanSuType.Fire)
                || (y2 == YuanSuType.Water && y1 == YuanSuType.Fire))
            {
                return ReactionType.BaoZheng;
            }
            //水雷 电流
            else if((y1 == YuanSuType.Water && y2 == YuanSuType.Storm)
                || (y2 == YuanSuType.Water && y1 == YuanSuType.Storm))
            {
                return ReactionType.DianLiu;
            }
            //水冰 冻僵
            else if ((y1 == YuanSuType.Water && y2 == YuanSuType.Ice)
                || (y2 == YuanSuType.Water && y1 == YuanSuType.Ice))
            {
                return ReactionType.DongJiang;
            }
            //火雷 爆炸
            else if ((y1 == YuanSuType.Fire && y2 == YuanSuType.Storm)
                || (y2 == YuanSuType.Fire && y1 == YuanSuType.Storm))
            {
                return ReactionType.BaoZha;
            }
            //火冰 融化
            else if ((y1 == YuanSuType.Fire && y2 == YuanSuType.Ice)
                || (y2 == YuanSuType.Fire && y1 == YuanSuType.Ice))
            {
                return ReactionType.RongHua;
            }
            //冰雷 雷枪
            else if ((y1 == YuanSuType.Ice && y2 == YuanSuType.Storm)
                || (y2 == YuanSuType.Ice && y1 == YuanSuType.Storm))
            {
                return ReactionType.LeiQiang;
            }
        }
        else if (sourceList.Count == 3)
        {
     
            if(sourceList[0]==sourceList[1]
                && sourceList[1] == sourceList[2])
            {     
                //3水潮湿
                if (sourceList[0] == YuanSuType.Water)
                {
                    return ReactionType.ChaoShi;
                }
                //3火灼烧
                else if (sourceList[0] == YuanSuType.Fire)
                {
                    return ReactionType.ZhuoShao;
                }
                //3冰冰牢
                else if (sourceList[0] == YuanSuType.Ice)
                {
                    return ReactionType.BingLao;
                }
                //3雷磁暴
                else if (sourceList[0] == YuanSuType.Storm)
                {
                    return ReactionType.CiBao;
                }
                //3光
                else if (sourceList[0] == YuanSuType.Light)
                {
                    return ReactionType.CiFu;
                }
                //3暗
                else if (sourceList[0] == YuanSuType.Dark)
                {
                    return ReactionType.XiongSha;
                }
            }
        }
        return ReactionType.None;
    }

    /// <summary>
    /// 给一组元素 反应后返回另一组
    /// </summary>
    /// <param name="sourceList"></param>
    /// <returns></returns>
    public YuanSuReactionRes CheckYuanSuReaction(List<YuanSuType> sourceList)
    {
        YuanSuReactionRes res = new YuanSuReactionRes();
        List<YuanSuType> removedYuanSuList = new List<YuanSuType>();
        List<YuanSuType> resList = new List<YuanSuType>();
        for(int i = 0; i < sourceList.Count; i++)
        {
            resList.Add(sourceList[i]);
        }
        YuanSuType[] arr= resList.ToArray();
        List<YuanSuType[]> ListCombination = PermutationAndCombination<YuanSuType>.GetCombination(arr, 2); //求全部的2-2组合
        List<YuanSuType> reactedYuanSu = null;
        ReactionType reactionType = ReactionType.None;
        //2-2组合
        if (ListCombination != null)
        {
            for (int i = 0; i < ListCombination.Count; i++)
            {

                List<YuanSuType> single = ListCombination[i].ToList();
                reactionType = JudgeIfCanReaction(single);
                if (reactionType != ReactionType.None)
                {
                    reactedYuanSu = single;
                    break;
                }
            }
        }

        if (reactedYuanSu == null&&arr.Length>=3)
        {
            ListCombination = PermutationAndCombination<YuanSuType>.GetCombination(arr, 3); //求全部的3-3组合
            //3-3组合
            for (int i = 0; i < ListCombination.Count; i++)
            {
                List<YuanSuType> single = ListCombination[i].ToList();
                reactionType = JudgeIfCanReaction(single);
                if (reactionType != ReactionType.None)
                {
                    reactedYuanSu = single;
                    break;
                }
            }
        }
        if (reactedYuanSu != null)
        {
            for (int i = resList.Count - 1; i >= 0; i--)
            {
                for (int j = reactedYuanSu.Count - 1; j >= 0; j--)
                {
                    YuanSuType yuan = reactedYuanSu[j];
                    removedYuanSuList.Add(yuan);
                    resList.Remove(yuan);
                    reactedYuanSu.Remove(yuan);

                }
            }
        }
        res.remainYuanSuList = resList;
  
        res.removedYuanSuList = removedYuanSuList;
        res.reactionType = reactionType;
        return res;
    }


    /// <summary>
    /// 计算剧变伤害数值
    /// </summary>
    /// <returns></returns>
    public int CalcJuBianDamage(PeopleData p, int rate,int jingTong)
    {
        //SinglePropertyData jingTongPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.JingTong, p);
        //if (jingTongPro == null)
        //{

        //}
        float jingTongAdd = 0.01f * 0.5f*(jingTong);
        int centerLevel = RoleManager.Instance.FindCenterLevel(p);
        int res = (int)(((centerLevel - 1) * 28 + 53) * rate * 0.01f * (1 + jingTongAdd));
        return res;
 
            
        //    (int)(((RoleManager.Instance.FindCenterLevel(p) - 1) * 28 + 53) **0.01f*(1+ jingTongAdd) rate*0.01f*(1+ jingTongAdd));
    }
}

/// <summary>
/// 元素反应剩下的结果
/// </summary>
public class YuanSuReactionRes
{
    public List<YuanSuType> remainYuanSuList = new List<YuanSuType>();//反应完剩下的
    public List<YuanSuType> removedYuanSuList = new List<YuanSuType>();//被移除的元素
    public ReactionType reactionType;//进行了什么反应
}

/// <summary>
/// 元素类型
/// </summary>
public enum YuanSuType
{
    None=0,
    Water=1,
    Fire=2,
    Storm=3,
    Ice=4,
    Light=5,
    Dark=6,
    End
}

/// <summary>
/// 反应类型
/// </summary>
public enum ReactionType
{
    None=0,
    BaoZheng=1,//爆蒸
    DianLiu=2,//电流
    DongJiang=3,//冻僵
    BaoZha=4,//爆炸
    RongHua=5,//融化
    LeiQiang=6,//雷枪
    ChaoShi=7,//潮湿
    ZhuoShao=8,//灼烧
    BingLao=9,//冰牢
    CiBao = 10,//磁暴
    CiFu = 11,//赐福
    XiongSha=12,//凶煞
}