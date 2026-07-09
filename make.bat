@echo off
set "PLUGIN_NAME=CustomFanCurve"
set "DLL_NAME=CustomFanCurve"

echo Building %PLUGIN_NAME% in Release configuration...
dotnet publish %PLUGIN_NAME%\%DLL_NAME%.csproj -c Release -o %PLUGIN_NAME%\bin\Publish > %PLUGIN_NAME%\build.log 2>&1 || (type %PLUGIN_NAME%\build.log && exit /b)

echo.
echo Copying output DLLs to LocalAppData (replacing original CustomFanCurve)...
xcopy /Y /I "%PLUGIN_NAME%\bin\Publish\%DLL_NAME%.dll" "%LOCALAPPDATA%\LenovoLegionToolkit\Plugins\%PLUGIN_NAME%\"
if exist "%LOCALAPPDATA%\LenovoLegionToolkit\Plugins\%PLUGIN_NAME%\UniversalFanControl.Lib.dll" (
    del /q "%LOCALAPPDATA%\LenovoLegionToolkit\Plugins\%PLUGIN_NAME%\UniversalFanControl.Lib.dll"
)

echo.
echo Build and deployment completed successfully!
