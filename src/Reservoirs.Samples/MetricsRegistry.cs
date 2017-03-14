using System;
using App.Metrics.Abstractions.ReservoirSampling;
using App.Metrics.Core.Options;
using App.Metrics.ReservoirSampling.ExponentialDecay;
using App.Metrics.ReservoirSampling.SlidingWindow;
using App.Metrics.ReservoirSampling.Uniform;

namespace Reservoirs.Samples
{
    public static class MetricsRegistry
    {
        public static readonly string Context = "Reservoirs";

        public static TimerOptions TimerUsingAlgorithmRReservoir = new TimerOptions
                                                                   {
                                                                       Context = Context,
                                                                       Name = "uniform",
                                                                       Reservoir = new Lazy<IReservoir>(
                                                                           () => new DefaultAlgorithmRReservoir())
                                                                   };

        public static TimerOptions TimerUsingExponentialForwardDecayingReservoir = new TimerOptions
                                                                                   {
                                                                                       Context = Context,
                                                                                       Name = "exponentially-decaying",
                                                                                       Reservoir =
                                                                                           new Lazy<IReservoir>(
                                                                                               () => new DefaultForwardDecayingReservoir())
                                                                                   };

        //public static TimerOptions TimerUsingHdrHistogramReservoir = new TimerOptions
        //                                                             {
        //                                                                 Context = Context,
        //                                                                 Name = "high-dynamic-range",
        //                                                                 Reservoir = new Lazy<IReservoir>(
        //                                                                     () => new HdrHistogramReservoir())
        //                                                             };

        public static TimerOptions TimerUsingSlidingWindowReservoir = new TimerOptions
                                                                      {
                                                                          Context = Context,
                                                                          Name = "sliding-window",
                                                                          Reservoir = new Lazy<IReservoir>(
                                                                              () => new DefaultSlidingWindowReservoir())
                                                                      };
    }
}