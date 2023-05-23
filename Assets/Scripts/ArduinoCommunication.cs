using System;
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


    public void Recive(AquaMinderSensor sensor)
    {
        // TODO
    }

    public void CloseArduinoCommunication()
    {
        Debug.Log("Close Arduino Communication");

        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}

public enum AquaMinderSensor
{
    USER,
    HUMIDITY,
    WEIGHT
}
