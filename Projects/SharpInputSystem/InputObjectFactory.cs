#region MIT/X11 License

/*
Sharp Input System Library
Copyright © 2007-2011 Michael Cummings

The overall design, and a majority of the core code contained within 
this library is a derivative of the open source Open Input System ( OIS ) , 
which can be found at http://www.sourceforge.net/projects/wgois.  
Many thanks to the Phillip Castaneda for maintaining such a high quality project.

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

using System;
using System.Collections.Generic;

namespace SharpInputSystem
{
    /// <summary>
    /// Interface for creating devices - all devices ultimately get enumerated/created via a factory.
    /// A factory can create multiple types of objects.
    /// </summary>
    public interface InputObjectFactory
    {
        /// <summary>
        /// Return a list of all unused devices the factory maintains.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<Type, string>> FreeDevices { get; }

        /// <summary>
        /// Number of total devices of requested type
        /// </summary>
        /// <typeparam name="T">Type of devices to check</typeparam>
        /// <returns></returns>
        int DeviceCount<T>( ) where T : InputObject;

        /// <summary>
        /// Number of free devices of requested type
        /// </summary>
        /// <typeparam name="T">Type of devices to check</typeparam>
        /// <returns></returns>
        int FreeDeviceCount<T>( ) where T : InputObject;

        /// <summary>
        /// Does a Type exist with the given vendor name
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <param name="vendor">Vendor name to test</param>
        /// <returns></returns>
        bool VendorExists<T>( string vendor ) where T : InputObject;

        /// <summary>
        /// Creates the InputObject
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        /// <param name="creator"></param>
        /// <param name="bufferMode">True to setup for buffered events</param>
        /// <param name="vendor">Create a device with the vendor name, "" means vendor name is unimportant</param>
        /// <returns></returns>
        InputObject CreateInputObject<T>( InputManager creator, bool bufferMode, string vendor ) where T : InputObject;

        /// <summary>
        /// Destroys an InputObject
        /// </summary>
        /// <param name="obj">the InputObject to destroy</param>
        void DestroyInputObject( InputObject obj );
    }
}
