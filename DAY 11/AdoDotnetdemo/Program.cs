// See https://aka.ms/new-console-template for more information
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;


var connectionString = @"Server=localhost\SQLEXPRESS;
Database=CrmDb;
Trusted_Connection=True;
TrustServerCertificate=True;";

using var connection = new SqlConnection(connectionString);

try
{
    connection.Open();
    Console.WriteLine("Connection opened successfully");

    // SqlCommand command = new SqlCommand(
    //     "SELECT EmpID, EmpName, Age, Department FROM Employee",
    //     connection);

    // SqlDataReader reader = command.ExecuteReader();

    // while (reader.Read())
    // {
    //     int EmpID = reader.GetInt32(0);
    //     string EmpName = reader.GetString(1);
    //     int Age = reader.GetInt32(2);
    //     string Department = reader.GetString(3);

    //     Console.WriteLine($"{EmpID}\t{EmpName}\t{Age}\t{Department}");
    // }

    // reader.Close();

    // ExecuteScalar – Count Employees
    // ExecuteScalar(connection);

     // ExecuteReader – Read Employees
    // ExecuteReader(connection);

    // ExecuteNonQuery – Insert Employee
    // ExecuteNonQuery(connection);

    // SqlDataAdapterDemo(connection);

    // Insert Customer Demo
    // InsertEmployeeDemo(connection);

    // SQL Injection Demo
    // SqlInjectionDemo(connection);

    // Parameterized Query Demo
    ParameterizedQueryDemo(connection);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    connection.Close();
}

void ExecuteScalar(SqlConnection connection)
{
    var query = "SELECT COUNT(*) FROM Employee";

    using var command = new SqlCommand(query, connection);
    var count = (int)command.ExecuteScalar();

    Console.WriteLine($"Total Employees: {count}\n");
}

void ExecuteReader(SqlConnection connection)
{
    var query = "SELECT * FROM Employee WHERE Age > 25";

    using var command = new SqlCommand(query, connection);
    using var reader = command.ExecuteReader();

    Console.WriteLine("Employees with Age > 25:\n");

    while (reader.Read())
    {
        Console.WriteLine($"{reader["EmpID"]}\t{reader["EmpName"]}\t{reader["Age"]}\t{reader["Department"]}");
    }

    Console.WriteLine();
}

void ExecuteNonQuery(SqlConnection connection)
{
    var query = "INSERT INTO Employee (EmpName, Age, Department) VALUES ('Neha', 29, 'HR')";

    using var command = new SqlCommand(query, connection);
    var rowsAffected = command.ExecuteNonQuery();

    Console.WriteLine($"Rows affected after insert: {rowsAffected}");
}

void SqlDataAdapterDemo(SqlConnection connection)
{
    var query = "SELECT * FROM Employee";

    SqlCommand sqlCommand = new SqlCommand(query, connection);

    using var adapter = new SqlDataAdapter(sqlCommand);

    var employeeDataTable = new DataTable();

    adapter.Fill(employeeDataTable);

    Console.WriteLine("\nData using SqlDataAdapter + DataTable:\n");

    foreach (DataRow row in employeeDataTable.Rows)
    {
        Console.WriteLine($"Id: {row["EmpID"]}, Name: {row["EmpName"]}, Age: {row["Age"]}, Dept: {row["Department"]}");
    }
}

void InsertEmployeeDemo(SqlConnection connection)
{
    var dataSet = new DataSet();

    var selectQuery = "SELECT * FROM Employee";

    using var selectCommand = new SqlCommand(selectQuery, connection);
    using var adapter = new SqlDataAdapter(selectCommand);

    adapter.Fill(dataSet, "Employee");

    var dataTable = dataSet.Tables["Employee"];

    // Create new row
    var newRow = dataTable.NewRow();
    newRow["EmpName"] = "Priya";
    newRow["Age"] = 26;
    newRow["Department"] = "HR";

    dataTable.Rows.Add(newRow);

    // InsertCommand (Parameter Mapping)
    adapter.InsertCommand = new SqlCommand(
        "INSERT INTO Employee (EmpName, Age, Department) VALUES (@EmpName, @Age, @Department)",
        connection);

    adapter.InsertCommand.Parameters.Add("@EmpName", SqlDbType.VarChar, 50, "EmpName");
    adapter.InsertCommand.Parameters.Add("@Age", SqlDbType.Int, 0, "Age");
    adapter.InsertCommand.Parameters.Add("@Department", SqlDbType.VarChar, 50, "Department");

    // Push changes to database
    adapter.Update(dataSet, "Employee");

    Console.WriteLine("Employee inserted successfully using DataSet.\n");

}

void SqlInjectionDemo(SqlConnection connection)
{
    // Query Example:
    // SELECT * FROM Employee WHERE EmpID = 1 OR 1 = 1

    var userInput = "1 OR 1 = 1";
    // var userInput = "1; DROP TABLE Employee;";
    // var userInput = "3";

    var query = $"SELECT * FROM Employee WHERE EmpID = {userInput}";

    using var command = new SqlCommand(query, connection);

    try
    {
        using var reader = command.ExecuteReader();

        // Console.WriteLine("\nSQL Injection Demo Result:\n");

        while (reader.Read())
        {
            Console.WriteLine($"ID: {reader["EmpID"]}, " +
                              $"Name: {reader["EmpName"]}, " +
                              $"Age: {reader["Age"]}, " +
                              $"Department: {reader["Department"]}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error executing query: {ex.Message}");
    }
}


void ParameterizedQueryDemo(SqlConnection connection)
{
    using (SqlCommand command = new SqlCommand(
        "SELECT * FROM Employee WHERE EmpName LIKE @EmpName",
        connection))
    {
        // var name = "Sakshi";
        // var name = "Sakshi OR 1 = 1";
        // var name = "Sakshi'; DROP TABLE Employee; --";

        // Add parameters - database treats them as DATA, never as SQL code
        var name = "Sakshi OR 1 = 1";

        command.Parameters.AddWithValue("@EmpName", name);

        using SqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            Console.WriteLine($"ID: {reader["EmpID"]}, " +
                              $"Name: {reader["EmpName"]}, " +
                              $"Age: {reader["Age"]}, " +
                              $"Department: {reader["Department"]}");
        }
        else
        {
            Console.WriteLine("No employee found with the specified Name.");
        }
    }
}
