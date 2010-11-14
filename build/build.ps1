properties {
    $base_directory   = resolve-path "..\."
    $build_directory  = "$base_directory\release"
    $source_directory = "$base_directory\src"
    $tools_directory  = "$base_directory\tools"
    $version          = "1.0.0.0"
}

include .\psake_ext.ps1

task default -depends Test

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
		-copyright "Copyright © MefContrib 2009 - 2010" `
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