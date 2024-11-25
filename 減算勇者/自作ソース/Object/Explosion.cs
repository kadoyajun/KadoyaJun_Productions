using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [System.Obsolete]
    void Start()
    {
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Play();
        float duration = particleSystem.main.duration;
        ParticleSystem.Burst burst = particleSystem.emission.GetBurst(0);
        float interval = burst.repeatInterval;
        for (int i = 0; i < duration / interval; i++)
        {
            AudioManager.Instance.PlaySE("Explosion");
            yield return new WaitForSeconds(interval);
        }
        Destroy(gameObject);
    }
}
