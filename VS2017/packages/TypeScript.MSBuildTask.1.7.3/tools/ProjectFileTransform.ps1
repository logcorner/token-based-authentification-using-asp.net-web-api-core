Function Get-Paths {
    param ($toolsPath)
    
    $packageDir = Split-Path -Path $toolsPath -Parent | Split-Path -Leaf
    $packagePath = "`$(ProjectDir)\..\packages\$packageDir"
    $originalTypeScriptPath = '$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\TypeScript'
    
    Return @{
        Props = "$packagePath\tools\MSBuild\Microsoft\VisualStudio\v`$(VisualStudioVersion)\TypeScript\Microsoft.TypeScript.Default.props";
        Target = "$packagePath\tools\MSBuild\Microsoft\VisualStudio\v`$(VisualStudioVersion)\TypeScript\Microsoft.TypeScript.targets";
        Targets = "$packagePath\tools\MSBuild\Microsoft\VisualStudio\v`$(VisualStudioVersion)\TypeScript\Microsoft.TypeScript.targets";
        OriginalProps = "$originalTypeScriptPath\Microsoft.TypeScript.Default.props";
        OriginalTargets = "$originalTypeScriptPath\Microsoft.TypeScript.targets";
        Sdk = "$toolsPath\Microsoft SDKs\TypeScript\1.7";
    }
}

Function ModifyProjectFile {
    param ($installPath, $toolsPath, $package, $xml)
        
    $paths = Get-Paths $toolsPath
    $project = CreateProjectWrapper $xml
    
    $typeScriptVersion = $project.GetProperty('TypeScriptToolsVersion')
    if ($typeScriptVersion -lt "1.4") { Return $typeScriptVersion}
        
    $project.ReplaceImport($paths.OriginalProps, $paths.Props)
    $project.ReplaceImport($paths.OriginalTargets, $paths.Target)
}

Function RestoreProjectFile {
    param ($installPath, $toolsPath, $package, $xml)
    
    $paths = Get-Paths $toolsPath
    $project = CreateProjectWrapper $xml
    
    $project.ReplaceImport($paths.Props, $paths.OriginalProps, $true)
    $project.ReplaceImport($paths.Targets, $paths.OriginalTargets, $true)
}

Function CreateProjectWrapper {
    param ($xml)
    
    if ($xml -eq $null) { return $null }
    
    $wrapper = New-Module {
        param ($xml)
        
        [XML]$Xml = $xml
        
        # Grab the namespace from the project element so your xpath works.   
        $nsmgr = New-Object System.Xml.XmlNamespaceManager -ArgumentList $xml.NameTable
        $nsmgr.AddNamespace('msbuild', $xml.Project.GetAttribute("xmlns"))
        
        Function ReplaceImport {
            param ($oldPath, $newPath, $ignoreError)
            $node = $Xml.Project.SelectSingleNode("//msbuild:Import[@Project='$oldPath']", $nsmgr)
            if ($node -eq $null -and $ignoreError) { return $false }
            $node.Project = [string]$newPath
            $node.SetAttribute('Condition', "Exists('$newPath')")
            return $true
        }
        
        Function GetProperty {
            param ($propertyName)
            $node = $xml.Project.SelectSingleNode("//msbuild:$propertyName", $nsmgr)
            if ($node -eq $null) { Return $null }
            Return $node.InnerText
        }
        
        Export-ModuleMember -Variable Xml -Function *
    } $xml -asCustomObject
    
    Return $wrapper
}

Function MessageBox {
  param ($message, $title, $buttons = 0, $icon = 0)
  [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
  Return [System.Windows.Forms.MessageBox]::Show($message, $title, $buttons, $icon)
}

