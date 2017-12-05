using GaSchedule.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GaSchedule.Algorithm
{
    public class Configuration
    {
        #region Fields

        private Random random = new Random();

        #endregion Fields

        #region Constructors and Destructors

        // Initialize data
        public Configuration()
        {
            this.Parameters = new AlgorithmParameters();
        }

        #endregion Constructors and Destructors

        #region Properties

        public AlgorithmParameters Parameters { get; set; } = new AlgorithmParameters();

        // Parsed professors
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();

        // Parsed student groups
        public List<StudentsGroup> StudentsGroups { get; set; } = new List<StudentsGroup>();

        // Parsed courses
        public List<Course> Courses { get; set; } = new List<Course>();

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
                return this.Parameters.WorkingDaysNumber * this.Parameters.NumberOfClassPerDay * this.StudentsGroups.Count;
            }
        }

        #endregion Properties

        #region Public Methods

        public bool VerifyContent()
        {
            this.Parameters.VerifyParameters();
            return true;
        }

        public int GetStudentNumber(List<int> groupIds)
        {
            int num = 0;
            foreach (var id in groupIds)
            {
                var group = this.StudentsGroups.FirstOrDefault(g => g.Id == id);
                if (group != null)
                {
                    num += group.NumberOfStudents;
                }
            }

            return num;
        }

        public int GetStudentsGroupIdx(CourseClass cc)
        {
            return this.StudentsGroups.FindIndex(sg => sg.Id == cc.StudentsGroupId);
        }

        #endregion Public Methods
    }
}