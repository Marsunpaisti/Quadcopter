using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialReader : MonoBehaviour {
    public PropellerController propellerController;
    public OptionsScript options;
    private SerialPort stream;
    private string databuffer;
    private float start_updates_time = 2f;
    public Rigidbody Copter_Rigidbody;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    private void FixedUpdate() { 
        if (stream == null || !stream.IsOpen)
        {
            string portname = options.getSerialPortName();
            stream = new SerialPort(portname, 500000);
            stream.Open();
            return;
        }

        char c = '\0';
        stream.ReadTimeout = 1;

        //Try to read from serial
        try
        {
            while (true)
            {
                c = (char)stream.ReadChar();
                if (c != '\0' && c != '\n')
                {
                    databuffer = databuffer + c;
                }
                if (c == '\n') break;
            }
            

        } catch (System.TimeoutException)
        {
            c = '\0';
        }

        //Once a complete line has been formed, handle the received message
        if (c == '\n')
        {
            handleMessage(databuffer);
            databuffer = "";
        }

        StartCoroutine("postFixedUpdateAction");
    }

    IEnumerator postFixedUpdateAction()
    {
        if (!stream.IsOpen)
        {
            yield break;
        }

        //Sends copter data after physics updates have happened
        yield return new WaitForFixedUpdate();
        if (Time.time - start_updates_time > 0f)
        {
            sendCopterData();
            Debug.Log("Sent data update at " + Time.time);
        }
    }

    private void handleMessage(string message)
    {
        //Called after reading serial messages in the beginning of fixedupdate
        Debug.Log("Received serial: " + message + " at " + Time.time);
        float[] speeds = parsePropellerSpeeds(message);
        propellerController.setSerialPropellerSpeeds( speeds );


    }

    private float[] parsePropellerSpeeds(string message)
    {
        //Split serial message and parse 4 floats
        float[] speeds = new float[4];
        string[] parts = message.Split(' ');

        speeds[0] = float.Parse(parts[0]);
        speeds[1] = float.Parse(parts[1]);
        speeds[2] = float.Parse(parts[2]);
        speeds[3] = float.Parse(parts[3]);


        return speeds;
    }

    private void writeMessage(string message)
    {
        //Sends a mesasge over serial
        stream.WriteLine(message);
    }

    private string sendCopterData()
    {
        //Sends current attitude values over serial
        double pitch = Copter_Rigidbody.transform.eulerAngles.x;
        double yaw = Copter_Rigidbody.transform.eulerAngles.y;
        double roll = Copter_Rigidbody.transform.eulerAngles.z;
        double x = Copter_Rigidbody.transform.position.x;
        double y = Copter_Rigidbody.transform.position.y;
        double z = Copter_Rigidbody.transform.position.z;
        int fb = 0;
        int lr = 0;
        int ud = 0;
        int pedal = 0;

        if (Input.GetKey(KeyCode.Q)) pedal = -1;
        else if (Input.GetKey(KeyCode.E)) pedal = 1;
        else pedal = 0;

        if (Input.GetKey(KeyCode.W)) fb = 1;
        else if (Input.GetKey(KeyCode.S)) fb = -1;
        else fb = 0;

        if (Input.GetKey(KeyCode.A)) lr = -1;
        else if (Input.GetKey(KeyCode.D)) lr = 1;
        else lr = 0;

        if (Input.GetKey(KeyCode.Space)) ud = 1;
        else if (Input.GetKey(KeyCode.LeftControl)) ud = -1;
        else ud = 0;

        if (pitch > 180)
        {
            pitch = pitch - 360;
        }
        else if (pitch < -180)
        {
            pitch = pitch + 360;
        }

        if (yaw > 180)
        {
            yaw = yaw - 360;

        }
        else if (yaw < -180)
        {
            yaw = yaw + 360;
        }

        if (roll > 180)
        {
            roll = roll - 360;

        }
        else if (roll < -180)
        {
            roll = roll + 360;
        }

        pitch = System.Math.Round(pitch, 2);
        yaw = System.Math.Round(yaw, 2);
        roll = System.Math.Round(roll, 2);
        x = System.Math.Round(x, 2);
        y = System.Math.Round(y, 2);
        z = System.Math.Round(z, 2);


        writeMessage(pitch + " " + yaw + " " + roll + " " + x + " " + y + " " + z + " " + fb + " " + lr + " " + ud + " " + pedal);
        return pitch + " " + yaw + " " + roll + " " + x + " " + y + " " + z + " " + fb + " " + lr + " " + ud + " " + pedal;
    }
}
