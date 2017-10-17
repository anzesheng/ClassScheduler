using GaSchedule.Algorithm;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GaSchedule.Model
{
    /// <summary>
    /// 一堂课
    /// </summary>
    public class CourseClass
    {
        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// 课程ID
        /// 需事先指定好
        /// </summary>
        public int CourseId { get; set; } = 1;

        /// <summary>
        /// 任课教师ID
        /// 需事先指定好
        /// </summary>
        public int TeacherId { get; set; } = 1;

        /// <summary>
        /// 班级ID
        /// 需事先指定好
        /// </summary>
        public int StudentsGroupId { get; set; } = 1;

        /// <summary>
        /// 教室ID
        /// 需事先指定好
        /// </summary>
        public int ClassroomId { get; set; } = 1;

        /// <summary>
        /// 课堂的长度
        /// 如果一堂课连续上N节，Duration为N
        /// 需事先指定好
        /// </summary>
        public int Duration { get; set; } = 1;

        /// <summary>
        /// 课程是否已经固定
        /// 需求：
        ///   1. 指定特定班级的特定课程的其中一节（或多节）必须安排在特定星期，特定科节上
        ///   2. 某天的某节禁止排课
        /// 需事先指定好
        /// </summary>
        public bool IsFixed { get; set; } = false;

        /// <summary>
        /// 偏好的时间，用第几节课表示。比如上午用[0,1,2,3]表示，假设上午有四节课。
        /// 需求：科目安排优先（比如优先排在上午）
        /// 需事先指定好
        /// </summary>
        public int[] ExpectedClassIndexes { get; set; }

        /// <summary>
        /// 在第几个工作日上课，从0开始。
        /// - 当IsFixed==true时，需事先指定好
        /// - 当IsFixed==false时，由算法计算
        /// </summary>
        public int DayIndex { get; set; } = 0;

        /// <summary>
        /// 一天中的第几节课，从0开始
        /// - 当IsFixed==true时，需事先指定好
        /// - 当IsFixed==false时，由算法计算
        /// </summary>
        public int ClassIndex { get; set; }

        /// <summary>
        /// 专门为DataGrid控件创建的属性
        /// </summary>
        public string ExpectedClassIndexesStr
        {
            get
            {
                return this.Convert(this.ExpectedClassIndexes);
            }
            set
            {
                this.ExpectedClassIndexes = this.Convert(value);
            }
        }

        #endregion

        #region PrivateMethods

        private string Convert(int[] arr)
        {
            StringBuilder sb = new StringBuilder();

            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i == 0)
                    {
                        sb.Append(arr[0]);
                    }
                    else
                    {
                        sb.Append($",{arr[i]}");
                    }
                }
            }

            return sb.ToString();
        }

        private int[] Convert(string str)
        {
            var list = new List<int>();
            foreach (var item in str.Split(','))
            {
                int idx = 0;
                if (int.TryParse(item.Trim(), out idx))
                {
                    list.Add(idx);
                }
            }

            return list.ToArray();
        }

        #endregion
    }
}
