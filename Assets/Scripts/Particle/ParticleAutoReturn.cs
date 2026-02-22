using UnityEngine;
using System.Collections;

public class ParticleAutoReturn : MonoBehaviour
{
    private ParticleSystem[] pts;

    private void Awake()
    {
        pts = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        pts = GetComponentsInChildren<ParticleSystem>();
        float maxDuration = 0f;
        foreach (var p in pts)
        {
            if (p == null) continue;
            p.Play();
            var main = p.main;
            float duration = main.duration + main.startLifetime.constantMax;
            if (duration > maxDuration) maxDuration = duration;
        }
        StartCoroutine(ReturnToPool(1f));
    }

    private IEnumerator ReturnToPool(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
