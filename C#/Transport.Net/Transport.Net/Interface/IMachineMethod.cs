using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transport.Net
{
    /// <summary>
    ///     Machine��д�����ӿ�
    /// </summary>
    public interface IMachineMethod
    {
    }

    /// <summary>
    ///     Machine�����ݶ�д�ӿ�
    /// </summary>
    public interface IMachineMethodDatas : IMachineMethod
    {
        /// <summary>
        ///     ��ȡ����
        /// </summary>
        /// <returns>���豸��ȡ������</returns>
        Task<ReturnStruct<Dictionary<string, ReturnUnit<double>>>> GetDatasAsync(MachineDataType getDataType);

        /// <summary>
        ///     д������
        /// </summary>
        /// <param name="setDataType">д������</param>
        /// <param name="values">��Ҫд��������ֵ䣬��д������ΪAddressʱ����Ϊ��Ҫд��ĵ�ַ����д������ΪCommunicationTagʱ����Ϊ��Ҫд��ĵ�Ԫ������</param>
        /// <returns>�Ƿ�д��ɹ�</returns>
        Task<ReturnStruct<bool>> SetDatasAsync(MachineDataType setDataType, Dictionary<string, double> values);
    }
}