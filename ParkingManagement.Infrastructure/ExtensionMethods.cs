using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace ParkingManagement.Infrastructure
{
    public static class ExtensionMethods
    {
        public static T Clone<T>(this T source)
        {
            if (source == null)
            {
                return default;
            }

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var serializeSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
        }

        public static T DeepCopyTo<T>(this object source) where T : new()
        {
            object target = new T();
            CopyObjectData(source, target, String.Empty, BindingFlags.Public | BindingFlags.Instance);
            return (T)target;
        }


        static void CopyObjectData(object source, object target, string excludedProperties, BindingFlags memberAccess)
        {
            string[] excluded = null;
            if (!string.IsNullOrEmpty(excludedProperties))
            {
                excluded = excludedProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            MemberInfo[] miT = target.GetType().GetMembers(memberAccess);
            foreach (MemberInfo Field in miT)
            {
                string name = Field.Name;

                if (string.IsNullOrEmpty(excludedProperties) == false
                    && excluded.Contains(name))
                {
                    continue;
                }

                if (Field.MemberType == MemberTypes.Field)
                {
                    FieldInfo sourcefield = source.GetType().GetField(name);
                    if (sourcefield == null) { continue; }

                    object SourceValue = sourcefield.GetValue(source);
                    ((FieldInfo)Field).SetValue(target, SourceValue);
                }
                else if (Field.MemberType == MemberTypes.Property)
                {
                    PropertyInfo piTarget = Field as PropertyInfo;
                    PropertyInfo sourceField = source.GetType().GetProperty(name, memberAccess);
                    if (sourceField == null) { continue; }

                    if (piTarget.CanWrite && sourceField.CanRead)
                    {
                        object targetValue = piTarget.GetValue(target, null);
                        object sourceValue = sourceField.GetValue(source, null);

                        if (sourceValue == null) { continue; }

                        if (sourceField.PropertyType.IsArray
                            && piTarget.PropertyType.IsArray
                            && sourceValue != null)
                        {
                            CopyArray(source, target, memberAccess, piTarget, sourceField, sourceValue);
                        }
                        else
                        {
                            CopySingleData(source, target, memberAccess, piTarget, sourceField, targetValue, sourceValue);
                        }
                    }
                }
            }
        }

        private static void CopySingleData(object source, object target, BindingFlags memberAccess, PropertyInfo piTarget, PropertyInfo sourceField, object targetValue, object sourceValue)
        {
            if (targetValue == null
                && piTarget.PropertyType.IsValueType == false
                && piTarget.PropertyType != typeof(string))
            {
                if (piTarget.PropertyType.IsArray)
                {
                    targetValue = Activator.CreateInstance(piTarget.PropertyType.GetElementType());
                }
                else
                {
                    targetValue = Activator.CreateInstance(piTarget.PropertyType);
                }
            }

            if (piTarget.PropertyType.IsValueType == false
                && piTarget.PropertyType != typeof(string))
            {
                CopyObjectData(sourceValue, targetValue, "", memberAccess);
                piTarget.SetValue(target, targetValue, null);
            }
            else
            {
                if (piTarget.PropertyType.FullName == sourceField.PropertyType.FullName)
                {
                    object tempSourceValue = sourceField.GetValue(source, null);
                    piTarget.SetValue(target, tempSourceValue, null);
                }
                else
                {
                    CopyObjectData(piTarget, target, "", memberAccess);
                }
            }
        }

        private static void CopyArray(object source, object target, BindingFlags memberAccess, PropertyInfo piTarget, PropertyInfo sourceField, object sourceValue)
        {
            int sourceLength = (int)sourceValue.GetType().InvokeMember("Length", BindingFlags.GetProperty, null, sourceValue, null);
            Array targetArray = Array.CreateInstance(piTarget.PropertyType.GetElementType(), sourceLength);
            Array array = (Array)sourceField.GetValue(source, null);

            for (int i = 0; i < array.Length; i++)
            {
                object o = array.GetValue(i);
                object tempTarget = Activator.CreateInstance(piTarget.PropertyType.GetElementType());
                CopyObjectData(o, tempTarget, "", memberAccess);
                targetArray.SetValue(tempTarget, i);
            }
            piTarget.SetValue(target, targetArray, null);
        }

    }
}
