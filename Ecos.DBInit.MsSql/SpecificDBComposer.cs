using System.Collections.Generic;
using System.Linq;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.MsSql
{
    public class SpecificDBComposer:ISpecificDBComposer
    {
        //TODO: Refactor repited code
        public virtual IEnumerable<Script> ComposeScriptsDelete(IEnumerable<string> tableNames)
        {
            return tableNames.
                Select(tableName =>
                    Script.From(string.Format("DELETE FROM [{0}];", tableName))
                );
        }

        public Script ComposeActivateReferentialIntegrity()
        {
            return Script.From("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'\nGO");
        }

        public Script ComposeDeactivateReferentialIntegrity()
        {
            return Script.From("EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';");
        }

        public virtual IEnumerable<Script> ComposeScriptsDropTables(IEnumerable<string> tableNames)
        {
            var result = new List<Script>();
            result.Add(ComposeScriptThatDropsAllConstraints());
            result.AddRange(tableNames.
                Select(objectName =>
                    Script.From(string.Format("" +
                        "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}') AND type in (N'U'))" +
                            "\n DROP {0} [{1}];\n" +
                        "GO",
                        "TABLE", objectName))));
            return result;
        }

        private static Script ComposeScriptThatDropsAllConstraints()
        {
            const string megaScript = @"
                create table #dropConstraintsStatements(
	                statement nvarchar(max)
                );

                EXEC sp_MSForEachTable '
                INSERT INTO
	                #dropConstraintsStatements
                (
	                [statement]
                )
                SELECT 
                    ''ALTER TABLE '' +  OBJECT_SCHEMA_NAME(parent_object_id) +
                    ''.['' + OBJECT_NAME(parent_object_id) + 
                    ''] DROP CONSTRAINT '' + name
                FROM sys.foreign_keys
                WHERE referenced_object_id = object_id(''?'')
                '
                DECLARE @cur_statement NVARCHAR(2000)
                DECLARE db_cursor CURSOR FOR select [statement] from #dropConstraintsStatements
                OPEN db_cursor   
                FETCH NEXT FROM db_cursor INTO @cur_statement
                WHILE @@FETCH_STATUS = 0 
                BEGIN
	                EXECUTE sp_executesql @cur_statement;
	                FETCH NEXT FROM db_cursor INTO @cur_statement
                END

                CLOSE db_cursor   
                DEALLOCATE db_cursor

                DROP table #dropConstraintsStatements;
                ";
            return Script.From(megaScript);
        }

        public virtual IEnumerable<Script> ComposeScriptsDropViews(IEnumerable<string> viewNames)
        {
            return viewNames.
                Select(objectName =>
                    Script.From(string.Format("" +
                        "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}') AND type in (N'V'))" +
                            "\n DROP {0} [{1}];\n" +
                        "GO",
                        "VIEW", objectName)));
        }

        public virtual IEnumerable<Script> ComposeScriptsDropStoredProcedures(IEnumerable<string> storedProcedureNames)
        {
            return storedProcedureNames.
                Select(objectName =>
                    Script.From(string.Format("" +
                        "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}') AND type in (N'P', N'PC'))" + 
                            "\n DROP {0} [{1}];\n" +
                        "GO", 
                        "PROCEDURE", objectName)));
        }

        public virtual IEnumerable<Script> ComposeScriptsDropFunctions(IEnumerable<string> functionNames)
        {
            return functionNames.
                Select(objectName =>
                    Script.From(string.Format("" +
                        "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}') AND type in (N'AF', N'FN', N'TF'))" +
                            "\n DROP {0} [{1}];\n" +
                        "GO",
                        "FUNCTION", objectName)));

        }

    }
}
