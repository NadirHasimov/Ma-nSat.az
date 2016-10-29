use master
if exists (select 1 from sys.databases where name = 'CarDB')C:\Users\hasim\Source\Repos\Ma-nSat.az\MyFirstProjectAlone\MyFirstProjectAlone\SQL\SqlDataBuilderCarDB.sql
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

create procedure uspInsertIntoCarsTBL 
(
	@TypeOfCarID int,@MarkID int,@ModelID int,@Oturucu_ID int,@TypeOfEngineID int,
	@TypesOfFuelID int,@CapacityOfEngineID int,@ProducedTimeID int,@UserNameID int,@Price int,

	@ABS_ID int,@ArxaGoruntuKamerasi_ID int,@DeriSalon_ID int,@Kondisoner_ID int,
	@KsenonLampalar_ID int,@Lyuk_ID int, @MerkeziQapanma_ID int,@OturacaqlarinIsisdilmesi_ID int,
	@OturacaqlarinVentilyasiyasi_ID int,@ParkRadari_ID int, @YagisSensoru_ID int,@YanPerdeler_ID int,
	@YungulLehimliDiskler_ID int,
	@PowerOfEngine int,@ColourID int
)
as
begin
	insert into tbl_car_data(TypeOfCarID,MarkID,ModelID,Oturucu_ID,TypeOfEngineID,
	TypesOfFuelID,CapacityOfEngineID,ProducedTimeID,UserNameID,Price,PowerOfEngine)
	
	 values (@TypeOfCarID,@MarkID,@ModelID,@Oturucu_ID,@TypeOfEngineID,
	@TypesOfFuelID,@CapacityOfEngineID,@ProducedTimeID,@UserNameID,@Price,@PowerOfEngine)

	insert into Equipments ([Yüngül lehimli disklərID],ABS_ID,Lyuk_ID,[Yağış sensoru_ID],[Mərkəzi qapanma_ID],
	[Park radarı_ID],Kondisioner_ID,[Oturacaqların isidilməsi_ID],[Dəri salon_ID],[Ksenon lampalar_ID],[Arxa görüntü kamerası_ID],
	[Yan pərdələr_ID],[Oturacaqların ventilyasiyası_ID],ColourID)
	
	values (@YungulLehimliDiskler_ID,@ABS_ID,@Lyuk_ID,@YagisSensoru_ID,
	@MerkeziQapanma_ID,@ParkRadari_ID,@Kondisoner_ID,@OturacaqlarinIsisdilmesi_ID,
	@DeriSalon_ID,@KsenonLampalar_ID,@ArxaGoruntuKamerasi_ID,
	@YanPerdeler_ID,@OturacaqlarinVentilyasiyasi_ID,@ColourID)
end

