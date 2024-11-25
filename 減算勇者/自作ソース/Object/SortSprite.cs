using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortSprite : MonoBehaviour
{
    [SerializeField]
    Script_SpriteStudio6_RootUnityNative sS6;

    void Update()
    {
        int x = (int)transform.position.x % 10 + 1;
        int y = 10 - (int)transform.position.y % 10 ;
        sS6.SortingOrder = x * y * 10;
    }
}
