using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller
{
    public GameObject gameObj;
    public Rigidbody forceBody;
    private int target_rpm;
    private int current_rpm;
    private int max_rpm;
    private int rpm_acceleration;
    private float thrust_multiplier;
    private float torque_multiplier;
    private bool reverse_direction;


    public Propeller(GameObject gameObject, Rigidbody forceBody, int maxrpm, int rpmacceleration, float thrustmultiplier, float torquemultiplier, bool reverse_direction)
    {
        this.gameObj = gameObject;
        this.forceBody = forceBody; 
        this.target_rpm = 0;
        this.current_rpm = 0;
        this.max_rpm = maxrpm;
        this.rpm_acceleration = rpmacceleration;
        this.thrust_multiplier = thrustmultiplier;
        this.torque_multiplier = torquemultiplier;
        this.reverse_direction = reverse_direction;
    }



    public void fixedUpdateRPM()
    {
        if (current_rpm != target_rpm)
        {
            int difference = target_rpm - current_rpm;
            if (Mathf.Abs(difference) <= rpm_acceleration * Time.fixedDeltaTime)
            {
                current_rpm = target_rpm;
            } else
            {
                if (difference < 0)
                {
                    current_rpm -= (int)(Time.fixedDeltaTime * rpm_acceleration);
                } else
                {
                    current_rpm += (int)(Time.fixedDeltaTime * rpm_acceleration);
                }
            }
        }
    }

    public void fixedApplyThrust()
    {
        float thrust = this.current_rpm * this.current_rpm * thrust_multiplier/ 10000000;
        Rigidbody rb = this.forceBody;
        Vector3 relativeUp = this.gameObj.transform.up;
        rb.AddForceAtPosition(relativeUp * thrust * Time.fixedDeltaTime, this.gameObj.transform.position);
    }

    public void drawGizmos()
    {
        Vector3 relativeUp = this.gameObj.transform.up;
        Color col = Color.green;
        DrawHelperAtCenter(relativeUp, this.gameObj.transform.position, col, 2f);
    }

    private void DrawHelperAtCenter( Vector3 direction, Vector3 start, Color color, float scale)
    {
        Gizmos.color = color;
        Vector3 destination = start + direction * scale;
        Gizmos.DrawLine(start, destination);
    }

    public void fixedApplyTorque()
    {
        float torque = this.current_rpm * this.current_rpm * torque_multiplier / 10000000;

        if (!this.reverse_direction)
        {
            torque = -torque;
        }

        Rigidbody rb = this.forceBody;
        rb.AddRelativeTorque(new Vector3(0, torque * Time.fixedDeltaTime, 0));
    }

    public void updateColor()
    {
        Color propellerColor = new Color(0.2f + 0.8f * ((float)current_rpm / (float)max_rpm), 0, 0);
        this.gameObj.GetComponent<Renderer>().material.SetColor("_Color", propellerColor);
    }

    public void setTargetRPM(int targetRPM)
    {
        this.target_rpm = Mathf.Clamp(targetRPM, 0, this.max_rpm);
    }

    public void setTargetRPMRelative(float percentage)
    {
        int targetRPM = (int)(Mathf.Clamp01(percentage) * this.max_rpm);
        this.target_rpm = Mathf.Clamp(targetRPM, 0, this.max_rpm);
    }

    public int getRPM()
    {
        return current_rpm;
    }

    public int getTargetRPM()
    {
        return target_rpm;
    }
}

public class PropellerController : MonoBehaviour {
    public GameObject Propeller1;
    public GameObject Propeller2;
    public GameObject Propeller3;
    public GameObject Propeller4;
    public Rigidbody Copter_Rigidbody;
    private Propeller[] propellers;

    public int max_rpm;
    public int rpm_acceleration;
    public float thrust_multiplier;
    public float torque_multiplier;

    public PIDController pitch_pid;
    public PIDController yaw_pid;
    public PIDController roll_pid;

    public double target_rpm_spacebar;

    private OptionsScript options;
    public float[] serial_PropellerSpeeds = new float[4];

    public void setSerialPropellerSpeeds(float[] speeds)
    {
        this.serial_PropellerSpeeds = speeds;
    }

    void Start () {
        options = Object.FindObjectOfType<OptionsScript>();

        //Initialize propeller array
        propellers = new Propeller[4];
        propellers[0] = new Propeller(Propeller1, Copter_Rigidbody, max_rpm, rpm_acceleration, thrust_multiplier, torque_multiplier, false);
        propellers[1] = new Propeller(Propeller2, Copter_Rigidbody, max_rpm, rpm_acceleration, thrust_multiplier, torque_multiplier, true);
        propellers[2] = new Propeller(Propeller3, Copter_Rigidbody, max_rpm, rpm_acceleration, thrust_multiplier, torque_multiplier, false);
        propellers[3] = new Propeller(Propeller4, Copter_Rigidbody, max_rpm, rpm_acceleration, thrust_multiplier, torque_multiplier, true);

        //Initialize built in PIDs
        this.pitch_pid = new PIDController(Copter_Rigidbody.transform, 0.016, 0, 0.002, -1, 1, 1);
        this.yaw_pid = new PIDController(Copter_Rigidbody.transform, 0.12, 0, 0.02, -1, 1, 2);
        this.roll_pid = new PIDController(Copter_Rigidbody.transform, 0.016, 0, 0.002, -1, 1, 3);
	}
	
