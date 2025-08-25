using UnityEngine;
using UnityEngine.UI;

public class ResponsiveScrollView : MonoBehaviour
{
    public RectTransform content;
    public GridLayoutGroup gridLayoutGroup;

    [Header("Configurações para Tela Pequena (Celular)")]
    public Vector2 smallScreenCellSize = new Vector2(100, 100);
    public Vector2 smallScreenSpacing = new Vector2(10, 10);
    public int smallScreenPaddingLeft = 10;
    public int smallScreenPaddingRight = 10;
    public int smallScreenPaddingTop = 10;
    public int smallScreenPaddingBottom = 10;
    public Vector3 smallScreenScale = new Vector3(1f, 1f, 1f); 

    [Space(10)]
    [Header("Configurações para Tela Grande (Tablet)")]
    public Vector2 largeScreenCellSize = new Vector2(150, 150);
    public Vector2 largeScreenSpacing = new Vector2(20, 20);
    public int largeScreenPaddingLeft = 20;
    public int largeScreenPaddingRight = 20;
    public int largeScreenPaddingTop = 20;
    public int largeScreenPaddingBottom = 20;
    public Vector3 largeScreenScale = new Vector3(.8f, .8f, .8f); 

    void Start()
    {   
        AdjustGridLayoutAndScale();
    }

    void AdjustGridLayoutAndScale()
    {
        float screenAspectRatio = (float)Screen.height / Screen.width;
        Debug.Log("Proporção da tela: " + screenAspectRatio);

        if (screenAspectRatio > 1.6f) 
        {
            gridLayoutGroup.cellSize = smallScreenCellSize;
            gridLayoutGroup.spacing = smallScreenSpacing;
            gridLayoutGroup.padding = new RectOffset(smallScreenPaddingLeft, smallScreenPaddingRight, smallScreenPaddingTop, smallScreenPaddingBottom);
            AdjustElementScales(smallScreenScale);
            Debug.Log("Usando configurações para celular.");
        }
        else 
        {
            gridLayoutGroup.cellSize = largeScreenCellSize;
            gridLayoutGroup.spacing = largeScreenSpacing;
            gridLayoutGroup.padding = new RectOffset(largeScreenPaddingLeft, largeScreenPaddingRight, largeScreenPaddingTop, largeScreenPaddingBottom);
            AdjustElementScales(largeScreenScale);
            Debug.Log("Usando configurações para tablet.");
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content); 
    }

    void AdjustElementScales(Vector3 targetScale)
    {
      
        foreach (Transform child in content)
        {
            child.localScale = targetScale;
        }
    }

}

