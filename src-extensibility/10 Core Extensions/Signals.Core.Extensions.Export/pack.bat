
REM !important
SET Version=0.6.27


SET NugetUrl=https://pkgs.dev.azure.com/emitknowledge/_packaging/Signals/nuget/v3/index.json
SET ApiKey=VSTS

for %%i in (bin\Release\*.nupkg) do del "%%i"
dotnet msbuild /t:pack /p:Configuration=Release /p:Version=%Version% /p:PackageVersion=%Version%
for %%i in (bin\Release\*.nupkg) do nuget push -Source %NugetUrl% -ApiKey %ApiKey% "%%i"
exit