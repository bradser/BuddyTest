﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BuddyTest
{
    public class EnumHelper<EnumType> 
    {
        private Type enumType;

        public EnumHelper()
        {
            this.enumType = typeof(EnumType);
 
            Debug.Assert(this.enumType.IsEnum);
        }

        public IOrderedEnumerable<string> AlphabeticalNames
        {
            get
            {
                return this.Names.OrderBy(name => name);
            }
        }

        public IEnumerable<string> Names
        {
            get
            {
                return this.enumType.GetFields().Where(fi => fi.IsLiteral).Select(fi => fi.Name);
            }
        }
        
        public EnumType GetValue(string name)
        {
            return (EnumType)Enum.Parse(this.enumType, name, false);
        }
    }
}
