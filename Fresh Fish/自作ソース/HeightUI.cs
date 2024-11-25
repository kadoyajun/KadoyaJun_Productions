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
        //fish�̍��W��ǂ݁A���̈ʒu��ύX
        Vector2 vector2 = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(50, fish.transform.position.y * 10 + 30);
    }
}
