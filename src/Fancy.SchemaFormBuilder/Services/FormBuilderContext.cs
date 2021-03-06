﻿using System;
using System.Globalization;
using System.Reflection;

using Newtonsoft.Json.Linq;
using Fancy.SchemaFormBuilder.Providers;

namespace Fancy.SchemaFormBuilder.Services
{
    /// <summary>
    /// Context with all information required in the form builder pipeline.
    /// </summary>
    public class FormBuilderContext
    {
        /// <summary>
        /// Gets the form builder.
        /// </summary>
        /// <value>
        /// The form builder.
        /// </value>
        public IFormBuilder FormBuilder { get; internal set; }

        /// <summary>
        /// Gets or sets the type of the dto beeing process in this context.
        /// </summary>
        /// <value>
        /// The type of the dto.
        /// </value>
        public Type DtoType { get; internal set; }

        /// <summary>
        /// Gets or sets the type of the origin dto which finally led to the processing of the <see cref="DtoType"/>.
        /// </summary>
        /// <value>
        /// The type of the origin dto.
        /// </value>
        public Type OriginDtoType { get; set; }

        /// <summary>
        /// Gets the property which is currently processed by the pipeline.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public PropertyInfo Property { get; internal set; }

        /// <summary>
        /// Gets the full property path.
        /// </summary>
        /// <value>
        /// The full property path.
        /// </value>
        public string FullPropertyPath { get; internal set; }

        /// <summary>
        /// Gets or sets the form element which will be written to the JSON form description.
        /// </summary>
        /// <remarks>
        /// This property contains the result of processing a property by the pipeline. All modules can write into this property
        /// to extends the schema information of the property being processed.
        /// </remarks>
        /// <value>
        /// The element.
        /// </value>
        public JObject CurrentFormElement { get; set; }

        /// <summary>
        /// Gets or sets the current parent of the form element.
        /// </summary>
        /// <value>
        /// The current form parent.
        /// </value>
        public JArray CurrentFormElementParent { get; set; }

        /// <summary>
        /// Gets or sets the complete form in the current processing stage.
        /// </summary>
        /// <value>
        /// The complete form.
        /// </value>
        public JArray CompleteForm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pipeline shall finish processing after the module has benn run or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the shall finish the processing; otherwise, <c>false</c>.
        /// </value>
        public bool FinishProcessing { get; set; }

        /// <summary>
        /// Gets or sets the target culture.
        /// </summary>
        /// <value>
        /// The target culture.
        /// </value>
        public CultureInfo TargetCulture { get; set; }

        /// <summary>
        /// Gets the or the current form element. If it does not exist yet it will be created.
        /// </summary>
        /// <returns>The form element JSON object.</returns>
        public JObject GetOrCreateCurrentFormElement()
        {
            if (CurrentFormElement == null)
            {
                CurrentFormElement = new JObject();

                // Add the form element to its current parent
                CurrentFormElementParent.Add(this.CurrentFormElement);
            }

            return CurrentFormElement;
        }

        /// <summary>
        /// Gets the language context.
        /// </summary>
        /// <returns></returns>
        public LanguageContext GetLanguageContext()
        {
            return new LanguageContext { Culture = TargetCulture, DtoType = DtoType, OriginDtoType = OriginDtoType };
        }
    }
}
