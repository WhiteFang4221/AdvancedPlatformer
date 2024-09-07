using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    private List<PlayerMoving> _detectedColliders = new List<PlayerMoving>();

    public IReadOnlyList<PlayerMoving> DetectedColliders => _detectedColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMoving player))
        {
            _detectedColliders.Add(player);
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMoving player))
        {
            _detectedColliders.Remove(player);
        }
    }
}