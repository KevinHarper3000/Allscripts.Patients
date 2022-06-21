namespace Patients.DataAccess
{
    public class MostSeverePatientDiagnosisByCategory
    {
        public int MemberId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int MostSevereDiagnosisId { get; set; }

        public string MostSevereDiagnosisDescription { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryDescription { get; set; } = string.Empty;

        public int CategoryScore { get; set; }

        public bool IsMostSevereCategory { get; set; }
    }
}