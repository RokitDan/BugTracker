using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugTracker.Extensions;

namespace BugTracker.Services
{
    public class BTInviteService : IBTInviteService
    {

        private readonly ApplicationDbContext _context;

        public BTInviteService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            try
            {
                Invite? invite = _context.Invites.FirstOrDefault(i => i.CompanyToken == token);

                if (invite == null)
                {
                    return false;
                }

                try
                {
                    invite.IsValid = false;
                    invite.InviteeId = userId;
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch
                {
                    throw;
                }

            }
            catch
            {
                throw;
            }
        }

        public async Task AddNewInviteAsync(Invite invite)
        {
            try
            {
                _context.AddAsync(invite);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool?> AnyInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                bool result = await _context.Invites.Where(i => i.CompanyId == companyId)
                                                    .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                Invite? invite = _context.Invites.Where(i => i.CompanyId == companyId)
                                                 .Include(i => i.Company)
                                                 .Include(i => i.Project)
                                                 .Include(i => i.Invitor)
                                                 .FirstOrDefault(i => i.Id == inviteId);
                return invite!;
            }
            catch
            {
                throw;
            }

        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                Invite? invite = _context.Invites.Where(i => i.CompanyToken == token)
                                                       .Where(i => i.CompanyId == companyId)
                                                       .Include(i => i.Project)
                                                       .Include(i => i.Invitor)
                                                       .Include(i => i.Company)
                                                       .FirstOrDefault(i => i.InviteeEmail == email && i.CompanyToken == token);

                return invite!;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            try
            {
                if (token == null)
                {
                    return false;
                }

                bool result = false;

                Invite? invite = _context.Invites.FirstOrDefault(i => i.CompanyToken == token);

                if (invite != null)
                {
                    DateTime inviteDate = invite.InviteDate;
                    bool validDate = (DateTime.Now - inviteDate).TotalDays <= 7;

                    if (validDate)
                    {
                        result = invite.IsValid;
                    }

                }
                return result;


            }
            catch
            {
                throw;
            }
        }
    }
}
