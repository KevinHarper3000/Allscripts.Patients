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
        @"DECLARE @MemberCategoryDiagnosis TABLE
            (
                MemberID INT NOT NULL,
                DiagnosisCategoryID INT NULL,
                DiagnosisID INT NULL
            );

            DECLARE @MinimumCategoryPerMember TABLE
            (
                MemberID INT NOT NULL,
                MinCategory INT NULL
            );

            DECLARE @MostSevereDiagnosisByCategoryPerPatient TABLE
            (
                MemberID INT NOT NULL,
                DiagnosisCategoryID INT NULL,
                MostSevereDiagnosisID INT NULL
            );	

            INSERT INTO @MemberCategoryDiagnosis
            SELECT DISTINCT
                  
                  md.MemberID,
                  sub2.DiagnosisCategoryID,
                  sub2.DiagnosisID

            FROM Pulse8TestDB.dbo.MemberDiagnosis AS md
            LEFT JOIN Pulse8TestDB.dbo.DiagnosisCategoryMap AS dcm
            ON md.DiagnosisID = dcm.DiagnosisID

            LEFT JOIN
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
            FROM @MemberCategoryDiagnosis
            GROUP BY MemberID


            INSERT INTO @MostSevereDiagnosisByCategoryPerPatient
            SELECT m.MemberID, m.DiagnosisCategoryID, MIN(DiagnosisID) AS MostSevereDiagnosisID
            FROM @MemberCategoryDiagnosis AS m
            GROUP BY m.MemberID, m.DiagnosisCategoryID

            SELECT DISTINCT
                m.MemberID,
                m.FirstName,
                m.LastName,
                mdc.DiagnosisCategoryID AS CategoryID,
                mdc.MostSevereDiagnosisID,
                d.DiagnosisDescription AS MostSevereDiagnosisDescription,
                dc.CategoryDescription,
                dc.CategoryScore,
                IsMostSevereCategory = 
                CASE
                WHEN mdc.DiagnosisCategoryID IN
                    (SELECT MinCategory WHERE mdc.MemberID = m.MemberID)
                    OR m.MemberID NOT IN (SELECT MemberID FROM MemberDiagnosis AS md)  
                    THEN 1 ELSE 0 END

            FROM Pulse8TestDB.dbo.Member AS m
            LEFT JOIN @MostSevereDiagnosisByCategoryPerPatient AS mdc
            ON m.MemberID = mdc.MemberID
            LEFT JOIN Pulse8TestDB.dbo.Diagnosis AS d
            ON mdc.MostSevereDiagnosisID = d.DiagnosisID
            LEFT JOIN Pulse8TestDB.dbo.DiagnosisCategory AS dc
            ON mdc.DiagnosisCategoryID = dc.DiagnosisCategoryID
            LEFT JOIN @MinimumCategoryPerMember AS mcpm
            ON m.MemberID = mcpm.MemberID
            WHERE 
            m.MemberID = @id
            AND
            (
                mdc.DiagnosisCategoryID IS NOT NULL
                OR
                m.MemberID NOT IN
                (SELECT MemberID FROM MemberDiagnosis AS md)
            )", new {id})
            .ToList();
    }
}