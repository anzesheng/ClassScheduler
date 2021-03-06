﻿using System.Linq;
using System.Text;

namespace GaSchedule.Algorithm
{
    public static class ResultAnalyzer
    {
        public static string GetResultByStudentsGroups(Configuration conf, GeneticAlgorithm ga)
        {
            StringBuilder result = new StringBuilder();
            string spliter = "\t";
            var schedule = ga.GetBestChromosome();

            result.AppendLine($"Fitness: {ga.GetBestChromosome().Fitness}, Evenness: {ga.GetBestChromosome().Evenness}, Generation: {ga.CurrentGeneration}");

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
                for (int periodIdx = 0; periodIdx < conf.Parameters.NumberOfClassPerDay; periodIdx++)
                {
                    StringBuilder line = new StringBuilder($"Class{periodIdx}{spliter}");

                    // 每个工作日
                    for (int dayIdx = 0; dayIdx < conf.Parameters.WorkingDaysNumber; dayIdx++)
                    {
                        var slot = schedule.Slots.FirstOrDefault(s => s.DayIndex == dayIdx && s.StudentsGroupIndex == groupIdx && s.ClassIndexInOneDay == periodIdx);

                        if (slot.Classes.Count < 1)
                        {
                            line.Append("NA");
                        }
                        else
                        {
                            foreach (var c in slot.Classes)
                            {
                                line.Append($"[I{conf.CourseClasses.FindIndex(cc => cc == c)}C{c.CourseId}T{c.TeacherId}G{c.StudentsGroupId}]");
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
    }
}