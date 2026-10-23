using AetherWM.Platform.Win32.Native;

namespace AetherWM.Platform.Win32.Services;

public class HardwareMonitorService
{
    private ulong _prevIdleTime;
    private ulong _prevKernelTime;
    private ulong _prevUserTime;
    private bool _hasPrevTimes;

    public double GetCpuUsagePercent()
    {
        if (!Kernel32.GetSystemTimes(out var idle, out var kernel, out var user))
            return 0.0;

        ulong currentIdle = idle.ToULong();
        ulong currentKernel = kernel.ToULong();
        ulong currentUser = user.ToULong();

        if (!_hasPrevTimes)
        {
            _prevIdleTime = currentIdle;
            _prevKernelTime = currentKernel;
            _prevUserTime = currentUser;
            _hasPrevTimes = true;
            return 0.0;
        }

        ulong idleDiff = currentIdle - _prevIdleTime;
        ulong kernelDiff = currentKernel - _prevKernelTime;
        ulong userDiff = currentUser - _prevUserTime;

        _prevIdleTime = currentIdle;
        _prevKernelTime = currentKernel;
        _prevUserTime = currentUser;

        ulong totalSys = kernelDiff + userDiff;
        if (totalSys == 0) return 0.0;

        // In Windows, kernel time includes idle time, so total system time is kernel + user, and active time is total - idle
        double cpu = (double)(totalSys - idleDiff) * 100.0 / totalSys;
        return Math.Clamp(cpu, 0.0, 100.0);
    }

    public (double UsedGb, double TotalGb, double Percent) GetRamUsage()
    {
        var memStatus = Kernel32.MEMORYSTATUSEX.Create();
        if (Kernel32.GlobalMemoryStatusEx(ref memStatus))
        {
            double totalGb = (double)memStatus.ullTotalPhys / (1024 * 1024 * 1024);
            double availGb = (double)memStatus.ullAvailPhys / (1024 * 1024 * 1024);
            double usedGb = totalGb - availGb;
            double percent = memStatus.dwMemoryLoad;

            return (Math.Round(usedGb, 1), Math.Round(totalGb, 1), Math.Round(percent, 0));
        }

        return (0, 0, 0);
    }
}
