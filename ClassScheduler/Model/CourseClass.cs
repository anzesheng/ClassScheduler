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
        /// 课程信息
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Gets or sets the professor who teaches.
        /// 任课教师
        /// </summary>
        public Professor Professor { get; set; }

        /// <summary>
        /// Gets or sets the list of student groups that attend the class.
        /// 上课的班级列表，因为有可能上合班课，所以是列表。
        /// </summary>
        public List<StudentsGroup> StrudentsGroups { get; set; } = new List<StudentsGroup>();

        /// <summary>
        /// Gets the number of seats (sum of student groups' sizes) are needed in the classroom.
        /// 上课的学生的数量，因为有可能需要合班上课，所以需要计算学生总和。
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
        /// 当前课程是否需要计算机
        /// </summary>
        public bool RequireComputers { get; set; }

        /// <summary>
        /// Gets or sets the duration of the class (in hours).
        /// 课堂的时长
        /// </summary>
        public int Duration { get; set; }

        // Returns TRUE if another class has one or overlapping student groups.
        // 如果有班级需要同时上另一节课，返回true。（表示冲突了）
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
        // 如果教师要同时上另一节课，返回true。（表示冲突了）
        public bool ProfessorOverlaps(CourseClass c)
        {
            return this.Professor == c.Professor;
        }
    }
}
