using System.IO;
using System.IO.Ports;
using UnityEngine;


public class ArduinoCommunication
{
    private static ArduinoCommunication Instance { get; }

    private SerialPort serialPort;
    private readonly string port = "/dev/cu.usbmodem21301";
    private readonly int baudrate = 115200;

    public ArduinoCommunication()
    {
        EstablishArduinoConnectionAndWaitForInitialization();
    }

    public static ArduinoCommunication GetInstance()
    {
        return Instance ?? new ArduinoCommunication();
    }

    public string ReceiveUser()
    {
        Debug.Log("Arduino User Request");

        return Recive(AquaMinderSensor.USER);
    }

    public float[] ReceiveTemperatureAndHumidity()
    {
        Debug.Log("Arduino Humidity Request");

        string[] temperatureHumidity = Recive(AquaMinderSensor.HUMIDITY).Split(';');

        var temperature = float.Parse(temperatureHumidity[0]);
        var humidity = float.Parse(temperatureHumidity[1]);

        return new float[] { temperature, humidity };
    }

    public float ReceiveDrankWeight()
    {
        Debug.Log("Arduino Weight Request");

        return float.Parse(Recive(AquaMinderSensor.WEIGHT));
    }

    public void CloseArduinoCommunication()
    {
        Debug.Log("Close Arduino Communication");

        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }

    private void EstablishArduinoConnectionAndWaitForInitialization()
    {
        try
        {
            Debug.Log($"Try to establish serial connection with Arduino on {port} with baudrate {baudrate}");
            
            serialPort = new SerialPort(port, baudrate);
            serialPort.Open();

            string response = "";
            while (!response.Contains("Ready"))
            {
                response = serialPort.ReadLine().Trim();
                Debug.Log(response);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Can not open a serial connection with the given connection data.\n" + e);
        }
    }

    private string Recive(AquaMinderSensor sensor)
    {
        Debug.Log($"Arduino Receive {sensor} Sensor.");

        if (serialPort == null)
            return "";

        serialPort.Write(((int)sensor).ToString());

        var response = serialPort.ReadLine().Trim();
        var responseJson = JsonUtility.FromJson<ArduinoJsonData>(response);

        return sensor switch
        {
            AquaMinderSensor.USER => responseJson.uid,
            AquaMinderSensor.HUMIDITY => $"{responseJson.temperature};{responseJson.humidity}",
            AquaMinderSensor.WEIGHT => responseJson.drankWeight,
            _ => "",
        };
    }

    private class ArduinoJsonData
    {
        public string uid, humidity, temperature, drankWeight;
    }
}

public enum AquaMinderSensor
{
    USER = 0,
    HUMIDITY = 1,
    WEIGHT = 2
}


