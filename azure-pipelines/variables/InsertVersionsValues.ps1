[string]::join(',',(@{
    ('MicrosoftVisualStudioValidationVersion') = & { (dotnet tool run nbgv get-version --project "$PSScriptRoot\..\..\src\Microsoft.VisualStudio.Validation" --format json | ConvertFrom-Json).AssemblyVersion };
}.GetEnumerator() |% { "$($_.key)=$($_.value)" }))
