namespace Entities.Exceptions
{
    public class CompanyCollecionBadRequest : BadRequestException
    {
        public CompanyCollecionBadRequest() : base("Company collection sent from a client is null.")
        {

        }
    }
}
