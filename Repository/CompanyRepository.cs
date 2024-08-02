using Contracts;
using Entities.Models;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext context) : base(context) { }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        {
            return FindAll(trackChanges).OrderBy(c => c.Name).ToList();
        }

        public Company GetCompany(Guid id, bool trackChanges)
        {
            return FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefault();
        }
    }
}
