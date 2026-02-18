using IpsaeService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "IpsaeIDS";
        });
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}