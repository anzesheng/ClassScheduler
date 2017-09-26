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
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();

        // Parsed student groups
        public List<StudentsGroup> StudentsGroups { get; set; } = new List<StudentsGroup>();

        // Parsed courses
        public List<Course> Courses { get; set; } = new List<Course>();

        // Parsed rooms
        public List<Classroom> Classrooms { get; set; } = new List<Classroom>();

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

        public bool VerifyContent()
        {
            this.Parameters.VerifyParameters();
            return true;
        }

        #endregion
    }
}
