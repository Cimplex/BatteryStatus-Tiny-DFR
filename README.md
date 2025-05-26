# BatteryStatus-Tiny-DFR
This is a simple dotnet AOT program that will check your battery status and update tiny-dfr to show a "dynamic-ish" icon"

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
cd ./bin/Release/net9.0/linux-arm64/publish/
sudo ./BatteryStatus --output=/usr/share/tiny-dfr/battery_status.svg --restart-tiny-dfr
```
