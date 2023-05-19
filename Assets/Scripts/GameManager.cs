using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject colorCycle;

    [SerializeField] private Material[] indicatorMaterials;

    // Start is called before the first frame update
    void Start()
    {
        colorCycle.GetComponent<Renderer>().material = indicatorMaterials[1];

        ArduinoCommunication.GetInstance().Recive(AquaMinderSensor.HUMIDITY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() 
    {
        ArduinoCommunication.GetInstance().CloseArduinoCommunication();
    }

    public enum AquaMinderState
    {
        A,
        B
    }
}
