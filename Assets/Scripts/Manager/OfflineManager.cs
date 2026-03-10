using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineManager : CommonInstance<OfflineManager>
{
    
    /// <summary>
    /// 最大离线时间（秒）
    /// </summary>
    public long MaxOfflineTime()
    {
       
        long res = ConstantVal.maxOfflineIncomeHour * 60 * 60;
        int addRate = 0;
        //乾坤井
        if (LianDanManager.Instance.FindMyFarmNum((int)DanFarmIdType.QianKunJing) > 0)
        {
            addRate += ConstantVal.qianKunJingAdd;
        }
        //芥子须弥
        if (LianDanManager.Instance.FindMyFarmNum((int)DanFarmIdType.JieZiXuMi) > 0)
        {
            addRate += ConstantVal.jieZiXuMiAdd;
        }
        res =(long)( res * (1 + addRate * 0.01f));
        return res;
    }
}
