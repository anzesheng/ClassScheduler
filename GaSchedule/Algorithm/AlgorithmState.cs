namespace GaSchedule.Algorithm
{
    /// <summary>
    /// 算法的执行状态
    /// </summary>
    public enum AlgorithmState
    {
        /// <summary>
        /// 用户终止了计算。
        /// </summary>
        AS_USER_STOPED,

        /// <summary>
        /// 算法因为满足条件而停止计算。
        /// </summary>
        AS_CRITERIA_STOPPED,

        /// <summary>
        /// 正在计算中。
        /// </summary>
        AS_RUNNING
    }
}