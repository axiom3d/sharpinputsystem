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
    /// 
    /// </summary>
    public enum InputType
    {
        Unknown,
        Keyboard,
        Mouse,
        Joystick,
        Tablet
    }

    public interface IInputObjectInterface { }

    public abstract class InputObjectEventArgs
    {
        public InputObjectEventArgs(InputObject obj)
        {
            this.Device = obj;
        }

        public InputObject Device { get; protected set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class InputObject : IDisposable
    {
        /// <summary>
        /// Get the type of device
        /// </summary>
        public InputType Type { get; protected set; }

        /// <summary>
        /// Get the vender string name
        /// </summary>
        public String Vendor { get; protected set; }

        /// <summary>
        /// Returns this input object's creator
        /// </summary>
        public InputManager Creator { get; protected set; }

        /// <summary>
        /// Get buffered mode - true is buffered, false otherwise
        /// </summary>
        public virtual bool IsBuffered { get; protected set; }

        /// <summary>
        /// Not fully implemented yet
        /// </summary>
        public string DeviceID { get; protected set; }

        /// <summary>
        /// Used for updating call once per frame before checking state or to update events
        /// </summary>
        public abstract void Capture();

        /// <summary>
        /// 
        /// </summary>
        protected internal abstract void Initialize();

        public virtual IInputObjectInterface QueryInterface<T>() where T : IInputObjectInterface
        {
            return default(T);
        }

        #region IDisposable Implementation

        protected bool IsDisposed { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!this.IsDisposed)
            {
                if (disposeManagedResources)
                {
                    // Dispose managed resources.
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            this.IsDisposed = true;

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            //base.Dispose( disposeManagedResources );
        }

        ~InputObject()
        {
            Dispose(false);
        }

        #endregion IDisposable Implementation
    }
}
