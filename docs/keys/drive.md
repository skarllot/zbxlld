# Drive

The key `drive.discovery` returns a list of machine drives in JSON format.

## Filters

### drive.discovery.fixed

Returns fixed drives.

### drive.discovery.mounted

Returns mounted volumes both folder mounted and letter mounted.

_Since 0.4_

### drive.discovery.mountfolder

Returns folder mounted volumes.

_Since 0.4_

### drive.discovery.mountletter

Returns mounted volumes with assigned letter.

_Since 0.4_

### drive.discovery.network

Returns mounted network drives.

::: info NOTE
network drives are mounted per user, then when running as service no
mapped drives will be found.
:::

_Since 0.7_

### drive.discovery.nomount

Returns unmounted volumes.

_Since 0.4_

### drive.discovery.noswap

Returns only volumes without any system memory paging file.

_Since 0.3_

### drive.discovery.removable

Returns removable drives.

### drive.discovery.swap

Returns only volumes with system memory paging file.

_Since 0.3_

## Macros

### \{#FSCAPTION}

Filesystem display name.

_Expected values:_ System (C:), SWAP (D:) etc.

_Since 0.4_

### \{#FSFORMAT}

Filesystem format.

_Expected values:_ NTFS, FAT32 etc.

### \{#FSLABEL}

Filesystem label.

_Expected values:_ System, SWAP etc.

### \{#FSNAME}

Assigned letter.

_Expected values:_ C:\, D:\, C:\Boot\ etc.

### \{#FSPERFMON}

Drive instance name from Performance Monitor.

_Expected values:_ C:, D:, C:\Boot, HarddiskVolume5 etc.

_Since 0.5_

## Output Examples

**drive.discovery.fixed**

```json
{
    "data": [
        {
            "{#FSNAME}": "\\\\?\\Volume{5de618bf-acd9-11e2-992c-edd1e5856534}\\",
            "{#FSPERFMON}": "HarddiskVolume5",
            "{#FSLABEL}": "VSS System",
            "{#FSFORMAT}": "NTFS",
            "{#FSCAPTION}": "VSS System (\\\\?\\5de618)"
        },
        {
            "{#FSNAME}": "C:\\",
            "{#FSPERFMON}": "C:",
            "{#FSLABEL}": "System",
            "{#FSFORMAT}": "NTFS",
            "{#FSCAPTION}": "System (C:)"
        },
        {
            "{#FSNAME}": "C:\\Boot\\",
            "{#FSPERFMON}": "C:\\Boot",
            "{#FSLABEL}": "Boot",
            "{#FSFORMAT}": "NTFS",
            "{#FSCAPTION}": "Boot (C:\\Boot)"
        },
        {
            "{#FSNAME}": "Y:\\",
            "{#FSPERFMON}": "Y:",
            "{#FSLABEL}": "",
            "{#FSFORMAT}": "FAT32",
            "{#FSCAPTION}": "Y:"
        }
    ]
}
```

**drive.discovery.nomount**

```json
{
    "data": [
        {
            "{#FSNAME}": "\\\\?\\Volume{5de618bf-acd9-11e2-992c-edd1e5856534}\\",
            "{#FSPERFMON}": "HarddiskVolume5",
            "{#FSLABEL}": "VSS System",
            "{#FSFORMAT}": "NTFS",
            "{#FSCAPTION}": "VSS System (\\\\?\\5de618)"
        }
    ]
}
```
