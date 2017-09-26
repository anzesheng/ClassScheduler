using GaSchedule.Algorithm;
using GaSchedule.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassScheduler
{
    public partial class InputForm : Form
    {
        private string fileName;
        private Configuration configuration;

        private BindingList<CourseClass> courseClasses;
        private BindingList<Teacher> teachers;
        private BindingList<Classroom> classrooms;
        private BindingList<Course> courses;
        private BindingList<StudentsGroup> studentsGroups;

        public InputForm(string fileName, Configuration configuration)
        {
            this.fileName = fileName;
            this.configuration = configuration;
            this.courseClasses = new BindingList<CourseClass>(configuration.CourseClasses);
            this.teachers = new BindingList<Teacher>(configuration.Teachers);
            this.classrooms = new BindingList<Classroom>(configuration.Classrooms);
            this.courses = new BindingList<Course>(configuration.Courses);
            this.studentsGroups = new BindingList<StudentsGroup>(configuration.StudentsGroups);

            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox1.Text)
            {
                case "课程安排":
                    this.dataGridView1.DataSource = this.courseClasses;
                    break;
                case "老师":
                    this.dataGridView1.DataSource = this.teachers;
                    break;
                case "班级":
                    this.dataGridView1.DataSource = this.studentsGroups;
                    break;
                case "学科":
                    this.dataGridView1.DataSource = this.courses;
                    break;
                case "教室":
                    this.dataGridView1.DataSource = this.classrooms;
                    break;
                default:
                    break;
            }
        }
    }
}
