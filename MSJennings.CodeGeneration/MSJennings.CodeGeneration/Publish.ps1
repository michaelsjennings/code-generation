$version = (Get-Item bin\Debug\*\MSJennings.CodeGeneration.dll).VersionInfo.FileVersion | Select-Object -first 1

Add-Type -AssemblyName System.IO.Compression.FileSystem

$nupkgFilename = "$(Get-Location)\bin\Debug\MSJennings.CodeGeneration.1.0.0.nupkg"

try
{
    $nupkgArchive =  [System.IO.Compression.ZipFile]::Open($nupkgFilename, "Update")

    $filesToEdit = $nupkgArchive.Entries.Where({$_.name -like "*.nuspec" -or $_.name -like "*.psmdcp"})

    foreach ($fileToEdit in $filesToEdit)
    {
        try
        {
	        $fileReader = [System.IO.StreamReader]($fileToEdit).Open()
	        $fileContents = $fileReader.ReadToEnd()
        }
        finally
        {
	        $fileReader.Close()
	        $fileReader.Dispose()
        }

	    $fileContents = $fileContents -replace '<version>.*?<\/version>', "<version>$version</version>"

        try
        {
	        $fileWriter = [System.IO.StreamWriter]($fileToEdit).Open()
	        $fileWriter.Write($fileContents)
        }
        finally
        {
            $fileWriter.Flush()
            $fileWriter.Close()
	        $fileWriter.Dispose()
        }
    }
}
finally
{
    $nupkgArchive.Dispose()
}

$nupkgNewFilename = $nupkgFilename -replace '(\.\d+\.\d+\.\d+)?\.nupkg', ".$version.nupkg"
Rename-Item $nupkgFilename -NewName $nupkgNewFilename

if (![System.IO.File]::Exists("Nuget.exe"))
{
    Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile "Nuget.exe"
}

#.\Nuget.exe push -Source https://www.myget.org/F/michaelsjennings/api/v2/package -ApiKey <your_API_key> $nupkgNewFilename
#.\Nuget.exe setApiKey <your_API_key> -Source https://www.myget.org/F/michaelsjennings/api/v2/package
.\Nuget.exe push -Source https://www.myget.org/F/michaelsjennings/api/v2/package $nupkgNewFilename
