properties {
    $base_directory   = resolve-path "..\."
    $build_directory  = "$base_directory\release"
    $source_directory = "$base_directory\src"
    $tools_directory  = "$base_directory\tools"
    $version          = "1.0.0.0"
}

include .\psake_ext.ps1

task default -depends Merge

task Clean {
    Remove-Item $build_directory\\MefContrib.Silverlight -Force -Recurse -ErrorAction SilentlyContinue
    Remove-Item $build_directory\\MefContrib.Silverlight.dll -Force -ErrorAction SilentlyContinue
}

task Init {  
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
        New-Item $build_directory\\MefContrib.Silverlight -ItemType Directory
    }
}

task Test {
    $previous_directory = pwd
    cd $build_directory\\MefContrib.Silverlight

    $testAssemblies = @(Get-ChildItem $build_directory\\MefContrib.Silverlight -Recurse -Include *.Tests.dll -Name | Split-Path -Leaf)
    
    foreach($assembly in $testAssemblies) {                   
        & $tools_directory\\nunit\\nunit-console.exe $assembly /nodots
        if ($lastExitCode -ne 0) {
            throw "Error: Failed to execute tests"
        }
    }
    
    cd $previous_directory 
}

task Compile -depends Clean, Init {
    exec { 
        msbuild $source_directory\MefContrib.Silverlight.sln `
            /p:outdir=$build_directory\\MefContrib.Silverlight\\ `
            /verbosity:quiet `
            /p:Configuration=Release
    }
}

task Merge -depends Compile, Test { 
    $previous_directory = pwd
    cd $build_directory\\MefContrib.Silverlight
    
	Rename-Item "MefContrib.Silverlight.dll" "MefContrib.Silverlight.Partial.dll"
                                
    & "$tools_directory\\ILMerge\ILMerge.exe" `
        MefContrib.Hosting.Conventions.Silverlight.dll `
        MefContrib.Hosting.Generics.Silverlight.dll `
        MefContrib.Integration.Unity.Silverlight.dll `
        MefContrib.Silverlight.Partial.dll `
        /targetplatform:"v4,$(GetSilverlightPath)"  `
        /out:MefContrib.Silverlight.dll `
        /target:library
        

    if ($lastExitCode -ne 0) {
        throw "Error: Failed to merge assemblies"
    }
    
    cd $previous_directory
     
    Move-Item -Path "$build_directory\\MefContrib.Silverlight\\MefContrib.Silverlight.dll" -Destination "$build_directory\\"
    Remove-Item "$build_directory\\MefContrib.Silverlight" -Force -Recurse -ErrorAction SilentlyContinue
}