namespace Shared.DataTransferObjects
{
    public record CompanyDto //Сделали явные свойства ради xml форматтера
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? FullAddress { get; init; }
    }
}