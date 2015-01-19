using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace AutoMapperAttributeMapping
{
    internal static class AutoMapperExtensions
    {

        /// <summary>
        /// Get Data Annotation from Model To set on ViewModel later.
        /// Do not get if the property is marked as ignored by AutoMapper.
        /// Transfert ValitationAttribute and DisplayAttribute
        /// </summary>
        /// <param name="mapper">AutoMapper configuration</param>
        /// <param name="viewModelType"></param>
        /// <param name="viewModelPropertyName"></param>
        /// <param name="viewModelPropertyAttributes"></param>
        /// <returns></returns>
        public static IEnumerable<Attribute> GetMappedAttributes(this IConfigurationProvider mapper,
            Type viewModelType,
            string viewModelPropertyName,
            IEnumerable<Attribute> viewModelPropertyAttributes)
        {
            if (viewModelType == null)
                throw new ArgumentNullException("viewModelType");

            //For all automapper configurations about the view model we are working with
            foreach (var typeMap in mapper.GetAllTypeMaps()
                .Where(i => i.DestinationType == viewModelType))
            {
                //Get the properties from the model we found from automapper
                var propertyMaps = typeMap.GetPropertyMaps()
                    .Where(propertyMap => !propertyMap.IsIgnored() && propertyMap.SourceMember != null)
                    .Where(propertyMap => propertyMap.DestinationProperty.Name == viewModelPropertyName);

                foreach (var propertyMap in propertyMaps)
                {
                    //Only get the attribute from the model if the view model does not define it
                    foreach (Attribute attribute in propertyMap.SourceMember.GetCustomAttributes(typeof(ValidationAttribute), true))
                    {
                        if (!viewModelPropertyAttributes.Any(i => i.GetType().IsInstanceOfType(attribute)
                                                                  || attribute.GetType().IsInstanceOfType(i)))
                            yield return attribute;
                    }
                    //Only get the attribute from the model if the view model does not define it
                    foreach (Attribute attribute in propertyMap.SourceMember.GetCustomAttributes(typeof(DisplayAttribute), true))
                    {
                        if (!viewModelPropertyAttributes.Any(i => i.GetType() == attribute.GetType()))
                            yield return attribute;
                    }

                }
            }

            //Add all view model attribute
            if (viewModelPropertyAttributes != null)
            {
                foreach (var attribute in viewModelPropertyAttributes)
                {
                    yield return attribute;
                }
            }
        }

    }
}