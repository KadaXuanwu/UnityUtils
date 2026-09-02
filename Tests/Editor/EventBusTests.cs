using System.Collections.Generic;
using KadaXuanwu.Utils.Runtime.EventBus;
using NUnit.Framework;

namespace KadaXuanwu.Utils.Tests {
    /// <summary>
    /// Tests for the event bus.
    ///
    /// These event types are declared inside this test assembly, which matters: until 1.1.7 the
    /// bus found event types by asking PredefinedAssemblyUtil, which by design only looks in
    /// Assembly-CSharp and Assembly-CSharp-firstpass. Anything declared behind an asmdef - which
    /// is every type in a package, and most types in a real project - was invisible, so its
    /// listeners were never cleared and survived play mode still referencing destroyed objects.
    /// A test living in an asmdef is therefore the only kind that would have caught it.
    /// </summary>
    public class EventBusTests {
        private struct ProbeEvent : IEvent {
            public int Value;
        }

        private struct OtherProbeEvent : IEvent {
        }

        [SetUp]
        public void SetUp() {
            // Buses are static, so a test must not inherit listeners from the one before it.
            EventBusUtil.ClearAllBuses();
        }

        [TearDown]
        public void TearDown() {
            EventBusUtil.ClearAllBuses();
        }

        [Test]
        public void RaiseInvokesBothTheParameterizedAndParameterlessCallbacks() {
            var received = new List<int>();
            var noArgsCount = 0;

            var binding = new EventBinding<ProbeEvent>(e => received.Add(e.Value));
            binding.Add(() => noArgsCount++);
            EventBus<ProbeEvent>.Register(binding);

            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 7 });

