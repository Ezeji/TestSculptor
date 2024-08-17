using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSculptor
{
    public class DbConfigurationFailedException : Exception
    {
        public DbConfigurationFailedException() { }
        public DbConfigurationFailedException(string message) : base(message) { }
        public DbConfigurationFailedException(string message, Exception inner) : base(message, inner) { }
    }
}
