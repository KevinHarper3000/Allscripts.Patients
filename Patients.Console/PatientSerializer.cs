using YamlDotNet.Serialization;

namespace Patients.Console;

public static class PatientSerializer
{
    public static void Output(this object item)
    {
        ISerializer serializer = new SerializerBuilder().Build();
        string yaml = serializer.Serialize(item);
        System.Console.WriteLine(yaml);
    }
}