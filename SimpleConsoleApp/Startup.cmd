SET COR_ENABLE_PROFILING=1
SET COR_PROFILER={324F817A-7420-4E6D-B3C1-143FBED6D855}
SET "COR_PROFILER_PATH_64=%~dp0\RTIA\x64\MicrosoftInstrumentationEngine_x64.dll"
SET "COR_PROFILER_PATH_32=%~dp0\RTIA\x86\MicrosoftInstrumentationEngine_x86.dll"
SET "MicrosoftInstrumentationEngine_HostPath_64=%~dp0\RTIA\x64\Microsoft.ApplicationInsights.ExtensionsHost_x64.dll"
SET "MicrosoftInstrumentationEngine_HostPath_32=%~dp0\RTIA\x86\Microsoft.ApplicationInsights.ExtensionsHost_x86.dll"
SET MicrosoftInstrumentationEngine_Host={CA487940-57D2-10BF-11B2-A3AD5A13CBC0}
REM SET MicrosoftInstrumentationEngine_MessageboxAtAttach=1
SET MicrosoftInstrumentationEngine_FileLog="Errors|Dumps"

"%~dp0\bin\Debug\SimpleConsoleApp.exe