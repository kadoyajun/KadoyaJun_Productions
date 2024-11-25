using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroudScroll : MonoBehaviour
{
    [SerializeField]
    private Transform[] frontSprites = null;

    [SerializeField]
    private GameObject back = null;

    [SerializeField]
    private GameObject fish = null;

    Vector3 unitPerGrid;

    void Start()
    {
        unitPerGrid = frontSprites[0].GetComponent<SpriteRenderer>().bounds.size;

        UpdateSprites();
    }

    // すべてのパネルの位置を更新します。
    private void UpdateSprites()
    {
        // カメラの位置
        var cameraGridX = Mathf.FloorToInt(Camera.main.transform.position.x / unitPerGrid.x);

        for (int index = 0; index < frontSprites.Length; index++)
        {
            var position = frontSprites[index].position;
            position.x = (index - 1 + cameraGridX) * unitPerGrid.x;
            frontSprites[index].position = position;
        }

        if(fish.transform.position.x > 5)
        {
            Vector3 backPos = new Vector3(fish.transform.position.x, transform.position.y, transform.position.z);
            back.transform.position = backPos;
        }
    }

    void Update()
    {
        UpdateSprites();
    }
}
