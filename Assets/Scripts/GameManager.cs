using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AquaMinderState State { get; private set; }

    public static event Action<AquaMinderState> OnAquaMinderStateChanged;

    private ArduinoCommunication arduinoInstance;

    void Awake() => Instance = this;

    async void Start()
    {
        await InitializeArduinoCommunication();

        UpdateAquaMinderState(AquaMinderState.ONBOARDING);
    }

    void UpdateAquaMinderState(AquaMinderState newState) 
    {
        State = newState;

        switch (newState)
        {
            case AquaMinderState.ONBOARDING:
                HandleOnboarding();
                break;

            case AquaMinderState.BOTTLE_ON:
                HandleBottleOn();
                break;

            case AquaMinderState.BOTTLE_OFF:
                HandleBottleOff();
                break;
        }

        OnAquaMinderStateChanged?.Invoke(newState);

        Debug.Log($"New Aqua-Minder state: {newState}.");
    }

    async void OnDestroy()
    {
        await CloseArduinoCommunication();
    }

    private void HandleOnboarding()
    {
        // TODO
    }

    private void HandleBottleOn()
    {
        // TODO
    }

    private void HandleBottleOff()
    {
        // TODO
    }

    private async Task InitializeArduinoCommunication()
    {
        arduinoInstance = await Task.Run(() => ArduinoCommunication.GetInstance());
    }

    private async Task ReceiveArduinoUser()
    {
        if (arduinoInstance != null)
            await Task.Run(() => arduinoInstance.ReceiveUser());
    }

    private async Task ReceiveArduinoTemperatureAndHumidity()
    {
        if (arduinoInstance != null)
            await Task.Run(() => arduinoInstance.ReceiveTemperatureAndHumidity());
    }

    private async Task ReceiveArduinoWeight()
    {
        if (arduinoInstance != null)
            await Task.Run(() => arduinoInstance.ReceiveDrankWeight());
    }

    private async Task CloseArduinoCommunication()
    {
        if (arduinoInstance != null)
            await Task.Run(() => arduinoInstance.CloseArduinoCommunication());
    }
}
public enum AquaMinderState
{
    ONBOARDING,
    BOTTLE_ON,
    BOTTLE_OFF
}