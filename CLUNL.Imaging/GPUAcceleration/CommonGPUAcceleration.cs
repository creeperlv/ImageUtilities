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
        static List<Device> devs = new();
        static List<Platform> platforms = new();
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
            if (CurrentContext is not null)
            {
                CurrentContext.Value.Dispose();
                CurrentQueue.Dispose();
            }
            if (index is >= 0)
            {
                if (index < devs.Count)
                {
                    GPUInUse = devs[index];
                }
            }
            var p = Cl.GetDeviceInfo(GPUInUse, DeviceInfo.Platform, out _).CastTo<Platform>();

            ErrorCode ec;
            CurrentContext = Cl.CreateContext(null, 1, new[] { GPUInUse }, null, IntPtr.Zero, out ec);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
            CurrentQueue = Cl.CreateCommandQueue(CurrentContext.Value, GPUInUse, (CommandQueueProperties)0, out ec);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
        }
        static Program program;
        public static Kernel Compile(string sourceCode, string targetKernel)
        {
            ErrorCode ec;
            program = Cl.CreateProgramWithSource(CurrentContext.Value, 1, new[] { sourceCode }, null, out ec);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception("Cannot create program.");
            }
            ec = Cl.BuildProgram(program, 1, new[] { GPUInUse }, string.Empty, null, IntPtr.Zero);
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
        public static void SetArg<T>(Kernel kernel, Bool BlockWrite, uint index, int intPtrSize, IMem DeviceMem, int Size, T RealPara, bool WriteBuffer = true) where T : struct
        {
            ErrorCode ec;
            ec = Cl.SetKernelArg(kernel, index, DeviceMem);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
            if (WriteBuffer) ec =
             Cl.EnqueueWriteBuffer(CurrentQueue, DeviceMem, BlockWrite, IntPtr.Zero,
                     new IntPtr(Size),
                     RealPara, 0, null, out _);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
        }
        public static void SetArg<T>(Kernel kernel, Bool BlockWrite, uint index, IMem<T> DeviceMem, int Size, T[] RealPara, bool WriteBuffer = true) where T : struct
        {
            ErrorCode ec;
            ec = Cl.SetKernelArg<T>(kernel, index, DeviceMem);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
            if (WriteBuffer) ec =
             Cl.EnqueueWriteBuffer<T>(CurrentQueue, DeviceMem, BlockWrite,
                     RealPara, 0, null, out _);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
        }
        public static IMem CreateBuffer(MemFlags flag, int Size)
        {
            ErrorCode ec;
            var c = Cl.CreateBuffer(CurrentContext.Value, flag, Size, out ec);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
            return c;
        }
        public static IMem<T> CreateBuffer<T>(MemFlags flag, int Size) where T:struct
        {
            ErrorCode ec;
            IMem<T> c = Cl.CreateBuffer<T>(CurrentContext.Value, flag, Size, out ec);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
            return c;
        }
        public static void Execute(Kernel kernel, uint WorkDim, IntPtr[] GlobalWorkSize, int size)
        {
            ErrorCode ec;
            ec = Cl.EnqueueNDRangeKernel(CurrentQueue, kernel, WorkDim, null, GlobalWorkSize, null, 0, null, out _);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
            Cl.EnqueueBarrier(CurrentQueue);
        }
        public static void ReadArg<T>(IMem DeviceMem, ref T[] obj) where T : struct
        {
            IntPtr event_handle = IntPtr.Zero;
            ErrorCode ec
             = Cl.EnqueueReadBuffer(CurrentQueue, DeviceMem, Bool.True, 0, obj.Length, obj, 0, null, out _);
            if (ec is not ErrorCode.Success)
            {
                throw new Exception(ec.ToString());
            }
        }
    }
}
