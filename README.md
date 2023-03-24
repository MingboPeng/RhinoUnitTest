# NUnitTestRunner

The NUnitTestRunner allows for running of NUnit test assemblies inside of Rhino from a command. Because if this, Rhino can be spun up, have tests run, results output, and rhino closed, perfect for a self-hosted CI/CD Machine. The NUnitTestRunner can run headed or headlessly

### How to use the NUnitTestRunner headed
![Image of RhinoUnitTest Command](https://raw.githubusercontent.com/MingboPeng/RhinoUnitTest/master/RhinoUnitTest.gif)



### How to use the NUnitTestRunner headlessly

Here is a simple bit of powershell that can launch the NUnitTestRunner headlessly

``` powershell
$base_dir = $PSScriptRoot
$dllPath = "$base_dir\bin\x64\Debug\net48\Integration.Tests.dll"

$process = "C:\Program Files\Rhino $RhinoVersion\System\Rhino.exe"

$args = "/nosplash /notemplate /runscript=`"-NUnitTestRunnerCommand $dllPath+--ExitOnFinish`""
$runner = Start-Process $process -ArgumentList $args -PassThru
```



# Command Syntax

Using the `NUnitTestRunner` Command and typing `--help` will give you all the available options inside of the runner.
