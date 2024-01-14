# Getting Started

Welcome to the ZBXLLD documentation! Join our [Discussions space](https://github.com/skarllot/zbxlld/discussions) on GitHub to exchange ideas with the community.

## What is ZBXLLD

The ZBXLLD command-line provides additional discovery features to Zabbix agent.

## Installation

1. Compile or download from latest [release](https://github.com/skarllot/zbxlld/releases).

2. Copy "zbxlld-win.exe" to Zabbix agent directory (eg: C:\Zabbix\bin).

3. Add the following line to zabbix_agentd.conf:
    ```ini
    UserParameter=zbxlld[*],C:\Zabbix\bin\zbxlld-win.exe $1 $2 $3
    ```

4. Restart the Zabbix agent if is already started.

## Usage

The _first parameter_ is the requested key (eg: drive.discovery.fixed), the _second parameter_ is the macro suffix and the _third parameter_ is ignored.

Use the second parameter to duplicate discoveries, eg:
- `zbxlld[drive.discovery.fixed,-s _SYSTEM]` (eg: {#FSCAPTION_SYSTEM})
- `zbxlld[drive.discovery.fixed,-s _SWAP]` (eg: {#FSCAPTION_SWAP})
- `zbxlld[drive.discovery.fixed,-s NULL]` (eg: {#FSCAPTION})
- `zbxlld[drive.discovery.fixed]` (eg: {#FSCAPTION})
