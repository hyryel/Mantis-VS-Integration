# Mantis-VS-Integration
Integrate Mantis Bug Tracker into VS 2015+

## First things to do
* Make sure to compile the project right after cloning. This allow NuGet to download and install required packages.

## Configure project to test extension
### Common steps
* Go to the properties of the project "VSMantisConnect"
* Got to the "Debug" Tab
* Check "Start external progam"
### Visual Studio 2015
* Be sure to be on the branch "/Features/VS_2015"
* Select your VS 2015 devenv.exe (ex: C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe)
* Add the followinf command line arguments : /RootSuffix Exp
### Visual Studio 2017
* Be sure to be on the branch "/Features/VS_2017"
* Select your VS 2017 devenv.exe (ex: C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\devenv.exe)
* Add the followinf command line arguments : /RootSuffix Exp
