using System;
using System.Collections.Generic;
using masz.Services;

namespace masz.Dtos.Enum
{
    public class EnumDto
    {
        public int Key { get; set; }
        public string Value { get; set; }

        public EnumDto(int key, string value)
        {
            Key = key;
            Value = value;
        }

        public static EnumDto Create(int key, string value) => new EnumDto(key, value);
    }
}