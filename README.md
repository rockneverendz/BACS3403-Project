# Mobile IELTS Listening Test System
## Backend Server/ Examiner Portal

This is a backend server also a examiner portal for the Mobile IELTS Listening Test System.

## Installation
### Software Requirement
- IDE: Visual Studio 2019
- Framework: .NET core 3.1 framwork

### Setup
1. Install Visuall Studio 2019.
2. Download the project as zip or clone using git.
3. Unzip the file and open the project using Visuall Studio 2019.
4. Open the project with `BACS3403-Project.sln` solution file.


## Usage

### Migrate Database
1. Go to `Tools` > `NuGet Package Manager` > `Package Manager Console`.
![](https://media.discordapp.net/attachments/751684116109590670/785842838259761172/unknown.png)

2. Run the following command to drop the database if existed
```PowerShell
Drop-Database
```
3. Run the following command to create migration.
```
Add-Migration InitialCreate
Update-Database
```
4. Seed the database using `INSERT.sql`. Open the file select `connect` in to top bar menu. Select `Local` then `MSSQLLocalDB` and click `connect` to continue
![](https://media.discordapp.net/attachments/751684116109590670/785845040080748554/unknown.png?width=570&height=603)
5. From the dropdown list, select `BACS3403_Project`
![](https://media.discordapp.net/attachments/751684116109590670/785845570584969226/unknown.png)
6. Select `Execute` statement.



