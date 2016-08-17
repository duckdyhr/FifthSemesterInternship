using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SERVICE
{
    public class Service
    {

        private static Service instance = null;
        static readonly object padlock = new object();
        private LINQDataContext DBX;

        public Boolean isCompaniesWindowActive { get; set; }
        public Boolean isSupervisorsWindowActive { get; set; }
        public Boolean isStudentsWindowActive { get; set; }
        public Boolean isMainProjectsWindowActive { get; set; }
        public Boolean isLoadStudentsWindowActive { get; set; }
        public static Service GetInstance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Service();
                    }
                    return instance;
                }
            }
        }

        private Service()
        {
            try
            {
                DBX = new LINQDataContext();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void submitChanges()
        {
            try
            {
                DBX.SubmitChanges();
            }
            catch
            {
                throw;
            }
        }

        #region Supervisor
        public List<Supervisor> getSupervisorList()
        {
            return DBX.Supervisors.ToList();
        }

        public List<Supervisor> getSupervisorSearchList(String s)
        {
            List<Supervisor> temp = new List<Supervisor>();
            foreach (Supervisor sup in DBX.Supervisors.ToList<Supervisor>())
            {
                if (sup.name.ToLower().Contains(s.ToLower()))
                {
                    temp.Add(sup);
                }
            }
            return temp;
        }

        public void updateSupervisor(Supervisor supervisor)
        {
            if (DBX.Supervisors.Contains(supervisor))
            {
                submitChanges();
            }
            else
            {
                DBX.Supervisors.InsertOnSubmit(supervisor);
                submitChanges();
            }
        }

        public void deleteSupervisor(Supervisor supervisor)
        {
            foreach (Student s in DBX.Students.ToList())
            {
                if (s.SupervisorID.Equals(supervisor.id))
                {
                    s.SupervisorID = null;
                    submitChanges();
                }
            }
            DBX.Supervisors.DeleteOnSubmit(supervisor);
            submitChanges();
        }

        #endregion

        #region Companies

        public List<Company> getCompaniesList()
        {
            return DBX.Companies.ToList();
        }

        public List<Company> getCompaniesSearchList(String s)
        {
            List<Company> temp = new List<Company>();
            foreach (Company c in DBX.Companies.ToList<Company>())
            {
                if (c.name.ToLower().Contains(s.ToLower()))
                {
                    temp.Add(c);
                }
            }
            return temp;
        }

        public void updateCompany(Company company)
        {
            if (DBX.Companies.Contains(company))
            {
                submitChanges();
            }
            else
            {
                DBX.Companies.InsertOnSubmit(company);
                submitChanges();
            }
        }

        public void deleteCompany(Company company)
        {
            foreach (Student s in DBX.Students.ToList())
            {
                if (s.CompanyID.Equals(company.id))
                {
                    s.CompanyID = null;
                    submitChanges();
                }
            }
            DBX.Companies.DeleteOnSubmit(company);
            submitChanges();
        }

        public List<Contact> getContactList(int i)
        {
            List<Contact> temp = new List<Contact>();
            foreach (Contact c in DBX.Contacts.ToList())
            {
                if (c.CompanyID == i)
                {
                    temp.Add(c);
                }
            }
            return temp;
        }

        public List<Offering> getOfferingList(int i)
        {
            List<Offering> temp = new List<Offering>();
            foreach (Offering o in DBX.Offerings.ToList())
            {
                if (o.CompanyID == i)
                {
                    temp.Add(o);
                }
            }
            return temp;
        }
        #endregion

        #region contact

        public void updateContact(Contact contact)
        {
            if (DBX.Contacts.Contains(contact))
            {
                submitChanges();
            }
            else
            {
                DBX.Contacts.InsertOnSubmit(contact);
                submitChanges();
            }
        }

        public void deleteContact(Contact contact)
        {
            DBX.Contacts.DeleteOnSubmit(contact);
            submitChanges();
        }
        #endregion

        #region offering

        public void updateOffering(Offering offering)
        {
            if (DBX.Offerings.Contains(offering))
            {
                submitChanges();
            }
            else
            {
                DBX.Offerings.InsertOnSubmit(offering);
                submitChanges();
            }
        }

        public void deleteOffering(Offering offering)
        {
            DBX.Offerings.DeleteOnSubmit(offering);
            submitChanges();
        }

        public void updateCompanyComments(Company company, String text)
        {
            company.comments = text;
            submitChanges();
        }

        #endregion

        #region Students

        public List<Student> getStudentList()
        {
            return DBX.Students.ToList();
        }

        public void updateStudent(Student student)
        {
            if (DBX.Students.Contains(student))
            {
                if (student.year != null)
                {
                    checkYear(student.year.Value);
                }
                submitChanges();
            }
            else
            {
                if (student.year != null)
                {
                    checkYear(student.year.Value);
                }
                DBX.Students.InsertOnSubmit(student);
                submitChanges();
            }
        }

        private void checkYear(Int32 year)
        {
            Year tempYear = new Year();
            tempYear.value = year;
            if (!DBX.Years.Contains(tempYear))
            {
                DBX.Years.InsertOnSubmit(tempYear);
                submitChanges();
            }
        }

        public List<Student> getStudentSearchList(String s)
        {
            List<Student> temp = new List<Student>();
            foreach (Student sup in DBX.Students.ToList<Student>())
            {
                if (sup.name.ToLower().Contains(s.ToLower()))
                {
                    temp.Add(sup);
                }
            }
            return temp;
        }

        public void updateStudentComments(Student student, String text)
        {
            student.comments = text;
            submitChanges();
        }

        public void deleteStudent(Student student)
        {
            DBX.Students.DeleteOnSubmit(student);
            submitChanges();
        }

        public void assignStudentToCompany(Company c, Student s)
        {
            s.Company = c;
            submitChanges();
        }

        public void assignSupervisorToStudent(Supervisor supervisor, Student student)
        {
            student.Supervisor = supervisor;
            submitChanges();
        }

        #endregion

        #region MainProjects

        public List<MainProject> getMainProjectList()
        {
            return DBX.MainProjects.ToList();
        }

        public void updateMainProject(MainProject mainproject)
        {
            if (DBX.MainProjects.Contains(mainproject))
            {
                if (mainproject.year != null)
                {
                    checkYear(mainproject.year.Value);
                }
                submitChanges();
            }
            else
            {
                if (mainproject.year != null)
                {
                    checkYear(mainproject.year.Value);
                }
                DBX.MainProjects.InsertOnSubmit(mainproject);
                submitChanges();
            }
        }

        public void updateMainProjectComments(MainProject mainproject, String text)
        {
            mainproject.comments = text;
            submitChanges();
        }

        public void deleteMainProject(MainProject mainproject)
        {
            DBX.MainProjects.DeleteOnSubmit(mainproject);
            submitChanges();
        }

        public void assignMainProjectToCompany(Company c, MainProject s)
        {
            s.Company = c;
            submitChanges();
        }

        public List<Student> getParticipantList(String i)
        {
            List<Student> temp = new List<Student>();
            foreach (Student c in DBX.Students.ToList())
            {
                if (c.mainProjectTitle == i)
                {
                    temp.Add(c);
                }
            }
            return temp;
        }

        public void assignSupervisorToMainProject(Supervisor supervisor, MainProject mainproject)
        {
            mainproject.Supervisor = supervisor;
            submitChanges();
        }
        public void updateMainProject(Student student, String title, String discription)
        {
            student.mainProjectTitle = title;
            submitChanges();
        }

        public void assignStudentToMainProject(Student student , String mainProject)
        {
            student.mainProjectTitle = mainProject;
            submitChanges();
        }

        #endregion

        public List<Int32> getYearList()
        {
            List<Int32> list = new List<Int32>();
            foreach (Year y in DBX.Years)
            {
                list.Add(y.value);
            }
            return list;
        }

        public List<Student> getStudentsByYear(Int32 year)
        {
            var query = from s in DBX.Students
                        where s.year.Equals(year)
                        select s;

            return query.ToList();
        }

        public List<MainProject> getMainProjectsByYear(Int32 year)
        {
            var query = from s in DBX.MainProjects
                        where s.year.Equals(year)
                        select s;

            return query.ToList();
        }

        public void updateOfferingSeason(Offering o, String season)
        {
            o.season = season;
            submitChanges();
        }

        public void updateStudentSeason(Student s, String season)
        {
            s.season = season;
            submitChanges();
        }

        public void updateMainProjectSeason(MainProject s, String season)
        {
            s.season = season;
            submitChanges();
        }

        public void loadStudents(String fileName, String year, String strSeason)
        {
            String [] lines = File.ReadAllLines(fileName);
            List<Student> temp = new List<Student>();
            char[] separator = new char[] { ';' };
            string[] strSplitArr;
            string str15;
            int value;
            foreach (String line in lines)
            {   strSplitArr = line.Split(separator);
                value = 0;
                Student selectedStudent = new Student();
                Student s = selectedStudent as Student;
                foreach (String word in strSplitArr)
                {
                    word.Trim();
                    switch (value)
                    {
                        case 0:
                            var @class = word.Substring(4, 7);
                            s.@class = @class;
                            break;
                        case 1:
                            s.name = word;
                            break;
                        case 2:
                            s.address = word;
                            break;
                        case 3:
                            s.zipcode = System.Convert.ToInt32(word);
                            break;
                        case 4:
                            break;
                        case 5:
                            s.email = word;
                            break;
                        case 6:
                            str15 = word + "               ";
                            s.phone = str15.Substring(0, 15);
                            break;
                    }
                    value++;
                }
                s.year = System.Convert.ToInt32(year);
                s.season = strSeason;
                temp.Add(s);
            }
            DBX.Students.InsertAllOnSubmit(temp);
            submitChanges();
        }
    }
}
