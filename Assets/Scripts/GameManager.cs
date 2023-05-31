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

    private bool runtime = true;

    void Awake() => Instance = this;

    async void Start()
    {
        UpdateAquaMinderState(AquaMinderState.ONBOARDING);

        await InitializeArduinoCommunication();
        await UpdateArduinoSensorData();
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

    private async Task UpdateArduinoSensorData()
    {
        while (runtime)
        {
            await UpdateArduinoUser();
            await UpdateArduinoWeight();
            await UpdateArduinoTemperatureAndHumidity();
        }
    }

    private async Task InitializeArduinoCommunication()
    {
        arduinoInstance = await Task.Run(() => ArduinoCommunication.GetInstance(arduinoPort, arduinoBaudrate));
    }

    private async Task UpdateArduinoUser()
    {
        if (arduinoInstance == null)
            return;

        var userId = await Task.Run(() => arduinoInstance.ReceiveUser());
        user = PlayerPrefsManager.GetUserByUid(userId);

        if (user != null)
            UpdateAquaMinderState(AquaMinderState.BOTTLE_ON);
    }

    private async Task UpdateArduinoWeight()
    {
        if (arduinoInstance == null)
            return;

        drankWeight = await Task.Run(() => arduinoInstance.ReceiveDrankWeight());
    }

    private async Task UpdateArduinoTemperatureAndHumidity()
    {
        if (arduinoInstance == null)
            return;

        var tmpHum = await Task.Run(() => arduinoInstance.ReceiveTemperatureAndHumidity());
        temperature = tmpHum[0];
        humidity = tmpHum[1];
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