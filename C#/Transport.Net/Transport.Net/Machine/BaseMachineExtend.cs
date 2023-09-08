﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Transport.Net
{
    /// <summary>
    ///     BaseMachine扩展类
    /// </summary>
    public static class BaseMachineExtend
    {
        /// <summary>
        ///     将获取的数据转换成可以向设备写入的数据格式
        ///     注意转换无法变更读写类型
        /// </summary>
        /// <param name="getValues">获取的数据</param>
        /// <returns>应该写入的数据</returns>
        public static Dictionary<string, TSend> MapGetValuesToSetValues<TSend>(this Dictionary<string, ReturnUnit<TSend>> getValues) where TSend : struct
        {
            if (getValues == null) return null;
            return (from getValue in getValues
                    where getValue.Value.DeviceValue != null
                    select new KeyValuePair<string, TSend>(getValue.Key, getValue.Value.DeviceValue.Value)).ToDictionary(
                p => p.Key, p => p.Value);
        }
    }

    /// <summary>
    ///     AddressUnit扩展
    /// </summary>
    public static class AddressUnitExtend
    {
        /// <summary>
        ///     映射泛型AddressUnit到字符串型AddressUnit
        /// </summary>
        /// <param name="addressUnit"></param>
        /// <returns></returns>
        public static AddressUnit<string, string, string> MapAddressUnitTUnitKeyToAddressUnit<TUnitKey, TAddressKey, TSubAddressKey>(this AddressUnit<TUnitKey, TAddressKey, TSubAddressKey> addressUnit) where TUnitKey : IEquatable<TUnitKey> where TAddressKey : IEquatable<TAddressKey> where TSubAddressKey : IEquatable<TSubAddressKey>
        {
            return new AddressUnit<string, string, string>()
            {
                Id = addressUnit.Id.ToString(),
                Area = addressUnit.Area,
                Address = addressUnit.Address?.ToString(),
                SubAddress = addressUnit.SubAddress?.ToString(),
                DataType = addressUnit.DataType,
                Zoom = addressUnit.Zoom,
                DecimalPos = addressUnit.DecimalPos,
                CommunicationTag = addressUnit.CommunicationTag,
                Name = addressUnit.Name,
                Unit = addressUnit.Unit,
                CanWrite = addressUnit.CanWrite,
            };
        }
    }
}