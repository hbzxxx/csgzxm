using DragonBones;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画帧事件需要单独的类
/// </summary>
public class RoleAnim : MonoBehaviour
{
    public BattlePeopleView role;
    public Animator animator;

    public UnityArmatureComponent armature;
    public SkeletonGraphic ske;
    Action _endCallBack;
    public Action endCallBack
    {
        get 
        {
            return _endCallBack;
        }
        set {

            _endCallBack = value;

        }
    }//结束回调

    // Start is called before the first frame update
    void Start()
    {
  

    }

    // Update is called once per frame
    void Update()
    {
  
    }

    /// <summary>
    /// animator播放 0代表循环播放 其余为播放次数
    /// </summary>
    public void Play(string animName, bool loop = true, Action endCallBack = null, float speed = 1)
    {

        //是龙骨动画
        if (armature != null)
        {

            if (!loop)
            {

                armature.animation.Play(animName, 1);

            }

            else
                armature.animation.Play(animName);

        }else if (ske != null)
        {
            
            ske.AnimationState.SetAnimation(0, animName, loop).Complete += delegate
            {
                
                if(endCallBack!=null)
                endCallBack();
            };
        }
        //是帧动画
        else
        {

            // anim.animator.speed = 1;
            _endCallBack = endCallBack;
            animator.Play(animName);

            if (!loop)
            {
                SetAniEvent(animName, endCallBack);
            }
        }

    }

    /// <summary>
    /// 显示刀光
    /// </summary>
    void test()
    {
        Debug.Log("test");
        return;
    }
  

    /// <summary>
    /// 动画播放结束
    /// </summary>
    void OnEnd()
    {
        //EntityManager.Instance.SetAnimatorSpeed(this, 0);
        //animator.speed = 0;

        if (endCallBack != null)
            endCallBack();
        //Debug.Log("bofangjieshu");
    }

    /// <summary>
    /// 设置动画事件
    /// </summary>
    /// <param name="name"> 动画文件名字</param>
    /// <param name="actName">事件</param>
    public void SetAniEvent(string name,Action endCallBack)
    {
        this.endCallBack = endCallBack;

        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            AnimationClip clip = animator.runtimeAnimatorController.animationClips[i];

            if (clip.name == name)
            {
                if (clip.events != null && clip.events.Length > 0)
                    for (int j = 0; j < clip.events.Length; j++)
                    {
                        if (clip.events[j].functionName == "OnEnd")
                            return;
                    }
                //if (clip.events.Length != 0) return;
                AnimationEvent m_animator_0_End = new AnimationEvent();
                //对应事件触发相应函数的名称
                m_animator_0_End.functionName = "OnEnd";
                m_animator_0_End.stringParameter = name;
                //设定对应事件在相应动画时间轴上的触发时间点
                m_animator_0_End.time = clip.length;//结尾
                                                    //把事件添加到时间轴上

                clip.AddEvent(m_animator_0_End);
            }
        }

    }
    /// <summary>
    /// 动画速度
    /// </summary>
    /// <returns></returns>
    public float GetAnimationSpeed()
    {
        if (armature != null)
            return armature.animation.timeScale;
        else
            return animator.speed;
    }


}
