using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 弹出的tips
/// </summary>
public class TipsPanel : EmptyClosePanel
{
    public string str;
    public Vector2 contentPos;//位置

    public Transform trans_TheBoundcontent;//所有内容父物体 用它判断边界
  //  public Transform trans_blockRange;//这个范围内点击不会关闭一般用bg
    public Text txt_name;//名字
    public Text txt_des;//描述
    public Image img_icon;//icon

    //世界边界
    public float leftBorder;
    public float rightBorder;
    public float topBorder;
    public float downBorder;
    private float myWidth;
    private float myHeight;

    public override void Init(object[] args)
    {
        base.Init(args);
        this.str = (string)args[0];
        this.contentPos = (Vector2)args[1];
    }

    public override void OnOpenIng()
    {

        Debug.Log("该tips的contentPos的y坐标为" + contentPos.y);
        //SetBlockParent(trans_blockRange);
        ShowDetail();
        SetHeight();
        //trans_content.position = contentPos;
        ShowPos();
        base.OnOpenIng();

    }

    /// <summary>
    /// 测试
    /// </summary>
    public override void Update()
    {
        base.Update();
        //if (Input.GetKeyUp(KeyCode.W))
        //{
        //    ShowPos();

        //}
        //ShowPos();

    }

    /// <summary>
    /// 设置尺寸
    /// </summary>
    void SetHeight()
    {
        RectTransform rt = trans_content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, txt_des.preferredHeight+21);
        //trans_content.GetComponent<RectTransform>().sizeDelta.y = txt_des.preferredHeight;
    }

    /// <summary>
    /// 显示位置 如果这里出现出框bug 那就是锚点设置有问题content的锚点为x0y1
    /// </summary>
    void ShowPos()
    {
        //首先出现在原始位置
        trans_content.position = contentPos;
        //世界坐标右上角
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f,
         Mathf.Abs(-Camera.main.transform.position.z)));
        //世界坐标左边界
        leftBorder = Camera.main.transform.position.x - (cornerPos.x - Camera.main.transform.position.x);
        //世界坐标右边界
        rightBorder = cornerPos.x;
        //世界坐标上边界
        topBorder = cornerPos.y;
        //世界坐标下边界
        downBorder = Camera.main.transform.position.y - (cornerPos.y - Camera.main.transform.position.y);

      

        //index从0到3分别为 左下 左上 右上 右下的世界坐标
        Vector3[] corners = new Vector3[4];
        trans_content.GetComponent<RectTransform>().GetWorldCorners(corners);
        //width = rightBorder - leftBorder;
        //height = topBorder - downBorder;

        Vector3 leftDownVec = corners[0];//左下
        Vector3 leftUpVec = corners[1];//左上
        Vector3 rightUpVec = corners[2];//右上
        Vector3 rightDownVec = corners[3];//右下

        myWidth = rightUpVec.x - leftUpVec.x;
        myHeight = leftUpVec.y - leftDownVec.y;

        if (leftUpVec.y >= topBorder)
        {
            Debug.Log("到达上边界");
            trans_content.position = new Vector3(trans_content.position.x, topBorder, 0);
        }
        //下
        if (leftDownVec.y <= downBorder)
        {
            Debug.Log("到达下边界");
            Debug.Log("原来的pos是" + trans_content.position.y);
            trans_content.position = new Vector3(trans_content.position.x, downBorder + myHeight, 0);
            Debug.Log("新的pos是" + trans_content.position.y);

        }
        //左
        if (leftDownVec.x <= leftBorder)
        {
            Debug.Log("到达左边界");

            trans_content.position = new Vector3(leftBorder + myWidth/2, trans_content.position.y, 0);
        }
        //右
        if (rightDownVec.x >= rightBorder)
        {
            Debug.Log("到达右边界");
            trans_content.position = new Vector3(rightBorder - myWidth / 2, trans_content.position.y, 0);

        }
    }

    /// <summary>
    /// 显示细节
    /// </summary>
    void ShowDetail()
    {
 
        
            txt_des.SetText(str);
         
      
        
    }



  

}
/// <summary>
/// tips数据
/// </summary>
public class TipsData
{
    public TipsType tipsType;
    public string customizeContent;//自定义内容
 
    public int awardId;//奖励id
}
/// <summary>
/// tips类型
/// </summary>
public enum TipsType
{
    Common,
    Customize,//自定义
}