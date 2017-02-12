using System;
using App.Metrics;
using App.Metrics.Abstractions.ReservoirSampling;
using App.Metrics.Core.Options;
using App.Metrics.ReservoirSampling.ExponentialDecay;
using App.Metrics.Tagging;

namespace Api.Sample
{
    public static class MetricsRegistry
    {
        public static class ApdexScores
        {
            public static ApdexOptions TestApdex { get; } = new ApdexOptions
                                                            {
                                                                Name = "Test Apdex"
                                                            };
        }

        public static class Contexts
        {
            public static class TestContext
            {
                public static readonly string TestContextName = "Test Context";

                public static class Counters
                {
                    public static CounterOptions TestCounter { get; } = new CounterOptions
                                                                        {
                                                                            Name = "Test Counter",
                                                                            MeasurementUnit = Unit.Calls
                                                                        };

                    public static CounterOptions TestCounterWithItem { get; } = new CounterOptions
                                                                                {
                                                                                    Name = "Test Counter With Item",
                                                                                    MeasurementUnit = Unit.Calls
                                                                                };
                }

                public static class Gauges
                {
                    public static GaugeOptions TestGauge { get; } = new GaugeOptions
                                                                    {
                                                                        Name = "Test Gauge",
                                                                        MeasurementUnit = Unit.Items
                                                                    };
                }

                public static class Histograms
                {
                    public static HistogramOptions TestHistogram { get; } = new HistogramOptions
                                                                            {
                                                                                Name = "Test Histogram",
                                                                                MeasurementUnit = Unit.MegaBytes
                                                                            };

                    public static HistogramOptions TestHistogramWithUserValue { get; } = new HistogramOptions
                                                                                         {
                                                                                             Name = "Test Histogram With User Value",
                                                                                             MeasurementUnit = Unit.Bytes
                                                                                         };
                }

                public static class Meters
                {
                    public static MeterOptions TestMeter { get; } = new MeterOptions
                                                                    {
                                                                        Name = "Test Meter",
                                                                        MeasurementUnit = Unit.Calls
                                                                    };
                }

                public static class Timers
                {
                    public static TimerOptions TestTimer { get; } = new TimerOptions
                                                                    {
                                                                        Name = "Test Timer",
                                                                        MeasurementUnit = Unit.Items,
                                                                        DurationUnit = TimeUnit.Milliseconds,
                                                                        RateUnit = TimeUnit.Milliseconds,
                                                                        Reservoir = new Lazy<IReservoir>(() => new DefaultForwardDecayingReservoir())
                                                                    };

                    public static TimerOptions TestTimerWithUserValue { get; } = new TimerOptions
                                                                                 {
                                                                                     MeasurementUnit = Unit.Items,
                                                                                     DurationUnit = TimeUnit.Milliseconds,
                                                                                     RateUnit = TimeUnit.Milliseconds,
                                                                                     Reservoir =
                                                                                         new Lazy<IReservoir>(
                                                                                             () => new DefaultForwardDecayingReservoir())
                                                                                 };
                }
            }

            public static class TestContextTwo
            {
                public static readonly string TestContextName = "Test Context Two";

                public static class Counters
                {
                    public static CounterOptions TestCounter { get; } = new CounterOptions
                                                                        {
                                                                            Name = "Test Counter",
                                                                            MeasurementUnit = Unit.Calls
                                                                        };

                    public static CounterOptions TestCounterWithItem { get; } = new CounterOptions
                                                                                {
                                                                                    Name = "Test Counter With Item",
                                                                                    MeasurementUnit = Unit.Calls
                                                                                };
                }

                public static class Gauges
                {
                    public static GaugeOptions TestGauge { get; } = new GaugeOptions
                                                                    {
                                                                        Name = "Test Gauge",
                                                                        MeasurementUnit = Unit.Items
                                                                    };
                }

                public static class Histograms
                {
                    public static HistogramOptions TestHistogram { get; } = new HistogramOptions
                                                                            {
                                                                                Name = "Test Histogram",
                                                                                MeasurementUnit = Unit.MegaBytes
                                                                            };

                    public static HistogramOptions TestHistogramWithUserValue { get; } = new HistogramOptions
                                                                                         {
                                                                                             Name = "Test Histogram With User Value",
                                                                                             MeasurementUnit = Unit.Bytes
                                                                                         };
                }

                public static class Meters
                {
                    public static MeterOptions TestMeter { get; } = new MeterOptions
                                                                    {
                                                                        Name = "Test Meter",
                                                                        MeasurementUnit = Unit.Calls
                                                                    };
                }

