
REM !important
SET Version=2.0.0


SET NugetUrl=https://api.nuget.org/v3/index.json
SET ApiKey=oy2bnwwctebzihdhnmlq6mzft3q4fgbenfwxqzoo346dcy

for %%i in (bin\Release\*.nupkg) do del "%%i"
dotnet msbuild /t:restore /p:Configuration=Release
dotnet msbuild /t:pack /p:Configuration=Release /p:Version=%Version% /p:PackageVersion=%Version%
for %%i in (bin\Release\*.nupkg) do nuget push -Source %NugetUrl% -ApiKey %ApiKey% "%%i"
exit
exit