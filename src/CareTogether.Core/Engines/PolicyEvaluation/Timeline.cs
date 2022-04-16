﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareTogether.Engines.PolicyEvaluation
{
    internal sealed record TerminatingTimelineStage(DateTime Start, DateTime End)
    {
        public TimeSpan Duration => End - Start;
    }
    internal sealed record NonTerminatingTimelineStage(DateTime Start);

    internal sealed record MappedTimeSpan(DateTime Start, DateTime End);

    /// <summary>
    /// The <see cref="Timeline"/> class simplifies temporal calculations by
    /// mapping high-level concepts like durations and intervals onto
    /// potentially-discontinuous underlying time segments.
    /// </summary>
    internal sealed class Timeline
    {
        private readonly ImmutableList<TerminatingTimelineStage> terminatingStages;
        private readonly NonTerminatingTimelineStage? nonTerminatingStage;


        public Timeline(ImmutableList<TerminatingTimelineStage> terminatingStages)
        {
            this.terminatingStages = terminatingStages;
            this.nonTerminatingStage = null;
        }

        public Timeline(ImmutableList<TerminatingTimelineStage> terminatingStages,
            NonTerminatingTimelineStage nonTerminatingStage)
        {
            this.terminatingStages = terminatingStages;
            this.nonTerminatingStage = nonTerminatingStage;
        }


        public MappedTimeSpan Map(TimeSpan startDelay, TimeSpan duration)
        {
            var start = Map(startDelay);
            var end = Map(startDelay + duration);
            
            return new MappedTimeSpan(start, end);
        }


        public DateTime Map(TimeSpan durationFromStart)
        {
            DateTime? mappedStageStartDate = null;
            TimeSpan mappedDurationPriorToCurrentStage = TimeSpan.Zero;

            foreach (var stage in terminatingStages)
            {
                if (stage.Duration + mappedDurationPriorToCurrentStage >= durationFromStart)
                {
                    mappedStageStartDate = stage.Start;
                    break;
                }
                else
                    mappedDurationPriorToCurrentStage += stage.Duration;
            }

            if (mappedStageStartDate == null)
            {
                if (nonTerminatingStage != null)
                    mappedStageStartDate = nonTerminatingStage.Start;
                else
                    throw new InvalidOperationException(
                        "The timeline is not long enough to accommodate mapping the requeste date.");
            }

            return (DateTime)(mappedStageStartDate!) +
                durationFromStart - mappedDurationPriorToCurrentStage;
        }
    }
}