	// Update is called once per frame
	void Update () {

        //Thrust
        if (Input.GetKey(KeyCode.Space)){
            target_rpm_spacebar += 0.3 * Time.deltaTime;
            target_rpm_spacebar = Mathf.Clamp((float)target_rpm_spacebar, 0, 0.85f);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            target_rpm_spacebar -= 0.3 * Time.deltaTime;
            target_rpm_spacebar = Mathf.Clamp((float)target_rpm_spacebar, 0, 0.85f);
        }

        //Steering
        if (Mathf.Abs(Input.GetAxis("Vertical")) >= 0.15)
        {
            pitch_pid.setPoint = 15 * Input.GetAxis("Vertical");
        } else
        {
            pitch_pid.setPoint = 0;
        }

        if (Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.15)
        {
            roll_pid.setPoint = -15 * Input.GetAxis("Horizontal");
        } else
        {
            roll_pid.setPoint = 0;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            yaw_pid.setPoint += -30 * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.E))
        {
            yaw_pid.setPoint += 30 * Time.deltaTime;
        }
        if (yaw_pid.setPoint > 180)
        {
            yaw_pid.setPoint -= 360;
        } else if (yaw_pid.setPoint < -180)
        {
            yaw_pid.setPoint += 360;
        }


        //Update propeller colors
        foreach (Propeller prop in propellers)
        {
            prop.updateColor();
        }



        //Debug.Log("Pitch P: " + System.Math.Round(pitch_pid.P_term, 3) + " I: " + System.Math.Round(pitch_pid.Integrator_Buildup, 3) + " D: " + System.Math.Round(pitch_pid.D_term, 3) + " Total: " + System.Math.Round(pitch_pid.readOutput(), 2));
        //Debug.Log("Yaw P: " + System.Math.Round(yaw_pid.P_term, 3) + " I: " + System.Math.Round(yaw_pid.Integrator_Buildup, 3) + " D: " + System.Math.Round(yaw_pid.D_term, 3) + " Total: " + System.Math.Round(yaw_pid.readOutput(), 2));
        //Debug.Log("Roll P: " + System.Math.Round(roll_pid.P_term, 3) + " I: " + System.Math.Round(roll_pid.Integrator_Buildup, 3) + " D: " + System.Math.Round(roll_pid.D_term, 3) + " Roll: " + System.Math.Round(roll_pid.readOutput(), 2));
    }

    private void FixedUpdate()
    {
        if (options.enableInternalPID)
        {
            //Calculate PIDS
            pitch_pid.calculateFixedUpdate();
            yaw_pid.calculateFixedUpdate();
            roll_pid.calculateFixedUpdate();

            //Add PID steering to propeller target RPMS
            float pid_steer_amount = 0.35f;
            propellers[0].setTargetRPMRelative((float)target_rpm_spacebar - pid_steer_amount * (float)pitch_pid.readOutput() - pid_steer_amount * (float)roll_pid.readOutput() - pid_steer_amount * (float)yaw_pid.readOutput());
            propellers[1].setTargetRPMRelative((float)target_rpm_spacebar - pid_steer_amount * (float)pitch_pid.readOutput() + pid_steer_amount * (float)roll_pid.readOutput() + pid_steer_amount * (float)yaw_pid.readOutput());
            propellers[2].setTargetRPMRelative((float)target_rpm_spacebar + pid_steer_amount * (float)pitch_pid.readOutput() + pid_steer_amount * (float)roll_pid.readOutput() - pid_steer_amount * (float)yaw_pid.readOutput());
            propellers[3].setTargetRPMRelative((float)target_rpm_spacebar + pid_steer_amount * (float)pitch_pid.readOutput() - pid_steer_amount * (float)roll_pid.readOutput() + pid_steer_amount * (float)yaw_pid.readOutput());
        } else
        {
            //Propeller speeds received from serialreader
            propellers[0].setTargetRPMRelative( serial_PropellerSpeeds[0] );
            propellers[1].setTargetRPMRelative( serial_PropellerSpeeds[1] );
            propellers[2].setTargetRPMRelative( serial_PropellerSpeeds[2] );
            propellers[3].setTargetRPMRelative( serial_PropellerSpeeds[3] );
        }

        //Run fixed update functions for propellers
        foreach (Propeller prop in propellers)
        {
            prop.fixedUpdateRPM();
            prop.fixedApplyThrust();
            prop.fixedApplyTorque();
            //Debug.Log(prop.getRPM() + " / " + prop.getTargetRPM());
        }
    }

}
