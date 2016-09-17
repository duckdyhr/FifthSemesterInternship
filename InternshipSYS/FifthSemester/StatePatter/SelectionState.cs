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
        public abstract void SeasonChanged();
        public abstract void YearChanged();
        public abstract Dictionary<string, string> GetColumns();
        public abstract void SetItemsSource(int year, string season);

        public List<Student> GetStudentList(int year, string season)
        {
            List<Student> studentList = context.GetService().getStudentsByYear(year)
                .ToList();
            if(season == null || season.Equals(""))
            {
                studentList = studentList
                    .Where(s => season.Equals("") || s.season.Equals(season))
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

        public override void YearChanged()
        {

            throw new NotImplementedException();
        }
        public override void SeasonChanged()
        {
            throw new NotImplementedException();
        }
    }
    class StudentCompanyState : SelectionState
    {
        private static readonly Dictionary<string, string> studentCompanyColumns = new Dictionary<string, string>()
        {
            {"Name", "name" },
            {"Class", "class"},
            {"Company", "Company.name" },
            {"Comments", "comments" }
        };
        public StudentCompanyState(PrintsWindow context) : base(context)
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

        public override void SeasonChanged()
        {
            throw new NotImplementedException();
        }
        
        public override void YearChanged()
        {
            throw new NotImplementedException();
        }
    }

}
