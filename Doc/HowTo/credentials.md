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

## `appsettings.Development.json` in /Code/UITManagerApi
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


## `registredUsers.json` in /Code/UITManagerApi
```json
[
  {
    "Name": "uitmanager",
    "Serial": "StrongerPassword!1"
  }
]
```

## `.env` in /Code/UITManagerAlarmManager Mail
```bash
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=UITMANAGER SYSTEM
SMTP_PASSWORD=sgpq vvbv gcpn lpac
SMTP_FROM_EMAIL=musica.proytb@gmail.com

```