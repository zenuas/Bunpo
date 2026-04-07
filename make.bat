@setlocal enabledelayedexpansion
@set PREVPROMPT=%PROMPT%
@prompt $E[1A
@set MAKE=make.bat
@echo on

@if "%1" == "" (set TARGET=build
) else (set TARGET=%1 && shift)

@call :%TARGET% %1 %2 %3 %4 %5 %6 %7 %8 %9
@prompt %PREVPROMPT%
@exit /b %ERRORLEVEL%

:build
	dotnet build --nologo -v q --clp:NoSummary
	@exit /b %ERRORLEVEL%

:clean
	dotnet clean --nologo -v q
	@exit /b %ERRORLEVEL%

:distclean
	@call :clean
	rmdir /S /Q src\bin   2>nul
	rmdir /S /Q src\obj   2>nul
	rmdir /S /Q test\bin  2>nul
	rmdir /S /Q test\obj  2>nul
	rmdir /S /Q bench\bin 2>nul
	rmdir /S /Q bench\obj 2>nul
	@exit /b %ERRORLEVEL%

:release
	git archive HEAD --output=Bunpo-%DATE:/=%.zip
	
	dotnet publish src --nologo -v q --clp:NoSummary -c Release -o .tmp
	powershell -NoProfile $ProgressPreference = 'SilentlyContinue' ; Compress-Archive -Force -Path .tmp\*, README.md, LICENSE -DestinationPath Bunpo-lib-%DATE:/=%.zip
	rmdir /S /Q .tmp 2>nul
	
	@exit /b %ERRORLEVEL%

:test
	dotnet test --nologo -v q
	@exit /b %ERRORLEVEL%

:bench
	dotnet run --project bench/Bunpo.Benchmark.csproj --no-launch-profile -c Release %*
	@exit /b %ERRORLEVEL%

:publish
	@call :setenv VERSION_NAME "powershell -Command Get-Date -Format yyyy.M.d"
	@call :setenv BUILD_NAME   "powershell -Command Get-Date -Format HHmm"
	@set      VERSION=%VERSION_NAME%
	@set PACKAGE_NAME=Zenu.Bunpo.%VERSION%.nupkg
	git tag %VERSION%
	git push origin %VERSION%
	dotnet pack --nologo -v q src/Bunpo.csproj -c Release -o nupkg -p:PackageVersion=%VERSION%
	dotnet nuget push nupkg/%PACKAGE_NAME% --api-key %NUGET_APIKEY% --source https://api.nuget.org/v3/index.json
	@exit /b %ERRORLEVEL%

:setenv
	@for /f "usebackq delims=" %%x in (`%~2`) do @set %1=%%x
	@exit /b %ERRORLEVEL%
