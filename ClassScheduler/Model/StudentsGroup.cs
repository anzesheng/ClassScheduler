using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Model
{
    public class StudentsGroup
    {
        /// <summary>
        /// Gets or sets the identifier of the group.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of students in the group.
        /// </summary>
        public int NumberOfStudents { get; set; }

        /// <summary>
        /// Gets or sets a list of classes that the students in the group attends.
        /// </summary>
        List<CourseClass> AttendClasses { get; set; } = new List<CourseClass>();
    }
}
