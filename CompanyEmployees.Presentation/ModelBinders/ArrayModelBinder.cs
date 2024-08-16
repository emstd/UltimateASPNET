using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace CompanyEmployees.Presentation.ModelBinders
{
    public class ArrayModelBinder : IModelBinder //Ћогика прив€зки модели, когда в строке запроса передаетс€ строка из нескольких значений IEnumerable (Guid например) мы создаем массив из этих значений
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!bindingContext.ModelMetadata.IsEnumerableType) //ѕровер€ем, что тип IEnumerable
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var providedValue = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName)
                .ToString();    //ѕолучаем строку значений (напр. 12,13,14)
            if (string.IsNullOrEmpty(providedValue)) //≈сли нет значений - прив€зывать нечего
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];   //ѕолучаем тип T (задумываетс€ Guid) из IEnumerable<T>
            var converter = TypeDescriptor.GetConverter(genericType);   //ѕолучаем конвертер типов в наш тип T (задумываетс€ Guid)

            var objectArray = providedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray(); // онвертируем каждое значение строки в тип T (задумываетс€ Guid) и создаем массив Object этих значений

            var guidArray = Array.CreateInstance(genericType, objectArray.Length); //создаем массив типа T (задумываетс€ Guid) с нужной длинной
            objectArray.CopyTo(guidArray, 0);   //копируем значени€ из Object массива (гуиды) в наш массив
            bindingContext.Model = guidArray; //говорим, что значчение модели теперь этот массив

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);   //√оворим, что операци€ прив€зки модели выполнена

            return Task.CompletedTask;
        }
    }
}
