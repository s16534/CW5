using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CW5.DTOs.Requests;
using CW5.DTOs.Responses;
using CW5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CW5.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var s = new Student();
            s.IndexNumber   = request.IndexNumber;
            s.FirstName     = request.FirstName;
            s.LastName      = request.LastName;
            s.Studies       = request.Studies;


            var response = new EnrollStudentResponse();



            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s16534;Integrated Security=True"))
            using(var com = new SqlCommand())
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
                        return BadRequest("Studia nie istnieja");
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
                        return BadRequest("IndexNumber exists!");
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
                    response.LastName = s.LastName;
                    response.Semester = 1;
                    response.StartDate = DateTime.Now;

                } catch (SqlException ex)
                {
                    tran.Rollback();
                    return BadRequest("SQL Exception: " + ex);
                }
            }
            return Created(new Uri("http://localhost:7408/"), response);
        }
    }
}