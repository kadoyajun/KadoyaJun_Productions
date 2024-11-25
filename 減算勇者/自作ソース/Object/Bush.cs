using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField]
    Material[] materials;
    void Start()
    {
        int randomValue = UnityEngine.Random.Range(0, materials.Length);
        gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[randomValue];
    }
}
