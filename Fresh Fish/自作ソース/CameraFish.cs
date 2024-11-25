using UnityEngine;

public class CameraFish : MonoBehaviour
{
    //追従するオブジェクト
    [SerializeField]
    [Tooltip("追従するオブジェクト")]
    private GameObject fish = null;

    [SerializeField]
    [Tooltip("魚の画面中央からのズレ")]
    private Vector3 cameraFish = Vector3.zero;

    float MAX = 17;
    float MIN = 7;
    void Update()
    {
        Vector3 cameraPos = fish.transform.position;

        cameraPos.x = Mathf.Clamp(cameraPos.x, 5, Mathf.Infinity);
        cameraPos.y = Mathf.Clamp(cameraPos.y, MIN, MAX);
        //cameraFish分だけずらす
        transform.position = new Vector3(cameraPos.x + cameraFish.x,cameraPos.y + cameraFish.y,-10);
    }
}
