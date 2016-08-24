using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQtoSQLTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            FifthSemesterDataContext dbx = new FifthSemesterDataContext();
            Console.WriteLine("Students: ");
            var students = dbx.Students.ToList();
            students.ForEach(s => Console.WriteLine("\t" + s.name));

            Console.WriteLine("Years:");
            var years = dbx.Years.ToList();
            years.ForEach(y => Console.WriteLine("\t" + y.value));
            Console.Read();
        }
    }
}