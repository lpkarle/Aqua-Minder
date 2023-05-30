using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AquaMinderState State { get; private set; }

    public static event Action<AquaMinderState> OnAquaMinderStateChanged;

    [SerializeField] private string arduinoPort;
    [SerializeField] private int arduinoBaudrate;

    private ArduinoCommunication arduinoInstance;

    private User user;
    private float humidity;
    private float temperature;
    private float drankWeight;

    private bool arduinoRequestIsLocked = false;

    void Awake() => Instance = this;

    async void Start()
    {
        await InitializeArduinoCommunication();

        UpdateAquaMinderState(AquaMinderState.ONBOARDING);
    }

    async void Update()
    {
        if (!arduinoRequestIsLocked)
            await ReceiveArduinoUser();

        if (!arduinoRequestIsLocked)
            await ReceiveArduinoTemperatureAndHumidity();

        if (!arduinoRequestIsLocked)
            await ReceiveArduinoWeight();
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

        Debug.Log($"----------- New Aqua-Minder state: {newState}. ----------");
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
        arduinoInstance = await Task.Run(() => ArduinoCommunication.GetInstance(arduinoPort, arduinoBaudrate));
    }

    private async Task ReceiveArduinoUser()
    {
        arduinoRequestIsLocked = true;

        if (arduinoInstance != null)
            await Task.Run(() => arduinoInstance.ReceiveUser());

        await Delay(2000);

        arduinoRequestIsLocked = false;
    }

    private async Task ReceiveArduinoTemperatureAndHumidity()
    {
        arduinoRequestIsLocked = true;

        if (arduinoInstance != null)
            await Task.Run(() => arduinoInstance.ReceiveTemperatureAndHumidity());

        await Delay(2000);

        arduinoRequestIsLocked = false;
    }

    private async Task ReceiveArduinoWeight()
    {
        arduinoRequestIsLocked = true;

        if (arduinoInstance != null)
            drankWeight = await Task.Run(() => arduinoInstance.ReceiveDrankWeight());

        await Delay(2000);

        arduinoRequestIsLocked = false;
    }

    private async Task CloseArduinoCommunication()
    {
        if (arduinoInstance != null)
            await Task.Run(() => arduinoInstance.CloseArduinoCommunication());
    }

    private async Task Delay(int ms) => await Task.Delay(ms);
}

public enum AquaMinderState
{
    ONBOARDING,
    BOTTLE_ON,
    BOTTLE_OFF
}