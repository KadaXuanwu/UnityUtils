using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.FPController.Core {
    public static partial class CharacterRefs {
        public static Camera Camera { get; set; }
        public static Transform CameraHolder { get; set; }
        public static FirstPersonController Controller { get; set; }
        public static CharacterController CharacterController { get; set; }
    }
}