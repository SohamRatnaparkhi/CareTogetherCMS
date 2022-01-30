using CareTogether.Managers;
using CareTogether.Resources;
using System;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CareTogether.Engines
{
    public sealed class AuthorizationEngine : IAuthorizationEngine
    {
        private readonly IPoliciesResource policiesResource;


        public AuthorizationEngine(IPoliciesResource policiesResource)
        {
            this.policiesResource = policiesResource;
        }


        public async Task<bool> AuthorizeFamilyCommandAsync(
            Guid organizationId, Guid locationId, ClaimsPrincipal user, FamilyCommand command)
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> AuthorizePersonCommandAsync(
            Guid organizationId, Guid locationId, ClaimsPrincipal user, PersonCommand command)
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> AuthorizeReferralCommandAsync(
            Guid organizationId, Guid locationId, ClaimsPrincipal user, ReferralCommand command)
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> AuthorizeArrangementCommandAsync(
            Guid organizationId, Guid locationId, ClaimsPrincipal user, ArrangementCommand command)
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> AuthorizeNoteCommandAsync(
            Guid organizationId, Guid locationId, ClaimsPrincipal user, NoteCommand command)
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> AuthorizeVolunteerFamilyCommandAsync(
            Guid organizationId, Guid locationId, ClaimsPrincipal user, VolunteerFamilyCommand command)
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> AuthorizeVolunteerCommandAsync(
            Guid organizationId, Guid locationId, ClaimsPrincipal user, VolunteerCommand command)
        {
            await Task.Yield();
            return true;
        }

        public async Task<Referral> DiscloseReferralAsync(ClaimsPrincipal user, Referral referral)
        {
            await Task.Yield();
            return referral;
        }

        public async Task<Arrangement> DiscloseArrangementAsync(ClaimsPrincipal user, Arrangement arrangement)
        {
            await Task.Yield();
            return arrangement;
        }

        public async Task<VolunteerFamilyInfo> DiscloseVolunteerFamilyInfoAsync(ClaimsPrincipal user, VolunteerFamilyInfo volunteerFamilyInfo)
        {
            await Task.Yield();
            return volunteerFamilyInfo with
            {
                FamilyRoleApprovals = user.HasPermission(Permission.ViewApprovalStatus)
                ? volunteerFamilyInfo.FamilyRoleApprovals
                : ImmutableDictionary<string, ImmutableList<RoleVersionApproval>>.Empty,
                RemovedRoles = user.HasPermission(Permission.ViewApprovalStatus)
                ? volunteerFamilyInfo.RemovedRoles
                : ImmutableList<RemovedRole>.Empty,
                IndividualVolunteers = user.HasPermission(Permission.ViewApprovalStatus)
                ? volunteerFamilyInfo.IndividualVolunteers
                : volunteerFamilyInfo.IndividualVolunteers.ToImmutableDictionary(
                    keySelector: kvp => kvp.Key,
                    elementSelector: kvp => kvp.Value with
                    {
                        RemovedRoles = ImmutableList<RemovedRole>.Empty,
                        IndividualRoleApprovals = ImmutableDictionary<string, ImmutableList<RoleVersionApproval>>.Empty
                    })
            };
        }

        public async Task<Family> DiscloseFamilyAsync(ClaimsPrincipal user, Family family)
        {
            await Task.Yield();
            return family;
        }

        public async Task<Person> DisclosePersonAsync(ClaimsPrincipal user, Person person)
        {
            await Task.Yield();
            return person;
        }

        public async Task<bool> DiscloseNoteAsync(ClaimsPrincipal user, Guid familyId, Note note)
        {
            await Task.Yield();
            return true;
        }
    }
}