            Assert.That(received, Is.EqualTo(new[] { 7 }));
            Assert.That(noArgsCount, Is.EqualTo(1));
        }

        [Test]
        public void UnregisterStopsDelivery() {
            var received = new List<int>();
            var binding = new EventBinding<ProbeEvent>(e => received.Add(e.Value));

            EventBus<ProbeEvent>.Register(binding);
            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 1 });
            EventBus<ProbeEvent>.Unregister(binding);
            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 2 });

            Assert.That(received, Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void RaisingWithNoBindingsDoesNothing() {
            Assert.DoesNotThrow(() => EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 1 }));
        }

        [Test]
        public void ClearRemovesEveryBindingFromThatBus() {
            var received = new List<int>();
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(e => received.Add(e.Value)));
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(e => received.Add(e.Value * 10)));

            EventBus<ProbeEvent>.Clear();
            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 1 });

            Assert.That(received, Is.Empty);
        }

        [Test]
        public void ClearAllBusesReachesABusDeclaredInsideAnAsmdef() {
            var received = new List<int>();
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(e => received.Add(e.Value)));

            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 1 });
            Assert.That(received, Is.EqualTo(new[] { 1 }), "the binding should receive before clearing");

            EventBusUtil.ClearAllBuses();
            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 2 });

            Assert.That(received, Is.EqualTo(new[] { 1 }), "the binding survived ClearAllBuses");
        }

        [Test]
        public void EachEventTypeGetsItsOwnBus() {
            var probeCount = 0;
            var otherCount = 0;
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(() => probeCount++));
            EventBus<OtherProbeEvent>.Register(new EventBinding<OtherProbeEvent>(() => otherCount++));

            EventBus<ProbeEvent>.Clear();
            EventBus<ProbeEvent>.Raise(new ProbeEvent());
            EventBus<OtherProbeEvent>.Raise(new OtherProbeEvent());

            Assert.That(probeCount, Is.EqualTo(0), "clearing one bus must not touch another");
            Assert.That(otherCount, Is.EqualTo(1));
        }

        [Test]
        public void ABusRegistersItselfWithTheUtilityExactlyOnce() {
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(() => { }));
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(() => { }));

            var occurrences = 0;
            for (var i = 0; i < EventBusUtil.EventTypes.Count; i++) {
                if (EventBusUtil.EventTypes[i] == typeof(ProbeEvent)) {
                    occurrences++;
                }
            }

            Assert.That(occurrences, Is.EqualTo(1), "registration happens in the static constructor, so once per closed type");
            Assert.That(EventBusUtil.EventBusTypes, Contains.Item(typeof(EventBus<ProbeEvent>)));
            Assert.That(EventBusUtil.EventTypes.Count, Is.EqualTo(EventBusUtil.EventBusTypes.Count));
        }

        [Test]
        public void UnregisteringAnotherBindingDuringARaiseTakesEffectImmediately() {
            // The shape that matters: a handler tears something down, that teardown unregisters,
            // and the torn-down binding must not then be invoked on a dead object.
            //
            // Each binding unregisters the other, so the assertion holds whichever one the bus
            // happens to reach first - HashSet iteration order is not a contract to lean on.
            var fired = 0;
            EventBinding<ProbeEvent> first = null;
            EventBinding<ProbeEvent> second = null;
            first = new EventBinding<ProbeEvent>(() => {
                fired++;
                EventBus<ProbeEvent>.Unregister(second);
            });
            second = new EventBinding<ProbeEvent>(() => {
                fired++;
                EventBus<ProbeEvent>.Unregister(first);
            });

            EventBus<ProbeEvent>.Register(first);
            EventBus<ProbeEvent>.Register(second);

            EventBus<ProbeEvent>.Raise(new ProbeEvent());

            Assert.That(fired, Is.EqualTo(1), "whichever ran first unregistered the other, which must then be skipped");
        }

        [Test]
        public void RegisteringDuringARaiseTakesEffectOnTheNextRaise() {
            var lateCount = 0;
            var added = false;
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(() => {
                if (added) {
                    return;
                }

                added = true;
                EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(() => lateCount++));
            }));

            EventBus<ProbeEvent>.Raise(new ProbeEvent());
            Assert.That(lateCount, Is.EqualTo(0), "a binding added mid-raise must not receive that same event");

            EventBus<ProbeEvent>.Raise(new ProbeEvent());
            Assert.That(lateCount, Is.EqualTo(1));
        }

        [Test]
        public void ANestedRaiseOfTheSameEventTypeIsDelivered() {
            // Each raise rents its own snapshot buffer, so re-entrancy needs no special case.
            var depth = 0;
            var maxDepth = 0;
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(e => {
                depth++;
                maxDepth = depth > maxDepth ? depth : maxDepth;
                if (e.Value > 0) {
                    EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = e.Value - 1 });
                }

                depth--;
            }));

            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 3 });

            Assert.That(maxDepth, Is.EqualTo(4), "nested raises should each be delivered");
            Assert.That(depth, Is.EqualTo(0));
        }

        [Test]
        public void AThrowingHandlerDoesNotBreakLaterRaises() {
            // The snapshot buffer is returned to the pool in a finally; if it were not, a throwing
            // handler would leak it and later raises would quietly misbehave.
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(() => { throw new System.InvalidOperationException("boom"); }));

            Assert.Throws<System.InvalidOperationException>(() => EventBus<ProbeEvent>.Raise(new ProbeEvent()));

            EventBus<ProbeEvent>.Clear();
            var received = new List<int>();
            EventBus<ProbeEvent>.Register(new EventBinding<ProbeEvent>(e => received.Add(e.Value)));
            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 5 });

            Assert.That(received, Is.EqualTo(new[] { 5 }));
        }

        [Test]
        public void AHandlerMayUnregisterItselfDuringARaise() {
            // Raise iterates a snapshot for exactly this reason: a listener that tears itself down
            // in response to an event is ordinary, and must not invalidate the iteration.
            var received = new List<int>();
            EventBinding<ProbeEvent> binding = null;
            binding = new EventBinding<ProbeEvent>(e => {
                received.Add(e.Value);
                EventBus<ProbeEvent>.Unregister(binding);
            });

            EventBus<ProbeEvent>.Register(binding);

            Assert.DoesNotThrow(() => EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 1 }));
            EventBus<ProbeEvent>.Raise(new ProbeEvent { Value = 2 });

            Assert.That(received, Is.EqualTo(new[] { 1 }));
        }
    }
}
