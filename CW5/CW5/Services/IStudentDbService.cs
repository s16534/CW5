using CW5.DTOs.Requests;
using CW5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CW5.Services
{
    public interface IStudentDbService
    {
        void EnrollStudent(EnrollStudentRequest request);
        Enrollment PromoteStudents(int semester, string studies);
    }
}
