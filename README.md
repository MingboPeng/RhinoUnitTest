# RhinoUnitTest
Demo shows how do set up NUnit test for Rhino
![Image of RhinoUnitTest Command](https://raw.githubusercontent.com/MingboPeng/RhinoUnitTest/master/RhinoUnitTest.gif)

## How to start
This repo is setting up with following structure:
* RhinoPlugin (this is a regular Rhino plugin)
* RhinoPlugin.Test (NUnit tests for testing RhinoPlugin)
* NUnitTestRunner (Writtern as RhinoPlugin, for loading RhinoPlugin.Test into Rhino, and run its tests)

### Things to ensure this works
1. Override RhinoPlugin LoadTime, because Rhino will only load this plugin when it is needed. 
```csharp
public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;
```

2. Edit RhinoPlugin.Test.csproj, add ```<Prefer32Bit>false</Prefer32Bit>``` to ensure the NUnit test runs under x64.
```xml
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <Prefer32Bit>false</Prefer32Bit>
  </PropertyGroup>

  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <Prefer32Bit>false</Prefer32Bit>
```

3. Set NUnitTestRunner as StartUp project in Visual Studio.
Once Rhino started, drag the ```NUnitTestRunner.rhp``` file under ``NUnitTestRunner\bin`` folder to Rhino to load the NUnitTestRunner as plugin.

4. Type in `NUnitTestRunnerCommand` to start debugging.

