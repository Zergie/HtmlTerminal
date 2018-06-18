using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlTerminal
{
    public enum StyleSelector
    {
        Id,
        Class,
        Tag,
    }

    class StyleCollection : Dictionary<string, Dictionary<string, string>>
    {
        internal StyleCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        private string GetKey(StyleSelector selector, string key)
        {
            switch (selector)
            {
                case StyleSelector.Id:
                    return $"#{key}";
                case StyleSelector.Class:
                    return $".{key}";
                case StyleSelector.Tag:
                    return key;
                default:
                    throw new NotImplementedException();
            }
        }

        public Dictionary<string, string> this[StyleSelector selector, string key]
        {
            get
            {
                return base[GetKey(selector, key)];
            }
            set
            {
                base[GetKey(selector, key)] = value;
            }
        }

        public void Add(string key, string value)
        {
            Add(StyleSelector.Tag, key, value);
        }

        public void Add(StyleSelector selector, string key, string value)
        {
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();

            foreach (var item in value.Split(';'))
            {
                string[] arr = (from s in item.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries) select s.Trim()).ToArray();

                if (arr.Length == 1)
                    valuePairs.Add(arr[0], arr[1]);
                else if (arr.Length == 2)
                    valuePairs.Add(arr[0], arr[1]);
            }

            Add(GetKey(selector, key), valuePairs);
        }


        public string GetString(string key)
        {
            return GetString(StyleSelector.Tag, key);
        }

        public string GetString(StyleSelector selector, string key)
        {
            var sb = new StringBuilder();

            foreach (var item in base[GetKey(selector, key)])
            {
                sb.Append(item.Key);
                sb.Append(" : ");
                sb.Append(item.Value);
                sb.Append(";\n");
            }

            return sb.ToString().TrimEnd();
        }

    }
}