go
dbcc checkident ('tbl_car_data',reseed,1)
dbcc checkident ('Equipments',reseed,1)
go
create procedure uspSelectCarINFO
(
	@ID int
)
as
begin
     select 
	 TypeOfCar.NameOfType as [Maşının növü],
	 Marks.MarkName as [Maşının markası],
	 CarModels.ModelName as [Modeli],
     TypeOfEngine.Name as [Mühərrikin növü],
	 CapacityOfEngine.Capacity as [Mühərrikin həcmi], 
	 TypesOfFuel.Name as [Yanacağın növü],
	 ProducedTime.Year as [Buraxılış ili],
	 Users.Username as [İstifadəçi adı],
	 Oturucu.Name as Ötürücü,
	 tbl_car_data.Price ,
	 CarColours.Colour,
	 tbl_car_data.PowerOfEngine,
	 case when [ABS].[Text] is null then 'Yoxdur' else [ABS].[Text] end as [ABS],
	 case when [Arxa görüntü kamerası].[Text] is null then 'Yoxdur' else [Arxa görüntü kamerası].[Text] end as [Arxa görüntü kamerası],
	 case when [Dəri salon].[Text] is null then 'Yoxdur' else [Dəri salon].[Text] end as [Dəri salon],
	 case when Kondisioner.[Text] is null then 'Yoxdur' else Kondisioner.[Text] end as Kondisioner,
	 case when [Ksenon lampalar].[Text] is null then 'Yoxdur' else [Ksenon lampalar].[Text] end as [Ksenon lampalar],
	 case when Lyuk.[Text] is null then 'Yoxdur' else Lyuk.[Text] end as Lyuk,
	 case when [Mərkəzi qapanma].[Text] is null then 'Yoxdur' else [Mərkəzi qapanma].[Text] end as [Mərkəzi qapanma],
	 case when [Oturacaqların isidilməsi].[Text] is null then 'Yoxdur' else [Oturacaqların isidilməsi].[Text] end as [Oturacaqların isidilməsi],
	 case when [Oturacaqların ventilyasiyası].[Text] is null then 'Yoxdur' else [Oturacaqların ventilyasiyası].[Text] end as [Oturacaqların ventilyasiyası],
	 case when [Park radarı].[Text] is null then 'Yoxdur' else [Park radarı].[Text] end as [Park radarı],
	 case when [Yağış sensoru].[Text] is null then 'Yoxdur' else [Yağış sensoru].[Text] end as [Yağış sensoru],
	 case when [Yan pərdələr].[Text] is null then 'Yoxdur' else [Yan pərdələr].[Text] end as [Yan pərdələr],
	 case when [Yüngül lehimli disklər].[Text] is null then 'Yoxdur' else [Yüngül lehimli disklər].[Text] end as [Yüngül lehimli disklər]
	 from tbl_car_data
	 inner join Equipments on tbl_car_data.ID=Equipments.CarID
     inner join TypeOfCar on TypeOfCar.ID=tbl_car_data.TypeOfCarID
     inner join Marks on Marks.ID=tbl_car_data.MarkID
     inner join TypeOfEngine on TypeOfEngine.ID=tbl_car_data.TypeOfEngineID
     inner join ProducedTime on ProducedTime.ID=tbl_car_data.ProducedTimeID
     inner join Users on Users.ID=tbl_car_data.UserNameID
     inner join TypesOfFuel on TypesOfFuel.ID=tbl_car_data.TypesOfFuelID
     inner join CapacityOfEngine on CapacityOfEngine.ID=tbl_car_data.CapacityOfEngineID
     inner join Oturucu on Oturucu.ID=tbl_car_data.Oturucu_ID
     inner join CarModels on CarModels.ID=tbl_car_data.ModelID
	 inner join CarColours on Equipments.ColourID=CarColours.ID
	 left join [ABS] on Equipments.ABS_ID=[ABS].ID
	 left join [Arxa görüntü kamerası] on Equipments.[Arxa görüntü kamerası_ID]=[Arxa görüntü kamerası].ID
	 left join [Dəri salon] on Equipments.[Dəri salon_ID]=[Dəri salon].ID
	 left join Kondisioner on Equipments.Kondisioner_ID=Kondisioner.ID
	 left join [Ksenon lampalar] on Equipments.[Ksenon lampalar_ID]=[Ksenon lampalar].ID
	 left join [Lyuk] on Equipments.Lyuk_ID=Lyuk.ID
	 left join [Mərkəzi qapanma] on Equipments.[Mərkəzi qapanma_ID]=[Mərkəzi qapanma].ID
	 left join [Oturacaqların isidilməsi] on Equipments.[Oturacaqların isidilməsi_ID]=[Oturacaqların isidilməsi].ID
	 left Join [Oturacaqların ventilyasiyası] on Equipments.[Oturacaqların ventilyasiyası_ID]=[Oturacaqların ventilyasiyası].ID
	 left join [Park radarı] on Equipments.[Park radarı_ID]=[Park radarı].ID
	 left join [Yağış sensoru] on Equipments.[Yağış sensoru_ID]=[Yağış sensoru].ID
	 left join [Yan pərdələr] on Equipments.[Yan pərdələr_ID]=[Yan pərdələr].ID
	 left join [Yüngül lehimli disklər] on Equipments.[Yüngül lehimli disklərID]=[Yüngül lehimli disklər].ID
	 where tbl_car_data.ID=@ID
