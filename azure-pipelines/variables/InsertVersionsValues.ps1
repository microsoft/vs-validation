$nbgv = & "$PSScriptRoot\..\Get-nbgv.ps1"
[string]::join(',',(@{
    ('MicrosoftVisualStudioValidationVersion') = & { (& $nbgv get-version --project "$PSScriptRoot\..\..\src\Microsoft.VisualStudio.Validation" --format json | ConvertFrom-Json).AssemblyVersion };
}.GetEnumerator() |% { "$($_.key)=$($_.value)" }))
