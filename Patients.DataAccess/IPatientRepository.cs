namespace Patients.DataAccess;

public interface IPatientRepository
{
    List<MostSeverePatientDiagnosisByCategory> GetMostSeverePatientDiagnosesPerCategory(int id);
}