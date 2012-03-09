using System;
using System.Collections.Generic;
using System.Text;

namespace MLifterTest.Tools
{
    public class TestTimeExceededException : Exception
    {
        public TestTimeExceededException(string message)
            : base(message)
        {
        }

        public TestTimeExceededException()
            : base("Test time limit was missed.")
        {
        }
    }
}
