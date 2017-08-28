using ClassScheduler.Model;
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

        private readonly int[] RoomSeats = new int[] { 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 500 };

        // Global instance
        private static Configuration instance;

        // Parsed professors
        private Dictionary<int, Professor> professors;

        // Parsed student groups
        private Dictionary<int, StudentsGroup> studentsGroups;

        // Parsed courses
        private Dictionary<int, Course> courses;

        // Parsed rooms
        private Dictionary<int, Classroom> classrooms;

        // Parsed classes
        private List<CourseClass> courseClasses;

        // Inidicate that configuration is not prsed yet
        private bool isEmpty;

        Random random = new Random();

        #endregion

        #region Constructors and Destructors

        // Initialize data
        private Configuration()
        {
            this.isEmpty = true;
        }

        // Frees used resources
        ~Configuration()
        {

        }

        #endregion

        #region Properties

        public static Configuration GetInstance()
        {
            return instance;
        }

        public bool IsEmpty()
        {
            return this.isEmpty;
        }

        #endregion

        #region Public Methods



        // Parse file and store parsed object
        public void ParseFile(string fileName)
        {
            // Initial professors
            this.professors.Clear();
            for (int i = 0; i < 10; i++)
            {
                this.professors[i] = new Professor(i, $"Professor{i}");
            }

            // Initial professors
            this.studentsGroups.Clear();
            for (int i = 0; i < 10; i++)
            {
                int numberOfStudents = random.Next(20, 100);
                this.studentsGroups[i] = new StudentsGroup(i, $"Group{i}", numberOfStudents);
            }

            // Initial courses
            this.courses.Clear();
            for (int i = 0; i < 10; i++)
            {
                this.courses[i] = new Course(i, $"Course{i}");
            }

            // Initial classrooms
            this.classrooms.Clear();
            for (int i = 0; i < 10; i++)
            {
                int n = random.Next(0, 10);
                bool hasComputer = n % 3 == 0;
                this.classrooms[i] = new Classroom($"Room{i}", hasComputer, RoomSeats[n]);
            }


            // Initial classes
            this.courseClasses.Clear();
            for (int i = 0; i < 10; i++)
            {
                int n = random.Next(0, 10);
                bool hasComputer = n % 3 == 0;
                //this.courseClasses[i] = new CourseClass($"Room{i}", hasComputer, RoomSeats[n]);
            }


            // clear previously parsed objects

            //Classroom.RestartIDs();

            //string line;

            //// open file
            //var file = new StreamReader(fileName);

            //while ((line = file.ReadLine()) != null)
            //{
            //    line.Trim();

            //    // get type of object, parse obect and store it

            //    if (line.Equals("#prof", StringComparison.InvariantCultureIgnoreCase))
            //    {

            //    }

            //    if (line.compare("#prof") == 0)
            //    {
            //        Professor* p = ParseProfessor(input);
            //        if (p)
            //            _professors.insert(pair<int, Professor*>(p->GetId(), p));
            //    }
            //    else if (line.compare("#group") == 0)
            //    {
            //        StudentsGroup* g = ParseStudentsGroup(input);
            //        if (g)
            //            _studentGroups.insert(pair<int, StudentsGroup*>(g->GetId(), g));
            //    }
            //    else if (line.compare("#course") == 0)
            //    {
            //        Course* c = ParseCourse(input);
            //        if (c)
            //            _courses.insert(pair<int, Course*>(c->GetId(), c));
            //    }
            //    else if (line.compare("#room") == 0)
            //    {
            //        Room* r = ParseRoom(input);
            //        if (r)
            //            _rooms.insert(pair<int, Room*>(r->GetId(), r));
            //    }
            //    else if (line.compare("#class") == 0)
            //    {
            //        CourseClass* c = ParseCourseClass(input);
            //        if (c)
            //            _courseClasses.push_back(c);
            //    }
            //}

            //input.close();

            //_isEmpty = false;
        }

        // Returns pointer to professor with specified ID
        // If there is no professor with such ID method returns NULL
        public Professor GetProfessorById(int id)
        {
            return this.professors.ContainsKey(id) ? this.professors[id] : null;
        }

        // Returns number of parsed professors
        public int GetNumberOfProfessors()
        {
            return this.professors.Count;
        }

        // Returns pointer to student group with specified ID
        // If there is no student group with such ID method returns NULL
        public StudentsGroup GetStudentsGroupById(int id)
        {
            return this.studentsGroups.ContainsKey(id) ? this.studentsGroups[id] : null;
        }

        // Returns number of parsed student groups
        public int GetNumberOfStudentGroups()
        {
            return this.studentsGroups.Count;
        }

        // Returns pointer to course with specified ID
        // If there is no course with such ID method returns NULL
        public Course GetCourseById(int id)
        {
            return this.courses.ContainsKey(id) ? this.courses[id] : null;
        }

        public int GetNumberOfCourses()
        {
            return this.courses.Count;
        }

        // Returns pointer to room with specified ID
        // If there is no room with such ID method returns NULL
        public Classroom GetRoomById(int id)
        {
            return this.classrooms.ContainsKey(id) ? this.classrooms[id] : null;
        }

        // Returns number of parsed rooms
        public int GetNumberOfRooms()
        {
            return this.classrooms.Count;
        }

        // Returns reference to list of parsed classes
        public List<CourseClass> GetCourseClasses()
        {
            return this.courseClasses;
        }

        // Returns number of parsed classes
        public int GetNumberOfCourseClasses()
        {
            return this.courseClasses.Count;
        }

        // Returns TRUE if configuration is not parsed yet

        #endregion


    }
}
