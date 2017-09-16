using GaSchedule.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaSchedule.Algorithm
{
    public class Configuration
    {
        #region Fields

        private Random random = new Random();

        #endregion

        #region Constructors and Destructors

        // Initialize data
        public Configuration()
        {
            this.Parameters = new AlgorithmParameters();

            this.Teachers.Clear();
            this.StudentsGroups.Clear();
            this.Courses.Clear();
            this.Classrooms.Clear();
            this.CourseClasses.Clear();
        }

        #endregion

        #region Properties

        // Number of working hours per day
        public int ClassNumberPerDay { get; set; } = 12;

        // Number of days in week
        public int WorkingDaysNumber { get; set; } = 5;

        public AlgorithmParameters Parameters { get; set; }

        // Parsed professors
        public Dictionary<int, Teacher> Teachers { get; set; } = new Dictionary<int, Teacher>();

        // Parsed student groups
        public Dictionary<int, StudentsGroup> StudentsGroups { get; set; } = new Dictionary<int, StudentsGroup>();

        // Parsed courses
        public Dictionary<int, Course> Courses { get; set; } = new Dictionary<int, Course>();

        // Parsed rooms
        public Dictionary<int, Classroom> Classrooms { get; set; } = new Dictionary<int, Classroom>();

        // Parsed classes
        // 待排的课程，由算法使用者提供
        public List<CourseClass> CourseClasses { get; set; } = new List<CourseClass>();

        /// <summary>
        /// 一个教学周期（一般是一周）内能够安排的课程的总数
        /// </summary>
        public int TotalSlots
        {
            get
            {
                return this.WorkingDaysNumber * this.ClassNumberPerDay * this.Classrooms.Count;
            }
        }

        #endregion

        #region Public Methods

        // Returns pointer to teacher with specified ID
        // If there is no teacher with such ID method returns NULL
        public Teacher GetTeacherById(int id)
        {
            return this.Teachers.ContainsKey(id) ? this.Teachers[id] : null;
        }

        // Returns pointer to student group with specified ID
        // If there is no student group with such ID method returns NULL
        public StudentsGroup GetStudentsGroupById(int id)
        {
            return this.StudentsGroups.ContainsKey(id) ? this.StudentsGroups[id] : null;
        }

        // Returns pointer to course with specified ID
        // If there is no course with such ID method returns NULL
        public Course GetCourseById(int id)
        {
            return this.Courses.ContainsKey(id) ? this.Courses[id] : null;
        }

        // Returns pointer to room with specified ID
        // If there is no room with such ID method returns NULL
        public Classroom GetRoomById(int id)
        {
            return this.Classrooms.ContainsKey(id) ? this.Classrooms[id] : null;
        }

        public CourseClass GetClassById(int id)
        {
            return this.CourseClasses.FirstOrDefault(c => c.Id == id);
        }

        public bool VerifyContent()
        {
            this.Parameters.VerifyParameters();
            return true;
        }

        #endregion
    }
}
