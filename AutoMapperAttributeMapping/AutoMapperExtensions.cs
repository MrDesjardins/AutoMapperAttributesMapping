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
        public const char PROPERTY_DELIMITER = '.';
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

        /// <summary>
        /// Get the destination property name from the source. It uses automapper to discover the map between 
        /// the source and destination and get back the destination name. 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="sourcePropertyName"></param>
        /// <returns>Null if no mapping found</returns>
        public static string GetMappedPropertyName<TSource, TDestination>(this IConfigurationProvider mapper, string sourcePropertyName)
        {
            if (sourcePropertyName == null)
                throw new ArgumentNullException("sourcePropertyName");
            return GetMappedPropertyName(mapper, typeof(TSource), typeof(TDestination), new List<string> { sourcePropertyName });
        }
        private static string GetMappedPropertyName(IConfigurationProvider mapper, Type sourceType, Type destinationType, List<string> sourcePropertyNames)
        {
            //Analyse the first property name of the list. The list can have only one property if this one is at the end of a complex mapping or if it is a direct property
            var sourcePropertyName = sourcePropertyNames.First();

            //Get all mappings that has the source and destination of the desired type.
            var typeMap = mapper.GetAllTypeMaps().FirstOrDefault(tm => tm.SourceType == sourceType && tm.DestinationType == destinationType);
            if (typeMap == null)
                return null;

            //Loop through all mappings for the source-destination members to find the destination value.
            var propertyMaps = typeMap.GetPropertyMaps().Where(pm => !pm.IsIgnored() && pm.SourceMember != null);

            foreach (var propertyMap in propertyMaps)
            {
                var sourceValueResolverPath = string.Empty;

                if (propertyMap.CustomExpression != null)//If it is a custom expression, then get directly the property text.
                {
                    sourceValueResolverPath = LambdaHelper.GetPropertyText(propertyMap.CustomExpression);
                }
                else
                {
                    var valueResolvers = propertyMap.GetSourceValueResolvers();
                    sourceValueResolverPath = valueResolvers.OfType<IMemberGetter>()
                        .Aggregate(sourceValueResolverPath, (current, memberGetter) => current + (memberGetter.Name + PROPERTY_DELIMITER))
                        .RemoveLastChar();
                }

                //Check if we have a source that match the source property
                if (sourceValueResolverPath == sourcePropertyName)
                {
                    //If we have move than one property in the source, then we must loop and analyze sub property
                    if (sourcePropertyNames.Count > 1)
                    {
                        sourcePropertyNames.RemoveAt(0);
                        //Recursive call without the first element because we will concatenate the destination result of the first since we have a source match
                        return string.Concat(propertyMap.DestinationProperty.Name, PROPERTY_DELIMITER, GetMappedPropertyName(mapper,
                            ((PropertyInfo)(propertyMap.SourceMember)).PropertyType,
                            propertyMap.DestinationProperty.MemberType,
                            sourcePropertyNames));
                    }
                    //They are no properties and the source matched the resolving path. We return the destination
                    return propertyMap.DestinationProperty.Name;
                }
            }

            //If we are this far it is because we have not found the source property. But, we can split the name by dot and check for individual property mapping
            var splitProperties = sourcePropertyName.Split(PROPERTY_DELIMITER);
            if (splitProperties.Length > 1)
            {
                return GetMappedPropertyName(mapper, sourceType, destinationType, splitProperties.ToList());
            }
            return null;
        }
    }
}