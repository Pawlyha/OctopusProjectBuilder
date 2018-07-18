Define-Step -Name 'Build' -Target 'build' -Body {
	$sln = "OctopusProjectBuilder.sln"

	Write-Status "Cleaning solution"
	call dotnet clean $sln --configuration "Release"

	Write-Status "Building solution"
	call dotnet build $sln --configuration "Release" --force
}

Define-Step -Name 'Tests' -Target 'build' -Body {
	. (require 'psmake.mod.testing')

	$projects = @("OctopusProjectBuilder.Uploader.Tests", "OctopusProjectBuilder.YamlReader.Tests")
	$tests = @()
	$tests += Define-DotnetTests -TestProject $projects

	$tests `
        | Run-Tests -EraseReportDirectory -Cover -CodeFilter '+[OctopusProjectBuilder*]* -[*.Tests*]*'`
        | Generate-CoverageSummary -ReportGeneratorVersion '3.1.2'`
        | Check-AcceptableCoverage -AcceptableCoverage 89

}

Define-Step -Name 'Publish' -Target 'build' -Body {
	if (Test-Path -Path "$PUBLISH_DIR"){
		Remove-Item "$PUBLISH_DIR" -Force -Recurse
	}

	$projects = @("OctopusProjectBuilder.Console", "OctopusProjectBuilder.DocGen")

	foreach ($project in $projects){
		Write-Status "Publishing $project"
		dotnet publish "$project" -c "Release" -r "$TARGET_PLATFORM" -o "..\$PUBLISH_DIR\$project" --self-contained
	}
}

Define-Step -Name 'Documentation generation' -Target 'build' -Body {
	call dotnet "$PUBLISH_DIR\OctopusProjectBuilder.DocGen\OctopusProjectBuilder.DocGen.dll" | out-file Manual.md -Encoding utf8
	if ($LastExitCode -ne 0) { throw "A program execution was not successful (Exit code: $LASTEXITCODE)." }
}

Define-Step -Name 'Packaging' -Target 'build' -Body {
	. (require 'psmake.mod.packaging')

	Package-DeployableNuSpec -Package 'OctopusProjectBuilder.Console.nuspec' -version $VERSION
}

Define-Step -Name 'Update Wiki' -Target 'update-wiki' -Body {
	$wikiUrl = & git config --get remote.origin.url
	$wikiUrl = $wikiUrl -replace '\.git$','.wiki.git'
	call git clone $wikiUrl wiki

	cp .\Manual.md wiki\Manual.md -Force
	try {
		pushd wiki
		call git commit '-a' '--allow-empty' '-m' "wiki commit $VERSION"
		call git push origin master
	}
	finally {
		popd
	}
}