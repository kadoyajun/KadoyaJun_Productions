using UnityEngine;

public class CameraFish : MonoBehaviour
{
    //�Ǐ]����I�u�W�F�N�g
    [SerializeField]
    [Tooltip("�Ǐ]����I�u�W�F�N�g")]
    private GameObject fish = null;

    [SerializeField]
    [Tooltip("���̉�ʒ�������̃Y��")]
    private Vector3 cameraFish = Vector3.zero;

    float MAX = 17;
    float MIN = 7;
    void Update()
    {
        Vector3 cameraPos = fish.transform.position;

        cameraPos.x = Mathf.Clamp(cameraPos.x, 5, Mathf.Infinity);
        cameraPos.y = Mathf.Clamp(cameraPos.y, MIN, MAX);
        //cameraFish���������炷
        transform.position = new Vector3(cameraPos.x + cameraFish.x,cameraPos.y + cameraFish.y,-10);
    }
}
