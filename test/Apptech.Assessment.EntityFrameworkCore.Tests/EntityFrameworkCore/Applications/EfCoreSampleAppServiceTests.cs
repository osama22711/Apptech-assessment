using Apptech.Assessment.Samples;
using Xunit;

namespace Apptech.Assessment.EntityFrameworkCore.Applications;

[Collection(AssessmentTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<AssessmentEntityFrameworkCoreTestModule>
{

}
