/****** Object:  UserDefinedFunction [dbo].[fun_CSVtoTable]    Script Date: 05-01-2021 16:57:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[fun_CSVtoTable](
 @csv nvarchar (4000),
 @Delimiter nvarchar (10)
 )
returns @returnValue table ([Item] nvarchar(4000))
begin
 declare @NextValue nvarchar(4000)
 declare @Position int
 declare @NextPosition int
 declare @comma nvarchar(1) 
 set @NextValue = ''
 set @comma = right(@csv,1)  
 set @csv = @csv + @Delimiter 
 set @Position = charindex(@Delimiter,@csv)
 set @NextPosition = 1 
 while (@Position <>  0)  
 begin
  set @NextValue = substring(@csv,1,@Position - 1) 
  insert into @returnValue ( [Item]) Values (@NextValue) 
  set @csv = substring(@csv,@Position +1,len(@csv))  
  set @NextPosition = @Position
  set @Position  = charindex(@Delimiter,@csv)
 end 
 return
end