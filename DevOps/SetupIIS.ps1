Import-Module WebAdministration

## VARIABLES
$IPG = "ParkingManagement"
$RootPath = Split-Path $PSScriptRoot
$ParkingManagementClientApi ="${RootPath}\Code\ParkingManagement\ParkingManagement.WebClient.Api" 
$ParkingManagementClientWeb ="${RootPath}\Code\ParkingManagement\ParkingManagement.WebClient.Web"

## Website folders
Function AddApiAppPool ($AppPoolName)
{
	$Path = "IIS:\AppPools\{0}" -f  $AppPoolName;

	$AppPool = Get-Item $Path -ErrorAction SilentlyContinue;

	If (!$AppPool)
	{
		Write-Host "Adding $AppPoolName ApplicationPool" -ForegroundColor Green
		$AppPool = New-WebAppPool $AppPoolName
	}
	Else
	{
		Write-Host "$AppPoolName  already exists" -ForegroundColor Green
	}

	$AppPool.managedRuntimeVersion = "v4.0";
	$AppPool.processModel.identityType = "LocalSystem";
	$AppPool | Set-Item;

	return (Get-Item $Path -ErrorAction Stop)
}

Function AddWebAppPool ($AppPoolName)
{
	$Path = "IIS:\AppPools\{0}" -f  $AppPoolName;

	$AppPool = Get-Item $Path -ErrorAction SilentlyContinue;

	If (!$AppPool)
	{
		Write-Host "Adding $AppPoolName ApplicationPool" -ForegroundColor Green
		$AppPool = New-WebAppPool $AppPoolName
	}
	Else
	{
		Write-Host "$AppPoolName  already exists" -ForegroundColor Green
	}

	$AppPool.managedRuntimeVersion = "";
	$AppPool.processModel.identityType = "NetworkService";
	$AppPool | Set-Item;

	return (Get-Item $Path -ErrorAction Stop)
}

Function AppPoolsExist
{
	$ParkingManagementApiAppPool = Test-Path "IIS:\AppPools\ParkingManagement.WebClient.Api";
	$ParkingManagementWebAppPool = Test-Path "IIS:\AppPools\ParkingManagement.WebClient.Web";
	
	$HasAppPools = ($ParkingManagementApiAppPool -and $ParkingManagementWebAppPool);

	return $HasAppPools;
}

Function CreateAppPools
{
	$HasAppPools = AppPoolsExist;

	If ($HasAppPools)
	{
		Write-Host "IIS application pools already exist." -ForegroundColor Green
	}
	Else
	{
		Write-Host "Adding Application Pools to IIS" -ForegroundColor Green
	
		AddApiAppPool "ParkingManagement.WebClient.Api";
		AddWebAppPool "ParkingManagement.WebClient.Web";

		Write-Host "IIS application pools added successfully" -ForegroundColor Green
	}
}

Function AddWebApplication ($Path, $Name, $AppPoolName)
{
	If (!(Get-WebApplication -Site $IPG -Name $Name ))
	{
		Write-Host "Adding WebApplication to IIS for $Name" -ForegroundColor Green
	}
	Else
	{
		Remove-WebApplication -Name $Name -Site $IPG;

		Write-Host "Overwriting existing WebApplication for $Name" -ForegroundColor Green
	}

	New-WebApplication -Name $Name -Site $IPG -PhysicalPath $Path -ApplicationPool $AppPoolName -Force;
}
Function CheckIfWebsiteFilesExist
{
	If ( !(Test-Path $ParkingManagementClientApi) -or !(Test-Path $ParkingManagementClientWeb))
	{
		throw "This script assumes you have downloaded the codebase from Git. The website files were not found.";
	}
}

Function CreateWebApplications
{
	Write-Host "Configuring IPG web-applications."  -ForegroundColor Green

	CheckIfWebsiteFilesExist;

	AddWebApplication $ParkingManagementClientApi "api" "ParkingManagement.WebClient.Api";

	Write-Host "Web-applications configured."  -ForegroundColor Green
}

Function AddIPGWebsite
{
	If (-Not (Get-WebBinding -Name $IPG))
	{
		## Reset bindings
		New-Item iis:\Sites\$IPG -bindings @{protocol="http";bindingInformation=":8034:"} -physicalPath $ParkingManagementClientWeb;
		Set-ItemProperty IIS:\Sites\$IPG -name applicationPool -value "ParkingManagement.WebClient.Web"
	}
}

## BEGIN

Write-Host "Configuring IIS." -ForegroundColor Green

Try
{
	## IIS application pools
	CreateAppPools;

	## Create new site
	AddIPGWebsite;

	## Add new web-applications to IIS
	CreateWebApplications;
	
	iisreset;
	
	Write-Host "IIS is set-up and configured." -ForegroundColor Green
}
Catch
{
	$ErrorMessage = $_.Exception.Message;

	throw "IIS Setup failed. Error: $ErrorMessage";
}

## END