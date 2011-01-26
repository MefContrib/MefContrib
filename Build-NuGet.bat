call build.bat

; MefContrib
copy .\release\MefContrib\MefContrib.dll .\nuget\MefContrib\lib
copy "%PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client\System.ComponentModel.Composition.dll" .\nuget\MefContrib\lib
copy "%PROGRAMFILES(X86)%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client\System.ComponentModel.Composition.dll" .\nuget\MefContrib\lib
.\tools\nuget\NuGet.exe pack .\nuget\MefContrib\MefContrib.nuspec
del /Q .\nuget\MefContrib\lib\*.*


; MefContrib.Interception.Castle
copy .\release\MefContrib\MefContrib.Hosting.Interception.Castle.dll .\nuget\MefContrib.Interception.Castle\lib
.\tools\nuget\NuGet.exe pack .\nuget\MefContrib.Interception.Castle\MefContrib.Interception.Castle.nuspec
del /Q .\nuget\MefContrib.Interception.Castle\lib\*.*


; MefContrib.Integration.Unity
copy .\release\MefContrib\MefContrib.Integration.Unity.dll .\nuget\MefContrib.Integration.Unity\lib
.\tools\nuget\NuGet.exe pack .\nuget\MefContrib.Integration.Unity\MefContrib.Integration.Unity.nuspec
del /Q .\nuget\MefContrib.Integration.Unity\lib\*.*


; MefContrib.MVC3
copy .\release\MefContrib\MefContrib.Web.* .\nuget\MefContrib.MVC3\lib
copy "%PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client\System.ComponentModel.Composition.dll" .\nuget\MefContrib.MVC3\lib
copy "%PROGRAMFILES(X86)%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client\System.ComponentModel.Composition.dll" .\nuget\MefContrib.MVC3\lib
.\tools\nuget\NuGet.exe pack .\nuget\MefContrib.MVC3\MefContrib.MVC3.nuspec
del /Q .\nuget\MefContrib.MVC3\lib\*.*

move .\*.nupkg .\release

Pause