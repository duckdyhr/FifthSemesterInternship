using SERVICE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FifthSemester.StatePatter
{
    public abstract class SelectionState
    {
        protected PrintsWindow context;
        public SelectionState(PrintsWindow context)
        {
            this.context = context;
        }
        public abstract void SeasonChanged(int year, string season);
        public abstract void YearChanged(int year, string season);
        public abstract Dictionary<string, string> GetColumns();
        public abstract void SetItemsSource(int year, string season);

        public List<Student> GetStudentList(int year, string season)
        {
            List<Student> studentList = context.GetService().getStudentsByYear(year)
                .ToList();
            if(season != null && !season.Equals(""))
            {
                studentList = studentList
                    .Where(s => s.season.Equals(season))
                    .ToList();
            }
            return studentList;
        }
    }

    public class StudentSupervisorCompanyState : SelectionState
    {
        private static readonly Dictionary<string, string> studentSupervisorColums = new Dictionary<string, string>() {
            {"Name", "name" },
            {"Class", "class" },
            {"Supervisor", "Supervisor.name" },
            {"Email", "email" },
            {"Company", "Company.name" },
            {"Comments", "comments" }
        };

        public StudentSupervisorCompanyState(PrintsWindow context) : base(context)
        { }

        public override Dictionary<string, string> GetColumns()
        {
            return studentSupervisorColums;
        }

        public override void SetItemsSource(int year, string season)
        {
            context.datagrid.ItemsSource = GetStudentList(year, season);
        }

        public override void YearChanged(int year, string season)
        {
            context.datagrid.ItemsSource = GetStudentList(year, season);
        }
        public override void SeasonChanged(int year, string season)
        {
            context.datagrid.ItemsSource = GetStudentList(year, season);
        }
    }
    class StudentCompanyAssignmentState : SelectionState
    {
        private static readonly Dictionary<string, string> studentCompanyColumns = new Dictionary<string, string>()
        {
            {"Name", "name" },
            {"Class", "class"},
            {"Company", "Company.name" },
            {"Comments", "comments" }
        };
        public StudentCompanyAssignmentState(PrintsWindow context) : base(context)
        {
        }

        public override Dictionary<string, string> GetColumns()
        {
            return studentCompanyColumns;
        }

        public override void SetItemsSource(int year, string season)
        {
            context.datagrid.ItemsSource = GetStudentList(year, season);
        }

        public override void SeasonChanged(int year, string season)
        {
            context.datagrid.ItemsSource = GetStudentList(year, season);
        }
        
        public override void YearChanged(int year, string season)
        {
            context.datagrid.ItemsSource = GetStudentList(year, season);
        }
    }

    class StudentsAssignedCompanyState : SelectionState
    {
        private static readonly Dictionary<string, string> studentsPrSeasonColumns = new Dictionary<string, string>()
        {
            {"Year", "year" },
            {"Season", "season" },
            {"Number", "count" }
        };

        public StudentsAssignedCompanyState(PrintsWindow context) : base(context)
        {}

        public override Dictionary<string, string> GetColumns()
        {
            return studentsPrSeasonColumns;
        }
        
        public override void SetItemsSource(int year, string season)
        {
            var result = context.GetService().getStudentsByYear(year)
                .Where(s => s.CompanyID == null || s.Company.name.Equals("Erhvervsakademi Aarhus"))
                .GroupBy(s => s.season)
                .Select(list => new { year = list.FirstOrDefault().year, season = list.FirstOrDefault().season, count = list.Count() })
                .ToList();
            context.datagrid.ItemsSource = result;
        }

        public override void YearChanged(int year, string season)
        {
            SetItemsSource(year, season);
        }
        public override void SeasonChanged(int year, string season)
        {
            SetItemsSource(year, season);
        }
    }

    class StudentsAtEAAA : SelectionState
    {
        private static readonly Dictionary<string, string> studentsPrSeasonColumns = new Dictionary<string, string>()
        {
            {"Year", "year" },
            {"Season", "season" },
            {"Number", "count" }
        };

        public StudentsAtEAAA(PrintsWindow context) : base(context)
        {}

        public override Dictionary<string, string> GetColumns()
        {
            return studentsPrSeasonColumns;
        }

        public override void SetItemsSource(int year, string season)
        {
            var result = context.GetService().getStudentsByYear(year)
                .Where(s=> s.Company != null && s.Company.name.Equals("Erhvervsakademi Aarhus"))
                .GroupBy(s => s.season)
                .Select(list => new { year = list.FirstOrDefault().year, season = list.FirstOrDefault().season, count = list.Count() })
                .ToList();
            context.datagrid.ItemsSource = result;
        }

        public override void YearChanged(int year, string season)
        {
            SetItemsSource(year, season);
        }
        public override void SeasonChanged(int year, string season)
        {
            SetItemsSource(year, season);
        }
    }

    class MainProjectOverviewSecretaryState : SelectionState
    {
        private static readonly Dictionary<string, string> columns = new Dictionary<string, string>()
        {
            {"Name", "name" },
            {"Class", "class" },
            {"Group", "groupnr" },
            {"Supervisor", "supervisor" },
            {"Project Title", "title" },
            {"Internship company", "company" }
        };
        public MainProjectOverviewSecretaryState(PrintsWindow context) : base(context)
        {
        }

        public override Dictionary<string, string> GetColumns()
        {
            return columns;
        }
        
        public override void SetItemsSource(int year, string season)
        {
            var mainprojects = context.GetService().GetMainProjectByYear(year)
                .Where(mp => mp.season.Equals(season));

            var students = context.GetService().getStudentsByYear(year)
                            .Where(s => s.season.Equals(season));

            var result =
                from s in students
                join mp in mainprojects on s.mainProjectTitle equals mp.title
                select new { name = s.name, @class = s.@class, groupnr = mp.groupNo, supervisor = mp.Supervisor.name, title = mp.title, company = mp.Company.name };

            result = result.OrderBy(s => s.title)
                .ThenBy(s => s.supervisor);

            context.datagrid.ItemsSource = result;
        }

        public override void YearChanged(int year, string season)
        {
            SetItemsSource(year, season);
        }
        public override void SeasonChanged(int year, string season)
        {
            SetItemsSource(year, season);
        }
    }
}
