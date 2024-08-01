using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VampirismBar : MonoBehaviour
{
    [SerializeField] private VampireAbility _vampireAbility;
    [SerializeField] private Image _fillArea;
    private Slider _slider;


    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        _vampireAbility.AbilityActivated += ChangeBar;
    }

    private void OnDisable()
    {
        _vampireAbility.AbilityActivated -= ChangeBar;
    }

    private void ChangeBar(float maxValue)
    {
        _slider.maxValue = maxValue;
        StartCoroutine(ChangeHealthBarState());
    }

    private IEnumerator ChangeHealthBarState()
    {
        while (_vampireAbility.IsCanUseAbility == false)
        {
            _slider.value = _vampireAbility.TimeLeft;
            yield return null;
        }
    }
}
