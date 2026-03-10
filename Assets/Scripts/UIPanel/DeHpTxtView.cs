using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 扣血特效
/// </summary>
public class DeHpTxtView : FinishKillEffect
{
    public Vector2Int RdmYRange=new Vector2Int(20,21);//y坐标随机数
    public Vector2Int RdmXRange = new Vector2Int(-10, 11);//x坐标随机数
    public int yOffset = 60;
    public Text txt;

    public Image img_crit;//暴击标志
    public float critXOffset;//暴击标志x差值
    public float critYOffset;//暴击标志y差值

    //public Material mat_crit;//暴击
    //public Material mat_common;//普通
    public Color32 color_common =new Color32(255,191,0,255);//普通
    public Color32 color_crit = new Color32(231, 37, 66, 255);//暴击

    public override void Init(params object[] args)
    {
        base.Init(args);
        int baseYPos = (int)args[0];
        //mat_crit = ResourceManager.Instance.GetObj<Material>(ConstantVal.critNumMaterialPath);
        //mat_common = ResourceManager.Instance.GetObj<Material>(ConstantVal.commonNumMaterialPath);

        AttackResData attackResData = args[1] as AttackResData;

        txt.SetText(attackResData.showDeHP.ToString());

        int realPosX = RandomManager.Next(RdmXRange.x, RdmXRange.y);
        int realPosY = baseYPos + RandomManager.Next(RdmYRange.x, RdmYRange.y);

        transform.localPosition = new Vector2(realPosX, realPosY);
        txt.transform.localPosition = Vector2.zero;
        txt.color = Color.white;
        if (attackResData.showDeHP < 0)
        {
            if (attackResData.ifCrit)
            {
                txt.color = color_crit;
                txt.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                img_crit.gameObject.SetActive(true);
                img_crit.DOKill();
                img_crit.transform.localPosition = (Vector2)txt.transform.localPosition - new Vector2(txt.preferredWidth / 2, 0) + new Vector2(critXOffset, critYOffset);

                img_crit.transform.DOLocalMoveY(40, 2f);
                img_crit.DOFade(0, 2f);
            }
            else
            {
                img_crit.gameObject.SetActive(false);

                txt.color = color_common;
                txt.transform.localScale = new Vector3(2, 2, 2);

            }
        }
        else
        {
            img_crit.gameObject.SetActive(false);
            txt.color = Color.green;
            txt.transform.localScale = new Vector3(2, 2, 2);
        }



        txt.DOKill();
        txt.transform.DOLocalMoveY(40, 2f);
        txt.DOFade(0, 2f);
    }

    //测试

}
