SELECT 
	CONCAT('DROP FUNCTION IF EXISTS ', routine_name, ';')
FROM 
	information_schema.ROUTINES 
WHERE
	ROUTINE_SCHEMA = '_DATABASE_NAME_' 
		AND
	ROUTINE_TYPE = 'FUNCTION';