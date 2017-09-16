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
    public class ConfigurationFactory
    {
        public static Configuration CreateFromJson(string fileName)
        {
            Configuration configuration = new Configuration();

            //try
            //{
                JObject o = JObject.Parse(File.ReadAllText(fileName));

                var professors = o.GetValue("Teachers")?.ToObject<Teacher[]>();
                foreach (var p in professors)
                {
                    configuration.Teachers[p.Id] = p;
                }

                var groups = o.GetValue("StudentsGroups")?.ToObject<StudentsGroup[]>();
                foreach (var g in groups)
                {
                    configuration.StudentsGroups[g.Id] = g;
                }

                var courses = o.GetValue("Courses")?.ToObject<Course[]>();
                foreach (var c in courses)
                {
                    configuration.Courses[c.Id] = c;
                }

                var rooms = o.GetValue("Classrooms")?.ToObject<Classroom[]>();
                foreach (var r in rooms)
                {
                    configuration.Classrooms[r.Id] = r;
                }

                var classTokens = o.GetValue("CourseClasses").ToArray();
                foreach (var token in classTokens)
                {
                    int professorId = token.Value<int>("Teacher");
                    int courseId = token.Value<int>("Course");
                    int duration = token.Value<int>("Duration");
                    int[] groupIds = token["StudentsGroups"].ToObject<int[]>();
                    bool requireComputers = token.Value<bool?>("RequireComputers") ?? false;

                    var groupsInClass = new List<StudentsGroup>();
                    foreach (var id in groupIds)
                    {
                        groupsInClass.Add(configuration.StudentsGroups[id]);
                    }

                    var cc = new CourseClass(configuration.Courses[courseId], configuration.Teachers[professorId],
                        duration, groupsInClass, requireComputers);

                    configuration.CourseClasses.Add(cc);
                    configuration.Teachers[professorId].TeachClasses.Add(cc);
                }
            //}
            //catch (Exception e)
            //{
            //    string str = e.Message;
            //    throw;
            //}

            return configuration;
        }
    }
}
