using Apptech.Assessment.Samples;
using Xunit;

namespace Apptech.Assessment.EntityFrameworkCore.Domains;

[Collection(AssessmentTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<AssessmentEntityFrameworkCoreTestModule>
{

}
