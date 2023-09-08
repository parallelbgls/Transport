using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Transport.Net
{
    /// <summary>
    ///		���豸���������
    /// </summary>
    public sealed class MultipleMachinesJobScheduler<TMachineMethod, TMachineKey, TReturnUnit> where TMachineKey : IEquatable<TMachineKey> where TReturnUnit : struct where TMachineMethod : class, IMachineMethod
    {
        private static int _machineCount = 0;

        /// <summary>
        ///     �����豸������
        /// </summary>
        /// <param name="machines">�豸�ļ���</param>
        /// <param name="machineJobTemplate">�豸������ģ��</param>
        /// <param name="count">�ظ�����������Ϊ����ѭ����0Ϊִ��һ��</param>
        /// <param name="intervalSecond">�������</param>
        /// <returns></returns>
        public static ParallelLoopResult RunScheduler(IEnumerable<IMachine<TMachineKey>> machines, Func<IMachine<TMachineKey>, MachineGetJobScheduler<TMachineMethod, TMachineKey, TReturnUnit>, Task> machineJobTemplate, int count = 0, int intervalSecond = 1)
        {
            _machineCount = machines.Count();
            return Parallel.ForEach(machines, (machine, state, index) =>
            {
                Task.Factory.StartNew(async () =>
                {
                    if (intervalSecond > 0)
                    {
                        Thread.Sleep((int)(intervalSecond * 1000.0 / _machineCount * index));
                    }
                    var getJobScheduler = await MachineJobSchedulerCreator<TMachineMethod, TMachineKey, TReturnUnit>.CreateScheduler("Trigger" + index, count, intervalSecond);
                    await machineJobTemplate(machine, getJobScheduler);
                });
            });
        }

        /// <summary>
        ///		ȡ������
        /// </summary>
        /// <returns></returns>
        public static ParallelLoopResult CancelJob()
        {
            return Parallel.For(0, _machineCount, async index =>
            {
                await MachineJobSchedulerCreator<TMachineMethod, TMachineKey, TReturnUnit>.CancelJob("Trigger" + index);
            });
        }
    }
}
