using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CW5.DTOs.Requests;
using CW5.DTOs.Responses;
using CW5.Models;
using CW5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CW5.Controllers
{

    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }

        [Route("api/enrollments")]
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            _service.EnrollStudent(request);
            var response = new EnrollStudentResponse();
            return Created(new Uri("http://localhost:7408/"), response);
        }


        [Route("api/enrollments/promotions")]
        [HttpPost]
        public IActionResult PromoteStudents(Study study)
        {
            var response = _service.PromoteStudents(study.Semester, study.Studies);
            if(response !=null)
            {
                return Created("Students promoted", response);
            } else
            {
                return NotFound("Enrollment doesn't exist!");
            }
            //return response !=null ? Created("Students promoted", response) : NotFound("Enrollment doesn't exist!");
        }
    }
}