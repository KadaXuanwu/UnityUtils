using System.Collections.Generic;
using KadaXuanwu.Utils.Runtime.FPController.Core;
using KadaXuanwu.Utils.Runtime.FPController.Modifiers.Crouch;
using NUnit.Framework;
using UnityEngine;

namespace KadaXuanwu.Utils.Tests {
    /// <summary>
    /// Tests for the capsule's stand-up check. Until 1.3.4 it was one ray up from the centre, so a ceiling
    /// edge over the side of the capsule let a crouching body stand up into it.
    /// </summary>
    public class CrouchHeadroomTests {
        private const float Radius = 0.35f;
        // Far from anything in whatever scene is open.
        private static readonly Vector3 Feet = new Vector3(10000f, 10000f, 10000f);

        private readonly List<GameObject> _objects = new List<GameObject>();
        private CrouchConfig _config;
        private CharacterController _self;

        [SetUp]
        public void SetUp() {
            _config = ScriptableObject.CreateInstance<CrouchConfig>();
            _config.StandingHeight = 1.8f;
            _config.StandingCenterY = 0.9f;
            _config.CrouchingHeight = 1.4f;
            _config.CrouchingCenterY = 0.7f;

            GameObject body = Track(new GameObject("Body"));
            body.transform.position = Feet;
            _self = body.AddComponent<CharacterController>();
            _self.radius = Radius;
            _self.height = _config.CrouchingHeight;
            _self.center = new Vector3(0f, _config.CrouchingCenterY, 0f);
        }

        [TearDown]
        public void TearDown() {
            foreach (GameObject obj in _objects) {
                Object.DestroyImmediate(obj);
            }
            _objects.Clear();
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void OpenSkyHasHeadroomAndTheBodyItselfDoesNotCount() {
            Assert.That(HasHeadroom(), Is.True);
        }

        [Test]
        public void ACeilingOverTheCentreBlocks() {
            Box(new Vector3(0f, 1.6f, 0f));
            Assert.That(HasHeadroom(), Is.False);
        }

        [Test]
        public void ACeilingEdgeOverTheSideBlocks() {
            // Spans x 0.2 to 0.4: inside the capsule's 0.35 radius, clear of a ray up the middle.
            Box(new Vector3(0.3f, 1.6f, 0f));
            Assert.That(HasHeadroom(), Is.False);
        }

        [Test]
        public void AWallBesideTheHeadDoesNotBlock() {
            // Spans x 0.4 to 0.6: just outside the capsule.
            Box(new Vector3(0.5f, 1.6f, 0f));
            Assert.That(HasHeadroom(), Is.True);
        }

        [Test]
        public void SomethingAboveStandingHeightDoesNotBlock() {
            Box(new Vector3(0f, 1.95f, 0f));
            Assert.That(HasHeadroom(), Is.True);
        }

        private bool HasHeadroom() {
            Physics.SyncTransforms();
            return new CharacterControllerMotor(_self).HasHeadroom(_config.StandingHeight, _config.StandingCenterY);
        }

        // A 0.2 m cube at an offset from the feet.
        private void Box(Vector3 offset) {
            GameObject box = Track(GameObject.CreatePrimitive(PrimitiveType.Cube));
            box.transform.position = Feet + offset;
            box.transform.localScale = Vector3.one * 0.2f;
        }

        private GameObject Track(GameObject obj) {
            _objects.Add(obj);
            return obj;
        }
    }
}
