using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextManager : MonoBehaviour {
    public Rigidbody Copter_Rigidbody;
    public Text TextField;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        double pitch = Copter_Rigidbody.transform.eulerAngles.x;
        double yaw = Copter_Rigidbody.transform.eulerAngles.y;
        double roll = Copter_Rigidbody.transform.eulerAngles.z;
        double x = Copter_Rigidbody.transform.position.x;
        double y = Copter_Rigidbody.transform.position.y;
        double z = Copter_Rigidbody.transform.position.z;

        if (pitch > 180)
        {
            pitch = pitch - 360;
        } else if (pitch < -180)
        {
            pitch = pitch + 360;
        }

        if (yaw > 180)
        {
            yaw = yaw - 360;

        } else if (yaw < -180)
        {
            yaw = yaw + 360;
        }

        if (roll > 180)
        {
            roll = roll - 360;

        } else if (roll < -180)
        {
            roll = roll + 360;
        }

        pitch = System.Math.Round(pitch, 2);
        yaw = System.Math.Round(yaw, 2);
        roll = System.Math.Round(roll, 2);
        x = System.Math.Round(x, 2);
        y = System.Math.Round(y, 2);
        z = System.Math.Round(z, 2);

        TextField.text = "Pitch: ";
        TextField.text += pitch;
        TextField.text += "\nYaw: ";
        TextField.text += yaw;
        TextField.text += "\nRoll: ";
        TextField.text += roll;
        TextField.text += "\nX: ";
        TextField.text += x;
        TextField.text += "\nY: ";
        TextField.text += y;
        TextField.text += "\nZ: ";
        TextField.text += z;
	}
}
