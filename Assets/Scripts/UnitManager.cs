using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    [SerializeField]
    private GameObject colorIndicatorCycle;

    [SerializeField]
    private Material[] indicatorMaterials;

    private void Awake() => Instance = this;

    void Start()
    {
        colorIndicatorCycle.GetComponent<Renderer>().material = indicatorMaterials[1];
    }


    void Update()
    {
            
    }

    public void UpdateColorIndicator()
    {
        // TODO
    }

    private void ScaleColorIndicatorUpAndDown()
    {

    }
}
