using CareTogether.Resources;
using CareTogether.Resources.Approvals;
using CareTogether.Resources.Directory;
using CareTogether.Resources.Policies;
using CareTogether.Resources.Referrals;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace CareTogether.Engines.PolicyEvaluation
{
    public sealed class PolicyEvaluationEngine : IPolicyEvaluationEngine
    {
        private readonly IPoliciesResource policiesResource;


        public PolicyEvaluationEngine(IPoliciesResource policiesResource)
        {
            this.policiesResource = policiesResource;
        }


        public async Task<VolunteerFamilyApprovalStatus> CalculateVolunteerFamilyApprovalStatusAsync(Guid organizationId, Guid locationId,
            Family family, ImmutableList<CompletedRequirementInfo> completedFamilyRequirements,
            ImmutableList<ExemptedRequirementInfo> exemptedFamilyRequirements,
            ImmutableList<RemovedRole> removedFamilyRoles,
            ImmutableDictionary<Guid, ImmutableList<CompletedRequirementInfo>> completedIndividualRequirements,
            ImmutableDictionary<Guid, ImmutableList<ExemptedRequirementInfo>> exemptedIndividualRequirements,
            ImmutableDictionary<Guid, ImmutableList<RemovedRole>> removedIndividualRoles)
        {
            var policy = await policiesResource.GetCurrentPolicy(organizationId, locationId);

            return ApprovalCalculations.CalculateVolunteerFamilyApprovalStatus(
                policy.VolunteerPolicy, family, DateTime.UtcNow,
                completedFamilyRequirements, exemptedFamilyRequirements, removedFamilyRoles,
                completedIndividualRequirements, exemptedIndividualRequirements, removedIndividualRoles);
        }


        public async Task<ReferralStatus> CalculateReferralStatusAsync(
            Guid organizationId, Guid locationId, Family family,
            ReferralEntry referralEntry)
        {
            var policy = await policiesResource.GetCurrentPolicy(organizationId, locationId);

            return ReferralCalculations.CalculateReferralStatus(
                policy.ReferralPolicy, referralEntry, DateTime.UtcNow);
        }
    }
}
