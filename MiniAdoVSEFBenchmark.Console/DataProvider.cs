using Bogus;
using MiniAdoVSEFBenchmark.Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAdoVSEFBenchmark.Console
{
    public class DataProvider
    {
        public static List<TestTable> GenerateRecords(int count)
        {
            var faker = new Faker<TestTable>("tr")
                .RuleFor(i => i.Id, i => new Random().Next(0, count - 1))
                .RuleFor(i => i.FirstName, i => i.Person.FirstName)
                .RuleFor(i => i.LastName, i => i.Person.LastName);

            return faker.Generate(count).ToList();
        }
    }
}
