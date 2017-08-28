using ClassScheduler.Model;
using System;
using System.Collections.Generic;
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
        private Dictionary<int, Professor> professors;

        // Parsed student groups
        private Dictionary<int, StudentsGroup> studentGroups;

        // Parsed courses
        private Dictionary<int, Course> courses;

        // Parsed rooms
        private private Dictionary<int, Classroom> rooms;

        // Parsed classes
        private List<CourseClass> courseClasses;

        // Inidicate that configuration is not prsed yet
        private bool isEmpty;

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
        public void ParseFile(char fileName)
        {

        }

        // Returns pointer to professor with specified ID
        // If there is no professor with such ID method returns NULL
        public Professor GetProfessorById(int id)
        {
            this.professors.FirstOrDefault(p=>p.id)
            hash_map<int, Professor*>::iterator it = _professors.find(id);
            return it != _professors.end() ? (*it).second : NULL;
        }

        // Returns number of parsed professors
        public int GetNumberOfProfessors() const { return (int)_professors.size(); }

    // Returns pointer to student group with specified ID
    // If there is no student group with such ID method returns NULL
    public StudentsGroup* GetStudentsGroupById(int id)
    {
        hash_map<int, StudentsGroup*>::iterator it = _studentGroups.find(id);
        return it != _studentGroups.end() ? (*it).second : NULL;
    }

    // Returns number of parsed student groups
    public int GetNumberOfStudentGroups() const { return (int)_studentGroups.size(); }

// Returns pointer to course with specified ID
// If there is no course with such ID method returns NULL
public Course* GetCourseById(int id)
{
    hash_map<int, Course*>::iterator it = _courses.find(id);
    return it != _courses.end() ? (*it).second : NULL;
}

public int GetNumberOfCourses() const { return (int)_courses.size(); }

	// Returns pointer to room with specified ID
	// If there is no room with such ID method returns NULL
	public Room* GetRoomById(int id)
{
    hash_map<int, Room*>::iterator it = _rooms.find(id);
    return it != _rooms.end() ? (*it).second : NULL;
}

// Returns number of parsed rooms
public int GetNumberOfRooms() const { return (int)_rooms.size(); }

	// Returns reference to list of parsed classes
	public const list<CourseClass*>& GetCourseClasses() const { return _courseClasses; }

	// Returns number of parsed classes
	public int GetNumberOfCourseClasses() const { return (int)_courseClasses.size(); }

	// Returns TRUE if configuration is not parsed yet
	
        #endregion


    }
}
