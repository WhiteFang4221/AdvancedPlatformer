using System;
using System.Collections.Generic;
using UnityEngine;

public class VampirismAura : MonoBehaviour
{
    private List<IVampirable> _detectedColliders = new List<IVampirable>();
    private float _damage = 10f;
    
    public event Action<float> Catched;

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