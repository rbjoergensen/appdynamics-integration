# Appdynamics Integration Test Project

Implementation of the Appdynamics agent in .Net Core 3.1 and .Net 5 projects running on the Debian base image.  

## Running the project

You need an AppDynamics instance running to forward the collected data to.  
The services will still run without AppDynamics.  

Update the following variables in docker-compose.yml.
- APPDYNAMICS_CONTROLLER_HOST_NAME
- APPDYNAMICS_AGENT_ACCOUNT_NAME
- APPDYNAMICS_AGENT_ACCOUNT_ACCESS_KEY
- APPDYNAMICS_CONTROLLER_SSL_CERTFILE (Only if you are using a private CA)

In the project root run the docker-compose command with the build parameter.  
``` bash
docker compose up -d --build
```

| Service  | Port  | Description  |
|:--------:|:-----:|:------------:|
| Backend  | 9080  | HTTP         |
| Frontend | 80    | HTTP         |
| SQL      | 1443  | MSSQL        |

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

## Running a MSSQL server in Docker for testing
``` bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu 
```
