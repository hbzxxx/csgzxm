using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

[RequireComponent(typeof(Image))]
public class GlowEffect : MonoBehaviour
{

	private Texture2D baseImg, glowImg;
	public GameObject glowObject = null;
	private float glowRange = 5.0f;
	public float glowSize = 1.5f;
	public float glowStrength = 0.5f;

	public Color glowColor = Color.white;
	private Material material;

	// USE RENDER TEXTURE TO GET UNPACKED TEXTURE
	Texture2D GetTextureFromSprite(Sprite sourceSprite)
	{
		Texture2D source = sourceSprite.texture;
		RenderTexture renderTex = RenderTexture.GetTemporary(
					source.width,
					source.height,
					0,
					RenderTextureFormat.Default,
					RenderTextureReadWrite.Linear);

		Graphics.Blit(source, renderTex);
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = renderTex;
		Texture2D readableText = new Texture2D((int)sourceSprite.rect.width, (int)sourceSprite.rect.height, TextureFormat.ARGB32, false);

		Rect spRect = sourceSprite.rect;
		readableText.ReadPixels(new Rect(spRect.xMin, source.height - spRect.yMax, spRect.width, spRect.height), 0, 0);
		Debug.Log(sourceSprite.rect.yMin);

		readableText.Apply();
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary(renderTex);
		return readableText;
	}

	// Use this for initialization
	void Start()
	{
		GenerateGlowObject();
	}

	public void GenerateGlowObject()
	{
		if (glowObject != null)
		{

			return;
		}

		baseImg = GetTextureFromSprite(GetComponent<Image>().sprite);

		glowImg = new Texture2D(baseImg.width + (int)(glowRange * 10), baseImg.height + (int)(glowRange * 10), TextureFormat.RGBA32, false);
		Vector2Int glowImgCen = new Vector2Int((int)(baseImg.width + (int)(glowRange * 10)) / 2, (int)(baseImg.height + (int)(glowRange * 10)) / 2);
		Vector2Int baseImgCen = new Vector2Int(baseImg.width / 2, baseImg.height / 2);

		Graphics.CopyTexture(baseImg, 0, 0, 0, 0, baseImg.width, baseImg.height, glowImg, 0, 0, glowImgCen.x - baseImgCen.x, glowImgCen.y - baseImgCen.y);

		for (int i = 0; i < glowImg.width; i++)
			for (int j = 0; j < glowImg.height; j++)
			{
				int x = i + baseImgCen.x - glowImgCen.x;
				int y = j + baseImgCen.y - glowImgCen.y;
				if (x < 0 || x >= baseImg.width || y < 0 || y >= baseImg.height || glowImg.GetPixel(i, j).a == 0)
					glowImg.SetPixel(i, j, Color.black);
			}
		glowImg.Apply();
		glowObject = new GameObject(this.name + "_GlowEffect", typeof(RectTransform), typeof(Image));
		glowObject.transform.SetParent(this.transform.parent);
		glowObject.transform.SetAsFirstSibling();
		glowObject.GetComponent<RectTransform>().localScale = Vector2.one;
		glowObject.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
		glowObject.GetComponent<RectTransform>().sizeDelta =
			new Vector2((int)(glowImg.width * 1.0f / baseImg.width * GetComponent<RectTransform>().sizeDelta.x),
						(int)(glowImg.height * 1.0f / baseImg.height * GetComponent<RectTransform>().sizeDelta.y));
		glowObject.GetComponent<Image>().sprite = Sprite.Create(glowImg, new Rect(0, 0, glowImg.width, glowImg.height), new Vector2(0.5f, 0.5f));
		glowObject.GetComponent<Image>().material = new Material(Shader.Find("Custom/SpriteGlow"));
		material = glowObject.GetComponent<Image>().material;
	}

	/// <summary>
	/// Called when the script is loaded or a value is changed in the
	/// inspector (Called in the editor only).
	/// </summary>

	private void UpdateEffect()
	{
		if (glowObject != null)
		{
			material.SetFloat("_GlowSize", glowSize);
			material.SetFloat("_GlowStrength", glowStrength);
			material.SetColor("_GlowColor", glowColor);
		}
	}

	void OnValidate()
	{
		UpdateEffect();
	}
}
