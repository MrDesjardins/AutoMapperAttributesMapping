using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;

namespace AutoMapperAttributeMapping
{
    internal class MappedDataAnnotationsModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly IConfigurationProvider mapper;

        public MappedDataAnnotationsModelMetadataProvider(IConfigurationProvider mapper)
        {
            this.mapper = mapper;
        }

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {

            //Copy attributes from the model and to the view model
            var mappedAttributes = containerType == null ? attributes : mapper.GetMappedAttributes(containerType, propertyName, attributes).ToArray();
            var modelMetadata = base.CreateMetadata(mappedAttributes, containerType, modelAccessor, modelType, propertyName);

            return modelMetadata;
        }
    }
}