using System.Threading.Tasks;

namespace Apptech.Assessment.Data;

public interface IAssessmentDbSchemaMigrator
{
    Task MigrateAsync();
}
