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
        public Slot(int day, int room, int classNo)
        {
            this.DayNo = day;
            this.RoomNo = room;
            this.ClassNo = classNo;
            this.IsFreezed = false;
            this.Classes = new List<int>();
        }

        /// <summary>
        /// 拷贝构造一个新槽位。
        /// </summary>
        public Slot(Slot slot)
        {
            this.DayNo = slot.DayNo;
            this.RoomNo = slot.RoomNo;
            this.ClassNo = slot.ClassNo;
            this.IsFreezed = slot.IsFreezed;

            this.Classes = new List<int>();
            foreach (var c in slot.Classes)
            {
                // 运算过程中是不应该产生新课题的，所以这里只是拷贝引用
                this.Classes.Add(c);
            }
        }

        #endregion

        #region 属性

        /// <summary>
        /// 第几工作日，从0开始。
        /// </summary>
        public int DayNo { get; set; }

        /// <summary>
        /// 教室编号，从0开始。
        /// </summary>
        public int RoomNo { get; set; }

        /// <summary>
        /// 一天中的第几节课，从0开始。
        /// </summary>
        public int ClassNo { get; set; }

        /// <summary>
        /// 该槽位是否被冻结。
        /// 特定班级的特定课程的其中一节（或多节）必须安排在特定槽位上。
        /// </summary>
        public bool IsFreezed { get; set; }

        /// <summary>
        /// 在当前槽位上排的课堂。
        /// 原则上一个槽位上只能排一堂课，但是算法在运算过程中需要周转。
        /// </summary>
        public List<int> Classes { get; set; } = new List<int>();

        #endregion

        #region 方法

        /// <summary>
        /// 排入一节新课堂。
        /// </summary>
        /// <param name="classId">新课题的编号</param>
        /// <returns>如果排入成功，返回true；如果新课堂已经在槽位中，返回false。</returns>
        public bool AddClass(int classId)
        {
            if (this.Classes.Contains(classId))
            {
                return false;
            }

            this.Classes.Add(classId);
            return true;
        }

        /// <summary>
        /// 删除一节课堂。
        /// </summary>
        /// <param name="classId">需要从槽位中删除的课堂编号。</param>
        /// <returns>如果删除成功，返回true；如果该课堂不在当前槽位中，返回false。</returns>
        public bool RemoveClass(int classId)
        {
            return this.Classes.Remove(classId);
        }

        #endregion
    }
}
