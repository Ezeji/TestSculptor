# TestSculptor

TestSculptor is a simple library built with the primary aim of solving a common software testing problem - getting rid of test doubles for automated testing(precisely, integration testing) and enhancing the productivity of engineers while testing. Test doubles such as fakes, stubs, mocks.etc appears to be the goto approach for automated testing in most .NET projects. Beyond the books and in reality, adopting either of such approach for automated testing will most times lead to producing tests that aren't reliable due to a good chance of false positives from test results. So with this in mind, can't we invent a tool to help us with testing right?

TestSculptor has two use cases for automated testing:
1. Providing the ability to re-use a single database instance for every test session(this is useful for integration tests where a team could have a dedicated database for testing that closely mirrors the production database). 
2. Providing the ability to use new database instances for every test session(this is useful for unit tests since each database for testing is temporal).

The above mentioned use cases also holds for functional testing where TestSculptor can help with automatic schema update and re-use of a similar/shared database used for automated testing in a local developer environment or automatic schema creation for a separate database instance from an instance used for automated testing.

## How do I get started?

First, create a folder in your system(local or virtual machine) following the below instructions:
1. For Windows: Create a folder named 'TestSculptorScripts' in either one of the directory paths - "C:\Program Files" or "C:\Users\[Username]". Where 'Username = User' for some windows operating system.
2. For MacOS: Create a folder named 'TestSculptorScripts' in the directory path - "/Users/[Username]".
3. For Linux: Create a folder named 'TestSculptorScripts' in the directory path - "/home/[Username]".

After the folder is created, copy the sample script for your database engine of choice found at https://github.com/Ezeji/TestSculptor/tree/master/src/TestSculptor/Scripts and paste that copy into the created folder from the previous step.

Next is to create a folder where your database schemas will live and they'll be referenced from the copied script that lives in the root folder named 'TestSculptorScripts'. The new folder to be created within 'TestSculptorScripts' folder should be named 'schema'. The overall folder structure should look like this:
```plaintext
/TestSculptorScripts
 ├── install-sqlserver.sql    [Copied script for SQL Server]
 └── /schema
     └── [Your database schema files will be placed here]
```

Once the above is done, remember to only edit the copied script to **fit** or **reference** your own database schemas. The paths should be provided from top to bottom whereby new database schemas to be added to the main or copied script should be added ontop of the previous path and not the other way round. Think of this step as a stack of database schema paths.

To carryout automated testing, ensure that you create two classes within your test project. One is for your application's shared fixture which inherits TestSculptor's shared fixture and the other is for your application's shared fixture collection as seen below:

```csharp
public class TestAppSharedFixture : SharedFixture
{
    public TestAppSharedFixture()
    {
        ConnectionString = $"Server={ServerName};Database={TestSculptorDefaults.DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"; // Set your preferred connection string here
        DatabaseEngine = DatabaseEngine.SqlServer;  // Set your preferred database engine here
        ShouldConfigureDatabaseAlways = false;
        ConfigureServices = services =>
        {
            switch (DatabaseEngine)
            {
                case DatabaseEngine.SqlServer:
                    services.AddDbContext<TestAppDbContext>(options =>
                            options.UseSqlServer(ConnectionString));
                    break;

                default:
                    break;
            }

            // Configure your services and repositories here
            services.AddCoreRepository();
            services.AddServices();
        };
    }
}
```
```csharp
[CollectionDefinition(nameof(TestAppSharedFixture))]
public class TestAppSharedFixtureCollection : ICollectionFixture<TestAppSharedFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
```
You can find a detailed guide regarding the needed setup for your test project here: https://github.com/Ezeji/TestSculptor/tree/master/src/TestAppTests/Setup

To carryout functional testing, ensure to configure TestSculptor in the startup of your application as seen below:

```csharp
// Initialize TestSculptor for functional testing
DatabaseRunner? _databaseRunner = new()
{
    ConnectionString = Configuration.GetConnectionString(TestSculptorDefaults.DatabaseName),
    DatabaseEngine = DatabaseEngine.SqlServer,
    ShouldConfigureDatabaseAlways = false
};
_databaseRunner.Start();
```

For more guide on automated and functional testing, refer to the sample projects in this repo. One is a web server named [TestApp](https://github.com/Ezeji/TestSculptor/tree/master/src/TestApp) and the other is a test project named [TestAppTests](https://github.com/Ezeji/TestSculptor/tree/master/src/TestAppTests).

## Where can I get it?

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [TestSculptor](https://www.nuget.org/packages/TestSculptor/) from the package manager console:

```
PM> Install-Package TestSculptor
```
Or from the .NET CLI as:
```
dotnet add package TestSculptor
``` 

## Do you have an issue?

If you find a bug or have a feature request, please report them at this repository's issues section.

## How do I contribute?

Just fork the project, make your changes and send me a PR.  
You can compile the project with Visual Studio 2022 and/or the [.NET Core](https://www.microsoft.com/net/core) CLI! See the [CONTRIBUTING GUIDE](https://git-scm.com/book/en/v2/GitHub-Contributing-to-a-Project) for details on contributing to this project.

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.

For more information, see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.