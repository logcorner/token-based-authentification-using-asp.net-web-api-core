param($installPath, $toolsPath, $package, $project)

. "$toolsPath\ProjectFileTransform.ps1"

if ($project -eq $null) { Exit }

# Delete bogus content file that serves to start this script
$project.ProjectItems | Where { $_.Name -eq "TypeScript.MSBuildTask.readme.txt" } | ForEach-Object { $_.Delete() }

# Save any current changes to project and update imports
$project.Save()
$xml = [XML] (gc $project.FullName)
if ($xml -eq $null) { exit }
ModifyProjectFile $installPath $toolsPath $package $xml
$xml.Save($project.FullName)