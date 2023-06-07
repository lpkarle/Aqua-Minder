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

    public User CurrentUser { get; private set; }
    private User previousUser;
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

    private void Update()
    {
        

        // Only for development
        MenuManager.Instance.UpdateSystemInfoText(CurrentUser, temperature, humidity, drankWeight);
    }

    void UpdateAquaMinderState(AquaMinderState newState) 
    {
        State = newState;

        switch (newState)
        {
            case AquaMinderState.ONBOARDING:
                HandleOnboarding();
                break;

            case AquaMinderState.USER_LOGIN:
                HandleUserLogin();
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
        runtime = false;
        await CloseArduinoCommunication();
    }

    private void HandleOnboarding()
    {
        Debug.Log("Handle ONBOARDING");
    }

    async private void HandleUserLogin()
    {
        Debug.Log("Handle USER_LOGIN");
        await Task.Delay(5000);

        UpdateAquaMinderState(AquaMinderState.BOTTLE_ON);
    }

    private void HandleBottleOn()
    {
        Debug.Log("Handle BOTTLE_ON");

        previousUser = CurrentUser;
    }

    async private void HandleBottleOff()
    {
        Debug.Log("Handle BOTTLE_OFF");

        await Task.Delay(5000);

        if (CurrentUser == null)
            UpdateAquaMinderState(AquaMinderState.ONBOARDING);
    }

    private async Task InitializeArduinoCommunication()
    {
        arduinoInstance = await Task.Run(() => ArduinoCommunication.GetInstance(arduinoPort, arduinoBaudrate));
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

    private async Task UpdateArduinoUser()
    {
        if (arduinoInstance == null)
            return;

        var userId = await Task.Run(() => arduinoInstance.ReceiveUser());
        CurrentUser = PlayerPrefsManager.GetUserByUid(userId);

        CheckAquaMinderUserChange();
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

    private void CheckAquaMinderUserChange()
    {
        

        if (CurrentUser == null)
        {
            Debug.Log("Current User IS Null");

            if (State == AquaMinderState.BOTTLE_ON)
            {
                UpdateAquaMinderState(AquaMinderState.BOTTLE_OFF);
                return;
            }
        }
        else
        {
            Debug.Log("Current User IS_NOT Null");

            if (State == AquaMinderState.ONBOARDING)
            {
                UpdateAquaMinderState(AquaMinderState.USER_LOGIN);
                return;
            }

            if (State == AquaMinderState.BOTTLE_OFF && CurrentUser.uid == previousUser.uid)
            {
                UpdateAquaMinderState(AquaMinderState.BOTTLE_ON);
                return;
            }

            if (State == AquaMinderState.BOTTLE_OFF && CurrentUser.uid != previousUser.uid)
            {
                UpdateAquaMinderState(AquaMinderState.USER_LOGIN);
                return;
            }

        }
    }
}

public enum AquaMinderState
{
    ONBOARDING,
    USER_LOGIN,
    BOTTLE_ON,
    BOTTLE_OFF
}