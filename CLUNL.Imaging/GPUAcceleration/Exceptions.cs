using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging.GPUAcceleration
{

    [Serializable]
    public class CLProgramCompilationException : Exception
    {
        public CLProgramCompilationException():base("Failed on compiling given CL program.") { }
        
    }

    [Serializable]
    public class ReadArgumentException : Exception
    {
        public ReadArgumentException() : base("Cannot read target argument.") { }
    }


    [Serializable]
    public class CLRuntimeException : Exception
    {
        public CLRuntimeException():base("Error happened in CL runtime.") { }
    }
}
