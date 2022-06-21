#region

using Patients.ArgProcessor;
using Patients.DataAccess;

#endregion

namespace Patients.Console;

public class PatientsArgumentProcessor : BaseArgumentProcessor
{
    private const string InitialPrompt = "Enter a patient ID . . .";

    private const string InvalidIdPrompt =
        "The entered patient ID is not an integer. Enter a single integer patient ID.";

    private const string PatientNotFound = "A patient with the provided ID was not found.";

    private readonly IPatientRepository patientRepository;

    public PatientsArgumentProcessor(IPatientRepository patientRepository)
    {
        this.patientRepository = patientRepository;
    }

    protected override void PreProcess()
    {
        Output.WriteLine();
        Output.WriteLine(InitialPrompt);
    }

    protected override void ProcessLine(string line)
    {
        try
        {
            int patientId = int.Parse(line);
            List<MostSeverePatientDiagnosisByCategory> patientDiagnoses =
                patientRepository.GetMostSeverePatientDiagnosesPerCategory(patientId);

            if (patientDiagnoses.Count < 1)
            {
                Output.WriteLine();
                Output.WriteLine(PatientNotFound);
                PreProcess();
            }
            else
            {
                Output.WriteLine();
                patientDiagnoses.Output();
                PreProcess();
            }

        }
        catch (FormatException)
        {
            Output.WriteLine(InvalidIdPrompt);
        }
    }
}