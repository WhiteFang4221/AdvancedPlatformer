using Bars;
using UnityEngine;


public class HealthProvider : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _provider;
    [SerializeField] private HealthBar _bar;

    private void Start()
    {
        if (_provider is IHealthProvider healthProvider)
        {
            _bar.Init(healthProvider.Health);
        }
        else
        {
            Debug.Log("не нследует IHealthProvider");
        }
    }
}
