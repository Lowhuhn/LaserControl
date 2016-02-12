using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Getter.cs
//  BoolGetter, DoubleGetter, IntGetter, LongGetter, StringGetter
// 
//  Created by David Südholt on 20/04/2015
//  Copyright (c) 2015 Forschungszentrum Jülich IEK-5. All rights reserved.
//

namespace LaserControl.Data
{
    public class IntGetter
    {
        private DataSafe ds;
        public int this[string key]
        {
            get
            {
                return ds.getInteger(key);
            }
            set
            {
                ds.setInteger(key, value);
            }
        }

        public int this[string key, int def]
        {
            get
            {
                if (!ds.containsKey(key))
                    return def;
                return this[key];
            }
        }

        public IntGetter(DataSafe ds)
        {
            this.ds = ds;
        }
    }

    public class DoubleGetter
    {
        private DataSafe ds;
        public double this[string key]
        {
            get
            {
                return ds.getDouble(key);
            }
            set
            {
                ds.setDouble(key, value);
            }
        }

        public double this[string key, double def]
        {
            get
            {
                if (!ds.containsKey(key))
                    return def;
                return this[key];
            }
        }

        public DoubleGetter(DataSafe ds)
        {
            this.ds = ds;
        }
    }

    public class BoolGetter
    {
        private DataSafe ds;
        public bool this[string key]
        {
            get
            {
                return ds.getBoolean(key);
            }
            set
            {
                ds.setBoolean(key, value);
            }
        }

        public bool this[string key, bool def]
        {
            get
            {
                if (!ds.containsKey(key))
                    return def;
                return this[key];
            }
        }

        public BoolGetter(DataSafe ds)
        {
            this.ds = ds;
        }
    }

    public class LongGetter
    {
        private DataSafe ds;
        public long this[string key]
        {
            get
            {
                return ds.getLong(key);
            }
            set
            {
                ds.setLong(key, value);
            }
        }

        public LongGetter(DataSafe ds)
        {
            this.ds = ds;
        }
    }

    public class StringGetter
    {
        private DataSafe ds;
        public string this[string key]
        {
            get
            {
                return ds.getString(key);
            }
            set
            {
                ds.setString(key, value);
            }
        }

        public string this[string key, string def]
        {
            get
            {
                if (!ds.containsKey(key))
                    return def;
                return this[key];
            }
        }

        public StringGetter(DataSafe ds)
        {
            this.ds = ds;
        }
    }
}
