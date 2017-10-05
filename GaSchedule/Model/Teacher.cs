using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaSchedule.Model
{
    public class Teacher
    {
        public Teacher()
        {
            this.Id = -1;
            this.Name = string.Empty;
        }

        public Teacher(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the identifier of a teacher.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of a teacher.
        /// </summary>
        public string Name { get; set; }
    }
}
