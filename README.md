# Paperspace Setup
* Paperspace using marc@skipp.ai login
* Preferably, use Mac desktop application
* Install AWS cli - Install the AWS CLI Using the MSI Installer
** https://docs.aws.amazon.com/cli/latest/userguide/install-windows.html#install-msi-on-windows
** `aws --configure` w/ access keys for AWS use 'paperspace'
** Example (From PowerShell): `aws s3 ls s3://skipp-xfer/`

# Powershell
* Start a terminal from "Search Windows"
* Home: C:\Users\paperspace

# Revit Projects
Desktop / "MARC TESTING PROJECTS" 
* Try Render Modules_v2 (Cumberland project...)

# Plugin Installation 
The plugin source is in github at https://github.com/mlimotte/revit-plugin 

## Steps
* Download revit pricing distribution zip file  ("s3://skipp-xfer/revit web pricing v1_0_0_1_0 3-12-2019.zip")
** Use google drive or some such to transfer files
** Or AWS cli (better, b/c then you don't have to "unblock"; use "Windows Powershell")
* (if you used Google) Unblock zip file - follow directions described in: https://www.thewindowsclub.com/fix-windows-blocked-access-file
* move zip file to clean folder
* Unzip file in same folder (right click -> 7-zip -> Extract Here)
* Locate file pricing.addin in distribution folder, and open it for Edit
** Update Assembly path entry for all commands to match location of file WebPricing.dll in folder
* Copy pricing.addin to C:\Users\paperspace\AppData\Roaming\Autodesk\Revit\Addins\2020

2018-09-25 Installed path: C:\Users\paperspace\Desktop\ReTest_Pricing_Update2\revit price addin 9-25-18\WebPricing.dll
2019-03-25 New path: C:\Users\paperspace\plugin_20190312\WebPricing.dll

# Running

If installation is correct, there should be several commands in Revit under Addins -> External Commands. Using the last one Update Pricing to connect to server and update Cost data to the instances in the Revit model. All the other commands are for testing.

# Pricing Service

To price, Add-Ins -> External Tools -> Update Pricing
* See Pricing Service logs in Cloudwatch  (search for the word "Elements" to find the full request)

## Pricing Service Logs

https://console.aws.amazon.com/cloudwatch/home?region=us-east-1#logStream:group=/prod/ecs/bridge
