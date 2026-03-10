using DG.Tweening;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuditionManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
     
    public AudioStruct[] audioStructs;
    public Dictionary<AudioClipType, AudioClip> audioDic = new Dictionary<AudioClipType, AudioClip>();

    public List<AudioClip> candidateGuZhengAudioList;//古筝

    public static AuditionManager Instance;

    public Transform trans_audioParent;

    public AudioClipType curAudioClipType;//当前bgm

    public bool muteAudio;//关闭音效

    //人的叫声
    public Dictionary<int, List<AudioClip>> femaleXingGeJiaoDic = new Dictionary<int, List<AudioClip>>();//女生叫
    public Dictionary<int, List<AudioClip>>  maleXingGeJiaoDic = new Dictionary<int, List<AudioClip>>();//男生叫

    public List<AudioClip> monsterJiaoList = new List<AudioClip>();
    private void Awake()
    {
        Instance = this;

        if (PlayerPrefs.HasKey("mute"))
        {
            int muteInt = PlayerPrefs.GetInt("mute");
            if (muteInt == 1)
                muteAudio = true;
            else
                muteAudio = false;
        }
        OnMuteVoice(muteAudio);

        ShowDic();
    }
    // Start is called before the first frame update
    void Start()
    {
      
 
    }
    public void ShowDic()
    {
        // 字典内容
        audioDic = new Dictionary<AudioClipType, AudioClip>();
        for (int i = 0; i < audioStructs.Length; i++)
        {
            // FruitType t = (FruitType)Enum.Parse(typeof(FruitType), sweetPrefabs[i].type, false);
            AudioClipType t = audioStructs[i].type;
            if (!audioDic.ContainsKey(t))
            {
                audioDic.Add(t, audioStructs[i].clip);
            }
            else
            {
                Debug.LogError("key" + "有重复");
            }
        }
        //Debug.Log(FruitPrefabDict.Count);
        if(SceneManager.GetActiveScene().name!="SkillEffect")
        PlayBGM(AudioClipType.BGM_XiuXian);
    }
  

    /// <summary>
    /// 音效
    /// </summary>
    public void PlayVoice(Transform trans, AudioClipType type)
    {
        if (muteAudio)
            return;
        AudioClip clip = audioDic[type];
        AudioSource.PlayClipAtPoint(clip, trans.position, 1);

    }
    /// <summary>
    /// 音效
    /// </summary>
    public void PlayVoice( AudioClipType type)
    {
        if (muteAudio)
            return;
        AudioClip clip = audioDic[type];
        AudioSource.PlayClipAtPoint(clip, transform.position, 1);

    }

    /// <summary>
    /// 音效
    /// </summary>
    public void PlayVoice(Transform trans, AudioClip clip,float volume=1)
    {
        if (muteAudio)
            return;
        if (clip == null)
            return;
        AudioSource.PlayClipAtPoint(clip, trans.position, volume);

    }

    /// <summary>
    /// 改变bgm
    /// </summary>
    public void PlayBGM(AudioClipType type)
    {
        if (!audioDic.TryGetValue(type, out AudioClip clip))
        {
            Debug.Log($"[AuditionManager] BGM不存在: {type}");
            return;
        }
        
        // 检查AudioSource是否启用，如果禁用则不播放
        if (!audioSource.enabled)
        {
            Debug.Log($"[AuditionManager] AudioSource已禁用，跳过BGM播放: {type}");
            return;
        }
        
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        curAudioClipType = type;

    }

    /// <summary>
    /// 播放地图bgm
    /// </summary>
    public void PlayMapBGM(int mapId)
    {
        AudioClipType clipType = AudioClipType.None;
        switch (mapId)
        {
            case 10000:
                clipType= AudioClipType.BGM_YunHaiZong;
                break;
            case 10001:
                clipType = AudioClipType.BGM_JiLiGuo;
                break;
            case 10002:
                clipType = AudioClipType.BGM_YanHuoGuo;
                break;
            case 10003:
                clipType = AudioClipType.BGM_BaiMinGuo;
                break;
            case 10004:
                clipType = AudioClipType.BGM_YuMinGuo;
                break;
        }
        PlayBGM(clipType);
    }

    public void OnMuteVoice(bool mute)
    {
        this.muteAudio = mute;
        audioSource.enabled = !mute;

        if (mute)
        {
            PlayerPrefs.SetInt("mute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("mute", 0);
        }
    }

    /// <summary>
    /// 叫声
    /// </summary>
    /// <param name="xingGe"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    public AudioClip JiaoAudio(int xingGe,Gender gender)
    {
        if (gender == Gender.None)
            return null;
        Dictionary<int, List<AudioClip>> theDic = null;
        string folderPath = "";
        if (gender == Gender.Male)
        {
            theDic = maleXingGeJiaoDic;
             folderPath = ConstantVal.MaleXingGeJiaoFolderPath(xingGe);

        
        }
        else if(gender == Gender.Female)
        {
            theDic = femaleXingGeJiaoDic;

            folderPath = ConstantVal.FeMaleXingGeJiaoFolderPath(xingGe);

        }
        //判断有没有加载成功
        if (!theDic.ContainsKey(xingGe))
        {
            theDic.Add(xingGe, new List<AudioClip>());
            List<AudioClip> candidateList = new List<AudioClip>();
            for (int i = 0; i < 10; i++)
            {
                string name = (i + 1).ToString();
                AudioClip clip = ResourceManager.Instance.GetObj<AudioClip>(folderPath + "/" + name);
                if (clip == null)
                    break;
                else
                {
                    candidateList.Add(clip);
                }
            }
            theDic[xingGe] = candidateList;
        }
        
            int index = RandomManager.Next(0, theDic[xingGe].Count);
            return theDic[xingGe][index];
        
    }
    /// <summary>
    /// 怪物叫
    /// </summary>
    /// <returns></returns>
    public AudioClip MonsterJiaoAudio()
    {
        if (monsterJiaoList.Count == 0)
        {
            for(int i = 0; i < 3; i++)
            {
                AudioClip clip = ResourceManager.Instance.GetObj<AudioClip>(ConstantVal.monsterJiaoFolderPath + (i + 1));
                monsterJiaoList.Add(clip);
            }
         }
        return monsterJiaoList[RandomManager.Next(0, monsterJiaoList.Count)];
    }

}
[System.Serializable]

public struct AudioStruct
{
    public AudioClipType type;
    public AudioClip clip;
}

public enum AudioClipType
{
    None=0,
    MaleBeHit=1,//男人被打
    BGM_XiuXian=2,//修仙
    BGM_Battle=3,//战斗
    BGM_WorldMap=4,//世界
    BGM_MiJing = 5,//秘境
    NewStudentInfo=6,//新弟子提示
    Train=7,//修炼界面
    Gu=8,//打鼓
    FanShu=9,//翻书
    Click=10,//点击
    BGM_Mountain,//山门
    BGM_YunHaiZong,//云海
    BGM_JiLiGuo,//季狸国
    BGM_YanHuoGuo,//厌火国
    BGM_BaiMinGuo,//白民国
    OpenKnapsack,//打开背包
    Equip,//装备
    TieJiangPu,//铁匠铺
    Building,//建造
    MakeFinish,//制造完毕
    EatDan,//吃丹
    BreakThrough,//突破
    GetJinBi,//获取金币
    ZuoZhen,//坐镇
    BGM_JingYingBattle,//精英battle
    BGM_BossBattle,//bossbattle
    BGM_ChooseYuanSu,//开局选元素
    ZhuanChang,//转场
    ChouKa,//抽卡
    BGM_YuMinGuo,//羽民国

}