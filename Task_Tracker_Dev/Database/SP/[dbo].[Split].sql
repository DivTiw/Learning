/****** Object:  UserDefinedFunction [dbo].[Split]    Script Date: 05-01-2021 16:57:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
select * from 
Split(',', '1,,2,,3,,4,,5,,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10')
*/
CREATE FUNCTION [dbo].[Split] (@sep varchar(2), @s varchar(8000))
RETURNS table
AS
RETURN (
    WITH Pieces(pn, start, stop) 
	AS 
	(
      SELECT 1, 1, CHARINDEX(@sep, @s)
		  UNION ALL
      SELECT pn + case when len(@sep)= 1 then 1 else 2 end, 
			stop + case when len(@sep)= 1 then 1 else 2 end, 
			CHARINDEX(@sep, @s, stop + case when len(@sep)= 1 then 1 else 2 end)
      FROM Pieces
      WHERE stop > 0
    )
    SELECT pn [pk],
      SUBSTRING(@s, start, CASE WHEN stop > 0 THEN stop-start ELSE 8000 END) AS val
    FROM Pieces
	--OPTION(MAXRECURSION 500)
  )
