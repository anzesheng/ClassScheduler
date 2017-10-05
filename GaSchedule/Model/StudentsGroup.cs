using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaSchedule.Model
{
    public class StudentsGroup
    {
        public StudentsGroup()
        {
            this.Id = -1;
            this.Name = string.Empty;
            this.NumberOfStudents = 0;
        }

        public StudentsGroup(int id, string name, int number)
        {
            this.Id = id;
            this.Name = name;
            this.NumberOfStudents = number;
        }

        /// <summary>
        /// Gets or sets the identifier of the group.
        /// </summary>
        public int Id { get; set; }

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
