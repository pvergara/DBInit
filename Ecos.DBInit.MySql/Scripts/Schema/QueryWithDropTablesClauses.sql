SELECT 
	CONCAT('DROP TABLE IF EXISTS ', table_name, ';')
FROM 
	information_schema.tables
WHERE
	table_schema = '_DATABASE_NAME_';