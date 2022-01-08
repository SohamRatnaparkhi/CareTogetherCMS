﻿using CareTogether.Engines;
using CareTogether.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CareTogether.Core.Test.ApprovalCalculationTests
{
    [TestClass]
    public class CalculateMissingIndividualRequirementsFromRequirementCompletion
    {
        [TestMethod]
        public void TestNoStatusNoneMissing()
        {
            var result = ApprovalCalculations.CalculateMissingIndividualRequirementsFromRequirementCompletion(
                status: null,
                Helpers.IndividualRequirementsMet(
                    ("A", RequirementStage.Application, true),
                    ("B", RequirementStage.Approval, true),
                    ("C", RequirementStage.Approval, true),
                    ("D", RequirementStage.Approval, true),
                    ("E", RequirementStage.Onboarding, true),
                    ("F", RequirementStage.Onboarding, true)));

            AssertEx.SequenceIs(result);
        }

        [TestMethod]
        public void TestNoStatusAllMissing()
        {
            var result = ApprovalCalculations.CalculateMissingIndividualRequirementsFromRequirementCompletion(
                status: null,
                Helpers.IndividualRequirementsMet(
                    ("A", RequirementStage.Application, false),
                    ("B", RequirementStage.Approval, false),
                    ("C", RequirementStage.Approval, false),
                    ("D", RequirementStage.Approval, false),
                    ("E", RequirementStage.Onboarding, false),
                    ("F", RequirementStage.Onboarding, false)));

            AssertEx.SequenceIs(result);
        }

        [TestMethod]
        public void TestProspectiveNoneMissing()
        {
            var result = ApprovalCalculations.CalculateMissingIndividualRequirementsFromRequirementCompletion(
                status: RoleApprovalStatus.Prospective,
                Helpers.IndividualRequirementsMet(
                    ("A", RequirementStage.Application, true),
                    ("B", RequirementStage.Approval, true),
                    ("C", RequirementStage.Approval, true),
                    ("D", RequirementStage.Approval, true),
                    ("E", RequirementStage.Onboarding, true),
                    ("F", RequirementStage.Onboarding, true)));

            AssertEx.SequenceIs(result);
        }

        [TestMethod]
        public void TestProspectiveAllMissing()
        {
            var result = ApprovalCalculations.CalculateMissingIndividualRequirementsFromRequirementCompletion(
                status: RoleApprovalStatus.Prospective,
                Helpers.IndividualRequirementsMet(
                    ("A", RequirementStage.Application, false),
                    ("B", RequirementStage.Approval, false),
                    ("C", RequirementStage.Approval, false),
                    ("D", RequirementStage.Approval, false),
                    ("E", RequirementStage.Onboarding, false),
                    ("F", RequirementStage.Onboarding, false)));

            AssertEx.SequenceIs(result, "B", "C", "D");
        }

        [TestMethod]
        public void TestApprovedNoneMissing()
        {
            var result = ApprovalCalculations.CalculateMissingIndividualRequirementsFromRequirementCompletion(
                status: RoleApprovalStatus.Approved,
                Helpers.IndividualRequirementsMet(
                    ("A", RequirementStage.Application, true),
                    ("B", RequirementStage.Approval, true),
                    ("C", RequirementStage.Approval, true),
                    ("D", RequirementStage.Approval, true),
                    ("E", RequirementStage.Onboarding, true),
                    ("F", RequirementStage.Onboarding, true)));

            AssertEx.SequenceIs(result);
        }

        [TestMethod]
        public void TestApprovedAllMissing()
        {
            var result = ApprovalCalculations.CalculateMissingIndividualRequirementsFromRequirementCompletion(
                status: RoleApprovalStatus.Approved,
                Helpers.IndividualRequirementsMet(
                    ("A", RequirementStage.Application, false),
                    ("B", RequirementStage.Approval, false),
                    ("C", RequirementStage.Approval, false),
                    ("D", RequirementStage.Approval, false),
                    ("E", RequirementStage.Onboarding, false),
                    ("F", RequirementStage.Onboarding, false)));

            AssertEx.SequenceIs(result, "E", "F");
        }

        [TestMethod]
        public void TestOnboardedNoneMissing()
        {
            var result = ApprovalCalculations.CalculateMissingIndividualRequirementsFromRequirementCompletion(
                status: RoleApprovalStatus.Onboarded,
                Helpers.IndividualRequirementsMet(
                    ("A", RequirementStage.Application, true),
                    ("B", RequirementStage.Approval, true),
                    ("C", RequirementStage.Approval, true),
                    ("D", RequirementStage.Approval, true),
                    ("E", RequirementStage.Onboarding, true),
                    ("F", RequirementStage.Onboarding, true)));

            AssertEx.SequenceIs(result);
        }

        [TestMethod]
        public void TestOnboardedAllMissing()
        {
            var result = ApprovalCalculations.CalculateMissingIndividualRequirementsFromRequirementCompletion(
                status: RoleApprovalStatus.Onboarded,
                Helpers.IndividualRequirementsMet(
                    ("A", RequirementStage.Application, false),
                    ("B", RequirementStage.Approval, false),
                    ("C", RequirementStage.Approval, false),
                    ("D", RequirementStage.Approval, false),
                    ("E", RequirementStage.Onboarding, false),
                    ("F", RequirementStage.Onboarding, false)));

            AssertEx.SequenceIs(result);
        }
    }
}