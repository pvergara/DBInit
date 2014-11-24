DBInit
======

The project aims to provide the initialization of data or databases in order to use it on integration tests

The goals of the first stable version are:

  * The project must be used over several Database engines
  * The project must have a wide range of extension points in order to easily customize.
  * The project must have some kind of mechanism to init the schema (database) only when the system detect that is necessary

How to start
======

You must have database created (e.g. 'sakila'). You can use the scripts included on the sakila-db folder or create the database from scratch.
If the database is empty you must to run the test that has this piece of code (at the moment the test calls "WhenIUseInitSchemaAllTheTablesWillBeEmpty"):

            //Act
            _dbInit.InitSchema();
