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
        static CommandQueue CurrentQueue;
        static Context? CurrentContext = null;
        public static void SetGPU(int index)
        {
            if(CurrentContext is not null)
            {
                CurrentContext.Value.Dispose();
                CurrentQueue.Dispose();
            }
            if(index is >= 0)
            {
                if(index < devs.Count)
                {
                    GPUInUse = devs[index];
                }
            }
            var p=Cl.GetDeviceInfo(GPUInUse, DeviceInfo.Platform, out _).CastTo<Platform>();

            ErrorCode ec;
            CurrentContext=Cl.CreateContext(null,1,    new[] { GPUInUse}, null, IntPtr.Zero, out ec);
            if(ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
            CurrentQueue=Cl.CreateCommandQueue(CurrentContext.Value, GPUInUse, (CommandQueueProperties)0, out ec);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
        }
        public static Kernel Compile(string sourceCode,string targetKernel)
        {
            ErrorCode ec;
            Program program = Cl.CreateProgramWithSource(CurrentContext.Value, 1, new[] { sourceCode }, null, out ec);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception("Cannot create program.");
            }
            ec =Cl.BuildProgram(program, 1, new[] { GPUInUse }, string.Empty, null, IntPtr.Zero);
            if (ec is not ErrorCode.Success)
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
        public static void SetArg(Kernel kernel,Bool BlockWrite,int index,int intPtrSize,IMem DeviceMem,int Size,object RealPara,bool WriteBuffer=true)
        {
            _=Cl.SetKernelArg(kernel, 0, new IntPtr(intPtrSize), DeviceMem);
            if(WriteBuffer)
            Cl.EnqueueWriteBuffer(CurrentQueue, DeviceMem, BlockWrite, IntPtr.Zero,
                    new IntPtr(Size),
                    RealPara, 0, null, out _);
        }
        public static IMem CreateBuffer(MemFlags flag,int Size)
        {
            return Cl.CreateBuffer(CurrentContext.Value, MemFlags.CopyHostPtr | flag, Size, out _);
        }
        public static void Execute(Kernel kernel,int size)
        {
            Cl.EnqueueNDRangeKernel(CurrentQueue, kernel, 1, null, new IntPtr[] {new IntPtr(size) }, null, 0, null, out _);
            Cl.EnqueueBarrier(CurrentQueue);
        }
        public static void ReadArg<T>(IMem DeviceMem,ref T[] obj) where T : struct
        {
            IntPtr event_handle = IntPtr.Zero;
            _= Cl.EnqueueReadBuffer(CurrentQueue, DeviceMem, Bool.True, 0, obj.Length, obj, 0, null, out _);
        }
    }
}
