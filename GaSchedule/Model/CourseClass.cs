using GaSchedule.Algorithm;
using Newtonsoft.Json;
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

        public CourseClass()
        {
            this.CourseId = 1;
            this.TeacherId = 1;
            this.Duration = 1;
            this.StudentsGroupIds = new List<int>();
            this.RequireComputers = false;
        }

        public CourseClass(int id, int course, int teacher, int duration,
            List<int> groups, bool requireComputers)
        {
            this.CourseId = course;
            this.TeacherId = teacher;
            this.Duration = duration;
            this.StudentsGroupIds = groups;
            this.RequireComputers = requireComputers;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the courset which the class belongs.
        /// 课程ID
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the teacher who teaches.
        /// 任课教师
        /// </summary>
        public int TeacherId { get; set; }

        /// <summary>
        /// Gets or sets the list of student groups that attend the class.
        /// 上课的班级列表，因为有可能上合班课，所以是列表。
        /// </summary>
        public List<int> StudentsGroupIds { get; set; } = new List<int>();

        [JsonIgnore]
        public string StudentsGroupsString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in this.StudentsGroupIds)
                {
                    sb.Append($"{id.ToString()},");
                }
                return sb.ToString();
            }
            set
            {
                this.StudentsGroupIds = new List<int>();
                foreach (var item in value.Split(','))
                {
                    int id;
                    if (int.TryParse(item.Trim(), out id))
                    {
                        this.StudentsGroupIds.Add(id);
                    }
                }
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
            foreach (var g1 in this.StudentsGroupIds)
            {
                foreach (var g2 in c.StudentsGroupIds)
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
            return this.TeacherId == c.TeacherId;
        }

        #endregion
    }
}
