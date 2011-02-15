properties {
    $base_directory   = resolve-path "..\."
    $build_directory  = "$base_directory\release"
    $source_directory = "$base_directory\src"
    $nuget_directory  = "$base_directory\nuget"
    $tools_directory  = "$base_directory\tools"
    $version          = "1.0.0.0"
}

include .\psake_ext.ps1

task default -depends NuGet

task Clean -description "This task cleans up the build directory" {
    Remove-Item $build_directory\\$solution -Force -Recurse -ErrorAction SilentlyContinue
}

task Init -description "This tasks makes sure the build environment is correctly setup" {  
    Generate-Assembly-Info `
		-file "$source_directory\MefContrib\Properties\SharedAssemblyInfo.cs" `
		-title "MefContrib $version" `
		-description "Community-developed library of extensions to the Managed Extensibility Framework (MEF)." `
		-company "MefContrib" `
		-product "MefContrib $version" `
		-version $version `
		-copyright "Copyright © MefContrib 2009 - 2011" `
		-clsCompliant "false"
        
    if ((Test-Path $build_directory) -eq $false) {
        New-Item $build_directory\\$solution -ItemType Directory
    }
}

task Test -depends Compile -description "This task executes all tests" {
    $previous_directory = pwd
    cd $build_directory\\$solution

    $testAssemblies = @(Get-ChildItem $build_directory\\$solution -Recurse -Include *.Tests.dll -Name | Split-Path -Leaf)
    
    foreach($assembly in $testAssemblies) {                   
        & $tools_directory\\nunit\\nunit-console.exe $assembly /nodots
        if ($lastExitCode -ne 0) {
            throw "Error: Failed to execute tests"
        }
    }
    
    cd $previous_directory 
}

task Compile -depends Clean, Init -description "This task compiles the solution" {
    exec { 
        msbuild $source_directory\$solution.sln `
            /p:outdir=$build_directory\\$solution\\ `
            /verbosity:quiet `
            /p:Configuration=Release `
			/property:WarningLevel=3
    }
}

task NuGet -depends Test -description "This task creates the NuGet packages" {
	Create-NuGet-Package `
		-version $version `
		-specfile "$nuget_directory\MefContrib\MefContrib.nuspec" `
		-sourceselection "$base_directory\release\MefContrib\MefContrib.dll" `
		-librariesroot "$nuget_directory\MefContrib\lib" `
		-nugetcommand "$tools_directory\nuget\NuGet.exe"
		
	Create-NuGet-Package `
		-version $version `
		-specfile "$nuget_directory\MefContrib.Interception.Castle\MefContrib.Interception.Castle.nuspec" `
		-sourceselection "$base_directory\release\MefContrib\MefContrib.Hosting.Interception.Castle.dll" `
		-librariesroot "$nuget_directory\MefContrib.Interception.Castle\lib" `
		-nugetcommand "$tools_directory\nuget\NuGet.exe"
		
	Create-NuGet-Package `
		-version $version `
		-specfile "$nuget_directory\MefContrib.Integration.Unity\MefContrib.Integration.Unity.nuspec" `
		-sourceselection "$base_directory\release\MefContrib\MefContrib.Integration.Unity.dll" `
		-librariesroot "$nuget_directory\MefContrib.Integration.Unity\lib" `
		-nugetcommand "$tools_directory\nuget\NuGet.exe"
		
	Create-NuGet-Package `
		-version $version `
		-specfile "$nuget_directory\MefContrib.MVC3\MefContrib.MVC3.nuspec" `
		-sourceselection "$base_directory\release\MefContrib\MefContrib.Web.Mvc.dll" `
		-librariesroot "$nuget_directory\MefContrib.MVC3\lib" `
		-nugetcommand "$tools_directory\nuget\NuGet.exe"

    Move-Item *.nupkg $base_directory\release
}