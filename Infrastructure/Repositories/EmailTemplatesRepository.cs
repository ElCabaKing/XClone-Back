
using Domain.Interfaces;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories;

public class EmailTemplateRepository(XDbContext context) 
    : GenericRepository<Domain.Entities.EmailTemplate>(context), IEmailTemplateRepository
{
}