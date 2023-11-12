/*Creating database.*/
create database CityNNdb

/*Entering the context our database.*/
use[CityNNdb]
go


/*Creating tables which contain the neccessery tags, the following tables: Station1,Station2*/

create table Station1(
Id int not null identity(1,1),
PressIn int  null,
PressOut int  null,
ErrorCode int  null,
Flow int null,
Dt datetime not null)


create table Station2(
Id int not null identity(1,1),
LevelA int  null,
LevelB int  null,
ErrorCode int  null,
Flow int  null,
Dt datetime not null)

use[CityNNdb]
go
/*The column id is primary key of tables such as Station1,Station2.*/
alter table Station1
add constraint PK_Station1_Id primary key clustered (Id)
alter table Station2
add constraint PK_Station2_Id primary key clustered (Id)

use[CityNNdb]
go
/*And here we added autofill for the column 'Dt' for the wallowing tables: Station1,Station2.*/
alter table Station1
add constraint deffault_Station1_Dt default (GetDate()) for Dt
alter table Station2
add constraint deffault_Station2_Dt default (GetDate()) for Dt

/*Creating the stored procedures for all stations (Station1,Station2).*/
use[CityNNdb]
go

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

create procedure [dbo].[InsertToStation2]
@LevelA int,
@LevelB int,
@ErrorCode int,
@Flow int
as
begin
insert into Station2(LevelA,LevelB,Station2.ErrorCode,Station2.Flow)
values(@LevelA,@LevelB,@ErrorCode,@Flow)
end
