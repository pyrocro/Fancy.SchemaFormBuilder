﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Fancy.SchemaFormBuilder.Annotations;
using Fancy.SchemaFormBuilder.Providers;
using Newtonsoft.Json.Linq;

namespace Fancy.SchemaFormBuilder.Services.FormModules
{
    /// <summary>
    /// Adds help information to the form if the current property has the help attribute.
    /// </summary>
    public class HelpFormModule : FormModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelpFormModule" /> class.
        /// </summary>
        /// <param name="languageProvider">The language provider.</param>
        public HelpFormModule(ILanguageProvider languageProvider) : base(languageProvider)
        {
        }

        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <param name="context">The context to process.</param>
        public override void Process(FormBuilderContext context)
        {
            // Get all help attributes
            List<FormHelpAttribute> helps = context.Property.GetCustomAttributes<FormHelpAttribute>().ToList();

            // If the current property has no helps this module has nothing to to
            if (!helps.Any())
            {
                return;
            }

            foreach (FormHelpAttribute help in helps)
            {
                string helpCssClases = DetermineHelpCssClasses(help.HelpType);

                string helpText = GetTextForKey(help.HelpText, context);

                // Create the help JSON object
                JObject helpObject = new JObject();
                helpObject["type"] = new JValue("help");
                helpObject["helpvalue"] = new JValue("<div class=\"" + helpCssClases + "\" >" + helpText + "</div>");

                if (!string.IsNullOrEmpty(help.Condition))
                {
                    helpObject["condition"] = ConvertConditionToAbsolutePath(context.DtoType.Name, context.FullPropertyPath, help.Condition);
                }

                context.CurrentFormElementParent.Add(helpObject);
            }
        }

        /// <summary>
        /// Determines the help CSS classes.
        /// </summary>
        /// <param name="helpType">Type of the help.</param>
        /// <returns>The CSS classes to use for help html item.</returns>
        private string DetermineHelpCssClasses(HelpType helpType)
        {
            switch (helpType)
            {
                case HelpType.Info:
                    return "alert alert-info";

                case HelpType.Warning:
                    return "alert alert-warning";

                case HelpType.Error:
                    return "alert alert-error";

                default:
                    return "alert";
            }
        }

        /// <summary>
        /// Converts the condition expression to absolute path.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="fullPropertyPath">The full property path.</param>
        /// <param name="conditionExpression">The condition expression.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        private string ConvertConditionToAbsolutePath(string typeName, string fullPropertyPath, string conditionExpression)
        {
            string fullPathToObject = "model";

            // Check the property path is valid
            if (!conditionExpression.Contains('.'))
            {
                throw new InvalidOperationException("The condition must start with the type name and must navigate to a property.");
            }

            // Set up the path to the object
            if (fullPropertyPath.Contains('.'))
            {
                // Clip the last path item because we want only the path to the object and not to the property itself
                fullPathToObject = fullPathToObject + "." + fullPropertyPath.Substring(0, fullPropertyPath.LastIndexOf('.'));
            }

            return conditionExpression.Replace(typeName, fullPathToObject);
        }
    }
}