end
go
create table [Equipments] 
(
	ID int primary key not null identity(1,1),
	CarID int not null,
	[Yüngül lehimli disklərID] int  null,
	[ABS_ID] int null ,
	[Lyuk_ID] int null,
	[Yağış sensoru_ID] int null,
	[Mərkəzi qapanma_ID] int null,
	[Park radarı_ID] int null,
	[Kondisioner_ID] int null,
	[Oturacaqların isidilməsi_ID] int null,
	[Dəri salon_ID] int null,
	[Ksenon lampalar_ID] int null,
	[Arxa görüntü kamerası_ID] int null,
	[Yan pərdələr_ID] int null,
	[Oturacaqların ventilyasiyası_ID] int null ,
	Constraint FK_CarID_ID foreign key (CarID) references tbl_car_data(ID)
)
go
create table [Yüngül lehimli disklər](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [ABS](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Lyuk](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Yağış sensoru](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Mərkəzi qapanma](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Park radarı](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Kondisioner](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Oturacaqların isidilməsi](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Dəri salon](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Ksenon lampalar](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Arxa görüntü kamerası](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Yan pərdələr](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
create table [Oturacaqların ventilyasiyası](
 ID int identity(1,1) primary key,
 [Text] nvarchar(50),
)
go
update ABS set Text='Var' where ABS.ID=1
update [Arxa görüntü kamerası] set Text='Var' where [Arxa görüntü kamerası].ID=1
update [Dəri salon] set Text='Var' where [Dəri salon].ID=1
update Kondisioner set Text='Var' where Kondisioner.ID=1
update [Ksenon lampalar] set Text='Var' where [Ksenon lampalar].ID=1
update Lyuk set Text='Var' where Lyuk.ID=1
update [Oturacaqların isidilməsi] set Text='Var' where [Oturacaqların isidilməsi].ID=1
update [Oturacaqların ventilyasiyası] set Text='Var' where [Oturacaqların ventilyasiyası].ID=1
update [Yağış sensoru] set Text='Var' where [Yağış sensoru].ID=1
update [Yan pərdələr] set Text='Var' where [Yan pərdələr].ID=1
update [Mərkəzi qapanma] set Text='Var' where [Mərkəzi qapanma].ID=1
update [Yüngül lehimli disklər] set Text='Var' where [Yüngül lehimli disklər].ID=1
update [Park radarı] set Text='Var' where [Park radarı].ID=1
go
create procedure uspGetTypesOfCar
as
begin
	select TypeOfCar.ID as ID, TypeOfCar.NameOfType as [Maşının Tipi] 
	from  TypeOfCar 
end
go
create procedure uspGetMarks
(
	@TypeID int
)
as
begin 
	select Marks.ID,Marks.MarkName as Markasi from  Marks where Marks.TypeOfCarID=@TypeID
end
go
create procedure uspGetModels
(
	@MarkID int
)
as
begin 
	select CarModels.ID,CarModels.ModelName as Modeli from CarModels where CarModels.MarksID=@MarkID
end
go
create Procedure uspGetOturucu
as
begin
	select Oturucu.ID, Oturucu.Name as Oturucu from Oturucu
end
go
create procedure uspGetTypesOfEngine
as
begin
	select TypeOfEngine.ID,TypeOfEngine.Name as Mühərrik from TypeOfEngine
end
go
create procedure uspGetCapacityOfEngine
as
begin
	select CapacityOfEngine.ID, CapacityOfEngine.Capacity from CapacityOfEngine
end
go
delete  tbl_car_data
delete Equipments
alter table tbl_car_data add Price int not null
go
create procedure uspUpdateCarPrice
(
	@CarID int,@Price int
)
as
begin 
	update tbl_car_data set Price=@Price where tbl_car_data.ID=@CarID
end
go
alter table Users add [Role] bit default(0)
alter table Users drop column  [Role]
delete Users
delete UserLogins
go
create procedure uspSetRole
(
	@UserID int ,@RoleID int
)
as
begin
	update Users set RoleID=@RoleID where Users.ID =@UserID 
end
go
create procedure uspGetRoles
as
begin
	select Roles.ID, Roles.name from Roles
end
go
create procedure uspDeleteCarID
(
	@CarID int
)
as
begin 
	delete tbl_car_data where ID=@CarID
	delete Equipments where Equipments.CarID=@CarID
end
go
create procedure uspGetTypesOfFuel
as
begin
	select TypesOfFuel.ID,TypesOfFuel.Name from TypesOfFuel
end
go
alter table tbl_car_data add Confirmed bit default('false')
go

 create type IntListType
     as table(item int)
go
create procedure uspConfirmAdvertises
(
	@CarIDS IntListType readonly
)
as
begin
	update tbl_car_data 
	set Confirmed='true'  
	where Exists
	(select 1 from @CarIDS where item=tbl_car_data.ID)
end 
go
alter procedure uspDeleteAdvertises
(
	@CarIDs IntListType readonly 
)
as
begin
	delete tbl_car_data 
	where 
	Exists
	(select 1 from @CarIDs where item=tbl_car_data.ID and tbl_car_data.Confirmed='false')
end
dbcc checkident ('tbl_car_data',reseed,1) 
go
	create type StringListType
	AS table(item nvarchar(100))
go
	create view  vWSelectTbl_carData
as
  select 
	 TypeOfCar.NameOfType as [Maşının növü],--1
	 Marks.MarkName as [Maşının markası],--2
	 CarModels.ModelName as [Modeli],--3
     TypeOfEngine.Name as [Mühərrikin növü],--4
	 CapacityOfEngine.Capacity as [Mühərrikin həcmi], --5
	 TypesOfFuel.Name as [Yanacağın növü],--6
	 ProducedTime.Year as [Buraxılış ili],--7
	 Users.Username as [İstifadəçi adı],--8
	 Oturucu.Name as Ötürücü,--9
	 tbl_car_data.Price ,--10
	 CarColours.Colour,--11
	 tbl_car_data.PowerOfEngine,--12

	 case when [ABS].[Text] is null then 'Yoxdur' else [ABS].[Text] end as [ABS],--13
	 case when [Arxa görüntü kamerası].[Text] is null then 'Yoxdur' else [Arxa görüntü kamerası].[Text] end as [Arxa görüntü kamerası],--14
	 case when [Dəri salon].[Text] is null then 'Yoxdur' else [Dəri salon].[Text] end as [Dəri salon],--15
	 case when Kondisioner.[Text] is null then 'Yoxdur' else Kondisioner.[Text] end as Kondisioner,--16
	 case when [Ksenon lampalar].[Text] is null then 'Yoxdur' else [Ksenon lampalar].[Text] end as [Ksenon lampalar],--17
	 case when Lyuk.[Text] is null then 'Yoxdur' else Lyuk.[Text] end as Lyuk,--18
	 case when [Mərkəzi qapanma].[Text] is null then 'Yoxdur' else [Mərkəzi qapanma].[Text] end as [Mərkəzi qapanma],--19
	 case when [Oturacaqların isidilməsi].[Text] is null then 'Yoxdur' else [Oturacaqların isidilməsi].[Text] end as [Oturacaqların isidilməsi],--20
	 case when [Oturacaqların ventilyasiyası].[Text] is null then 'Yoxdur' else [Oturacaqların ventilyasiyası].[Text] end as [Oturacaqların ventilyasiyası],--21
	 case when [Park radarı].[Text] is null then 'Yoxdur' else [Park radarı].[Text] end as [Park radarı],--22
	 case when [Yağış sensoru].[Text] is null then 'Yoxdur' else [Yağış sensoru].[Text] end as [Yağış sensoru],--23
	 case when [Yan pərdələr].[Text] is null then 'Yoxdur' else [Yan pərdələr].[Text] end as [Yan pərdələr],--24
	 case when [Yüngül lehimli disklər].[Text] is null then 'Yoxdur' else [Yüngül lehimli disklər].[Text] end as [Yüngül lehimli disklər]--25
	 from tbl_car_data
	 inner join Equipments on tbl_car_data.ID=Equipments.CarID
     inner join TypeOfCar on TypeOfCar.ID=tbl_car_data.TypeOfCarID
     inner join Marks on Marks.ID=tbl_car_data.MarkID
     inner join TypeOfEngine on TypeOfEngine.ID=tbl_car_data.TypeOfEngineID
     inner join ProducedTime on ProducedTime.ID=tbl_car_data.ProducedTimeID
     inner join Users on Users.ID=tbl_car_data.UserNameID
     inner join TypesOfFuel on TypesOfFuel.ID=tbl_car_data.TypesOfFuelID
     inner join CapacityOfEngine on CapacityOfEngine.ID=tbl_car_data.CapacityOfEngineID
     inner join Oturucu on Oturucu.ID=tbl_car_data.Oturucu_ID
	 inner join CarColours on CarColours.ID=Equipments.ColourID
     inner join CarModels on CarModels.ID=tbl_car_data.ModelID
	 left join [ABS] on Equipments.ABS_ID=[ABS].ID
	 left join [Arxa görüntü kamerası] on Equipments.[Arxa görüntü kamerası_ID]=[Arxa görüntü kamerası].ID
	 left join [Dəri salon] on Equipments.[Dəri salon_ID]=[Dəri salon].ID
	 left join Kondisioner on Equipments.Kondisioner_ID=Kondisioner.ID
	 left join [Ksenon lampalar] on Equipments.[Ksenon lampalar_ID]=[Ksenon lampalar].ID
	 left join [Lyuk] on Equipments.Lyuk_ID=Lyuk.ID
	 left join [Mərkəzi qapanma] on Equipments.[Mərkəzi qapanma_ID]=[Mərkəzi qapanma].ID
	 left join [Oturacaqların isidilməsi] on Equipments.[Oturacaqların isidilməsi_ID]=[Oturacaqların isidilməsi].ID
	 left Join [Oturacaqların ventilyasiyası] on Equipments.[Oturacaqların ventilyasiyası_ID]=[Oturacaqların ventilyasiyası].ID
	 left join [Park radarı] on Equipments.[Park radarı_ID]=[Park radarı].ID
	 left join [Yağış sensoru] on Equipments.[Yağış sensoru_ID]=[Yağış sensoru].ID
	 left join [Yan pərdələr] on Equipments.[Yan pərdələr_ID]=[Yan pərdələr].ID
	 left join [Yüngül lehimli disklər] on Equipments.[Yüngül lehimli disklərID]=[Yüngül lehimli disklər].ID
go
alter procedure uspGetFilteredCars
(
	@TypeOfCarIDs IntListType readonly,
	@MarkIDs IntListType readonly,
	@ModelIDs IntListType readonly,
	@OturucuIDs IntListType readonly,
	@TypesOfFuelIDs IntListType readonly,
	@SearchText StringListType readonly,
	@ColourIDs IntListType readonly,
	@LowPrice int ,@HighPrice int,
	@LowCapacityOfEngineID int , @HighCapacityOfEngineID int,
	@LowProducedTimeID int ,@HighProducedTimeID int , 
	@PageNumber int ,
	@PageLength int
)
as
begin
	select COUNT(*) from 
		 tbl_car_data
		 inner join Equipments on tbl_car_data.ID=Equipments.CarID
		 inner join TypeOfCar on TypeOfCar.ID=tbl_car_data.TypeOfCarID
		 inner join Marks on Marks.ID=tbl_car_data.MarkID
		 inner join TypeOfEngine on TypeOfEngine.ID=tbl_car_data.TypeOfEngineID
		 inner join ProducedTime on ProducedTime.ID=tbl_car_data.ProducedTimeID
		 inner join Users on Users.ID=tbl_car_data.UserNameID
		 inner join TypesOfFuel on TypesOfFuel.ID=tbl_car_data.TypesOfFuelID
		 inner join CapacityOfEngine on CapacityOfEngine.ID=tbl_car_data.CapacityOfEngineID
		 inner join Oturucu on Oturucu.ID=tbl_car_data.Oturucu_ID
		 inner join CarModels on CarModels.ID=tbl_car_data.ModelID
		 inner join CarColours on CarColours.ID=Equipments.ColourID
		 left join [ABS] on Equipments.ABS_ID=[ABS].ID
		 left join [Arxa görüntü kamerası] on Equipments.[Arxa görüntü kamerası_ID]=[Arxa görüntü kamerası].ID
		 left join [Dəri salon] on Equipments.[Dəri salon_ID]=[Dəri salon].ID
		 left join Kondisioner on Equipments.Kondisioner_ID=Kondisioner.ID
		 left join [Ksenon lampalar] on Equipments.[Ksenon lampalar_ID]=[Ksenon lampalar].ID
		 left join [Lyuk] on Equipments.Lyuk_ID=Lyuk.ID
		 left join [Mərkəzi qapanma] on Equipments.[Mərkəzi qapanma_ID]=[Mərkəzi qapanma].ID
		 left join [Oturacaqların isidilməsi] on Equipments.[Oturacaqların isidilməsi_ID]=[Oturacaqların isidilməsi].ID
		 left Join [Oturacaqların ventilyasiyası] on Equipments.[Oturacaqların ventilyasiyası_ID]=[Oturacaqların ventilyasiyası].ID
		 left join [Park radarı] on Equipments.[Park radarı_ID]=[Park radarı].ID
		 left join [Yağış sensoru] on Equipments.[Yağış sensoru_ID]=[Yağış sensoru].ID
		 left join [Yan pərdələr] on Equipments.[Yan pərdələr_ID]=[Yan pərdələr].ID
		 left join [Yüngül lehimli disklər] on Equipments.[Yüngül lehimli disklərID]=[Yüngül lehimli disklər].ID
	
	 where tbl_car_data.Confirmed='true'
	
	 and (( not Exists (select 1 from @TypeOfCarIDs)) or Exists(select 1 from @TypeOfCarIDs where tbl_car_data.TypeOfCarID=item))
	 and ((not Exists (select 1 from @MarkIDs)) or Exists(select 1 from @MarkIDs where item=tbl_car_data.MarkID))
	 and ((not Exists (select 1 from @ModelIDs)) or Exists (select 1 from @ModelIDs where item=tbl_car_data.ModelID))
	 and ((not Exists (select 1 from @OturucuIDs)) or Exists (select 1 from @OturucuIDs where item=tbl_car_data.Oturucu_ID))
	 and ((not Exists (select 1 from @TypesOfFuelIDs)) or Exists (select 1 from @TypesOfFuelIDs where item=tbl_car_data.TypesOfFuelID))
	 and ((not Exists (select 1 from @ColourIDs)) or Exists (select 1 from @ColourIDs where item=Equipments.ColourID))
	 and (tbl_car_data.ProducedTimeID between @LowProducedTimeID and @HighProducedTimeID)
	 and (tbl_car_data.Price between @LowPrice and @HighPrice);

	 select 
	 TypeOfCar.NameOfType as [Maşının növü],--1
	 Marks.MarkName as [Maşının markası],--2
	 CarModels.ModelName as [Modeli],--3
     TypeOfEngine.Name as [Mühərrikin növü],--4
	 CapacityOfEngine.Capacity as [Mühərrikin həcmi], --5
	 TypesOfFuel.Name as [Yanacağın növü],--6
	 ProducedTime.Year as [Buraxılış ili],--7
	 Users.Username as [İstifadəçi adı],--8
	 Oturucu.Name as Ötürücü,--9
	 tbl_car_data.Price ,--10
	 CarColours.Colour,--11
	 tbl_car_data.PowerOfEngine,--12

	 case when [ABS].[Text] is null then 'Yoxdur' else [ABS].[Text] end as [ABS],--13
	 case when [Arxa görüntü kamerası].[Text] is null then 'Yoxdur' else [Arxa görüntü kamerası].[Text] end as [Arxa görüntü kamerası],--14
	 case when [Dəri salon].[Text] is null then 'Yoxdur' else [Dəri salon].[Text] end as [Dəri salon],--15
	 case when Kondisioner.[Text] is null then 'Yoxdur' else Kondisioner.[Text] end as Kondisioner,--16
	 case when [Ksenon lampalar].[Text] is null then 'Yoxdur' else [Ksenon lampalar].[Text] end as [Ksenon lampalar],--17
	 case when Lyuk.[Text] is null then 'Yoxdur' else Lyuk.[Text] end as Lyuk,--18
	 case when [Mərkəzi qapanma].[Text] is null then 'Yoxdur' else [Mərkəzi qapanma].[Text] end as [Mərkəzi qapanma],--19
	 case when [Oturacaqların isidilməsi].[Text] is null then 'Yoxdur' else [Oturacaqların isidilməsi].[Text] end as [Oturacaqların isidilməsi],--20
	 case when [Oturacaqların ventilyasiyası].[Text] is null then 'Yoxdur' else [Oturacaqların ventilyasiyası].[Text] end as [Oturacaqların ventilyasiyası],--21
	 case when [Park radarı].[Text] is null then 'Yoxdur' else [Park radarı].[Text] end as [Park radarı],--22
	 case when [Yağış sensoru].[Text] is null then 'Yoxdur' else [Yağış sensoru].[Text] end as [Yağış sensoru],--23
	 case when [Yan pərdələr].[Text] is null then 'Yoxdur' else [Yan pərdələr].[Text] end as [Yan pərdələr],--24
	 case when [Yüngül lehimli disklər].[Text] is null then 'Yoxdur' else [Yüngül lehimli disklər].[Text] end as [Yüngül lehimli disklər]--25
	 from tbl_car_data
	 inner join Equipments on tbl_car_data.ID=Equipments.CarID
     inner join TypeOfCar on TypeOfCar.ID=tbl_car_data.TypeOfCarID
     inner join Marks on Marks.ID=tbl_car_data.MarkID
     inner join TypeOfEngine on TypeOfEngine.ID=tbl_car_data.TypeOfEngineID
     inner join ProducedTime on ProducedTime.ID=tbl_car_data.ProducedTimeID
     inner join Users on Users.ID=tbl_car_data.UserNameID
     inner join TypesOfFuel on TypesOfFuel.ID=tbl_car_data.TypesOfFuelID
     inner join CapacityOfEngine on CapacityOfEngine.ID=tbl_car_data.CapacityOfEngineID
     inner join Oturucu on Oturucu.ID=tbl_car_data.Oturucu_ID
	 inner join CarColours on CarColours.ID=Equipments.ColourID
     inner join CarModels on CarModels.ID=tbl_car_data.ModelID
	 left join [ABS] on Equipments.ABS_ID=[ABS].ID
	 left join [Arxa görüntü kamerası] on Equipments.[Arxa görüntü kamerası_ID]=[Arxa görüntü kamerası].ID
	 left join [Dəri salon] on Equipments.[Dəri salon_ID]=[Dəri salon].ID
	 left join Kondisioner on Equipments.Kondisioner_ID=Kondisioner.ID
	 left join [Ksenon lampalar] on Equipments.[Ksenon lampalar_ID]=[Ksenon lampalar].ID
	 left join [Lyuk] on Equipments.Lyuk_ID=Lyuk.ID
	 left join [Mərkəzi qapanma] on Equipments.[Mərkəzi qapanma_ID]=[Mərkəzi qapanma].ID
	 left join [Oturacaqların isidilməsi] on Equipments.[Oturacaqların isidilməsi_ID]=[Oturacaqların isidilməsi].ID
	 left Join [Oturacaqların ventilyasiyası] on Equipments.[Oturacaqların ventilyasiyası_ID]=[Oturacaqların ventilyasiyası].ID
	 left join [Park radarı] on Equipments.[Park radarı_ID]=[Park radarı].ID
	 left join [Yağış sensoru] on Equipments.[Yağış sensoru_ID]=[Yağış sensoru].ID
	 left join [Yan pərdələr] on Equipments.[Yan pərdələr_ID]=[Yan pərdələr].ID
	 left join [Yüngül lehimli disklər] on Equipments.[Yüngül lehimli disklərID]=[Yüngül lehimli disklər].ID
	 where tbl_car_data.Confirmed='true'
	
	 and (( not Exists (select 1 from @TypeOfCarIDs)) or Exists(select 1 from @TypeOfCarIDs where tbl_car_data.TypeOfCarID=item))
	 and ((not Exists (select 1 from @MarkIDs)) or Exists(select 1 from @MarkIDs where item=tbl_car_data.MarkID))
	 and ((not Exists (select 1 from @ModelIDs)) or Exists (select 1 from @ModelIDs where item=tbl_car_data.ModelID))
	 and ((not Exists (select 1 from @OturucuIDs)) or Exists (select 1 from @OturucuIDs where item=tbl_car_data.Oturucu_ID))
	 and ((not Exists (select 1 from @TypesOfFuelIDs)) or Exists (select 1 from @TypesOfFuelIDs where item=tbl_car_data.TypesOfFuelID))
	 and ((not Exists (select 1 from @ColourIDs)) or Exists (select 1 from @ColourIDs where item=Equipments.ColourID))

	 and (tbl_car_data.ProducedTimeID between @LowProducedTimeID and @HighProducedTimeID)
	 and (tbl_car_data.Price between @LowPrice and @HighPrice)
	 order by tbl_car_data.ID  desc offset @PageNumber * @PageLength rows fetch next @PageLength Rows only;
end
go
create table CarColours
(
	ID int not null primary key identity(1,1),
	Colour nvarchar(50) not null
)
go
insert into CarColours  values ('Qırmızı')
insert into CarColours  values ('Qara')
insert into CarColours  values ('Ag')
insert into CarColours  values ('Sarı')
insert into CarColours  values ('Göy')
insert into CarColours  values ('Mavi')
alter table Equipments add ColourID int
alter table  tbl_car_data drop  column ColourID
alter table Equipments add constraint FK_ColourID_ID foreign key  (ColourID) references CarColours(ID)
alter table tbl_car_data add PowerOfEngine int 
go 