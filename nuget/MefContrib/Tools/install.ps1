param($installPath, $toolsPath, $package, $project)

if ($host.Version.Major -eq 1 -and $host.Version.Minor -lt 1) {
     "NOTICE: This package only works with NuGet 1.1 or above. Please update your NuGet install at http://nuget.codeplex.com. You can manually add a reference to System.ComponentModel.Composition if you want to ensure this package functions correctly."
} else {
     $project.Object.References.Add("System.ComponentModel.Composition");
}