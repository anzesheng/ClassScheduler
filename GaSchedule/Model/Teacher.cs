using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaSchedule.Model
{
    public class Teacher
    {
        /// <summary>
        /// Gets or sets the identifier of a teacher.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of a teacher.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a list of classes that a teacher teaches..
        /// </summary>
        public List<CourseClass> TeachClasses { get; set; } = new List<CourseClass>();

        public Teacher(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
