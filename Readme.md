# Appdynamics Integration Test Project

## Running an MSSQL server in Docker for testing
``` bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu 
```

## Manual POST
``` Powershell
Invoke-Webrequest -Uri "http://localhost:8080/api/v1/weathertype" -Method Post -Body '{"WeatherType":"Moist"}' -ContentType "application/json"
```