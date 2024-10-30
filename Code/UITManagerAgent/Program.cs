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
        DiskCollector diskCollector = new DiskCollector();
        Console.WriteLine(diskCollector.Collect().ToString());
        return Task.CompletedTask;
    }
}