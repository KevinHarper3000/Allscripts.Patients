using Microsoft.Extensions.Configuration;
using Patients.DataAccess;
using static System.Console;


namespace Patients.Console;

internal class Program
{
    private static IConfigurationRoot config;

    private static void Main(string[] args)
    {
        Initialize();
        IPatientRepository repository = CreateRepository();
        PatientsArgumentProcessor argProcessor = new(repository);
        argProcessor.Process(args);
    }

    private static void Initialize()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json");
        config = builder.Build();
    }

    private static IPatientRepository CreateRepository()
    {
        return new PatientRepository(config.GetConnectionString("DefaultConnectionString"));
    }
}