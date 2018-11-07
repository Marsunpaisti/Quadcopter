#include <AutoPID.h>
#include <string.h>

char serialBuf[150]; 
float pitch;
float yaw;
float roll;
float x;
float y;
float z;
float fb_steer;
float lr_steer;
float ud_steer;
float pedal_steer;

double pitch_dbl;
double yaw_dbl;
double roll_dbl;
double pitchOutput;
double yawOutput;
double rollOutput;
double pitchSetpt = 0;
double yawSetpt = 0;
double rollSetpt = 0;
AutoPID pitchPID = AutoPID(&pitch_dbl, &pitchSetpt, &pitchOutput, -1, 1, 0.007, 0, 1000);;
AutoPID yawPID = AutoPID(&yaw_dbl, &yawSetpt, &yawOutput, -1, 1, 0.05, 0, 5550);
AutoPID rollPID = AutoPID(&roll_dbl, &rollSetpt, &rollOutput, -1, 1, 0.007, 0, 1000);

double pid_steer_amount = 0.35;
double target_rpm_base = 0;

    
void sendPropellerSpeeds(){
    //Calculate propeller speeds from PID values and base rpm
    double prop1 = target_rpm_base - pid_steer_amount * pitchOutput - pid_steer_amount * rollOutput - pid_steer_amount * yawOutput;
    double prop2 = target_rpm_base - pid_steer_amount * pitchOutput + pid_steer_amount * rollOutput + pid_steer_amount * yawOutput;
    double prop3 = target_rpm_base + pid_steer_amount * pitchOutput + pid_steer_amount * rollOutput - pid_steer_amount * yawOutput;
    double prop4 = target_rpm_base + pid_steer_amount * pitchOutput - pid_steer_amount * rollOutput + pid_steer_amount * yawOutput;
    
    Serial.print(prop1, 3);
    Serial.print(" ");
    Serial.print(prop2, 3);
    Serial.print(" ");
    Serial.print(prop3, 3);
    Serial.print(" ");
    Serial.print(prop4, 3);

    Serial.print('\n');
}

void parseSerialMessage(){
  //Split serialBuf into tokens and parse
  char * i;
  char * token = strtok_r(serialBuf, " \n", &i);
  pitch = atof(token);
  token = strtok_r(NULL, " \n", &i);
  yaw = atof(token);
  token = strtok_r(NULL, " \n", &i);
  roll = atof(token);
  token = strtok_r(NULL, " \n", &i);
  x = atof(token);
  token = strtok_r(NULL, " \n", &i);
  y = atof(token);
  token = strtok_r(NULL, " \n", &i);
  z = atof(token);
  token = strtok_r(NULL, " \n", &i);
  fb_steer = atof(token);
  token = strtok_r(NULL, " \n", &i);
  lr_steer = atof(token);
  token = strtok_r(NULL, " \n", &i);
  ud_steer = atof(token);
  token = strtok_r(NULL, " \n", &i);
  pedal_steer = atof(token);

  //Empty Buffer
  serialBuf[0] = '\n';
}

void setup() {
  Serial.begin(500000);
  Serial.setTimeout(200);
  
  pitchPID.setTimeStep(9);
  yawPID.setTimeStep(9);
  rollPID.setTimeStep(9);

}

void loop() {
  //When update message is available, parse values
  while (Serial.available() > 0){
    int  r = Serial.readBytesUntil('\n', serialBuf, 150);
    if (r > 0){
      parseSerialMessage();
      onUpdateMessage();
    }
  }
  delay(1);
}

//onUpdateMessage is called every time a serial message is received and parsed and the variables are updated
//Put your controller code here
void onUpdateMessage(){
    //Update PID values
    pitch_dbl = static_cast<double>(pitch);
    yaw_dbl = static_cast<double>(yaw);
    roll_dbl = static_cast<double>(roll);

    //Adjust setpoint according to steering
    pitchSetpt = 8 * fb_steer;
    rollSetpt = -8 * lr_steer;
    
    pitchPID.run();
    yawPID.run();
    rollPID.run();

    //Adjust base target rpm
    target_rpm_base += ud_steer * 0.004;
    if (target_rpm_base > 0.85){
      target_rpm_base = 0.85;
    } else if (target_rpm_base < 0){
      target_rpm_base = 0;
    }

    
    sendPropellerSpeeds();
}
