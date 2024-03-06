using System;
using System.Collections.Generic;

namespace CoreAPI.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public bool IsRegular { get; set; }

    public DateTime BirhDate { get; set; }

    public string? ImageName { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
