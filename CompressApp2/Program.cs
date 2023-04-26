using Microsoft.Data.Sqlite;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CompressApp2
{
    internal class Program
    {
        static int? count1 = 0, count2 = 0;
        static void Main(string[] args)
        {
            int counter = 0, recordCount;
           
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

                delTableCmd.CommandText = "DROP TABLE IF EXISTS table2";
                delTableCmd.ExecuteNonQuery();

                delTableCmd.CommandText = "DROP TABLE IF EXISTS mapTable";
                delTableCmd.ExecuteNonQuery();

                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = "CREATE TABLE table1(name VARCHAR(50), id VARCHAR(50), phone VARCHAR(50), country VARCHAR(50))";
                createTableCmd.ExecuteNonQuery();
                createTableCmd.CommandText = "CREATE TABLE table2(name VARCHAR(50), id VARCHAR(50), phone VARCHAR(50), country VARCHAR(50))";
                createTableCmd.ExecuteNonQuery();
                createTableCmd.CommandText = "CREATE TABLE mapTable(key VARCHAR(50), value VARCHAR(50))";
                createTableCmd.ExecuteNonQuery();

                Console.WriteLine("Enter Number of Employee Records");
                recordCount = Convert.ToInt32(Console.ReadLine());

                for(int i = 0; i < recordCount; i++)
                {
                    InsetToDB(connection, counter); // Insert employee data to DB
                    counter++;
                }
                ReadDB(connection);    // Read employee data from DB

            }
        }

        static void InsetToDB(SqliteConnection connection, int counter)
        {
            Employee Emp = TakeInput();
            string mapId = GetValue(connection, Emp.Id);
            if(mapId == "") 
            {
                mapId =  CreateValue(connection, Emp.Id, counter);
            }


            //Seed some data:
            using (var transaction = connection.BeginTransaction())
            {
                var insertCmd = connection.CreateCommand();

                insertCmd.CommandText = "INSERT INTO table1 VALUES('" + Emp.Name + "','" + mapId + "','" + Emp.phone + "','" + Emp.country + "')";
                insertCmd.ExecuteNonQuery();

                transaction.Commit();
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
            count1 += employee.Id?.Length + employee.Name?.Length + employee.country?.Length + employee.phone?.Length;
            return employee;
        }

        static void PrintEmployee(Employee emp)
        {
            Console.WriteLine("Phone :" + emp.phone);
            Console.WriteLine("Id :" + emp.Id);
            Console.WriteLine("country :" + emp.country);
            Console.WriteLine("Name :" + emp.Name);
            Console.WriteLine("");
        }

        static void ReadDB(SqliteConnection connection)
        {
            var Employee = new Employee();

            //Read the newly inserted data:
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM table1";

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Employee.Name = reader.GetString(0);
                    Employee.Id = reader.GetString(1);
                    Employee.phone = reader.GetString(2);
                    Employee.country = reader.GetString(3);

                    Console.WriteLine("Before Decompression");
                    PrintEmployee(Employee);
                    count2 += Employee.Id?.Length + Employee.Name?.Length + Employee.country?.Length + Employee.phone?.Length;

                    Employee.Id = GetKeyFromValue(connection, Employee.Id);
                    Console.WriteLine("After Decompression");

                    PrintEmployee(Employee);
                }
            }
            float cr = (float)(count1 - count2) / (float)count1;
            Console.WriteLine("Compression Ratio = " + cr);
        }

        static string GetValue(SqliteConnection connection, string? key)
        {
            string value = "";
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT value FROM mapTable WHERE key = '" + key + "'";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    value = reader.GetString(0);
                }
            }
            return value;
        }

        static string CreateValue(SqliteConnection connection, string? key, int counter)
        {
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO mapTable VALUES('" + key + "','" + counter.ToString() + "')";
            insertCmd.ExecuteNonQuery();

            return counter.ToString();
        }

        static string GetKeyFromValue(SqliteConnection connection, string? value)
        {
            string key = "";
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT key FROM mapTable WHERE value = '" + value + "'";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    key = reader.GetString(0);
                }
            }
            return key;
        }
    }
}