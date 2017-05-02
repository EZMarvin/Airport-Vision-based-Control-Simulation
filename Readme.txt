Youtube Link: https://youtu.be/khAgGfqcwUI

File sepcification:
	matlab - contains the function file and workspace which store the data of background (1280 * 600)
	pipeclient - contains the C# solution which runs as client
	pipeserver - contains the unity3d package of the project and some of the important script and code
	Slide - demonstration files

Environment:
	windows with .NET Framework 3.5 or higer edition
	unity3d
	Matlab

Usage instruction:
	detail instruction see 'project report' in 'slides' folder
	1. install unity3d , create new project and import unity3d package in the pipeserver. Set the screen size to 1280*600 in Game tab(OR run exe file in the 'build' folder)
	2. install any IDE (visual studio) which able to run pipeclient C# solution
	3. install Matlab OR get Matlab COM object reference
	4. run unity3d first, then run client program
	result :Get process result from  Matlab COM; Get distance result from C# program console; In unity3D the UAV will stop before hit the truck.

Two camera version:
	change the corresponding file in the pipeclient folder. reload unity package
	Process two camera image and add more scenario, 
	UAV will stop at first time with yellow truck, an back when get close to blue bus.
	In unity: Use Start/Pause to control simulation, and Bus to make blue bus move and UAV will continue moving.  

Slides:
	Presentation slides
	Project report
