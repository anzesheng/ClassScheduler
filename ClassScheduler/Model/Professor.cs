using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Model
{
    public class Professor
    {
        /// <summary>
        /// Gets or sets the identifier of a professor.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of a professor.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a list of classes that a professor teaches..
        /// </summary>
        List<CourseClass> TeachClasses { get; set; } = new List<CourseClass>();

        public Professor(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
