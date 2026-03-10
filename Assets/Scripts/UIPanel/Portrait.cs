using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Portrait : MonoBehaviour
{

    public Image img_houfa;
    public Image img_zhongfa;
    public Image img_face;
    public Image img_bizi;
    public Image img_meimao;
    public Image img_eye;
    public Image img_zui;
    public Image img_body;
    public Image img_lianshi;
    public Image img_qianfa;

    /// <summary>
    /// 刷新
    /// </summary>
    public void Refresh(PeopleData p)
    {
        if (p.portraitIndexList.Count< 10)
            return;
        int houfaIndex = p.portraitIndexList[0];
        int zhongfaIndex = p.portraitIndexList[1];
        int faceIndex = p.portraitIndexList[2];
        int biziIndex = p.portraitIndexList[3];
        int meimaoIndex = p.portraitIndexList[4];
        int eyeIndex = p.portraitIndexList[5];
        int zuiIndex = p.portraitIndexList[6];
        int bodyIndex = p.portraitIndexList[7];
        int lianshiIndex = p.portraitIndexList[8];
        int qianfaIndex = p.portraitIndexList[9];


        string folderPath = "";
        string femaleStr = "";
        switch ((Gender)(int)p.gender)
        {
            case Gender.Male:
                folderPath = ConstantVal.malePortraitPath;

                break;

            case Gender.Female:
                folderPath = ConstantVal.femalePortraitPath;
                femaleStr = "w";
                break;
        }
        img_houfa.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.houfaPath + femaleStr + "houfa_" + (houfaIndex + 1));
        img_zhongfa.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.zhongfaPath + femaleStr + "zhongfa_" + (zhongfaIndex + 1));
        img_face.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.facePath + femaleStr + "face_" + (faceIndex + 1));
        img_bizi.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.biziPath + femaleStr + "bizi_" + (biziIndex + 1));
        img_meimao.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.meimaoPath + femaleStr + "meimao_" + (meimaoIndex + 1));
        img_eye.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.eyePath + femaleStr + "eye_" + (eyeIndex + 1));
        img_zui.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.zuiPath + femaleStr + "zui_" + (zuiIndex + 1));
        img_body.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.bodyPath + femaleStr + "body_" + (bodyIndex + 1));
        img_lianshi.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.lianshiPath + femaleStr + "lianshi_" + (lianshiIndex + 1));
        img_qianfa.sprite = ResourceManager.Instance.GetObj<Sprite>(folderPath + ConstantVal.qianfaPath + femaleStr + "qianfa_" + (qianfaIndex + 1));

    }

}
