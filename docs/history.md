# History

## v0.7 - 2014-04-02 [⬇️](https://github.com/skarllot/zbxlld/releases/download/v0.7/zbxlld-win.exe)
- Improved debugging and error handling.
- Added helper message to show when a invalid key is requested.
- Improved drive discovery to detect permission issues.
- Added new filter to drive discovery to get network drives.
- Some fixes and improvements when drive discovery is done with native code.
- Some code improvements.

## v0.6.1 - 2014-02-07 [⬇️](https://github.com/skarllot/zbxlld/releases/download/v0.6.1/zbxlld-win.exe)
- Fixed exception when tries to read PerfMon duplicated keys ([Thanks to aib](https://www.zabbix.com/forum/showpost.php?p=144541&postcount=17)).

## v0.6 - 2013-05-29 [⬇️](http://goo.gl/CpZhhY)
- New feature offers macro suffix to allow duplicated discovery.

## v0.5.1 - 2013-05-02 [⬇️](http://goo.gl/TXyQ3)
- Fixed compatibility to Windows XP.

## v0.5 - 2013-05-02 [⬇️](http://goo.gl/SSKN0)
- Added {#FSPERFMON} macro that returns Performance Monitor instance name.
- Modified {#FSCAPTION} to return 6 characters from GUID instead of 4.

## v0.4 - 2013-04-28 [⬇️](http://goo.gl/r6LW5)
- Tidied "drive.discovery" filters to meaningful names.
- Now fixed volumes are divided into three categories: folder mounted, letter mounted and unmounted.
- Fixed {#FSNAME} to return full original name, no replaces or trims.
- Added {#FSCAPTION} macro that returns friendly display name.
- Fixed JSON output to not print comma after last item.
- Fixed JSON output to escape all backslashes and double quotation marks.
- Fixed exception to print null volume label.

## v0.3 - 2013-04-26 [⬇️](http://goo.gl/uBe45)
- Option "drive.discovery.fixed" now returns volumes without letters too (folder mounted volumes).
- New option to return folder mounted volumes.
- New option to return volumes with assigned letter.
- New option to return volumes with page file.
- New option to return volumes without page file.

## v0.2 - 2013-01-04 [⬇️](http://goo.gl/EzDyw)
- New service discovery feature.

## v0.1 - 2012-12-21 [⬇️](http://goo.gl/tZFfS)
- Allow multiple discovery to the same key.
- Discovery keys returns only useful data.
- Discovery returns drive label.
- Discovery returns network interface friendly name.
- It's GPL 3.
