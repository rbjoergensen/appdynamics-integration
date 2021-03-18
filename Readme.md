# Appdynamics Integration Test Project

Implementation of the Appdynamics agent in .Net Core 3.1 and .Net 5 projects running on the Ubuntu base image.  

## Running an MSSQL server in Docker for testing
``` bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu 
```

## Manual POST
``` Powershell
$Uri = "http://localhost:8080"

while ($true)
{
    Invoke-Webrequest -Uri "$Uri/api/v1/weathertype" -Method Post `
        -Body ('{"WeatherType":"' + $(-join ((65..90) + (97..122) | `
            Get-Random -Count 17 | % {[char]$_})) + '"}') -ContentType "application/json" | `
            select -ExpandProperty Content

    Start-Sleep -Milliseconds ((700..1500)| Get-Random)
}
```
