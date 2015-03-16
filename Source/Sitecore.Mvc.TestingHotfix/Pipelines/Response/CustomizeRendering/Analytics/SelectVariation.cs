namespace Sitecore.Mvc.TestingHotfix.Pipelines.Response.CustomizeRendering.Analytics
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using Sitecore.Analytics.Testing;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Layouts;
    using Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering;
    using Sitecore.Mvc.Extensions;
    using Sitecore.Mvc.Presentation;
    using Sitecore.SecurityModel;

    public class SelectVariation : Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering.SelectVariation
    {
        protected override void Evaluate(CustomizeRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var renderingReference = GetRenderingReference(args.Rendering, Context.Language, args.PageContext.Database);
            if (string.IsNullOrEmpty(renderingReference.Settings.MultiVariateTest))
            {
                return;
            }

            using (new SecurityDisabler())
            {
                var mvVariateTestForLang = renderingReference.Settings.GetMultiVariateTestForLanguage(Context.Language);
                Sitecore.Data.Items.Item variableItem = null;

                if (mvVariateTestForLang != null)
                {
                    variableItem = args.PageContext.Database.GetItem(mvVariateTestForLang);
                }

                if (variableItem == null)
                {
                    return;
                }

                var variation = this.GetVariation(variableItem);
                if (variation == null)
                {
                    return;
                }

                var context = new ComponentTestContext(
                    variation,
                    renderingReference,
                    new List<RenderingReference> { renderingReference });
                this.ApplyVariation(args, context);
            }

            args.IsCustomized = true;
        }

        public static RenderingReference GetRenderingReference(
            Rendering rendering,
            Language language,
            Database database)
        {
            Assert.IsNotNull(rendering, "rendering");
            Assert.IsNotNull(language, "language");
            Assert.IsNotNull(database, "database");

            var text = rendering.Properties["RenderingXml"];
            return string.IsNullOrEmpty(text)
                       ? null
                       : new RenderingReference(XElement.Parse(text).ToXmlNode(), language, database);
        }
    }
}
