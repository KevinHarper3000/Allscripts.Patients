#region

using System.Data;
using System.Data.SqlClient;
using Dapper;

#endregion

namespace Patients.DataAccess;

public class PatientRepository : IPatientRepository
{
    private readonly IDbConnection connection;

    public PatientRepository(string connection)
    {
        this.connection = new SqlConnection(connection);
    }

    public List<MostSeverePatientDiagnosisByCategory> GetMostSeverePatientDiagnosesPerCategory(int id)
    {
        return connection.Query<MostSeverePatientDiagnosisByCategory>(
    @"DECLARE @MemberDiagnosisCategory TABLE
        (
           MemberID INT NOT NULL,
           DiagnosisCategoryID INT NULL,
           MostSevereDiagnosisID INT NULL
        );

        DECLARE @MinimumCategoryPerMember TABLE
        (
          MemberID INT NOT NULL,
          MinCategory INT NULL
        );


        INSERT INTO @MemberDiagnosisCategory
        SELECT DISTINCT
          md.MemberID,
          sub2.DiagnosisCategoryID,
          sub3.MostSevereDiagnosisID

          FROM Pulse8TestDB.dbo.MemberDiagnosis AS md
          LEFT JOIN Pulse8TestDB.dbo.DiagnosisCategoryMap AS dcm
          ON md.DiagnosisID = dcm.DiagnosisID
        
        LEFT JOIN
        (
          SELECT
            DiagnosisCategoryID,
            MIN(dcm.DiagnosisID) AS dID
            
            FROM Pulse8TestDB.dbo.MemberDiagnosis AS md
            LEFT JOIN Pulse8TestDB.dbo.DiagnosisCategoryMap AS dcm
            ON md.DiagnosisID = dcm.DiagnosisID
            GROUP BY DiagnosisCategoryID
        ) AS sub1
            
            ON dcm.DiagnosisID = sub1.dID
        
        JOIN
        (
          SELECT DISTINCT
            md.MemberID,
            dcm.DiagnosisCategoryID,
            d.DiagnosisID,
            d.DiagnosisDescription
            
            FROM Pulse8TestDB.dbo.MemberDiagnosis AS md
            LEFT JOIN Pulse8TestDB.dbo.DiagnosisCategoryMap AS dcm
            ON md.DiagnosisID = dcm.DiagnosisID
            INNER JOIN Pulse8TestDB.dbo.Diagnosis AS d
            ON dcm.DiagnosisID = d.DiagnosisID
        ) AS sub2
        
        ON dcm.DiagnosisCategoryID = sub2.DiagnosisCategoryID
        AND md.MemberID = sub2.MemberID
        
        JOIN
        (
          SELECT DISTINCT
            md.MemberID,
            MIN(md.DiagnosisID) AS MostSevereDiagnosisID
            FROM Pulse8TestDB.dbo.MemberDiagnosis AS md
            GROUP BY md.MemberID
        ) AS sub3
        ON md.MemberID = sub3.MemberID;
        
       INSERT INTO @MinimumCategoryPerMember
       SELECT
         MemberID, MIN(DiagnosisCategoryID)
         FROM @MemberDiagnosisCategory
         GROUP BY MemberID
        
       SELECT DISTINCT
         m.MemberID,
         m.FirstName,
         m.LastName,
         mdc.MostSevereDiagnosisID,
         d.DiagnosisDescription AS MostSevereDiagnosisDescription,
         mdc.DiagnosisCategoryID AS CategoryID,
         dc.CategoryDescription,
         dc.CategoryScore,
         IsMostSevereCategory =
         CASE
         WHEN mdc.DiagnosisCategoryID IN
          (SELECT MinCategory WHERE mdc.MemberID = m.MemberID) THEN 1 ELSE 0 END
         
         FROM Pulse8TestDB.dbo.Member AS m
         LEFT JOIN Pulse8TestDB.dbo.MemberDiagnosis AS md
         ON m.MemberID = md.MemberID
         LEFT JOIN @MemberDiagnosisCategory AS mdc
         ON m.MemberID = mdc.MemberID
         LEFT JOIN Pulse8TestDB.dbo.Diagnosis AS d
         ON MostSevereDiagnosisID = d.DiagnosisID
         LEFT JOIN Pulse8TestDB.dbo.DiagnosisCategory AS dc
         ON mdc.DiagnosisCategoryID = dc.DiagnosisCategoryID
         LEFT JOIN @MinimumCategoryPerMember AS mcpm
         ON m.MemberID = mcpm.MemberID
         WHERE m.MemberID = @id", new {id})
         .ToList();
    }
}