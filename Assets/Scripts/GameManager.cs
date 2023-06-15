using System;
using System.Threading.Tasks;
using UnityEngine;
using System.IO.Ports;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AquaMinderState State { get; private set; }

    public static event Action<AquaMinderState> OnAquaMinderStateChanged;

    private string arduinoPort;
    [SerializeField] private int arduinoBaudrate;

    private ArduinoCommunication arduinoInstance;

    public User CurrentUser { get; private set; }
    private User previousUser;
    private float humidity;
    private float temperature;
    private float weightArduino;
    private float weightBottleOn;

    private bool runtime = true;

    void Awake() => Instance = this;

    async void Start()
    {
        //Debug.Log("Get User");

        //CurrentUser = PlayerPrefsManager.GetUserByUid("ab8a90b9");
        //Debug.Log("CurrentUser: " + CurrentUser.name);

        //CurrentUser.drankWeight = 104;
        //PlayerPrefsManager.SetUser(CurrentUser);

        //var test = PlayerPrefsManager.GetUserByUid("ab8a90b9");
        //Debug.Log("CurrentUser: " + test.name +  " " + test.drankWeight);


        UpdateAquaMinderState(AquaMinderState.ONBOARDING);

        await InitializeArduinoCommunication();
        await UpdateArduinoSensorData();
    }

    private void Update()
    {
        // Only for development
        MenuManager.Instance.UpdateSystemInfoText(CurrentUser, temperature, humidity, weightArduino);
    }

    private void UpdateAquaMinderState(AquaMinderState newState)
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

    private async void HandleUserLogin()
    {
        Debug.Log("Handle USER_LOGIN");
        await Task.Delay(5000);

        UpdateAquaMinderState(AquaMinderState.BOTTLE_ON);
    }

    private async void HandleBottleOn()
    {
        Debug.Log("Handle BOTTLE_ON");


        await Task.Delay(2000);

        if (previousUser != null && CurrentUser != null)
        {
            if (previousUser.name == CurrentUser.name)
            {
                Debug.Log("Previous == Current");

                CurrentUser.drankWeight = weightBottleOn - weightArduino;
                PlayerPrefsManager.SetUser(CurrentUser);
            }
        }

        previousUser = CurrentUser;

        weightBottleOn = weightArduino;
    }

    private async void HandleBottleOff()
    {
        Debug.Log("Handle BOTTLE_OFF");

        await Task.Delay(5000);

        if (CurrentUser == null)
            UpdateAquaMinderState(AquaMinderState.ONBOARDING);
    }

    private async Task InitializeArduinoCommunication()
    {
        arduinoPort = SerialPort.GetPortNames().FirstOrDefault(s => s.Contains("usbmodem"));
        arduinoInstance = await Task.Run(() => ArduinoCommunication.GetInstance(arduinoPort, arduinoBaudrate));
    }

    private async Task UpdateArduinoSensorData()
    {
        while (runtime)
        {
            await UpdateAllData();
        }
    }

    private async Task UpdateAllData()
    {
        if (arduinoInstance == null)
            return;

        var allData = await Task.Run(() => arduinoInstance.ReceiveAllData());

        // User Stuff
        var userId = allData[0];
        CurrentUser = PlayerPrefsManager.GetUserByUid(userId);
        CheckAquaMinderUserChange();

        // Temperature and Humidity Stuff
        var tmpHum = allData[1].Split(";");
        temperature = float.Parse(tmpHum[0]);
        humidity = float.Parse(tmpHum[1]);

        // Weight Stuff
        weightArduino = float.Parse(allData[2]);
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