using System.Collections.Generic;
using System.Linq;

namespace GaSchedule.Model
{
    /// <summary>
    /// 在一个排课周期中用于排课的槽位。
    /// </summary>
    public class Slot
    {
        #region 构造

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Slot(int index, int day, int room, int classNo)
        {
            this.SlotIdx = index;
            this.DayIdx = day;
            this.RoomIdx = room;
            this.PeriodIdx = classNo;
            this.IsFreezed = false;
            this.Classes = new List<CourseClass>();
        }

        /// <summary>
        /// 拷贝构造一个新槽位。
        /// </summary>
        public Slot(Slot slot)
        {
            this.SlotIdx = slot.SlotIdx;
            this.DayIdx = slot.DayIdx;
            this.RoomIdx = slot.RoomIdx;
            this.PeriodIdx = slot.PeriodIdx;
            this.IsFreezed = slot.IsFreezed;

            this.Classes = new List<CourseClass>();
            foreach (var c in slot.Classes)
            {
                // 运算过程中是不应该产生新课题的，所以这里只是拷贝引用
                this.Classes.Add(c);
            }
        }

        #endregion

        #region 属性

        public int SlotIdx { get; }

        /// <summary>
        /// 第几工作日，从0开始。
        /// </summary>
        public int DayIdx { get; }

        /// <summary>
        /// 教室编号，从0开始。
        /// </summary>
        public int RoomIdx { get; }

        /// <summary>
        /// 一天中的第几节课，从0开始。
        /// </summary>
        public int PeriodIdx { get; }

        /// <summary>
        /// 该槽位是否被冻结。
        /// 特定班级的特定课程的其中一节（或多节）必须安排在特定槽位上。
        /// </summary>
        public bool IsFreezed { get; set; }

        /// <summary>
        /// 在当前槽位上排的课堂。
        /// 原则上一个槽位上只能排一堂课，但是算法在运算过程中需要周转。
        /// </summary>
        public List<CourseClass> Classes { get; set; } = new List<CourseClass>();

        #endregion

        #region 方法

        public List<CourseClass> GetClassesByStudentsGroupId(int groupId)
        {
            return this.Classes.FindAll(c => c.StudentsGroupIds.Contains(groupId));
        }

        #endregion
    }
}
