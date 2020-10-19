
REM !important
SET Version=1.0.20


SET NugetUrl=https://api.nuget.org/v3/index.json
SET ApiKey=oy2jt7aifor352qeokrqnhpptc6yvngedjgsbrasawmyzy

for %%i in (bin\Release\*.nupkg) do del "%%i"
dotnet msbuild /t:restore /p:Configuration=Release
dotnet msbuild /t:pack /p:Configuration=Release /p:Version=%Version% /p:PackageVersion=%Version%
for %%i in (bin\Release\*.nupkg) do nuget push -Source %NugetUrl% -ApiKey %ApiKey% "%%i"
exit