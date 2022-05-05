select * from  release_instructions

ALTER TABLE release_instructions
ADD Remarks VARCHAR(MAX),
	is_Released BIT DEFAULT(0)