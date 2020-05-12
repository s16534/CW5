using CW5.DTOs.Requests;
using CW5.DTOs.Responses;
using CW5.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CW5.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        private string connectionInfo = "Data Source=db-mssql;Initial Catalog=s16534;Integrated Security=True";
        public void EnrollStudent(EnrollStudentRequest request)
        {
            var student = new Student();
            student.IndexNumber = request.IndexNumber;
            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.Studies = request.Studies;


            using (var con = new SqlConnection(connectionInfo))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.Transaction = tran;

                try
                {
                    com.CommandText = "SELECT IdStudy FROM studies WHERE name = @name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    var er = com.ExecuteReader();
                    if (!er.Read())
                    {
                        er.Close();
                        tran.Rollback();
                       // return BadRequest("Studia nie istnieja");
                    }
                    int idStudies = (int)er["IdStudy"];
                    Console.WriteLine(idStudies);
                    er.Close();

                    //----------------------------------------------------
                    com.CommandText = "SELECT MAX(IdEnrollment) as IdEnrollment FROM Enrollment";
                    er = com.ExecuteReader();
                    int idEnrollment;
                    if (er.Read())
                    {
                        idEnrollment = (int)er["IdEnrollment"] + 1;
                        Console.WriteLine(idEnrollment);
                    }
                    else
                    {
                        idEnrollment = 1;
                    }
                    Console.WriteLine(idEnrollment);
                    er.Close();

                    //----------------------------------------------------

                    com.CommandText = "SELECT * FROM Enrollment WHERE IdStudy = @idstudy AND Semester = @semester";
                    com.Parameters.AddWithValue("idstudy", idStudies);
                    com.Parameters.AddWithValue("semester", 1);
                    er = com.ExecuteReader();
                    if (!er.Read())
                    {
                        er.Close();
                        Console.WriteLine("E: " + idEnrollment + "/S: " + idStudies + "/SD: " + DateTime.Now);
                        com.CommandText = "INSERT INTO Enrollment VALUES (@iden, @sem, @ids, @StartDate)";
                        com.Parameters.AddWithValue("iden", idEnrollment);
                        com.Parameters.AddWithValue("sem", 1);
                        com.Parameters.AddWithValue("ids", idStudies);
                        com.Parameters.AddWithValue("StartDate", DateTime.Now);
                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        idEnrollment = (int)er["IdEnrollment"];
                        er.Close();
                    }
                    //----------------------------------------------------

                    com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = @indexnumber";
                    com.Parameters.AddWithValue("indexnumber", request.IndexNumber);
                    er = com.ExecuteReader();
                    if (er.Read())
                    {
                        er.Close();
                        tran.Rollback();
                    }
                    else
                    {
                        er.Close();
                        string bd = Convert.ToDateTime(request.BirthDate).ToString("yyyy-MM-dd");
                        com.CommandText = "INSERT INTO Student VALUES (@inr, @fn, @ln, @bd, @idenroll)";
                        com.Parameters.AddWithValue("inr", request.IndexNumber);
                        com.Parameters.AddWithValue("fn", request.FirstName);
                        com.Parameters.AddWithValue("ln", request.LastName);
                        com.Parameters.AddWithValue("bd", bd);
                        com.Parameters.AddWithValue("idenroll", idEnrollment);

                        com.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                catch (SqlException ex)
                {
                    tran.Rollback();
                }
            }
        }


        public Enrollment PromoteStudents(int semester, string studies)
        {
            string infoConnection = connectionInfo;

            var study = new Study();
            study.Studies = studies;
            study.Semester = semester;

            using (var con = new SqlConnection(infoConnection))
            using (var com = new SqlCommand())
            {
                con.Open();
                com.Connection = con;

                com.CommandText = "SELECT * FROM Enrollment LEFT JOIN Studies ON Enrollment.IdStudy=Studies.IdStudy WHERE Studies.Name=@Name AND Enrollment.Semester=@Semester";
                com.Parameters.AddWithValue("Name", study.Studies);
                com.Parameters.AddWithValue("Semester", study.Semester);
                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    dr.Close();
                    return null;
                }
                dr.Close();

            }


            int EnrollmentId = 0;
            using (var con = new SqlConnection(infoConnection))
            using (var com = new SqlCommand("PromoteStudents", con)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                com.Parameters.Add(new SqlParameter("@Studies", study.Studies));
                com.Parameters.Add(new SqlParameter("@Semester", study.Semester));
                var returnParameter = com.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                con.Open();
                com.ExecuteNonQuery();
                EnrollmentId = (int)returnParameter.Value;
            }

            using (var con = new SqlConnection(infoConnection))
            using (var com = new SqlCommand())
            {
                con.Open();
                com.Connection = con;

                com.CommandText = "SELECT * FROM Enrollment WHERE IdEnrollment=@IdEnrollment";
                com.Parameters.AddWithValue("IdEnrollment", EnrollmentId);
                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    dr.Close();
                }

                var enroll = new Enrollment();
                enroll.IdEnrollment = (int)dr["IdEnrollment"];
                enroll.Semester = (int)dr["Semester"];
                enroll.IdStudy = (int)dr["IdStudy"];
                enroll.StartDate = (DateTime)dr["StartDate"];

                dr.Close();
                return enroll;
            }
        }

        public Student GetStudent(string index)
        {
            using (SqlConnection con = new SqlConnection(connectionInfo))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Index", index);

                con.Open();
                var student = new Student();
                SqlDataReader sql = com.ExecuteReader();
                if (sql.Read())
                {
                    student.IndexNumber = sql["IndexNumber"].ToString();
                    return student;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
