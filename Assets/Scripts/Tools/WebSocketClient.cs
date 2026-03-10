using System.Collections;
using System.Collections.Generic;
 using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    public bool isConnect = false;

    string serverId = "0";  // 你的服务器 ID 总服先连

    string connectionId = "";

    public static WebSocketClient Instance;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

    }


    //电脑测试
    public void PCTestLogin(string diSanFangId = "")
    {

        if (!string.IsNullOrWhiteSpace(diSanFangId))
            connectionId = diSanFangId;

        //if (string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TheGuid))
        //{

        //    RoleManager.Instance._CurGameInfo.TheGuid = Guid.NewGuid().ToString();

        //}
        //connectionId = RoleManager.Instance._CurGameInfo.TheGuid;

        OnConnect(serverId);

    }


    void OnConnect(string serverId)
    {
        isConnect = true;
     }
}
