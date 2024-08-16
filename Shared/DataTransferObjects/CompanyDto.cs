namespace Shared.DataTransferObjects
{
    public record CompanyDto //������� ����� �������� ���� xml ����������
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? FullAddress { get; init; }
    }
}