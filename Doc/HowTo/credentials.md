# UITMANAGER
## `.env` in /Database folder
```
postgres_user=develop
postgres_password=hdksldjufiad
postgres_database=uitmanager
```
## Webapp Login
- Technician
  - username: `devauxisaac`
  - password: `StrongerPassword!1`
- Maintenance Manager
  - username: `milletcamille`
  - password: `StrongerPassword!1`
- IT Director
  - username: `oroger`
  - password: `StrongerPassword!1`

## `appsettings.Development.json` in /code/UITManagerApi
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey" : "LeScHaUsSeTtEdElArChIdUcHeSsEsOnTeLlEsEcHeArChIsEcHe",
    "Issuer" : "http://localhost:5014/",
    "Audience" : "http://localhost:5014/",
    "ExpiryMinutes" : 60
  },
  "AllowedHosts": "*"
}
```


## `registredUsers.json` in /code/UITManagerApi
```json
[
  {
    "Name": "oroger",
    "Serial": "StrongerPassword!1"
  },
  {
    "Name": "devauxisaac",
    "Serial": "StrongerPassword!1"
  },
  {
    "Name": "milletcamille",
    "Serial": "StrongerPassword!1"
  }
]
```