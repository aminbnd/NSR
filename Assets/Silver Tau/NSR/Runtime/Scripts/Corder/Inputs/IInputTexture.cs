namespace SilverTau.NSR.Recorders.Inputs {

    using System;
    using UnityEngine;

    public interface IInputTexture : IDisposable {
        (int width, int height) frameSize { get; }
        void CommitFrame (Texture texture, long timestamp);
    }
}