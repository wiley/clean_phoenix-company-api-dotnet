# If not using a Docker Container for UsersAPI Development

In order to use the shared URIs in the AppSettings.Development.json file simply add the following two lines to your c:/windows/system32/drivers/etc/hosts file:

```
127.0.0.1 companyapi-db
```

# Setting up the Database
* Simply running the application will create the MySQL database structure.  However, the database "CompanyAPI"(for Development) must exist prior to first execution, because the API uses the connection string to know where to put the data and the database name is fixed.  The database name might be different if you are using an environment variable for a container.
* If you want the database pre-populated with Crunchbase data, then you'll have to initiate that import. To import from CSV file, run the application from a terminal window using the <code>--import</code> command-line argument:
```
dotnet run --import [filename]</pre>
```
where filename is the CSV path (default locatation is \Crunchbase\_csv\).
* Finally, run ./Crunchbase/ModifySchema.sql against the database. (If you are going to add Crunchbase data, do this <i>after</i> the initial Crunchbase import has completed.  Otherwise this step still needs to be completed.)
