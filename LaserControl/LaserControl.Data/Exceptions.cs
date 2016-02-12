using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Exceptions.cs
//  ElementNotFoundException, WrongElementTypeException
// 
//  Created by David Südholt on 20/04/2015
//  Copyright (c) 2015 Forschungszentrum Jülich IEK-5. All rights reserved.
//

namespace LaserControl.Data
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string elem)
            : base("Element not found: " + elem) { }
    }

    public class WrongElementTypeException : Exception
    {
        public WrongElementTypeException(string elem, string type1, string type2)
            : base(String.Format("Element {0} is {1}, not {2}", elem, type1, type2)) { }
    }
}
