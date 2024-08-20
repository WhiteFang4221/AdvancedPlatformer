using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VampireAbility))]
public class VampirismDetectionZone : MonoBehaviour
{
    [SerializeField] private List<Health> _detectedColliders = new List<Health>();

    public Action<Health> EnemyCatched;
    public Action<Health> EnemyLost;

    public IReadOnlyList<Health> DetectedColliders => _detectedColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Health enemy))
        {
            _detectedColliders.Add(enemy);
            EnemyCatched?.Invoke(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Health enemy))
        {
            _detectedColliders.Remove(enemy);
            EnemyLost?.Invoke(enemy);
        }
    }
}
