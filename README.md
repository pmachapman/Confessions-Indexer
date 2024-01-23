# Creeds and Confessions Indexer
An application for generating the full text search database for Conglomo's Creeds and Confessions app.

## Supported databases

* SQLite
* Microsoft SQL Server
* MySQL
* MariaDB

## Usage

`INDEX [database type] "[drive:][path]" "[ConnectionString]"`

## Usage Notes
 * If a path is not specified, the index is generated for the current working directory.
 * The path is either the [website directory](https://github.com/pmachapman/Confessions), or the Android app's assets directory.
 * If the connection string contains a clear identifier to the database file type (i.e. .db, .mdf), you may not need to specify the database type
 * Parameters can be entered in any order, so long as they are correctly formed
 * The error code 0 will be emitted on success, 1 on an error

## Examples

`INDEX MSSQL "Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Confessions;Integrated Security=SSPI;MultipleActiveResultSets=true;"`

`INDEX SQLite "Data Source=C:\Confessions\confessions.db" "C:\Confessions"`

`INDEX MariaDB "C:\Confessions" "Server=localhost;Database=confessions;User=root;Password=;"`

`INDEX MySQL "Server=localhost;Database=confessions;User=root;Password=;"`

`INDEX "Data Source=D:\confessions.db"`

# Final Notes

 * This app is Copyright 2021-2024 Conglomo Limited. Please see [LICENSE.md](LICENSE.md) for license details.
 * The app is still under development, so if you wish to add features, feel free to make a pull request.
 * I provide no warranty or support for this app or its code. Use at your own risk.