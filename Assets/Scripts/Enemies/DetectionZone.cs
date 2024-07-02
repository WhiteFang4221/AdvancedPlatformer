using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    [SerializeField] private List<Collider2D> _detectedColliders = new List<Collider2D>();

    public List<Collider2D> DetectedColliders
    {
        get
        {
            return _detectedColliders;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _detectedColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedColliders.Remove(collision);
    }
}
