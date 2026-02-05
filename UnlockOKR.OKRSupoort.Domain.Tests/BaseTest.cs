using System;
namespace UnlockOKR.OKRSupoort.Domain.Tests
{
    public class BaseTest
    {
        public T GetModelTestData<T>(T newModel)
        {
            var type = newModel.GetType();
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var propTypeInfo = type.GetProperty(prop.Name.Trim());
                if (propTypeInfo is { CanRead: true })
                    prop.GetValue(newModel);
            }
            return newModel;
        }

        public T SetModelTestData<T>(T newModel)
        {
            var type = newModel.GetType();
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var propTypeInfo = type.GetProperty(prop.Name.Trim());
                var propType = prop.GetType();

                if (!(propTypeInfo is { CanWrite: true })) continue;
                if (prop.PropertyType.Name == "String")
                {
                    prop.SetValue(newModel, string.Empty);
                }
                else if (propType.IsValueType)
                {
                    prop.SetValue(newModel, Activator.CreateInstance(propType));
                }
                else
                {
                    prop.SetValue(newModel, null);
                }
            }
            return newModel;
        }
    }
}