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

#endregion Namespace Declarations

namespace SharpInputSystem
{
    /// <summary>
    /// Force Feedback is a relatively complex set of properties to upload to a device.
    /// The best place for information on the different properties, effects, etc is in
    /// the DX Documentation and MSDN - there are even pretty graphs ther =)
    /// As this class is modeled on the the DX interface you can apply that same
    /// knowledge to creating effects via this class on any OS supported by OIS.
    /// In anycase, this is the main class you will be using. There is *absolutely* no
    /// need to instance any of the supporting ForceEffect classes yourself.
    /// </summary>
    public class Effect
    {
        #region Enumerations and Constants

        #region EDirection enum

        /// <summary>
        /// Direction of the Force
        /// </summary>
        public enum EDirection
        {
            NorthWest,
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West
        };

        #endregion

        #region EForce enum

        /// <summary>
        /// Type of force
        /// </summary>
        public enum EForce
        {
            UnknownForce = 0,
            ConstantForce,
            RampForce,
            PeriodicForce,
            ConditionalForce,
            CustomForce
        };

        #endregion

        #region EType enum

        /// <summary>
        /// Type of effect
        /// </summary>
        public enum EType
        {
            //Type ----- Pairs with force:
            Unknown = 0, //UnknownForce
            Constant, //ConstantForce
            Ramp, //RampForce
            Square, //PeriodicForce
            Triangle, //PeriodicForce
            Sine, //PeriodicForce
            SawToothUp, //PeriodicForce
            SawToothDown, //PeriodicForce
            Friction, //ConditionalForce
            Damper, //ConditionalForce
            Inertia, //ConditionalForce
            Spring, //ConditionalForce
            Custom //CustomForce
        };

        #endregion

        /// <summary>
        /// Infinite Time
        /// </summary>
        public const uint INFINITE_TIME = 0xFFFFFFFF;

        #endregion Enumerations and Constants

        #region Fields and Properties

        #region Handle Property

        private int _handle;

        public int Handle
        {
            get { return this._handle; }
            internal set { this._handle = value; }
        }

        #endregion Handle Property

        #region Force Property

        /// <summary>
        /// Properties depend on EForce
        /// </summary>
        public EForce Force { get; set; }

        #endregion Force Property

        #region Type Property

        /// <summary>
        /// 
        /// </summary>
        public EType Type { get; set; }

        #endregion Type Property

        #region Direction Property

        /// <summary>
        /// Direction to apply to the force - affects two axes+ effects
        /// </summary>
        public EDirection Direction { get; set; }

        #endregion Direction Property

        #region TrigerButton Property

        /// <summary>
        /// Number of button triggering an effect (-1 means no trigger)
        /// </summary>
        public int TriggerButton { get; set; }

        #endregion TriggerButton Property

        #region TriggerInterval Property

        /// <summary>
        /// Time to wait before an effect can be re-triggered (microseconds)
        /// </summary>
        public uint TriggerInterval { get; set; }

        #endregion TriggerInterval Property

        #region ReplayLength Property

        /// <summary>
        /// Duration of an effect (microseconds)
        /// </summary>
        public uint ReplayLength { get; set; }

        #endregion ReplayLength Property

        #region ReplayDelay Property

        /// <summary>
        /// Time to wait before to start playing an effect (microseconds)
        /// </summary>
        public uint ReplayDelay { get; set; }

        #endregion ReplayDelay Property

        #region ForceEffect Property

        /// <summary>
        /// the specific Force Effect
        /// </summary>
        private readonly IForceEffect _forceEffect;

        /// <summary>
        /// Get the specific Force Effect. This should be cast depending on the EForce
        /// </summary>
        public IForceEffect ForceEffect
        {
            get { return this._forceEffect; }
        }

        #endregion ForceEffect Property

        #region AxesCount Property

        /// <summary>
        /// Number of axes to use in effect
        /// </summary>
        private int _axes;

        /// <summary>
        /// Get/Set the number of Axes to use before the initial creation of the effect.
        /// </summary>
        /// <remarks>
        /// Can only be done prior to creation! Use the FF interface to determine
        /// how many axes can be used (are availiable)
        /// </remarks>
        public int AxesCount
        {
            get { return this._axes; }
            set
            {
                //Can only be set before a handle was assigned (effect created)
                if (this._handle != -1)
                    this._axes = value;
            }
        }

        #endregion AxesCount Property

        #endregion Fields and Properties

        #region Constructors

        /// <summary>
        /// hidden so this class cannot be instanced with default constructor
        /// </summary>
        private Effect()
        {
            this._axes = 1;
        }

        /// <summary>
        /// Create and effect with the specified Force and Type
        /// </summary>
        /// <param name="force"></param>
        /// <param name="type"></param>
        public Effect(EForce effectForce, EType effectType)
            : this()
        {
            this.Force = effectForce;
            this.Type = effectType;
            this.Direction = EDirection.North;
            this.TriggerButton = -1;
            this.ReplayLength = INFINITE_TIME;
            this._handle = -1;

            switch (effectForce)
            {
                case EForce.ConstantForce:
                    this._forceEffect = new ConstantEffect();
                    break;
                case EForce.RampForce:
                    this._forceEffect = new RampEffect();
                    break;
                case EForce.PeriodicForce:
                    this._forceEffect = new PeriodicEffect();
                    break;
                case EForce.ConditionalForce:
                    this._forceEffect = new ConditionalEffect();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("effectForce value not supported.");
            }
        }

        #endregion Constructors
    }

    /// <summary>
    /// Base class of all effect property classes
    /// </summary>
    public interface IForceEffect { }

