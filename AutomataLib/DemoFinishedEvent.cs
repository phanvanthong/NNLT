using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataLib
{
    public class DemoFinishedEvent : EventArgs
    {
        public bool LanguageIsAccepted { get; set; }
    }
}
