using Xunit;

namespace Apptech.Assessment.EntityFrameworkCore;

[CollectionDefinition(AssessmentTestConsts.CollectionDefinitionName)]
public class AssessmentEntityFrameworkCoreCollection : ICollectionFixture<AssessmentEntityFrameworkCoreFixture>
{

}
