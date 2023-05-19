using System;
using System.IO.Ports;
using UnityEngine;


public class ArduinoCommunication
{
    private static ArduinoCommunication Instance;

    private SerialPort SerialPort;
    private string Port = "/dev/cu.usbmodem21301";
    private int Baudrate = 115200;

    public ArduinoCommunication()
    {
        EstablishArduinoConnection();
    }

    // Public static method to get the singleton instance
    public static ArduinoCommunication GetInstance()
    {
        return Instance == null ? new ArduinoCommunication() : Instance;
    }

    private void EstablishArduinoConnection()
    {
        Debug.Log($"Establish serial connection with Arduino on {Port} with baudrate {115200}");

        SerialPort = new SerialPort(Port, Baudrate);
        SerialPort.Open();

        // Wait for Arduino to initialize
        string response = "";
        while (!response.Contains("Ready"))
        {
            response = SerialPort.ReadLine().Trim();
            Debug.Log(response);
        }
    }


    public void Recive(AquaMinderSensor sensor)
    {
        // TODO
    }

    public void CloseArduinoCommunication()
    {
        Debug.Log("Close Arduino Communication");

        if (SerialPort != null && SerialPort.IsOpen)
            SerialPort.Close();
    }
}

public enum AquaMinderSensor
{
    USER,
    HUMIDITY,
    WEIGHT
}
