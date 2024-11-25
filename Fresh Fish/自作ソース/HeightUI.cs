using UnityEngine;

public class HeightUI : MonoBehaviour
{
    [SerializeField]
    private GameObject fish = null;

    RectTransform rectTransform;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        //fishの座標を読み、矢印の位置を変更
        Vector2 vector2 = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(50, fish.transform.position.y * 10 + 30);
    }
}
