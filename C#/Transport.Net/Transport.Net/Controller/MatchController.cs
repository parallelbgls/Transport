﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Transport.Net
{
    /// <summary>
    ///     通讯号匹配模式的控制器
    /// </summary>
    public class MatchController : FifoController
    {
        /// <summary>
        ///     匹配字典
        /// </summary>
        protected ICollection<(int, int)>[] KeyMatches { get; }

        /// <summary>
        ///     构造器
        /// </summary>
        /// <param name="keyMatches">匹配字典，每个Collection代表一个匹配集合，每一个匹配集合中的数字代表需要匹配的位置，最后计算出来的数字是所有位置数字按照集合排序后叠放在一起</param>
        /// <param name="acquireTime">获取间隔</param>
        /// <param name="lengthCalc">包长度计算</param>
        /// <param name="checkRightFunc">包校验函数</param>
        /// <param name="waitingListMaxCount">包等待队列长度</param>
        public MatchController(ICollection<(int, int)>[] keyMatches, int acquireTime,
            Func<byte[], int> lengthCalc = null, Func<byte[], bool?> checkRightFunc = null, int? waitingListMaxCount = null) : base(acquireTime, lengthCalc, checkRightFunc, waitingListMaxCount)
        {
            KeyMatches = keyMatches;
        }

        /// <inheritdoc />
        protected override (string, string)? GetKeyFromMessage(byte[] message)
        {
            string ans1 = "";
            string ans2 = "";
            foreach (var matchPoses in KeyMatches)
            {
                int tmpCount = 0, tmpCount2 = 0;
                foreach (var matchPos in matchPoses)
                {
                    if (matchPos.Item1 > message.Length - 1 || matchPos.Item2 > message.Length - 1) return null;
                    tmpCount = tmpCount * 256 + message[matchPos.Item1];
                    tmpCount2 = tmpCount2 * 256 + message[matchPos.Item2];
                }
                ans1 += tmpCount + " ";
                ans2 += tmpCount2 + " ";
            }
            return (ans1, ans2);
        }

        /// <inheritdoc />
        protected override MessageWaitingDef GetMessageFromWaitingList(byte[] receiveMessage)
        {
            if (receiveMessage == null) return null;
            var returnKey = GetKeyFromMessage(receiveMessage);
            MessageWaitingDef ans;
            lock (WaitingMessages)
            {
                ans = WaitingMessages.FirstOrDefault(p => returnKey.HasValue && p.Key == returnKey.Value.Item2);
            }
            return ans;
        }
    }
}
