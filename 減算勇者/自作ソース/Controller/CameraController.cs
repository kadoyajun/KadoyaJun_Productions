using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public void CameraPositionChange(int n)
    {
        if(n == 9)
        {
            transform.position = new Vector3(10, 0, 30);
        }
        else if(n == 10)
        {
            transform.position = new Vector3(10, 0, 40);
        }
        else
        {
            int a = n / 3;
            int b = n % 3;
            transform.position = new Vector3(a * 10, 0, b * 10);
        }
        
    }
}
