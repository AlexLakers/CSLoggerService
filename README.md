# CSLoggerService
---
This service allows you to copy the data from scada system 'ClearScada2015' to database 'MSSQLServer' evey 60 sec. 
You see my first experiance of creating a service.

This Service has the name 'CSLogger' and it works together with scada system 'ClearScada2015' and database 'MSSQLServer2022'.My app is a layer between a scada system and a database.
The service let us copy the neccessary data from scada system and write it to database.The rules which are used in the copping process are described in the XML-document.
---
Why it was created??
When I used the scada system 'ClearScada2015' I needed to upload data from it to external database, but
the scada system didn't let do it and I decided to develop my own service.



**How does it work?**
My project in the scada system have the following structure:
set of stations and each station contains set of variables.CSlogger read value of variables from the scada system using name of variables. These names must be unique id's.
My database 'MSSQLServer' contains several tables(Stations) which include a set of columns, name of columns are the same with name of variables.Also I created a few stored procedures  which 'CSLogger' calles and then called procedures writes value of variables to the table of each station.All the commands include in the '~\ServiceLogger\utils\ExecMe.sql', just execute it.The connection string is sett in the file ~\ServiceLogger\App.config. My service use the API ClearScada2015 for the reading value of variables from scada system. So we must add name of variables and name of stored procedures to the file '~/cfg/Stations.xml'.
When we started the service 'CSLogger' data will be recorded to every 60 sec. Service includes an additionall function for  debugging and logging different states to the following directories:
'~\ServiceLogger\logs\errors' or '~\ServiceLogger\logs\main'. You can set a new path to directory of logs if you'll change file ~\ServiceLogger\cfgNLog.config

**How to install this service?**
So..Firstly we needed to execute SQLQuery from the file ~\ServiceLogger\utils\ExecMe.msql in the database 'MSSQLServer',
after that the database,tables,constraints and stored procedures will been created and ready to work.But we have to create a new user.
Also you needed to install scada system 'ClearScada2015' and learn API in it.After the last actioin you can import my project '~\ServiceLogger\utils\TheProjectClearScada2015'
to the scada system.
Okay, let's continue.The second thing what we need to do is add name of variables and name of stored procedures to the file '~/cfg/Stations.xml'.
You can install the service ~\ServiceLogger\utils\InstallTheService.bat or uninstall it ~\ServiceLogger\utils\UninstallTheService.bat.

**For example**
1).My project '~\ServiceLogger\utils\TheProjectClearScada2015' of scada system contains the following structure:
1. Station 1 
  - ProjectCityNN.Station1.PressIn
  - ProjectCityNN.Station1.PressOut
  - ProjectCityNN.Station1.ErrorCode
  - ProjectCityNN.Station1.Flow
  

2). The file '~/cfg/Stations.xml' contains the following:
   ```xml
<Stations>
  <Station procName="InsertToStation1">
    <Tags>
      <Tag tagName="PressIn" tagSource="ProjectCityNN.Station1.PressIn" />
      <Tag tagName="PressOut" tagSource="ProjectCityNN.Station1.PressOut"/>
      <Tag tagName="ErrorCode" tagSource="ProjectCityNN.Station1.ErrorCode" />
      <Tag tagName="Flow" tagSource="ProjectCityNN.Station1.Flow"/>	
    </Tags>
  </Station>
  ```

3).The SQLQuerry '~\ServiceLogger\utils\ExecMe.sql' includes different commands.I want to show you the main commands.
`
......
create table Station1(
Id int not null identity(1,1),
PressIn int  null,
PressOut int  null,
ErrorCode int  null,
Flow int null,
Dt datetime not null)

create procedure [dbo].[InsertToStation1]
@PressIn int,
@PressOut int,
@ErrorCode int,
@Flow int
as
begin
insert into Station1(PressIn,PressOut,ErrorCode,Flow)
values(@PressIn,@PressOut,@ErrorCode,@Flow)
end
.......
`

4). After that all the data will be added to the table 'Station1'.The data will be update evey 60 sec.The result of working this service we can see bellow:

| Id     | PressIn  | PressOut  | ErrorCode | Flow         | Dt                    |
| -------|----------| --------- | ----------|--------------| ----------------------|
| 269    | 3        | 6         |33         | 3            |2023-11-11 20:51:26.827|
| ...    | ...      |  ...      |...        |...           |
| 300|   |4         |   7       |44         |4             |2023-11-11 21:20:26.827|

You can more information about it (See also '~\ServiceLogger\utils\ExecMe.sql').

5).If  the service will be worked incorrect then system  of logging show us the following resilt(for example):
2023-11-11 21:33:27.3520|ERROR|Socket error during a 301 request 
2023-11-11 21:33:27.3520|ERROR|An error occurred communicating with the server.
or
2023-11-11 19:07:25.5844|DEBUG|The connection to DB of ClearSCADA is successfull 
2023-11-11 19:07:25.6253|DEBUG|The connection to DB of MSSQL is successfull 
See also '~\ServiceLogger\logs'
---
I want to tell you something else.In this project I used the following additional packages and libraries:
 NLog 4.7.5.dll,ClearScada.Client.dll.
























