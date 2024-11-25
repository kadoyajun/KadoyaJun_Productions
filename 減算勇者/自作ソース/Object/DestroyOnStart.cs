using UnityEngine;

public class DestroyOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject);
    }
}
