//using CoreAPI.Models;

namespace CoreAPI.DTOs
{
    public class Common
    {
        public int StudentId { get; set; }

        public string ? StudentName { get; set; }

        public bool IsRegular { get; set; }

        public DateTime BirhDate { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? ImageName { get; set; }
        public string? Courses { get; set; }
    }
}
