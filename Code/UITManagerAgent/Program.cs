using System;
using System.Collections.Generic;
using UITManagerAgent.DataCollectors;

public class Program
{
    public static async Task Main(string[] args)
    {
        await RunOnce();
    }

    private static Task RunOnce()
    {
        UserCollector userCollector = new UserCollector();
        Console.WriteLine(userCollector.Collect().ToString());
        CpuCollectors cupCollected = new CpuCollectors();
        Console.WriteLine(cupCollected.Collect().ToString());
        return Task.CompletedTask;
    }
}