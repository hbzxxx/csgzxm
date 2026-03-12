using DG.Tweening;
using Framework.Data;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using cfg;
/// <summary>
/// 工具类
/// </summary>
public static class CommonUtil
{    //默认密钥向量
    private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    /// <summary>
    /// 根据T值，计算贝塞尔曲线上面相对应的点
    /// </summary>
    /// <param name="t"></param>T值
    /// <param name="p0"></param>起始点
    /// <param name="p1"></param>控制点
    /// <param name="p2"></param>目标点
    /// <returns></returns>根据T值计算出来的贝赛尔曲线点
    private static Vector2 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    /// <summary>
    /// 获取存储贝塞尔曲线点的数组
    /// </summary>
    /// <param name="startPoint"></param>起始点
    /// <param name="controlPoint"></param>控制点
    /// <param name="endPoint"></param>目标点
    /// <param name="segmentNum"></param>采样点的数量
    /// <returns></returns>存储贝塞尔曲线点的数组
    public static Vector2[] GetBeizerList(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint, int segmentNum)
    {
        Vector2[] Path = new Vector2[segmentNum + 1];
        Path[0] = startPoint;
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector2 pixel = CalculateCubicBezierPoint(t, startPoint,
                controlPoint, endPoint);
            Path[i] = pixel;

        }
        return Path;

    }


    static public double ToDouble(this string str)
    {
        double ret = 0;

        try
        {
            if (!String.IsNullOrEmpty(str))
            {
                bool success = double.TryParse(str, out ret);
                if (!success)
                    Debug.LogError("str无法转double---" + str);
            }
        }
        catch (Exception)
        {
        }
        return ret;
    }

    static public float ToFloat(this string str)
    {
        float ret = 0;

        try
        {
            if (!String.IsNullOrEmpty(str))
            {
                bool success = float.TryParse(str, out ret);
                if (!success)
                    Debug.LogError("str无法转float---" + str);
            }
        }
        catch (Exception)
        {
        }
        return ret;
    }
    static public byte ToByte(this string str)
    {
        byte ret = 0;

        try
        {
            if (!String.IsNullOrEmpty(str))
            {
                ret = Convert.ToByte(str);
            }
        }
        catch (Exception)
        {
        }
        return ret;
    }
    static public Int32 ToInt32(this string str)
    {
        Int32 ret = 0;

        try
        {
            if (!String.IsNullOrEmpty(str))
            {
                ret = Convert.ToInt32(str);
            }
        }
        catch (Exception)
        {
        }
        return ret;
    }
    static public UInt64 ToUInt64(this string str)
    {
        UInt64 ret = 0;

        try
        {
            if (!String.IsNullOrEmpty(str))
            {
                ret = Convert.ToUInt64(str);
            }
        }
        catch (Exception)
        {
        }
        return ret;
    }
    static public Int32 ToInt32(this double val)
    {
        Int32 ret = 0;

        try
        {

            ret = Convert.ToInt32(val);

        }
        catch (Exception)
        {
        }
        return ret;
    }

    static public Int64 ToInt64(this string str)
    {
        Int64 ret = 0;

        try
        {
            if (!String.IsNullOrEmpty(str))
            {
                ret = Convert.ToInt64(str);
            }
        }
        catch (Exception)
        {
        }
        return ret;
    }
    /// <summary>
    /// 获取ab的x距离
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float GetDistanceX(Transform a, Transform b)
    {
        return Mathf.Abs(b.position.x - a.position.x);

    }
    /// <summary>
    /// 把2|10001|20$1|10002|30转换为[[2,10001,20],[1,10002,30]] 支持小数
    /// </summary>
    /// <returns></returns>
    public static List<List<float>> Split2CfgFloat(string source)
    {
        string[] str = source.Split('$');
        List<List<float>> totalList = new List<List<float>>();

        for (int i = 0; i < str.Length; i++)
        {
            string singleStr = str[i];
            string[] singleArr = singleStr.Split('|');

            List<float> smallList = new List<float>();
            for (int j = 0; j < singleArr.Length; j++)
            {
                float theFloat = singleArr[j].ToFloat();
                smallList.Add(theFloat);
            }
            totalList.Add(smallList);
        }
        return totalList;
    }

    /// <summary>
    /// 把2|10001|20$1|10002|30,2|10001|20$1|10002|30转换为{[[2,10001,20],[1,10002,30]],[[2,10001,20],[1,10002,30]]
    /// </summary>
    /// <returns></returns>
    public static List<List<List<int>>> SplitThreeCfg(string source)
    {
        string[] str = source.Split(',');
        List<List<List<int>>> res = new List<List<List<int>>>();

        for (int i = 0; i < str.Length; i++)
        {
            string str1 = str[i];
            string[] arr1 = str1.Split('$');
            List<List<int>> list2 = new List<List<int>>();
            for (int j = 0; j < arr1.Length; j++)
            {
                string str2 = arr1[j];
                string[] arr2 = str2.Split('|');
                List<int> list3 = new List<int>();
                for (int k = 0; k < arr2.Length; k++)
                {
                    string str3 = arr2[k];
                    list3.Add(str3.ToInt32());
                }
                list2.Add(list3);
            }
            res.Add(list2);
        }


        return res;
    }


    /// <summary>
    /// 把2|10001|20$1|10002|30转换为[[2,10001,20],[1,10002,30]]
    /// </summary>
    /// <returns></returns>
    public static List<List<int>> SplitCfg(string source)
    {
        string[] str = source.Split('$');
        List<List<int>> totalList = new List<List<int>>();

        for (int i = 0; i < str.Length; i++)
        {
            string singleStr = str[i];
            string[] singleArr = singleStr.Split('|');

            List<int> smallList = new List<int>();
            for (int j = 0; j < singleArr.Length; j++)
            {
                int theInt = singleArr[j].ToInt32();
                smallList.Add(theInt);
            }
            totalList.Add(smallList);
        }
        return totalList;
    }

    /// <summary>
    /// 把1|10002|30转换为[1,10002,30]
    /// </summary>
    /// <returns></returns>
    public static List<int> SplitCfgOneDepth(string source)
    {

        string[] singleArr = source.Split('|');

        List<int> res = new List<int>();
        for (int j = 0; j < singleArr.Length; j++)
        {
            int theInt = singleArr[j].ToInt32();
            res.Add(theInt);
        }

        return res;
    }
    /// <summary>
    /// 把1|10002|30转换为[1,10002,30] float
    /// </summary>
    /// <returns></returns>
    public static List<float> SplitCfgFloatOneDepth(string source)
    {

        string[] singleArr = source.Split('|');

        List<float> res = new List<float>();
        for (int j = 0; j < singleArr.Length; j++)
        {
            float theFloat = singleArr[j].ToFloat();
            res.Add(theFloat);
        }

        return res;
    }
    /// <summary>
    /// 把1|10002|30转换为[1,10002,30] string形式
    /// </summary>
    /// <returns></returns>
    public static List<string> SplitCfgStringOneDepth(string source)
    {

        string[] singleArr = source.Split('|');

        List<string> res = new List<string>();
        for (int j = 0; j < singleArr.Length; j++)
        {
            string va = singleArr[j];
            res.Add(va);
        }

        return res;
    }

    /// <summary>
    /// 通过权重得到第几项
    /// </summary>
    /// <returns></returns>
    public static int GetIndexByWeight(List<int> weightList)
    {
        int totalWeight = 0;

        for (int i = 0; i < weightList.Count; i++)
        {
            totalWeight += weightList[i];
        }

        //上下限
        List<int> AList = new List<int>();
        List<int> BList = new List<int>();

        //int ultra = 1000;
        //int val = RandomManager.Next(1, (totalWeight + 1)* ultra);
        //val = 10000;
        //int index = val % totalWeight;

        int index = RandomManager.Next(1, (totalWeight + 1));

        for (int j = 0; j < weightList.Count; j++)
        {
            int currWeight = weightList[j];
            int A = 1;
            int B = 0;
            if (j > 0)
            {
                for (int k = 0; k < j; k++)
                {
                    A = A + weightList[k];
                }
            }
            else
            {
                A = 1;
            }
            B = A - 1 + currWeight;
            AList.Add(A);
            BList.Add(B);
        }
        int choosedIndex = 0;
        for (int i = 0; i < AList.Count; i++)
        {
            int theA = AList[i];
            int theB = BList[i];
            if (index >= theA && index <= theB)
            {
                choosedIndex = i;
                break;
            }
        }
        return choosedIndex;

    }
    /// <summary>
    /// 通过权重得到第几项string数组
    /// </summary>
    /// <returns></returns>
    public static int GetIndexByWeight(List<string> weightList)
    {
        int totalWeight = 0;
        List<int> weightIntList = new List<int>();
        for (int i = 0; i < weightList.Count; i++)
        {
            weightIntList.Add(weightList[i].ToInt32());
        }

        for (int i = 0; i < weightIntList.Count; i++)
        {
            totalWeight += weightIntList[i];
        }

        //上下限
        List<int> AList = new List<int>();
        List<int> BList = new List<int>();

        //int ultra = 1000;
        //int val = RandomManager.Next(1, (totalWeight + 1)* ultra);
        //val = 10000;
        //int index = val % totalWeight;

        int index = RandomManager.Next(1, (totalWeight + 1));

        for (int j = 0; j < weightIntList.Count; j++)
        {
            int currWeight = weightIntList[j];
            int A = 1;
            int B = 0;
            if (j > 0)
            {
                for (int k = 0; k < j; k++)
                {
                    A = A + weightIntList[k];
                }
            }
            else
            {
                A = 1;
            }
            B = A - 1 + currWeight;
            AList.Add(A);
            BList.Add(B);
        }
        int choosedIndex = 0;
        for (int i = 0; i < AList.Count; i++)
        {
            int theA = AList[i];
            int theB = BList[i];
            if (index >= theA && index <= theB)
            {
                choosedIndex = i;
                break;
            }
        }
        return choosedIndex;

    }


    /// <summary>
    /// 当前是哪个阶段
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    public static int GetPhaseIndex(int process, Vector2Int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (process >= arr[i].x && process < arr[i].y)
            {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// 当前是哪个阶段
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    public static int GetPhaseIndex(float process, List<float> arr)
    {
        for (int i = 0; i < arr.Count; i++)
        {
            if (i < arr.Count - 1)
            {
                if (process >= arr[i] && process < arr[i + 1])
                {
                    return i;
                }
            }
            else
            {
                if (process >= arr[i])
                {
                    return i;
                }
            }

        }
        return -1;
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    /// <returns></returns>
    public static List<T> Shuffle<T>(List<T> theList)
    {
        int currentIndex;


        T tempValue;

        for (int i = 0; i < theList.Count; i++)
        {


            currentIndex = RandomManager.Next(0, theList.Count - i);


            tempValue = theList[currentIndex];


            theList[currentIndex] = theList[theList.Count - i - 1];


            theList[theList.Count - i - 1] = tempValue;


        }
        return theList;
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    /// <returns></returns>
    public static RepeatedField<T> Shuffle<T>(RepeatedField<T> theList)
    {
        int currentIndex;


        T tempValue;

        for (int i = 0; i < theList.Count; i++)
        {


            currentIndex = RandomManager.Next(0, theList.Count - i);


            tempValue = theList[currentIndex];


            theList[currentIndex] = theList[theList.Count - i - 1];


            theList[theList.Count - i - 1] = tempValue;


        }
        return theList;
    }
    /// <summary>
    /// 选择排序从小到大
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="theList"></param>
    /// <returns></returns>
    public static List<T> SelectSort<T>(List<T> theList) where T : IComparable
    {
        for (int i = 0; i < theList.Count - 1; i++)
        {
            for (int j = 0; j < theList.Count - 1 - i; j++)
            {
                //前面的大于后面的，则二者交换
                if (theList[j].CompareTo(theList[j + 1]) == 1)
                {
                    T temp = theList[j];
                    theList[j] = theList[j + 1];
                    theList[j + 1] = temp;

                }
            }
        }
        return theList;
    }

 

    /// <summary>
    /// 通过稀有度得到稀有名
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static string RarityName(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Fan:
                return "凡";
            case Rarity.Huang:
                return "黄";
            case Rarity.Xuan:
                return "玄";
            case Rarity.Di:
                return "地";
            case Rarity.Tian:
                return "天";

        }
        return "";
    }
    /// <summary>
    /// 通过品质得到品质名
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static string QualityName(Quality quality)
    {
        switch (quality)
        {
            case Quality.Green:
                return "绿色";
            case Quality.Blue:
                return "蓝色";
            case Quality.Purple:
                return "紫色";
            case Quality.Orange:
                return "橙色";
            case Quality.Gold:
                return "金色";

        }
        return "";
    }
    /// <summary>
    /// 通过品质得到品质名
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static Color QualityColor(Quality quality)
    {
        switch (quality)
        {
            case Quality.Green:
                return ConstantVal.color_fan;
            case Quality.Blue:
                return ConstantVal.color_huang;
            case Quality.Purple:
                return ConstantVal.color_xuan;
            case Quality.Orange:
                return ConstantVal.color_di;
            case Quality.Gold:
                return ConstantVal.color_tian;

        }
        return Color.black;
    }

    /// <summary>
    /// 通过品质得到品质名
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static Color RarityColor(Rarity rarity)
    {
        
        switch (rarity)
        {
            case Rarity.Fan:
                return ConstantVal.color_fan;
            case Rarity.Huang:
                return ConstantVal.color_huang;
            case Rarity.Xuan:
                return ConstantVal.color_xuan;
            case Rarity.Di:
                return ConstantVal.color_di;
            case Rarity.Tian:
                return ConstantVal.color_tian;

        }
        return Color.white;
    }


    /// <summary>
    /// 通过品质得到黑一点的品质名
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static Color RarityBlackColor(Rarity rarity)
    {

        switch (rarity)
        {
            case Rarity.Fan:
                return ConstantVal.color_blackfan;
            case Rarity.Huang:
                return ConstantVal.color_blackhuang;
            case Rarity.Xuan:
                return ConstantVal.color_blackxuan;
            case Rarity.Di:
                return ConstantVal.color_blackdi;
            case Rarity.Tian:
                return ConstantVal.color_blacktian;

        }
        return Color.black;
    }
    //public static void ShowStudentTxtColor(this Text txt_name, PeopleData PeopleData)
    //{
    //    switch ((Rarity)PeopleData.StudentRarity)
    //    {
    //        case Rarity.Huang:
    //            txt_name.color = Color.white;
    //            break;

    //        case Rarity.Xuan:
    //            txt_name.color = Color.green;
    //            break;

    //        case Rarity.Di:
    //            txt_name.color = Color.blue;
    //            break;

    //        case Rarity.Tian:
    //            txt_name.color = new Color32(255, 0, 255, 255);
    //            break;

    //    }
    //}

    /// <summary>
    /// 显示物品tag
    /// </summary>
    /// <param name="trans_tag"></param>
    /// <param name="itemData"></param>
    public static void ShowItemTag(this Transform trans_tag,ItemData itemData)
    {
        ItemSetting setting = DataTable.FindItemSetting(itemData.settingId);
        if (setting == null) return;
        Text txt_tag = trans_tag.GetComponentInChildren<Text>();
        if (setting.ItemType.ToInt32() == (int)ItemType.Equip)
        {
            //if (RoleManager.Instance._CurGameInfo.playerPeople.curEquipItem != null
            //    && itemData.equipProtoData.onlyId ==RoleManager.Instance._CurGameInfo.playerPeople.curEquipItem.onlyId)
            //{
            //    trans_tag.gameObject.SetActive(true);
            //    txt_tag.SetText("已装备");
            //}
            //else
            //{
            //    trans_tag.gameObject.SetActive(false);
            //}
        }
        else if (setting.ItemType.ToInt32() == (int)ItemType.Gem)
        {
            if (itemData.gemData.isInlayed)
            {
                trans_tag.gameObject.SetActive(true);
                txt_tag.SetText("已镶嵌");
            }
            else
            {
                trans_tag.gameObject.SetActive(false);
            }
        }
        else
        {
            trans_tag.gameObject.SetActive(false);
        }
    }



    /// <summary>
    /// 显示头像
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="itemData"></param>
    public static void ShowItemIcon(this Image icon,ItemData itemData)
    {
        //if (itemData == null || itemData.setting == null)
        //{
        //    Debug.LogError("ShowItemIcon: itemData 或 itemData.setting 为空");
        //    return;
        //}

        string iconPath = "";
        if (itemData.setting != null)
        {
            if (itemData.setting.ItemType.ToInt32() == (int)ItemType.TouXiang)
            {
                string[] arr = itemData.setting.UiName.Split('|');
                if (RoleManager.Instance._CurGameInfo.playerPeople.gender == (int)Gender.Male)
                {
                    iconPath = arr[0];
                }
                else
                {
                    iconPath = arr[1];
                }

            }
            else
            {
                iconPath = itemData.setting.UiName;
            }
            icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + iconPath);
        }
    }

    /// <summary>
    /// 通过逻辑坐标得到逻辑index
    /// </summary>
    /// <returns></returns>
    public static int GetLogicIndexByLogicPos(Vector2Int logicPos,int lieNum)
    {
       return logicPos.x+ logicPos.y* lieNum;
    }

    /// <summary>
    /// 通过逻辑index得到逻辑坐标
    /// </summary>
    /// <returns></returns>
    public static Vector2Int GetLogicPosByLogicIndex(int logicIndex, int lieNum)
    {
        int x = logicIndex % lieNum;
        int y = logicIndex / lieNum;
        return new Vector2Int(x, y);
    }


    /// <summary>
    /// 贝塞尔曲线移动 height为曲的多少
    /// </summary>
    /// <param name="moveItem"></param>
    public static void LocalBezierMove2(Transform moveItem, Vector3 startPos, Vector3 endPos, int resolution, float height, Action cb, float time = 0.8f)
    {
        Vector3 vec1 = (endPos - startPos).normalized;
        Vector3 vec2 = new Vector2(-vec1.y, vec1.x);
        var bezierControlPoint = (startPos + endPos) * 0.5f + vec2 * height;
        Vector3[] _path = new Vector3[resolution];//resolution为int类型，表示要取得路径点数量，值越大，取得的路径点越多，曲线最后越平滑
        for (int i = 0; i < resolution; i++)
        {
            var t = (i + 1) / (float)resolution;//归化到0~1范围
            _path[i] = GetBezierPoint(t, startPos, bezierControlPoint, endPos);//使用贝塞尔曲线的公式取得t时的路径点
        }
        moveItem.DOLocalPath(_path, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (cb != null)
                cb();
        });

    }

    /// <summary>
    /// 贝塞尔曲线移动
    /// </summary>
    /// <param name="moveItem"></param>
    public static void LocalBezierMove(Transform moveItem,Vector3 startPos,Vector3 endPos,int resolution, float height,Action cb,float time=0.8f)
    {
        var bezierControlPoint = (startPos + endPos) * 0.5f + (Vector3.up * height);
        Vector3[] _path = new Vector3[resolution];//resolution为int类型，表示要取得路径点数量，值越大，取得的路径点越多，曲线最后越平滑
        for (int i = 0; i < resolution; i++)
        {
            var t = (i + 1) / (float)resolution;//归化到0~1范围
            _path[i] = GetBezierPoint(t, startPos, bezierControlPoint, endPos);//使用贝塞尔曲线的公式取得t时的路径点
        }
        moveItem.DOLocalPath(_path, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (cb != null)
                cb();
        });

    }

    /// <param name="t">0到1的值，0获取曲线的起点，1获得曲线的终点</param>
    /// <param name="start">曲线的起始位置</param>
    /// <param name="center">决定曲线形状的控制点</param>
    /// <param name="end">曲线的终点</param>
    public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }


    /// <summary>
    /// 弟子所有属性影响比例
    /// </summary>
    public static void ShowStudentInfulenceRateByTotalPro(Transform parent,int proNum)
    {
        TestStudentTotalProInfluenceSetting setting = DataTable.FindtestStudentTotalProInfluenceByPro(proNum);

        List<int> weightList = CommonUtil.SplitCfgOneDepth(setting.Influence);
        //List<Quality> qualityList = new List<Quality> { Quality.Green, Quality.Blue, Quality.Purple, Quality.Orange, Quality.Gold };

        for(int i = 0; i < weightList.Count; i++)
        {
            PanelManager.Instance.OpenSingle<SingleStudentInfluenceRateView>(parent, (Quality)(i + 1), weightList[i]);
        }

    }

    public static Sprite StudentKuang(PeopleData p)
    {
        if (p == null)
        {

        }
        int rarity =  p.studentRarity;
        if (rarity == 0)
            rarity = 1;
        rarity = Mathf.Clamp(rarity, 1, 5);
        if (p.isPlayer)
        {
            return ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + "img_dizijibiek6");

        }
        if(p.talent==(int)StudentTalent.LianGong)     
            return ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_dizik_" + rarity);
        else
            return ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_dizikn_" + rarity);
    }

    public static Sprite StudentKuangNew(PeopleData p)
    {
        int rarity = p.studentRarity;
        if (rarity == 0)
            rarity = 1;
        rarity = Mathf.Clamp(rarity, 1, 5);
        if (p.talent == (int)StudentTalent.LianGong)
            return ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_diziknew_" + rarity);
        else
            return ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_diziknew_" + rarity);
    }


    public static Sprite StudentBgKuang(PeopleData p) {
        int rarity = p.studentRarity;
        if (rarity == 0)
            rarity = 1;
        // 限制 rarity 在有效范围内（资源文件只有 1-5）
        rarity = Mathf.Clamp(rarity, 1, 5);
        if (p.talent == (int)StudentTalent.LianGong)
            return ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_dizikbgk_" + rarity);
        else
            return ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_diziknbgk_" + rarity);
    }
    public static void ShowDangerouseTxt(this Text txt,int dangerouseLevel)
    {
        string dangerousStr = "";
        Color dangerouseColor = Color.white;
        switch (dangerouseLevel)
        {
            case 1:
                dangerousStr = "简单";
                dangerouseColor = ConstantVal.color_fan;
                break;
            case 2:
                dangerousStr = "普通";
                dangerouseColor = ConstantVal.color_huang;
                break;
            case 3:
                dangerousStr = "困难";
                dangerouseColor = ConstantVal.color_xuan;
                break;
            case 4:
                dangerousStr = "极难";
                dangerouseColor = ConstantVal.color_di;
                break;
            case 5:
                dangerousStr = "超难";
                dangerouseColor = ConstantVal.color_tian;
                break;
        }
        txt.SetText(dangerousStr);
        txt.color = dangerouseColor;
    }

    /// <summary>
    /// 显示物品外框
    /// </summary>
    /// <param name="img"></param>
    /// <param name="itemData"></param>
    public static void ShowItemFrameImg(this Image img_bottom, ItemData itemData)
    {
        if ((Quality)(int)itemData.quality == Quality.None)
        {
            ItemSetting setting = DataTable.FindItemSetting(itemData.settingId);
            if (setting == null)
            {
                Debug.LogError($"[配置表错误] 找不到物品配置，settingId={itemData.settingId}，请检查shop.xlsx中的param字段是否引用了已删除的物品");
                return;
            }
            itemData.quality = setting.Quality.ToInt32();
        }

        switch ((Quality)(int)itemData.quality)
        {
            case Quality.Green:

                img_bottom.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.itemGreenBgName);
                break;

            case Quality.Blue:

                img_bottom.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.itemBlueBgName);

                break;

            case Quality.Purple:

                img_bottom.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.itemPurpleBgName);

                break;
            case Quality.Orange:

                img_bottom.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.itemOrangeBgName);

                break;
            case Quality.Gold:

                img_bottom.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.itemGoldBgName);
                PanelManager.Instance.CloseAllSingle(img_bottom.transform);
                //PanelManager.Instance.OpenSingle<jinsewuping>(img_bottom.transform);
                break;
        }
    }


    /// <summary>
    /// 显示弟子稀有度背景
    /// </summary>
    /// <param name="img"></param>
    /// <param name="itemData"></param>
    public static void ShowStudentRarityBgImg(this Image img, int rarity)
    {
        if (rarity <= 0)
            rarity = 1;
        img.sprite= ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + "img_dizijibie" + rarity);
    }


    public static void BtnChooseColor(List<Button> btns,Button cbtn, Color choose, Color unchoose, Sprite choosesprite = null, Sprite unchooseprite = null) {
        for (int i = 0; i < btns.Count; i++)
        {
            if (btns[i] == cbtn)
            {
                if (choose != null) btns[i].GetComponentInChildren<Text>().color = choose;
                if (choosesprite != null) btns[i].GetComponent<Image>().sprite = choosesprite;
            }
            else {
                if (unchoose != null) btns[i].GetComponentInChildren<Text>().color = unchoose;
                if (unchooseprite != null) btns[i].GetComponent<Image>().sprite = unchooseprite;
            }
        }
    }

    /// <summary>
    /// 删掉该文件夹中的所有文件和文件夹
    /// </summary>
    public static void DeleteAllFile(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        // 如果目标目录不存在，无视
        if (!dir.Exists)
        {
            return;
        }
        FileInfo[] fil = dir.GetFiles();
        DirectoryInfo[] dii = dir.GetDirectories();
        foreach (FileInfo f in fil)
        {
            f.Delete();

        }
        //获取子文件夹内的文件列表，递归遍历  
        foreach (DirectoryInfo d in dii)
        {
            DeleteAllFile(d.FullName);
        }
        dir.Delete();
    }


    /// <summary>
    /// DES加密字符串
    /// </summary>
    /// <param name="encryptString">待加密的字符串</param>
    /// <param name="encryptKey">加密密钥,要求为8位</param>
    /// <returns>加密成功返回加密后的字符串，失败返回源串 </returns>
    public static string EncryptDES(string encryptString, string encryptKey)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));//转换为字节
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();//实例化数据加密标准
            MemoryStream mStream = new MemoryStream();//实例化内存流
                                                      //将数据流链接到加密转换的流
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }
        catch
        {
            return encryptString;
        }
    }

    /// <summary>
    /// DES解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
    /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
    public static string DecryptDES(string decryptString, string decryptKey)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        catch
        {
            return decryptString;
        }
    }

    public static List<T> ToList<T>(this T[] resource)
    {
        List<T> res = new List<T>();
        for(int i = 0; i < resource.Length; i++)
        {
            res.Add(resource[i]);
        }
        return res;
    }
}
