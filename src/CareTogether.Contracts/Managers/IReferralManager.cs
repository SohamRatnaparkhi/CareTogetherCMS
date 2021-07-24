﻿using CareTogether.Resources;
using JsonPolymorph;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace CareTogether.Managers
{
    public record Referral(Guid Id, string PolicyVersion,
        DateTime CreatedUtc, ReferralCloseReason? CloseReason,
        Family PartneringFamily,
        ImmutableList<ContactInfo> Contacts,
        ImmutableList<FormUploadInfo> ReferralFormUploads,
        ImmutableList<ActivityInfo> ReferralActivitiesPerformed,
        ImmutableList<Arrangement> Arrangements);

    public enum ReferralCloseReason { NotAppropriate, Resourced, NoCapacity, NoLongerNeeded, NeedMet };

    public record Arrangement(Guid Id, string PolicyVersion, string ArrangementType,
        ArrangementState State,
        ImmutableList<FormUploadInfo> ArrangementFormUploads,
        ImmutableList<ActivityInfo> ArrangementActivitiesPerformed,
        ImmutableList<VolunteerAssignment> VolunteerAssignments,
        ImmutableList<PartneringFamilyChildAssignment> PartneringFamilyChildAssignments,
        ImmutableList<ChildrenLocationHistoryEntry> ChildrenLocationHistory,
        ImmutableList<Note> Notes);

    public enum ArrangementState { Setup, Open, Closed };

    public sealed record FormUploadInfo(Guid UserId, DateTime TimestampUtc,
        string FormName, string FormVersion, string UploadedFileName);
    public sealed record ActivityInfo(Guid UserId, DateTime TimestampUtc,
        string ActivityName);

    [JsonHierarchyBase]
    public abstract partial record VolunteerAssignment(string ArrangementFunction);
    public sealed record IndividualVolunteerAssignment(Guid PersonId, string ArrangementFunction)
        : VolunteerAssignment(ArrangementFunction);
    public sealed record FamilyVolunteerAssignment(Guid FamilyId, string ArrangementFunction)
        : VolunteerAssignment(ArrangementFunction);

    public sealed record PartneringFamilyChildAssignment(Guid PersonId);
    public sealed record ChildrenLocationHistoryEntry(Guid UserId, DateTime TimestampUtc,
        ImmutableList<Guid> ChildrenIds, Guid FamilyId, ChildrenLocationPlan Plan, string AdditionalExplanation);

    public enum ChildrenLocationPlan { OvernightHousing, DaytimeChildCare, ReturnToFamily }

    public record Note(Guid Id, Guid AuthorId, DateTime TimestampUtc,
        string Contents, NoteStatus Status);
    public enum NoteStatus { Draft, Approved };

    [JsonHierarchyBase]
    public abstract partial record ReferralCommand(Guid ReferralId, Guid UserId, DateTime TimestampUtc);
    public sealed record CreateReferral(Guid ReferralId, Guid UserId, DateTime TimestampUtc,
        Guid FamilyId, string PolicyVersion)
        : ReferralCommand(ReferralId, UserId, TimestampUtc);
    public sealed record PerformReferralActivity(Guid ReferralId, Guid UserId, DateTime TimestampUtc,
        string ActivityName)
        : ReferralCommand(ReferralId, UserId, TimestampUtc);
    public sealed record UploadReferralForm(Guid ReferralId, Guid UserId, DateTime TimestampUtc,
        string FormName, string FormVersion, string UploadedFileName)
        : ReferralCommand(ReferralId, UserId, TimestampUtc);
    public sealed record CloseReferral(Guid ReferralId, Guid UserId, DateTime TimestampUtc,
        ReferralCloseReason CloseReason)
        : ReferralCommand(ReferralId, UserId, TimestampUtc);

    [JsonHierarchyBase]
    public abstract partial record ArrangementCommand(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc);
    public sealed record CreateArrangement(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc,
        string PolicyVersion, string ArrangementType)
        : ArrangementCommand(ReferralId, ArrangementId, UserId, TimestampUtc);
    public sealed record AssignIndividualVolunteer(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc,
        Guid PersonId, string ArrangementFunction)
        : ArrangementCommand(ReferralId, ArrangementId, UserId, TimestampUtc);
    public sealed record AssignVolunteerFamily(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc,
        Guid FamilyId, string ArrangementFunction)
        : ArrangementCommand(ReferralId, ArrangementId, UserId, TimestampUtc);
    public sealed record AssignPartneringFamilyChildren(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc,
        ImmutableList<Guid> ChildrenIds)
        : ArrangementCommand(ReferralId, ArrangementId, UserId, TimestampUtc);
    public sealed record InitiateArrangement(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc)
        : ArrangementCommand(ReferralId, ArrangementId, UserId, TimestampUtc);
    public sealed record UploadArrangementForm(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc,
        string FormName, string FormVersion, string UploadedFileName)
        : ArrangementCommand(ReferralId, ArrangementId, UserId, TimestampUtc);
    public sealed record PerformArrangementActivity(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc,
        Guid CompletedByPersonId, string ActivityName)
        : ArrangementCommand(ReferralId, ArrangementId, UserId, TimestampUtc);
    public sealed record TrackChildrenLocationChange(Guid ReferralId, Guid ArrangementId, Guid UserId, DateTime TimestampUtc,
        ImmutableList<Guid> ChildrenIds, Guid FamilyId, ChildrenLocationPlan Plan, string AdditionalExplanation)
        : ArrangementCommand(ReferralId, ArrangementId, UserId, TimestampUtc);
    //TODO: EndArrangement?

    [JsonHierarchyBase]
    public abstract partial record ArrangementNoteCommand(Guid ReferralId, Guid ArrangementId, Guid NoteId,
        Guid UserId, DateTime TimestampUtc);
    public sealed record CreateDraftArrangementNote(Guid ReferralId, Guid ArrangementId, Guid NoteId,
        Guid UserId, DateTime TimestampUtc)
        : ArrangementNoteCommand(ReferralId, ArrangementId, NoteId, UserId, TimestampUtc);
    public sealed record EditDraftArrangementNote(Guid ReferralId, Guid ArrangementId, Guid NoteId,
        Guid UserId, DateTime TimestampUtc)
        : ArrangementNoteCommand(ReferralId, ArrangementId, NoteId, UserId, TimestampUtc);
    public sealed record DiscardDraftArrangementNote(Guid ReferralId, Guid ArrangementId, Guid NoteId,
        Guid UserId, DateTime TimestampUtc)
        : ArrangementNoteCommand(ReferralId, ArrangementId, NoteId, UserId, TimestampUtc);
    public sealed record ApproveArrangementNote(Guid ReferralId, Guid ArrangementId, Guid NoteId,
        Guid UserId, DateTime TimestampUtc, string FinalizedNoteContents)
        : ArrangementNoteCommand(ReferralId, ArrangementId, NoteId, UserId, TimestampUtc);

    /// <summary>
    /// The <see cref="IReferralManager"/> models the lifecycle of people's referrals to CareTogether organizations,
    /// including various forms, arrangements, and policy changes, as well as authorizing related queries.
    /// </summary>
    public interface IReferralManager
    {
        Task<IImmutableList<Referral>> ListReferralsAsync(Guid organizationId, Guid locationId);

        Task<ManagerResult<Referral>> ExecuteReferralCommandAsync(Guid organizationId, Guid locationId,
            AuthorizedUser user, ReferralCommand command);

        Task<ManagerResult<Referral>> ExecuteArrangementCommandAsync(Guid organizationId, Guid locationId,
            AuthorizedUser user, ArrangementCommand command);

        Task<ManagerResult<Referral>> ExecuteArrangementNoteCommandAsync(Guid organizationId, Guid locationId,
            AuthorizedUser user, ArrangementNoteCommand command);
    }
}
