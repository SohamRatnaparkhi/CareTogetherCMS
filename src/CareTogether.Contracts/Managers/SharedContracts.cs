﻿using CareTogether.Engines;
using CareTogether.Resources;
using System;
using System.Collections.Immutable;

namespace CareTogether.Managers
{
    public sealed record CombinedFamilyInfo(Family Family,
        PartneringFamilyInfo? PartneringFamilyInfo, VolunteerFamilyInfo? VolunteerFamilyInfo,
        ImmutableList<Note> Notes);

    public sealed record PartneringFamilyInfo(
        Referral? OpenReferral,
        ImmutableList<Referral> ClosedReferrals);

    public sealed record Referral(Guid Id,
        DateTime OpenedAtUtc, DateTime? ClosedAtUtc, ReferralCloseReason? CloseReason,
        ImmutableList<CompletedRequirementInfo> CompletedRequirements,
        ImmutableList<UploadedDocumentInfo> UploadedDocuments,
        ImmutableList<string> MissingIntakeRequirements,
        ImmutableList<Arrangement> Arrangements);

    public sealed record Arrangement(Guid Id, string ArrangementType,
        ArrangementPhase Phase, DateTime RequestedAtUtc, DateTime? StartedAtUtc, DateTime? EndedAtUtc,
        ImmutableList<CompletedRequirementInfo> CompletedRequirements,
        ImmutableList<UploadedDocumentInfo> UploadedDocuments,
        ImmutableList<string> MissingSetupRequirements,
        ImmutableList<string> MissingMonitoringRequirements, //TODO: Better define this, especially when recurrence is involved
        ImmutableList<string> MissingCloseoutRequirements,
        ImmutableList<IndividualVolunteerAssignment> IndividualVolunteerAssignments,
        ImmutableList<FamilyVolunteerAssignment> FamilyVolunteerAssignments,
        ImmutableList<PartneringFamilyChildAssignment> PartneringFamilyChildAssignments,
        ImmutableList<ChildLocationHistoryEntry> ChildrenLocationHistory);

    public sealed record Note(Guid Id, Guid AuthorId, DateTime TimestampUtc,
        string? Contents, NoteStatus Status);

    public sealed record VolunteerFamilyInfo(
        ImmutableList<CompletedRequirementInfo> CompletedRequirements,
        ImmutableList<UploadedDocumentInfo> UploadedDocuments,
        ImmutableList<RemovedRole> RemovedRoles,
        ImmutableList<string> MissingRequirements,
        ImmutableList<string> AvailableApplications,
        ImmutableDictionary<string, ImmutableList<RoleVersionApproval>> FamilyRoleApprovals,
        ImmutableDictionary<Guid, VolunteerInfo> IndividualVolunteers);

    public sealed record VolunteerInfo(
        ImmutableList<CompletedRequirementInfo> CompletedRequirements,
        ImmutableList<RemovedRole> RemovedRoles,
        ImmutableList<string> MissingRequirements,
        ImmutableList<string> AvailableApplications,
        ImmutableDictionary<string, ImmutableList<RoleVersionApproval>> IndividualRoleApprovals);
}
