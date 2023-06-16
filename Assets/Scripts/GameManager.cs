using System;
using System.Collections;
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

    private Task task1;
    private Task task2;
    private Task task3;

    void Awake() => Instance = this;

    async void Start()
    {
        Debug.Log("Starting AquaMinder");
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
            
            case AquaMinderState.WAIT_BOTTLE:
                HandleWaitBottle();
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

    private async void OnDestroy()
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
        await Task.Delay(1000);
        UpdateAquaMinderState(AquaMinderState.WAIT_BOTTLE);
    }
    
    private async void HandleWaitBottle()
    {
        Debug.Log("Handle WAIT_BOTTLE");
        
        await Task.Delay(5000);

        // Check if bottle is on the scale, if not, log out user
        if (weightArduino > 200)
        {
            Debug.Log("Detected Bottle");
            UpdateAquaMinderState(AquaMinderState.BOTTLE_ON);
        } else
        {
            Debug.Log("Detected NO Bottle, logging out user, going to ONBOARDING");
            CurrentUser = null;
            UpdateAquaMinderState(AquaMinderState.ONBOARDING);
        }
    }
    
    private async void HandleBottleOn()
    {
        Debug.Log("Handle BOTTLE_ON");
        
        await Task.Delay(2000);
        
        if (CurrentUser != null)
        {
            var weightDiff = weightArduino - CurrentUser.mostRecentRawWeight;
            
            // If weight is smaller than 200, user removed bottle
            if (weightArduino < 200) 
            {
                Debug.Log("User removed bottle");
                // User removed bottle
                UpdateAquaMinderState(AquaMinderState.BOTTLE_OFF);
            }
            
            // Diff weight has to be smaller than negative 10 to be considered as a drink
            if (weightDiff < -10)
            {
                // User drank
                var absWeightDiff = Mathf.Abs(weightDiff);
                Debug.Log("User drank: " + absWeightDiff);
                CurrentUser.drankWeight += absWeightDiff;
                CurrentUser.mostRecentRawWeight = weightArduino;
                PlayerPrefsManager.SetUser(CurrentUser);
            }
            // Diff weight has to be bigger than 10 to be considered as a refill
            else
            if (weightDiff > 10)
            {
                Debug.Log("User refilled: " + weightDiff);
                CurrentUser.mostRecentRawWeight = weightArduino;
                PlayerPrefsManager.SetUser(CurrentUser);
            }
        }
    }

    private async void HandleBottleOff()
    {
        Debug.Log("Handle BOTTLE_OFF");
        
        // Wait before reminding user (1st stage)
        Debug.Log("Wait before reminding user");
        await Task.Run(() => Wait1());
        
        // Wait before logging out user (2nd stage)
        Debug.Log("Wait before logging out user");
        await Task.Run(() => Wait2());

        if (State == AquaMinderState.BOTTLE_OFF)
        {
            // Log out user (3rd and final stage)
            Debug.Log("Logging out user, going to ONBOARDING");
            CurrentUser = null;
            UpdateAquaMinderState(AquaMinderState.ONBOARDING);
        }
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
            // Debug.Log("Current User IS Null");

            if (State == AquaMinderState.BOTTLE_ON)
            {
                UpdateAquaMinderState(AquaMinderState.BOTTLE_OFF);
                return;
            }
        }
        else
        {
            // Debug.Log("Current User IS_NOT Null");

            if (State == AquaMinderState.ONBOARDING)
            {
                UpdateAquaMinderState(AquaMinderState.USER_LOGIN);
                return;
            }
            
            if (previousUser != null && State == AquaMinderState.BOTTLE_OFF && CurrentUser.uid == previousUser.uid)
            {
                UpdateAquaMinderState(AquaMinderState.BOTTLE_ON);
                return;
            }

            if (previousUser != null && State == AquaMinderState.BOTTLE_OFF && CurrentUser.uid != previousUser.uid)
            {
                UpdateAquaMinderState(AquaMinderState.USER_LOGIN);
                return;
            }
            
            if (previousUser == null && CurrentUser.uid != null && State == AquaMinderState.BOTTLE_OFF)
            {
                UpdateAquaMinderState(AquaMinderState.USER_LOGIN);
                return;
            }
        }
    }
    
    private void Wait1()
    {
        var trigger = false;
        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < 10 && State == AquaMinderState.BOTTLE_OFF)
        {
            if (!trigger)
            {
                // Do something here
                Debug.Log("Wait 1 executed");
                trigger = true;
            }
        }
    }

    
    private void Wait2()
    {
        var trigger = false;
        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < 10 && State == AquaMinderState.BOTTLE_OFF)
        {
            if (!trigger)
            {
                // Do something here
                Debug.Log("Wait 2 executed");
                trigger = true;
            }
        }
    }
}




public enum AquaMinderState
{
    ONBOARDING,
    USER_LOGIN,
    WAIT_BOTTLE,
    BOTTLE_ON,
    BOTTLE_OFF
}