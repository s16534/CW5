using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CW5.DTOs.Requests;
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
            /*
            using(var con = new SqlConnection(""))
            using(var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.CommandText = "SELECT IdStudies FROM studies WHERE name = @name";
                com.Parameters.AddWithValue("name", request.Studies);

                var er = com.ExecuteReader();

            }*/
            return Ok();
        }
    }
}