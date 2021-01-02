using OpenCL.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUNL.Imaging.GPUAcceleration
{
    public class CommonGPUAcceleration
    {
        static CommonGPUAcceleration()
        {
            InitLibrary();
        }
        static List<Device> devs = new ();
        static List<Platform> platforms = new ();
        public static void InitLibrary()
        {
            {
                //Get all gpus.
                ErrorCode ec;
                var Platforms = Cl.GetPlatformIDs(out ec);
                platforms.AddRange(Platforms);
                foreach (var item in Platforms)
                {
                    devs.AddRange(Cl.GetDeviceIDs(item, DeviceType.Gpu, out ec));
                }
                SetGPU(0);// Ensure that there is always a GPU selected.
            }
        }
        public static List<Platform> EnumeratePlatforms() => platforms;
        public static List<Device> EnumerateGPUs()
        {
            return devs;
        }
        static Device GPUInUse;
        static Context? CurrentContext = null;
        public static void SetGPU(int index)
        {
            if(CurrentContext is not null)
            {
                CurrentContext.Value.Dispose();
            }
            if(index is >= 0)
            {
                if(index < devs.Count)
                {
                    GPUInUse = devs[index];
                }
            }
            var p=Cl.GetDeviceInfo(GPUInUse, DeviceInfo.Platform, out _).CastTo<Platform>();

            CurrentContext=Cl.CreateContext(null,1,    new[] { GPUInUse}, null, IntPtr.Zero, out _);
        }
        public static Kernel Compile(string sourceCode,string targetKernel)
        {
            ErrorCode ec;
            Program program = Cl.CreateProgramWithSource(CurrentContext.Value, 1, new[] { sourceCode }, null, out ec);
            if(ec is not ErrorCode.Success)
            {
                throw new Exception("Cannot create program.");
            }

            Kernel kernel = Cl.CreateKernel(program, targetKernel, out ec);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception("Cannot create kernel.");
            }

            return kernel;
        }
    }
}
