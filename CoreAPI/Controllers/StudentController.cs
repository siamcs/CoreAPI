using CoreAPI.DTOs;
using CoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
  //  [Authorize]
    [ApiController]
    
    public class StudentController : ControllerBase
    {
        private readonly CoreDbContext _db;
        private IWebHostEnvironment _e;

        public StudentController(CoreDbContext db, IWebHostEnvironment e)
        {
            _db = db;
            _e = e;
        }

        [HttpGet]
        public IActionResult GetAllStudent()
        {
           
            List<Student> students = _db.Students.Include(x => x.Courses).ToList();  //EgerLoading
           
            string JsonString = JsonConvert.SerializeObject(students, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

            });
            return Content(JsonString, "application/Json");
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {

            Student   student = _db.Students.Include(x => x.Courses).FirstOrDefault(x => x.StudentId == id);

            if (student == null)
            {
                return NotFound("Empty data");
            }

            string jsonstring = JsonConvert.SerializeObject(student, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });

            return Content(jsonstring, "application/Json");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var st = await _db.Students.FindAsync(id);

            if (st == null)
            {
                return BadRequest("Student not found.");
            }

            _db.Students.Remove(st);
            _db.SaveChanges();
            return Ok("deleted");
        }

        [HttpPost]
        public async Task<IActionResult> PostStudents([FromForm] Common common)
        {
            string FN = common.ImageName + ".jpg";
            string Url = "\\Upload\\" + FN;
            if (common.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_e.WebRootPath + "\\Upload\\"))
                {
                    Directory.CreateDirectory(_e.WebRootPath + "\\Upload\\");
                }
                using (FileStream fileStream = System.IO.File.Create(_e.WebRootPath + "\\Upload\\" + common.ImageFile.FileName))
                {
                    common.ImageFile.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }
            Student student = new Student();
            student.StudentName = common.StudentName;
            student.IsRegular = common.IsRegular;
            student.BirhDate = common.BirhDate;
            student.ImageName = FN;
            student.ImageUrl = Url;
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
            var emp = _db.Students.FirstOrDefault(x => x.StudentName == common.StudentName);
            int stid = emp.StudentId;
            List<Course> list = JsonConvert.DeserializeObject<List<Course>>(common.Courses);
            AddCourses(stid, list);
            await _db.SaveChangesAsync();
            return Ok("Saved.");
        }

        private void AddCourses(int stid, List<Course>? list)
        {
            {
                foreach (var item in list)
                {
                    Course experience = new Course()
                    {
                        StudentId = stid,
                        CourseHour = item.CourseHour,
                        CourseName = item.CourseName,
                    };

                    _db.Courses.Add(experience);
                    _db.SaveChanges();
                }
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudents(int id, [FromForm] Common common)
        {
            var student = await _db.Students.FindAsync(id);
            if (id != common.StudentId)
            {
                return BadRequest();
            }
            if (student == null)
            {
                return NotFound("Employee not found.");
            }
            string fileName = common.ImageName + ".jpg";
            string imageUrl = "\\Upload\\" + fileName;
            if (common.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_e.WebRootPath + "\\Upload\\"))
                {
                    Directory.CreateDirectory(_e.WebRootPath + "\\Upload\\");
                }
                using (FileStream fileStream = System.IO.File.Create(_e.WebRootPath + "\\Upload\\" + common.ImageFile.FileName))
                {
                    common.ImageFile.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }

            student.StudentName = common.StudentName;
            student.IsRegular = common.IsRegular;
            student.BirhDate = common.BirhDate;
            student.ImageName = fileName;
            student.ImageUrl = imageUrl;

            var exis = _db.Courses.Where(x => x.StudentId == id);
            _db.Courses.RemoveRange(exis);

            List<Course> list = JsonConvert.DeserializeObject<List<Course>>(common.Courses);
            AddCourses(student.StudentId, list);
            await _db.SaveChangesAsync();
            return Ok("updated");
        }
    }
}
