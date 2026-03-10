using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyManager : CommonInstance<PropertyManager>
{
     
    public List<SinglePropertyData> CombineProperty(List<SinglePropertyData> list)
    {
        List<SinglePropertyData> res = new List<SinglePropertyData>();
        List<int> idList = new List<int>();
        for(int i = 0; i < list.Count; i++)
        {
            SinglePropertyData theData = list[i];
            if (!idList.Contains(theData.id))
            {
                idList.Add(theData.id);
                SinglePropertyData newData = new SinglePropertyData();
                newData.id = theData.id;
                newData.limit = theData.limit;
                newData.quality = theData.quality;
                res.Add(newData);
            }
            int index = idList.IndexOf(theData.id);
            res[index].num += theData.num;
        }
        return res;
    }

}
