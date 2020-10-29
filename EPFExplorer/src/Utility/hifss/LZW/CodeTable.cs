using System;
using System.Collections.Generic;

namespace Hifss.LZW
{
    internal class CodeTable
    {
        private Dictionary<uint, uint[]> _table = new Dictionary<uint, uint[]>();
        private uint _index = 0;
        private byte _lzwMinCodeSize = 2;

        public uint[] this[uint i]
        {
            get
            {
                return _table[i];
            }
        }

        public CodeTable(byte lzwMinCodeSize)
        {
            _lzwMinCodeSize = lzwMinCodeSize;
            Initialize();
        }

        public void Add(params uint[] value)
        {
            _table.Add(_index, value);
            _index++;
        }

        public bool HasCode(uint value)
        {
            return _table.ContainsKey(value);
        }

        public void Initialize()
        {
            _index = 0;
            _table.Clear();
            
            for (int i = 0; i < (1 << _lzwMinCodeSize); i++)
            {
                Add(_index);
            }

            Add((uint)(1 << _lzwMinCodeSize));
            Add((uint)(1 << _lzwMinCodeSize) + 1);
        }
    }
}
