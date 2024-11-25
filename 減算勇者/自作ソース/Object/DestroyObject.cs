using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField]
    GameObject particle;

    public void DeleteThisObject(bool explosion)
    {
        if (explosion)
        {
            Instantiate(particle, transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
}