                public static class Timers
                {
                    public static TimerOptions TestTimer { get; } = new TimerOptions
                                                                    {
                                                                        Name = "Test Timer",
                                                                        MeasurementUnit = Unit.Items,
                                                                        DurationUnit = TimeUnit.Milliseconds,
                                                                        RateUnit = TimeUnit.Milliseconds
                                                                    };

                    public static TimerOptions TestTimerWithUserValue { get; } = new TimerOptions
                                                                                 {
                                                                                     Name = "Test Timer With User Value",
                                                                                     MeasurementUnit = Unit.Items,
                                                                                     DurationUnit = TimeUnit.Milliseconds,
                                                                                     RateUnit = TimeUnit.Milliseconds
                                                                                 };
                }
            }
        }

        public static class Counters
        {
            public static CounterOptions TestCounter { get; } = new CounterOptions
                                                                {
                                                                    Name = "Test Counter",
                                                                    MeasurementUnit = Unit.Calls
                                                                };

            public static CounterOptions TestCounterWithItem { get; } = new CounterOptions
                                                                        {
                                                                            Name = "Test Counter With Item",
                                                                            MeasurementUnit = Unit.Calls
                                                                        };
        }

        public static class Gauges
        {
            public static GaugeOptions CacheHitRatioGauge { get; } = new GaugeOptions
                                                                     {
                                                                         Name = "Cache Gauge",
                                                                         MeasurementUnit = Unit.Calls
                                                                     };

            public static GaugeOptions DerivedGauge { get; } = new GaugeOptions
                                                               {
                                                                   Name = "Derived Gauge",
                                                                   MeasurementUnit = Unit.MegaBytes
                                                               };

            public static GaugeOptions TestGauge { get; } = new GaugeOptions
                                                            {
                                                                Name = "Test Gauge",
                                                                MeasurementUnit = Unit.Bytes
                                                            };
        }

        public static class Histograms
        {
            public static HistogramOptions TestHAdvancedistogram { get; } = new HistogramOptions
                                                                            {
                                                                                Name = "Test Advanced Histogram",
                                                                                MeasurementUnit = Unit.MegaBytes
                                                                            };

            public static HistogramOptions TestHistogram { get; } = new HistogramOptions
                                                                    {
                                                                        Name = "Test Histogram",
                                                                        MeasurementUnit = Unit.MegaBytes
                                                                    };

            public static HistogramOptions TestHistogramWithUserValue { get; } = new HistogramOptions
                                                                                 {
                                                                                     Name = "Test Histogram With User Value",
                                                                                     MeasurementUnit = Unit.Bytes
                                                                                 };
        }

        public static class Meters
        {
            public static MeterOptions CacheHits { get; } = new MeterOptions
                                                            {
                                                                Name = "Cache Hits Meter",
                                                                MeasurementUnit = Unit.Calls
                                                            };
        }

        public static class Timers
        {
            public static TimerOptions DatabaseQueryTimer { get; } = new TimerOptions
                                                                     {
                                                                         Name = "Database Query Timer",
                                                                         MeasurementUnit = Unit.Calls,
                                                                         DurationUnit = TimeUnit.Milliseconds,
                                                                         RateUnit = TimeUnit.Milliseconds
                                                                     };

            public static TimerOptions TestTimer { get; } = new TimerOptions
                                                            {
                                                                Name = "Test Timer",
                                                                MeasurementUnit = Unit.Items,
                                                                DurationUnit = TimeUnit.Milliseconds,
                                                                RateUnit = TimeUnit.Milliseconds
                                                            };

            public static TimerOptions TestTimerTwo { get; } = new TimerOptions
                                                               {
                                                                   Name = "Test Timer 2",
                                                                   MeasurementUnit = Unit.Items,
                                                                   DurationUnit = TimeUnit.Milliseconds,
                                                                   RateUnit = TimeUnit.Milliseconds
                                                               };

            public static TimerOptions TestTimerTwoWithUserValue { get; } = new TimerOptions
                                                                            {
                                                                                Name = "Test Timer 2 With User Value",
                                                                                MeasurementUnit = Unit.Items,
                                                                                DurationUnit = TimeUnit.Milliseconds,
                                                                                RateUnit = TimeUnit.Milliseconds
                                                                            };

            public static TimerOptions TestTimerWithUserValue { get; } = new TimerOptions
                                                                         {
                                                                             Name = "Test Timer With User Value",
                                                                             MeasurementUnit = Unit.Items,
                                                                             DurationUnit = TimeUnit.Milliseconds,
                                                                             RateUnit = TimeUnit.Milliseconds
                                                                         };
        }
    }
}