using HealthSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VampirismAura : MonoBehaviour
{
    [SerializeField] private List<EnemyHealth> _detectedColliders = new List<EnemyHealth>();

    public Action<EnemyHealth> EnemyCatched;
    public Action<EnemyHealth> EnemyLost;

    public IReadOnlyList<EnemyHealth> DetectedColliders => _detectedColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyHealth enemy))
        {
            _detectedColliders.Add(enemy);
            EnemyCatched?.Invoke(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyHealth enemy))
        {
            _detectedColliders.Remove(enemy);
            EnemyLost?.Invoke(enemy);
        }
    }
}
