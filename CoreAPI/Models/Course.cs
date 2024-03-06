using System;
using System.Collections.Generic;

namespace CoreAPI.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public decimal CourseHour { get; set; }

    public int StudentId { get; set; }

    public virtual Student Student { get; set; } = null!;
}
