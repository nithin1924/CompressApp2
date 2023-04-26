using Microsoft.Data.Sqlite;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CompressApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "./SqliteDB.db";
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                //Create a table (drop if already exists first):

                var delTableCmd = connection.CreateCommand();
                delTableCmd.CommandText = "DROP TABLE IF EXISTS table1";
                delTableCmd.ExecuteNonQuery();

                delTableCmd.CommandText = "DROP TABLE IF EXISTS mapTable";
                delTableCmd.ExecuteNonQuery();

                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = "CREATE TABLE table1(name VARCHAR(50), id VARCHAR(50), phone VARCHAR(50), country VARCHAR(50))";
                createTableCmd.ExecuteNonQuery();
                createTableCmd.CommandText = "CREATE TABLE mapTable(key VARCHAR(50), value VARCHAR(50))";
                createTableCmd.ExecuteNonQuery();

                InsetToDB(connection); // Insert employee data to DB
                ReadDB(connection);    // Read employee data from DB

            }
        }

        static Employee TakeInput()
        {
            Employee employee = new Employee();
            Console.WriteLine("Enter Id");
            employee.Id = Console.ReadLine();
            Console.WriteLine("Enter Name");
            employee.Name = Console.ReadLine();
            Console.WriteLine("Enter country");
            employee.country = Console.ReadLine();
            Console.WriteLine("Enter phone");
            employee.phone = Console.ReadLine();
            //Console.WriteLine("insideTakeInput" );
            return employee;
        }

        static void PrintEmployee(Employee emp)
        {
            Console.WriteLine(emp.phone);
            Console.WriteLine(emp.Id);
            Console.WriteLine(emp.country);
            Console.WriteLine(emp.Name);
        }

        static void InsetToDB(SqliteConnection connection)
        {
            var Emp = TakeInput();


            PropertyInfo[] properties = typeof(Employee).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                property.SetValue(Emp, "ok");
            }

            //Seed some data:
            using (var transaction = connection.BeginTransaction())
            {
                var insertCmd = connection.CreateCommand();

                insertCmd.CommandText = "INSERT INTO table1 VALUES('" + Emp.Name + "','" + Emp.Id + "','" + Emp.phone + "','" + Emp.country + "')";
                insertCmd.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        static void ReadDB(SqliteConnection connection)
        {
            var Employee = new Employee();

            //Read the newly inserted data:
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT name FROM table1";

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var message = reader.GetString(0);
                    Console.WriteLine(message);
                }
            }

            PrintEmployee(Employee);
        }

        static string GetValue(SqliteConnection connection)
        {
            string value = "";
            return value;
        }

        static string CreateValue(SqliteConnection connection, string key)
        {
            string value = "";
            return value;
        }

        static string GetKeyFromValue(SqliteConnection connection, string value)
        {
            string key = "";
            return key;
        }
    }
}