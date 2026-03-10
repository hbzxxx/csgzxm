using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using System;

public class ZhaoChaPeopleManager : CommonInstance<ZhaoChaPeopleManager>
{

    //public PeopleData curZhaoChaPeople;

    public bool isStudentBattle = false;//是弟子代打

    /// <summary>
    /// 生成找茬者
    /// </summary>
    public void GenerateZhaoChaPeople()
    {
      
    }
    /// <summary>
    /// 找茬
    /// </summary>
    public void ZhaoCha()
    {
       
    }


    bool ZhaoChaRes(ZhaoChaHandleType type)
    {
       


        return true;
    }

    /// <summary>
    /// 选择弟子出战
    /// </summary>
    public bool ChoosedStudentToBattle(PeopleData p)
    {
         return false;
    }

    /// <summary>
    /// 赢得找茬
    /// </summary>
    public void WinZhaoCha()
    {
   
    }

    /// <summary>
    /// 输了找茬
    /// </summary>
    public void LoseZhaoCha()
    {
   
    }

    /// <summary>
    /// 我求饶
    /// </summary>
    void MeBeg()
    {
 

    }
    /// <summary>
    /// 我给钱
    /// </summary>
    void MeGiveMoney()
    {
  

    }

    /// <summary>
    /// 宁死不屈
    /// </summary>
    void MeRatherDie()
    {
    

    }
 
    /// <summary>
    /// 杀人爆装备 量大但极品概率小
    /// </summary>
    bool OnKill()
    {
  
        return true;
    }
    /// <summary>
    /// 放走送一个装备（极品概率大
    /// </summary>
    bool OnFree()
    {
 
        return true;

    }
    /// <summary>
    /// 收为徒弟/道侣
    /// </summary>
    bool OnReceive()
    {
        //弹窗
        if (BuildingManager.Instance.CheckIfReachBuildingMaxNeiMenNumLimit())
        {
            //弹窗要替换
             //PanelManager.Instance.OpenPanel<ChooseStudentPanel>(PanelManager.Instance.trans_layer2,
            //        ChooseStudentType.FreeReplaceStudent,
            //      chooseStudentFunc);

            return false;

        }
        else
        {
        
            SuccessReceive();

        
            FinishZhaoCha();
            return true;
        }

     
    }
 
    /// <summary>
    /// 成功获得徒弟
    /// </summary>
    void SuccessReceive()
    {
   
    }



    /// <summary>
    /// 结束找茬
    /// </summary>
    public void FinishZhaoCha()
    {
        EventCenter.Broadcast(TheEventType.ZhaoChaResolved);
    }
}

public enum ZhaoChaHandleType
{
    None=0,
    Battle=1,
    StudentBattle=2,//弟子先上
    GiveMoney=3,
}