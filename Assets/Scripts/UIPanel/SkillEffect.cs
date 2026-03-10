using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : FinishKillEffect
{
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        AudioSource[] sources = this.GetComponents<AudioSource>();
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].enabled = !AuditionManager.Instance.muteAudio;
        }
        AudioSource[] sources2 = this.GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < sources2.Length; i++)
        {
            sources2[i].enabled = !AuditionManager.Instance.muteAudio;
        }
    }
    public override void OnClose()
    {
        base.OnClose();
        SkillEffectSingleParent skillEffectSingleParent = transform.parent.GetComponent<SkillEffectSingleParent>();
        if (skillEffectSingleParent != null)
            PanelManager.Instance.CloseSingle(skillEffectSingleParent);



    }
}
