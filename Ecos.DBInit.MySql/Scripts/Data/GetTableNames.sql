SELECT 
    table_name
FROM 
	information_schema.tables
WHERE
	table_schema = '_DATABASE_NAME_'   
        AND
    TABLE_TYPE = 'BASE TABLE';