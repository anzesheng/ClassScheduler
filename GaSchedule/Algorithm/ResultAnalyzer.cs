using GaSchedule.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaSchedule.Algorithm
{
    public static class ResultAnalyzer
    {
        public static string GetResultByStudentsGroups(Configuration conf, GeneticAlgorithm ga)
        {
            StringBuilder result = new StringBuilder();
            string spliter = ", ";
            var schedule = ga.GetBestChromosome();

            result.AppendLine($"Fitness: {ga.GetBestChromosome().Fitness}, Generation: {ga.CurrentGeneration}");

            // 每个班级为一个小节
            for (int groupIdx = 0; groupIdx < conf.StudentsGroups.Count; groupIdx++)
            {
                var group = conf.StudentsGroups[groupIdx];

                // 小节的标题
                result.AppendLine($"GroupId: {group.Id}{spliter}Name: {group.Name}{spliter}Student Number:{group.NumberOfStudents}");

                // 小节的表头
                StringBuilder header = new StringBuilder("Class");
                for (int day = 1; day <= conf.Parameters.WorkingDaysNumber; day++)
                {
                    header.Append($"{spliter}Day{day}");
                }
                result.AppendLine(header.ToString());

                // 每节课 （一天中的）
                for (int periodIdx = 0; periodIdx < conf.Parameters.ClassNumberPerDay; periodIdx++)
                {
                    StringBuilder line = new StringBuilder($"Class{periodIdx}{spliter}");

                    // 每个工作日
                    for (int dayIdx = 0; dayIdx < conf.Parameters.WorkingDaysNumber; dayIdx++)
                    {
                        var slots = schedule.Slots.FindAll(s => s.DayIdx == dayIdx && s.PeriodIdx == periodIdx);
                        List<CourseClass> classes = new List<CourseClass>();

                        foreach (var slot in slots)
                        {
                            classes.AddRange(slot.GetClassesByStudentsGroupId(group.Id));
                        }

                        if (classes.Count < 1)
                        {
                            line.Append("NA");
                        }
                        else
                        {
                            foreach (var c in classes)
                            {
                                line.Append($"[I{conf.CourseClasses.FindIndex(cc => cc == c)}C{c.CourseId}T{c.TeacherId}G{c.StudentsGroupsString.Remove(c.StudentsGroupsString.Length - 1)}]");
                            }
                        }

                        line.Append(spliter);
                    }

                    result.AppendLine(line.ToString());
                }

                result.AppendLine();
            }

            return result.ToString();
        }

        public static string GetResultByClassRooms(Configuration conf, GeneticAlgorithm ga)
        {
            StringBuilder result = new StringBuilder();
            string delimiter = ", ";
            var schedule = ga.GetBestChromosome();

            // 每间教室，每间教师输出一个小节
            for (int roomIdx = 0; roomIdx < conf.Classrooms.Count; roomIdx++)
            {
                var room = conf.Classrooms[roomIdx];

                // 小节的标题
                result.AppendLine($"RoomId: {room.Id}{delimiter}Name: {room.Name}{delimiter}Student Number:{room.NumberOfSeats}");

                // 小结中的表头
                StringBuilder header = new StringBuilder("No");
                for (int dayIdx = 1; dayIdx <= conf.Parameters.WorkingDaysNumber; dayIdx++)
                {
                    header.Append($"{delimiter}Day{dayIdx}");
                }
                result.AppendLine(header.ToString());

                // 每节课 （一天中的）
                for (int sequnece = 0; sequnece < conf.Parameters.ClassNumberPerDay; sequnece++)
                {
                    StringBuilder line = new StringBuilder($"Class{sequnece}{delimiter}");

                    // 每个工作日
                    for (int dayIdx = 0; dayIdx < conf.Parameters.WorkingDaysNumber; dayIdx++)
                    {
                        var slot = schedule.Slots.FirstOrDefault(s => s.DayIdx == dayIdx && s.RoomIdx == roomIdx && s.PeriodIdx == sequnece);
                        Debug.Assert(slot != null);

                        if (slot.Classes.Count < 1)
                        {
                            line.Append($"NA{delimiter}");
                        }
                        else
                        {
                            foreach (var c in slot.Classes)
                            {
                                line.Append($"[I{conf.CourseClasses.FindIndex(cc => cc == c)}C{c.CourseId}T{c.TeacherId}G{c.StudentsGroupsString.Remove(c.StudentsGroupsString.Length - 1)}]");
                            }

                            line.Append(delimiter);
                        }
                    }

                    result.AppendLine(line.ToString());
                }

                result.AppendLine();
            }

            return result.ToString();
        }
    }
}
