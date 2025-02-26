﻿using CareTogether.Engines;
using CareTogether.Engines.PolicyEvaluation;
using CareTogether.Resources;
using CareTogether.Resources.Policies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;

namespace CareTogether.Core.Test.ReferralCalculationTests
{
    [TestClass]
    public class CalculateMissingMonitoringRequirements
    {
        [TestMethod]
        public void TestNotStarted()
        {
            var result = ReferralCalculations.CalculateMissingMonitoringRequirements(
                requiredMonitoringActionNames: ImmutableList<MonitoringRequirement>.Empty
                .Add(new MonitoringRequirement("A", new DurationStagesRecurrencePolicy(ImmutableList<RecurrencePolicyStage>.Empty
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(2), 1))
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(7), 4))
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(14), null)))))
                .Add(new MonitoringRequirement("B", new DurationStagesRecurrencePolicy(ImmutableList<RecurrencePolicyStage>.Empty
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(7), null))))),
                startedAtUtc: null, endedAtUtc: null,
                Helpers.Completed(),
                Helpers.Exempted(),
                Helpers.LocationHistoryEntries(),
                utcNow: new DateTime(2022, 1, 31));

            AssertEx.SequenceIs(result);
        }

        [TestMethod]
        public void TestStartedNoCompletions()
        {
            var result = ReferralCalculations.CalculateMissingMonitoringRequirements(
                requiredMonitoringActionNames: ImmutableList<MonitoringRequirement>.Empty
                .Add(new MonitoringRequirement("A", new DurationStagesRecurrencePolicy(ImmutableList<RecurrencePolicyStage>.Empty
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(2), 1))
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(7), 4))
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(14), null)))))
                .Add(new MonitoringRequirement("B", new DurationStagesRecurrencePolicy(ImmutableList<RecurrencePolicyStage>.Empty
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(7), null))))),
                startedAtUtc: new DateTime(2022, 1, 1), endedAtUtc: null,
                Helpers.Completed(),
                Helpers.Exempted(),
                Helpers.LocationHistoryEntries(),
                utcNow: new DateTime(2022, 1, 31));

            AssertEx.SequenceIs(result,
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 3)),
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 10)),
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 17)),
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 24)),
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 31)),
                new MissingArrangementRequirement("A", DueBy: new DateTime(2022, 2, 14), PastDueSince: null),
                new MissingArrangementRequirement("B", DueBy: null, PastDueSince: new DateTime(2022, 1, 8)),
                new MissingArrangementRequirement("B", DueBy: null, PastDueSince: new DateTime(2022, 1, 15)),
                new MissingArrangementRequirement("B", DueBy: null, PastDueSince: new DateTime(2022, 1, 22)),
                new MissingArrangementRequirement("B", DueBy: null, PastDueSince: new DateTime(2022, 1, 29)),
                new MissingArrangementRequirement("B", DueBy: new DateTime(2022, 2, 5), PastDueSince: null));
        }

        [TestMethod]
        public void TestStartedSomeCompletions()
        {
            var result = ReferralCalculations.CalculateMissingMonitoringRequirements(
                requiredMonitoringActionNames: ImmutableList<MonitoringRequirement>.Empty
                .Add(new MonitoringRequirement("A", new DurationStagesRecurrencePolicy(ImmutableList<RecurrencePolicyStage>.Empty
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(2), 1))
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(7), 4))
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(14), null)))))
                .Add(new MonitoringRequirement("B", new DurationStagesRecurrencePolicy(ImmutableList<RecurrencePolicyStage>.Empty
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(7), null))))),
                startedAtUtc: new DateTime(2022, 1, 1), endedAtUtc: null,
                Helpers.Completed(("A", 3), ("B", 7)),
                Helpers.Exempted(),
                Helpers.LocationHistoryEntries(),
                utcNow: new DateTime(2022, 1, 31));

            AssertEx.SequenceIs(result,
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 10)),
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 17)),
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 24)),
                new MissingArrangementRequirement("A", DueBy: null, PastDueSince: new DateTime(2022, 1, 31)),
                new MissingArrangementRequirement("A", DueBy: new DateTime(2022, 2, 14), PastDueSince: null),
                new MissingArrangementRequirement("B", DueBy: null, PastDueSince: new DateTime(2022, 1, 14)),
                new MissingArrangementRequirement("B", DueBy: null, PastDueSince: new DateTime(2022, 1, 21)),
                new MissingArrangementRequirement("B", DueBy: null, PastDueSince: new DateTime(2022, 1, 28)),
                new MissingArrangementRequirement("B", DueBy: new DateTime(2022, 2, 4), PastDueSince: null));
        }

        [TestMethod]
        public void TestStartedUpToDate()
        {
            var result = ReferralCalculations.CalculateMissingMonitoringRequirements(
                requiredMonitoringActionNames: ImmutableList<MonitoringRequirement>.Empty
                .Add(new MonitoringRequirement("A", new DurationStagesRecurrencePolicy(ImmutableList<RecurrencePolicyStage>.Empty
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(2), 1))
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(7), 4))
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(14), null)))))
                .Add(new MonitoringRequirement("B", new DurationStagesRecurrencePolicy(ImmutableList<RecurrencePolicyStage>.Empty
                    .Add(new RecurrencePolicyStage(TimeSpan.FromDays(7), null))))),
                startedAtUtc: new DateTime(2022, 1, 1), endedAtUtc: null,
                Helpers.Completed(("A", 3), ("B", 7)),
                Helpers.Exempted(),
                Helpers.LocationHistoryEntries(),
                utcNow: new DateTime(2022, 1, 8));

            AssertEx.SequenceIs(result,
                new MissingArrangementRequirement("A", DueBy: new DateTime(2022, 1, 10), PastDueSince: null),
                new MissingArrangementRequirement("B", DueBy: new DateTime(2022, 1, 14), PastDueSince: null));
        }
    }
}
