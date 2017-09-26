using System;
using System.Collections.Generic;
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
            string padding = "";
            string tab = "";
            string spliter = ", ";

            for (int g = 0; g < conf.StudentsGroups.Count; g++)
            {
                var group = conf.StudentsGroups[g];

                // Section title
                result.AppendLine($"ID: {group.Id}{spliter}Name: {group.Name}{spliter}Student Number:{group.NumberOfStudents}");

                // Table header
                StringBuilder header = new StringBuilder($"Class{padding}");
                for (int day = 1; day <= conf.WorkingDaysNumber; day++)
                {
                    header.Append($"{tab}Day{day}{padding}{spliter}");
                }
                result.AppendLine(header.ToString());

                //table content
                for (int no = 0; no < conf.ClassNumberPerDay; no++)
                {
                    StringBuilder line = new StringBuilder($"Class{no + 1}{spliter}");

                    for (int day = 0; day < conf.WorkingDaysNumber; day++)
                    {
                        line.Append(tab);

                        int idx = day * g * conf.ClassNumberPerDay + g * conf.ClassNumberPerDay + no;
                        var schedule = ga.GetBestChromosome();
                        var classes = schedule.ScheduledClasses.Where(sc => sc.Value == idx).Select(sc => sc.Key).ToArray();

                        if (classes.Length < 1)
                        {
                            line.Append($"NA{padding}{spliter}");
                        }
                        else
                        {
                            foreach (var c in classes)
                            {
                                line.Append($"ID:{c.Id} Course:{c.Course} Teacher:{c.Teacher} Students Group:[{c.StudentsGroupsString}]");
                            }

                            line.Append(spliter);
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
