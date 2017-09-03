using ClassScheduler.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Algorithm
{
    public class Configuration
    {
        #region Fields

        // Global instance
        private static Configuration instance;

        // Parsed professors
        public Dictionary<int, Professor> Professors { get; set; } = new Dictionary<int, Professor>();

        // Parsed student groups
        public Dictionary<int, StudentsGroup> StudentsGroups { get; set; } = new Dictionary<int, StudentsGroup>();

        // Parsed courses
        public Dictionary<int, Course> Courses { get; set; } = new Dictionary<int, Course>();

        // Parsed rooms
        public Dictionary<int, Classroom> Classrooms { get; set; } = new Dictionary<int, Classroom>();

        // Parsed classes
        // 待排的课程，有算法使用者提供
        public List<CourseClass> CourseClasses { get; set; } = new List<CourseClass>();

        // Inidicate that configuration is not prsed yet
        public bool IsEmpty { get; private set; }

        Random random = new Random();

        #endregion

        #region Constructors and Destructors

        // Initialize data
        private Configuration()
        {
            this.IsEmpty = true;
        }

        // Frees used resources
        ~Configuration()
        {

        }

        #endregion

        #region Properties

        public static Configuration GetInstance()
        {
            if (instance == null)
            {
                instance = new Configuration();
            }
            return instance;
        }

        #endregion

        #region Public Methods

        // Parse file and store parsed object
        public void ParseFile(string fileName)
        {
            this.Professors.Clear();
            this.StudentsGroups.Clear();
            this.Courses.Clear();
            this.Classrooms.Clear();
            this.CourseClasses.Clear();

            try
            {
                JObject o = JObject.Parse(File.ReadAllText(fileName));

                var professors = o.GetValue("Professors").ToObject<Professor[]>();
                foreach (var p in professors)
                {
                    this.Professors[p.Id] = p;
                }

                var groups = o.GetValue("StudentsGroups").ToObject<StudentsGroup[]>();
                foreach (var g in groups)
                {
                    this.StudentsGroups[g.Id] = g;
                }

                var courses = o.GetValue("Courses").ToObject<Course[]>();
                foreach (var c in courses)
                {
                    this.Courses[c.Id] = c;
                }

                var rooms = o.GetValue("Classrooms").ToObject<Classroom[]>();
                foreach (var r in rooms)
                {
                    this.Classrooms[r.Id] = r;
                }

                var classTokens = o.GetValue("CourseClasses").ToArray();
                foreach (var token in classTokens)
                {
                    int professorId = token.Value<int>("Professor");
                    int courseId = token.Value<int>("Course");
                    int duration = token.Value<int>("Duration");
                    int[] groupIds = token["StudentsGroups"].ToObject<int[]>();
                    bool requireComputers = token.Value<bool?>("RequireComputers") ?? false;

                    var groupsInClass = new List<StudentsGroup>();
                    foreach (var id in groupIds)
                    {
                        groupsInClass.Add(this.StudentsGroups[id]);
                    }

                    var cc = new CourseClass(this.Courses[courseId], this.Professors[professorId],
                        duration, groupsInClass, requireComputers);

                    this.CourseClasses.Add(cc);
                }

                this.IsEmpty = false;
            }
            catch (Exception e)
            {
                string str = e.Message;
                throw;
            }
        }

        // Returns pointer to professor with specified ID
        // If there is no professor with such ID method returns NULL
        public Professor GetProfessorById(int id)
        {
            return this.Professors.ContainsKey(id) ? this.Professors[id] : null;
        }

        // Returns number of parsed professors
        public int GetNumberOfProfessors()
        {
            return this.Professors.Count;
        }

        // Returns pointer to student group with specified ID
        // If there is no student group with such ID method returns NULL
        public StudentsGroup GetStudentsGroupById(int id)
        {
            return this.StudentsGroups.ContainsKey(id) ? this.StudentsGroups[id] : null;
        }

        // Returns number of parsed student groups
        public int GetNumberOfStudentGroups()
        {
            return this.StudentsGroups.Count;
        }

        // Returns pointer to course with specified ID
        // If there is no course with such ID method returns NULL
        public Course GetCourseById(int id)
        {
            return this.Courses.ContainsKey(id) ? this.Courses[id] : null;
        }

        //public int GetNumberOfCourses()
        //{
        //    return this.courses.Count;
        //}

        // Returns pointer to room with specified ID
        // If there is no room with such ID method returns NULL
        public Classroom GetRoomById(int id)
        {
            return this.Classrooms.ContainsKey(id) ? this.Classrooms[id] : null;
        }

        // Returns TRUE if configuration is not parsed yet

        #endregion


    }
}
