using ClassScheduler.Algorithm;
using System.Collections.Generic;

namespace ClassScheduler.Model
{
    /// <summary>
    /// 一堂课
    /// </summary>
    public class CourseClass
    {
        #region Constructors

        public CourseClass(Course course, Professor professor, int duration,
            List<StudentsGroup> groups, bool requireComputers)
        {
            this.Course = course;
            this.Professor = professor;
            this.Duration = duration;
            this.StudentsGroups = groups;
            this.RequireComputers = requireComputers;
        }

        public CourseClass(CourseClass c)
        {
            this.Course = c.Course;
            this.Professor = c.Professor;
            this.Duration = c.Duration;
            this.StudentsGroups = new List<StudentsGroup>();
            foreach (var g in c.StudentsGroups)
            {
                this.StudentsGroups.Add(g);
            }

            this.RequireComputers = c.RequireComputers;
        }

        #endregion

        #region Properties

        // 课堂的编号
        public int Id { get; set; }

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
        public List<StudentsGroup> StudentsGroups { get; set; } = new List<StudentsGroup>();

        /// <summary>
        /// Gets the number of seats (sum of student groups' sizes) are needed in the classroom.
        /// 上课的学生的数量，因为有可能需要合班上课，所以需要计算学生总和。
        /// </summary>
        public int NumberOfStuduents
        {
            get
            {
                int count = 0;
                foreach (var group in this.StudentsGroups)
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

        #endregion

        #region Methods

        public bool GroupsOverlap(int classId)
        {
            var c = Configuration.GetInstance().GetClassById(classId);
            if (c == null)
            {
                return false;
            }

            return this.GroupsOverlap(c);
        }

        // Returns TRUE if another class has one or overlapping student groups.
        // 如果有班级需要同时上另一节课，返回true。（表示冲突了）
        public bool GroupsOverlap(CourseClass c)
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

        public bool ProfessorOverlaps(int classId)
        {
            var c = Configuration.GetInstance().GetClassById(classId);
            return this.Professor == c?.Professor;
        }

        // Returns TRUE if another class has same professor.
        // 如果教师要同时上另一节课，返回true。（表示冲突了）
        public bool ProfessorOverlaps(CourseClass c)
        {
            return this.Professor == c.Professor;
        }

        #endregion
    }
}
