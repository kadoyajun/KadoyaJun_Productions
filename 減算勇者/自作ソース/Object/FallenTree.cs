using UnityEngine;

public class FallenTree : MonoBehaviour
{
    [SerializeField]
    GameObject fallenTreeHitBox;
    private void Awake()
    {
        int angle = (int)transform.eulerAngles.y;
        switch (angle / 90)
        {
            case 0:
                {
                    Vector3 vector3 = transform.position;
                    GameObject hitBox0 = Instantiate(fallenTreeHitBox, new Vector3(vector3.x + 1, vector3.y, vector3.z), Quaternion.identity);
                    hitBox0.transform.parent = this.transform.parent;
                    GameObject hitBox1 = Instantiate(fallenTreeHitBox, new Vector3(vector3.x + 2, vector3.y, vector3.z), Quaternion.identity);
                    hitBox1.transform.parent = this.transform.parent;
                    break;
                }
            case 1:
                {
                    Vector3 vector3 = transform.position;
                    GameObject hitBox0 = Instantiate(fallenTreeHitBox, new Vector3(vector3.x, vector3.y, vector3.z - 1), Quaternion.identity);
                    hitBox0.transform.parent = this.transform.parent;
                    GameObject hitBox1 = Instantiate(fallenTreeHitBox, new Vector3(vector3.x, vector3.y, vector3.z - 2), Quaternion.identity);
                    hitBox1.transform.parent = this.transform.parent;
                    break;
                }
            case 2:
                {
                    Vector3 vector3 = transform.position;
                    GameObject hitBox0 = Instantiate(fallenTreeHitBox, new Vector3(vector3.x - 1, vector3.y, vector3.z), Quaternion.identity);
                    hitBox0.transform.parent = this.transform.parent;
                    GameObject hitBox1 = Instantiate(fallenTreeHitBox, new Vector3(vector3.x - 2, vector3.y, vector3.z), Quaternion.identity);
                    hitBox1.transform.parent = this.transform.parent;
                    break;
                }
            case 3:
                {
                    Vector3 vector3 = transform.position;
                    GameObject hitBox0 = Instantiate(fallenTreeHitBox, new Vector3(vector3.x, vector3.y, vector3.z + 1), Quaternion.identity);
                    hitBox0.transform.parent = this.transform.parent;
                    GameObject hitBox1 = Instantiate(fallenTreeHitBox, new Vector3(vector3.x, vector3.y, vector3.z + 2), Quaternion.identity);
                    hitBox1.transform.parent = this.transform.parent;
                    break;
                }
            default:
                break;
        }
    }
}
