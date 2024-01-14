# Service

Returns a list of machine services in JSON format.

## Filters

### service.discovery.any

Returns all services.

_Since 0.2_

### service.discovery.auto

Returns only automatically started services.

_Since 0.2_

### service.discovery.demand

Returns only manually started services.

_Since 0.2_

### service.discovery.disabled

Returns only disabled services.

_Since 0.2_

## Macros

### \{#SVCDESC}

Service displayed name.

### \{#SVCNAME}

Service internal name.

### \{#SVCSTARTTYPE}

Shows when the service should be started.

_Valid values:_ Auto, Demand and Disabled.

### \{#SVCSTATUS}

Service status

_Valid values:_ ContinuePending, Paused, PausePending, Running, StartPending, Stopped and StopPending.

## Output Example

**service.discovery.disabled**

```json
{
    "data": [
        {
            "{#SVCNAME}": "clr_optimization_v2.0.50727_32",
            "{#SVCDESC}": "Microsoft .NET Framework NGEN v2.0.50727_X86",
            "{#SVCSTATUS}": "Stopped",
            "{#SVCSTARTTYPE}": "Disabled"
        },
        {
            "{#SVCNAME}": "Mcx2Svc",
            "{#SVCDESC}": "Serviço do Media Center Extender",
            "{#SVCSTATUS}": "Stopped",
            "{#SVCSTARTTYPE}": "Disabled"
        },
        {
            "{#SVCNAME}": "MSSQLServerADHelper100",
            "{#SVCDESC}": "SQL Active Directory Helper Service",
            "{#SVCSTATUS}": "Stopped",
            "{#SVCSTARTTYPE}": "Disabled"
        },
        {
            "{#SVCNAME}": "SharedAccess",
            "{#SVCDESC}": "ICS (Compartilhamento de Conexão com a Internet)",
            "{#SVCSTATUS}": "Stopped",
            "{#SVCSTARTTYPE}": "Disabled"
        },
        {
            "{#SVCNAME}": "SQLAgent$SQLEXPRESS",
            "{#SVCDESC}": "SQL Server Agent (SQLEXPRESS)",
            "{#SVCSTATUS}": "Stopped",
            "{#SVCSTARTTYPE}": "Disabled"
        },
        {
            "{#SVCNAME}": "SQLBrowser",
            "{#SVCDESC}": "SQL Server Browser",
            "{#SVCSTATUS}": "Stopped",
            "{#SVCSTARTTYPE}": "Disabled"
        }
    ]
}
```
