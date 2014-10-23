namespace Sitecore.Mvc.TestingHotfix.Pipelines.Response.CustomizeRendering.ExperienceEditor
{
    using System.Linq;

    using Sitecore.Analytics.Data.Items;
    using Sitecore.Analytics.Shell.Applications.WebEdit;
    using Sitecore.Analytics.Testing.TestingUtils;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering;
    using Sitecore.Web;

    public class SelectVariation : Analytics.SelectVariation
    {
        public override void Process(CustomizeRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.IsCustomized || !Context.PageMode.IsPageEditor || !Settings.Analytics.Enabled)
            {
                return;
            }

            this.Evaluate(args);
        }

        protected override MultivariateTestValueItem GetVariation(Item variableItem)
        {
            Assert.ArgumentNotNull((object)variableItem, "variableItem");

            var variable = (MultivariateTestVariableItem)variableItem;
            if (variable == null)
            {
                return null;
            }

            var testDefinition = (MultivariateTestDefinitionItem)variableItem.Parent;
            if (testDefinition != null)
            {
                UpdateTestSettings(testDefinition);
            }

            return TestingUtil.MultiVariateTesting.GetVariableValues(variable).LastOrDefault();
        }

        private static void UpdateTestSettings(MultivariateTestDefinitionItem testDefinition)
        {
            Assert.ArgumentNotNull((object)testDefinition, "testDefinition");

            if (WebEditUtil.Testing.CurrentSettings != null)
            {
                return;
            }

            var isTestRunning = TestingUtil.MultiVariateTesting.IsTestRunning(testDefinition);
            WebEditUtil.Testing.CurrentSettings = new WebEditUtil.Testing.TestSettings(
                testDefinition,
                WebEditUtil.Testing.TestType.Multivariate,
                isTestRunning);
            if (!isTestRunning)
            {
                return;
            }

            var testDefinitionItem = (TestDefinitionItem)testDefinition;
            Assert.IsNotNull(testDefinitionItem, "testDefinitionItem");
            PageStatisticsContext.SaveTestStatisticsToSession(
                PageStatisticsContext.GetTestStatistics(testDefinitionItem, true, false));
        }
    }
}
