# Network

Returns a list of network interfaces in JSON format. Returns all but Loopback and Tunnel interfaces.

## Filters

Don't have.

## Macros

### \{#IFADDR}

Network physical address, return only numbers.

### \{#IFDESC}

Interface driver name.

### \{#IFNAME}

User assigned name to network interface.

## Output Example

```json
{
    "data": [
        {
            "{#IFDESC}": "Realtek RTL8139C+ Fast Ethernet NIC",
            "{#IFNAME}": "LAN0",
            "{#IFADDR}": "525400648347"
        }
    ]
}
```
