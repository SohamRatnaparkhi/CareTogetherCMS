﻿using CareTogether.Resources.Policies;
using JsonPolymorph;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace CareTogether.Resources.Referrals
{
    public record ReferralEntry(Guid Id, Guid FamilyId,
        DateTime OpenedAtUtc, DateTime? ClosedAtUtc, ReferralCloseReason? CloseReason,
        ImmutableList<CompletedRequirementInfo> CompletedRequirements,
        ImmutableList<ExemptedRequirementInfo> ExemptedRequirements,
        ImmutableDictionary<string, CompletedCustomFieldInfo> CompletedCustomFields,
        ImmutableDictionary<Guid, ArrangementEntry> Arrangements);

    public record ArrangementEntry(Guid Id, string ArrangementType,
        DateTime RequestedAtUtc, DateTime? StartedAtUtc, DateTime? EndedAtUtc,
        Guid PartneringFamilyPersonId,
        ImmutableList<CompletedRequirementInfo> CompletedRequirements,
        ImmutableList<ExemptedRequirementInfo> ExemptedRequirements,
        ImmutableList<IndividualVolunteerAssignment> IndividualVolunteerAssignments,
        ImmutableList<FamilyVolunteerAssignment> FamilyVolunteerAssignments,
        ImmutableSortedSet<ChildLocationHistoryEntry> ChildrenLocationHistory);

    public enum ReferralCloseReason { NotAppropriate, NoCapacity, NoLongerNeeded, Resourced, NeedMet };

    public sealed record IndividualVolunteerAssignment(Guid FamilyId, Guid PersonId, string ArrangementFunction);
    public sealed record FamilyVolunteerAssignment(Guid FamilyId, string ArrangementFunction);
    public sealed record ChildLocationHistoryEntry(Guid UserId, DateTime TimestampUtc,
        Guid ChildLocationFamilyId, ChildLocationPlan Plan) : IComparable<ChildLocationHistoryEntry>
    {
        public int CompareTo(ChildLocationHistoryEntry? other)
        {
            return other == null
                ? 1
                : DateTime.Compare(TimestampUtc, other.TimestampUtc);
        }
    }
    public enum ChildLocationPlan { OvernightHousing, DaytimeChildCare, WithParent }

    [JsonHierarchyBase]
    public abstract partial record ReferralCommand(Guid FamilyId, Guid ReferralId);
    public sealed record CreateReferral(Guid FamilyId, Guid ReferralId,
        DateTime OpenedAtUtc)
        : ReferralCommand(FamilyId, ReferralId);
    public sealed record CompleteReferralRequirement(Guid FamilyId, Guid ReferralId,
        Guid CompletedRequirementId, string RequirementName, DateTime CompletedAtUtc, Guid? UploadedDocumentId)
        : ReferralCommand(FamilyId, ReferralId);
    public sealed record MarkReferralRequirementIncomplete(Guid FamilyId, Guid ReferralId,
        Guid CompletedRequirementId, string RequirementName)
        : ReferralCommand(FamilyId, ReferralId);
    public sealed record ExemptReferralRequirement(Guid FamilyId, Guid ReferralId,
        string RequirementName, string AdditionalComments, DateTime? ExemptionExpiresAtUtc)
        : ReferralCommand(FamilyId, ReferralId);
    public sealed record UnexemptReferralRequirement(Guid FamilyId, Guid ReferralId,
        string RequirementName)
        : ReferralCommand(FamilyId, ReferralId);
    public sealed record UpdateCustomReferralField(Guid FamilyId, Guid ReferralId,
        Guid CompletedCustomFieldId, string CustomFieldName, CustomFieldType CustomFieldType, object? Value)
        : ReferralCommand(FamilyId, ReferralId);
    public sealed record CloseReferral(Guid FamilyId, Guid ReferralId,
        ReferralCloseReason CloseReason, DateTime ClosedAtUtc)
        : ReferralCommand(FamilyId, ReferralId);

    [JsonHierarchyBase]
    public abstract partial record ArrangementCommand(Guid FamilyId, Guid ReferralId, Guid ArrangementId);
    public sealed record CreateArrangement(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        string ArrangementType, DateTime RequestedAtUtc, Guid PartneringFamilyPersonId)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record AssignIndividualVolunteer(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        Guid VolunteerFamilyId, Guid PersonId, string ArrangementFunction)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record AssignVolunteerFamily(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        Guid VolunteerFamilyId, string ArrangementFunction)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record StartArrangement(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        DateTime StartedAtUtc)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record CompleteArrangementRequirement(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        Guid CompletedRequirementId, string RequirementName, DateTime CompletedAtUtc, Guid? UploadedDocumentId)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record MarkArrangementRequirementIncomplete(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        Guid CompletedRequirementId, string RequirementName)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record ExemptArrangementRequirement(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        string RequirementName, DateTime? DueDate, string AdditionalComments, DateTime? ExemptionExpiresAtUtc)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record UnexemptArrangementRequirement(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        string RequirementName, DateTime? DueDate)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record TrackChildLocationChange(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        DateTime ChangedAtUtc, Guid ChildLocationFamilyId, ChildLocationPlan Plan)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);
    public sealed record EndArrangement(Guid FamilyId, Guid ReferralId, Guid ArrangementId,
        DateTime EndedAtUtc)
        : ArrangementCommand(FamilyId, ReferralId, ArrangementId);

    /// <summary>
    /// The <see cref="IReferralsResource"/> models the lifecycle of people's referrals to CareTogether organizations,
    /// including various forms, arrangements, and policy changes, as well as authorizing related queries.
    /// </summary>
    public interface IReferralsResource
    {
        Task<ImmutableList<ReferralEntry>> ListReferralsAsync(Guid organizationId, Guid locationId);

        Task<ReferralEntry> GetReferralAsync(Guid organizationId, Guid locationId, Guid referralId);

        Task<ReferralEntry> ExecuteReferralCommandAsync(Guid organizationId, Guid locationId,
            ReferralCommand command, Guid userId);

        Task<ReferralEntry> ExecuteArrangementCommandAsync(Guid organizationId, Guid locationId,
            ArrangementCommand command, Guid userId);
    }
}