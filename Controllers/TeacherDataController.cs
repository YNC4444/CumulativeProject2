using CumulativeProject2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace CumulativeProject2.Controllers
{
    public class TeacherDataController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Return a list of teachers in the school.
        /// </summary>
        /// <example> GET api/TeaacherData/ListTeachers </example>
        /// <returns>A list of teachers ID, first name, last name, employee number, hire date, and salary. </returns>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null)
        {
            // Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection between the web server & the database
            Conn.Open();

            // Make a new command/query for the database
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL query
            cmd.CommandText = "Select * from teachers where lower(teacherfname) like lower(@key) or " +
                "lower(teacherlname) like lower(@key) or " +
                "lower(concat(teacherfname, ' ', teacherlname)) like lower(@key) or " +
                "cast(salary as decimal(10,2)) = @SalaryKey or " +
                "date(hiredate) = str_to_date(@DateKey, '%Y-%m-%d')";

            // paramaterizing the query for name search
            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");

            // parameter for salary search
            decimal SalaryKey;
            if (Decimal.TryParse(SearchKey, out SalaryKey))
            {
                cmd.Parameters.AddWithValue("@SalaryKey", SalaryKey);
            }
            else
            {
                cmd.Parameters.AddWithValue("@SalaryKey", DBNull.Value);
            }

            // parameter for hiredate search
            DateTime DateKey;
            if (DateTime.TryParse(SearchKey, out DateKey))
            {
                cmd.Parameters.AddWithValue("@DateKey", DateKey.ToString("yyyy-MM-dd"));
            }
            else
            {
                cmd.Parameters.AddWithValue("@DateKey", DBNull.Value);
            }

            cmd.Prepare();

            // Gather result set of query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            List<Teacher> Teachers = new List<Teacher>();

            while (ResultSet.Read())
            {
                // access column information by the database column name as an index
                int TeacherId = (int)ResultSet["teacherid"];
                string TeacherFname = (string)ResultSet["teacherfname"];
                string TeacherLname = (string)ResultSet["teacherlname"];
                string EmployeeNumber = (string)ResultSet["employeenumber"];
                DateTime HireDate = (DateTime)ResultSet["hiredate"];
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                Teacher NewTeacher = new Teacher();
                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.EmployeeNumber = EmployeeNumber;
                NewTeacher.HireDate = HireDate;
                NewTeacher.Salary = Salary;

                // add author name to the list created above
                Teachers.Add(NewTeacher);
            }

            // close the connection between the MySQL database & the web server
            Conn.Close();

            return Teachers;
        }

        [HttpGet]

        // used in the Show or List cshtml 
        public Teacher findTeacher(int id)
        {
            Teacher NewTeacher = new Teacher();

            // Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection between the web server & the database
            Conn.Open();

            // Make a new command/query for the database
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL query
            cmd.CommandText = "Select * from teachers where teacherid =" + id;

            // Gather result set of query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                // access column information by the database column name as an index
                int TeacherId = (int)ResultSet["teacherid"];
                string TeacherFname = (string)ResultSet["teacherfname"];
                string TeacherLname = (string)ResultSet["teacherlname"];
                string EmployeeNumber = (string)ResultSet["employeenumber"];
                DateTime HireDate = (DateTime)ResultSet["hiredate"];
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.EmployeeNumber = EmployeeNumber;
                NewTeacher.HireDate = HireDate;
                NewTeacher.Salary = Salary;
            }

            return NewTeacher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <example> POST : /api/TeacherData/DeleteTeacher/3 </example>
        [HttpPost]
        public void DeleteTeacher(int id)
        {
            // Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            // Open the connection between the web server & the database
            Conn.Open();

            // Make a new command/query for the database
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL query
            cmd.CommandText = "Delete from Teachers where teacherid=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();
        }

    }
}
