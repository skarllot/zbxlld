# ZBXLLD

_ZBXLLD provides additional discovery features to Zabbix agent._

[![Build status](https://github.com/skarllot/zbxlld/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/skarllot/zbxlld/actions)
[![GitHub Release](https://img.shields.io/github/v/release/skarllot/zbxlld)](https://github.com/skarllot/zbxlld/releases)
[![GitHub license](https://img.shields.io/badge/license-GPL%203.0-blue.svg?style=flat)](https://raw.githubusercontent.com/skarllot/zbxlld/master/LICENSE)

<hr />

## Documentation
For deeper detail of available features, be sure also to check out [Documentation Page](https://fgodoy.me/zbxlld/).

## Installation

- Compile or download from latest [release](https://github.com/skarllot/zbxlld/releases).

- Copy "zbxlld-win.exe" to Zabbix agent directory (eg: C:\Zabbix\bin).

- Add the following line to zabbix_agentd.conf:
```ini
UserParameter=zbxlld[*],C:\Zabbix\bin\zbxlld-win.exe $1 $2 $3
```

- Restart the Zabbix agent if is already started.

- Read [documentation](https://fgodoy.me/zbxlld/) for further instructions.


## Contributing

If something is not working for you or if you think that the source file
should change, feel free to create an issue or Pull Request.
I will be happy to discuss and potentially integrate your ideas!

## License

This library is licensed under the [GPL 3.0 License](./LICENSE).
