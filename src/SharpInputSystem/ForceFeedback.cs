#region MIT/X11 License

/*
Sharp Input System Library
Copyright © 2007-2019 Michael Cummings

The overall design, and a majority of the core code contained within 
this library is a derivative of the open source Open Input System ( OIS ) , 
which can be found at http://www.sourceforge.net/projects/wgois.  
Many thanks to Phillip Castaneda for maintaining such a high quality project.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

*/

#endregion MIT/X11 License

#region Namespace Declarations

using System;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace SharpInputSystem
{
    /// <summary>
    /// Interface class for dealing with Force Feedback devices
    /// </summary>
    public abstract class ForceFeedback : IInputObjectInterface
    {
        #region Fields and Properties

        /// <summary>
        /// a list of all supported effects
        /// </summary>
        private readonly EffectsList _supportedEffects = new EffectsList();

        /// <summary>
        /// This is like setting the master volume of an audio device.
        /// Individual effects have gain levels; however, this affects all
        /// effects at once.
        /// </summary>
        /// <remarks>
        /// A value between 0.0 and 1.0 represent the percentage of gain. 1.0
        /// being the highest possible force level (means no scaling).
        /// </remarks>
        public abstract float MasterGain { set; }

        /// <summary>
        /// If using Force Feedback effects, this should be turned off
        /// before uploading any effects. Auto centering is the motor moving
        /// the joystick back to center. DirectInput only has an on/off setting,
        /// whereas linux has levels.. Though, we go with DI's on/off mode only
        /// </summary>
        /// <remarks>
        /// true to turn auto centering on, false to turn off.
        /// </remarks>
        public abstract bool AutoCenterMode { set; }

        /// <summary>
        /// Get the number of supported Axes for ForceFeedback usage
        /// </summary>
        public abstract int SupportedAxesCount { get; }

        /// <summary>
        /// Get a list of all supported effects
        /// </summary>
        public EffectsList SupportedEffects
        {
            get { return this._supportedEffects; }
        }

        #endregion Fields and Properties

        #region Methods

        /// <summary>
        /// Creates and Plays the effect immediately. If the device is full
        /// of effects, it will fail to be uploaded. You will know this by
        /// an invalid Effect Handle
        /// </summary>
        /// <param name="effect"></param>
        public abstract void Upload(Effect effect);

        /// <summary>
        /// Modifies an effect that is currently playing
        /// </summary>
        /// <param name="effect"></param>
        public abstract void Modify(Effect effect);

        /// <summary>
        /// Remove the effect from the device
        /// </summary>
        /// <param name="effect"></param>
        public abstract void Remove(Effect effect);

        public void AddEffectType(Effect.EForce force, Effect.EType type)
        {
            if (force == Effect.EForce.UnknownForce || type == Effect.EType.Unknown)
                throw new ArgumentException("Added Unknown force|type.");
            this._supportedEffects.Add(force, type);
        }

        #endregion Methods
    }

    public sealed class EffectsList : Dictionary<Effect.EForce, Effect.EType> { }
}
