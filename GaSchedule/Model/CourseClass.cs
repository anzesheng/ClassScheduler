using GaSchedule.Algorithm;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GaSchedule.Model
{
    /// <summary>
    /// 一堂课
    /// </summary>
    public class CourseClass
    {
        #region Constructors

        private static int index = 0;

        private Configuration configuration;

        public CourseClass(Configuration configuration, int course, int teacher, int duration,
            List<int> groups, bool requireComputers)
        {
            this.configuration = configuration;
            this.Id = index++;
            this.Course = course;
            this.Teacher = teacher;
            this.Duration = duration;
            this.StudentsGroups = groups;
            this.RequireComputers = requireComputers;
        }

        #endregion

        #region Properties

        // 课堂的编号
        public int Id { get; }

        /// <summary>
        /// Gets or sets the courset which the class belongs.
        /// 课程ID
        /// </summary>
        public int Course { get; set; }

        /// <summary>
        /// Gets or sets the teacher who teaches.
        /// 任课教师
        /// </summary>
        public int Teacher { get; set; }

        /// <summary>
        /// Gets or sets the list of student groups that attend the class.
        /// 上课的班级列表，因为有可能上合班课，所以是列表。
        /// </summary>
        public List<int> StudentsGroups { get; set; } = new List<int>();

        public string StudentsGroupsString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in this.StudentsGroups)
                {
                    sb.Append($"{id.ToString()}&");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the number of seats (sum of student groups' sizes) are needed in the classroom.
        /// 上课的学生的数量，因为有可能需要合班上课，所以需要计算学生总和。
        /// </summary>
        public int NumberOfStuduents
        {
            get
            {
                int count = 0;
                foreach (var id in this.StudentsGroups)
                {
                    var group = this.configuration.StudentsGroups.FirstOrDefault(g => g.Id == id);
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

        #endregion

        #region Methods

        // Returns TRUE if another class has one or overlapping student groups.
        // 如果有班级需要同时上另一节课，返回true。（表示冲突了）
        public bool StudentsGroupsOverlap(CourseClass c)
        {
            foreach (var g1 in this.StudentsGroups)
            {
                foreach (var g2 in c.StudentsGroups)
                {
                    if (g1 == g2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Returns TRUE if another class has same teacher.
        // 如果教师要同时上另一节课，返回true。（表示冲突了）
        public bool TeacherOverlaps(CourseClass c)
        {
            return this.Teacher == c.Teacher;
        }

        #endregion
    }
}
