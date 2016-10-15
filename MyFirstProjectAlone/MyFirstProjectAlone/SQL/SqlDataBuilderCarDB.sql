use master
if exists (select 1 from sys.databases where name = 'CarDB')
begin
	alter database CarDB set single_user with rollback immediate;
	drop database  CarDB;
end
go
create database CarDB1
go 
use CarDB1
go
Create table Users 
(
	ID int not null primary key identity(1,1),
	FirstName nvarchar(20) not null ,
	LastName nvarchar(20) not null,
	Email nvarchar(50) not null unique ,
	Username nvarchar(20) not null ,
	[Password] nvarchar(50) not null ,
	PhoneNumber nvarchar(50) not null,
	RoleID  int not null default(1)
)
go
create table UserLogins
(
	ID int primary key identity(1,1) not null,
	UserID int not null ,
	[Guid] nvarchar(100) not null unique ,
	LastLogin datetimeoffset(7) not null,
	ExpireTime datetimeoffset(7) not null,
	constraint Fk_UserLogins_Users_ID Foreign Key (UserID) references Users (ID) 
)
go
create table Roles
(
	ID int not null primary key identity (1,1),
	name nvarchar(20) not null,
)
go
insert into Roles values ('User')
insert into Roles values ('Moderator')
insert into Roles values ('Admin')
go
alter table Users
add Constraint FK_Users_RoleID_ID foreign key (RoleID) references Roles(ID)
go
create table CarModels 
(
	ID int not null primary key identity(1,1),
	ModelName nvarchar(50) not null,
)
go
create table Marks 
(
	ID int primary key identity(1,1) not null ,
	MarkName nvarchar(50) not null 
)
go
alter table CarModels add MarksID int not null
go
alter table CarModels add constraint FK_MarksID_ID foreign key (MarksID) references Marks(ID)
go 
create table ProducedTime
(
	 ID int primary key not null identity(1,1),
	 [Year] int  not null 
)
go 
create table TypeOfCar
(
	ID int primary key not null identity(1,1),
	NameOfType nvarchar(50) not null
)
go
alter table CarModels add TypeID int not null
go
alter table CarModels add constraint FK_TypeID_ID foreign key (TypeID) references TypeOfCar(ID)
go
create table Oturucu
(
	ID int not null primary key identity(1,1),
	Name nvarchar(20) not null ,
)
go
create table TypesOfFuel
(
	ID int not null primary key identity(1,1),
	Name nvarchar(20) not null 
)
go
create table CapacityOfEngine
(
	ID int not null primary key identity (1,1),
	Capacity int not null 
)
go 
create table TypeOfEngine 
(
	ID int  primary key identity(1,1) not null,
	Name nvarchar(20) not null  
)
go
create table tbl_car_data
(
	ID int primary key identity(1,1) not null,
	TypeOfCarID int not null,
	MarkID int not null,
	ModelID int not null,
	Oturucu_ID int not null,
	TypeOfEngineID int not null,
	TypesOfFuelID int not null,
	CapacityOfEngineID int not null,
	ProducedTimeID int not null,
	UserNameID int null,
	constraint FK_TypeOfCarID_ID foreign key (TypeOfCarID) references TypeOfCar(ID),
	constraint FK_MarkID_ID foreign key (MarkID) references Marks(ID),
	constraint FK_ModelID_ID foreign key (ModelID) references CarModels(ID),
	constraint FK_OturucuID_ID foreign key (Oturucu_ID) references Oturucu(ID),
	constraint FK_TypeOfEngineID_ID foreign key (TypeOfEngineID) references TypeOfEngine(ID),
	constraint FK_TypesOfFuelID_ID  foreign key (TypesOfFuelID ) references TypesOfFuel(ID),
	constraint FK_CapacityOfEngineID_ID foreign key (CapacityOfEngineID) references CapacityOfEngine(ID),
	constraint FK_ProducedTimeID_ID foreign key (ProducedTimeID) references ProducedTime(ID),
	constraint FK_UserNameID_ID foreign key (UserNameID) references Users(ID)
)
go 
alter table Marks add TypeOfCarID int not null 
go
alter table Marks add constraint FK_TypeOfCarID_MarksID foreign key (TypeOfCarID) references Marks(ID)
go
Create procedure uspLogin
(
	@Email nvarchar(50), @Password nvarchar(50)
)
as
begin 
	select Users.ID from Users where Email=@Email and [Password]=@Password
end
go
create procedure uspInsertUserLogins
(
	@UserID int,@Guid nvarchar(36),
	@LastLogin datetimeoffset(7),@ExpireTime datetimeoffset(7)
)
as
begin
	insert into UserLogins values (@UserID,@Guid,@LastLogin,@ExpireTime)
end
go
create procedure uspCheckGuid
(
	@Guid nvarchar(36)
)
as
begin
	select Users.RoleID from UserLogins 
	inner join Users on UserLogins.UserID=Users.ID 
	where [Guid]=@Guid
end
go
create procedure uspUpdateLastLogin
(
	@Guid nvarchar(36), @LastLogin datetimeoffset(7)
)
as
begin
	update UserLogins set LastLogin=@LastLogin where [Guid]=@Guid
end
go
create procedure uspGetUserID
(
	@Guid nvarchar(36)
)
as
begin
	select UserLogins.UserID from UserLogins where Guid=@Guid
end
go

