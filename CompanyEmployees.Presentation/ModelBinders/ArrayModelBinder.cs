using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace CompanyEmployees.Presentation.ModelBinders
{
    public class ArrayModelBinder : IModelBinder //������ �������� ������, ����� � ������ ������� ���������� ������ �� ���������� �������� IEnumerable (Guid ��������) �� ������� ������ �� ���� ��������
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!bindingContext.ModelMetadata.IsEnumerableType) //���������, ��� ��� IEnumerable
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var providedValue = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName)
                .ToString();    //�������� ������ �������� (����. 12,13,14)
            if (string.IsNullOrEmpty(providedValue)) //���� ��� �������� - ����������� ������
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];   //�������� ��� T (������������ Guid) �� IEnumerable<T>
            var converter = TypeDescriptor.GetConverter(genericType);   //�������� ��������� ����� � ��� ��� T (������������ Guid)

            var objectArray = providedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray(); //������������ ������ �������� ������ � ��� T (������������ Guid) � ������� ������ Object ���� ��������

            var guidArray = Array.CreateInstance(genericType, objectArray.Length); //������� ������ ���� T (������������ Guid) � ������ �������
            objectArray.CopyTo(guidArray, 0);   //�������� �������� �� Object ������� (�����) � ��� ������
            bindingContext.Model = guidArray; //�������, ��� ��������� ������ ������ ���� ������

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);   //�������, ��� �������� �������� ������ ���������

            return Task.CompletedTask;
        }
    }
}
