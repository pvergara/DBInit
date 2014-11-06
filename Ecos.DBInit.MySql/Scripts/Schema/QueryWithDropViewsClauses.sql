SELECT 
	CONCAT('DROP VIEW IF EXISTS ', table_name, ';')
FROM 
	information_schema.VIEWS 
WHERE
	table_schema = '_DATABASE_NAME_';