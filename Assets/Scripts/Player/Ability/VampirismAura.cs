using System;
using System.Collections.Generic;
using UnityEngine;

public class VampirismAura : MonoBehaviour
{
    private static readonly List<IVampirable> vampirables = new List<IVampirable>();
    [SerializeField] private List<IVampirable> _detectedColliders = vampirables;
    
    public event Action<float> Catched;

    private float _damage = 10f;
    
    public IReadOnlyList<IVampirable> DetectedColliders => _detectedColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IVampirable vampirable))
        {
            _detectedColliders.Add(vampirable);

            if (_detectedColliders.Count <= 1)
            {
                vampirable.ReactToVampirism(_damage);
                Catched?.Invoke(_damage);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IVampirable vampirable))
        {
            _detectedColliders.Remove(vampirable);
            vampirable.GetOutVampirism();
        }
    }
}