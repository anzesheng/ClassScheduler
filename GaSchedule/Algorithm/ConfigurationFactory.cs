﻿using GaSchedule.Model;
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

            configuration.Teachers = o.GetValue("Teachers")?.ToObject<List<Teacher>>();
            configuration.StudentsGroups = o.GetValue("StudentsGroups")?.ToObject<List<StudentsGroup>>();
            configuration.Courses = o.GetValue("Courses")?.ToObject<List<Course>>();
            configuration.Classrooms = o.GetValue("Classrooms")?.ToObject<List<Classroom>>();

            var classTokens = o.GetValue("CourseClasses").ToArray();
            foreach (var token in classTokens)
            {
                int teacherId = token.Value<int>("Teacher");
                int courseId = token.Value<int>("Course");
                int duration = token.Value<int>("Duration");
                int[] groupIds = token["StudentsGroups"].ToObject<int[]>();
                bool requireComputers = token.Value<bool?>("RequireComputers") ?? false;

                var cc = new CourseClass(configuration, courseId, teacherId, duration, groupIds.ToList(), requireComputers);

                configuration.CourseClasses.Add(cc);
                var teacher = configuration.Teachers.FirstOrDefault(t => t.Id == teacherId);
                teacher.TeachClasses.Add(cc);
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
