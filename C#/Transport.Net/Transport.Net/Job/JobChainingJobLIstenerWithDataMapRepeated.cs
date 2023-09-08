﻿using Quartz;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Transport.Net
{
    /// <summary>
    ///     Repeated JobChaningJobListener
    /// </summary>
    public class JobChainingJobLIstenerWithDataMapRepeated : JobChainingJobListenerWithDataMap
    {
        /// <summary>
        ///     Job repeat count, -1 means infinity, 0 means 1 time.
        /// </summary>
        protected int RepeatCount { get; set; }

        /// <summary>
        /// JobChaningJobListener with DataMap passing from parent job to next job
        /// </summary>
        /// <param name="name">Job name</param>
        /// <param name="overwriteKeys">If key is overwritable, parent job will pass the value to next job event next job contains that key</param>
        /// <param name="repeatCount">Repeatation count for job chain</param>
        public JobChainingJobLIstenerWithDataMapRepeated(string name, ICollection<string> overwriteKeys, int repeatCount) : base(name, overwriteKeys)
        {
            RepeatCount = repeatCount;
        }

#nullable enable
        /// <inheritdoc />
        public override async Task JobWasExecuted(IJobExecutionContext context,
            JobExecutionException? jobException,
            CancellationToken cancellationToken = default)
        {
            await base.JobWasExecuted(context, jobException, cancellationToken);
            if (RepeatCount == 0) return;
            ChainLinks.TryGetValue(context.JobDetail.Key, out var sj);
            if (sj == null)
            {
                var chainRoot = context.JobDetail.Key;
                var chainParent = ChainLinks.FirstOrDefault(p => p.Value == context.JobDetail.Key).Key;
                while (chainParent != null)
                {
                    chainRoot = chainParent;
                    chainParent = ChainLinks.FirstOrDefault(p => p.Value == chainParent).Key;
                }
                if (RepeatCount > 0)
                {
                    RepeatCount--;
                }
                var sjJobDetail = await context.Scheduler.GetJobDetail(chainRoot);
                await context.Scheduler.AddJob(sjJobDetail!, true, false);
                await context.Scheduler.TriggerJob(chainRoot, cancellationToken).ConfigureAwait(false);
            }
        }
#nullable disable
    }
}
