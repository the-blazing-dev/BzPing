@echo off

:: this will result in a ~15MB file
dotnet publish BzPingNetCore -c Release -r win-x64 --self-contained ^
    /p:DebugType=None /p:DebugSymbols=false ^
    -p:PublishSingleFile=true -p:PublishTrimmed=true ^
    -o ./publish/netcore-selfcontained

dotnet msbuild BzPingNetFramework /t:Build /p:Configuration=Release /p:OutDir=..\publish\netframework\ ^
    /p:DebugType=None /p:DebugSymbols=false