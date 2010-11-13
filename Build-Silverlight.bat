powershell -Command "& {Import-Module .\build\psake.psm1; $psake.use_exit_on_error = $true; Invoke-psake .\build\build.ps1 -framework 4.0x86 -parameters @{solution='MefContrib.Silverlight'}}"

Pause