using System.Collections.Generic;

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
        public Slot(int slotIndex, int dayIndex, int studentsGroupIndex, int classIndex)
        {
            this.SlotIndex = slotIndex;
            this.DayIndex = dayIndex;
            this.StudentsGroupIndex = studentsGroupIndex;
            this.ClassIndexInOneDay = classIndex;
            this.IsFixed = false;
            this.Classes = new List<CourseClass>();
        }

        /// <summary>
        /// 拷贝构造一个新槽位。
        /// </summary>
        public Slot(Slot slot)
        {
            this.SlotIndex = slot.SlotIndex;
            this.DayIndex = slot.DayIndex;
            this.StudentsGroupIndex = slot.StudentsGroupIndex;
            this.ClassIndexInOneDay = slot.ClassIndexInOneDay;
            this.IsFixed = slot.IsFixed;

            this.Classes = new List<CourseClass>();
            foreach (var c in slot.Classes)
            {
                // 运算过程中是不应该产生新课题的，所以这里只是拷贝引用
                this.Classes.Add(c);
            }
        }

        #endregion 构造

        #region 属性

        /// <summary>
        /// 槽位序号
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// 第几工作日，从0开始。
        /// </summary>
        public int DayIndex { get; }

        /// <summary>
        /// 班级的顺序号（非ID），从0开始
        /// 该槽位只能放置该班级的课程
        /// </summary>
        public int StudentsGroupIndex { get; }

        /// <summary>
        /// 一天中的第几节课，从0开始。
        /// </summary>
        public int ClassIndexInOneDay { get; }

        /// <summary>
        /// 该槽位是否被冻结。
        /// 特定班级的特定课程的其中一节（或多节）必须安排在特定槽位上。
        /// </summary>
        public bool IsFixed { get; set; }

        /// <summary>
        /// 在当前槽位上排的课堂。
        ///   1. 一个槽位只能为一个班级排课。
        ///   2. 原则上一个槽位上只能排一堂课，但是算法在运算过程中需要周转。
        /// </summary>
        public List<CourseClass> Classes { get; set; } = new List<CourseClass>();

        #endregion 属性
    }
}