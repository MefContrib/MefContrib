function Get-Git-Commit
{
	$gitLog = git log --oneline -1
	return $gitLog.Split(' ')[0]
}

function GetFrameworkPath
{
    $bitness = "Framework"
    if (Test-Path HKLM:Software\Wow6432Node) {
        $bitness = 'Framework64'
    }
      
    return "$env:windir\Microsoft.NET\$bitness\v4.0.30319\"
}

function GetSilverlightPath
{
    $root = "\SOFTWARE"
    if (Test-Path HKLM:Software\Wow6432Node) {
        $root = "\SOFTWARE\Wow6432Node"
    }
    
    $path = (Get-ItemProperty -Path "HKLM:\$root\Microsoft\Microsoft SDKs\Silverlight\v4.0\ReferenceAssemblies").SLRuntimeInstallPath
    
    if($path.EndsWith("\") -eq $true)
    {
        $path = $path.SubString(0, $path.Length - 1)
    }
    
    return $path
}

function Generate-Assembly-Info
{
param(
	[string]$clsCompliant = "true",
	[string]$title, 
	[string]$description, 
	[string]$company, 
	[string]$product, 
	[string]$copyright, 
	[string]$version,
	[string]$file = $(throw "file is a required parameter.")
)
  $commit = Get-Git-Commit
  $asmInfo = "using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliantAttribute($clsCompliant )]
[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyTitleAttribute(""$title"")]
[assembly: AssemblyDescriptionAttribute(""$description"")]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyInformationalVersionAttribute(""$version / $commit"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyDelaySignAttribute(false)]
"

	$dir = [System.IO.Path]::GetDirectoryName($file)
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-Host "Generating assembly info file: $file"
	out-file -filePath $file -encoding UTF8 -inputObject $asmInfo
}