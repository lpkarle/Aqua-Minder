using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class LEDController : MonoBehaviour
{
    SerialPort sp = new SerialPort("/dev/cu.usbmodem21301", 9600);

    public void TurnOnLED()
    {
        Debug.Log("ON");
        sp.Open();
        sp.Write("A");
        sp.Close();
    }

    public void TurnOffLED()
    {
        Debug.Log("OFF");
        sp.Open();
        sp.Write("B");
        sp.Close();
    }
}