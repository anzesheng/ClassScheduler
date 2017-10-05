using GaSchedule.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GaSchedule.Algorithm
{
    public class ConfigurationFactory
    {
        public static Configuration CreateFromJson(string fileName)
        {
            Configuration configuration = new Configuration();

            try
            {
                string content = File.ReadAllText(fileName);
                JObject o = JObject.Parse(content);

                configuration.Teachers = o.GetValue("Teachers")?.ToObject<List<Teacher>>();
                configuration.StudentsGroups = o.GetValue("StudentsGroups")?.ToObject<List<StudentsGroup>>();
                configuration.Courses = o.GetValue("Courses")?.ToObject<List<Course>>();
                configuration.Classrooms = o.GetValue("Classrooms")?.ToObject<List<Classroom>>();

                var classTokens = o.GetValue("CourseClasses").ToArray();
                int id = 0;
                foreach (var token in classTokens)
                {
                    int teacherId = token.Value<int>("TeacherId");
                    int courseId = token.Value<int>("CourseId");
                    int duration = token.Value<int>("Duration");
                    int[] groupIds = token["StudentsGroupIds"].ToObject<int[]>();
                    bool requireComputers = token.Value<bool?>("RequireComputers") ?? false;

                    var cc = new CourseClass(id++, courseId, teacherId, duration, groupIds.ToList(), requireComputers);

                    configuration.CourseClasses.Add(cc);
                    var teacher = configuration.Teachers.FirstOrDefault(t => t.Id == teacherId);
                }
            }
            catch (Exception e)
            {
                string str = e.Message;
                throw;
            }

            return configuration;
        }
    }
}
