using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugTracker.Extensions;
using BugTracker.Data;

namespace BugTracker.Services
{
    public class BTCompanyService : IBTCompanyService
    {

        public readonly ApplicationDbContext _context;

        public BTCompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> GetCompanyInfoAsync(int? companyId)
        {
            try
            {
                Company? company = new();
                if (companyId != null)
                {
                    company = await _context.Companies
                                            .Include(c => c.Members)
                                            .Include(c => c.Projects)
                                            .Include(c => c.Invites)
                                            .FirstOrDefaultAsync(c => c.Id == companyId);
                }
                return company!;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetCompanyMembersAsync(int? companyId)
        {
            try
            {
                List<BTUser> companyMembers = _context.Users.Where(u => u.CompanyId == companyId).ToList();
                return companyMembers;
            }
            catch
            {
                throw;
            }
        }
    }
}
