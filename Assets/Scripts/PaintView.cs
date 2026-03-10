//-----------------------------------------------------------------------
// <copyright file="PaintView.cs" company="Codingworks Game Development">
//     Copyright (c) codingworks. All rights reserved.
// </copyright>
// <author> codingworks </author>
// <email> coding2233@163.com </email>
// <time> 2017-12-10 </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PaintView : MonoBehaviour
{
    #region 属性
    public GameObject obj;
    public RectTransform rect_drawRange;//画画范围
    //绘图shader&material
    [SerializeField]
    private Shader _paintBrushShader;
    private Material _paintBrushMat;
    //清理renderTexture的shader&material
    [SerializeField]
    private Shader _clearBrushShader;
    private Material _clearBrushMat;
    //默认笔刷RawImage

    //默认笔刷&笔刷合集
    [SerializeField]
    private Texture _defaultBrushTex;
    //renderTexture
    private RenderTexture _renderTex;

    //绘画的画布
    [SerializeField]
    private RawImage _paintCanvas;
    //笔刷的默认颜色&颜色合集
    [SerializeField]
    private Color _defaultColor;

    
    //笔刷的大小
    float _brushSize;
    //笔刷的间隔大小
    private float _brushLerpSize;
    //默认上一次点的位置
    private Vector2 _lastPoint;
	#endregion

	void Start()
	{
		InitData();
	}

	private void Update()
	{
		Color clearColor = new Color(0, 0, 0, 0);
		if (Input.GetKeyDown(KeyCode.Space))
			_paintBrushMat.SetColor("_Color", clearColor);
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (obj != null)
            {
                Image render = obj.GetComponent<Image>();
                render.material.SetTexture("_DecalTex", _renderTex);

            }
            _paintBrushMat.SetColor("_Color", _defaultColor);

        }
    }


	#region 外部接口

    /// <summary>
    /// 拖拽
    /// </summary>
    public void DragUpdate()
    {
        if (_renderTex && _paintBrushMat)
        {

            if (Input.GetMouseButton(0))
                LerpPaint(Input.mousePosition);

           
        }
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    public void DragEnd()
    {
        if (Input.GetMouseButtonUp(0))
            _lastPoint = Vector2.zero;
    }

    #endregion

    #region 内部函数
	
    //初始化数据
    void InitData()
    {
        _brushSize = 300.0f;
        _brushLerpSize = (_defaultBrushTex.width + _defaultBrushTex.height) / 2.0f / _brushSize;
        _lastPoint = Vector2.zero;

        if (_paintBrushMat == null)
        {
            UpdateBrushMaterial();
        }
        if(_clearBrushMat==null)
        _clearBrushMat = new Material(_clearBrushShader);
        if (_renderTex == null)
        {
            _renderTex = RenderTexture.GetTemporary(512, 512, 24);
            _paintCanvas.texture = _renderTex;
        }
        if (obj != null)
        {
            Image render = obj.GetComponentInChildren<Image>();
            render.material.SetTexture("_DecalTex", null);
        }

        Graphics.Blit(null, _renderTex, _clearBrushMat);
        _brushSize = 15;
        //_paintBrushMat.SetFloat("_Size", _brushSize);

    }

    

    //更新笔刷材质
    private void UpdateBrushMaterial()
    {
        _paintBrushMat = new Material(_paintBrushShader);
        _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
        _paintBrushMat.SetColor("_Color", _defaultColor);
        _paintBrushMat.SetFloat("_Size", _brushSize);
    }

    //插点
    private void LerpPaint(Vector2 point)
    {
        Paint(point);

        if (_lastPoint == Vector2.zero)
        {
            _lastPoint = point;
            return;
        }

        float dis = Vector2.Distance(point, _lastPoint);
        if (dis > _brushLerpSize)
        {
            Vector2 dir = (point - _lastPoint).normalized;
            int num = (int)(dis / _brushLerpSize);
            for (int i = 0; i < num; i++)
            {
                Vector2 newPoint = _lastPoint + dir * (i + 1) * _brushLerpSize;
                Paint(newPoint);
            }
        }
        _lastPoint = point;
    }

    //画点
    private void Paint(Vector2 point)
    {
        if (point.x < Board(Direction.Left) || point.x > Board(Direction.Right)
            || point.y < Board(Direction.Down) || point.y > Board(Direction.Up))
            return;

        Vector2 uv = new Vector2((point.x- Board(Direction.Left)) / (float)rect_drawRange.sizeDelta.x,
           (point.y- Board(Direction.Down)) / (float)rect_drawRange.sizeDelta.y);
        _paintBrushMat.SetVector("_UV", uv);
        Graphics.Blit(_renderTex, _renderTex, _paintBrushMat);
    }

    public float Board(Direction direction)
    {
        float res=0;
        switch (direction)
        {
            case Direction.Left:
                res= Screen.width / 2 + rect_drawRange.localPosition.x - rect_drawRange.sizeDelta.x / 2;
                break;
            case Direction.Right:
                res= Screen.width / 2 + rect_drawRange.localPosition.x + rect_drawRange.sizeDelta.x / 2;
                break;

            case Direction.Up:
                res= Screen.height / 2 + rect_drawRange.localPosition.y + rect_drawRange.sizeDelta.y / 2;
                break;

            case Direction.Down:
                res= Screen.height / 2 + rect_drawRange.localPosition.y - rect_drawRange.sizeDelta.y / 2;
                break;

        }
        return res;
    }
    public enum Direction
    {
        None=0,
        Left,
        Right,
        Up,
        Down
    }
    /// <summary>
    /// 重映射  默认  value 为1-100
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    /// <returns></returns>
    private float Remap(float value, float startValue, float enValue)
    {
        float returnValue = (value - 1.0f) / (100.0f - 1.0f);
        returnValue = (enValue - startValue) * returnValue + startValue;
        return returnValue;
    }

    #endregion

}
