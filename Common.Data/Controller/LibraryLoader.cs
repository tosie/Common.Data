using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Common.Data {
    public static class LibraryLoader {

        /// <summary>
        /// Loads the specified module into the address space of the calling process. The specified module may cause other modules to be loaded.
        /// </summary>
        /// <param name="librayName">
        /// The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file).
        /// The name specified is the file name of the module and is not related to the name stored in the library module itself,
        /// as specified by the LIBRARY keyword in the module-definition (.def) file.
        /// 
        /// If the string specifies a full path, the function searches only that path for the module.
        /// 
        /// If the string specifies a relative path or a module name without a path, the function uses a standard search strategy
        /// to find the module; for more information, see the Remarks.
        /// 
        /// If the function cannot find the module, the function fails. When specifying a path, be sure to use backslashes (\), not
        /// forward slashes (/). For more information about paths, see Naming a File or Directory.
        /// 
        /// If the string specifies a module name without a path and the file name extension is omitted, the function appends the default
        /// library extension .dll to the module name. To prevent the function from appending .dll to the module name, include a trailing
        /// point character (.) in the module name string.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the module.
        /// 
        /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("kernel32")]
        public extern static int LoadLibrary(string librayName);

        /// <summary>
        /// This method tries to preload a library file. It does so by looking in <paramref name="libDir"/> first and using the default search paths afterwards.
        /// </summary>
        /// <param name="libDir">The full name of the directory containing libraries (may contain environment variables as well as %appdir%, which will be replaced by the directory of the applications executable path, and %platform%, which will be replaced by "x86" or "x64" depending on the current platform the process is running on).</param>
        /// <param name="lib">The file name (without path) of the library to load.</param>
        /// <returns>True, if the SQLite library was loaded, false otherwise.</returns>
        public static bool TryLoad(string libDir, string lib) {
            var debug_category = "LibraryLoader";

            var appdir = Path.GetDirectoryName(Application.ExecutablePath);
            var platform = (IntPtr.Size == 4 ? "x86" : "x64");

            // String Replacements
            // - %appdir% and %platform% are special
            libDir = libDir.Replace("%appdir%", appdir);
            libDir = libDir.Replace("%platform%", platform);
            // - all other environment variables are simple
            libDir = Environment.ExpandEnvironmentVariables(libDir);

            var handle = 0;

            // Look in "%appdir%\Libs\%platform%
            if (Directory.Exists(libDir)) {
                handle = LoadLibrary(Path.Combine(libDir, lib));
                if (handle > 0) {
                    Debug.WriteLine(String.Format("Loaded the library \"{0}\" from \"{1}\".", lib, libDir), debug_category);
                    return true;
                }
            } else {
                Debug.WriteLine(String.Format("The given directory \"{1}\" to load the library \"{1}\" from does not exist.", lib, libDir), debug_category);
            }

            // Look in "%path%"
            handle = LoadLibrary(lib);
            if (handle > 0) {
                Debug.WriteLine(String.Format("Loaded the library \"{0}\" from the search paths.", lib), debug_category);
                return true;
            }

            Debug.WriteLine(String.Format("Could not load the library \"{0}\". It was neither found in the library directory (\"{1}\") nor in the search path.", lib, libDir), debug_category);
            return false;
        }

    }
}
