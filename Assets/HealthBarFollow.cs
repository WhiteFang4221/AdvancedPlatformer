using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0,1,0); 

    void LateUpdate()
    {
        transform.position = _target.position + _offset;
    }
}
