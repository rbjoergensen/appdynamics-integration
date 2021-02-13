CREATE TABLE [AppdynamicsTest].[dbo].[WeatherTypes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[WeatherType] [varchar](64) NOT NULL,
)
INSERT [AppdynamicsTest].[dbo].[WeatherTypes] (WeatherType)
    VALUES ('Damp')
INSERT [AppdynamicsTest].[dbo].[WeatherTypes] (WeatherType)
    VALUES ('Sunny')
USE master