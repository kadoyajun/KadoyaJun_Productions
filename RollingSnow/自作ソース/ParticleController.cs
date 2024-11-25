using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField]
    private int stageNo;

    [SerializeField]
    private Transform mainCamera = null;

    [SerializeField]
    private GameObject[] particle = null;

    private int cameraPos;
    void Start()
    {

    }

    void Update()
    {
        if(stageNo == 1)
        {
            cameraPos = Mathf.FloorToInt(mainCamera.transform.localPosition.x / 100);
            for (int i = 0; i < particle.Length; i++)
            {
                if (cameraPos - 1 == i || cameraPos == i || cameraPos + 1 == i)
                {
                    particle[i].SetActive(true);
                }
                else
                {
                    particle[i].SetActive(false);
                }
            }
        }
        

        //particle[number].transform.position = new Vector3(cameraPos * 100, 25, 0);
        //transform.position = new Vector3(cameraPos * 100,transform.position.y,transform.position.z);

    }

    public void particleChangeActive(int n)
    {
        if (n == 21)
        {
            for (int i = 0; i < particle.Length; i++)
            {
                if (i == 20||i == 21||i == 22||i == 26)
                {
                    particle[i].SetActive(true);
                }
                else
                {
                    particle[i].SetActive(false);
                }
            }
        }
        else if (n == 25)
        {
            for (int i = 0; i < particle.Length; i++)
            {
                if (i == 24 || i == 25 || i == 30)
                {
                    particle[i].SetActive(true);
                }
                else
                {
                    particle[i].SetActive(false);
                }
            }
        }
        else if (n == 26)
        {
            for (int i = 0; i < particle.Length; i++)
            {
                if (i == 21 || i == 26 || i == 27)
                {
                    particle[i].SetActive(true);
                }
                else
                {
                    particle[i].SetActive(false);
                }
            }
        }
        else if (n == 30)
        {
            for (int i = 0; i < particle.Length; i++)
            {
                if (i == 25 || i == 29 || i == 30||i == 31)
                {
                    particle[i].SetActive(true);
                }
                else
                {
                    particle[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < 36; i++)
            {
                if (n - 1 == i || n == i || n + 1 == i)
                {
                    particle[i].SetActive(true);
                }
                else
                {
                    particle[i].SetActive(false);
                }
            }
        }
        if(n == 0)
        {
            particle[36].SetActive(false);
            particle[37].SetActive(false);
            particle[38].SetActive(false);
            particle[39].SetActive(false);
        }

        if(n == 10 || n == 16)
        {
            particle[36].SetActive(true);
            particle[37].SetActive(true);
        }
        if(n == 17 || n == 18)
        {
            particle[36].SetActive(false);
            particle[37].SetActive(false);
            particle[38].SetActive(true);
        }
        if(n == 19)
        {
            particle[38].SetActive(false);
        }
        if (n == 30 || n == 31)
        {
            particle[39].SetActive(true);
        }
        if (n == 32)
        {
            particle[39].SetActive(false);
        }

    }

    
}
