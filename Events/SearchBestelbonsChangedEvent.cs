﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Events
{
   public class SearchBestelbonsChangedEvent
    {
        public string Search { get; set; }

        public SearchBestelbonsChangedEvent(string message)
        {
            Search = message;
        }
    }
}
