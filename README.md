# About this project

> ## Mobile IELTS Listening Test System
>
> ---
>
> IELTS, is a system or an international standardization test of English language that tests proficiency for the non-native English speaker. One of
> the assessments of IELTS is the listening test. There exist paper-based tests and computer-based tests, the Mobile IELTS Listening Test
> System is to provide an additional option which is to allow English listening tests to be conducted using a mobile phone. In which could reduce
> the tools required, and further expand IELTS testing centers.

### Know more about this project [here](https://github.com/rockneverendz/BACS3403-Project/blob/master/Mobile%20IELTS%20Listening%20Test%20System%20Poster.pdf).

### This is a **backend server** of this project, also a **examiner portal** for the Mobile IELTS Listening Test System.

<br>

# Sample screenshots (Examiner Portal)

### Login Page

<img src="./screenshots/1_login.png" width="600"  />
<br>
<br>

### Questions and Recordings

<img src="./screenshots/2_question_bank.png" width="600"/>
<br>
<br>

### Recording details

<img src="./screenshots/3_recordings_details.png" width="600"/>
<br>
<br>

### Editing recordings

<img src="./screenshots/4_editing_recordings.png" width="600"/>
<br>
<br>

### Add New Recording

<img src="./screenshots/5_add_new_recordings.png" width="600"/>
<br>
<br>

### Recording added

<img src="./screenshots/6_recording_added.png" width="600"/>
<br>
<br>

### Delete Recording

<img src="./screenshots/7_delete_recording.png" width="600"/>
<br>
<br>

### Disable recording

<img src="./screenshots/8_disable_recording.png" width="600"/>
<br>
<br>

### Add new question group for recording

<img src="./screenshots/9_create_new_question_group.png" width="600"/>
<br>
<br>
<img src="./screenshots/10_create_new_question_group_2.png" width="600"/>
<br>
<br>
<img src="./screenshots/11_create_new_question_group_3.png" width="600"/>
<br>
<br>

### Generate Seat Number

<img src="./screenshots/12_gen_seatNum.png" width="600"/>
<br>
<br>
<img src="./screenshots/13_seat_qr.png" width="600"/>
<br>
<br>

### View Attendance

<img src="./screenshots/14_view_attendance.png" width="600"/>
<br>
<br>

### Review Answers

<img src="./screenshots/15_review_answers.png" width="600"/>
<br>
<br>
<img src="./screenshots/16_review_answers_2.png" width="600"/>
<br>
<br>

### Generate Grade Report

<img src="./screenshots/17_gen_grade_report.png" width="600"/>
<br>
<br>

### Account Settings

<img src="./screenshots/18_account_settings.png" width="600"/>
<br>
<br>


# Installation

## Software Requirement

-   IDE: Visual Studio 2019
-   Framework: .NET core 3.1 framwork

## Setup

1. Install Visuall Studio 2019.
2. Download the project as zip or clone using git.
3. Unzip the file and open the project using Visuall Studio 2019.
4. Open the project with `BACS3403-Project.sln` solution file.

## Migrate Database

1. Go to `Tools` > `NuGet Package Manager` > `Package Manager Console`.

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
