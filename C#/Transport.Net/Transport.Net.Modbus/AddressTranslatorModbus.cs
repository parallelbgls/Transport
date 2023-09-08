﻿using System.Collections.Generic;

namespace Transport.Net.Modbus
{
    /// <summary>
    ///     Modbus地址翻译器基类
    /// </summary>
    public abstract class ModbusTranslatorBase : AddressTranslator
    {
        /// <summary>
        ///     地址转换
        /// </summary>
        /// <param name="address">格式化的地址</param>
        /// <param name="isRead">是否为读取，是为读取，否为写入</param>
        /// <param name="isSingle">是否只写入一个数据</param>
        /// <returns>翻译后的地址</returns>
        public abstract AddressDef AddressTranslate(string address, bool isRead, bool isSingle);

        /// <summary>
        ///     地址转换
        /// </summary>
        /// <param name="address">格式化的地址</param>
        /// <param name="isRead">是否为读取，是为读取，否为写入</param>
        /// <returns>翻译后的地址</returns>
        public override AddressDef AddressTranslate(string address, bool isRead)
        {
            return AddressTranslate(address, isRead, false);
        }
    }

    /// <summary>
    ///     Modbus数据单元翻译器
    /// </summary>
    public class AddressTranslatorModbus : ModbusTranslatorBase
    {
        /// <summary>
        ///     读功能码
        /// </summary>
        protected Dictionary<string, AreaOutputDef> ReadFunctionCodeDictionary;

        /// <summary>
        ///     写功能码
        /// </summary>
        protected Dictionary<(string, bool), AreaOutputDef> WriteFunctionCodeDictionary;

        /// <summary>
        ///     构造器
        /// </summary>
        public AddressTranslatorModbus()
        {
            ReadFunctionCodeDictionary = new Dictionary<string, AreaOutputDef>
            {
                {
                    "0X",
                    new AreaOutputDef
                    {
                        Code = (int) ModbusProtocolFunctionCode.ReadCoilStatus,
                        AreaWidth = 0.125
                    }
                },
                {
                    "1X",
                    new AreaOutputDef
                    {
                        Code = (int) ModbusProtocolFunctionCode.ReadInputStatus,
                        AreaWidth = 0.125
                    }
                },
                {
                    "3X",
                    new AreaOutputDef {Code = (int) ModbusProtocolFunctionCode.ReadInputRegister, AreaWidth = 2}
                },
                {
                    "4X",
                    new AreaOutputDef {Code = (int) ModbusProtocolFunctionCode.ReadHoldRegister, AreaWidth = 2}
                }
            };
            WriteFunctionCodeDictionary = new Dictionary<(string, bool), AreaOutputDef>
            {
                {
                    ("0X", false),
                    new AreaOutputDef
                    {
                        Code = (int) ModbusProtocolFunctionCode.WriteMultiCoil,
                        AreaWidth = 0.125
                    }
                },
                {
                    ("4X", false),
                    new AreaOutputDef
                    {
                        Code = (int) ModbusProtocolFunctionCode.WriteMultiRegister,
                        AreaWidth = 2
                    }
                },
                {
                    ("0X", true),
                    new AreaOutputDef
                    {
                        Code = (int) ModbusProtocolFunctionCode.WriteSingleCoil,
                        AreaWidth = 0.125
                    }
                },
                {
                    ("4X", true),
                    new AreaOutputDef
                    {
                        Code = (int) ModbusProtocolFunctionCode.WriteSingleRegister,
                        AreaWidth = 2
                    }
                }
            };
        }

        /// <summary>
        ///     地址转换
        /// </summary>
        /// <param name="address">格式化的地址</param>
        /// <param name="isRead">是否为读取，是为读取，否为写入</param>
        /// <param name="isSingle">是否只写入一个数据</param>
        /// <returns>翻译后的地址</returns>
        public override AddressDef AddressTranslate(string address, bool isRead, bool isSingle)
        {
            address = address.ToUpper();
            var splitString = address.Split(' ');
            var head = splitString[0];
            var tail = splitString[1];
            string sub;
            if (tail.Contains("."))
            {
                var splitString2 = tail.Split('.');
                sub = splitString2[1];
                tail = splitString2[0];
            }
            else
            {
                sub = "0";
            }
            return isRead
                ? new AddressDef
                {
                    AreaString = head,
                    Area = ReadFunctionCodeDictionary[head].Code,
                    Address = int.Parse(tail) - 1,
                    SubAddress = int.Parse(sub)
                }
                : new AddressDef
                {
                    AreaString = head,
                    Area = WriteFunctionCodeDictionary[(head, isSingle)].Code,
                    Address = int.Parse(tail) - 1,
                    SubAddress = int.Parse(sub)
                };
        }

        /// <summary>
        ///     获取区域中的单个地址占用的字节长度
        /// </summary>
        /// <param name="area">区域名称</param>
        /// <returns>字节长度</returns>
        public override double GetAreaByteLength(string area)
        {
            return ReadFunctionCodeDictionary[area].AreaWidth;
        }
    }
}