call build.bat

; MefContrib
copy .\release\MefContrib\MefContrib.* .\nuget\MefContrib\lib
.\tools\nuget\NuGet.exe pack .\nuget\MefContrib\MefContrib.nuspec
del /Q .\nuget\MefContrib\lib\*.*

; MefContrib.MVC3
copy .\release\MefContrib\MefContrib.Web.* .\nuget\MefContrib.MVC3\lib
.\tools\nuget\NuGet.exe pack .\nuget\MefContrib.MVC3\MefContrib.MVC3.nuspec
del /Q .\nuget\MefContrib.MVC3\lib\*.*

copy .\*.nupkg .\release

Pause