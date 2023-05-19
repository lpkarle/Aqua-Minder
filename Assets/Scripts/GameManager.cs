using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject colorCycle;

    [SerializeField] private Material[] indicatorMaterials;

    private ArduinoCommunication ArduinoInstance;

    async void Start()
    {
        colorCycle.GetComponent<Renderer>().material = indicatorMaterials[1];

        await InitializeArduinoCommunication();
    }

    void Update()
    {
    }

    async void OnDestroy() 
    {
        await CloseArduinoCommunication();
    }

    private async Task InitializeArduinoCommunication()
    {
        ArduinoInstance = await Task.Run(() => ArduinoCommunication.GetInstance());
    }

    private async Task ReceiveArduinoCommunication()
    {
        if (ArduinoInstance != null)
            await Task.Run(() => ArduinoInstance.Recive(AquaMinderSensor.HUMIDITY));
    }

    private async Task CloseArduinoCommunication()
    {
        if (ArduinoInstance != null)
            await Task.Run(() => ArduinoInstance.CloseArduinoCommunication());
    }

    public enum AquaMinderState
    {
        A,
        B
    }
}
