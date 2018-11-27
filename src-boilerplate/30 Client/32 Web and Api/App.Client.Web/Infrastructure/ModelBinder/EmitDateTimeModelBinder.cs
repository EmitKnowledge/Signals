using System;
using System.Web.Mvc;
using App.Common.Base.Dates;

namespace App.Client.Web.Infrastructure.ModelBinder
{
    public class EmitDateTimeModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// Binds the model by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound object.
        /// </returns>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param><param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param><exception cref="T:System.ArgumentNullException">The <paramref name="bindingContext "/>parameter is null.</exception>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (result == null ||
                result.RawValue == null) return null;

            var dateTimeValue = (string)result.ConvertTo(typeof(string));
            DateTime date;
            var isParsed = DateTime.TryParse(dateTimeValue, out date);
            if (!isParsed) return null;
            return new EmitDateTime(date, false);
        }
    }
}