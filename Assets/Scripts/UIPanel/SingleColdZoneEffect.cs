using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleColdZoneEffect:FinishKillEffect
{

    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.localPosition = (Vector3)args[0];
    }

    //发生粒子碰撞的回调函数
    private void OnParticleCollision(GameObject other)
    {
        print(other.name);
    }
    //粒子触发的回调函数
    private void OnParticleTrigger()
    {
        //只要勾选了粒子系统的trigger，程序运行后会一直打印
        print("触发了");

        //官方示例，拿来说明
        ParticleSystem ps = transform.GetComponent<ParticleSystem>();

        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();
        //particleSystemTriggerEventType为枚举类型，Enter,Exit,Inside,Outside,对应粒子系统属性面板上的四个选项
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        int numExit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
        //进入触发器，粒子变为红色
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];
            p.startColor = Color.red;
            enter[i] = p;
        }
        //退出触发器 粒子变为蓝绿色
        for (int i = 0; i < numExit; i++)
        {
            ParticleSystem.Particle p = exit[i];
            p.startColor = Color.cyan;
            exit[i] = p;
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
    }

}
