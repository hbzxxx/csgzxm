using Framework.Data;
 
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPLoadFTP;

public class RangeManager : CommonInstance<RangeManager>
{

    GameInfo gameInfo;
     public override void Init()
    {
        base.Init();
        gameInfo = RoleManager.Instance._CurGameInfo;
 
    }
 
    /// <summary>
    /// 上传排名
    /// </summary>
    public void UploadRange( )
    {
        if (Game.Instance.isRightEdition
            &&Game.Instance.isLogin)
        {
            UpLoadFiles.OnUploadArchive(ConstantVal.GetArchiveSavePath(Game.Instance.curServerIndex),ArchiveManager.Instance.ArchiveUploadName());
             
         }
    }

 
 
}

