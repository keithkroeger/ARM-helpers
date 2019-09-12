#This file should be hosted in a location that can be reachable for ARM deployment w/script extension.
#for example, this can be hosted in azure blob storage and reachable with a SAS Token URL.

$networkProfile = Get-NetConnectionProfile

Set-NetConnectionProfile -Name $networkProfile.Name -NetworkCategory Private

New-NetFirewallRule -DisplayName "Allow 1433 Inbound" -Direction Inbound -Action Allow -Protocol TCP -LocalPort 1433 -Profile Domain,Private