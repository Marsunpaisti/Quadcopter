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

void sendValuesOverSerial(){
    Serial.print("P: ");
    Serial.print(pitch, 2);
    Serial.print(" Y: ");
    Serial.print(yaw, 2);
    Serial.print(" R: ");
    Serial.print(roll, 2);
    Serial.print(" X: ");
    Serial.print(x, 2);
    Serial.print(" Y: ");
    Serial.print(y, 2);
    Serial.print(" Z: ");
    Serial.print(z, 2);
    Serial.print(" fb: ");
    Serial.print(fb_steer);
    Serial.print(" lr: ");
    Serial.print(lr_steer);
    Serial.print(" ud: ");
    Serial.print(ud_steer);
    Serial.print(" pedal: ");
    Serial.print(pedal_steer);
    Serial.print('\n');
}

void sendPropellerSpeeds(){
    if (ud_steer == 1){
      Serial.print("0.7 0.7 0.7 0.7");
    } else {
      Serial.print("0 0 0 0");
    }
    
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
    //sendValuesOverSerial();
    sendPropellerSpeeds();
}