    /// <summary>
    /// An optional envelope to be applied to the start/end of an effect. 
    /// </summary>
    /// <remarks>
    /// If any of these values are nonzero, then the envelope will be used in setting up the
    /// effect. Not currently utilised.. But, will be soon.
    /// </remarks>
    public struct Envelope : IForceEffect
    {
        #region Fields And Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsUsed
        {
            get { return (this._attackLength + this._attackLevel + this._fadeLength + this._fadeLevel != 0); }
        }

        #region AttackLength Property

        /// <summary>
        /// 
        /// </summary>
        private short _attackLength;

        /// <summary>
        /// 
        /// </summary>
        public short AttackLength
        {
            get { return this._attackLength; }
            set { this._attackLength = value; }
        }

        #endregion AttackLength Property

        #region AttackLevel Property

        /// <summary>
        /// 
        /// </summary>
        private short _attackLevel;

        /// <summary>
        /// 
        /// </summary>
        public short AttackLevel
        {
            get { return this._attackLevel; }
            set { this._attackLevel = value; }
        }

        #endregion AttackLevel Property

        #region FadeLength Property

        /// <summary>
        /// 
        /// </summary>
        private short _fadeLength;

        /// <summary>
        /// 
        /// </summary>
        public short FadeLength
        {
            get { return this._fadeLength; }
            set { this._fadeLength = value; }
        }

        #endregion FadeLength Property

        #region FadeLevel Property

        /// <summary>
        /// 
        /// </summary>
        private short _fadeLevel;

        /// <summary>
        /// 
        /// </summary>
        public short FadeLevel
        {
            get { return this._fadeLevel; }
            set { this._fadeLevel = value; }
        }

        #endregion FadeLevel Property

        #endregion Fields and Properties
    }

    /// <summary>
    /// Use this class when dealing with Force type of Constant
    /// </summary>
    public struct ConstantEffect : IForceEffect
    {
        #region Fields and Properties

        #region Envelope Property

        /// <summary>
        /// Optional envolope
        /// </summary>
        public Envelope Envelope { get; set; }

        #endregion Envelope Property

        #region Level Property

        /// <summary>
        /// -10K to +10k
        /// </summary>
        public short Level { get; set; }

        #endregion Level Property

        #endregion Fields And Properties
    }

    /// <summary>
    /// Use this class when dealing with Force type of Ramp
    /// </summary>
    public struct RampEffect : IForceEffect
    {
        #region Fields and Properties

        #region Envelope Property

        /// <summary>
        /// Optional envolope
        /// </summary>
        public Envelope Envelope { get; set; }

        #endregion Envelope Property

        #region StartLevel Property

        /// <summary>
        /// -10K to +10k
        /// </summary>
        public short StartLevel { get; set; }

        #endregion StartLevel Property

        #region EndLevel Property

        /// <summary>
        /// -10K to +10k
        /// </summary>
        public short EndLevel { get; set; }

        #endregion EndLevel Property

        #endregion Fields And Properties
    }

    /// <summary>
    /// Use this class when dealing with Force type of Periodic
    /// </summary>
    public struct PeriodicEffect : IForceEffect
    {
        #region Fields and Properties

        #region Envelope Property

        /// <summary>
        /// Optional envolope
        /// </summary>
        public Envelope Envelope { get; set; }

        #endregion Envelope Property

        #region Magnitude Property

        /// <summary>
        /// 0 to 10,0000
        /// </summary>
        public ushort Magnitude { get; set; }

        #endregion Magnitude Property

        #region Offset Property

        /// <summary>
        /// 
        /// </summary>
        public short Offset { get; set; }

        #endregion Offset Property

        #region Phase Property

        /// <summary>
        /// Position at which playback begins 0 to 35,999
        /// </summary>
        public ushort Phase { get; set; }

        #endregion Phase Property

        #region Period Property

        /// <summary>
        /// Period of effect (microseconds)
        /// </summary>
        public ushort Period { get; set; }

        #endregion Period Property

        #endregion Fields And Properties
    }

    /// <summary>
    /// Use this class when dealing with Force type of Conditional
    /// </summary>
    public struct ConditionalEffect : IForceEffect
    {
        #region Fields and Properties

        #region RightCoefficient Property

        /// <summary>
        /// -10k to +10k (Positive Coeff)
        /// </summary>
        public short RightCoefficient { get; set; }

        #endregion RightCoefficient Property

        #region LeftCoefficient Property

        /// <summary>
        /// -10k to +10k (Negative Coefficient)
        /// </summary>
        public short LeftCoefficient { get; set; }

        #endregion LeftCoefficient Property

        #region RightSaturation Property

        /// <summary>
        /// 0 to 10,0000 (Positive Saturation)
        /// </summary>
        public ushort RightSaturation { get; set; }

        #endregion RightSaturation Property

        #region LeftSaturation Property

        /// <summary>
        /// 0 to 10,0000 (Negative Saturation)
        /// </summary>
        public ushort LeftSaturation { get; set; }

        #endregion LeftSaturation Property

        #region DeadBand Property

        /// <summary>
        /// Region around center in which the condition is not active
        /// </summary>
        /// <remarks>
        /// has a range of 0 to 10K
        /// </remarks>
        public ushort DeadBand { get; set; }

        #endregion DeadBand Property

        #region Center Property

        /// <summary>
        /// -10k to +10k 
        /// </summary>
        /// <remarks>(Offset in DX)</remarks>
        public short Center { get; set; }

        #endregion Center Property

        #endregion Fields And Properties
    }
}
