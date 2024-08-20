using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0,5,0); 

    private void LateUpdate()
    {
        transform.position = _target.position + _offset;
    }
}
