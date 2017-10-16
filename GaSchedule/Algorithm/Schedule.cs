using GaSchedule.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GaSchedule.Algorithm
{
    /// <summary>
    /// 本遗传算法中的染色体（基因串）
    /// </summary>
    public class Schedule
    {
        #region Fields

        private readonly Configuration configuration;
        private Random random = new Random();

        #endregion

        #region Properties

        // 染色体的适应度
        public float Fitness { get; set; } = 0;

        /// <summary>
        /// 时间槽列表。一个时间槽表示一间教室的一个小时。
        /// (1) 时间槽的排列顺序如下：
        ///   [
        ///     {工作日1:[{班级1:[第1节课，第2节课,...第n节课]}, {班级2:[第1节课，第2节课,...第n节课]}, ... {班级m:[第1节课，第2节课,...第n节课]}]},
        ///     {工作日2:[{班级1:[第1节课，第2节课,...第n节课]}, {班级2:[第1节课，第2节课,...第n节课]}, ... {班级m:[第1节课，第2节课,...第n节课]}]},
        ///     ...,
        ///     {工作日5:[{班级1:[第1节课，第2节课,...第n节课]}, {班级2:[第1节课，第2节课,...第n节课]}, ... {班级m:[第1节课，第2节课,...第n节课]}]},
        ///   ]
        /// (2) 在这里一个时间槽里允许可以放置多个课堂，因为计算过程中会出现冲突的情况。
        /// (3) 每节课的教室与班级绑定，不能更改。
        /// (4) 该列表的长度 = 上课日数×每日课时数×班级数
        /// </summary>
        public List<Slot> Slots { get; private set; }

        /// <summary>
        /// 排课结果，key是课堂，value是课堂的时间槽序号。
        /// </summary>
        //public Dictionary<CourseClass, int> ScheduledClasses { get; set; } = new Dictionary<CourseClass, int>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes chromosomes with configuration block (setup of chromosome)
        /// 初始化染色体
        /// </summary>
        /// <param name="numberOfCrossoverPoints">交换点的数量</param>
        /// <param name="mutationSize">突变的数量</param>
        /// <param name="crossoverProbability">杂交的可能性</param>
        /// <param name="mutationProbability">突变的可能性</param>
        public Schedule(Configuration configuration)
        {
            this.configuration = configuration;
            this.Fitness = 0;

            // 准备好时间槽容器，数组初始化会调用Slot的默认构造函数
            this.InitialEmptySlots();
        }

        /// <summary>
        /// 准备好时间槽容器。一个教学周期（一般是一周）内能够安排的所有课程。
        /// </summary>
        public void InitialEmptySlots()
        {
            this.Slots = new List<Slot>();
            int i = 0;

            // 第几日
            for (int d = 0; d < this.configuration.Parameters.WorkingDaysNumber; d++)
            {
                // 第几个班级
                for (int g = 0; g < this.configuration.StudentsGroups.Count; g++)
                {
                    //第几节课
                    for (int c = 0; c < this.configuration.Parameters.NumberOfClassPerDay; c++)
                    {
                        this.Slots.Add(new Slot(i++, d, g, c));
                    }
                }
            }

            // 根据输入参数冻结槽位
            foreach (var cc in this.configuration.CourseClasses.Where(cc => cc.IsFixed))
            {
                for (int d = 0; d < cc.Duration; d++)
                {
                    var slot = this.GetSlot(cc.DayIndex, this.configuration.GetStudentsGroupIdx(cc), cc.ClassIndex + d);

                    if (slot.IsFixed)
                    {
                        throw new ArgumentException("试图冻结一个已经被冻结的槽位。");
                    }

                    slot.Classes.Add(cc);
                    slot.IsFixed = true;
                }
            }
        }

        /// <summary>
        /// 拷贝构造
        /// </summary>
        /// <param name="schedule">被拷贝者</param>
        /// <param name="setupOnly">是否只拷贝size。True时只拷贝size，不拷贝内容。</param>
        public Schedule(Schedule schedule, bool setupOnly)
        {
            this.configuration = schedule.configuration;

            // 拷贝排课内容
            if (setupOnly)
            {
                this.InitialEmptySlots();
            }
            else
            {
                // 拷贝槽位数据
                this.Slots = new List<Slot>();
                foreach (var s in schedule.Slots)
                {
                    this.Slots.Add(new Slot(s));
                }

                // 拷贝适应性
                this.Fitness = schedule.Fitness;

                this.SelfCheck();
            }
        }

        // 创建新染色体，拷贝设置但是随机排列课程
        public Schedule MakeNewFromPrototype()
        {
            // 创建新染色体，只拷贝当前染色体的size，不拷贝内容
            Schedule newSchedule = new Schedule(this, true);

            // 将每一个需要上的课随机地排到一个时段里
            foreach (CourseClass c in this.configuration.CourseClasses.Where(c => !c.IsFixed))
            {
                // 随机确定课堂的位置
                var slot = newSchedule.GetNewSlot(c);

                // 在当前教室的指定时段上排课，有的课堂需要占用多个时段
                for (int i = 0; i < c.Duration; i++)
                {
                    newSchedule.Slots[slot.SlotIndex].Classes.Add(c);
                }
            }

            //计算新染色体的适应性
            newSchedule.CalculateFitness();

            newSchedule.SelfCheck();

            // return smart pointer
            return newSchedule;
        }

        #endregion

        #region Properties



        #endregion

        #region Members

        // Performes crossover operation using to chromosomes and returns pointer to offspring
        // 执行均匀杂交
        public Schedule Crossover(Schedule parent2)
        {
            this.SelfCheck();
            parent2.SelfCheck();

            // check probability of crossover operation
            // 看看这次是否需要杂交，如果不需要
            if (this.Rand() % 100 > this.configuration.Parameters.CrossoverProbability)
            {
                // no crossover, just copy first parent
                // 不进行杂交，而是直接复制和返回第一个父本
                return new Schedule(this, false);
            }

            // new chromosome object, copy chromosome setup
            // 拷贝第一个父本创建新染色体。只拷贝配置信息，不拷贝内容。
            Schedule newSchedule = new Schedule(this, true);

            // 交换点数组
            bool[] cp = new bool[this.configuration.CourseClasses.Count];

            // determine crossover point (randomly)
            // 随机选择指定数量的交换点
            for (int i = 0; i < this.configuration.Parameters.NumberOfCrossoverPoints; i++)
            {
                while (true)
                {
                    int pos = Rand() % this.configuration.CourseClasses.Count;
                    if (!cp[pos])
                    {
                        cp[pos] = true;
                        break;
                    }
                }
            }

            // 确定第一个基因从那个父亲染色体中获取，双亲各有一半的概率被选中。
            bool first = Rand() % 2 == 0;

            int idx = 0;
            foreach (var cc in this.configuration.CourseClasses)
            {
                // 选择父本
                Schedule parent = first ? this : parent2;

                // all time-space slots of class are copied
                foreach (var slot in parent.GetSlots(cc))
                {
                    // may already has class for fixed slot
                    if (!newSchedule.Slots[slot.SlotIndex].Classes.Contains(cc))
                    {
                        newSchedule.Slots[slot.SlotIndex].Classes.Add(cc);
                    }
                }

                // 交叉点
                if (cp[idx++])
                {
                    // 切换父本
                    first = !first;
                }
            }

            // 计算新染色体的适应度
            newSchedule.CalculateFitness();

            newSchedule.SelfCheck();

            // return smart pointer to offspring
            return newSchedule;
        }

        // Performs mutation on chromosome
        // 在当前染色体上执行变异
        public void Mutation()
        {
            this.SelfCheck();

            // 检查是否需要变异
            if (Rand() % 100 > this.configuration.Parameters.MutationProbability)
            {
                return;
            }

            // move selected number of classes at random position
            // 随机移动指定数量课堂的时间槽
            for (int i = this.configuration.Parameters.MutationSize; i > 0; i--)
            {
                // 随机选择一个课堂
                CourseClass cc = this.GetRandomUnfixedClass();

                // 将课堂从现在的时间槽里删除
                foreach (var slot in this.GetSlots(cc))
                {
                    slot.Classes.Remove(cc);
                }

                // 随机决定课堂新的时间槽
                int newSlotIndex = this.GetNewSlot(cc).SlotIndex;

                // 放入新的时间槽
                for (int j = 0; j < cc.Duration; j++)
                {
                    this.Slots[newSlotIndex + j].Classes.Add(cc);
                }

                // 验证课程节数
                var count = this.Slots.FindAll(s => s.Classes.Contains(cc)).Count;
                Debug.Assert(cc.Duration == count);
            }

            // 重新计算适应度
            CalculateFitness();

            this.SelfCheck();
        }

        /// <summary>
        /// 随机选择一个可排课的课堂
        /// </summary>
        /// <returns>一个课堂</returns>
        private CourseClass GetRandomUnfixedClass()
        {
            CourseClass cc = null;

            do
            {
                cc = this.configuration.CourseClasses[Rand() % this.configuration.CourseClasses.Count];
            } while (cc.IsFixed);

            return cc;
        }

        private Slot GetNewSlot(CourseClass cc)
        {
            //TODO: 没有考虑duration，午休和duration冻结
            Slot newSlot = null;
            int count = 0;
            int group = this.configuration.GetStudentsGroupIdx(cc);
            bool isFixed = false;

            // 尽量选中空slot，最多尝试10次
            do
            {
                // 第几个工作日
                int day = Rand() % this.configuration.Parameters.WorkingDaysNumber;

                // 第几节课开始上，考虑了duration
                int time = Rand() % (this.configuration.Parameters.NumberOfClassPerDay + 1 - cc.Duration);

                // 得到新的时间槽
                newSlot = this.GetSlot(day, group, time);

                isFixed = false;
                bool isOccupied = false;
                for (int i = 0; i < cc.Duration; i++)
                {
                    // 是否包含已经冻结的slot
                    if (this.Slots[newSlot.SlotIndex + i].IsFixed)
                    {
                        isFixed = true;
                        break;
                    }

                    // Slot是否为空，我们希望优先选中空slot
                    if (this.Slots[newSlot.SlotIndex + i].Classes.Count > 0)
                    {
                        isOccupied = true;
                    }
                }

                if (!isOccupied)
                {
                    break;
                }

            } while (isFixed || count++ < 10);

            return newSlot;
        }

        // Calculates fitness value of chromosome
        // 计算染色体的适应度
        public void CalculateFitness()
        {
            int score = 0;

            // check criterias and calculate scores for each class in schedule
            foreach (var cc in this.configuration.CourseClasses)
            {
                // 同一时间，班级和教室只能上一门课
                bool teacherOverlapped = false;
                bool groupOverlapped = false;
                bool roomOverlapped = false;
                foreach (var slot in this.GetSlots(cc))
                {
                    if (teacherOverlapped && groupOverlapped && roomOverlapped)
                    {
                        break;
                    }

                    var concurrentSlots = this.Slots.FindAll(s => s.DayIndex == slot.DayIndex && s.ClassIndexInOneDay == slot.ClassIndexInOneDay);

                    if (!teacherOverlapped)
                    {
                        teacherOverlapped = concurrentSlots.FindAll(s => s.Classes.FirstOrDefault(c => c.TeacherId == cc.TeacherId) != null).Count > 1;
                    }

                    if (!groupOverlapped)
                    {
                        if (slot.Classes.Count > 1)
                        {
                            groupOverlapped = true;
                        }
                        else
                        {
                            groupOverlapped = concurrentSlots.FindAll(s => s.Classes.FirstOrDefault(c => cc.StudentsGroupId == c.StudentsGroupId) != null).Count > 1;
                        }
                    }

                    //if (!roomOverlapped)
                    //{
                    //    roomOverlapped = concurrentSlots.FindAll(s => s.Classes.FirstOrDefault(c => cc.ClassroomId == c.ClassroomId) != null).Count > 1;
                    //}
                }

                // 如果教师没有冲突，加1分
                if (!teacherOverlapped)
                {
                    score++;
                }

                // 如果学生组上课时间没有冲突，加1分
                if (!groupOverlapped)
                {
                    score++;
                }

                //if (!roomOverlapped)
                //{
                //    score++;
                //}
                //this.criteria[ci + 1] = !roomOverlapped;
            }

            // calculate fitess value based on score
            this.Fitness = (float)score / (this.configuration.CourseClasses.Count * 2);
        }

        #endregion

        #region Priviate methods

        private int Rand()
        {
            return this.random.Next();
        }

        private Slot GetSlot(int d, int g, int c)
        {
            return this.Slots.FirstOrDefault(s => s.DayIndex == d && s.StudentsGroupIndex == g && s.ClassIndexInOneDay == c);
        }

        private List<Slot> GetSlots(CourseClass cc)
        {
            return this.Slots.FindAll(s => s.Classes.Contains(cc));
        }

        private void SelfCheck()
        {
            int count1 = 0;
            foreach (var c in this.configuration.CourseClasses)
            {
                count1 += c.Duration;
            }

            int count2 = 0;
            foreach (var s in this.Slots)
            {
                count2 += s.Classes.Count;
            }

            Debug.Assert(count2 == 0 || count1 == count2);
        }

        #endregion
    }
}
