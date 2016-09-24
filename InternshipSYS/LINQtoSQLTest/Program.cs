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
            students.ForEach(s => Console.WriteLine("\t" + s.id + " " + s.name + " " + s.year + " " + s.season));

            Console.WriteLine("Companies: ");
            var comps = dbx.Companies.ToList();
            comps.ForEach(c => Console.WriteLine("\t" + c.id + " " + c.name));
            //Console.WriteLine("Years:");
            //var years = dbx.Years.ToList();
            //years.ForEach(y => Console.WriteLine("\t" + y.value));
            //Console.Read();

            List<Student> temp1 = new List<Student>();
            Int32 year = 2016;
            temp1 = dbx.Students.Where(s => s.year.Equals(year)).ToList();

            Console.WriteLine("Students in 2016");
            temp1.ForEach(s => Console.WriteLine("\t" + s.name + " Class: " + s.@class + " CompanyId: " + s.CompanyID));

            var temp2 = temp1.Where(s => s.CompanyID == null || s.Company.name.Equals("Erhvervsakademi Aarhus")).ToList();
            Console.WriteLine("Students in 2016 who have EAAA or no one listed as company");
            temp2.ForEach(s => Console.WriteLine("\t" + s.name));

            var temp3 = temp1.GroupBy(s => s.season).ToList();
            
            Console.WriteLine("Temp3: " + temp3.Count);
            temp3.ForEach(t => t.ToList().ForEach(s => Console.WriteLine("\t" + s.name + " " + s.season)));

            var temp4 = temp3
                .Select(list => new { Year = list.FirstOrDefault().year, Season = list.FirstOrDefault().season, Count = list.Count() })
                .ToList();
            Console.WriteLine("\nTemp4: " + temp4.Count());
            temp4.ForEach(s => Console.WriteLine("\t" + s.Year + " " + s.Season + " " + s.Count));

            var result = dbx.Students
                .Where(s=> s.year.Equals(2016) && s.CompanyID == null || s.Company.name.Equals("Erhvervsakademi Aarhus"))
                .GroupBy(s => s.season)
                .Select(list => new { year = list.FirstOrDefault().year, season = list.FirstOrDefault().season, count = list.Count() })
                .ToList();
            Console.WriteLine("Result: " + result.Count());
             result.ForEach(r => Console.WriteLine("\t" + r.year + " " + r.season + " " + r.count));


            var temp5 = dbx.Students
                .Where(s => s.year.Equals(year) && s.Company.name.Equals("Erhvervsakademi Aarhus"))
                .GroupBy(s => s.season)
                .Select(list => new { year = list.FirstOrDefault().year, season = list.FirstOrDefault().season, count = list.Count()})
                .ToList();
            Console.WriteLine("temp5: ");
            temp5.ForEach(list => Console.WriteLine(list.year + " " + list.season + " " + list.count));

            var mps = dbx.MainProjects
                .Where(mp => mp.year == 2016 && mp.season.Equals("Spring"))
                .ToList();

            var stus = dbx.Students
                            .Where(s => s.year == 2016 && s.season.Equals("Spring"))
                            .ToList();

            var temp7 =
                from s in stus
                join mp in mps on s.mainProjectTitle equals mp.title
                select new { name = s.name, @class = s.@class, groupnr = mp.groupNo, supervisor = mp.Supervisor.name, title = mp.title, company = mp.Company.name };

            temp7 = temp7.OrderBy(s => s.title)
                .ThenBy(s => s.supervisor);

            Console.WriteLine("temp7 Students ordered by main project");
            temp7.ToList()
                .ForEach(s => Console.WriteLine( "\t" + s.name + " " + s?.@class + " Gr: " + s.groupnr + " " + s.title));
            
            var groupby1 = dbx.Students
                //.Where(s => s.CompanyID == null || s.Company.name.Equals("Erhvervsakademi Aarhus"))
                .Where(s => !s.Company.name.Equals("Erhvervsakademi Aarhus"))
                .GroupBy(s => s.season, s => s)
                .ToList();

            Console.WriteLine("\nGroupBy1 size: " + groupby1.Count);
            foreach(IGrouping<string, Student> list in groupby1)
            {
                Console.WriteLine(list.Key);
                foreach(Student st in list)
                {
                    Console.WriteLine("\t {0}", st.name + " " + st.season);
                }
            }

            var groupby2 = dbx.Students.Where(s => !s.Company.name.Equals("Erhvervsakademi Aarhus")).ToList();
            Console.WriteLine("\nGroupBy2 size: " + groupby2.Count);
            foreach(Student st in groupby2)
            {
                Console.WriteLine("\t {0}", st.name + " " + st.season);
            }
            Console.Read();
        }
    }
}