using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using OptimizeQueryWithBenchmark.Services;
using System;
using System.IO;

namespace OptimizeQueryWithBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkService>();
            //InitializeDatabase();
        }


        public static void InitializeDatabase()
        {
            string postgreSqlConnectionString = @"Server=ANKZRVBLK23241\ZRVSQL2014;Database=OptimizeQueryWithBenchmark;Trusted_Connection=True;Integrated Security=true;MultipleActiveResultSets=true";
            string direction = Environment.CurrentDirectory;
            string path = Path.Combine(Directory.GetParent(direction).Parent.Parent.FullName, @"databasescript.sql");
            string script = File.ReadAllText(path);

            SqlConnection connection = new SqlConnection(postgreSqlConnectionString);
            Server server = new Server(new ServerConnection(connection));
            server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}
