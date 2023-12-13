using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorScreen : MonoBehaviour
{
    [SerializeField] private GameObject indicatorObject;
    public void SetActive(bool active)
    {
        ActivateIndicator(false);
        gameObject.SetActive(active);
    }

    public void ActivateIndicator(bool active) => indicatorObject.gameObject.SetActive(active);
}
