using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

//
//  DataSafe.cs
//  DataSafe
// 
//  Created by David Südholt on 20/04/2015
//  Copyright (c) 2015 Forschungszentrum Jülich IEK-5. All rights reserved.
//

namespace LaserControl.Data
{
    public class DataSafe
    {
        private XElement data;
        private string path;
        private HashSet<string> names;

        public IntGetter Ints;
        public LongGetter Longs;
        public BoolGetter Bools;
        public StringGetter Strings;
        public DoubleGetter Doubles;

        public DataSafe(string path, string name)
        {
            Ints = new IntGetter(this);
            Longs = new LongGetter(this);
            Bools = new BoolGetter(this);
            Strings = new StringGetter(this);
            Doubles = new DoubleGetter(this);


            path = path + Path.DirectorySeparatorChar + name;
            if(!path.EndsWith(".xml",true,System.Globalization.CultureInfo.CurrentCulture))
                path +=".xml";

            this.path = path;
            this.names = new HashSet<string>();
            if (!File.Exists(path))
            {
                data = new XElement(name);
                data.Save(path);
            }
            else
            {
                data = XElement.Load(path);
                foreach (XElement elem in data.Descendants())
                {
                    names.Add(elem.Name.ToString());
                }
            }
        }

        public void setInteger(string key, int val)
        {
            XElement elem = getElem(key);
            if (elem != null)
            {
                checkType(elem, "integer");
                elem.Value = val.ToString();
            }
            else
            {
                elem = new XElement(key, val);
                elem.SetAttributeValue("type", "integer");
                data.Add(elem);
                names.Add(key);
            }
            data.Save(path);
        }

        public int getInteger(string key)
        {
            XElement elem = getElem(key);
            if (elem == null)
            {
                throw new ElementNotFoundException(key);
            }
            checkType(elem, "integer");
            return Int32.Parse(elem.Value);
        }

        public void setDouble(string key, double val)
        {
            XElement elem = getElem(key);
            if (elem != null)
            {
                checkType(elem, "double");
                elem.Value = val.ToString();
            }
            else
            {
                elem = new XElement(key, val);
                elem.SetAttributeValue("type", "double");
                data.Add(elem);
                names.Add(key);
            }
            data.Save(path);
        }

        public double getDouble(string key)
        {
            XElement elem = getElem(key);
            if (elem == null)
            {
                throw new ElementNotFoundException(key);
            }
            checkType(elem, "double");
            return Double.Parse(elem.Value);
        }

        public void setLong(string key, long val)
        {
            XElement elem = getElem(key);
            if (elem != null)
            {
                checkType(elem, "long");
                elem.Value = val.ToString();
            }
            else
            {
                elem = new XElement(key, val);
                elem.SetAttributeValue("type", "long");
                data.Add(elem);
                names.Add(key);
            }
            data.Save(path);
        }

        public long getLong(string key)
        {
            XElement elem = getElem(key);
            if (elem == null)
            {
                throw new ElementNotFoundException(key);
            }
            checkType(elem, "long");
            return long.Parse(elem.Value);
        }

        public void setBoolean(string key, bool val)
        {
            XElement elem = getElem(key);
            if (elem != null)
            {
                checkType(elem, "boolean");
                elem.Value = val.ToString();
            }
            else
            {
                elem = new XElement(key, val);
                elem.SetAttributeValue("type", "boolean");
                data.Add(elem);
                names.Add(key);
            }
            data.Save(path);
        }

        public bool getBoolean(string key)
        {
            XElement elem = getElem(key);
            if (elem == null)
            {
                throw new ElementNotFoundException(key);
            }
            checkType(elem, "boolean");
            return Boolean.Parse(elem.Value);
        }

        public void setString(string key, string val)
        {
            XElement elem = getElem(key);
            if (elem != null)
            {
                checkType(elem, "string");
                if (val == null)
                    val = "";
                elem.Value = val.ToString();
            }
            else
            {
                elem = new XElement(key, val);
                elem.SetAttributeValue("type", "string");
                data.Add(elem);
                names.Add(key);
            }
            data.Save(path);
        }

        public string getString(string key)
        {
            XElement elem = getElem(key);
            if (elem == null)
            {
                //throw new ElementNotFoundException(key);
                return "";
            }
            checkType(elem, "string");
            return elem.Value;
        }

        private XElement getElem(string key)
        {
            if (!names.Contains(key)) return null;
            var q =
                from elem in data.Descendants()
                where elem.Name.ToString() == key
                select elem;
            return q.First();
        }

        private void checkType(XElement elem, string type)
        {
            string type2 = elem.Attribute("type").Value;
            if (type != type2)
            {
                throw new WrongElementTypeException(elem.Name.ToString(), type2, type);
            }
        }

        public bool containsKey(string key)
        {
            return names.Contains(key);
        }
    }
}
