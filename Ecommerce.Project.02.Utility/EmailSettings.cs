﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Project._02.Utility
{
    public class EmailSettings
    {
        public String PrimaryDomain { get; set; }
        public int PrimaryPort { get; set; }
        public String SecondaryDomain { get; set; }
        public int SecondaryPort { get; set; }
        public String UsernameEmail {  get; set; }
        public String UsernamePassword {  get; set; }
        public String FromEmail {  get; set; }
        public String ToEmail {  get; set; }
        public String CcEmail {  get; set; }
    }
}
