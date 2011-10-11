function Get-Git-Commit
{
	$gitLog = git log --oneline -1
	return $gitLog.Split(' ')[0]
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

function Update-Nuspec-Version 
{
param( 
	[string]$version,
	[string]$file = $(throw "file is a required parameter.")
)
	# Load nuspec file
	[xml] $spec = gc $file
	
	# Generate version number
	$oldVersion = new-object System.Version($spec.package.metadata.version)	
	$newVersion = new-object System.Version($version)
	$newVersionString = $newVersion.Major.ToString() + "." + $newVersion.Minor.ToString() + "." + $newVersion.Build.ToString() + "." + ($oldVersion.Revision + 1).ToString()

	Write-Host "Setting NuSpec file $file version to $newVersionString"
	
	# Update nuspec file
	$spec.package.metadata.version = $newVersionString 
	$spec.Save($file)  
}

function Create-NuGet-Package 
{
param( 
	[string]$version,
	[string]$specfile = $(throw "specfile is a required parameter."),
	[string]$sourceselection,
	[string]$librariesroot,
	[string]$nugetcommand
)
	Update-Nuspec-Version -version $version -file $specfile
    Copy-Item $sourceselection $librariesroot
    exec { &"$nugetcommand" pack $specfile -Symbols }
    Remove-Item $librariesroot\* -exclude .empty,Linfu.*
}