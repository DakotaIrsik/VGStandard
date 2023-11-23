using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using VGStandard.Core.Extensions;

namespace VGStandard.Core.Utility;

public static class Reflection
{
    public static IEnumerable<PropertyInfo> GetNonInheritedProperties(this object obj)
    {
        if (obj == null)
            return new List<PropertyInfo>();

        var allProperties = obj.GetType().GetProperties();
        var implementedProps = obj.GetType().GetInterfaces().SelectMany(i => i.GetProperties());
        var classProperties = allProperties.Select(prop => prop.Name).Except(implementedProps.Select(prop => prop.Name)).ToList();
        return obj.GetType().GetProperties().Where(p => classProperties.Contains(p.Name));
    }

    public static bool IsAnonymousType(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        // HACK: The only way to detect anonymous types right now.
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
            && type.IsGenericType && type.Name.Contains("AnonymousType")
            && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
            && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
    }

    public static string ElasticIndexResolver(this Type type)
    {
        return type.Name.Replace("Elastic", "").ToLower();
    }

    public static T GetModelFromDTO<T>(this object dto) where T : class
    {
        var result = Activator.CreateInstance(typeof(T));

        var properties = dto.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.GetValue(dto) == null)
            {
                //do nothing, it's already set to null
            }
            else if (property.PropertyType == typeof(IEnumerable))
            {
                var theNewValue = ((IEnumerable<object>)property.GetValue(dto)).ToCsvFromList();
                property.SetValue(result, theNewValue);
            }
            else if (property.PropertyType.Assembly == Assembly.GetExecutingAssembly())
            {
                var theNewValue = JsonConvert.SerializeObject(property.GetValue(dto));
                property.SetValue(result, theNewValue);
            }
            else if (property.PropertyType.IsSealed)
            {
                try
                {
                    var value = property.GetValue(dto);

                    property.SetValue(property, value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception($"{property.Name} could not be mapped in method {new StackTrace().GetFrame(1).GetMethod().Name}");
            }
        }
        return (T)result;
    }
}