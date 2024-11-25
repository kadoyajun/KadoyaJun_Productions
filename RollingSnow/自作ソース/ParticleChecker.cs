using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChecker : MonoBehaviour
{
    [SerializeField]
    private int particleNo;
    [SerializeField]
    private ParticleController particleController;
    

    public void returnNo()
    {
        particleController.particleChangeActive(particleNo);
        //ParticleController particleController = GetComponent<ParticleController>();
    }
}
