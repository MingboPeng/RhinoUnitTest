## Unit Test Runner

## --- Variables
param (
    # C:\files\test1.dll;C:\files\test2.dll
    [string[]]$dllPaths,
    [string]$RhinoVersion="7",
    [int]$timeout=30*60
)

$process = "C:\Program Files\Rhino $RhinoVersion\System\Rhino.exe"

$outputFiles = @()

$now = Get-Date

foreach($dllPath in $dllPaths)
{
    $now = [System.DateTime]::Now

    $debug = $dllPath.ToLower().Contains("debug")

    $aarrggss = "/nosplash /notemplate /runscript=`"NUnitTestRunnerCommand $dllPath+-exitonfinish`""
    $runner = Start-Process $process -ArgumentList $aarrggss -PassThru

    $filename = [System.IO.Path]::GetFileNameWithoutExtension($dllPath);
    $filenamePlusExtens = [System.IO.Path]::GetFileName($dllPath);
    
    $dlldir = [System.IO.Path]::GetDirectoryName($dllPath);
    $outputPath = [System.IO.Path]::Combine($dlldir, "$fileName`_integrated_out.xml");

    if (Test-Path $outputPath)
    {
        Remove-Item -Path $outputPath
        if (!(Test-path $outputPath))
        {
            Write-Host "Removed old xml file"
        }
        else
        {
            Write-Host "Old xml not removed. This may cause errors."
        }
    }

    $timeoutCount = 0
    $flag = $true
    while($flag)
    {
        Start-Sleep -Seconds 1
        $timeoutCount += 1
    
        if ($timeoutCount % 10 -eq 0)
        {
            Write-Host "Timeout is $timeoutCount/$timeout"
        }

        if ($timeoutCount -ge $timeout)
        {
            Write-Host $Process Timed Out!
            $flag = $false
            break;
        }
        if ($runner.HasExited)
        {
            Write-Host "Rhino was closed somehow"
            break;
        }
    
        if (Test-Path $outputPath)
        {
            $lastWrite = [datetime]((Get-ChildItem $outputPath).LastWriteTime)
            if ($lastWrite -ge $now)
            {
                if ($debug)
                {
                    Read-host
                }

                try { Stop-Process $runner.Id -Force } catch { }

                $flag = $false
                break;
            }
        }
    }

    if (Test-Path $outputPath)
    {
        Write-Host "Finished test suite $filenamePlusExtens, results can be found at $outputPath"
        $outputFiles += $outputPath

        $log_dir = "$env:APPDATA\nunittestrunner"
        $log = Get-ChildItem "$log_dir\*.log" | Where-Object LastWriteTime -gt $now
        
        if ($null -ne $log)
        {
            $log_data = Get-Content $log
            foreach($line in $log_data)
            {
                Write-Host $line
            }
        }
    }
    else
    {
        Write-Host "Test suite ended unexpectidly. No output file created."   
    }
}
    
Write-Host "Finished All Test Suites!"
return $outputFiles