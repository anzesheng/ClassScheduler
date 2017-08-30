using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Model
{
    public class CourseClass
    {
        /// <summary>
        /// Gets or sets the courset which the class belongs.
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Gets or sets the professor who teaches.
        /// </summary>
        public Professor Professor { get; set; }

        /// <summary>
        /// Gets or sets the list of student groups that attend the class.
        /// </summary>
        public List<StudentsGroup> StrudentsGroups { get; set; } = new List<StudentsGroup>();

        /// <summary>
        /// Gets the number of seats (sum of student groups' sizes) are needed in the classroom.
        /// </summary>
        public int NumberOfStuduents
        {
            get
            {
                int count = 0;
                foreach (var group in this.StrudentsGroups)
                {
                    count += group.NumberOfStudents;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the class requires computers in the classroom.
        /// </summary>
        public bool RequireComputers { get; set; }

        /// <summary>
        /// Gets or sets the duration of the class (in hours).
        /// </summary>
        public int Duration { get; set; }

        // Returns TRUE if another class has one or overlapping student groups.
        public bool GroupsOverlap(CourseClass c)
        {
            foreach (var g1 in this.StrudentsGroups)
            {
                foreach (var g2 in c.StrudentsGroups)
                {
                    if (g1 == g2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Returns TRUE if another class has same professor.
        public bool ProfessorOverlaps(CourseClass c)
        {
            return this.Professor == c.Professor;
        }
    }
}
