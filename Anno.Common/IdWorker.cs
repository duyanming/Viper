using System;
using System.Collections.Generic;
using System.Text;

namespace Anno
{
    /// <summary>
    /// Twitter的snowflake算法
    /// snowflake是Twitter开源的分布式ID生成算法，结果是一个long型的ID。其核心思想是：
    /// 使用41bit作为毫秒数，10bit作为机器的ID（5个bit是数据中心，5个bit的机器ID），
    /// 12bit作为毫秒内的流水号（意味着每个节点在每毫秒可以产生 4096 个 ID），最后还有一个符号位，永远是0。
    /// 具体实现的代码可以参看https://github.com/twitter/snowflake。
    /// </summary>
    public class IdWorker
    {
        private long workerId;
        private long datacenterId;
        private long sequence = 0L;

        /// <summary>
        /// Tue Apr 10 2018 13:12:46 GMT+0800 (中国标准时间)
        /// </summary>
        private static long twepoch = 1523337166808L;

        private static long workerIdBits = 5L;
        private static long datacenterIdBits = 5L;
        private static long maxWorkerId = -1L ^ (-1L << (int)workerIdBits);//最大数据中心
        private static long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);//最大工作站
        private static long sequenceBits = 12L;
        private static long timestampBits = 41L;

        private long workerIdShift = sequenceBits+timestampBits;
        private long datacenterIdShift = sequenceBits + workerIdBits+ timestampBits;
        private long timestampLeftShift = sequenceBits;
        private long sequenceMask = -1L ^ (-1L << (int)sequenceBits);

        private long lastTimestamp = -1L;
        private static object syncRoot = new object();

        private static readonly IdWorker idWorker = new IdWorker(Const.SettingService.WorkerId, Const.SettingService.DatacenterId);
        /// <summary>
        /// 获取ID
        /// </summary>
        /// <returns></returns>
        public static long NextId()
        {
            return idWorker.BuildId();
        }
        /// <summary>
        /// 设置工作站
        /// </summary>
        /// <param name="workerId">5个bit是数据中心</param>
        /// <param name="datacenterId">5个bit的机器ID</param>
        private IdWorker(long workerId, long datacenterId)
        {

            // sanity check for workerId
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id can't be greater than %d or less than 0", maxWorkerId));
            }
            if (datacenterId > maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(string.Format("datacenter Id can't be greater than %d or less than 0", maxDatacenterId));
            }
            this.workerId = workerId;
            this.datacenterId = datacenterId;
        }

        /// <summary>
        /// 获取ID
        /// </summary>
        /// <returns></returns>
        private long BuildId()
        {
            lock (syncRoot)
            {
                long timestamp = TimeGen();

                if (timestamp < lastTimestamp)
                {
                    throw new ApplicationException(string.Format("Clock moved backwards.  Refusing to generate id for %d milliseconds", lastTimestamp - timestamp));
                }

                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;
                    if (sequence == 0)
                    {
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0L;
                }

                lastTimestamp = timestamp;

                return (datacenterId << (int)datacenterIdShift) | (workerId << (int)workerIdShift) | ((timestamp - twepoch) << (int)timestampLeftShift) | sequence;
            }
        }
        long TilNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        long TimeGen()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
