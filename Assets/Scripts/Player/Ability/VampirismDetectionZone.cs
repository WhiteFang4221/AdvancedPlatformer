using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VampireAbility))]
public class VampirismDetectionZone : MonoBehaviour
{
    [SerializeField] private List<Collider2D> _detectedColliders = new List<Collider2D>();

    public Action<EnemyHealthManager> EnemyCatched;
    public Action<EnemyHealthManager> EnemyLost;

    public IReadOnlyList<Collider2D> DetectedColliders => _detectedColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyHealthManager enemy))
        {
            _detectedColliders.Add(collision);
            EnemyCatched?.Invoke(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyHealthManager enemy))
        {
            _detectedColliders.Remove(collision);
            EnemyLost?.Invoke(enemy);
        }
    }
}
