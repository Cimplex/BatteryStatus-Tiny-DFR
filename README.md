# BatteryStatus-Tiny-DFR
This is a simple dotnet AOT program that will check your battery status and update tiny-dfr to show a "dynamic-ish" icon"

<a href="https://t.co/CkxPfWYqdu">pic.twitter.com/CkxPfWYqdu</a>

## Installing
See <a href="https://github.com/Cimplex/BatteryStatus-Tiny-DFR/releases">Releases</a> for details on how to install pre-built binaries.

## Configure Tiny-DFR

Edit your `/usr/share/tiny-dfr/config.toml` to include the following:

`    { Icon = "battery_status",  Action = "Battery"        }`

## Building

Dependencies: `sudo dnf install dotnet-sdk-9.0 clang gcc gcc-c++ make cmake`
  
Publish AoT (M-Series): `dotnet publish -c Release -r linux-arm64`

Publish AoT (Intel): `dotnet publish -c Release -r linux-x64`

## Running

Requires elevated privileges to restart tiny-dfr:

#### (Without AOT)

`sudo dotnet run --output=/usr/share/tiny-dfr/battery_status.svg --restart-tiny-dfr`

#### (With AOT)

```
sudo ./bin/Release/net9.0/linux-arm64/publish/BatteryStatus --output=/usr/share/tiny-dfr/battery_status.svg --restart-tiny-dfr
```

## Additional Notes

If you want to move BatteryStatus executable somewhere else, make sure the `./Icons/` folder gets moved with it. It needs to be in the working directory of the program.

BatteryStatus-Tiny-DFR has a MIT License. It is NOT affiliated with tiny-dfr in anyway. Google Matrerial Icons are licensed under Apache License Version 2.0. 
