

#include <Stepper.h>

const int stepsPerRevolution = 200;  // change this to fit the number of steps per revolution
// for your moto
String com;
String line1;
// initialize the stepper library on pins 8 through 11:
Stepper myStepper(stepsPerRevolution, 8, 9, 10, 11);

void setup() {
  // set the speed at 60 rpm:
  myStepper.setSpeed(400);
  // initialize the serial port:
  Serial.begin(9600);
  
  
    
}

void loop() {
  if(Serial.available()>0){
    com = Serial.readString();
  }
  line1=com.substring(0,3);
  if(line1=="far"){
  Serial.println("counterclockwise");
  myStepper.step(-stepsPerRevolution);
  //delay(500);
   }
   if(line1=="bac"){
  Serial.println("clockwise");
  myStepper.step(stepsPerRevolution);
 // delay(500);
   }
    if(line1=="stp"){
 
  //delay(500);
   }

    
    

}
