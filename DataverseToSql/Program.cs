using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

if (args.Contains("--help") || args.Contains("-h"))
{
    PrintHelp();
    return;
}

string connectionString = GetParameterValue(args, "--connection");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Error: The connection string is required.");
    return;
}

string xmlPath = GetParameterValue(args, "--xml");
if (string.IsNullOrEmpty(xmlPath))
{
    Console.WriteLine("Error: The XML file path is required.");
    return;
}

if (!File.Exists(xmlPath))
{
    Console.WriteLine($"Error: The file '{xmlPath}' does not exist.");
    return;
}

var entities = LoadEntities(xmlPath);
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    foreach (var entity in entities)
    {
        string createTableSql = GenerateCreateTableSql(entity);
        Console.WriteLine(createTableSql);

        using (SqlCommand command = new SqlCommand(createTableSql, connection))
        {
            command.ExecuteNonQuery();
        }
    }
}

Console.WriteLine("Database generation complete. Press any key to exit ...");
Console.ReadKey();

static void PrintHelp()
{
    Console.WriteLine("Usage: DataverseToSql [options]");
    Console.WriteLine("Options:");
    Console.WriteLine("  --help, -h           Show this help message and exit");
    Console.WriteLine("  --connection         SQL connection string");
    Console.WriteLine("  --xml                Path to the customizations.xml file");
    Console.WriteLine();
    Console.WriteLine("Description:");
    Console.WriteLine("  This utility reads a customizations.xml file containing metadata about entities and generates");
    Console.WriteLine("  a SQL database schema. It connects to a specified SQL Server instance and creates tables based");
    Console.WriteLine("  on the entity definitions in the XML file. Users can specify the connection string and XML file");
    Console.WriteLine("  path via command-line arguments.");
}

static string GetParameterValue(string[] args, string parameterName)
{
    int index = Array.IndexOf(args, parameterName);
    return index >= 0 && index < args.Length - 1 ? args[index + 1] : null;
}

static List<EntityModel> LoadEntities(string customizationsXml)
{
    var xdoc = XDocument.Load(customizationsXml);
    return xdoc.Descendants("Entity")
        .Select(entity => new EntityModel
        {
            LogicalName = (string)entity.Element("Name"),
            Fields = entity.Descendants("attribute")
                .Select(attribute => new EntityField
                {
                    LogicalName = (string)attribute.Element("LogicalName"),
                    Type = (string)attribute.Element("Type"),
                    MaxLength = (int?)attribute.Element("MaxLength") ?? 250,
                    RequiredLevel = (string)attribute.Element("RequiredLevel")
                }).ToList()
        }).ToList();
}

static string GenerateCreateTableSql(EntityModel entity)
{
    var sql = $"CREATE TABLE {entity.LogicalName} (";
    var primaryKey = string.Empty;

    foreach (var field in entity.Fields)
    {
        string columnName = field.LogicalName;
        string columnType = GetSqlType(field);
        string nullable = field.RequiredLevel == "systemrequired" ? "NOT NULL" : "NULL";

        if (field.Type == "primarykey")
        {
            primaryKey = columnName;
        }

        sql += $"{columnName} {columnType} {nullable}, ";
    }

    sql = primaryKey != string.Empty
        ? sql + $"PRIMARY KEY ({primaryKey})"
        : sql.TrimEnd(',', ' ');

    sql += ");";
    return sql;
}

static string GetSqlType(EntityField field)
{
    return field.Type.ToLower() switch
    {
        "nvarchar" => $"nvarchar({field.MaxLength})",
        "int" => "int",
        "datetime" => "datetime",
        "uniqueidentifier" => "uniqueidentifier",
        _ => "nvarchar(250)"
    };
}

public class EntityModel
{
    public string LogicalName { get; set; }
    public List<EntityField> Fields { get; set; } = new List<EntityField>();
}

public class EntityField
{
    public string LogicalName { get; set; }
    public string Type { get; set; }
    public int? MaxLength { get; set; }
    public string RequiredLevel { get; set; }
}
