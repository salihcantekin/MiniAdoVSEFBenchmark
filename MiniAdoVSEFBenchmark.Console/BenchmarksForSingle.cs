using BenchmarkDotNet.Attributes;
using Bogus;
using MiniAdoVSEFBenchmark.Console.EF;
using MiniAdoVSEFBenchmark.Console.Models;
using System.Data.SqlClient;

namespace MiniAdoVSEFBenchmark.Console
{
    [SimpleJob(launchCount: 10, warmupCount: 1, targetCount: 1, id: "Sinle Insert Benchmark")]
    public class BenchmarksForSingle
    {
        private List<TestTable> table;
        private int rowCount = 10_000;

        [GlobalSetup]
        public void Setup()
        {
            table = DataProvider.GenerateRecords(rowCount);
        }

        public TestTable GetRandomRecord() => table[new Random().Next(0, rowCount - 1)];

        [Benchmark(Baseline = true)]
        public async Task InsertSingleWithADO()
        {
            string sql = "INSERT INTO TestTable(Id, FirstName, LastName) VALUES(@Id, @FirstName, @LastName)";
            var data = GetRandomRecord();

            using var connection = new SqlConnection(Constants.ConnectionString);
            await connection.OpenAsync();

            var cmmnd = connection.CreateCommand();
            cmmnd.CommandText = sql;
            cmmnd.Parameters.Add(new SqlParameter() { ParameterName = "Id", Value = data.Id });
            cmmnd.Parameters.Add(new SqlParameter() { ParameterName = "FirstName", Value = data.FirstName });
            cmmnd.Parameters.Add(new SqlParameter() { ParameterName = "LastName", Value = data.LastName });

            await cmmnd.ExecuteNonQueryAsync();
            connection.Close();
        }


        [Benchmark]
        public async Task InsertSingleWithEF()
        {
            using var dbContext = new AppDbContext();
            await dbContext.TestTable.AddAsync(GetRandomRecord());
            await dbContext.SaveChangesAsync();
        }

    }
}
