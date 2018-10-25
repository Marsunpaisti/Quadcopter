using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDController {
    Transform BalanceTransform;
    private int mode = 0;
    private double P;
    private double I;
    private double D;
    public double min_output;
    public double max_output;
    public double Integrator_Buildup = 0;
    public double P_term = 0;
    public double D_term = 0;
    private double last_measurement;
    private double last_e;
    public double setPoint = 0;
    


    public PIDController(Transform BalanceTransform, double P, double I, double D, double minoutput, double maxoutput, int mode)
    {
        this.BalanceTransform = BalanceTransform;
        this.P = P;
        this.I = I;
        this.D = D;
        this.mode = mode;
        this.min_output = minoutput;
        this.max_output = maxoutput;
    }
    
    private double getCurrentValue()
    {
        double current_value = 0;

        if (mode == 1)
        {
            current_value = BalanceTransform.eulerAngles.x;
            if (current_value > 180)
            {
                current_value = current_value - 360;
            } else if (current_value < -180)
            {
                current_value = current_value + 360;
            }
        }
        else if (mode == 2)
        {
            current_value = BalanceTransform.eulerAngles.y;
            if (current_value > 180)
            {
                current_value = current_value - 360;
            }
            else if (current_value < -180)
            {
                current_value = current_value + 360;
            }
        }
        else if (mode == 3)
        {
            current_value = BalanceTransform.eulerAngles.z;
            if (current_value > 180)
            {
                current_value = current_value - 360;
            }
            else if (current_value < -180)
            {
                current_value = current_value + 360;
            }
        } else
        {
            Debug.Log("Invalid PID mode!");
            return 0.0D;
        }

        return current_value;
    }

    public void calculateFixedUpdate()
    {
        double current_value = getCurrentValue();
        double e = setPoint - current_value;

        //Fix jumps over angles 180 and -180
        if (e > 180)
        {
            e -= 360;
        } else if (e < -180)
        {
            e += 360;
        }

        //Integrate
        Integrator_Buildup += (last_e + e)/2 * Time.fixedDeltaTime * this.I;

        //Anti-Windup
        if (Integrator_Buildup > this.max_output)
        {
            Integrator_Buildup = this.max_output;
        } else if (Integrator_Buildup < this.min_output)
        {
            Integrator_Buildup = this.min_output;
        }

        //Derivative
        this.D_term = ((current_value - last_measurement) / Time.fixedDeltaTime) * this.D * (-1);
        //Ignore too big jumps
        if (Mathf.Abs((float)current_value - (float)last_measurement) > 45)
        {
            this.D_term = 0;
        }

        //Proportional
        this.P_term = e * this.P;

        last_e = e;
        last_measurement = current_value;
    }

    public double readOutput()
    {
        return (double)Mathf.Clamp((float)this.P_term + (float)this.D_term + (float)this.Integrator_Buildup, (float)this.min_output, (float)this.max_output);
    }
}
