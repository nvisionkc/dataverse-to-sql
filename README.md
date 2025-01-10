# Dataverse to SQL Database Generator

This utility reads a `customizations.xml` file containing metadata about entities and generates SQL database tables based on the definition. It connects to a specified SQL Server instance and creates tables based on the defined entities and fields.

## Features
- Parses `customizations.xml` to extract entity and field metadata.
- Dynamically generates SQL `CREATE TABLE` statements.
- Supports primary key and field-level constraints.
- Configurable database connection and file paths via command-line arguments.

## Usage

### Command-Line Options
```
Usage: DataverseToSql [options]

Options:
  --help, -h           Show this help message and exit
  --connection         SQL connection string
  --xml                Path to the customizations.xml file
```

### Example

- Specify a connection string and the path to the `customizations.xml` file:
  ```
  DataverseToSql --connection "Data Source=myserver;User ID=admin;Password=secret;Database=mydb;" --xml "C:\path\to\customizations.xml"
  ```

## Exporting the Solution from Power Apps

To generate the `customizations.xml` file, you need to export your Power Apps solution as unmanaged:

1. Navigate to [Power Apps](https://make.powerapps.com/).
2. Select your environment from the top-right corner.
3. Click on **Solutions** from the left-hand menu.
4. Select the solution you want to export.
5. Click **Export** and choose the **Unmanaged** option.
6. Once the solution is downloaded, extract the `.zip` file to a folder.
7. Locate the `customizations.xml` file inside the extracted folder.

This file will be used as input for the SQL Database Generator.

## Requirements
- .NET 6.0 SDK or higher
- Access to a SQL Server instance
- A valid `customizations.xml` file

## Building and Running

1. Clone the repository:
   ```
   git clone https://github.com/nvisionkc/dataverse-to-sql.git
   ```

2. Navigate to the project directory:
   ```
   cd dataverse-to-sql
   ```

3. Build the project:
   ```
   dotnet build
   ```

4. Run the application:
   ```
   dotnet run -- [options]
   ```

## Output
The application outputs the generated SQL `CREATE TABLE` statements to the console and executes them against the specified database.

## Notes
- Ensure the SQL Server instance is accessible and the connection string has appropriate permissions.
- Back up your database before running the utility or alter to output the SQL statements to file.

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.

