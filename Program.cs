// Parse Arguement: --output=./Icons/battery_status.svg
// Parse Arguement: --restart-tiny-dfr

using System.Diagnostics;

string outputPath = string.Empty;
bool restartTinyDfr = false;

foreach (string arg in args)
{
    if (arg.StartsWith("--output=", StringComparison.OrdinalIgnoreCase))
    {
        outputPath = arg["--output=".Length..];
        if (!string.IsNullOrEmpty(outputPath))
        {
            Console.WriteLine($"Output path set to: {outputPath}");
        }
        else
        {
            Console.Error.WriteLine("Invalid output path specified.");
            return;
        }
    }

    if (arg.Equals("--restart-tiny-dfr", StringComparison.OrdinalIgnoreCase))
    {
        restartTinyDfr = true;
    }
}

bool running = true;
AppDomain.CurrentDomain.ProcessExit += (s, e) => running = false;

BatteryInfo battery = new();

string currentIcon = "./Icons/unknown.svg";
File.Copy(currentIcon, outputPath, overwrite: true);

while (running)
{
    string icon;
    if (battery.IsValid)
    {
        icon = battery switch
        {
            { Charging: true, Percentage: null } => "./Icons/ac.svg",
            { Charging: true, Percentage: int percentage } => percentage switch
            {
                >= 96 => "./Icons/charging7.svg",
                >= 84 => "./Icons/charging6.svg",
                >= 70 => "./Icons/charging5.svg",
                >= 56 => "./Icons/charging4.svg",
                >= 42 => "./Icons/charging3.svg",
                >= 28 => "./Icons/charging2.svg",
                >= 14 => "./Icons/charging1.svg",
                _ => "./Icons/charging0.svg",
            },
            { Charging: false, Percentage: int percentage } => percentage switch
            {
                >= 96 => "./Icons/7.svg",
                >= 84 => "./Icons/6.svg",
                >= 70 => "./Icons/5.svg",
                >= 56 => "./Icons/4.svg",
                >= 42 => "./Icons/3.svg",
                >= 28 => "./Icons/2.svg",
                >= 14 => "./Icons/1.svg",
                _ => "./Icons/0.svg",
            },
            _ => "./Icons/alert.svg",
        };
    }
    else
    {
        icon = "./Icons/unknown.svg";
    }

    if (currentIcon != icon)
    {
        Console.WriteLine($"Battery icon changed: {currentIcon} -> {icon}");
        currentIcon = icon;
        File.Copy(icon, outputPath, overwrite: true);

        // Run "service tiny-dfr restart"
        if (restartTinyDfr)
        {
            try
            {
                Console.WriteLine("Restarting tiny-dfr service...");
                await TinyDFRHelper.RestartService();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to restart tiny-dfr service: {ex.Message}");
            }
        }
    }

    await Task.Delay(1000);
}

Console.WriteLine("Exiting...");

static class TinyDFRHelper
{
    public static async Task RestartService()
    {
        Process? process = null;
        try
        {
            process = Process.Start("service", "tiny-dfr restart");
            await process.WaitForExitAsync();
        }
        catch
        {
            throw;
        }
        finally
        {
            process?.Dispose();
        }
    }
}

class BatteryInfo
{
    private readonly bool _valid;

    private readonly string? _battery_path;

    public BatteryInfo()
    {
        string? path = Directory.GetDirectories("/sys/class/power_supply")
            .Select(dir => new DirectoryInfo(dir))
            .Where(info => info.Name.Contains("battery", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault()
            ?.FullName;

        if (string.IsNullOrEmpty(path))
            Console.Error.WriteLine("No battery found.");

        _battery_path = path;

        _valid = !string.IsNullOrEmpty(_battery_path) && Directory.Exists(_battery_path);
    }

    public bool IsValid => _valid;

    public bool Charging => _battery_path is null
        ? throw new InvalidOperationException("No valid battery")
        : File.ReadAllText(Path.Combine(_battery_path, "status")).Trim()
            .Equals("Charging", StringComparison.OrdinalIgnoreCase);

    public int? Percentage => _battery_path is null
        ? throw new InvalidOperationException("No valid battery")
        : int.TryParse(File.ReadAllText(Path.Combine(_battery_path, "capacity")).Trim(), out int capacity)
            ? capacity : null;
}
