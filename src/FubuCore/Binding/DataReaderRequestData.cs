﻿using System;
using System.Collections.Generic;
using System.Data;
using FubuCore.Util;
using System.Linq;

namespace FubuCore.Binding
{
    public class DataReaderRequestData : RequestDataBase
    {
        private readonly Cache<string, string> _aliases = new Cache<string, string>(key => key);
        private readonly Dictionary<string, string> _columns;
        private readonly IDataReader _reader;

        public DataReaderRequestData(IDataReader reader, Cache<string, string> aliases) : this(reader)
        {
            aliases.OnMissing = key => key;
            _aliases = aliases;
        }

        public DataReaderRequestData(IDataReader reader)
        {
            _reader = reader;
            _columns = new Dictionary<string, string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                _columns.Add(reader.GetName(i), null);
            }
        }

        #region IRequestData Members

        protected override object fetch(string key)
        {
            var rawValue = _reader[_aliases[key]];
            return rawValue == DBNull.Value ? null : rawValue.ToString();
        }

        protected override bool hasValue(string key)
        {
            return _columns.ContainsKey(key) || _aliases.Has(key);
        }

        #endregion

        public void SetAlias(string name, string alias)
        {
            _aliases[name] = alias;
        }

        public override IEnumerable<string> GetKeys()
        {
            return _columns.Keys.Union(_aliases.GetAllKeys());
        }
    }
}