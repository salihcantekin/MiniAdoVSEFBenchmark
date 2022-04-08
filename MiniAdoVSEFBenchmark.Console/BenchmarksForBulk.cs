using BenchmarkDotNet.Attributes;
using Bogus;
using MiniAdoVSEFBenchmark.Console.EF;
using MiniAdoVSEFBenchmark.Console.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAdoVSEFBenchmark.Console
{
    [SimpleJob(launchCount: 10, warmupCount: 1, targetCount: 1, id: "Sinle Insert Benchmark")]
    public class BenchmarksForBulk
    {
        private List<TestTable> table;
        private int rowCount = 10_000;
        private int bulkRowCount = 5;

        [GlobalSetup]
        public void Setup()
        {
            table = DataProvider.GenerateRecords(rowCount);
        }


        public IEnumerable<TestTable> GetRandomRecords(int count) => table.OrderBy(i => Guid.NewGuid()).Take(count);


        [Benchmark]
        public async Task BulkInsertWithADO()
        {
            string sql = "INSERT INTO TestTable(Id, FirstName, LastName) VALUES(@Id, @FirstName, @LastName)";

            using var connection = new SqlConnection(Constants.ConnectionString);
            await connection.OpenAsync();

            var records = GetRandomRecords(bulkRowCount);

            foreach (var item in records)
            {
                var cmmnd = connection.CreateCommand();
                cmmnd.CommandText = sql;
                cmmnd.Parameters.Add(new SqlParameter() { ParameterName = "Id", Value = item.Id });
                cmmnd.Parameters.Add(new SqlParameter() { ParameterName = "FirstName", Value = item.FirstName });
                cmmnd.Parameters.Add(new SqlParameter() { ParameterName = "LastName", Value = item.LastName });

                await cmmnd.ExecuteNonQueryAsync();
            }


            connection.Close();
        }


        [Benchmark]
        public async Task BulkInsertWithEF()
        {
            var records = GetRandomRecords(bulkRowCount);

            var dbContext = new AppDbContext();
            await dbContext.TestTable.AddRangeAsync(records);
            await dbContext.SaveChangesAsync();
            
        }

    }
}
