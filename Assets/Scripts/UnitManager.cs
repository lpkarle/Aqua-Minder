using System;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    [SerializeField]
    private GameObject colorIndicatorCycle;

    [SerializeField] private Material sunnyTopf;
    private Color sunnyTopfDefaultColor = new(0.9f, 1.0f, 1.0f);
    
    [SerializeField] private Material[] indicatorMaterials;

    private void Awake() => Instance = this;

    private void Start() => sunnyTopf.color = sunnyTopfDefaultColor;

    public void ActivateColorCycle() => colorIndicatorCycle.SetActive(true);
    
    public void DeactivateColorCycle() => colorIndicatorCycle.SetActive(false);

    public void UpdateSunnyColor(Color color) => sunnyTopf.color = color;
    
    public void ResetSunnyColor() => sunnyTopf.color = sunnyTopfDefaultColor;
}
